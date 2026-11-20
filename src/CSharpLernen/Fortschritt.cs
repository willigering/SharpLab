#nullable disable
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CSharpLernen;

/// <summary>Alles, was sich die App merkt. Lernstand → fortschritt.json am gewählten Speicherort (Standard %AppData%\CSharpLernen),
/// lokale Einstellungen → %AppData%\CSharpLernen\einstellungen.json.</summary>
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
    public double MatrixTempo { get; set; } = 1.0;
    public bool Autovervollstaendigung { get; set; } = true;
    /// <summary>Beim Start still auf GitHub nach einer neuen Version suchen.</summary>
    public bool UpdateBeimStart { get; set; } = true;
    /// <summary>„Diese Version überspringen“: diese Version wird beim Start nicht mehr angeboten.</summary>
    public string UpdateUebersprungen { get; set; }
    public DateTime ErsterStart { get; set; } = DateTime.Now;

    /// <summary>Lokaler App-Ordner (%AppData%\CSharpLernen): Einstellungen, Zeiger-Datei, fehler.log, eigene Lektionen.
    /// Der Name bleibt bewusst „CSharpLernen“ (alter App-Name), damit gespeicherter Fortschritt erhalten bleibt.</summary>
    public static string Ordner => Speicherort.StandardOrdner;
    /// <summary>Ordner des Lernstands (Standard oder z. B. OneDrive\SharpLab). Wird von <see cref="Laden"/> gesetzt.</summary>
    public static string LernstandOrdner { get; set; }
    public static string Datei => Speicherort.LernstandDatei(LernstandOrdner ?? Ordner);
    public static string EinstellungenDatei => Path.Combine(Ordner, "einstellungen.json");
    /// <summary>Meldung für den Start, z. B. wenn der gewählte Ordner nicht erreichbar war (dann gilt der Standard).</summary>
    public static string StartHinweis { get; set; }
    /// <summary>Wird einmal je Sitzung ausgelöst, wenn der Lernstand nicht gespeichert werden konnte.</summary>
    public static event Action<Exception> Speicherfehler;
    static bool speicherfehlerGemeldet;

    /// <summary>Diese Werte bleiben auf jedem Rechner lokal (einstellungen.json), nur der Rest wandert mit dem Lernstand.</summary>
    public static readonly string[] EinstellungsFelder =
        { nameof(Schriftgroesse), nameof(Zeitlimit), nameof(MatrixRegen), nameof(MatrixStufe), nameof(MatrixTempo), nameof(Autovervollstaendigung), nameof(AllesFreigeschaltet),
          nameof(UpdateBeimStart), nameof(UpdateUebersprungen) };

    static readonly JsonSerializerOptions Optionen = new() { WriteIndented = true };

    public static void Protokoll(Exception ex, string quelle) =>
        CSharpLernen.Engine.FehlerProtokoll.Schreiben(Path.Combine(Ordner, "fehler.log"), ex, quelle);

    /// <summary>Lädt Einstellungen (lokal) und Lernstand (Standard oder gewählter Ordner). Ist der gewählte Ordner
    /// nicht erreichbar, gilt wieder der Standard und <see cref="StartHinweis"/> erklärt das.</summary>
    public static Fortschritt Laden()
    {
        StartHinweis = null;
        string basis = Speicherort.LeseBasis();
        string ordner = Speicherort.LernstandOrdner(basis);
        if (basis != null && !Speicherort.IstBeschreibbar(ordner, out var fehler))
        {
            Protokoll(new IOException($"Lernstand-Ordner nicht erreichbar: {ordner} – {fehler}"), "Speicherort beim Start");
            StartHinweis = $"Der Ordner für deinen Lernstand ist gerade nicht erreichbar:\n{ordner}\n\nSharpLab benutzt deshalb wieder den Standard-Ordner. In den Einstellungen kannst du den Ordner neu wählen.";
            try { Speicherort.SchreibeBasis(null); } catch { }
            ordner = Speicherort.StandardOrdner;
        }
        LernstandOrdner = ordner;

        Fortschritt f = LadeDatei(Datei) ?? new Fortschritt();
        // Einstellungen: einstellungen.json; ältere Versionen hatten alles zusammen in der lokalen fortschritt.json.
        Fortschritt e = LadeDatei(EinstellungenDatei) ?? LadeDatei(Speicherort.LernstandDatei(Ordner));
        if (e != null) f.UebernimmEinstellungen(e);
        f.Bereinigen();
        return f;
    }

    /// <summary>Liest eine Datei; bei kaputtem Inhalt wird eine Kopie „.kaputt“ abgelegt und null geliefert.</summary>
    public static Fortschritt LadeDatei(string datei)
    {
        try
        {
            if (File.Exists(datei)) return AusJson(File.ReadAllText(datei));
        }
        catch (Exception ex)
        {
            Protokoll(ex, "Laden " + datei);
            try { File.Copy(datei, datei + ".kaputt", true); } catch { }
        }
        return null;
    }

    public void UebernimmEinstellungen(Fortschritt q)
    {
        foreach (var name in EinstellungsFelder)
        {
            var p = typeof(Fortschritt).GetProperty(name);
            p.SetValue(this, p.GetValue(q));
        }
    }

    /// <summary>Übernimmt alles außer den lokalen Einstellungen (z. B. nach „Diesen übernehmen“).</summary>
    public void UebernimmLernstand(Fortschritt q)
    {
        foreach (var p in typeof(Fortschritt).GetProperties().Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0 && !EinstellungsFelder.Contains(p.Name)))
            p.SetValue(this, p.GetValue(q));
        Bereinigen();
    }

    /// <summary>JSON des Lernstands (ohne lokale Einstellungen).</summary>
    public string LernstandJson()
    {
        var obj = JsonSerializer.SerializeToNode(this, Optionen).AsObject();
        foreach (var name in EinstellungsFelder) obj.Remove(name);
        return obj.ToJsonString(Optionen);
    }

    /// <summary>JSON nur der lokalen Einstellungen.</summary>
    public string EinstellungenJson()
    {
        var alles = JsonSerializer.SerializeToNode(this, Optionen).AsObject();
        var obj = new JsonObject();
        foreach (var name in EinstellungsFelder) obj[name] = alles[name]?.DeepClone();
        return obj.ToJsonString(Optionen);
    }

    /// <summary>Liest gespeicherten Fortschritt und bereinigt ihn (alte Dateien können leere oder fehlende Werte enthalten).</summary>
    public static Fortschritt AusJson(string json)
    {
        var f = JsonSerializer.Deserialize<Fortschritt>(json) ?? new Fortschritt();
        f.Bereinigen();
        return f;
    }

    /// <summary>Ersetzt null, leere oder unsinnige Werte durch Standardwerte, damit nichts davon die Oberfläche stört.</summary>
    public void Bereinigen()
    {
        GeloesteAufgaben = Ohne(GeloesteAufgaben);
        GeleseneLektionen = Ohne(GeleseneLektionen);
        Code = Code == null ? new() : Code.Where(kv => !string.IsNullOrWhiteSpace(kv.Key) && kv.Value != null).ToDictionary(kv => kv.Key, kv => kv.Value);
        QuizPunkte = QuizPunkte == null ? new() : QuizPunkte.Where(kv => !string.IsNullOrWhiteSpace(kv.Key)).ToDictionary(kv => kv.Key, kv => Math.Max(0, kv.Value));
        if (string.IsNullOrWhiteSpace(LetzteLektion)) LetzteLektion = null;
        Schriftgroesse = Zahl(Schriftgroesse, 10, 30, 15);
        Zeitlimit = Zeitlimit < 3 || Zeitlimit > 60 ? 10 : Zeitlimit;
        MatrixStufe = Zahl(MatrixStufe, 0, 1, 0.7);
        MatrixTempo = Zahl(MatrixTempo, 0.3, 2.5, 1.0);
        if (ErsterStart == default || ErsterStart > DateTime.Now.AddDays(1)) ErsterStart = DateTime.Now;
        Ausfuehrungen = Math.Max(0, Ausfuehrungen);
        ErfolgreicheLaeufe = Math.Max(0, ErfolgreicheLaeufe);
        SpielwieseLaeufe = Math.Max(0, SpielwieseLaeufe);
    }

    static HashSet<string> Ohne(HashSet<string> menge) =>
        menge == null ? new() : new HashSet<string>(menge.Where(s => !string.IsNullOrWhiteSpace(s)));

    /// <summary>Außerhalb des Bereichs oder NaN → Standardwert (bewusst nicht geklemmt: ein kaputter Wert soll nicht am Rand kleben).</summary>
    static double Zahl(double wert, double min, double max, double standard) =>
        double.IsNaN(wert) || double.IsInfinity(wert) || wert < min || wert > max ? standard : wert;

    /// <summary>Speichert Einstellungen lokal und den Lernstand am gewählten Ort – beides atomar (Temp-Datei + Replace).</summary>
    public void Speichern()
    {
        try { Speicherort.AtomarSchreiben(EinstellungenDatei, EinstellungenJson()); }
        catch (Exception ex) { Protokoll(ex, "Einstellungen speichern"); }
        try
        {
            Speicherort.AtomarSchreiben(Datei, LernstandJson());
        }
        catch (Exception ex)
        {
            Protokoll(ex, "Lernstand speichern");
            if (!speicherfehlerGemeldet) { speicherfehlerGemeldet = true; try { Speicherfehler?.Invoke(ex); } catch { } }
        }
    }
}
