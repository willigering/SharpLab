using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using CSharpLernen.Engine;
using Xunit;
using Xunit.Abstractions;

namespace CSharpLernen.Tests;

/// <summary>
/// Reiter „Freie Aufgaben“: Jede Aufgabe wird wie in der App geprüft (Roslyn, de-DE).
/// Musterlösung besteht, Startcode und typische Fehllösungen („wrong“ im JSON) fallen durch.
/// </summary>
public class FreieAufgabenTests
{
    readonly ITestOutputHelper log;
    public FreieAufgabenTests(ITestOutputHelper log) => this.log = log;

    static readonly FreieAufgabenKatalog Katalog = FreieAufgabenLader.Laden();

    static string RohJson()
    {
        using var s = typeof(FreieAufgabenLader).Assembly.GetManifestResourceStream(FreieAufgabenLader.RessourcenName)!;
        using var r = new StreamReader(s, Encoding.UTF8);
        return r.ReadToEnd();
    }

    /// <summary>Typische Fehllösungen je Aufgabe. Sie stehen nur zum Testen im JSON, die App liest sie nicht.</summary>
    static readonly Dictionary<string, List<string>> Fehlloesungen = LadeFehlloesungen();

    static Dictionary<string, List<string>> LadeFehlloesungen()
    {
        var d = new Dictionary<string, List<string>>();
        using var doc = JsonDocument.Parse(RohJson());
        foreach (var stufe in doc.RootElement.GetProperty("levels").EnumerateArray())
            foreach (var a in stufe.GetProperty("exercises").EnumerateArray())
                d[a.GetProperty("id").GetString()!] = a.TryGetProperty("wrong", out var w)
                    ? w.EnumerateArray().Select(x => x.GetString()!).ToList()
                    : new List<string>();
        return d;
    }

    public static IEnumerable<object[]> AlleAufgaben() => Katalog.Alle.Select(a => new object[] { a.Id });

    public static IEnumerable<object[]> AlleFehlloesungen() =>
        Fehlloesungen.SelectMany(kv => kv.Value.Select((_, i) => new object[] { kv.Key, i }));

    static Aufgabe Finde(string id) => Katalog.Alle.Single(a => a.Id == id);

    [Fact]
    public void Katalog_ist_vollstaendig()
    {
        Assert.Equal(new[] { "leicht", "mittel", "schwer" }, Katalog.Stufen.Select(s => s.Id));
        Assert.Equal(new[] { "Leicht", "Mittel", "Schwer" }, Katalog.Stufen.Select(s => s.Titel));
        var lektionsIds = KursLader.Laden().Lektionen.SelectMany(l => l.Aufgaben).Select(a => a.Id).ToHashSet();
        foreach (var s in Katalog.Stufen)
        {
            Assert.False(string.IsNullOrWhiteSpace(s.Beschreibung), s.Id);
            Assert.InRange(s.Aufgaben.Count, 10, 15);
            foreach (var a in s.Aufgaben)
            {
                Assert.Matches(@"^f[lms]\d\d$", a.Id);
                Assert.DoesNotContain(a.Id, lektionsIds);
                Assert.False(string.IsNullOrWhiteSpace(a.Titel), a.Id + " Titel");
                Assert.False(string.IsNullOrWhiteSpace(a.Thema), a.Id + " Thema");
                Assert.False(string.IsNullOrWhiteSpace(a.Aufgabentext), a.Id + " Text");
                Assert.False(string.IsNullOrWhiteSpace(a.Tipp), a.Id + " Tipp");
                Assert.False(string.IsNullOrWhiteSpace(a.Loesung), a.Id + " Lösung");
                Assert.False(string.IsNullOrWhiteSpace(a.Startcode), a.Id + " Startcode");
                Assert.Contains(a.Vergleich, new[] { "exact", "lenient" });
                Assert.NotEmpty(a.Tests);
                Assert.NotEmpty(Fehlloesungen[a.Id]);
            }
        }
        var ids = Katalog.Alle.Select(a => a.Id).ToList();
        Assert.Equal(ids.Count, ids.Distinct().Count());
        Assert.Equal(Katalog.Alle.Count(), Katalog.Alle.Select(a => a.Titel).Distinct().Count());
    }

