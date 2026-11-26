using System.Text.Json;
using System.Text.Json.Serialization;

namespace CSharpLernen.Engine;

/// <summary>Alle freien Aufgaben (Reiter „Freie Aufgaben“), nach Schwierigkeit gruppiert.</summary>
public sealed class FreieAufgabenKatalog
{
    [JsonPropertyName("levels")] public List<FreieStufe> Stufen { get; set; } = new();

    public IEnumerable<Aufgabe> Alle => Stufen.SelectMany(s => s.Aufgaben);

    public FreieStufe? StufeVon(Aufgabe a) => Stufen.FirstOrDefault(s => s.Aufgaben.Contains(a));
}

/// <summary>Eine Schwierigkeit (Leicht, Mittel, Schwer) mit ihren Aufgaben.</summary>
public sealed class FreieStufe
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
    [JsonPropertyName("title")] public string Titel { get; set; } = "";
    [JsonPropertyName("description")] public string Beschreibung { get; set; } = "";
    [JsonPropertyName("exercises")] public List<Aufgabe> Aufgaben { get; set; } = new();

    public override string ToString() => Titel;
}

public static class FreieAufgabenLader
{
    public const string RessourcenName = "CSharpLernen.FreieAufgaben.json";

    static readonly JsonSerializerOptions Optionen = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    /// <summary>Lädt die eingebetteten freien Aufgaben.</summary>
    public static FreieAufgabenKatalog Laden()
    {
        using var stream = typeof(FreieAufgabenLader).Assembly.GetManifestResourceStream(RessourcenName)
            ?? throw new InvalidOperationException("Eingebettete freie Aufgaben nicht gefunden.");
        using var reader = new StreamReader(stream);
        return AusJson(reader.ReadToEnd());
    }

    public static FreieAufgabenKatalog AusJson(string json) =>
        JsonSerializer.Deserialize<FreieAufgabenKatalog>(json, Optionen) ?? new FreieAufgabenKatalog();
}

/// <summary>
/// Regeln für den Reiter „Freie Aufgaben“: Was als erledigt zählt und wohin „Überspringen“ führt.
/// Erledigt bleibt erledigt, auch wenn die Aufgabe später noch einmal gemacht wird.
/// </summary>
public static class FreieAufgabenStatus
{
    public static int Erledigt(FreieStufe stufe, ISet<string> erledigt) => stufe.Aufgaben.Count(a => erledigt.Contains(a.Id));

    public static bool AlleErledigt(FreieStufe stufe, ISet<string> erledigt) =>
        stufe.Aufgaben.Count > 0 && stufe.Aufgaben.All(a => erledigt.Contains(a.Id));

    /// <summary>Beim Öffnen einer Schwierigkeit: die erste offene Aufgabe, sonst die erste überhaupt.</summary>
    public static Aufgabe? Start(FreieStufe stufe, ISet<string> erledigt) =>
        stufe.Aufgaben.FirstOrDefault(a => !erledigt.Contains(a.Id)) ?? stufe.Aufgaben.FirstOrDefault();

    /// <summary>
    /// „Überspringen“: die nächste offene Aufgabe hinter der aktuellen (am Ende geht es vorn weiter).
    /// Ist alles erledigt, einfach die nächste Aufgabe der Reihe nach. Gibt es nur eine Aufgabe: null.
    /// </summary>
    public static Aufgabe? Naechste(FreieStufe stufe, Aufgabe aktuelle, ISet<string> erledigt)
    {
        var liste = stufe.Aufgaben;
        int i = liste.IndexOf(aktuelle);
        if (liste.Count < 2 || i < 0) return null;
        for (int schritt = 1; schritt < liste.Count; schritt++)
        {
            var kandidat = liste[(i + schritt) % liste.Count];
            if (!erledigt.Contains(kandidat.Id)) return kandidat;
        }
        return liste[(i + 1) % liste.Count];
    }
}
