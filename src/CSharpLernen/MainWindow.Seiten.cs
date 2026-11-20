using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using CSharpLernen.Engine;

namespace CSharpLernen;

public partial class MainWindow
{
    const string SpielwieseStart = """
// Spielwiese: Hier kannst du alles ausprobieren!
// F5 = Kompilieren & Ausführen

Console.WriteLine("Hallo! Was möchtest du heute programmieren?");
""";

    void Nav_Checked(object sender, RoutedEventArgs e)
    {
        if (!geladen) return;
        string neu = sender == NavUebungen ? "uebungen"
            : sender == NavEditor ? "editor"
            : sender == NavProjekte ? "projekte"
            : sender == NavCompiler ? "compiler"
            : sender == NavBegriffe ? "begriffe"
            : sender == NavFortschritt ? "fortschritt"
            : sender == NavErfolge ? "erfolge"
            : sender == NavEinstellungen ? "einstellungen"
            : "lernpfad";
        seite = neu;
        bool lernpfad = neu == "lernpfad";
        LernpfadSpalte.Visibility = lernpfad ? Visibility.Visible : Visibility.Collapsed;
        LektionSpalte.Visibility = lernpfad ? Visibility.Visible : Visibility.Collapsed;
        SeitenSpalte.Visibility = lernpfad ? Visibility.Collapsed : Visibility.Visible;
        if (lernpfad)
        {
            if (aktuelleLektion != null) ZeigeLektion(aktuelleLektion, aktuellerReiter, aktuelleAufgabe);
            return;
        }
        BaueSeite(neu);
    }

    void BaueSeite(string name)
    {
        SeitenInhalt.Children.Clear();
        einstMatrixAn = null; einstMatrixRegler = null; einstMatrixText = null; einstMatrixTempo = null; einstMatrixTempoText = null;
        if (SeitenSpalte.Child is ScrollViewer sv) sv.ScrollToTop();
        switch (name)
        {
            case "uebungen": SeiteUebungen(); break;
            case "editor": SeiteSpielwiese(); break;
            case "projekte": SeiteProjekte(); break;
            case "compiler": SeiteCompiler(); break;
            case "begriffe": SeiteBegriffe(); break;
            case "fortschritt": SeiteFortschritt(); break;
            case "erfolge": SeiteErfolge(); break;
            case "einstellungen": SeiteEinstellungen(); break;
        }
    }

