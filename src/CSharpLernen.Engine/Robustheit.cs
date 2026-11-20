using System.Text;

namespace CSharpLernen.Engine;

/// <summary>Schriftnamen aus Texten/Einstellungen absichern: Leere oder unbrauchbare Namen führen zur Theme-Schrift.</summary>
public static class SchriftNamen
{
    /// <summary>Theme-Schrift (eingebettete JetBrains Mono NL, dann Systemschriften).</summary>
    public const string ThemeSchrift = "pack://application:,,,/Ressourcen/Schrift/#JetBrains Mono NL, Cascadia Mono, Consolas, Courier New";

    /// <summary>Liefert einen bereinigten Schriftnamen oder <paramref name="ersatz"/>, wenn der Name leer ist
    /// bzw. keinen brauchbaren Familiennamen enthält. <paramref name="istInstalliert"/> (optional) prüft einzelne
    /// Familiennamen; ist keine der genannten Familien vorhanden, wird ebenfalls der Ersatz genommen.</summary>
    public static string Bereinige(string? name, string ersatz = ThemeSchrift, Func<string, bool>? istInstalliert = null)
    {
        if (string.IsNullOrWhiteSpace(name)) return ersatz;
        // Eine pack-URI enthält selbst Kommas (pack://application:,,,/…#Familie) – sie bleibt als ein Eintrag zusammen.
        var teile = new List<string>();
        string rest = name.Trim();
        if (rest.StartsWith("pack://", StringComparison.OrdinalIgnoreCase))
        {
            int raute = rest.IndexOf('#');
            int ende = raute < 0 ? -1 : rest.IndexOf(',', raute);
            string pack = (ende < 0 ? rest : rest[..ende]).Trim();
            if (raute >= 0 && raute < pack.Length - 1) teile.Add(pack);
            rest = ende < 0 ? "" : rest[(ende + 1)..];
        }
        teile.AddRange(rest.Split(',').Select(t => t.Trim()).Where(t => t.Length > 0 && t != "#" && !t.StartsWith("pack://", StringComparison.OrdinalIgnoreCase)));
        if (teile.Count == 0) return ersatz;
        if (istInstalliert != null && !teile.Any(t => t.Contains('#') || istInstalliert(t))) return ersatz;
        return string.Join(", ", teile);
    }
}

/// <summary>Schreibt unerwartete Fehler mit Stacktrace in eine Protokolldatei (z. B. %AppData%\CSharpLernen\fehler.log).</summary>
public static class FehlerProtokoll
{
    public const long MaxGroesse = 512 * 1024;

    /// <summary>Hängt einen Eintrag an. Wird die Datei zu groß, bleibt nur die jüngere Hälfte erhalten. Wirft nie.</summary>
    public static bool Schreiben(string datei, Exception fehler, string quelle, string? version = null)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(datei)!);
            if (File.Exists(datei) && new FileInfo(datei).Length > MaxGroesse)
            {
                string alt = File.ReadAllText(datei, Encoding.UTF8);
                File.WriteAllText(datei, "… (ältere Einträge gekürzt)" + Environment.NewLine + alt[(alt.Length / 2)..], Encoding.UTF8);
            }
            var sb = new StringBuilder();
            sb.AppendLine($"===== {DateTime.Now:yyyy-MM-dd HH:mm:ss} · {quelle}" + (version != null ? $" · SharpLab {version}" : "") + $" · {Environment.OSVersion} · .NET {Environment.Version}");
            sb.AppendLine(fehler.ToString());
            sb.AppendLine();
            File.AppendAllText(datei, sb.ToString(), Encoding.UTF8);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
