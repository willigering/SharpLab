using System.Net;
using System.Text;
using System.Text.Json;
using CSharpLernen.Engine;
using Xunit;

namespace CSharpLernen.Tests;

/// <summary>Feedback-Knopf (1.0.5): Prüfen, JSON/Anfrage, Senden mit Fehlerfällen, mailto-Ausweichweg, Protokoll-Auszug.</summary>
public class FeedbackTests : IDisposable
{
    readonly string tmp = Path.Combine(Path.GetTempPath(), "sl-fb-test-" + Guid.NewGuid().ToString("N"));
    public FeedbackTests() => Directory.CreateDirectory(tmp);
    public void Dispose() { try { Directory.Delete(tmp, true); } catch { } }

    static FeedbackDaten D(string betreff = "Idee", string nachricht = "Mehr Übungen bitte!", string email = "", string? log = null) =>
        Feedback.Erstellen(betreff, nachricht, email, "1.0.5", log, "Microsoft Windows 10.0.22631 (X64)");

    [Theory]
    [InlineData("", "Hallo", "", 0)]
    [InlineData(null, "Hallo", null, 0)]
    [InlineData("Betreff", "Hallo", "willi@example.de", 0)]
    [InlineData("", "", "", 1)]
    [InlineData("", "   \n  ", "", 1)]
    [InlineData("", "Hallo", "keine-mail", 1)]
    [InlineData("", "Hallo", "a@b", 1)]
    [InlineData("", "Hallo", "a b@c.de", 1)]
    [InlineData("", "", "kaputt@", 2)]
    public void Pruefen(string? betreff, string nachricht, string? email, int fehler) =>
        Assert.Equal(fehler, Feedback.Pruefen(betreff, nachricht, email).Count);

    [Fact]
    public void Laengengrenzen()
    {
        Assert.Empty(Feedback.Pruefen(new string('b', 120), new string('n', 5000), null));
        Assert.Contains(Feedback.Pruefen(new string('b', 121), "x", null), f => f.Contains("Betreff"));
        Assert.Contains(Feedback.Pruefen(null, new string('n', 5001), null), f => f.Contains("Nachricht"));
        Assert.Contains(Feedback.Pruefen(null, "x", new string('a', 250) + "@b.de"), f => f.Contains("E-Mail"));
    }

    [Theory]
    [InlineData("  Idee  ", "Idee")]
    [InlineData("Zeile1\r\nZeile2", "Zeile1 Zeile2")]
    [InlineData("SharpLab: Fehler", "Fehler")]    // Präfix setzt der Server – nicht doppelt
    [InlineData("sharplab:Fehler", "Fehler")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void Betreff_wird_bereinigt(string? ein, string aus) => Assert.Equal(aus, Feedback.BetreffBereinigen(ein));

    [Fact]
    public void Json_hat_genau_die_Felder_des_Servers()
    {
        using var doc = JsonDocument.Parse(Feedback.Json(D(email: " willi@example.de ")));
        var r = doc.RootElement;
        Assert.Equal(new[] { "subject", "message", "email", "appVersion", "os", "website" }, r.EnumerateObject().Select(p => p.Name));
        Assert.Equal("Idee", r.GetProperty("subject").GetString());          // ohne „SharpLab: “
        Assert.Equal("Mehr Übungen bitte!", r.GetProperty("message").GetString());
        Assert.Equal("willi@example.de", r.GetProperty("email").GetString());
        Assert.Equal("1.0.5", r.GetProperty("appVersion").GetString());
        Assert.Equal("Microsoft Windows 10.0.22631 (X64)", r.GetProperty("os").GetString());
        Assert.Equal("", r.GetProperty("website").GetString());               // Honeypot immer leer
    }

    [Fact]
    public void Protokoll_nur_wenn_angehaengt()
    {
        Assert.DoesNotContain("\"log\"", Feedback.Json(D()));
        using var doc = JsonDocument.Parse(Feedback.Json(D(log: "Fehler XY")));
        Assert.Equal("Fehler XY", doc.RootElement.GetProperty("log").GetString());
    }

    [Fact]
    public async Task Anfrage_ist_POST_JSON_mit_Client_Header()
    {
        using var req = Feedback.Anfrage(D());
        Assert.Equal(HttpMethod.Post, req.Method);
        Assert.Equal("https://geringdigital.de/sharplab/feedback.php", req.RequestUri!.ToString());
        Assert.Equal("1", req.Headers.GetValues("X-SharpLab-Client").Single());
        Assert.Equal("application/json", req.Content!.Headers.ContentType!.MediaType);
        Assert.Equal("utf-8", req.Content.Headers.ContentType.CharSet);
        Assert.Contains("SharpLab/1.0.5", req.Headers.UserAgent.ToString());
        Assert.Equal(Feedback.Json(D()), await req.Content.ReadAsStringAsync());
    }

    sealed class Stub(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> a) : HttpMessageHandler
    {
        public string? Koerper;
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage r, CancellationToken ct)
        {
            Koerper = r.Content == null ? null : await r.Content.ReadAsStringAsync(ct);
            return await a(r, ct);
        }
    }

    static Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> Antwort(HttpStatusCode code, string text = "") =>
        (_, _) => Task.FromResult(new HttpResponseMessage(code) { Content = new StringContent(text, Encoding.UTF8, "application/json") });

