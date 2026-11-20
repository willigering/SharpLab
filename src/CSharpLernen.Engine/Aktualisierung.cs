using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CSharpLernen.Engine;

/// <summary>Ein veröffentlichtes GitHub-Release (nur stabile Releases, keine Entwürfe/Vorabversionen).</summary>
public sealed record ReleaseInfo(
    Version Version,
    string Tag,
    string Notizen,
    string? SeitenUrl,
    string? AssetName,
    string? AssetUrl,
    long AssetGroesse,
    string? Sha256,
    string? Sha256AssetUrl);

/// <summary>Fehler beim Aktualisieren mit einer freundlichen Meldung für die Oberfläche.</summary>
public sealed class AktualisierungsFehler : Exception
{
    public AktualisierungsFehler(string meldung, Exception? innen = null) : base(meldung, innen) { }
}

/// <summary>
/// Auto-Update über GitHub-Releases – ohne Oberfläche, damit alles testbar ist:
/// Versionsvergleich, JSON auswerten, Asset wählen, herunterladen, prüfen (SHA-256), Dateien austauschen und zurückrollen.
/// </summary>
public static class Aktualisierung
{
    public const string Repo = "willigering/SharpLab";
    public const string ApiUrl = "https://api.github.com/repos/" + Repo + "/releases/latest";
    public const string ReleaseSeite = "https://github.com/" + Repo + "/releases/latest";
    public const string AssetName = "SharpLab-Windows.zip";
    public const string ExeName = "SharpLab.exe";
    public const string AltEndung = ".old";
    public const string NachUpdateArgument = "--nach-update";
    /// <summary>Diese Dateien werden (falls im Paket) neben der exe mit ausgetauscht.</summary>
    public static readonly string[] Begleitdateien = { "LIESMICH.txt", "LICENSE", "THIRD-PARTY-NOTICES.txt" };

    // ------------------------------------------------------------------ Versionen

    /// <summary>„v1.2.3“, „1.2“, „V2.0.0-beta“ → 1.2.3 / 1.2.0 / 2.0.0. Ungültig → null.</summary>
    public static Version? VersionAusTag(string? tag)
    {
        if (string.IsNullOrWhiteSpace(tag)) return null;
        var m = Regex.Match(tag.Trim(), @"^[vV]?(\d+)(?:\.(\d+))?(?:\.(\d+))?(?:\.(\d+))?(?:[-+].*)?$");
        if (!m.Success) return null;
        int Teil(int i) => m.Groups[i].Success ? int.Parse(m.Groups[i].Value) : 0;
        try { return new Version(Teil(1), Teil(2), Teil(3)); } catch { return null; }
    }

    /// <summary>Vergleicht nur Major.Minor.Build (die Revision der Assembly-Version spielt keine Rolle).</summary>
    public static bool IstNeuer(Version? entfernt, Version? lokal)
    {
        if (entfernt == null) return false;
        if (lokal == null) return true;
        static Version N(Version v) => new(v.Major, Math.Max(v.Minor, 0), Math.Max(v.Build, 0));
        return N(entfernt) > N(lokal);
    }

    public static string Anzeige(Version v) => $"{v.Major}.{Math.Max(v.Minor, 0)}.{Math.Max(v.Build, 0)}";

    // ------------------------------------------------------------------ JSON

    /// <summary>Wertet die Antwort von /releases/latest aus. Entwürfe, Vorabversionen und ungültige Tags → null.</summary>
    public static ReleaseInfo? ReleaseAuswerten(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var r = doc.RootElement;
        if (r.ValueKind != JsonValueKind.Object) return null;
        if (Bool(r, "draft") || Bool(r, "prerelease")) return null;
        string tag = Str(r, "tag_name") ?? "";
        var version = VersionAusTag(tag);
        if (version == null) return null;
        string body = Str(r, "body") ?? "";

        var assets = new List<(string Name, string Url, long Groesse)>();
        if (r.TryGetProperty("assets", out var a) && a.ValueKind == JsonValueKind.Array)
            foreach (var x in a.EnumerateArray())
            {
                string? n = Str(x, "name"), u = Str(x, "browser_download_url");
                if (n != null && u != null) assets.Add((n, u, x.TryGetProperty("size", out var s) && s.TryGetInt64(out var g) ? g : 0));
            }

        var zip = AssetWaehlen(assets.Select(x => x.Name));
        var gewaehlt = zip == null ? default : assets.First(x => x.Name == zip);
        string? hashAsset = zip == null ? null : assets.Select(x => x.Name).FirstOrDefault(n =>
            n.Equals(zip + ".sha256", StringComparison.OrdinalIgnoreCase) || n.Equals("SHA256SUMS", StringComparison.OrdinalIgnoreCase)
            || n.Equals("SHA256SUMS.txt", StringComparison.OrdinalIgnoreCase));

        return new ReleaseInfo(version, tag, KurzeNotizen(body), Str(r, "html_url"),
            zip, gewaehlt.Url, gewaehlt.Groesse, zip == null ? null : HashAusText(body, zip),
            hashAsset == null ? null : assets.First(x => x.Name == hashAsset).Url);
    }