    [Theory]
    [MemberData(nameof(AlleAufgaben))]
    public async Task Musterloesung_besteht(string id)
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
        // Keine „fast richtig“-Hinweise: Die Musterlösung trifft die erwartete Ausgabe genau.
        foreach (var t in a.Tests)
        {
            var lauf = await CodeAusfuehrer.AusfuehrenAsync(a.Loesung, new Laufoptionen { Eingabe = t.Eingabe, Zeitlimit = TimeSpan.FromSeconds(5) });
            var v = AufgabenPruefer.Vergleichen(t.Erwartet, lauf.Ausgabe, a.Vergleich == "lenient");
            Assert.True(v.Gleich && v.Schoenheitshinweis == null, $"{id}: {v.Erklaerung}{v.Schoenheitshinweis}");
        }
    }

    [Theory]
    [MemberData(nameof(AlleAufgaben))]
    public async Task Startcode_besteht_nicht(string id)
    {
        var a = Finde(id);
        var r = await AufgabenPruefer.PruefenAsync(a, a.Startcode);
        Assert.False(r.Bestanden, id + ": der Startcode darf die Aufgabe noch nicht lösen");
        // Vorgegebener Startcode (Gerüst) soll fehlerfrei kompilieren, damit niemand mit roten Fehlern startet.
        if (!a.Startcode.TrimStart().StartsWith("//"))
            Assert.NotEqual(Ausgang.Kompilierfehler, r.Lauf?.Ausgang);
    }

    [Theory]
    [MemberData(nameof(AlleFehlloesungen))]
    public async Task Typische_Fehlloesung_faellt_durch(string id, int nr)
    {
        var a = Finde(id);
        string code = Fehlloesungen[id][nr];
        var kompilat = CodeAusfuehrer.Kompilieren(code);
        Assert.True(kompilat.Ok, $"{id} Fehllösung {nr + 1} soll kompilieren: " + string.Join("; ", kompilat.Meldungen.Select(m => m.Anzeige)));
        var r = await AufgabenPruefer.PruefenAsync(a, code);
        Assert.False(r.Bestanden, $"{id}: Fehllösung {nr + 1} wurde fälschlich akzeptiert ({r.Details})");
    }

    /// <summary>
    /// Die Prüfung verzeiht kleine Tippfehler und Zeilenanfänge (nachsichtiger Vergleich). Deshalb darf keine erwartete Zeile
    /// einer anderen Aufgabenzeile so ähnlich sein, dass sie als „fast richtig“ durchginge (z. B. „eine“/„keine“, „gerade“/„ungerade“).
    /// Zahlen werden dafür gleichgesetzt, weil sie ohnehin streng verglichen werden.
    /// </summary>
    [Theory]
    [MemberData(nameof(AlleAufgaben))]
    public void Erwartete_Zeilen_sind_trennscharf(string id)
    {
        var a = Finde(id);
        var zeilen = a.Tests.SelectMany(t => AufgabenPruefer.Zeilen(t.Erwartet))
            .Select(z => Regex.Replace(z, @"-?\d+", "0")).Where(z => z.Length > 0).Distinct().ToList();
        var fehler = new List<string>();
        foreach (var x in zeilen)
            foreach (var y in zeilen)
                if (x != y && OhneLeer(x) != OhneLeer(y) && AufgabenPruefer.Vergleichen(x, y, a.Vergleich == "lenient").Gleich)
                    fehler.Add($"«{y}» ginge als «{x}» durch");
        Assert.True(fehler.Count == 0, id + ": " + string.Join("; ", fehler));
    }

    /// <summary>Reine Leerzeichen-Unterschiede verzeiht die App absichtlich (mit freundlichem Hinweis).</summary>
    static string OhneLeer(string s) => new(s.Where(c => !char.IsWhiteSpace(c)).ToArray());

    /// <summary>Rechtschreibung: echte Umlaute und ß statt Ersatzschreibweisen, keine doppelten Leerzeichen.</summary>
    [Fact]
    public void Texte_mit_echten_Umlauten()
    {
        string[] ersatz =
        {
            @"\bfuer\b", @"\bueber", @"\bUeber", @"\b[Gg]roess", @"\b[Gg]ross\b", @"\bUebung", @"\b[Ll]oesung", @"\b[Nn]aechst", @"\b[Mm]oeglich",
            @"\bkoenn", @"\bmuess", @"\b[Zz]urueck", @"\b[Ww]aehl", @"\b[Ss]tueck", @"\b[Hh]oechst", @"\b[Zz]aehl", @"\b[Ll]aeng", @"\b[Pp]ruef",
            @"\bheisst\b", @"\bweiss\b", @"\b[Ss]chluessel", @"\b[Bb]uchstab\w*ae", @"\bausser", @"\bgemaess", @"\b[Ww]uerf", @"\bMaedchen",
        };
        var fehler = new List<string>();
        foreach (var s in Katalog.Stufen)
            Pruefe(s.Id, s.Titel + " " + s.Beschreibung);
        foreach (var a in Katalog.Alle)
        {
            // Code (``` … ``` und `…`) ausnehmen: Bezeichner wie „Zaehler“ sind dort richtig.
            string text = Regex.Replace(a.Aufgabentext, "```[\\s\\S]*?```", "CODE");
            text = Regex.Replace(text, "`[^`]*`", "CODE");
            Pruefe(a.Id, a.Titel + " " + a.Thema + " " + text + " " + a.Tipp + " " + string.Join(" ", a.Anforderungen.Select(r => Regex.Replace(r.Meldung, "`[^`]*`", "CODE"))));
        }
        Assert.True(fehler.Count == 0, string.Join(Environment.NewLine, fehler));

        void Pruefe(string id, string text)
        {
            foreach (var m in ersatz)
                if (Regex.IsMatch(text, m)) fehler.Add($"{id}: Ersatzschreibweise ({m})");
            if (Regex.IsMatch(text, @"[^\s] {2,}[^\s]")) fehler.Add($"{id}: doppeltes Leerzeichen");
            if (Regex.IsMatch(text, @"\s[,.;:!?](\s|$)")) fehler.Add($"{id}: Leerzeichen vor Satzzeichen");
            if (text.Contains('"')) fehler.Add($"{id}: gerade Anführungszeichen im Fließtext (bitte „…“)");
        }
    }

    // ---------------- Erledigt-Logik ----------------

    static FreieStufe TestStufe(int n) => new() { Id = "t", Titel = "Test", Aufgaben = Enumerable.Range(1, n).Select(i => new Aufgabe { Id = "a" + i }).ToList() };

    [Fact]
    public void Start_ist_erste_offene_Aufgabe()
    {
        var s = TestStufe(4);
        Assert.Equal("a1", FreieAufgabenStatus.Start(s, new HashSet<string>())!.Id);
        Assert.Equal("a3", FreieAufgabenStatus.Start(s, new HashSet<string> { "a1", "a2" })!.Id);
        Assert.Equal("a1", FreieAufgabenStatus.Start(s, new HashSet<string> { "a1", "a2", "a3", "a4" })!.Id);
        Assert.Null(FreieAufgabenStatus.Start(TestStufe(0), new HashSet<string>()));
    }

    [Fact]
    public void Ueberspringen_fuehrt_zur_naechsten_offenen_Aufgabe()
    {
        var s = TestStufe(5);
        var erledigt = new HashSet<string> { "a2", "a3" };
        Assert.Equal("a4", FreieAufgabenStatus.Naechste(s, s.Aufgaben[0], erledigt)!.Id);   // erledigte werden übersprungen
        Assert.Equal("a1", FreieAufgabenStatus.Naechste(s, s.Aufgaben[4], erledigt)!.Id);   // am Ende geht es vorn weiter
        Assert.Equal("a5", FreieAufgabenStatus.Naechste(s, s.Aufgaben[3], erledigt)!.Id);
        // Alles erledigt: einfach der Reihe nach weiter
        var alle = s.Aufgaben.Select(a => a.Id).ToHashSet();
        Assert.Equal("a3", FreieAufgabenStatus.Naechste(s, s.Aufgaben[1], alle)!.Id);
        Assert.Equal("a1", FreieAufgabenStatus.Naechste(s, s.Aufgaben[4], alle)!.Id);
        Assert.Null(FreieAufgabenStatus.Naechste(TestStufe(1), TestStufe(1).Aufgaben[0], erledigt));
    }

    [Fact]
    public void Erledigt_zaehlt_nur_Aufgaben_der_Stufe()
    {
        var s = TestStufe(3);
        var erledigt = new HashSet<string> { "a1", "a3", "fremd" };
        Assert.Equal(2, FreieAufgabenStatus.Erledigt(s, erledigt));
        Assert.False(FreieAufgabenStatus.AlleErledigt(s, erledigt));
        erledigt.Add("a2");
        Assert.True(FreieAufgabenStatus.AlleErledigt(s, erledigt));
    }

    [Fact]
    public void Erledigte_freie_Aufgaben_werden_gespeichert_und_geladen()
    {
        var f = new Fortschritt();
        f.ErledigteFreieAufgaben.Add("fl01");
        f.ErledigteFreieAufgaben.Add("fs12");
        f.LetzteFreieAufgabe = "fs12";
        var geladen = Fortschritt.AusJson(f.LernstandJson());
        Assert.Equal(new[] { "fl01", "fs12" }, geladen.ErledigteFreieAufgaben.OrderBy(x => x));
        Assert.Equal("fs12", geladen.LetzteFreieAufgabe);
        // Freie Aufgaben zählen nicht als gelöste Lektionsaufgaben (Fortschritt und Erfolge der Lektionen bleiben unverändert).
        Assert.Empty(geladen.GeloesteAufgaben);
    }

    [Fact]
    public void Alter_Lernstand_ohne_freie_Aufgaben_laedt()
    {
        var f = Fortschritt.AusJson("{\"GeloesteAufgaben\":[\"l01a1\"]}");
        Assert.NotNull(f.ErledigteFreieAufgaben);
        Assert.Empty(f.ErledigteFreieAufgaben);
        var g = Fortschritt.AusJson("{\"ErledigteFreieAufgaben\":null,\"LetzteFreieAufgabe\":\"  \"}");
        Assert.NotNull(g.ErledigteFreieAufgaben);
        Assert.Null(g.LetzteFreieAufgabe);
    }
}
