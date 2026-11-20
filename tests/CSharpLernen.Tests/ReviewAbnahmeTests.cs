using System.Text;
using System.Text.RegularExpressions;
using CSharpLernen.Engine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using Xunit.Abstractions;

namespace CSharpLernen.Tests;

/// <summary>
/// Abnahme-Prüfungen aus der Inhalts-Review (Nov 2026). Wiederholbar: dotnet test --filter ReviewAbnahme
/// </summary>
public class ReviewAbnahmeTests
{
    readonly ITestOutputHelper log;
    public ReviewAbnahmeTests(ITestOutputHelper log) => this.log = log;

    static readonly string Json = LadeJson();
    static readonly Kurs Kurs = KursLader.AusJson(Json);

    /// <summary>Reine Ausgabe-Übungen: l01a1/l01a2/l04a1/l07a2 wie bisher, dazu l01a6 (Sternen-Dreieck) und l01a7 (Visitenkarte) –
    /// in Lektion 1 ist das Abtippen der Ausgabe selbst der Lerninhalt.</summary>
    static readonly HashSet<string> NurAusgabeErlaubt = new() { "l01a1", "l01a2", "l01a6", "l01a7", "l04a1", "l07a2" };

    static string LadeJson()
    {
        using var s = typeof(KursLader).Assembly.GetManifestResourceStream(KursLader.RessourcenName)!;
        using var r = new StreamReader(s, Encoding.UTF8);
        return r.ReadToEnd();
    }

    static IEnumerable<Aufgabe> Alle => Kurs.Lektionen.SelectMany(l => l.Aufgaben);
    static Aufgabe A(string id) => Alle.Single(a => a.Id == id);
    static Lektion L(string id) => Kurs.Lektionen.Single(l => l.Id == id);

    public const string StandardStartcode = "// Schreib hier deinen Code";

    /// <summary>Startcode, der Zeilen der Lösung enthalten darf, weil sie zwingend Material der Aufgabe sind
    /// (gleiche Liste wie startcode.py im Generator).</summary>
    public static readonly Dictionary<string, string> StartcodeMaterial = new()
    {
        ["l02a1"] = "Kommentar-Übung: die Zeilen sind das Material",
        ["l02a2"] = "Kommentar-Übung: die Zeile ist das Material",
        ["l02a3"] = "Kommentar-Übung: die Zeilen sind das Material",
        ["l07a1"] = "Der klassische Aufbau ist das Material – ergänzt wird nur der Befehl",
        ["l07a2"] = "Reparatur-Übung: das kaputte Programm ist das Material",
        ["l13a3"] = "gegebene Daten (Array)",
        ["l14a1"] = "gegebene Daten (Array)",
        ["l14a2"] = "gegebene Daten (Array)",
        ["l15a3"] = "gegebene Daten (Gästeliste)",
        ["l24a2"] = "gegebene Daten (Produktliste mit Klasse)",
        ["l16a2"] = "Testcode ruft die zu schreibende Methode auf",
        ["l16a3"] = "Testcode ruft die zu schreibende Methode auf",
        ["l17a1"] = "Testcode ruft die zu schreibende Methode auf",
        ["l21a1"] = "Testcode nutzt die zu schreibende Klasse",
        ["l21a3"] = "Testcode nutzt die zu schreibende Klasse",
        ["l22a2"] = "Testcode nutzt die zu schreibende Klasse",
        ["l22a3"] = "Testcode nutzt die zu schreibende Klasse",
        ["l23a2"] = "Testcode nutzt die zu schreibende Klasse",
        ["l22a1"] = "vorgegebene Klasse, die ergänzt wird",
        ["l23a1"] = "vorgegebene Klasse, die ergänzt wird",
        ["l23a3"] = "vorgegebene Klasse, die ergänzt wird",
        ["l24a1"] = "vorgegebene Klasse und Liste, die befüllt wird",
        ["l24a3"] = "vorgegebene Klasse und Liste, die befüllt wird",
        ["l27a3"] = "vorgegebene Klasse und Liste",
        ["l27a2"] = "Fortsetzung von l27a1",
    };