    /// <summary>Bevorzugt „SharpLab-Windows.zip“, sonst eine ZIP mit „win“ im Namen, sonst die einzige ZIP.</summary>
    public static string? AssetWaehlen(IEnumerable<string> namen)
    {
        var zips = namen.Where(n => n.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)).ToList();
        return zips.FirstOrDefault(n => n.Equals(AssetName, StringComparison.OrdinalIgnoreCase))
            ?? zips.FirstOrDefault(n => n.Contains("win", StringComparison.OrdinalIgnoreCase) && !n.Contains("quell", StringComparison.OrdinalIgnoreCase) && !n.Contains("source", StringComparison.OrdinalIgnoreCase))
            ?? (zips.Count == 1 ? zips[0] : null);
    }

    /// <summary>Sucht einen SHA-256 (64 Hex-Zeichen) in Release-Text oder .sha256-Datei:
    /// bevorzugt in der Zeile mit dem Dateinamen („&lt;hash&gt;  SharpLab-Windows.zip“ oder „SHA-256: &lt;hash&gt;“).</summary>
    public static string? HashAusText(string? text, string? dateiname = null)
    {
        if (string.IsNullOrEmpty(text)) return null;
        var hex = new Regex(@"(?<![0-9a-fA-F])[0-9a-fA-F]{64}(?![0-9a-fA-F])");
        var zeilen = text.Split('\n');
        if (dateiname != null)
            foreach (var z in zeilen)
                if (z.Contains(dateiname, StringComparison.OrdinalIgnoreCase) && hex.Match(z) is { Success: true } m) return m.Value.ToLowerInvariant();
        foreach (var z in zeilen)
            if (Regex.IsMatch(z, @"sha-?256", RegexOptions.IgnoreCase) && hex.Match(z) is { Success: true } m) return m.Value.ToLowerInvariant();
        var alle = hex.Matches(text);
        return alle.Count == 1 ? alle[0].Value.ToLowerInvariant() : null;
    }

    /// <summary>Release-Notizen für den Dialog: Zeilenenden vereinheitlicht, Markdown-Überschriften/Fettdruck entfernt, gekürzt.</summary>
    public static string KurzeNotizen(string? body, int max = 3000)
    {
        if (string.IsNullOrWhiteSpace(body)) return "";
        string t = body.Replace("\r\n", "\n").Trim();
        t = Regex.Replace(t, @"^#{1,6}\s*", "", RegexOptions.Multiline);
        t = t.Replace("**", "").Replace("__", "");
        t = Regex.Replace(t, @"^\s*[-*]\s+", "• ", RegexOptions.Multiline);
        t = Regex.Replace(t, @"\n{3,}", "\n\n");
        return t.Length <= max ? t : t[..max].TrimEnd() + " …";
    }

    static string? Str(JsonElement e, string n) => e.TryGetProperty(n, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
    static bool Bool(JsonElement e, string n) => e.TryGetProperty(n, out var v) && v.ValueKind == JsonValueKind.True;

    // ------------------------------------------------------------------ Netz

    public static HttpClient NeuerClient(string? eigeneVersion)
    {
        var c = new HttpClient { Timeout = Timeout.InfiniteTimeSpan };
        c.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("SharpLab", eigeneVersion ?? "1.0"));
        c.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("(+https://github.com/" + Repo + ")"));
        return c;
    }

    /// <summary>Fragt das neueste Release ab (Timeout z. B. 5 s). Kein Release (404) → null; Netzfehler → Ausnahme.</summary>
    public static async Task<ReleaseInfo?> NeuestesReleaseAsync(HttpClient client, TimeSpan timeout, CancellationToken ct = default, string url = ApiUrl)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(timeout);
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        req.Headers.Add("X-GitHub-Api-Version", "2022-11-28");
        using var resp = await client.SendAsync(req, cts.Token).ConfigureAwait(false);
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;
        resp.EnsureSuccessStatusCode();
        return ReleaseAuswerten(await resp.Content.ReadAsStringAsync(cts.Token).ConfigureAwait(false));
    }

    /// <summary>Lädt eine Datei mit Fortschritt (0..1; -1 wenn die Größe unbekannt ist).</summary>
    public static async Task HerunterladenAsync(HttpClient client, string url, string ziel, IProgress<double>? fortschritt, CancellationToken ct = default)
    {
        using var resp = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
        resp.EnsureSuccessStatusCode();
        long? gesamt = resp.Content.Headers.ContentLength;
        await using var quelle = await resp.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
        await using var datei = File.Create(ziel);
        var puffer = new byte[81920];
        long gelesen = 0; int n;
        while ((n = await quelle.ReadAsync(puffer, ct).ConfigureAwait(false)) > 0)
        {
            await datei.WriteAsync(puffer.AsMemory(0, n), ct).ConfigureAwait(false);
            gelesen += n;
            fortschritt?.Report(gesamt > 0 ? Math.Min(1.0, (double)gelesen / gesamt.Value) : -1);
        }
        fortschritt?.Report(1.0);
    }

    // ------------------------------------------------------------------ Paket prüfen

    public static string Sha256(string datei)
    {
        using var s = File.OpenRead(datei);
        return Convert.ToHexString(SHA256.HashData(s)).ToLowerInvariant();
    }

    /// <summary>Entpackt das Paket in einen eigenen Temp-Ordner und liefert den Ordner, in dem SharpLab.exe liegt
    /// (direkt oder in genau einem Unterordner, z. B. „SharpLab/“). Ohne exe → AktualisierungsFehler.</summary>
    public static string Entpacken(string zip, string zielOrdner)
    {
        if (Directory.Exists(zielOrdner)) Directory.Delete(zielOrdner, true);
        Directory.CreateDirectory(zielOrdner);
        try { ZipFile.ExtractToDirectory(zip, zielOrdner); }   // schützt selbst vor „../“-Pfaden
        catch (Exception ex) when (ex is InvalidDataException or IOException) { throw new AktualisierungsFehler("Das Update-Paket ist beschädigt.", ex); }
        return FindeExeOrdner(zielOrdner) ?? throw new AktualisierungsFehler("Im Update-Paket fehlt " + ExeName + ".");
    }

    public static string? FindeExeOrdner(string ordner)
    {
        if (File.Exists(Path.Combine(ordner, ExeName))) return ordner;
        var unter = Directory.GetDirectories(ordner).Where(d => File.Exists(Path.Combine(d, ExeName))).ToList();
        return unter.Count == 1 ? unter[0] : null;
    }

    /// <summary>Prüft, ob im Programmordner geschrieben werden darf (z. B. nicht in „Programme“ ohne Adminrechte).</summary>
    public static bool IstBeschreibbar(string ordner)
    {
        try
        {
            string probe = Path.Combine(ordner, ".sharplab-update-probe-" + Guid.NewGuid().ToString("N"));
            File.WriteAllText(probe, "");
            File.Delete(probe);
            return true;
        }
        catch { return false; }
    }

    // ------------------------------------------------------------------ Austausch

    /// <summary>
    /// Tauscht die laufende exe aus: aktuelle exe → „.old“ umbenennen (geht unter Windows auch, während sie läuft),
    /// neue exe und Begleitdateien hineinkopieren. Bei einem Fehler wird alles zurückgerollt.
    /// </summary>
    /// <returns>Der Austausch mit dem Pfad der neuen exe (gleicher Name wie die laufende) und <see cref="Austausch.Rueckgaengig"/>
    /// für den Fall, dass die neue exe danach nicht startet.</returns>
    public static Austausch Austauschen(string aktuelleExe, string neuOrdner)
    {
        string ordner = Path.GetDirectoryName(Path.GetFullPath(aktuelleExe))!;
        if (!IstBeschreibbar(ordner))
            throw new AktualisierungsFehler($"SharpLab darf im Ordner „{ordner}“ nichts ändern (fehlende Schreibrechte).");

        var paare = new List<(string Neu, string Ziel)> { (Path.Combine(neuOrdner, ExeName), Path.GetFullPath(aktuelleExe)) };
        foreach (var b in Begleitdateien)
            if (File.Exists(Path.Combine(neuOrdner, b))) paare.Add((Path.Combine(neuOrdner, b), Path.Combine(ordner, b)));

        var umbenannt = new List<(string Ziel, string Alt)>();
        var kopiert = new List<string>();
        try
        {
            foreach (var (neu, ziel) in paare)
            {
                if (File.Exists(ziel))
                {
                    string alt = AltName(ziel);
                    File.Move(ziel, alt);
                    umbenannt.Add((ziel, alt));
                }
                File.Copy(neu, ziel, overwrite: false);
                kopiert.Add(ziel);
            }
            return new Austausch(paare[0].Ziel, kopiert, umbenannt);
        }
        catch (Exception ex)
        {
            Zurueckrollen(kopiert, umbenannt);
            throw ex as AktualisierungsFehler ?? new AktualisierungsFehler("Die neuen Dateien konnten nicht kopiert werden: " + ex.Message, ex);
        }
    }

    internal static void Zurueckrollen(List<string> kopiert, List<(string Ziel, string Alt)> umbenannt)
    {
        foreach (var k in kopiert) try { if (File.Exists(k)) File.Delete(k); } catch { }
        foreach (var (ziel, alt) in Enumerable.Reverse(umbenannt))
            try { if (!File.Exists(ziel) && File.Exists(alt)) File.Move(alt, ziel); } catch { }
    }

    /// <summary>„SharpLab.exe.old“; ist die noch gesperrt (alter Prozess läuft noch), „SharpLab.exe.1.old“ usw.</summary>
    static string AltName(string ziel)
    {
        string alt = ziel + AltEndung;
        if (!File.Exists(alt)) return alt;
        try { File.Delete(alt); return alt; } catch { }
        for (int i = 1; ; i++)
        {
            string a = $"{ziel}.{i}{AltEndung}";
            if (!File.Exists(a)) return a;
            try { File.Delete(a); return a; } catch { }
        }
    }

    /// <summary>Löscht übrig gebliebene „.old“-Dateien eines früheren Updates. Liefert, wie viele noch gesperrt sind.</summary>
    public static int AlteDateienLoeschen(string ordner)
    {
        int rest = 0;
        var namen = new[] { ExeName }.Concat(Begleitdateien).ToList();
        foreach (var f in Directory.Exists(ordner) ? Directory.GetFiles(ordner, "*" + AltEndung) : Array.Empty<string>())
        {
            string n = Path.GetFileName(f);
            if (!namen.Any(x => n.StartsWith(x + ".", StringComparison.OrdinalIgnoreCase))) continue;
            try { File.Delete(f); } catch { rest++; }
        }
        return rest;
    }

    /// <summary>Liest „--nach-update 1.2.3“ aus den Startargumenten.</summary>
    public static string? NachUpdateVersion(IReadOnlyList<string> args)
    {
        for (int i = 0; i < args.Count; i++)
            if (args[i].Equals(NachUpdateArgument, StringComparison.OrdinalIgnoreCase))
                return i + 1 < args.Count && !args[i + 1].StartsWith("--") ? args[i + 1] : "";
        return null;
    }

    /// <summary>Registry-Schlüssel (HKCU) des Installers für „Apps &amp; Features“.</summary>
    public const string UninstallSchluessel = @"Software\Microsoft\Windows\CurrentVersion\Uninstall\SharpLab";

    /// <summary>Gehört der Uninstall-Eintrag (InstallLocation) zu dieser exe? Dann darf die App nach einem Update
    /// „DisplayVersion“ nachziehen. Portable Kopien woanders lassen den Eintrag in Ruhe.</summary>
    public static bool GehoertZurInstallation(string? installLocation, string? exeOrdner)
    {
        if (string.IsNullOrWhiteSpace(installLocation) || string.IsNullOrWhiteSpace(exeOrdner)) return false;
        static string N(string p) => p.Trim().Trim('"').Replace('/', '\\').TrimEnd('\\');
        return string.Equals(N(installLocation), N(exeOrdner), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Ob für dieses Release ein Hinweis gezeigt werden soll (neuer und nicht übersprungen).</summary>
    public static bool SollAnbieten(ReleaseInfo? r, Version? lokal, string? uebersprungen, bool manuell) =>
        r != null && IstNeuer(r.Version, lokal) && (manuell || !string.Equals(Anzeige(r.Version), VersionAusTag(uebersprungen) is { } u ? Anzeige(u) : null));
}

/// <summary>Ergebnis von <see cref="Aktualisierung.Austauschen"/>.</summary>
public sealed class Austausch
{
    readonly List<string> kopiert;
    readonly List<(string Ziel, string Alt)> umbenannt;
    internal Austausch(string neueExe, List<string> kopiert, List<(string Ziel, string Alt)> umbenannt)
    { NeueExe = neueExe; this.kopiert = kopiert; this.umbenannt = umbenannt; }

    public string NeueExe { get; }
    /// <summary>Die umbenannten alten Dateien (z. B. SharpLab.exe.old).</summary>
    public IReadOnlyList<string> AlteDateien => umbenannt.Select(u => u.Alt).ToList();

    /// <summary>Stellt den Zustand vor dem Austausch wieder her.</summary>
    public void Rueckgaengig() => Aktualisierung.Zurueckrollen(kopiert, umbenannt);
}
