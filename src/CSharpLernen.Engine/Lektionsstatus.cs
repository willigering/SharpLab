namespace CSharpLernen.Engine;

/// <summary>
/// Regeln für „Lektion abgeschlossen“ und „Nächste Lektion“ (ab 1.0.5: 8 Aufgaben je Lektion).
/// Eine Lektion gilt ab <see cref="Schwelle"/> gelösten Aufgaben als abgeschlossen und schaltet die nächste frei;
/// „komplett“ ist sie erst, wenn alle Aufgaben gelöst sind.
/// Kompatibilität: Wer vor 1.0.5 alle damaligen Aufgaben einer Lektion gelöst hatte (3 oder 4 Stück), behält „abgeschlossen“.
/// </summary>
public static class Lektionsstatus
{
    public const int Schwelle = 5;

    /// <summary>Anzahl Aufgaben je Lektion bis einschließlich 1.0.5 (vor der Erweiterung auf 8). Die alten IDs blieben gleich,
    /// neue wurden hinten angehängt – deshalb reicht „die ersten n Aufgaben“.</summary>
    public static readonly IReadOnlyDictionary<string, int> AlteAnzahl = new Dictionary<string, int>
    {
        ["l01"] = 3, ["l02"] = 3, ["l03"] = 3, ["l04"] = 4, ["l05"] = 3, ["l06"] = 4, ["l07"] = 3, ["l08"] = 3, ["l09"] = 3,
        ["l10"] = 3, ["l11"] = 4, ["l12"] = 4, ["l13"] = 4, ["l14"] = 3, ["l15"] = 3, ["l16"] = 3, ["l17"] = 4, ["l18"] = 3,
        ["l19"] = 3, ["l20"] = 3, ["l21"] = 3, ["l22"] = 3, ["l23"] = 3, ["l24"] = 3, ["l25"] = 3, ["l26"] = 3, ["l27"] = 3,
    };

    public static int Geloest(Lektion l, ISet<string> geloest) => l.Aufgaben.Count(a => geloest.Contains(a.Id));

    /// <summary>Wie viele gelöste Aufgaben zum Abschließen nötig sind (bei Lektionen mit weniger Aufgaben: alle).</summary>
    public static int Noetig(Lektion l) => Math.Min(Schwelle, l.Aufgaben.Count);

    public static bool IstKomplett(Lektion l, ISet<string> geloest) => l.Aufgaben.Count > 0 && l.Aufgaben.All(a => geloest.Contains(a.Id));

    static bool AltFertig(Lektion l, ISet<string> geloest) =>
        AlteAnzahl.TryGetValue(l.Id, out int n) && n > 0 && l.Aufgaben.Count >= n && l.Aufgaben.Take(n).All(a => geloest.Contains(a.Id));

    public static bool IstAbgeschlossen(Lektion l, ISet<string> geloest) =>
        l.Aufgaben.Count > 0 && (Geloest(l, geloest) >= Noetig(l) || AltFertig(l, geloest));

    /// <summary>„Nächste Lektion“ im Übungsbereich: unsichtbar, bis die Lektion abgeschlossen ist.</summary>
    public static bool NaechsteSichtbar(Lektion l, ISet<string> geloest) => l.Aufgaben.Count == 0 || IstAbgeschlossen(l, geloest);

    /// <summary>Grün hervorgehoben erst, wenn alle Aufgaben gelöst sind.</summary>
    public static bool NaechsteHervorgehoben(Lektion l, ISet<string> geloest) => l.Aufgaben.Count == 0 || IstKomplett(l, geloest);

    public static bool IstFreigeschaltet(IReadOnlyList<Lektion> lektionen, int index, ISet<string> geloest, bool allesFrei) =>
        allesFrei || index <= 0 || IstAbgeschlossen(lektionen[index - 1], geloest) || (index < lektionen.Count && Geloest(lektionen[index], geloest) > 0);
}