    void SeitenKopf(string titel, string untertitel, string icon)
    {
        var kopf = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 6) };
        kopf.Children.Add(Ui.Icon(icon, 24));
        kopf.Children.Add(Ui.Text("  " + titel, 23.5, "TextHell", FontWeights.Bold));
        SeitenInhalt.Children.Add(kopf);
        SeitenInhalt.Children.Add(Ui.Text(untertitel, 13, "TextMittel", rand: new Thickness(0, 0, 0, 18)));
    }

    // ---------------- Übungen ----------------

    void SeiteUebungen()
    {
        int gesamt = lektionen.Sum(l => l.Aufgaben.Count);
        int geloest = lektionen.Sum(Geloest);
        SeitenKopf("Übungen", $"Alle {gesamt} Aufgaben auf einen Blick – {geloest} davon hast du schon gelöst. Klick auf eine Aufgabe, um sie zu öffnen.", "\uE73A");

        foreach (var stufe in kurs.Stufen)
        {
            var inStufe = lektionen.Where(l => l.Stufe == stufe.Nummer).ToList();
            if (inStufe.Count == 0) continue;
            SeitenInhalt.Children.Add(Ui.Text($"Stufe {stufe.Nummer}: {stufe.Titel}", 15.5, "Akzent", FontWeights.SemiBold, new Thickness(0, 8, 0, 8)));
            foreach (var l in inStufe)
            {
                var sp = new StackPanel();
                var kopf = new Grid();
                kopf.Children.Add(Ui.Text($"{lektionen.IndexOf(l) + 1}. {l.Titel}", 13.5, "TextHell", FontWeights.SemiBold));
                var status = Ui.Text(IstKomplett(l) ? $"✓ alle {l.Aufgaben.Count} gelöst" : IstAbgeschlossen(l) ? $"✓ abgeschlossen · {Geloest(l)}/{l.Aufgaben.Count} gelöst" : IstFreigeschaltet(l) ? $"{Geloest(l)}/{l.Aufgaben.Count} gelöst" : "🔒 gesperrt", 11,
                    IstAbgeschlossen(l) ? "Gruen" : "TextGedimmt");
                status.HorizontalAlignment = HorizontalAlignment.Right;
                kopf.Children.Add(status);
                sp.Children.Add(kopf);
                var pillen = new WrapPanel { Margin = new Thickness(0, 10, 0, -8) };
                for (int i = 0; i < l.Aufgaben.Count; i++)
                {
                    var a = l.Aufgaben[i];
                    var knopf = Ui.Knopf($"{i + 1}. {a.Titel}", (_, _) => OeffneLektion(l, "uebung", a), false, IstGeloest(a) ? "\uE73E" : "\uE943");
                    knopf.Margin = new Thickness(0, 0, 8, 8);
                    if (IstGeloest(a)) knopf.BorderBrush = Ui.B("Gruen");
                    pillen.Children.Add(knopf);
                }
                sp.Children.Add(pillen);
                SeitenInhalt.Children.Add(Ui.Karte(sp, innen: new Thickness(16, 12, 16, 12)));
            }
        }
    }

    // ---------------- Spielwiese ----------------

    static readonly (string Titel, string Beschreibung, string Code)[] Vorlagen =
    {
        ("Hallo Welt", "Das klassische erste Programm.", "Console.WriteLine(\"Hallo, Welt!\");\n"),
        ("Eingabe & Ausgabe", "Name und Alter abfragen.", "Console.Write(\"Wie heißt du? \");\nstring name = Console.ReadLine();\n\nConsole.Write(\"Wie alt bist du? \");\nint alter = int.Parse(Console.ReadLine());\n\nConsole.WriteLine($\"Hallo {name}! In 5 Jahren bist du {alter + 5}.\");\n"),
        ("Schleifen", "for, while und foreach im Vergleich.", "for (int i = 1; i <= 3; i++)\n{\n    Console.WriteLine($\"for: {i}\");\n}\n\nint j = 3;\nwhile (j > 0)\n{\n    Console.WriteLine($\"while: {j}\");\n    j--;\n}\n\nstring[] farben = { \"rot\", \"grün\", \"blau\" };\nforeach (string f in farben)\n{\n    Console.WriteLine($\"foreach: {f}\");\n}\n"),
        ("Zahlenraten", "Das Spiel mit echter Zufallszahl.", "int geheim = Random.Shared.Next(1, 101);\nint versuche = 0;\nint tipp = 0;\nConsole.WriteLine(\"Ich denke mir eine Zahl zwischen 1 und 100.\");\n\nwhile (tipp != geheim)\n{\n    Console.Write(\"Dein Tipp: \");\n    if (!int.TryParse(Console.ReadLine(), out tipp))\n    {\n        Console.WriteLine(\"Bitte eine Zahl eingeben!\");\n        continue;\n    }\n    versuche++;\n    if (tipp < geheim) Console.WriteLine(\"Zu klein!\");\n    else if (tipp > geheim) Console.WriteLine(\"Zu groß!\");\n}\nConsole.WriteLine($\"Richtig! Du hast {versuche} Versuche gebraucht.\");\n"),
        ("Klasse & Objekt", "Eine eigene Klasse mit Konstruktor.", "Haustier h = new Haustier(\"Mimi\", \"Katze\");\nh.Vorstellen();\n\nclass Haustier\n{\n    public string Name { get; }\n    public string Art { get; }\n\n    public Haustier(string name, string art)\n    {\n        Name = name;\n        Art = art;\n    }\n\n    public void Vorstellen()\n    {\n        Console.WriteLine($\"Ich bin {Name}, eine {Art}.\");\n    }\n}\n"),
        ("Klassischer Aufbau (Main)", "So sieht es mit namespace, class Program und Main aus.", "using System;\n\nnamespace MeinProgramm\n{\n    class Program\n    {\n        static void Main(string[] args)\n        {\n            Console.WriteLine(\"Hallo aus Main!\");\n        }\n    }\n}\n"),
    };

    void SeiteSpielwiese()
    {
        PruefenKnopf.Visibility = Visibility.Collapsed;
        SetzeEditorKontext("spielwiese", SpielwieseStart, Array.Empty<string>(), "Spielwiese");
        SeitenKopf("Code-Editor – Spielwiese", "Hier gibt es keine Aufgaben und nichts kann kaputtgehen. Schreib rechts beliebigen C#-Code und führe ihn mit F5 aus. Dein Spielwiesen-Code wird automatisch gespeichert.", "\uE943");

        SeitenInhalt.Children.Add(Ui.Text("Vorlagen zum Ausprobieren", 15.5, "TextHell", FontWeights.SemiBold, new Thickness(0, 0, 0, 10)));
        var raster = new UniformGrid { Columns = 3 };
        foreach (var (titel, beschreibung, code) in Vorlagen)
        {
            var sp = new StackPanel();
            sp.Children.Add(Ui.Text(titel, 13.5, "TextHell", FontWeights.SemiBold, new Thickness(0, 0, 0, 4)));
            sp.Children.Add(Ui.Text(beschreibung, 11.5, "TextGedimmt", rand: new Thickness(0, 0, 0, 10)));
            var vorlage = code;
            sp.Children.Add(Ui.Knopf("In den Editor laden", (_, _) => CodeInEditor(vorlage), false, "\uE768"));
            raster.Children.Add(Ui.Karte(sp, new Thickness(0, 0, 12, 12)));
        }
        SeitenInhalt.Children.Add(raster);
        SeitenInhalt.Children.Add(Ui.Hinweis("Keine Sorge", "Wenn du eine Vorlage lädst, holst du deinen alten Code mit **Strg+Z** zurück."));

        SeitenInhalt.Children.Add(Ui.Text("Tastenkürzel", 15.5, "TextHell", FontWeights.SemiBold, new Thickness(0, 8, 0, 10)));
        var tasten = new StackPanel();
        foreach (var (taste, was) in new[]
                 {
                     ("F5", "Kompilieren & Ausführen"), ("Umschalt+F5", "Programm stoppen"), ("F6", "Aufgabe prüfen (im Lernpfad)"),
                     ("Strg+S", "Speichern"), ("Strg+Z / Strg+Y", "Rückgängig / Wiederholen"), ("Strg+Mausrad", "Schrift größer/kleiner"),
                     ("Tab / Umschalt+Tab", "Einrücken / Ausrücken"), ("Strg+F", "Suchen im Code"),
                 })
        {
            var g = new Grid { Margin = new Thickness(0, 0, 0, 6) };
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(190) });
            g.ColumnDefinitions.Add(new ColumnDefinition());
            g.Children.Add(new Border
            {
                Background = Ui.B("InlineCode"), CornerRadius = new CornerRadius(0), Padding = new Thickness(8, 3, 8, 3), HorizontalAlignment = HorizontalAlignment.Left,
                Child = new TextBlock { Text = taste, FontFamily = Ui.CodeSchrift, FontSize = 11.5, Foreground = Ui.B("Akzent") },
            });
            var t = Ui.Text(was, 12.5, "TextMittel");
            t.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(t, 1);
            g.Children.Add(t);
            tasten.Children.Add(g);
        }
        SeitenInhalt.Children.Add(Ui.Karte(tasten));
    }

    // ---------------- Projekte ----------------

    void SeiteProjekte()
    {
        SeitenKopf("Projekte", "In den Projekten baust du richtige kleine Programme und wendest alles an, was du gelernt hast.", "\uE8B7");
        var raster = new UniformGrid { Columns = 3 };
        foreach (var l in lektionen.Where(l => l.Stufe == 5))
        {
            var sp = new StackPanel();
            sp.Children.Add(Ui.Text(l.Titel.Replace("Projekt: ", ""), 15.5, "TextHell", FontWeights.SemiBold, new Thickness(0, 0, 0, 6)));
            sp.Children.Add(Ui.Text(l.Einleitung, 11.5, "TextMittel", rand: new Thickness(0, 0, 0, 10)));
            sp.Children.Add(new ProgressBar { Maximum = Math.Max(1, l.Aufgaben.Count), Value = Geloest(l), Margin = new Thickness(0, 0, 0, 6) });
            sp.Children.Add(Ui.Text($"{Geloest(l)} von {l.Aufgaben.Count} Schritten erledigt", 11, IstAbgeschlossen(l) ? "Gruen" : "TextGedimmt", rand: new Thickness(0, 0, 0, 12)));
            var lektion = l;
            sp.Children.Add(Ui.Knopf(IstFreigeschaltet(l) ? "Projekt öffnen" : "🔒 Projekt öffnen", (_, _) => OeffneLektion(lektion, Geloest(lektion) > 0 ? "uebung" : "theorie"), IstFreigeschaltet(l), "\uE768"));
            raster.Children.Add(Ui.Karte(sp, new Thickness(0, 0, 12, 12)));
        }
        SeitenInhalt.Children.Add(raster);

        SeitenInhalt.Children.Add(Ui.Text("Weitere Projektideen für die Spielwiese", 15.5, "TextHell", FontWeights.SemiBold, new Thickness(0, 12, 0, 10)));
        var ideen = new StackPanel();
        foreach (var (titel, text) in new[]
                 {
                     ("Taschenrechner", "Zwei Zahlen und ein Rechenzeichen einlesen, Ergebnis ausgeben – mit Schutz gegen Division durch 0."),
                     ("Vokabeltrainer", "Eine Liste mit Wortpaaren abfragen und die richtigen Antworten zählen."),
                     ("Würfelspiel", "Mit Random.Shared.Next(1, 7) würfeln: Wer zuerst 20 Punkte hat, gewinnt."),
                     ("Kassensystem", "Artikel mit Preis einscannen (eingeben), Summe, Rabatt und Rückgeld berechnen."),
                     ("Tic-Tac-Toe", "Ein 3×3-Spielfeld als Array, zwei Spieler setzen abwechselnd X und O."),
                     ("Passwort-Prüfer", "Prüfen, ob ein Passwort lang genug ist und Ziffern sowie Großbuchstaben enthält."),
                 })
        {
            var g = new Grid { Margin = new Thickness(0, 0, 0, 10) };
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(28) });
            g.ColumnDefinitions.Add(new ColumnDefinition());
            g.Children.Add(Ui.Icon("\uEA80", 15, "Gelb"));
            var t = new TextBlock { TextWrapping = TextWrapping.Wrap, FontSize = 12.5, Foreground = Ui.B("TextMittel") };
            t.Inlines.Add(new System.Windows.Documents.Run(titel + ": ") { FontWeight = FontWeights.SemiBold, Foreground = Ui.B("TextHell") });
            t.Inlines.Add(new System.Windows.Documents.Run(text));
            Grid.SetColumn(t, 1);
            g.Children.Add(t);
            ideen.Children.Add(g);
        }
        ideen.Children.Add(Ui.Knopf("Auf der Spielwiese loslegen", (_, _) => NavEditor.IsChecked = true, true, "\uE943"));
        SeitenInhalt.Children.Add(Ui.Karte(ideen));
    }

    // ---------------- Compiler ----------------

    void SeiteCompiler()
    {
        SeitenKopf("Compiler", "Diese App hat einen echten C#-Compiler eingebaut – denselben (Roslyn), den auch Visual Studio benutzt.", "\uE90F");

        var schritte = new StackPanel();
        schritte.Children.Add(Ui.Text("So wird dein Code ausgeführt", 15.5, "TextHell", FontWeights.SemiBold, new Thickness(0, 0, 0, 10)));
        int n = 1;
        foreach (var (titel, text) in new[]
                 {
                     ("Du schreibst C#-Code", "Das ist der Quellcode in der Datei Program.cs."),
                     ("Der Compiler prüft und übersetzt", "Roslyn prüft die Grammatik (Syntax) und die Typen. Findet er Fehler, siehst du sie im Tab „Fehler“ – das Programm startet dann gar nicht."),
                     ("Zwischencode (IL)", "Ist alles in Ordnung, entsteht eine .NET-Assembly mit Zwischencode (IL) – hier direkt im Arbeitsspeicher."),
                     ("Die .NET-Laufzeit führt aus", ".NET 8 übersetzt den Zwischencode in Maschinencode und startet dein Programm. Die Ausgabe erscheint im Tab „Ausgabe“."),
                 })
        {
            var g = new Grid { Margin = new Thickness(0, 0, 0, 12) };
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(42) });
            g.ColumnDefinitions.Add(new ColumnDefinition());
            g.Children.Add(new Border
            {
                Width = 28, Height = 28, CornerRadius = new CornerRadius(0), Background = Ui.B("AkzentHintergrund"), BorderBrush = Ui.B("Akzent"), BorderThickness = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top,
                Child = new TextBlock { Text = (n++).ToString(), Foreground = Ui.B("Akzent"), FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center },
            });
            var sp = new StackPanel();
            sp.Children.Add(Ui.Text(titel, 13, "TextHell", FontWeights.SemiBold));
            sp.Children.Add(Ui.Text(text, 12, "TextMittel"));
            Grid.SetColumn(sp, 1);
            g.Children.Add(sp);
            schritte.Children.Add(g);
        }
        SeitenInhalt.Children.Add(Ui.Karte(schritte));

        var daten = new StackPanel();
        daten.Children.Add(Ui.Text("Technische Daten", 15.5, "TextHell", FontWeights.SemiBold, new Thickness(0, 0, 0, 10)));
        foreach (var (k, v) in new[]
                 {
                     ("Compiler", $"Roslyn (Microsoft.CodeAnalysis.CSharp) {CodeAusfuehrer.RoslynVersion}"),
                     ("Sprachversion", "C# 12"),
                     ("Laufzeit", $".NET {Environment.Version}"),
                     ("Programmart", "Konsolen-App · Top-Level-Anweisungen und klassisches static void Main"),
                     ("Implizite usings", "System, System.Collections.Generic, System.IO, System.Linq, System.Net.Http, System.Threading, System.Threading.Tasks"),
                     ("Zahlenformat", "Deutsch (de-DE): Ausgabe 3,5 – im Code schreibst du 3.5"),
                     ("Zeitlimit", $"{fortschritt.Zeitlimit} Sekunden (Wartezeit auf Eingaben zählt nicht) – änderbar in den Einstellungen"),
                     ("Nicht verfügbar", "Console.ReadKey wartet hier nicht (gibt sofort Enter zurück), Console.Clear leert die Ausgabe, Farben werden ignoriert"),
                 })
        {
            var g = new Grid { Margin = new Thickness(0, 0, 0, 8) };
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(170) });
            g.ColumnDefinitions.Add(new ColumnDefinition());
            g.Children.Add(Ui.Text(k, 12, "TextGedimmt"));
            var t = Ui.Text(v, 12, "TextHell");
            Grid.SetColumn(t, 1);
            g.Children.Add(t);
            daten.Children.Add(g);
        }
        SeitenInhalt.Children.Add(Ui.Karte(daten));

        var test = new StackPanel();
        var ergebnis = Ui.Text("Teste, ob der eingebaute Compiler funktioniert und wie schnell er ist.", 12, "TextMittel", rand: new Thickness(0, 0, 0, 10));
        test.Children.Add(Ui.Text("Compiler-Test", 15.5, "TextHell", FontWeights.SemiBold, new Thickness(0, 0, 0, 6)));
        test.Children.Add(ergebnis);
        test.Children.Add(Ui.Knopf("Compiler testen", async (_, _) =>
        {
            ergebnis.Text = "⏳ Teste …";
            var uhr = Stopwatch.StartNew();
            var r = await CodeAusfuehrer.AusfuehrenAsync("Console.WriteLine(\"Compiler funktioniert!\");", new Laufoptionen { Zeitlimit = TimeSpan.FromSeconds(10) });
            ergebnis.Text = r.Erfolgreich
                ? $"✓ {r.Ausgabe.Trim()}  –  übersetzt und ausgeführt in {uhr.Elapsed.TotalSeconds:0.00} s."
                : $"✗ Problem: {r.Ausgang} {r.FehlerText} {string.Join("; ", r.Meldungen.Select(m => m.Text))}";
            ergebnis.Foreground = r.Erfolgreich ? Ui.B("Gruen") : Ui.B("Rot");
            LogZeile("Compiler-Test: " + ergebnis.Text);
        }, true, "\uE768"));
        SeitenInhalt.Children.Add(Ui.Karte(test));
    }

    // ---------------- Begriffe ----------------

    void SeiteBegriffe()
    {
        SeitenKopf("Begriffe", $"{kurs.Begriffe.Count} Fachbegriffe einfach erklärt – von „deklarieren“ bis „Konstruktor“. Tipp: Lerne sie mit den Karteikarten!", "\uE82D");

        var karteHost = new StackPanel();
        var leiste = new WrapPanel { Margin = new Thickness(0, 0, 0, 14) };
        var suche = new TextBox { Width = 320, FontSize = 12.5, Padding = new Thickness(10, 7, 10, 7), BorderThickness = new Thickness(1), ToolTip = "Begriff suchen" };
        var platzhalter = Ui.Text("🔍  Begriff suchen …", 12.5, "TextGedimmt");
        platzhalter.IsHitTestVisible = false;
        platzhalter.Margin = new Thickness(12, 8, 0, 0);
        var sucheRaster = new Grid { Margin = new Thickness(0, 0, 12, 0) };
        sucheRaster.Children.Add(suche);
        sucheRaster.Children.Add(platzhalter);
        leiste.Children.Add(sucheRaster);
        leiste.Children.Add(Ui.Knopf("Karteikarten üben", (_, _) => Karteikarte(karteHost, new Random()), true, "\uE8FD"));
        SeitenInhalt.Children.Add(leiste);
        SeitenInhalt.Children.Add(karteHost);

        var liste = new StackPanel();
        SeitenInhalt.Children.Add(liste);

        void Fuellen()
        {
            liste.Children.Clear();
            string f = suche.Text.Trim();
            platzhalter.Visibility = f.Length == 0 ? Visibility.Visible : Visibility.Collapsed;
            var treffer = kurs.Begriffe.Where(b => f.Length == 0
                || b.Wort.Contains(f, StringComparison.OrdinalIgnoreCase)
                || b.Erklaerung.Contains(f, StringComparison.OrdinalIgnoreCase)).ToList();
            if (treffer.Count == 0) liste.Children.Add(Ui.Text("Kein Begriff gefunden.", 12.5, "TextGedimmt"));
            foreach (var b in treffer)
            {
                var sp = new StackPanel();
                sp.Children.Add(Ui.Text(b.Wort, 14.5, "Akzent", FontWeights.SemiBold, new Thickness(0, 0, 0, 4)));
                sp.Children.Add(Ui.Text(b.Erklaerung, 12.5, "TextMittel"));
                if (!string.IsNullOrWhiteSpace(b.Beispiel))
                    sp.Children.Add(new Border
                    {
                        Background = Ui.B("CodeHintergrund"), CornerRadius = new CornerRadius(0), Padding = new Thickness(10, 5, 10, 5), Margin = new Thickness(0, 8, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Child = new TextBlock { Text = b.Beispiel, FontFamily = Ui.CodeSchrift, FontSize = 11.5, Foreground = Ui.Farbe("#B3CCBB"), TextWrapping = TextWrapping.Wrap },
                    });
                liste.Children.Add(Ui.Karte(sp, new Thickness(0, 0, 0, 10), new Thickness(16, 12, 16, 12)));
            }
        }
        suche.TextChanged += (_, _) => Fuellen();
        Fuellen();
    }

    void Karteikarte(StackPanel host, Random zufall)
    {
        host.Children.Clear();
        if (kurs.Begriffe.Count == 0) return;
        var b = kurs.Begriffe[zufall.Next(kurs.Begriffe.Count)];
        var sp = new StackPanel();
        sp.Children.Add(Ui.Text("Karteikarte – was bedeutet …", 11, "TextGedimmt", rand: new Thickness(0, 0, 0, 6)));
        sp.Children.Add(Ui.Text(b.Wort, 21.5, "Akzent", FontWeights.Bold, new Thickness(0, 0, 0, 12)));
        var antwort = new StackPanel { Visibility = Visibility.Collapsed };
        antwort.Children.Add(Ui.Text(b.Erklaerung, 13.5, "TextHell"));
        if (!string.IsNullOrWhiteSpace(b.Beispiel))
            antwort.Children.Add(Ui.Text(b.Beispiel, 11.5, "TextMittel", rand: new Thickness(0, 8, 0, 0)));
        sp.Children.Add(antwort);
        var leiste = new WrapPanel { Margin = new Thickness(0, 14, 0, 0) };
        Button aufdecken = null;
        aufdecken = Ui.Knopf("Aufdecken", (_, _) => { antwort.Visibility = Visibility.Visible; aufdecken.Visibility = Visibility.Collapsed; }, true, "\uE7B3");
        leiste.Children.Add(aufdecken);
        leiste.Children.Add(Ui.Knopf("Nächste Karte", (_, _) => Karteikarte(host, zufall), false, "\uE76C"));
        leiste.Children.Add(Ui.Knopf("Schließen", (_, _) => host.Children.Clear(), false, "\uE711"));
        sp.Children.Add(leiste);
        host.Children.Add(Ui.Karte(sp, new Thickness(0, 0, 0, 18), new Thickness(22, 18, 22, 18)));
    }

    // ---------------- Fortschritt ----------------

    void SeiteFortschritt()
    {
        int fertig = lektionen.Count(IstAbgeschlossen);
        int aufgaben = lektionen.Sum(l => l.Aufgaben.Count);
        int geloest = lektionen.Sum(Geloest);
        int quizze = lektionen.Count(l => l.Quiz.Count > 0);
        int quizPerfekt = lektionen.Count(l => l.Quiz.Count > 0 && fortschritt.QuizPunkte.GetValueOrDefault(l.Id) == l.Quiz.Count);
        SeitenKopf("Fortschritt", $"Du lernst seit dem {fortschritt.ErsterStart:dd.MM.yyyy}. Weiter so!", "\uE9D2");

        var kacheln = new UniformGrid { Columns = 4 };
        foreach (var (zahl, text) in new[]
                 {
                     ($"{fertig}/{lektionen.Count}", "Lektionen abgeschlossen"),
                     ($"{geloest}/{aufgaben}", "Aufgaben gelöst"),
                     ($"{quizPerfekt}/{quizze}", "Quizze perfekt"),
                     ($"{fortschritt.Ausfuehrungen}", "Programme ausgeführt"),
                 })
        {
            var sp = new StackPanel();
            sp.Children.Add(Ui.Text(zahl, 23.5, "Akzent", FontWeights.Bold));
            sp.Children.Add(Ui.Text(text, 11.5, "TextMittel"));
            kacheln.Children.Add(Ui.Karte(sp, new Thickness(0, 0, 12, 12)));
        }
        SeitenInhalt.Children.Add(kacheln);

        var stufen = new StackPanel();
        stufen.Children.Add(Ui.Text("Fortschritt je Stufe", 15.5, "TextHell", FontWeights.SemiBold, new Thickness(0, 0, 0, 12)));
        foreach (var s in kurs.Stufen)
        {
            var inStufe = lektionen.Where(l => l.Stufe == s.Nummer).ToList();
            int a = inStufe.Sum(l => l.Aufgaben.Count), g = inStufe.Sum(Geloest);
            var zeile = new Grid { Margin = new Thickness(0, 0, 0, 4) };
            zeile.Children.Add(Ui.Text($"Stufe {s.Nummer}: {s.Titel}", 12.5, "TextHell"));
            var z = Ui.Text($"{g}/{a} Aufgaben", 11, g == a && a > 0 ? "Gruen" : "TextGedimmt");
            z.HorizontalAlignment = HorizontalAlignment.Right;
            zeile.Children.Add(z);
            stufen.Children.Add(zeile);
            stufen.Children.Add(new ProgressBar { Maximum = Math.Max(1, a), Value = g, Height = 8, Margin = new Thickness(0, 0, 0, 14) });
        }
        SeitenInhalt.Children.Add(Ui.Karte(stufen));

        var weiter = new StackPanel();
        weiter.Children.Add(Ui.Text("Als Nächstes", 15.5, "TextHell", FontWeights.SemiBold, new Thickness(0, 0, 0, 8)));
        // Zuerst die erste noch nicht abgeschlossene Lektion (5 von 8), danach offene Zusatz-Aufgaben von vorn.
        var offen = lektionen.SelectMany(l => l.Aufgaben.Select(a => (l, a))).Where(x => !IstGeloest(x.a)).ToList();
        var naechste = offen.FirstOrDefault(x => !IstAbgeschlossen(x.l));
        if (naechste.a == null) naechste = offen.FirstOrDefault();
        if (naechste.a != null)
        {
            weiter.Children.Add(Ui.Text($"Lektion „{naechste.l.Titel}“ – Aufgabe „{naechste.a.Titel}“", 12.5, "TextMittel", rand: new Thickness(0, 0, 0, 10)));
            weiter.Children.Add(Ui.Knopf("Weitermachen", (_, _) => OeffneLektion(naechste.l, "uebung", naechste.a), true, "\uE768"));
        }
        else weiter.Children.Add(Ui.Text("Du hast alle Aufgaben gelöst! 🎉 Auf der Spielwiese und bei den Projektideen kannst du weiter üben.", 12.5, "Gruen"));
        SeitenInhalt.Children.Add(Ui.Karte(weiter));
    }

    // ---------------- Erfolge ----------------

    void SeiteErfolge()
    {
        var erreicht = Erfolge.Erreicht(fortschritt, kurs);
        SeitenKopf("Erfolge", $"{erreicht.Count} von {Erfolge.Alle.Count} Erfolgen freigeschaltet.", "\uE734");
        var raster = new UniformGrid { Columns = 3 };
        foreach (var e in Erfolge.Alle)
        {
            bool hat = erreicht.Contains(e.Id);
            var g = new Grid();
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(62) });
            g.ColumnDefinitions.Add(new ColumnDefinition());
            g.Children.Add(new Border
            {
                Width = 48, Height = 48, CornerRadius = new CornerRadius(0),
                Background = Ui.Farbe(hat ? "#3A2F0E" : "#18211B"),
                BorderBrush = hat ? Ui.B("Gelb") : Ui.B("Rand"), BorderThickness = new Thickness(2),
                HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top,
                Child = new TextBlock { Text = hat ? e.Icon : "\uE72E", FontFamily = Ui.Icons, FontSize = 18, Foreground = hat ? Ui.B("Gelb") : Ui.B("TextGedimmt"), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center },
            });
            var sp = new StackPanel();
            sp.Children.Add(Ui.Text(e.Titel, 13.5, hat ? "TextHell" : "TextMittel", FontWeights.SemiBold));
            sp.Children.Add(Ui.Text(e.Beschreibung, 11.5, "TextGedimmt", rand: new Thickness(0, 2, 0, 4)));
            sp.Children.Add(Ui.Text(hat ? "✓ Freigeschaltet" : "Noch gesperrt", 11, hat ? "Gruen" : "TextGedimmt"));
            Grid.SetColumn(sp, 1);
            g.Children.Add(sp);
            var karte = Ui.Karte(g, new Thickness(0, 0, 12, 12));
            if (!hat) karte.Opacity = 0.75;
            raster.Children.Add(karte);
        }
        SeitenInhalt.Children.Add(raster);
    }

    // ---------------- Einstellungen ----------------

    void SeiteEinstellungen()
    {
        SeitenKopf("Einstellungen", "Passe die App an dich an.", "\uE713");

        var editor = new StackPanel();
        editor.Children.Add(Ui.Text("Editor", 15.5, "TextHell", FontWeights.SemiBold, new Thickness(0, 0, 0, 10)));
        var groesseText = Ui.Text($"Schriftgröße: {fortschritt.Schriftgroesse:0}", 12.5, "TextMittel");
        editor.Children.Add(groesseText);
        var groesse = new Slider { Minimum = 10, Maximum = 30, Value = fortschritt.Schriftgroesse, TickFrequency = 1, IsSnapToTickEnabled = true, Margin = new Thickness(0, 4, 0, 14), MaxWidth = 420, HorizontalAlignment = HorizontalAlignment.Left, Width = 420 };
        groesse.ValueChanged += (_, _) =>
        {
            fortschritt.Schriftgroesse = groesse.Value;
            Editor.FontSize = groesse.Value;
            groesseText.Text = $"Schriftgröße: {groesse.Value:0}";
            fortschritt.Speichern();
        };
        editor.Children.Add(groesse);

        var auto = new CheckBox { Content = "Autovervollständigung (Vorschläge beim Tippen, wie in Visual Studio)", IsChecked = fortschritt.Autovervollstaendigung, FontSize = 12.5, Foreground = Ui.B("TextHell"), Margin = new Thickness(0, 0, 0, 4) };
        auto.Click += (_, _) =>
        {
            fortschritt.Autovervollstaendigung = auto.IsChecked == true;
            if (!fortschritt.Autovervollstaendigung) vervollstaendiger?.Schliessen();
            fortschritt.Speichern();
        };
        editor.Children.Add(auto);
        editor.Children.Add(Ui.Text("Öffnet sich nach einem Punkt und beim ersten Buchstaben, sonst mit Strg+Leertaste. Tab oder Enter übernimmt, Esc schließt. Schließt außerdem ( [ \" automatisch.", 11, "TextGedimmt", rand: new Thickness(0, 0, 0, 14)));

        var zeitText = Ui.Text($"Zeitlimit für Programme: {fortschritt.Zeitlimit} Sekunden", 12.5, "TextMittel");
        editor.Children.Add(zeitText);
        var zeit = new Slider { Minimum = 3, Maximum = 60, Value = fortschritt.Zeitlimit, TickFrequency = 1, IsSnapToTickEnabled = true, Margin = new Thickness(0, 4, 0, 4), Width = 420, HorizontalAlignment = HorizontalAlignment.Left };
        zeit.ValueChanged += (_, _) =>
        {
            fortschritt.Zeitlimit = (int)zeit.Value;
            zeitText.Text = $"Zeitlimit für Programme: {fortschritt.Zeitlimit} Sekunden";
            fortschritt.Speichern();
        };
        editor.Children.Add(zeit);
        editor.Children.Add(Ui.Text("Läuft ein Programm länger (z. B. wegen einer Endlosschleife), wird es automatisch gestoppt. Wartezeit auf deine Eingaben zählt nicht mit.", 11, "TextGedimmt"));
        SeitenInhalt.Children.Add(Ui.Karte(editor));

        var optik = new StackPanel();
        optik.Children.Add(Ui.Text("Aussehen", 15.5, "TextHell", FontWeights.SemiBold, new Thickness(0, 0, 0, 10)));
        optik.Children.Add(Ui.Text("Matrixcode", 13, "TextHell", FontWeights.SemiBold, new Thickness(0, 0, 0, 6)));
        var regenAn = new CheckBox { Content = "Matrixcode im Hintergrund anzeigen", IsChecked = fortschritt.MatrixRegen, FontSize = 12.5 };
        var staerkeText = Ui.Text($"Helligkeit: {fortschritt.MatrixStufe * 100:0} %", 12.5, "TextMittel", rand: new Thickness(0, 12, 0, 0));
        var staerke = new Slider { Minimum = 0, Maximum = 1, Value = fortschritt.MatrixStufe, SmallChange = 0.05, LargeChange = 0.1, Margin = new Thickness(0, 4, 0, 4), Width = 420, HorizontalAlignment = HorizontalAlignment.Left, IsEnabled = fortschritt.MatrixRegen };
        var tempoText = Ui.Text($"Geschwindigkeit: {TempoText(fortschritt.MatrixTempo)}", 12.5, "TextMittel", rand: new Thickness(0, 8, 0, 0));
        var tempo = new Slider { Minimum = 0.3, Maximum = 2.5, Value = fortschritt.MatrixTempo, SmallChange = 0.1, LargeChange = 0.25, Margin = new Thickness(0, 4, 0, 4), Width = 420, HorizontalAlignment = HorizontalAlignment.Left, IsEnabled = fortschritt.MatrixRegen };
        einstMatrixAn = regenAn; einstMatrixRegler = staerke; einstMatrixText = staerkeText; einstMatrixTempo = tempo; einstMatrixTempoText = tempoText;
        regenAn.Click += (_, _) => SetzeMatrix(regenAn.IsChecked == true, fortschritt.MatrixStufe);
        staerke.ValueChanged += (_, _) => { if (!matrixSync) SetzeMatrix(fortschritt.MatrixRegen, staerke.Value); };
        tempo.ValueChanged += (_, _) => { if (!matrixSync) SetzeMatrix(fortschritt.MatrixRegen, fortschritt.MatrixStufe, tempo.Value); };
        optik.Children.Add(regenAn);
        optik.Children.Add(staerkeText);
        optik.Children.Add(staerke);
        optik.Children.Add(tempoText);
        optik.Children.Add(tempo);
        optik.Children.Add(Ui.Text("Zeichen bauen sich im Hintergrund auf und klingen ab – auch links im Menü regelbar. Ab 50 % Helligkeit werden die Flächen etwas durchsichtiger. Wird angehalten, wenn das Fenster minimiert ist.", 11, "TextGedimmt", rand: new Thickness(0, 0, 0, 0)));
        SeitenInhalt.Children.Add(Ui.Karte(optik));

        var lernen = new StackPanel();
        lernen.Children.Add(Ui.Text("Lernpfad", 15.5, "TextHell", FontWeights.SemiBold, new Thickness(0, 0, 0, 10)));
        var frei = new CheckBox { Content = "Alle Lektionen freischalten (z. B. wenn du im Kurs schon weiter bist)", IsChecked = fortschritt.AllesFreigeschaltet, FontSize = 12.5, Foreground = Ui.B("TextHell") };
        frei.Click += (_, _) =>
        {
            fortschritt.AllesFreigeschaltet = frei.IsChecked == true;
            fortschritt.Speichern();
            BaueLernpfad();
        };
        lernen.Children.Add(frei);
        lernen.Children.Add(Ui.Text("Normalerweise wird eine Lektion freigeschaltet, sobald du 5 Aufgaben der vorigen gelöst hast.", 11, "TextGedimmt", rand: new Thickness(0, 6, 0, 0)));
        SeitenInhalt.Children.Add(Ui.Karte(lernen));

        lernstandHost = new StackPanel();
        BaueLernstandKarte();
        SeitenInhalt.Children.Add(Ui.Karte(lernstandHost));

        var updates = new StackPanel();
        updates.Children.Add(Ui.Text("Updates", 15.5, "TextHell", FontWeights.SemiBold, new Thickness(0, 0, 0, 8)));
        var updateStart = new CheckBox { Content = "Beim Start nach Updates suchen", IsChecked = fortschritt.UpdateBeimStart, FontSize = 12.5, Foreground = Ui.B("TextHell"), Margin = new Thickness(0, 0, 0, 4) };
        updateStart.Click += (_, _) =>
        {
            fortschritt.UpdateBeimStart = updateStart.IsChecked == true;
            fortschritt.Speichern();
        };
        updates.Children.Add(updateStart);
        updates.Children.Add(Ui.Text("Fragt beim Start kurz bei GitHub nach, ob es eine neue Version gibt. Offline passiert einfach nichts. Dein Lernstand bleibt beim Aktualisieren erhalten.", 11, "TextGedimmt", rand: new Thickness(0, 0, 0, 10)));
        var updateLeiste = new WrapPanel();
        updateLeiste.Children.Add(Ui.Knopf("Jetzt nach Updates suchen", async (_, _) =>
        {
            await NachUpdatesSuchenAsync(manuell: true);
        }, false, "\uE895"));
        updates.Children.Add(updateLeiste);
        SeitenInhalt.Children.Add(Ui.Karte(updates));

        var ueber = new StackPanel();
        ueber.Children.Add(Ui.Text("Über", 15.5, "TextHell", FontWeights.SemiBold, new Thickness(0, 0, 0, 8)));
        string version = typeof(MainWindow).Assembly.GetName().Version is { } v ? $"{v.Major}.{v.Minor}.{v.Build}" : "1.0.0";
        ueber.Children.Add(Ui.Text($"SharpLab {version} · Entwickelt von Willi Gering · Gering Digital", 12.5, "TextHell", FontWeights.SemiBold, new Thickness(0, 0, 0, 4)));
        ueber.Children.Add(Ui.Text($"Der interaktive C#-Trainer · {lektionen.Count} Lektionen, {lektionen.Sum(l => l.Aufgaben.Count)} Aufgaben, {kurs.Begriffe.Count} Begriffe", 12, "TextMittel", rand: new Thickness(0, 0, 0, 10)));
        const string repoUrl = "https://github.com/willigering/SharpLab";
        var linkLeiste = new WrapPanel();
        linkLeiste.Children.Add(Ui.Knopf("Projekt auf GitHub", (_, _) =>
        {
            try { Process.Start(new ProcessStartInfo(repoUrl) { UseShellExecute = true }); }
            catch (Exception ex) { _ = DialogService.ShowAsync("Link nicht geöffnet", "Der Browser konnte nicht gestartet werden:\n" + ex.Message, DialogKind.Warnung, "OK", hint: repoUrl); }
        }, false, "\uE774"));
        var lizenzKnopf = Ui.Knopf("Lizenzen", (_, _) => ZeigeLizenzen(), false, "\uE8A5");
        lizenzKnopf.Margin = new Thickness(8, 0, 0, 0);
        linkLeiste.Children.Add(lizenzKnopf);
        var feedbackKnopf = Ui.Knopf("Feedback senden", (_, _) => _ = FeedbackOeffnen(), true, "\uED15");
        feedbackKnopf.Margin = new Thickness(8, 0, 0, 0);
        linkLeiste.Children.Add(feedbackKnopf);
        ueber.Children.Add(linkLeiste);
        ueber.Children.Add(Ui.Text(repoUrl, 11, "TextGedimmt", rand: new Thickness(0, 6, 0, 0)));
        ueber.Children.Add(Ui.Text("Schriftart: JetBrains Mono (SIL Open Font License) · Compiler: Roslyn (MIT) · Editor: AvalonEdit (MIT) · Details unter „Lizenzen“", 11, "TextGedimmt", rand: new Thickness(0, 4, 0, 0)));
        SeitenInhalt.Children.Add(Ui.Karte(ueber));
    }

    /// <summary>Zeigt die Lizenztexte der mitgelieferten Komponenten (THIRD-PARTY-NOTICES.txt).</summary>
    void ZeigeLizenzen()
    {
        static string Lies(string name)
        {
            try
            {
                using var s = typeof(MainWindow).Assembly.GetManifestResourceStream("CSharpLernen." + name);
                return s == null ? name + " wurde nicht gefunden." : new System.IO.StreamReader(s).ReadToEnd().Trim();
            }
            catch (Exception ex) { return name + " konnte nicht geladen werden: " + ex.Message; }
        }
        string linie = new string('=', 78);
        // Oben SharpLabs eigene MIT-Lizenz, danach die Hinweise zu Komponenten Dritter.
        string text = "SharpLab – Lizenz\n" + linie + "\n\n" + Lies("LICENSE") + "\n\n\n" + Lies("THIRD-PARTY-NOTICES.txt");
        var box = new TextBox
        {
            Text = text,
            IsReadOnly = true,
            TextWrapping = TextWrapping.Wrap,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            FontFamily = Ui.CodeSchrift,
            FontSize = 12,
            Background = Ui.B("Hintergrund"),
            Foreground = Ui.B("TextHell"),
            BorderThickness = new Thickness(0),
            Padding = new Thickness(14),
        };
        var fenster = new Window
        {
            Title = "SharpLab – Lizenzen",
            Width = 820,
            Height = 640,
            Owner = this,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Background = Ui.B("Hintergrund"),
            Foreground = Ui.B("TextHell"),
            FontFamily = Ui.UiSchrift,
            ShowInTaskbar = false,
            Content = box,
        };
        fenster.PreviewKeyDown += (_, e) => { if (e.Key == System.Windows.Input.Key.Escape) fenster.Close(); };
        fenster.Show();
    }
}
