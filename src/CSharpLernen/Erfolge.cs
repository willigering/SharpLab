using CSharpLernen.Engine;

namespace CSharpLernen;

public sealed record Erfolg(string Id, string Titel, string Beschreibung, string Icon, Func<Fortschritt, Kurs, bool> Bedingung);

public static class Erfolge
{
    static bool StufeFertig(Fortschritt f, Kurs k, int stufe) =>
        k.Lektionen.Where(l => l.Stufe == stufe).SelectMany(l => l.Aufgaben).All(a => f.GeloesteAufgaben.Contains(a.Id));

    static int AlleAufgaben(Kurs k) => k.Lektionen.Sum(l => l.Aufgaben.Count);

    public static readonly IReadOnlyList<Erfolg> Alle = new List<Erfolg>
    {
        new("hallo", "Hallo, Welt!", "Dein erstes Programm erfolgreich ausgeführt.", "\uE768", (f, k) => f.ErfolgreicheLaeufe >= 1),
        new("erste", "Erste Übung", "Deine erste Aufgabe richtig gelöst.", "\uE73E", (f, k) => f.GeloesteAufgaben.Count >= 1),
        new("fehlerjaeger", "Fehlerjäger", "Einen Kompilierfehler gefunden und behoben.", "\uEBE8", (f, k) => f.FehlerBehoben),
        new("endlos", "Endlos? Nicht mit mir!", "Eine Endlosschleife wurde gestoppt – passiert jedem mal.", "\uE895", (f, k) => f.EndlosschleifeGestoppt),
        new("quiz", "Quiz-Starter", "Ein Quiz komplett richtig beantwortet.", "\uE9CE", (f, k) => k.Lektionen.Any(l => l.Quiz.Count > 0 && f.QuizPunkte.GetValueOrDefault(l.Id) == l.Quiz.Count)),
        new("quiz5", "Quiz-Ass", "5 Quizze komplett richtig beantwortet.", "\uE9CE", (f, k) => k.Lektionen.Count(l => l.Quiz.Count > 0 && f.QuizPunkte.GetValueOrDefault(l.Id) == l.Quiz.Count) >= 5),
        new("zehn", "Fleißig", "10 Aufgaben gelöst.", "\uE8FB", (f, k) => f.GeloesteAufgaben.Count >= 10),
        new("halb", "Halbzeit", "Die Hälfte aller Aufgaben gelöst.", "\uE9D2", (f, k) => f.GeloesteAufgaben.Count * 2 >= AlleAufgaben(k)),
        new("spiel", "Spielkind", "5 Programme auf der Spielwiese ausgeführt.", "\uE7FC", (f, k) => f.SpielwieseLaeufe >= 5),
        new("hundert", "Dauerläufer", "100 Programme ausgeführt.", "\uE945", (f, k) => f.Ausfuehrungen >= 100),
        new("stufe1", "Grundlagen gemeistert", "Stufe 1 komplett abgeschlossen.", "\uE8F1", (f, k) => StufeFertig(f, k, 1)),
        new("stufe2", "Herr der Schleifen", "Stufe 2 (Kontrollstrukturen) abgeschlossen.", "\uE8EE", (f, k) => StufeFertig(f, k, 2)),
        new("stufe3", "Methodiker", "Stufe 3 (Methoden) abgeschlossen.", "\uE943", (f, k) => StufeFertig(f, k, 3)),
        new("stufe4", "Objekt-Architekt", "Stufe 4 (Klassen & Objekte) abgeschlossen.", "\uE8B7", (f, k) => StufeFertig(f, k, 4)),
        new("stufe5", "Projekt-Profi", "Alle Projekte abgeschlossen.", "\uE7C1", (f, k) => StufeFertig(f, k, 5)),
        new("alles", "C#-Profi", "Wirklich alle Aufgaben gelöst. Wahnsinn!", "\uE734", (f, k) => f.GeloesteAufgaben.Count >= AlleAufgaben(k) && AlleAufgaben(k) > 0),
    };

    public static HashSet<string> Erreicht(Fortschritt f, Kurs k) =>
        Alle.Where(e => e.Bedingung(f, k)).Select(e => e.Id).ToHashSet();
}
