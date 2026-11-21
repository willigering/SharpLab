using CSharpLernen.Engine;
using Xunit;

namespace CSharpLernen.Tests;

public class MotorTests
{
    [Fact]
    public async Task HalloWelt_wird_ausgegeben()
    {
        var r = await CodeAusfuehrer.AusfuehrenAsync("Console.WriteLine(\"Hallo, Welt!\");");
        Assert.Equal(Ausgang.Erfolg, r.Ausgang);
        Assert.Equal("Hallo, Welt!", r.Ausgabe.Trim());
    }

    [Fact]
    public async Task Klassischer_Main_Stil_funktioniert()
    {
        string code = """
using System;

namespace MeinProgramm
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Main läuft");
            Console.ReadKey();
        }
    }
}
""";
        var r = await CodeAusfuehrer.AusfuehrenAsync(code);
        Assert.Equal(Ausgang.Erfolg, r.Ausgang);
        Assert.Equal("Main läuft", r.Ausgabe.Trim());
    }

    [Fact]
    public async Task Fehlendes_Semikolon_wird_mit_Zeile_und_Tipp_gemeldet()
    {
        var r = await CodeAusfuehrer.AusfuehrenAsync("Console.WriteLine(\"a\")\nConsole.WriteLine(\"b\");");
        Assert.Equal(Ausgang.Kompilierfehler, r.Ausgang);
        var m = Assert.Single(r.Meldungen, m => !m.IstWarnung);
        Assert.Equal("CS1002", m.Code);
        Assert.Equal(1, m.Zeile);
        Assert.NotNull(m.Tipp);
        Assert.Contains("erwartet", m.Text); // deutsche Compiler-Meldung
    }

    [Fact]
    public async Task Unbekannter_Name_CS0103()
    {
        var r = await CodeAusfuehrer.AusfuehrenAsync("int zahl = 3;\n\nConsole.WriteLine(Zahl);");
        Assert.Equal(Ausgang.Kompilierfehler, r.Ausgang);
        var m = r.Meldungen.First(m => m.Code == "CS0103");
        Assert.Equal(3, m.Zeile);
        Assert.NotNull(m.Tipp);
    }

    [Fact]
    public async Task ReadLine_liest_Eingabezeilen()
    {
        string code = "Console.Write(\"Name: \");\nstring n = Console.ReadLine();\nint a = int.Parse(Console.ReadLine());\nConsole.WriteLine($\"Hallo {n}, bald bist du {a + 1}!\");";
        var r = await CodeAusfuehrer.AusfuehrenAsync(code, new Laufoptionen { Eingabe = new[] { "Anna", "25" } });
        Assert.Equal(Ausgang.Erfolg, r.Ausgang);
        Assert.Equal("Name: Hallo Anna, bald bist du 26!", r.Ausgabe.Trim());
    }

    [Fact]
    public async Task Interaktive_Eingabe_wird_abgefragt()
    {
        int gefragt = 0;
        var r = await CodeAusfuehrer.AusfuehrenAsync("string s = Console.ReadLine(); Console.WriteLine(s + \"!\");",
            new Laufoptionen { InteraktiveEingabe = _ => { gefragt++; return "Hey"; } });
        Assert.Equal(1, gefragt);
        Assert.Equal("Hey!", r.Ausgabe.Trim());
    }

