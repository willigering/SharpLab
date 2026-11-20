using System.Text.RegularExpressions;
using Xunit;

namespace CSharpLernen.Tests;

/// <summary>Die Konsole (AusgabeBox) muss beim Öffnen einer Aufgabe/Lektion/Seite leer sein, bis man selbst ausführt.
/// Quelltext-Prüfung, weil die Oberfläche (WPF) hier nicht startet.</summary>
public class KonsoleTests
{
    static string Wurzel()
    {
        var d = new DirectoryInfo(AppContext.BaseDirectory);
        while (d != null && !File.Exists(Path.Combine(d.FullName, "CSharpLernen.sln"))) d = d.Parent;
        Assert.NotNull(d);
        return d!.FullName;
    }

    static string Datei(string name) => File.ReadAllText(Path.Combine(Wurzel(), "src", "CSharpLernen", name));

    static string Methode(string quelle, string kopf)
    {
        int i = quelle.IndexOf(kopf, StringComparison.Ordinal);
        Assert.True(i >= 0, kopf + " nicht gefunden");
        int auf = quelle.IndexOf('{', i), tiefe = 0;
        for (int j = auf; j < quelle.Length; j++)
        {
            if (quelle[j] == '{') tiefe++;
            else if (quelle[j] == '}' && --tiefe == 0) return quelle[auf..(j + 1)];
        }
        throw new Exception("Methodenende fehlt");
    }

    [Fact]
    public void Kontextwechsel_leert_die_Konsole_auch_bei_gleicher_Aufgabe()
    {
        string m = Methode(Datei("MainWindow.xaml.cs"), "void SetzeEditorKontext(");
        // Früher: „if (schluessel == editorSchluessel) return;“ – dann blieb die alte Ausgabe stehen (z. B. nach dem Quiz).
        Assert.DoesNotMatch(new Regex(@"if\s*\(\s*schluessel\s*==\s*editorSchluessel\s*\)\s*return\s*;"), m);
        Assert.True(Regex.Matches(m, @"KonsoleZuruecksetzen\(\)").Count >= 2, "Beide Wege (neue und gleiche Aufgabe) müssen die Konsole zurücksetzen.");
    }

    [Fact]
    public void Leeren_wird_nicht_von_einem_laufenden_Programm_verhindert()
    {
        string q = Datei("MainWindow.xaml.cs");
        // Früher brach KonsoleLeeren bei laufendem Programm ab – ein Programm, das auf ReadLine wartete,
        // schrieb seine Frage dann in die Konsole der nächsten Aufgabe.
        Assert.DoesNotContain("if (laeuft) return;", Methode(q, "void KonsoleLeeren("));
        string z = Methode(q, "void KonsoleZuruecksetzen(");
        Assert.Contains("laufCts?.Cancel()", z);
        Assert.Contains("laufNr++", z);
        Assert.Contains("ausgabePuffer.TryDequeue", z);
    }

    [Fact]
    public void Ausgaben_und_Ergebnisse_alter_Laeufe_werden_verworfen()
    {
        string q = Datei("MainWindow.xaml.cs");
        string aus = Methode(q, "async Task Ausfuehren(");
        Assert.Contains("if (nr == laufNr) ausgabePuffer.Enqueue(s)", aus);
        Assert.Matches(new Regex(@"if \(nr != laufNr\) return;\s*\n\s*fortschritt\.Ausfuehrungen\+\+"), aus);
        string pr = Methode(Datei("MainWindow.Lektion.cs"), "async Task Pruefen(");
        Assert.True(pr.IndexOf("if (nr != laufNr) return;") < pr.IndexOf("AusgabeBox.Text = r.Lauf.Ausgabe"), "Prüfergebnis erst nach der Laufnummer-Prüfung anzeigen.");
    }

    [Fact]
    public void Keine_Platzhalter_oder_gespeicherte_Ausgabe_in_der_Konsole()
    {
        string xaml = Datei("MainWindow.xaml");
        var box = Regex.Match(xaml, @"<TextBox[^>]*x:Name=""AusgabeBox""[^>]*>");
        Assert.True(box.Success);
        Assert.DoesNotMatch(new Regex(@"\bText\s*="), box.Value);
        // AusgabeBox wird nur hier beschrieben: Lauf, Echo der Eingabe, Kürzen, „nichts ausgegeben“,
        // Prüfergebnis und noch einmal nach dem Neuaufbau einer bestandenen Aufgabe.
        var schreiber = Directory.GetFiles(Path.Combine(Wurzel(), "src", "CSharpLernen"), "*.cs")
            .SelectMany(f => File.ReadAllLines(f).Select((z, i) => (Datei: Path.GetFileName(f), Zeile: z.Trim())))
            .Where(t => Regex.IsMatch(t.Zeile, @"AusgabeBox\.(Text\s*=|AppendText)"))
            .Select(t => t.Datei).ToList();
        Assert.Equal(6, schreiber.Count);
        Assert.DoesNotContain(schreiber, d => d.Contains("Seiten") || d.Contains("Lernstand"));
    }
}