    /// <summary>Zeilen ohne Kommentare und ohne Leerraum; reine Klammerzeilen zählen nicht.</summary>
    public static HashSet<string> NormierteZeilen(string code)
    {
        string ohne = Regex.Replace(code.Replace("\r", ""), @"/\*[\s\S]*?\*/", "");
        return ohne.Split('\n')
            .Select(z => Regex.Replace(Regex.Replace(z, @"//.*$", ""), @"\s+", ""))
            .Where(z => z.Length > 0 && !Regex.IsMatch(z, @"^[{}();,]+$"))
            .ToHashSet();
    }

    /// <summary>Willi (1.0.5): „ich habe noch nichts eingetragen, aber die konsole ist vorausgefüllt.“
    /// Kein Startcode enthält eine Zeile der Lösung (normalisiert verglichen) – außer bei Material (Liste oben).</summary>
    [Fact]
    public void Startcode_enthaelt_keine_Zeile_der_Loesung()
    {
        var treffer = new List<string>();
        foreach (var a in Alle)
        {
            var gemeinsam = NormierteZeilen(a.Startcode).Intersect(NormierteZeilen(a.Loesung)).ToList();
            if (gemeinsam.Count > 0 && !StartcodeMaterial.ContainsKey(a.Id))
                treffer.Add($"{a.Id}: {string.Join(" | ", gemeinsam)}");
        }
        Assert.True(treffer.Count == 0, string.Join(Environment.NewLine, treffer));
    }

    [Fact]
    public void Ohne_Material_steht_nur_der_Standard_Startcode()
    {
        foreach (var a in Alle.Where(a => !StartcodeMaterial.ContainsKey(a.Id)))
            Assert.True(a.Startcode.Trim() == StandardStartcode, $"{a.Id}: Startcode ist nicht der Standard");
    }

    [Fact]
    public void Material_Ausnahmen_sind_noch_noetig()
    {
        foreach (var (id, grund) in StartcodeMaterial)
        {
            Assert.False(string.IsNullOrWhiteSpace(grund));
            var a = A(id);
            Assert.NotEqual(StandardStartcode, a.Startcode.Trim());
        }
        Assert.True(StartcodeMaterial.Count <= 25, "Die Ausnahmeliste soll kurz bleiben");
    }

    [Fact]
    public void Json_ist_valide_und_laedt()
    {
        using var _ = System.Text.Json.JsonDocument.Parse(Json);
        Assert.Equal(27, Kurs.Lektionen.Count);
        Assert.Equal(216, Alle.Count());
        Assert.All(Kurs.Lektionen, l => Assert.Equal(8, l.Aufgaben.Count));
    }

