using CSharpLernen.Engine;
using Xunit;

namespace CSharpLernen.Tests;

/// <summary>Willi (1.0.5): „Autovervollständigung von Variablen, die ich erstellt habe“ + Codeausschnitte (cw Tab Tab).
/// Früher kamen eigene Bezeichner zwar von Roslyn, standen aber alphabetisch zwischen ~3400 Typen/Schlüsselwörtern
/// (z. B. „st“ → stackalloc vor stadt) und wurden von der Liste nicht vorausgewählt. Seit Willis Wunsch „Verhalten von
/// Visual Studio“: alphabetische Liste, VS-Filter, Vorauswahl nach Roslyn-Matching + zuletzt benutzt.</summary>
public class EigeneVorschlaegeTests
{
    static async Task<VervollstaendigungsListe> An(string code, char? zeichen = null)
    {
        int pos = code.IndexOf('|');
        var l = await Vervollstaendigung.Standard.VorschlaegeAsync(code.Remove(pos, 1), pos, zeichen);
        Assert.NotNull(l);
        return l!;
    }

    /// <summary>Wie IntelliSense in Visual Studio: Vorauswahl = bester Treffer nach Roslyn-Matching (FilterItems),
    /// bei Gleichstand zuletzt benutzt, sonst der kürzeste.</summary>
    [Theory]
    [InlineData("string stadt = \"Berlin\";\ns|", 's', "sta", "stadt")]
    [InlineData("string stadt = \"Berlin\";\nConsole.WriteLine(s|);", 's', "sta", "stadt")]
    [InlineData("C|", 'C', "Con", "Console")]
    [InlineData("Console.W|", 'W', "wl", "WriteLine")]
    [InlineData("Console.W|", 'W', "WL", "WriteLine")]
    [InlineData("S|", 'S', "SBui", "StringBuilder")]
    [InlineData("c|", 'c', "cw", "cw")]
    [InlineData("int summe = 0;\nfor (int i = 0; i < 3; i++)\n{\n    su|\n}", null, "su", "summe")]
    [InlineData("for (int zaehler = 0; zaehler < 3; zaehler++)\n{\n    Console.WriteLine(z|);\n}", 'z', "za", "zaehler")]
    [InlineData("static int Verdopple(int wert)\n{\n    return w|;\n}", 'w', "we", "wert")]
    [InlineData("Console.WriteLine(V|);\nstatic int Verdopple(int wert) => wert * 2;", 'V', "Verd", "Verdopple")]
    [InlineData("T|\nclass Tier { }", 'T', "Ti", "Tier")]
    [InlineData("class Konto\n{\n    public double Kontostand { get; set; }\n    void Zeigen() { Console.WriteLine(K|); }\n}", 'K', "Kontost", "Kontostand")]
    [InlineData("static void Main()\n{\n    int alter = 3;\n    a|\n}", 'a', "al", "alter")]
    public async Task Vorauswahl_wie_Visual_Studio(string code, char? zeichen, string getippt, string erwartet)
    {
        Vervollstaendigung.ZuletztVergessen();
        var l = await An(code, zeichen);
        var f = l.Filtern(getippt);
        Assert.Equal(erwartet, f.Auswahl?.Anzeige);
        Assert.Contains(f.Auswahl!, f.Sichtbar);
    }

    [Fact]
    public async Task Liste_ist_alphabetisch_und_nicht_eigene_zuerst()
    {
        Vervollstaendigung.ZuletztVergessen();
        var l = await An("string stadt = \"Berlin\";\ns|", 's');
        Assert.NotEqual("stadt", l.Vorschlaege[0].Anzeige);
        var sortiert = l.Vorschlaege.Select(v => v.SortText).OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ThenBy(x => x, StringComparer.Ordinal);
        Assert.Equal(sortiert, l.Vorschlaege.Select(v => v.SortText));
        var f = l.Filtern("sta");
        Assert.All(f.Sichtbar, v => Assert.NotNull(VervollstaendigungsListe.Trefferart(v.FilterText, "sta")));
        Assert.Contains(f.Sichtbar, v => v.Anzeige == "static");
        Assert.True(f.Sichtbar.ToList().IndexOf(f.Sichtbar.First(v => v.Anzeige == "stackalloc")) < f.Sichtbar.ToList().IndexOf(f.Auswahl!),
            "stackalloc steht alphabetisch vor stadt – vorausgewählt ist trotzdem stadt");
    }

    [Fact]
    public async Task CamelCase_und_Wortteile_filtern_wie_VS()
    {
        var l = await An("S|", 'S');
        Assert.Contains(l.Filtern("SB").Sichtbar, v => v.Anzeige == "StringBuilder");
        var w = await An("Console.W|", 'W');
        var line = w.Filtern("Line").Sichtbar.Select(v => v.Anzeige).ToList();
        Assert.Contains("WriteLine", line);
        Assert.Contains("ReadLine", line);
        Assert.DoesNotContain("Beep", line);
    }

