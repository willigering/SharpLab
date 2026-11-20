using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace CSharpLernen.Engine;

/// <summary>Inhalt einer Feedback-Nachricht (Nutzer-Betreff ohne „SharpLab: “ – das setzt der Server davor).</summary>
public sealed record FeedbackDaten(string Betreff, string Nachricht, string? Email, string AppVersion, string Os, string? Protokoll = null);

public sealed record FeedbackErgebnis(bool Erfolg, string? Fehler = null);

/// <summary>Feedback an Gering Digital: Prüfen, Anfrage bauen, senden, mailto-Ausweichweg – ohne Oberfläche, damit testbar.</summary>
public static class Feedback
{
    public const string Endpunkt = "https://geringdigital.de/sharplab/feedback.php";
    public const string Empfaenger = "info@geringdigital.de";
    public const string BetreffPraefix = "SharpLab: ";
    public const int MaxBetreff = 120;
    public const int MaxNachricht = 5000;
    public const int MaxEmail = 254;
    public const int MaxProtokollBytes = 20 * 1024;
    /// <summary>mailto-Links werden von Windows/Mailprogrammen ab ca. 2000 Zeichen abgeschnitten.</summary>
    public const int MaxMailtoLaenge = 1900;
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(15);

    static readonly Regex EmailMuster = new(@"^[^@\s<>()\[\],;:""]+@[^@\s<>()\[\],;:""]+\.[^@\s<>()\[\],;:""]{2,}$");

    /// <summary>Liefert verständliche Fehler (leer = alles in Ordnung).</summary>
    public static IReadOnlyList<string> Pruefen(string? betreff, string? nachricht, string? email)
    {
        var f = new List<string>();
        if (string.IsNullOrWhiteSpace(nachricht)) f.Add("Bitte schreib eine Nachricht.");
        else if (nachricht.Trim().Length > MaxNachricht) f.Add($"Die Nachricht ist zu lang (höchstens {MaxNachricht} Zeichen).");
        if ((betreff ?? "").Trim().Length > MaxBetreff) f.Add($"Der Betreff ist zu lang (höchstens {MaxBetreff} Zeichen).");
        string e = (email ?? "").Trim();
        if (e.Length > 0 && (e.Length > MaxEmail || !EmailMuster.IsMatch(e))) f.Add("Die E-Mail-Adresse sieht nicht gültig aus. Lass das Feld leer oder prüfe die Schreibweise.");
        return f;
    }

    /// <summary>Betreff: eine Zeile, ohne Steuerzeichen, gekürzt; ein vom Nutzer selbst getipptes „SharpLab:“ wird nicht verdoppelt.</summary>
    public static string BetreffBereinigen(string? betreff)
    {
        string b = Regex.Replace(betreff ?? "", @"[\r\n\t\p{Cc}]+", " ").Trim();
        b = Regex.Replace(b, @"^SharpLab\s*:\s*", "", RegexOptions.IgnoreCase);
        return b.Length > MaxBetreff ? b[..MaxBetreff].TrimEnd() : b;
    }

    public static FeedbackDaten Erstellen(string? betreff, string nachricht, string? email, string appVersion, string? protokoll = null, string? os = null) =>
        new(BetreffBereinigen(betreff), nachricht.Trim(), string.IsNullOrWhiteSpace(email) ? "" : email.Trim(), appVersion, os ?? Betriebssystem(), protokoll);

    public static string Betriebssystem() => $"{RuntimeInformation.OSDescription.Trim()} ({RuntimeInformation.OSArchitecture})";

    /// <summary>JSON wie vom Server erwartet: subject, message, email, appVersion, os, website (Honeypot, immer leer), optional log.</summary>
    public static string Json(FeedbackDaten d)
    {
        var o = new JsonObject
        {
            ["subject"] = d.Betreff,
            ["message"] = d.Nachricht,
            ["email"] = d.Email ?? "",
            ["appVersion"] = d.AppVersion,
            ["os"] = d.Os,
            ["website"] = "",
        };
        if (!string.IsNullOrEmpty(d.Protokoll)) o["log"] = d.Protokoll;
        return o.ToJsonString();
    }

