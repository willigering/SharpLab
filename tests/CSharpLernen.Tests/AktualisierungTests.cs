using System.IO.Compression;
using System.Net;
using System.Text;
using CSharpLernen.Engine;
using Xunit;

namespace CSharpLernen.Tests;

/// <summary>Auto-Update (1.0.5): alles ohne Oberfläche – Versionen, GitHub-JSON, Asset-Wahl, Prüfsumme, Entpacken, Dateitausch.</summary>
public class AktualisierungTests : IDisposable
{
    readonly string tmp = Path.Combine(Path.GetTempPath(), "sl-upd-test-" + Guid.NewGuid().ToString("N"));
    public AktualisierungTests() => Directory.CreateDirectory(tmp);
    public void Dispose() { try { Directory.Delete(tmp, true); } catch { } }

    const string Hash = "3f2a9c0d4b5e6f708192a3b4c5d6e7f8091a2b3c4d5e6f708192a3b4c5d6e7f8";

    static string ReleaseJson(string tag = "v1.1.0", bool draft = false, bool pre = false, string body = "## Neu\r\n- **Schneller** Start\r\n- Fix", params string[] assets)
    {
        if (assets.Length == 0) assets = new[] { "SharpLab-Windows.zip", "SharpLab-Quellcode.zip" };
        var a = string.Join(",", assets.Select(n => $$"""{"name":"{{n}}","size":12345678,"browser_download_url":"https://github.com/willigering/SharpLab/releases/download/{{tag}}/{{n}}"}"""));
        return $$"""
        {"tag_name":"{{tag}}","name":"SharpLab {{tag}}","draft":{{(draft ? "true" : "false")}},"prerelease":{{(pre ? "true" : "false")}},
         "html_url":"https://github.com/willigering/SharpLab/releases/tag/{{tag}}","body":{{System.Text.Json.JsonSerializer.Serialize(body)}},"assets":[{{a}}]}
        """;
    }

