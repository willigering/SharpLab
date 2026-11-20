#nullable disable
using System.IO;
using System.Text;
using System.Text.Json;

namespace CSharpLernen;

/// <summary>
/// Wo der Lernstand (fortschritt.json) liegt. Die Zeiger-Datei speicherort.json bleibt IMMER lokal in
/// %AppData%\CSharpLernen – sie wird nie synchronisiert. Ein eigener Ordner (z. B. OneDrive) bekommt den
/// Unterordner „SharpLab“. Enthält keine Oberfläche, damit alles ohne WPF testbar ist.
/// </summary>
public static class Speicherort
{
    public const string Unterordner = "SharpLab";
    public const string DateiName = "fortschritt.json";

    /// <summary>Nur für Tests: ersetzt %AppData%\CSharpLernen.</summary>
    public static string StandardOrdnerUeberschrieben { get; set; }

    // Ordnername bleibt bewusst „CSharpLernen“ (alter App-Name), damit gespeicherter Fortschritt erhalten bleibt.
    public static string StandardOrdner => StandardOrdnerUeberschrieben
        ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CSharpLernen");
    public static string ZeigerDatei => Path.Combine(StandardOrdner, "speicherort.json");

    sealed class Zeiger { public string Ordner { get; set; } }

    /// <summary>Der vom Benutzer gewählte Basisordner, oder null für den Standard.</summary>
    public static string LeseBasis()
    {
        try
        {
            if (!File.Exists(ZeigerDatei)) return null;
            var z = JsonSerializer.Deserialize<Zeiger>(File.ReadAllText(ZeigerDatei));
            return string.IsNullOrWhiteSpace(z?.Ordner) ? null : z.Ordner.Trim();
        }
        catch { return null; }
    }

    /// <summary>Speichert den Basisordner (null/leer = Standard, dann wird die Zeiger-Datei entfernt).</summary>
    public static void SchreibeBasis(string basis)
    {
        if (string.IsNullOrWhiteSpace(basis) || IstStandard(LernstandOrdner(basis)))
        {
            if (File.Exists(ZeigerDatei)) File.Delete(ZeigerDatei);
            return;
        }
        AtomarSchreiben(ZeigerDatei, JsonSerializer.Serialize(new Zeiger { Ordner = basis.Trim() }, new JsonSerializerOptions { WriteIndented = true }));
    }

    /// <summary>Ordner, in dem die fortschritt.json liegt. Leer = Standard. Heißt der gewählte Ordner schon
    /// „SharpLab“, wird kein zweiter Unterordner angehängt.</summary>
    public static string LernstandOrdner(string basis)
    {
        if (string.IsNullOrWhiteSpace(basis)) return StandardOrdner;
        string b = Path.GetFullPath(basis.Trim());
        if (IstGleich(b, StandardOrdner)) return StandardOrdner;
        string name = Path.GetFileName(Path.TrimEndingDirectorySeparator(b));
        return string.Equals(name, Unterordner, StringComparison.OrdinalIgnoreCase) ? Path.TrimEndingDirectorySeparator(b) : Path.Combine(b, Unterordner);
    }

    public static string LernstandDatei(string ordner) => Path.Combine(ordner, DateiName);

    public static bool IstStandard(string ordner) => IstGleich(ordner, StandardOrdner);