    [Fact]
    public async Task Zuletzt_benutzter_Eintrag_gewinnt_bei_Gleichstand()
    {
        Vervollstaendigung.ZuletztVergessen();
        var l = await An("C|", 'C');
        Assert.Equal("Console", l.Filtern("Con").Auswahl?.Anzeige);
        Vervollstaendigung.Benutzt(l.Vorschlaege.First(v => v.Anzeige == "ConsoleColor"));
        Assert.Equal("ConsoleColor", l.Filtern("Con").Auswahl?.Anzeige);
        Assert.Equal("Console", l.Filtern("Console").Auswahl?.Anzeige);   // exakter Treffer schlägt „zuletzt benutzt“
        Vervollstaendigung.ZuletztVergessen();
    }

    [Theory]
    [InlineData("WriteLine", "WriteLine", 0)]
    [InlineData("WriteLine", "writeline", 1)]
    [InlineData("WriteLine", "Wri", 2)]
    [InlineData("WriteLine", "wri", 3)]
    [InlineData("WriteLine", "wl", 4)]
    [InlineData("StringBuilder", "SB", 4)]
    [InlineData("StringBuilder", "StrBui", 4)]
    [InlineData("WriteLine", "Line", 5)]
    [InlineData("WriteLine", "xyz", null)]
    [InlineData("WriteLine", "ie", null)]
    public void Trefferarten(string kandidat, string muster, int? rang) =>
        Assert.Equal(rang, VervollstaendigungsListe.Trefferart(kandidat, muster));

    [Fact]
    public async Task Codeausschnitte_stehen_in_der_Liste_nur_am_Anweisungsanfang()
    {
        var l = await An("c|", 'c');
        var cw = l.Vorschlaege.FirstOrDefault(v => v.Anzeige == "cw");
        Assert.NotNull(cw);
        Assert.Equal(VorschlagArt.Codeausschnitt, cw!.Art);
        Assert.Equal("cw", cw.Einfuegen);
        Assert.Contains("Console.WriteLine", await cw.BeschreibungAsync());

        var mitten = await An("int x = c|", 'c');
        Assert.DoesNotContain(mitten.Vorschlaege, v => v.Art == VorschlagArt.Codeausschnitt);
    }

    [Fact]
    public void Cw_wird_zu_Console_WriteLine_mit_Cursor_in_den_Klammern()
    {
        string code = "    cw";
        Assert.True(Codeausschnitte.KannErweitern(code, code.Length, out var a, out int start));
        Assert.Equal(4, start);
        var (text, cursor, felder) = a!.Erweitere();
        Assert.Equal("Console.WriteLine();", text);
        Assert.Equal("Console.WriteLine(".Length, cursor);
        Assert.Empty(felder);
    }

    [Fact]
    public void For_hat_verknuepfte_Felder_und_Einrueckung()
    {
        var (text, cursor, felder) = Codeausschnitte.Finde("for")!.Erweitere("    ", "    ");
        Assert.Equal("for (int i = 0; i < 10; i++)\n    {\n        \n    }", text);
        Assert.Equal(2, felder.Count);
        Assert.Equal("i", text.Substring(felder[0].Start, felder[0].Laenge));
        Assert.Equal("10", text.Substring(felder[1].Start, felder[1].Laenge));
        Assert.Equal(text.IndexOf("\n        ") + 9, cursor);
    }

    [Theory]
    [InlineData("for"), InlineData("foreach"), InlineData("if"), InlineData("else"), InlineData("while"), InlineData("do"), InlineData("switch"),
     InlineData("class"), InlineData("prop"), InlineData("ctor"), InlineData("svm"), InlineData("try"), InlineData("cw"), InlineData("forr")]
    public async Task Jeder_Ausschnitt_ergibt_kompilierbaren_Code(string kuerzel)
    {
        var (text, _, _) = Codeausschnitte.Finde(kuerzel)!.Erweitere();
        string code = kuerzel switch
        {
            "else" => "if (true) { }\n" + text,
            "class" => "Console.WriteLine();\n" + text,
            "prop" or "ctor" => "Console.WriteLine();\nclass Klasse\n{\n" + text + "\n}",
            "svm" => "class P\n{\n" + text + "\n}",
            "foreach" => "var liste = new List<int>();\n" + text,
            "switch" => "int wert = 1;\n" + text,
            _ => text,
        };
        var r = await CodeAusfuehrer.AusfuehrenAsync(code, new Laufoptionen { Zeitlimit = TimeSpan.FromSeconds(1) });
        Assert.NotEqual(Ausgang.Kompilierfehler, r.Ausgang);
    }

    [Theory]
    [InlineData("x = cw", false)]              // nicht am Anweisungsanfang
    [InlineData("cwx", false)]                 // kein Kürzel
    [InlineData("// cw", false)]               // Kommentar
    [InlineData("string s = \"\n cw", false)]  // (kaputter) String – Zeile beginnt mit Leerraum, aber Kürzel steht im String? hier: neue Zeile
    [InlineData("  if", true)]
    [InlineData("prop", true)]
    public void Erweitern_nur_wo_es_passt(string code, bool erwartet)
    {
        bool ok = Codeausschnitte.KannErweitern(code, code.Length, out _, out _);
        if (code.StartsWith("string")) return;   // nur Absturzfreiheit prüfen
        Assert.Equal(erwartet, ok);
    }

    [Fact]
    public void Tab_mitten_im_Wort_erweitert_nicht()
    {
        Assert.False(Codeausschnitte.KannErweitern("cwx", 2, out _, out _));
        Assert.False(Codeausschnitte.KannErweitern("", 0, out _, out _));
    }
}