    [Fact]
    public async Task Endlosschleife_wird_nach_Zeitlimit_gestoppt()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var r = await CodeAusfuehrer.AusfuehrenAsync("int i = 0;\nwhile (i < 10)\n{\n    int x = i * 2;\n}", new Laufoptionen { Zeitlimit = TimeSpan.FromSeconds(1) });
        Assert.Equal(Ausgang.Zeitueberschreitung, r.Ausgang);
        Assert.True(sw.Elapsed < TimeSpan.FromSeconds(8), "zu langsam gestoppt: " + sw.Elapsed);
    }

    [Fact]
    public async Task Endlosschleife_ohne_Klammern_und_for_wird_gestoppt()
    {
        var r = await CodeAusfuehrer.AusfuehrenAsync("for (;;) ;", new Laufoptionen { Zeitlimit = TimeSpan.FromSeconds(1) });
        Assert.Equal(Ausgang.Zeitueberschreitung, r.Ausgang);
    }

    [Fact]
    public async Task Endlose_Ausgabe_wird_gestoppt()
    {
        var r = await CodeAusfuehrer.AusfuehrenAsync("while (true) Console.WriteLine(\"x\");", new Laufoptionen { Zeitlimit = TimeSpan.FromSeconds(5) });
        Assert.True(r.Ausgang is Ausgang.ZuVielAusgabe or Ausgang.Zeitueberschreitung);
    }

    [Fact]
    public async Task Endlose_Rekursion_stuerzt_nicht_ab()
    {
        var r = await CodeAusfuehrer.AusfuehrenAsync("int F(int n) => F(n + 1);\nConsole.WriteLine(F(1));\nvoid G() { G(); }");
        Assert.Equal(Ausgang.Laufzeitfehler, r.Ausgang);
        Assert.Equal("InsufficientExecutionStackException", r.FehlerTyp);
        Assert.Equal(1, r.FehlerZeile);
    }

    [Fact]
    public async Task Stopp_Knopf_bricht_ab()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));
        var r = await CodeAusfuehrer.AusfuehrenAsync("while (true) { }", new Laufoptionen { Zeitlimit = TimeSpan.FromSeconds(30), Abbruch = cts.Token });
        Assert.Equal(Ausgang.Abgebrochen, r.Ausgang);
    }

    [Fact]
    public async Task Laufzeitfehler_mit_Zeilennummer()
    {
        var r = await CodeAusfuehrer.AusfuehrenAsync("int[] a = new int[3];\nConsole.WriteLine(\"vorher\");\na[3] = 1;");
        Assert.Equal(Ausgang.Laufzeitfehler, r.Ausgang);
        Assert.Equal(3, r.FehlerZeile);
        Assert.Equal("vorher", r.Ausgabe.Trim());
    }

    [Fact]
    public async Task Zeilennummer_in_Methode_und_Schleife_stimmt()
    {
        string code = "Console.WriteLine(Teile(10, 0));\n\nint Teile(int a, int b)\n{\n    for (int i = 0; i < 3; i++)\n        Console.WriteLine(i);\n    return a / b;\n}\n";
        var r = await CodeAusfuehrer.AusfuehrenAsync(code);
        Assert.Equal(Ausgang.Laufzeitfehler, r.Ausgang);
        Assert.Equal("DivideByZeroException", r.FehlerTyp);
        Assert.Equal(7, r.FehlerZeile);
    }

    [Fact]
    public async Task Zu_wenig_Eingabe_gibt_Hinweis()
    {
        var r = await CodeAusfuehrer.AusfuehrenAsync("int z = int.Parse(Console.ReadLine());");
        Assert.Equal(Ausgang.Laufzeitfehler, r.Ausgang);
        Assert.True(r.EingabeAufgebraucht);
    }

    [Fact]
    public async Task Deutsche_Zahlenformatierung()
    {
        var r = await CodeAusfuehrer.AusfuehrenAsync("double d = 3.5; Console.WriteLine(d); Console.WriteLine(double.Parse(\"2,25\") * 2);");
        Assert.Equal("3,5\n4,5", AufgabenPruefer.Normalisieren(r.Ausgabe));
    }

    [Fact]
    public async Task Environment_Exit_und_Clear_beenden_die_App_nicht()
    {
        var r = await CodeAusfuehrer.AusfuehrenAsync("Console.WriteLine(\"weg\"); Console.Clear(); Console.WriteLine(\"da\"); Environment.Exit(0); Console.WriteLine(\"nie\");");
        Assert.Equal(Ausgang.Erfolg, r.Ausgang);
        Assert.Equal("da", r.Ausgabe.Trim());
    }

    [Fact]
    public async Task Warnung_Semikolon_hinter_if()
    {
        var r = await CodeAusfuehrer.AusfuehrenAsync("int x = 1;\nif (x > 5);\n{\n    Console.WriteLine(\"groß\");\n}");
        Assert.Equal(Ausgang.Erfolg, r.Ausgang);
        Assert.Contains(r.Meldungen, m => m.IstWarnung && m.Code == "CS0642");
    }

    [Fact]
    public void Vergleich_mit_Hinweisen()
    {
        Assert.True(AufgabenPruefer.Vergleichen("A\nB", "A\r\nB  \r\n\r\n", false).Gleich);
        var v = AufgabenPruefer.Vergleichen("Hallo Welt", "hallo welt", false);
        Assert.False(v.Gleich);
        Assert.Contains("Groß", v.Erklaerung);
        Assert.True(AufgabenPruefer.Vergleichen("Hallo Anna!", "Wie heißt du? Hallo Anna!", true).Gleich);
    }

    [Fact]
    public void Vergessene_Leerzeichen_sind_kein_Fehler_nur_ein_Hinweis()
    {
        var v = AufgabenPruefer.Vergleichen("Hallo, Welt!", "Hallo,Welt!", false);
        Assert.True(v.Gleich);
        Assert.Contains("Leerzeichen", v.Schoenheitshinweis);
        Assert.Contains("Zeile 1", v.Schoenheitshinweis);

        var zu = AufgabenPruefer.Vergleichen("A B", "A  B", false);
        Assert.True(zu.Gleich);
        Assert.Contains("zu viel", zu.Schoenheitshinweis);

        // Leerzeilen dazwischen sind ebenfalls nur Kosmetik
        Assert.True(AufgabenPruefer.Vergleichen("A\nB", "A\n\nB", false).Gleich);

        // Streng bleibt: andere Zeichen, Groß-/Kleinschreibung, Zeilenumbrüche
        Assert.False(AufgabenPruefer.Vergleichen("Hallo", "Hallo?", false).Gleich);
        Assert.False(AufgabenPruefer.Vergleichen("Hallo", "hallo", false).Gleich);
        Assert.False(AufgabenPruefer.Vergleichen("A B", "A\nB", false).Gleich);
        Assert.Null(AufgabenPruefer.Vergleichen("A\nB", "A\nB", false).Schoenheitshinweis);
    }

    [Fact]
    public void Nachsichtiger_Vergleich_toleriert_Leerzeichen()
    {
        var v = AufgabenPruefer.Vergleichen("Name: Mia", "Wie heißt du? Name:Mia", true);
        Assert.True(v.Gleich);
        Assert.NotNull(v.Schoenheitshinweis);
    }

    [Fact]
    public async Task Pruefung_besteht_mit_Schoenheitshinweis()
    {
        var a = new Aufgabe { Id = "x", Titel = "x", Tests = { new Testfall { Erwartet = "Hallo, Welt!" } } };
        var r = await AufgabenPruefer.PruefenAsync(a, "Console.WriteLine(\"Hallo,Welt!\");");
        Assert.True(r.Bestanden);
        Assert.Contains("Leerzeichen", r.Details);
        var sauber = await AufgabenPruefer.PruefenAsync(a, "Console.WriteLine(\"Hallo, Welt!\");");
        Assert.True(sauber.Bestanden);
        Assert.DoesNotContain("Leerzeichen", sauber.Details);
    }

    [Fact]
    public void Satzzeichen_und_kleine_Tippfehler_sind_nur_ein_Hinweis()
    {
        var p = AufgabenPruefer.Vergleichen("Hallo, Welt!", "Hallo, Welt.", false);
        Assert.True(p.Gleich);
        Assert.Contains("Satzzeichen", p.Schoenheitshinweis);
        Assert.True(AufgabenPruefer.Vergleichen("Hallo, Welt!", "Hallo, Welt", false).Gleich);

        var t = AufgabenPruefer.Vergleichen("Schön, dass du da bist.", "Schön, das du da bist.", false);
        Assert.True(t.Gleich);
        Assert.Contains("Tippfehler", t.Schoenheitshinweis);

        // Zahlen, kurze Wörter und Groß-/Kleinschreibung bleiben streng
        Assert.False(AufgabenPruefer.Vergleichen("Level 13", "Level 12", false).Gleich);
        Assert.False(AufgabenPruefer.Vergleichen("Rot", "Rat", false).Gleich);
        Assert.False(AufgabenPruefer.Vergleichen("gerade", "ungerade", false).Gleich);
        Assert.False(AufgabenPruefer.Vergleichen("Zahl ist gerade.", "Zahl ist ungerade.", false).Gleich);
        Assert.False(AufgabenPruefer.Vergleichen("Hallo Welt", "hallo welt", false).Gleich);
        Assert.False(AufgabenPruefer.Vergleichen("True", "False", false).Gleich);

        Assert.True(AufgabenPruefer.Vergleichen("Hallo Anna!", "Wie heißt du? Hallo Anna.", true).Gleich);
    }
}