    public static bool IstGleich(string a, string b)
    {
        if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b)) return false;
        try { return string.Equals(Path.TrimEndingDirectorySeparator(Path.GetFullPath(a)), Path.TrimEndingDirectorySeparator(Path.GetFullPath(b)), StringComparison.OrdinalIgnoreCase); }
        catch { return false; }
    }

    /// <summary>Prüft, ob der Ordner angelegt und beschrieben werden kann (legt ihn ggf. an, schreibt und löscht eine Probedatei).</summary>
    public static bool IstBeschreibbar(string ordner, out string fehler)
    {
        fehler = null;
        try
        {
            if (string.IsNullOrWhiteSpace(ordner)) { fehler = "Kein Ordner angegeben."; return false; }
            Directory.CreateDirectory(ordner);
            string probe = Path.Combine(ordner, ".sharplab-schreibtest-" + Guid.NewGuid().ToString("N")[..8]);
            File.WriteAllText(probe, "ok");
            File.Delete(probe);
            return true;
        }
        catch (Exception ex)
        {
            fehler = ex.Message;
            return false;
        }
    }

    /// <summary>OneDrive-Ordner aus den Umgebungsvariablen (privat oder geschäftlich), falls vorhanden.</summary>
    public static string OneDriveOrdner(Func<string, string> umgebung = null)
    {
        umgebung ??= Environment.GetEnvironmentVariable;
        foreach (var name in new[] { "OneDrive", "OneDriveConsumer", "OneDriveCommercial" })
        {
            string p = umgebung(name);
            if (!string.IsNullOrWhiteSpace(p) && Directory.Exists(p)) return p.Trim();
        }
        return null;
    }

    /// <summary>
    /// Schreibt atomar: erst eine Temp-Datei im selben Ordner, dann ersetzen. So sehen Sync-Clients nie eine halbe Datei.
    /// </summary>
    public static void AtomarSchreiben(string datei, string inhalt)
    {
        string ordner = Path.GetDirectoryName(Path.GetFullPath(datei));
        Directory.CreateDirectory(ordner);
        string tmp = Path.Combine(ordner, "~" + Path.GetFileName(datei) + "." + Guid.NewGuid().ToString("N")[..8] + ".tmp");
        try
        {
            using (var fs = new FileStream(tmp, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            using (var w = new StreamWriter(fs, new UTF8Encoding(false)))
            {
                w.Write(inhalt);
                w.Flush();
                fs.Flush(true);
            }
            if (File.Exists(datei))
            {
                try { File.Replace(tmp, datei, null, true); }
                catch (PlatformNotSupportedException) { File.Move(tmp, datei, true); }
                catch (IOException) { File.Move(tmp, datei, true); }
            }
            else File.Move(tmp, datei);
        }
        finally
        {
            try { if (File.Exists(tmp)) File.Delete(tmp); } catch { }
        }
    }

    public enum Wechselart { MeinenKopieren, VorhandenenUebernehmen }

    /// <summary>
    /// Wechselt den Speicherort (ohne Oberfläche). Bei <see cref="Wechselart.MeinenKopieren"/> wird der aktuelle Lernstand
    /// in den Zielordner geschrieben; eine dort vorhandene Datei wird vorher als Sicherung umbenannt. Bei
    /// <see cref="Wechselart.VorhandenenUebernehmen"/> bleibt die Datei im Ziel unverändert. Danach zeigt der Zeiger auf
    /// den neuen Ort. Gibt den neuen Lernstand-Ordner zurück; wirft bei Fehlern (der Zeiger bleibt dann unverändert).
    /// </summary>
    public static string Wechseln(string basis, string aktuellerLernstandJson, Wechselart art)
    {
        string ziel = LernstandOrdner(basis);
        if (!IstBeschreibbar(ziel, out var fehler)) throw new IOException($"Der Ordner „{ziel}“ ist nicht beschreibbar: {fehler}");
        string datei = LernstandDatei(ziel);
        if (art == Wechselart.MeinenKopieren)
        {
            if (File.Exists(datei))
                File.Copy(datei, Path.Combine(ziel, $"fortschritt.vorher-{DateTime.Now:yyyyMMdd-HHmmss}.json"), true);
            AtomarSchreiben(datei, aktuellerLernstandJson ?? "{}");
        }
        else if (!File.Exists(datei))
        {
            AtomarSchreiben(datei, aktuellerLernstandJson ?? "{}");
        }
        SchreibeBasis(IstStandard(ziel) ? null : basis);
        return ziel;
    }
}