    static string Literal(string s) => "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", "\\n") + "\"";

    async Task<List<string>> BestehtMitFesterAusgabe(Func<string, string> programm)
    {
        var bestanden = new List<string>();
        foreach (var a in Alle)
        {
            var r = await AufgabenPruefer.PruefenAsync(a, programm(a.Tests[0].Erwartet));
            if (r.Bestanden) bestanden.Add(a.Id);
        }
        bestanden.Sort(StringComparer.Ordinal);
        return bestanden;
    }

    /// <summary>Ein Programm, das nur die erwartete Ausgabe des ersten Tests druckt (eine Zeile pro WriteLine),
    /// darf nur noch bei den reinen Ausgabe-Übungen (NurAusgabeErlaubt) bestehen.</summary>
    [Fact]
    public async Task Fest_eingetippte_Ausgabe_besteht_nur_bei_den_vier_Ausnahmen()
    {
        var bestanden = await BestehtMitFesterAusgabe(erwartet =>
            string.Concat(erwartet.Replace("\r", "").Split('\n').Select(z => $"Console.WriteLine({Literal(z)});\n")));
        log.WriteLine("Besteht mit fester Ausgabe (je Zeile ein WriteLine): " + string.Join(", ", bestanden));
        Assert.Equal(NurAusgabeErlaubt.OrderBy(x => x, StringComparer.Ordinal), bestanden);
    }

    /// <summary>Gleiches mit einem einzigen WriteLine und \n. Hier besteht zusätzlich l05a3 –
    /// dort IST genau das die verlangte Lösung („mit einem einzigen WriteLine und \n“).</summary>
    [Fact]
    public async Task Fest_eingetippte_Ausgabe_als_ein_Text()
    {
        var bestanden = await BestehtMitFesterAusgabe(erwartet => $"Console.WriteLine({Literal(erwartet)});\n");
        log.WriteLine("Besteht mit fester Ausgabe (ein Text mit \\n): " + string.Join(", ", bestanden));
        Assert.Equal(NurAusgabeErlaubt.Append("l05a3").OrderBy(x => x, StringComparer.Ordinal), bestanden);
    }

    /// <summary>Ausnahmen für den Startcode-Angriff: reine Ausgabe-Übungen, l05a3 (dort ist der feste Text mit \n die Lösung)
    /// und l07a1 („schreib die Ausgabe in Main“ – der eingefügte WriteLine IST die Lösung).</summary>
    static readonly HashSet<string> AngriffErlaubt = new(NurAusgabeErlaubt) { "l05a3", "l07a1" };

    /// <summary>Anforderungen, die nur als Leitplanke dienen (Startcode nicht löschen / Write statt WriteLine) –
    /// bei diesen Übungen ist die Ausgabe selbst der Lerninhalt.</summary>
    static readonly HashSet<string> LeitplankenAnforderung = new() { "l01a3", "l02a1", "l07a1" };

    /// <summary>Startcode unverändert lassen und die erwartete Ausgabe des ersten Tests fest einbauen –
    /// am Ende (bzw. am Anfang) von Main oder der Top-Level-Anweisungen.</summary>
    static string Angriff(string starter, string erwartet, bool amAnfang)
    {
        string zeilen = string.Concat(erwartet.Replace("\r", "").Split('\n').Select(z => $"Console.WriteLine({Literal(z)});\n"));
        var wurzel = (CompilationUnitSyntax)CSharpSyntaxTree.ParseText(starter).GetRoot();
        int pos;
        var main = wurzel.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault(m => m.Identifier.Text == "Main" && m.Body != null);
        var global = wurzel.Members.OfType<GlobalStatementSyntax>().ToList();
        if (main != null) pos = amAnfang ? main.Body!.OpenBraceToken.Span.End : main.Body!.CloseBraceToken.SpanStart;
        else if (global.Count > 0) pos = amAnfang ? global[0].FullSpan.Start : global[^1].FullSpan.End;
        else pos = amAnfang ? (wurzel.Members.FirstOrDefault()?.FullSpan.Start ?? starter.Length) : (wurzel.Members.FirstOrDefault()?.FullSpan.Start ?? starter.Length);
        return starter.Insert(pos, "\n" + zeilen);
    }

    [Fact]
    public async Task Startcode_plus_feste_Ausgabe_besteht_nicht()
    {
        var bestanden = new List<string>();
        foreach (var a in Alle)
            foreach (bool anfang in new[] { false, true })
            {
                string code = Angriff(a.Startcode, a.Tests[0].Erwartet, anfang);
                var r = await AufgabenPruefer.PruefenAsync(a, code);
                if (r.Bestanden) { bestanden.Add(a.Id); break; }
            }
        bestanden.Sort(StringComparer.Ordinal);
        log.WriteLine("Besteht mit Startcode + fester Ausgabe: " + string.Join(", ", bestanden));
        var unerwartet = bestanden.Where(id => !AngriffErlaubt.Contains(id)).ToList();
        Assert.Empty(unerwartet);
    }

    /// <summary>Bei jeder Übung mit Anforderungen muss mindestens eine Anforderung am Startcode scheitern –
    /// sonst schützt sie nicht gegen „Startcode + fest eingetippte Ausgabe“.</summary>
    [Fact]
    public void Anforderungen_verlangen_eigenen_Code()
    {
        var offen = new List<string>();
        foreach (var a in Alle.Where(a => a.Anforderungen.Count > 0 && !AngriffErlaubt.Contains(a.Id) && !LeitplankenAnforderung.Contains(a.Id)))
        {
            string ohne = AufgabenPruefer.OhneKommentare(a.Startcode);
            if (a.Anforderungen.All(r => Regex.IsMatch(r.MitKommentaren ? a.Startcode : ohne, r.Muster))) offen.Add(a.Id);
        }
        log.WriteLine("Anforderungen schon im Startcode erfüllt: " + string.Join(", ", offen));
        Assert.Empty(offen);
    }

    [Fact]
    public void Anforderungen_passen_zur_Loesung()
    {
        foreach (var a in Alle)
        {
            string ohne = AufgabenPruefer.OhneKommentare(a.Loesung);
            foreach (var r in a.Anforderungen)
                Assert.True(Regex.IsMatch(r.MitKommentaren ? a.Loesung : ohne, r.Muster), $"{a.Id}: Muster {r.Muster} passt nicht zur Lösung");
            Assert.True(a.Anforderungen.Count > 0 || NurAusgabeErlaubt.Contains(a.Id) || a.Tests.Count > 1, $"{a.Id}: nur ein Test und keine Anforderung");
        }
    }

    [Fact]
    public void Alle_Uebungen_haben_Tipps_und_alle_Quizfragen_Erklaerungen()
    {
        foreach (var a in Alle)
        {
            Assert.False(string.IsNullOrWhiteSpace(a.Tipp), a.Id + ": Tipp fehlt");
            int saetze = Regex.Matches(a.Tipp.Replace("z. B.", "zB").TrimEnd(), @"[.!?](\s|$)").Count;
            Assert.True(saetze <= 2, $"{a.Id}: Tipp hat {saetze} Sätze");
        }
        foreach (var l in Kurs.Lektionen)
        {
            Assert.True(l.Quiz.Count >= 3, l.Id + ": weniger als 3 Quizfragen");
            for (int i = 0; i < l.Quiz.Count; i++)
            {
                var q = l.Quiz[i];
                Assert.False(string.IsNullOrWhiteSpace(q.Erklaerung), $"{l.Id} Frage {i + 1}: Erklärung fehlt");
                Assert.InRange(q.Richtig, 0, q.Antworten.Count - 1);
            }
        }
        Assert.Equal(4, L("l25").Quiz[2].Antworten.Count);
        Assert.Equal(4, L("l26").Quiz[2].Antworten.Count);
    }

    [Fact]
    public void Textkorrekturen_sind_drin()
    {
        var l20a3 = A("l20a3");
        Assert.All(l20a3.Tests, t => Assert.StartsWith("Anzahl Pizzen: ", t.Erwartet));
        Assert.Contains("Anzahl Pizzen: 4", l20a3.Aufgabentext);
        Assert.Contains("Anzahl Pizzen:", l20a3.Loesung);

        foreach (var id in new[] { "l26a2", "l26a3" })
        {
            var a = A(id);
            Assert.DoesNotContain("Versuche gebraucht", a.Aufgabentext + a.Loesung + string.Concat(a.Tests.Select(t => t.Erwartet)));
            Assert.Contains("Versuche: ", a.Aufgabentext);
        }
        Assert.DoesNotContain("Versuche gebraucht", L("l26").Erklaerung + L("l26").Beispiel);
        Assert.Contains("Versuche: 3", L("l26").Erklaerung);

        var l04a4 = A("l04a4");
        Assert.Equal("Durchschnitt: 2,33", l04a4.Tests.Single().Erwartet);
        Assert.Contains("Durchschnitt: 2,33", l04a4.Aufgabentext);
        Assert.Contains(":F2", l04a4.Loesung);

        Assert.DoesNotContain("Count()", L("l13").Beispiel);
        Assert.Contains("anzahl3.Length", L("l13").Beispiel);
        Assert.Contains("6 or 7", L("l10").Erklaerung);

        var l20a2 = A("l20a2");
        Assert.Contains("`5`", l20a2.Aufgabentext);
        Assert.Contains("Umfang: 31,42", l20a2.Aufgabentext);
        Assert.Contains("Fläche: 78,54", l20a2.Aufgabentext);
        Assert.Equal(3, l20a2.Tests.Count);
    }
}
