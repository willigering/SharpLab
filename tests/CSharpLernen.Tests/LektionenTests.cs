using CSharpLernen.Engine;
using Xunit;
using Xunit.Abstractions;

namespace CSharpLernen.Tests;

public class LektionenTests
{
    readonly ITestOutputHelper log;
    public LektionenTests(ITestOutputHelper log) => this.log = log;

    static readonly Kurs Kurs = KursLader.Laden();

    public static IEnumerable<object[]> AlleAufgaben() =>
        Kurs.Lektionen.SelectMany(l => l.Aufgaben.Select(a => new object[] { a.Id }));

    static Aufgabe Finde(string id) => Kurs.Lektionen.SelectMany(l => l.Aufgaben).Single(a => a.Id == id);

    [Fact]
    public void Kurs_ist_vollstaendig()
    {
        Assert.True(Kurs.Lektionen.Count >= 25, "mindestens 25 Lektionen");
        Assert.Equal(5, Kurs.Stufen.Count);
        Assert.True(Kurs.Begriffe.Count >= 25);
        foreach (var l in Kurs.Lektionen)
        {
            Assert.False(string.IsNullOrWhiteSpace(l.Erklaerung), l.Id);
            Assert.Equal(8, l.Aufgaben.Count);
            Assert.True(l.Quiz.Count >= 2, l.Id + " Quiz");
            Assert.Contains(Kurs.Stufen, s => s.Nummer == l.Stufe);
            foreach (var q in l.Quiz) Assert.InRange(q.Richtig, 0, q.Antworten.Count - 1);
            foreach (var a in l.Aufgaben) Assert.NotEmpty(a.Tests);
        }
        Assert.Equal(Kurs.Lektionen.Count, Kurs.Lektionen.Select(l => l.Id).Distinct().Count());
    }

    [Theory]
    [MemberData(nameof(AlleAufgaben))]
    public async Task Musterloesung_besteht_eigene_Pruefung(string id)
    {
        var a = Finde(id);
        var r = await AufgabenPruefer.PruefenAsync(a, a.Loesung);
        if (!r.Bestanden)
        {
            log.WriteLine(r.Titel + "\n" + r.Details);
            foreach (var m in r.Lauf?.Meldungen ?? Array.Empty<CompilerMeldung>()) log.WriteLine(m.Anzeige);
            log.WriteLine("Ausgabe:\n" + r.TatsaechlicheAusgabe);
        }
        Assert.True(r.Bestanden, $"{id}: {r.Titel} – {r.Details}");
        Assert.NotNull(r.Lauf);
        var letzter = a.Tests[^1];
        Assert.True(
            AufgabenPruefer.Vergleichen(letzter.Erwartet, r.Lauf!.Ausgabe, a.Vergleich == "lenient").Gleich,
            $"{id}: die zurückgegebene Ausgabe passt nicht zum letzten Test.\n{r.Lauf.Ausgabe}");
    }

    [Theory]
    [MemberData(nameof(AlleAufgaben))]
    public async Task Startcode_besteht_nicht(string id)
    {
        var a = Finde(id);
        var r = await AufgabenPruefer.PruefenAsync(a, a.Startcode);
        Assert.False(r.Bestanden, id + ": der Startcode darf die Aufgabe noch nicht lösen");
    }

    [Fact]
    public async Task Alle_Beispiele_laufen_fehlerfrei()
    {
        foreach (var l in Kurs.Lektionen)
        {
            var r = await CodeAusfuehrer.AusfuehrenAsync(l.Beispiel, new Laufoptionen { Eingabe = l.BeispielEingabe, Zeitlimit = TimeSpan.FromSeconds(5) });
            Assert.True(r.Erfolgreich, $"{l.Id}: {r.Ausgang} {r.FehlerText} {string.Join("; ", r.Meldungen.Select(m => m.Anzeige))}");
        }
    }
}