    public static HttpRequestMessage Anfrage(FeedbackDaten d, string url = Endpunkt)
    {
        var r = new HttpRequestMessage(HttpMethod.Post, url) { Content = new StringContent(Json(d), Encoding.UTF8, "application/json") };
        r.Headers.Add("X-SharpLab-Client", "1");
        r.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        r.Headers.UserAgent.Add(new ProductInfoHeaderValue("SharpLab", d.AppVersion));
        return r;
    }

    /// <summary>Sendet das Feedback (Timeout 15 s). Wirft nicht – Fehler stehen im Ergebnis.</summary>
    public static async Task<FeedbackErgebnis> SendenAsync(HttpClient client, FeedbackDaten d, TimeSpan? timeout = null, CancellationToken ct = default, string url = Endpunkt)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(timeout ?? Timeout);
        try
        {
            using var req = Anfrage(d, url);
            using var resp = await client.SendAsync(req, cts.Token).ConfigureAwait(false);
            string text = await resp.Content.ReadAsStringAsync(cts.Token).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode) return new(false, $"Der Server hat mit {(int)resp.StatusCode} geantwortet.");
            // Antwortet der Server mit JSON {"ok":false,...}, gilt das als Fehler.
            try
            {
                if (JsonNode.Parse(text) is JsonObject o && o["ok"] is JsonValue v && v.TryGetValue<bool>(out bool ok) && !ok)
                    return new(false, o["error"]?.ToString() ?? "Der Server hat die Nachricht abgelehnt.");
            }
            catch (JsonException) { }
            return new(true);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested) { return new(false, "Zeitüberschreitung – der Server hat nicht rechtzeitig geantwortet."); }
        catch (HttpRequestException ex) { return new(false, "Keine Verbindung zum Server (" + ex.Message + ")."); }
    }

    /// <summary>Ausweichweg: mailto-Link mit „SharpLab: Betreff“ und der Nachricht (gekürzt, falls der Link zu lang würde).</summary>
    public static string MailtoLink(FeedbackDaten d)
    {
        string betreff = (BetreffPraefix + d.Betreff).TrimEnd();
        string fuss = $"\n\n---\nSharpLab {d.AppVersion} · {d.Os}" + (string.IsNullOrEmpty(d.Email) ? "" : $"\nAntwort an: {d.Email}");
        string Link(string nachricht) =>
            $"mailto:{Empfaenger}?subject={Uri.EscapeDataString(betreff)}&body={Uri.EscapeDataString(nachricht + fuss)}";
        string n = d.Nachricht;
        string link = Link(n);
        if (link.Length <= MaxMailtoLaenge) return link;
        const string gekuerzt = "\n[… gekürzt – bitte den Rest hier einfügen]";
        int lo = 0, hi = n.Length;
        while (lo < hi)   // längster Anfang, der noch passt
        {
            int mid = (lo + hi + 1) / 2;
            if (Link(n[..mid] + gekuerzt).Length <= MaxMailtoLaenge) lo = mid; else hi = mid - 1;
        }
        if (lo > 0 && char.IsHighSurrogate(n[lo - 1])) lo--;
        return Link(n[..lo] + gekuerzt);
    }

    /// <summary>Die letzten ~20 KB von fehler.log (ab einem Zeilenanfang), oder null, wenn es keins gibt.</summary>
    public static string? ProtokollEnde(string datei, int maxBytes = MaxProtokollBytes)
    {
        try
        {
            if (!File.Exists(datei)) return null;
            using var s = new FileStream(datei, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            long start = Math.Max(0, s.Length - maxBytes);
            s.Seek(start, SeekOrigin.Begin);
            var puffer = new byte[s.Length - start];
            int gelesen = 0, n;
            while (gelesen < puffer.Length && (n = s.Read(puffer, gelesen, puffer.Length - gelesen)) > 0) gelesen += n;
            int ab = 0;
            if (start > 0)   // angeschnittene erste Zeile weglassen
            {
                int nl = Array.IndexOf(puffer, (byte)'\n', 0, gelesen);
                ab = nl >= 0 && nl + 1 < gelesen ? nl + 1 : 0;
                while (ab < gelesen && (puffer[ab] & 0xC0) == 0x80) ab++;   // nicht mitten in einem UTF-8-Zeichen beginnen
            }
            string t = Encoding.UTF8.GetString(puffer, ab, gelesen - ab).Trim();
            return t.Length == 0 ? null : t;
        }
        catch { return null; }
    }
}
