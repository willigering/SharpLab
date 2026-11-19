using System.IO;
using System.Text.Json;

namespace CSharpLernen;

/// <summary>Alles, was sich die App merkt. Wird als JSON in %AppData%\CSharpLernen\fortschritt.json gespeichert.</summary>
public sealed class Fortschritt
{
    public HashSet<string> GeloesteAufgaben { get; set; } = new();
    /// <summary>Zuletzt geschriebener Code je Aufgabe (Schlüssel = Aufgaben-Id, "bsp:Lektion" oder "spielwiese").</summary>
    public Dictionary<string, string> Code { get; set; } = new();
    /// <summary>Beste Anzahl richtiger Quiz-Antworten je Lektion.</summary>
    public Dictionary<string, int> QuizPunkte { get; set; } = new();
    public HashSet<string> GeleseneLektionen { get; set; } = new();
    public string LetzteLektion { get; set; }
    public int Ausfuehrungen { get; set; }
    public int ErfolgreicheLaeufe { get; set; }
    public int SpielwieseLaeufe { get; set; }
    public bool FehlerBehoben { get; set; }
    public bool EndlosschleifeGestoppt { get; set; }
    public bool AllesFreigeschaltet { get; set; }
    public double Schriftgroesse { get; set; } = 15;
    public int Zeitlimit { get; set; } = 10;
    public bool MatrixRegen { get; set; } = true;
    public double MatrixStufe { get; set; } = 0.7;
    public DateTime ErsterStart { get; set; } = DateTime.Now;

    // Ordnername bleibt bewusst „CSharpLernen“ (alter App-Name), damit gespeicherter Fortschritt nach der Umbenennung in SharpLab erhalten bleibt.
    public static string Ordner => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CSharpLernen");
    public static string Datei => Path.Combine(Ordner, "fortschritt.json");

    static readonly JsonSerializerOptions Optionen = new() { WriteIndented = true };

    public static Fortschritt Laden()
    {
        try
        {
            if (File.Exists(Datei))
                return JsonSerializer.Deserialize<Fortschritt>(File.ReadAllText(Datei)) ?? new Fortschritt();
        }
        catch
        {
            try { File.Copy(Datei, Datei + ".kaputt", true); } catch { }
        }
        return new Fortschritt();
    }

    public void Speichern()
    {
        try
        {
            Directory.CreateDirectory(Ordner);
            string tmp = Datei + ".tmp";
            File.WriteAllText(tmp, JsonSerializer.Serialize(this, Optionen));
            File.Move(tmp, Datei, true);
        }
        catch
        {
            // Speichern ist nicht kritisch
        }
    }
}