    [Theory]
    [InlineData("v1.2.3", "1.2.3")]
    [InlineData("V1.2.3", "1.2.3")]
    [InlineData("1.2", "1.2.0")]
    [InlineData("v2", "2.0.0")]
    [InlineData("v1.0.6-beta.1", "1.0.6")]
    [InlineData(" v1.0.10 ", "1.0.10")]
    [InlineData("1.2.3.4", "1.2.3")]
    public void Tag_wird_zur_Version(string tag, string erwartet) => Assert.Equal(Version.Parse(erwartet), Aktualisierung.VersionAusTag(tag));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("latest")]
    [InlineData("release-1")]
    public void Ungueltige_Tags(string tag) => Assert.Null(Aktualisierung.VersionAusTag(tag));

    [Theory]
    [InlineData("1.0.6", "1.0.5.0", true)]
    [InlineData("1.0.10", "1.0.9", true)]
    [InlineData("1.1.0", "1.0.99", true)]
    [InlineData("2.0.0", "1.9.9", true)]
    [InlineData("1.0.5", "1.0.5.0", false)]
    [InlineData("1.0.5", "1.0.5.7", false)]   // Revision zählt nicht
    [InlineData("1.0.4", "1.0.5", false)]
    public void Versionsvergleich(string entfernt, string lokal, bool neuer) =>
        Assert.Equal(neuer, Aktualisierung.IstNeuer(Version.Parse(entfernt), Version.Parse(lokal)));

    [Fact]
    public void Release_JSON_wird_ausgewertet()
    {
        var r = Aktualisierung.ReleaseAuswerten(ReleaseJson())!;
        Assert.Equal(new Version(1, 1, 0), r.Version);
        Assert.Equal("v1.1.0", r.Tag);
        Assert.Equal("SharpLab-Windows.zip", r.AssetName);
        Assert.EndsWith("/v1.1.0/SharpLab-Windows.zip", r.AssetUrl);
        Assert.Equal(12345678, r.AssetGroesse);
        Assert.Equal("https://github.com/willigering/SharpLab/releases/tag/v1.1.0", r.SeitenUrl);
        Assert.Equal("Neu\n• Schneller Start\n• Fix", r.Notizen);
        Assert.Null(r.Sha256);
    }

    [Fact]
    public void Entwuerfe_und_Vorabversionen_werden_ignoriert()
    {
        Assert.Null(Aktualisierung.ReleaseAuswerten(ReleaseJson(draft: true)));
        Assert.Null(Aktualisierung.ReleaseAuswerten(ReleaseJson(pre: true)));
        Assert.Null(Aktualisierung.ReleaseAuswerten(ReleaseJson(tag: "nightly")));
        Assert.Null(Aktualisierung.ReleaseAuswerten("[]"));
    }

    [Fact]
    public void Hash_aus_Body_und_als_Asset()
    {
        var r = Aktualisierung.ReleaseAuswerten(ReleaseJson(body: $"Neu: Updates\n\nSHA-256 (SharpLab-Windows.zip): {Hash.ToUpperInvariant()}"))!;
        Assert.Equal(Hash, r.Sha256);
        var r2 = Aktualisierung.ReleaseAuswerten(ReleaseJson(assets: new[] { "SharpLab-Windows.zip", "SharpLab-Windows.zip.sha256" }))!;
        Assert.Null(r2.Sha256);
        Assert.EndsWith("SharpLab-Windows.zip.sha256", r2.Sha256AssetUrl);
        Assert.Equal(Hash, Aktualisierung.HashAusText($"{Hash}  SharpLab-Windows.zip\n{new string('a', 64)}  SharpLab-Quellcode.zip", "SharpLab-Windows.zip"));
        Assert.Equal(Hash, Aktualisierung.HashAusText($"{new string('b', 64)}  andere.zip\n{Hash} *SharpLab-Windows.zip", "SharpLab-Windows.zip"));
        Assert.Null(Aktualisierung.HashAusText("kein Hash hier, nur Text", "SharpLab-Windows.zip"));
        Assert.Null(Aktualisierung.HashAusText(new string('c', 65)));   // zu lang = kein SHA-256
    }

    [Theory]
    [InlineData("SharpLab-Windows.zip", "SharpLab-Quellcode.zip|SharpLab-Windows.zip|notes.txt")]
    [InlineData("SharpLab-1.1.0-win-x64.zip", "SharpLab-Quellcode.zip|SharpLab-1.1.0-win-x64.zip")]
    [InlineData("SharpLab.zip", "SharpLab.zip|LIESMICH.txt")]
    [InlineData(null, "SharpLab-App.zip|SharpLab-Quellcode.zip")]   // mehrdeutig → lieber nichts automatisch
    [InlineData(null, "LIESMICH.txt")]
    public void Asset_Auswahl(string erwartet, string namen) => Assert.Equal(erwartet, Aktualisierung.AssetWaehlen(namen.Split('|')));

    [Fact]
    public void Release_ohne_passendes_Asset_hat_keine_Url()
    {
        var r = Aktualisierung.ReleaseAuswerten(ReleaseJson(assets: new[] { "SharpLab-App.zip", "SharpLab-Quellcode.zip" }))!;
        Assert.Null(r.AssetUrl);
        Assert.NotNull(r.SeitenUrl);
    }

    [Fact]
    public void Lange_Notizen_werden_gekuerzt()
    {
        string n = Aktualisierung.KurzeNotizen(new string('x', 5000), 3000);
        Assert.True(n.Length <= 3002);
        Assert.EndsWith("…", n);
        Assert.Equal("", Aktualisierung.KurzeNotizen(null));
    }

    [Theory]
    [InlineData(false, "1.1.0", null, true)]
    [InlineData(false, "1.1.0", "1.1.0", false)]   // übersprungen
    [InlineData(false, "1.1.0", "v1.1.0", false)]
    [InlineData(false, "1.2.0", "1.1.0", true)]    // neuere als die übersprungene
    [InlineData(true, "1.1.0", "1.1.0", true)]     // manuell: trotzdem zeigen
    [InlineData(true, "1.0.5", null, false)]       // gleich alt
    public void Anbieten_beachtet_Ueberspringen(bool manuell, string entfernt, string uebersprungen, bool erwartet)
    {
        var r = Aktualisierung.ReleaseAuswerten(ReleaseJson(tag: "v" + entfernt));
        Assert.Equal(erwartet, Aktualisierung.SollAnbieten(r, new Version(1, 0, 5, 0), uebersprungen, manuell));
    }

    [Fact]
    public void Startargument_nach_Update()
    {
        Assert.Equal("1.1.0", Aktualisierung.NachUpdateVersion(new[] { "SharpLab.exe", "--nach-update", "1.1.0" }));
        Assert.Equal("", Aktualisierung.NachUpdateVersion(new[] { "SharpLab.exe", "--nach-update" }));
        Assert.Null(Aktualisierung.NachUpdateVersion(new[] { "SharpLab.exe" }));
    }

    // ------------------------------------------------------------------ Netz (ohne echtes Netz)

    sealed class Stub : HttpMessageHandler
    {
        public Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> Antwort = null!;
        public HttpRequestMessage? Letzte;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage r, CancellationToken ct) { Letzte = r; return Antwort(r, ct); }
    }

    static HttpClient Client(Stub s)
    {
        var c = new HttpClient(s);
        c.DefaultRequestHeaders.UserAgent.ParseAdd("SharpLab/1.0.5");
        return c;
    }

    [Fact]
    public async Task Abfrage_setzt_Header_und_wertet_aus()
    {
        var s = new Stub { Antwort = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(ReleaseJson("v1.0.6")) }) };
        var r = await Aktualisierung.NeuestesReleaseAsync(Client(s), TimeSpan.FromSeconds(5));
        Assert.Equal(new Version(1, 0, 6), r!.Version);
        Assert.Equal(Aktualisierung.ApiUrl, s.Letzte!.RequestUri!.ToString());
        Assert.Contains("application/vnd.github+json", s.Letzte.Headers.Accept.ToString());
        Assert.Contains("SharpLab", s.Letzte.Headers.UserAgent.ToString() + Client(s).DefaultRequestHeaders.UserAgent);
    }

    [Fact]
    public void Eigener_Client_hat_User_Agent() =>
        Assert.StartsWith("SharpLab/1.0.5", Aktualisierung.NeuerClient("1.0.5").DefaultRequestHeaders.UserAgent.ToString());

    [Fact]
    public async Task Kein_Release_ist_kein_Fehler()
    {
        var s = new Stub { Antwort = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)) };
        Assert.Null(await Aktualisierung.NeuestesReleaseAsync(Client(s), TimeSpan.FromSeconds(5)));
    }

    [Fact]
    public async Task Timeout_bricht_ab()
    {
        var s = new Stub { Antwort = async (_, ct) => { await Task.Delay(10000, ct); return new HttpResponseMessage(HttpStatusCode.OK); } };
        var sw = System.Diagnostics.Stopwatch.StartNew();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Aktualisierung.NeuestesReleaseAsync(Client(s), TimeSpan.FromMilliseconds(200)));
        Assert.True(sw.ElapsedMilliseconds < 5000);
    }

    [Fact]
    public async Task Serverfehler_wirft()
    {
        var s = new Stub { Antwort = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden)) };   // z. B. Rate-Limit
        await Assert.ThrowsAsync<HttpRequestException>(() => Aktualisierung.NeuestesReleaseAsync(Client(s), TimeSpan.FromSeconds(5)));
    }

    [Fact]
    public async Task Download_mit_Fortschritt_und_Pruefsumme()
    {
        var daten = Encoding.UTF8.GetBytes(new string('z', 300_000));
        var s = new Stub { Antwort = (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(daten) }) };
        var werte = new List<double>();
        string ziel = Path.Combine(tmp, "dl.zip");
        await Aktualisierung.HerunterladenAsync(Client(s), "https://example.invalid/x.zip", ziel, new SofortProgress(werte.Add));
        Assert.Equal(daten, File.ReadAllBytes(ziel));
        Assert.True(werte.Count >= 2);
        Assert.Equal(1.0, werte[^1]);
        Assert.Equal(werte.OrderBy(x => x), werte);
        Assert.Equal(Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(daten)).ToLowerInvariant(), Aktualisierung.Sha256(ziel));
    }

    sealed class SofortProgress(Action<double> a) : IProgress<double> { public void Report(double v) => a(v); }

    // ------------------------------------------------------------------ Paket & Dateitausch

    string Paket(bool mitExe = true, string unterordner = "SharpLab/")
    {
        string zip = Path.Combine(tmp, "paket-" + Guid.NewGuid().ToString("N") + ".zip");
        using var z = ZipFile.Open(zip, ZipArchiveMode.Create);
        void Eintrag(string n, string t) { using var w = new StreamWriter(z.CreateEntry(unterordner + n).Open()); w.Write(t); }
        if (mitExe) Eintrag("SharpLab.exe", "NEUE-EXE");
        Eintrag("LIESMICH.txt", "neues liesmich");
        Eintrag("LICENSE", "MIT neu");
        Eintrag("THIRD-PARTY-NOTICES.txt", "notices neu");
        return zip;
    }

    [Fact]
    public void Entpacken_findet_exe_im_Unterordner_und_im_Wurzelordner()
    {
        string a = Aktualisierung.Entpacken(Paket(), Path.Combine(tmp, "x1"));
        Assert.Equal(Path.Combine(tmp, "x1", "SharpLab"), a);
        string b = Aktualisierung.Entpacken(Paket(unterordner: ""), Path.Combine(tmp, "x2"));
        Assert.Equal(Path.Combine(tmp, "x2"), b);
    }

    [Fact]
    public void Paket_ohne_exe_oder_kaputt_wird_abgelehnt()
    {
        var e = Assert.Throws<AktualisierungsFehler>(() => Aktualisierung.Entpacken(Paket(mitExe: false), Path.Combine(tmp, "x3")));
        Assert.Contains("SharpLab.exe", e.Message);
        string kaputt = Path.Combine(tmp, "kaputt.zip");
        File.WriteAllText(kaputt, "das ist keine zip");
        Assert.Throws<AktualisierungsFehler>(() => Aktualisierung.Entpacken(kaputt, Path.Combine(tmp, "x4")));
    }

    string Programmordner()
    {
        string p = Path.Combine(tmp, "programm");
        Directory.CreateDirectory(p);
        File.WriteAllText(Path.Combine(p, "SharpLab.exe"), "ALTE-EXE");
        File.WriteAllText(Path.Combine(p, "LIESMICH.txt"), "altes liesmich");
        File.WriteAllText(Path.Combine(p, "fortschritt.json"), "{\"lernstand\":1}");
        return p;
    }

    [Fact]
    public void Austausch_benennt_alt_um_und_kopiert_neu()
    {
        string p = Programmordner();
        string neu = Aktualisierung.Entpacken(Paket(), Path.Combine(tmp, "neu"));
        var t = Aktualisierung.Austauschen(Path.Combine(p, "SharpLab.exe"), neu);
        Assert.Equal(Path.Combine(p, "SharpLab.exe"), t.NeueExe);
        Assert.Equal("NEUE-EXE", File.ReadAllText(Path.Combine(p, "SharpLab.exe")));
        Assert.Equal("ALTE-EXE", File.ReadAllText(Path.Combine(p, "SharpLab.exe.old")));
        Assert.Equal("altes liesmich", File.ReadAllText(Path.Combine(p, "LIESMICH.txt.old")));
        Assert.Equal("MIT neu", File.ReadAllText(Path.Combine(p, "LICENSE")));
        Assert.Equal("{\"lernstand\":1}", File.ReadAllText(Path.Combine(p, "fortschritt.json")));   // Lernstand unberührt

        // Nächster Start: .old aufräumen, andere Dateien bleiben
        File.WriteAllText(Path.Combine(p, "eigene.old"), "nicht von uns");
        Assert.Equal(0, Aktualisierung.AlteDateienLoeschen(p));
        Assert.False(File.Exists(Path.Combine(p, "SharpLab.exe.old")));
        Assert.False(File.Exists(Path.Combine(p, "LIESMICH.txt.old")));
        Assert.True(File.Exists(Path.Combine(p, "eigene.old")));
    }

    [Fact]
    public void Rueckgaengig_stellt_alten_Zustand_her()
    {
        string p = Programmordner();
        string neu = Aktualisierung.Entpacken(Paket(), Path.Combine(tmp, "neu"));
        var t = Aktualisierung.Austauschen(Path.Combine(p, "SharpLab.exe"), neu);
        t.Rueckgaengig();
        Assert.Equal("ALTE-EXE", File.ReadAllText(Path.Combine(p, "SharpLab.exe")));
        Assert.Equal("altes liesmich", File.ReadAllText(Path.Combine(p, "LIESMICH.txt")));
        Assert.False(File.Exists(Path.Combine(p, "LICENSE")));
        Assert.Empty(Directory.GetFiles(p, "*.old"));
    }

    [Fact]
    public void Fehler_mitten_im_Austausch_rollt_zurueck()
    {
        string p = Programmordner();
        string neu = Aktualisierung.Entpacken(Paket(), Path.Combine(tmp, "neu"));
        Directory.CreateDirectory(Path.Combine(p, "LIESMICH.txt.old"));   // blockiert das Umbenennen der 2. Datei
        var e = Assert.Throws<AktualisierungsFehler>(() => Aktualisierung.Austauschen(Path.Combine(p, "SharpLab.exe"), neu));
        Assert.Contains("nicht kopiert", e.Message);
        Assert.Equal("ALTE-EXE", File.ReadAllText(Path.Combine(p, "SharpLab.exe")));
        Assert.Equal("altes liesmich", File.ReadAllText(Path.Combine(p, "LIESMICH.txt")));
        Assert.False(File.Exists(Path.Combine(p, "SharpLab.exe.old")));
    }

    [Fact]
    public void Altes_old_von_frueherem_Update_wird_ersetzt()
    {
        string p = Programmordner();
        File.WriteAllText(Path.Combine(p, "SharpLab.exe.old"), "GANZ-ALT");
        string neu = Aktualisierung.Entpacken(Paket(), Path.Combine(tmp, "neu"));
        Aktualisierung.Austauschen(Path.Combine(p, "SharpLab.exe"), neu);
        Assert.Equal("ALTE-EXE", File.ReadAllText(Path.Combine(p, "SharpLab.exe.old")));
    }

    [Fact]
    public void Umbenannte_exe_behaelt_ihren_Namen()
    {
        string p = Programmordner();
        File.Move(Path.Combine(p, "SharpLab.exe"), Path.Combine(p, "MeinSharpLab.exe"));
        string neu = Aktualisierung.Entpacken(Paket(), Path.Combine(tmp, "neu"));
        var t = Aktualisierung.Austauschen(Path.Combine(p, "MeinSharpLab.exe"), neu);
        Assert.Equal(Path.Combine(p, "MeinSharpLab.exe"), t.NeueExe);
        Assert.Equal("NEUE-EXE", File.ReadAllText(t.NeueExe));
        Assert.False(File.Exists(Path.Combine(p, "SharpLab.exe")));
    }

    [Fact]
    public void Schreibschutz_wird_erkannt()
    {
        Assert.True(Aktualisierung.IstBeschreibbar(tmp));
        Assert.False(Aktualisierung.IstBeschreibbar(Path.Combine(tmp, "gibt-es-nicht")));
    }

    [Theory]
    [InlineData(@"C:\Users\willi\AppData\Local\Programs\SharpLab", @"C:\Users\willi\AppData\Local\Programs\SharpLab", true)]
    [InlineData(@"C:\Users\willi\AppData\Local\Programs\SharpLab\", @"c:\users\willi\appdata\local\programs\sharplab", true)]
    [InlineData(@"""C:\Programs\SharpLab""", @"C:\Programs\SharpLab", true)]
    [InlineData(@"C:\Users\willi\AppData\Local\Programs\SharpLab", @"C:\Users\willi\Downloads\SharpLab", false)]   // portable Kopie
    [InlineData(null, @"C:\x", false)]
    [InlineData("", @"C:\x", false)]
    public void Uninstall_Eintrag_nur_fuer_die_eigene_Installation(string? ort, string exeOrdner, bool erwartet) =>
        Assert.Equal(erwartet, Aktualisierung.GehoertZurInstallation(ort, exeOrdner));
}