    [Theory]
    [InlineData(HttpStatusCode.OK, "{\"ok\":true}", true)]
    [InlineData(HttpStatusCode.OK, "", true)]
    [InlineData(HttpStatusCode.NoContent, "", true)]
    [InlineData(HttpStatusCode.OK, "{\"ok\":false,\"error\":\"Zu viele Nachrichten\"}", false)]
    [InlineData(HttpStatusCode.BadRequest, "{\"ok\":false}", false)]
    [InlineData(HttpStatusCode.TooManyRequests, "", false)]
    [InlineData(HttpStatusCode.InternalServerError, "<html>", false)]
    [InlineData(HttpStatusCode.NotFound, "", false)]
    public async Task Antworten_des_Servers(HttpStatusCode code, string text, bool erfolg)
    {
        var s = new Stub(Antwort(code, text));
        var r = await Feedback.SendenAsync(new HttpClient(s), D());
        Assert.Equal(erfolg, r.Erfolg);
        Assert.Equal(erfolg, r.Fehler == null);
        Assert.Equal(Feedback.Json(D()), s.Koerper);
    }

    [Fact]
    public async Task Server_Fehlertext_wird_weitergegeben()
    {
        var r = await Feedback.SendenAsync(new HttpClient(new Stub(Antwort(HttpStatusCode.OK, "{\"ok\":false,\"error\":\"Zu viele Nachrichten\"}"))), D());
        Assert.Equal("Zu viele Nachrichten", r.Fehler);
    }

    [Fact]
    public async Task Offline_und_Timeout_werfen_nicht()
    {
        var offline = new Stub((_, _) => throw new HttpRequestException("Host nicht gefunden"));
        var r1 = await Feedback.SendenAsync(new HttpClient(offline), D());
        Assert.False(r1.Erfolg);
        Assert.Contains("Keine Verbindung", r1.Fehler);

        var langsam = new Stub(async (_, ct) => { await Task.Delay(10000, ct); return new HttpResponseMessage(HttpStatusCode.OK); });
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var r2 = await Feedback.SendenAsync(new HttpClient(langsam), D(), TimeSpan.FromMilliseconds(200));
        Assert.False(r2.Erfolg);
        Assert.Contains("Zeitüberschreitung", r2.Fehler);
        Assert.True(sw.ElapsedMilliseconds < 5000);
        Assert.Equal(TimeSpan.FromSeconds(15), Feedback.Timeout);
    }

    [Fact]
    public void Mailto_mit_SharpLab_Betreff_und_Nachricht()
    {
        string link = Feedback.MailtoLink(D(betreff: "Fehler & Frage", nachricht: "Zeile 1\nÄrger mit ? und #", email: "w@example.de"));
        Assert.StartsWith("mailto:info@geringdigital.de?subject=SharpLab%3A%20Fehler%20%26%20Frage&body=", link);
        string body = Uri.UnescapeDataString(link[(link.IndexOf("&body=") + 6)..]);
        Assert.StartsWith("Zeile 1\nÄrger mit ? und #", body);
        Assert.Contains("SharpLab 1.0.5", body);
        Assert.Contains("Antwort an: w@example.de", body);
        Assert.DoesNotContain("&", link[(link.IndexOf("&body=") + 6)..]);
        Assert.Equal("mailto:info@geringdigital.de?subject=SharpLab%3A", Feedback.MailtoLink(D(betreff: "")).Split("&body=")[0]);
    }

    [Fact]
    public void Lange_Nachricht_wird_fuer_mailto_gekuerzt()
    {
        string link = Feedback.MailtoLink(D(nachricht: string.Concat(Enumerable.Repeat("Ümlaut 😀 Text ", 400))));
        Assert.True(link.Length <= Feedback.MaxMailtoLaenge);
        Assert.Contains("gekürzt", Uri.UnescapeDataString(link));
        Assert.DoesNotContain('\uFFFD', Uri.UnescapeDataString(link));
    }

    [Fact]
    public void Protokoll_Auszug_nimmt_die_letzten_20_KB_ab_Zeilenanfang()
    {
        string datei = Path.Combine(tmp, "fehler.log");
        Assert.Null(Feedback.ProtokollEnde(datei));
        var sb = new StringBuilder();
        for (int i = 0; i < 2000; i++) sb.Append($"Zeile {i:0000} – Fehler äöü\n");
        File.WriteAllText(datei, sb.ToString());
        string t = Feedback.ProtokollEnde(datei)!;
        Assert.True(Encoding.UTF8.GetByteCount(t) <= Feedback.MaxProtokollBytes);
        Assert.StartsWith("Zeile ", t);
        Assert.EndsWith("Zeile 1999 – Fehler äöü", t);
        File.WriteAllText(datei, "kurz\n");
        Assert.Equal("kurz", Feedback.ProtokollEnde(datei));
        File.WriteAllText(datei, "  \n");
        Assert.Null(Feedback.ProtokollEnde(datei));
    }

    [Fact]
    public void Protokoll_lesbar_waehrend_es_geoeffnet_ist()
    {
        string datei = Path.Combine(tmp, "offen.log");
        using var w = new FileStream(datei, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        w.Write(Encoding.UTF8.GetBytes("läuft noch\n")); w.Flush();
        Assert.Equal("läuft noch", Feedback.ProtokollEnde(datei));
    }

    [Fact]
    public void Betriebssystem_ist_gesetzt() => Assert.False(string.IsNullOrWhiteSpace(Feedback.Betriebssystem()));
}
