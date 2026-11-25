using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Xml;
using CSharpLernen.Engine;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Indentation.CSharp;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.Win32;

namespace CSharpLernen;

public partial class MainWindow : Window
{
    readonly Kurs kurs;
    readonly List<Lektion> lektionen;
    readonly Fortschritt fortschritt;
    readonly bool geladen;

    // Editor-Zustand
    string editorSchluessel;
    string editorStandardCode = "";
    bool editorGeaendert;
    bool ladeEditor;
    FehlerMarkierer markierer;

    // Lauf-Zustand
    bool laeuft;
    /// <summary>Nummer des aktuellen Laufs. Wird beim Kontextwechsel erhöht – Ausgaben und Ergebnisse älterer Läufe werden dann verworfen.</summary>
    volatile int laufNr;
    /// <summary>Nur während eine Übung nach dem Prüfen neu aufgebaut wird: Ausgabe der Prüfung stehen lassen.</summary>
    bool ausgabeBehalten;
    CancellationTokenSource laufCts;
    BlockingCollection<string> eingabeWarteschlange;
    readonly ConcurrentQueue<string> ausgabePuffer = new();
    const string LoeschMarke = "\u0000LOESCHEN\u0000";
    readonly DispatcherTimer ausgabeTimer;
    bool letzterLaufKompilierfehler;

    HashSet<string> bekannteErfolge;
    readonly DispatcherTimer toastTimer;
    string seite = "lernpfad";
    CodeNotizenSeite notizSeite;

    public MainWindow()
    {
        InitializeComponent();
        DialogService.Host = Wurzel;
        PruefenKnopf.IsVisibleChanged += (_, _) => LaufLeisteAnpassen();
        fortschritt = Fortschritt.Laden();
        SpeicherortHinweiseEinrichten();
        matrixSpeichern = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
        matrixSpeichern.Tick += (_, _) => { matrixSpeichern.Stop(); fortschritt.Speichern(); };
        Farbdesign.Anwenden(fortschritt.Design);
        SetzeMatrix(fortschritt.MatrixRegenAktiv(), fortschritt.MatrixStufe, fortschritt.MatrixTempo, speichern: false);
        kurs = LadeKurs();
        lektionen = kurs.Lektionen;

        EditorEinrichten();

        ausgabeTimer = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromMilliseconds(60) };
        ausgabeTimer.Tick += (_, _) => AusgabeFlush();
        toastTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4.5) };
        toastTimer.Tick += (_, _) => ToastAusblenden();

        bekannteErfolge = Erfolge.Erreicht(fortschritt, kurs);

        Loaded += (_, _) =>
        {
            DunklerTitel();
            if (SystemParameters.WorkArea.Width < 1660 || SystemParameters.WorkArea.Height < 960)
                WindowState = WindowState.Maximized;
            Task.Run(CodeAusfuehrer.Vorwaermen);
            UpdateBeimStartEinrichten();
        };
        PreviewKeyDown += Tasten;
        Closing += (_, e) =>
        {
            if (notizSeite?.EntwurfAbschliessen() == false)
            {
                e.Cancel = true;
                return;
            }
            SpeichereEditor();
            matrixSpeichern?.Stop();
            fortschritt.Speichern();
            try { laufCts?.Cancel(); } catch { }
        };

        geladen = true;

        Lektion start = lektionen.FirstOrDefault(l => l.Id == fortschritt.LetzteLektion)
                        ?? lektionen.FirstOrDefault(l => !IstAbgeschlossen(l))
                        ?? lektionen.FirstOrDefault();
        if (start != null)
        {
            offeneStufen.Add(start.Stufe);
            ZeigeLektion(start, "theorie");
        }
        AktualisiereFortschritt();
        EingabeZaehlerAktualisieren();
    }

    Kurs LadeKurs()
    {
        string eigene = Path.Combine(Fortschritt.Ordner, "Lektionen.json");
        try
        {
            var k = KursLader.Laden(eigene);
            if (k.Lektionen.Count > 0) return k;
        }
        catch (Exception ex)
        {
            _ = DialogService.ShowAsync("Eigene Lektionen fehlerhaft",
                "Die Datei „Lektionen.json“ in deinem Datenordner konnte nicht gelesen werden:\n" + ex.Message,
                DialogKind.Warnung, "Verstanden", hint: "Es werden die eingebauten Lektionen verwendet. Datei: " + eigene);
        }
        return KursLader.Laden();
    }

    // ===================================================== Fenster

    [DllImport("dwmapi.dll")]
    static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    void DunklerTitel()
    {
        try
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            int an = 1;
            if (DwmSetWindowAttribute(hwnd, 20, ref an, sizeof(int)) != 0)
                DwmSetWindowAttribute(hwnd, 19, ref an, sizeof(int));
        }
        catch { }
    }

    void Tasten(object sender, KeyEventArgs e)
    {
        if (DialogService.IstOffen) return;   // Dialog bekommt Enter/Esc selbst
        if (e.Key == Key.F5 && Keyboard.Modifiers == ModifierKeys.Shift) { Stoppen(); e.Handled = true; }
        else if (e.Key == Key.F5) { _ = Ausfuehren(); e.Handled = true; }
        else if (e.Key == Key.F6) { _ = Pruefen(); e.Handled = true; }
        else if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control && notizSeite?.IsKeyboardFocusWithin != true)
        { Speichern(); e.Handled = true; }
    }

    // ===================================================== Editor

    void EditorEinrichten()
    {
        try
        {
            using var s = typeof(MainWindow).Assembly.GetManifestResourceStream("CSharpLernen.CSharpDunkel.xshd");
            using var r = XmlReader.Create(s!);
            Editor.SyntaxHighlighting = HighlightingLoader.Load(r, HighlightingManager.Instance);
        }
        catch
        {
            Editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
        }

        _ = new Klammerfarben(Editor);
        Editor.Options.ConvertTabsToSpaces = true;
        Editor.Options.IndentationSize = 4;
        Editor.Options.EnableHyperlinks = false;
        Editor.Options.EnableEmailHyperlinks = false;
        Editor.Options.HighlightCurrentLine = true;
        Editor.TextArea.TextView.CurrentLineBackground = Ui.B("EditorZeile");
        var rand = new Pen(Ui.B("EditorZeilenRand"), 1);
        Editor.TextArea.TextView.CurrentLineBorder = rand;
        Editor.TextArea.SelectionBrush = Ui.B("EditorAuswahl");
        Editor.TextArea.SelectionBorder = null;
        Editor.TextArea.SelectionForeground = null;
        Editor.TextArea.Caret.CaretBrush = Ui.B("Akzent");
        Editor.TextArea.IndentationStrategy = new CSharpIndentationStrategy(Editor.Options);
        Editor.FontSize = fortschritt.Schriftgroesse;

        vervollstaendiger = new CodeVervollstaendiger(Editor, () => fortschritt.Autovervollstaendigung);

        markierer = new FehlerMarkierer();
        Editor.TextArea.TextView.BackgroundRenderers.Add(markierer);

        Editor.TextChanged += (_, _) =>
        {
            if (ladeEditor) return;
            editorGeaendert = true;
            UngespeichertPunkt.Visibility = Visibility.Visible;
            if (markierer != null) { markierer.Leeren(); Editor.TextArea.TextView.InvalidateLayer(KnownLayer.Background); }
        };
        Editor.PreviewMouseWheel += (_, e) =>
        {
            if (Keyboard.Modifiers != ModifierKeys.Control) return;
            SchriftAendern(e.Delta > 0 ? 1 : -1);
            e.Handled = true;
        };
    }

    void SchriftAendern(double delta)
    {
        fortschritt.Schriftgroesse = Math.Clamp(fortschritt.Schriftgroesse + delta, 10, 30);
        Editor.FontSize = fortschritt.Schriftgroesse;
    }

    /// <summary>Wechselt, welcher Code im Editor steht (Beispiel, Aufgabe, Spielwiese). Der alte Code wird gespeichert.</summary>
    void SetzeEditorKontext(string schluessel, string standardCode, IEnumerable<string> eingabe, string beschreibung)
    {
        if (schluessel == editorSchluessel)
        {
            // Gleiche Aufgabe erneut geöffnet (z. B. zurück vom Quiz oder aus den Einstellungen):
            // auch dann startet die Konsole leer – außer direkt nach dem Prüfen.
            if (!ausgabeBehalten) KonsoleZuruecksetzen();
            return;
        }
        SpeichereEditor();
        editorSchluessel = schluessel;
        editorStandardCode = standardCode ?? "";
        vervollstaendiger?.Schliessen();
        ladeEditor = true;
        Editor.Text = fortschritt.Code.TryGetValue(schluessel, out var c) ? c : editorStandardCode;
        Editor.Document.UndoStack.ClearAll();
        Editor.ScrollToHome();
        ladeEditor = false;
        editorGeaendert = false;
        UngespeichertPunkt.Visibility = Visibility.Collapsed;
        EditorKontext.Text = beschreibung;
        EditorKopfAnpassen();
        // Konsole startet leer: keine Ausgabe vom vorigen Lauf, keine vorausgefüllten Eingaben
        // (Console.ReadLine fragt beim Ausführen unten nach).
        EingabeBox.Text = "";
        KonsoleZuruecksetzen();
        markierer.Leeren();
        Editor.TextArea.TextView.InvalidateLayer(KnownLayer.Background);
    }

    void SpeichereEditor()
    {
        if (editorSchluessel == null || !editorGeaendert) return;
        if (Editor.Text == editorStandardCode) fortschritt.Code.Remove(editorSchluessel);
        else fortschritt.Code[editorSchluessel] = Editor.Text;
        editorGeaendert = false;
        UngespeichertPunkt.Visibility = Visibility.Collapsed;
        fortschritt.Speichern();
    }

    void Speichern()
    {
        editorGeaendert = true;
        SpeichereEditor();
        fortschritt.Speichern();
        Toast("Gespeichert", "Dein Code ist gesichert.", "\uE74E", "Akzent");
    }

    /// <summary>Ersetzt den Editor-Inhalt (rückgängig machbar mit Strg+Z).</summary>
    void CodeInEditor(string code)
    {
        Editor.Document.Text = code;
        Editor.ScrollToHome();
        Editor.Focus();
    }

    void GeheZuZeile(int zeile, int spalte)
    {
        if (zeile <= 0 || zeile > Editor.Document.LineCount) return;
        Editor.TextArea.Caret.Line = zeile;
        Editor.TextArea.Caret.Column = Math.Max(1, spalte);
        Editor.ScrollTo(zeile, spalte);
        var line = Editor.Document.GetLineByNumber(zeile);
        Editor.Select(line.Offset, line.Length);
        Editor.Focus();
    }

    void CodeAlsNotiz_Click(object sender, RoutedEventArgs e)
    {
        NavNotizen.IsChecked = true;
        notizSeite.EditorUebernehmen();
    }

    void Speichern_Click(object sender, RoutedEventArgs e) => Speichern();
    void Rueckgaengig_Click(object sender, RoutedEventArgs e) { Editor.Undo(); Editor.Focus(); }
    void Wiederholen_Click(object sender, RoutedEventArgs e) { Editor.Redo(); Editor.Focus(); }
    void SchriftKleiner_Click(object sender, RoutedEventArgs e) => SchriftAendern(-1);
    void SchriftGroesser_Click(object sender, RoutedEventArgs e) => SchriftAendern(1);
    void EinstellungenOeffnen_Click(object sender, RoutedEventArgs e) => NavEinstellungen.IsChecked = true;

    async void Zuruecksetzen_Click(object sender, RoutedEventArgs e)
    {
        if (await DialogService.ShowAsync("Code zurücksetzen?", "Der Code im Editor wird auf den Startcode zurückgesetzt.",
                DialogKind.Frage, "Zurücksetzen", "Abbrechen", "Tipp: Mit Strg+Z bekommst du deinen Code danach wieder zurück.") == DialogAntwort.Primaer)
            CodeInEditor(editorStandardCode);
    }

    void DateiOeffnen_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog { Filter = "C#-Dateien (*.cs)|*.cs|Textdateien (*.txt)|*.txt|Alle Dateien (*.*)|*.*", Title = "C#-Datei öffnen" };
        if (dlg.ShowDialog(this) == true)
        {
            try { CodeInEditor(File.ReadAllText(dlg.FileName)); }
            catch (Exception ex) { _ = DialogService.ShowAsync("Datei nicht geöffnet", "Die Datei konnte nicht geöffnet werden:\n" + ex.Message, DialogKind.Gefahr, "OK"); }
        }
    }

    void DateiExport_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new SaveFileDialog { Filter = "C#-Dateien (*.cs)|*.cs", FileName = "Program.cs", Title = "Code als Datei speichern" };
        if (dlg.ShowDialog(this) == true)
        {
            try
            {
                File.WriteAllText(dlg.FileName, Editor.Text, new UTF8Encoding(true));
                Toast("Exportiert", Path.GetFileName(dlg.FileName) + " wurde gespeichert.", "\uE896", "Akzent");
            }
            catch (Exception ex) { _ = DialogService.ShowAsync("Nicht gespeichert", "Die Datei konnte nicht gespeichert werden:\n" + ex.Message, DialogKind.Gefahr, "OK"); }
        }
    }

    // ===================================================== Ausführen

    void Ausfuehren_Click(object sender, RoutedEventArgs e) => _ = Ausfuehren();

    // ===================================================== Matrix-Regen (Menü + Einstellungen)

    CodeVervollstaendiger vervollstaendiger;
    DispatcherTimer matrixSpeichern;
    bool matrixSync;
    CheckBox einstMatrixAn;          // Steuerelemente der Einstellungs-Seite (falls gerade offen)
    Slider einstMatrixRegler, einstMatrixTempo;
    TextBlock einstMatrixText, einstMatrixTempoText;

    static string TempoText(double t) => t.ToString("0.0", CodeAusfuehrer.Deutsch) + "×";

    /// <summary>Eine Stelle für den Matrixcode: Hintergrund, Panel-Deckkraft, Speichern und alle Regler synchron halten.</summary>
    void SetzeMatrix(bool an, double stufe, double? tempo = null, bool speichern = true)
    {
        stufe = Math.Clamp(stufe, 0, 1);
        double t = Math.Clamp(tempo ?? fortschritt.MatrixTempo, 0.3, 2.5);
        fortschritt.MatrixRegenSetzen(an);
        fortschritt.MatrixStufe = stufe;
        fortschritt.MatrixTempo = t;
        Regen.Staerke = stufe;
        Regen.Tempo = t;
        Regen.Visibility = an ? Visibility.Visible : Visibility.Collapsed;
        App.SetzeFlaechenDeckkraft(stufe, an);

        matrixSync = true;
        try
        {
            NavMatrixAn.IsChecked = an;
            if (Math.Abs(NavMatrixRegler.Value - stufe) > 0.0001) NavMatrixRegler.Value = stufe;
            if (Math.Abs(NavMatrixTempo.Value - t) > 0.0001) NavMatrixTempo.Value = t;
            NavMatrixRegler.IsEnabled = an;
            NavMatrixTempo.IsEnabled = an;
            NavMatrixWert.Text = an ? $"{stufe * 100:0} %" : "aus";
            NavMatrixTempoWert.Text = an ? TempoText(t) : "aus";
            if (einstMatrixAn != null) einstMatrixAn.IsChecked = an;
            if (einstMatrixRegler != null)
            {
                if (Math.Abs(einstMatrixRegler.Value - stufe) > 0.0001) einstMatrixRegler.Value = stufe;
                einstMatrixRegler.IsEnabled = an;
            }
            if (einstMatrixTempo != null)
            {
                if (Math.Abs(einstMatrixTempo.Value - t) > 0.0001) einstMatrixTempo.Value = t;
                einstMatrixTempo.IsEnabled = an;
            }
            if (einstMatrixText != null) einstMatrixText.Text = $"Helligkeit: {stufe * 100:0} %";
            if (einstMatrixTempoText != null) einstMatrixTempoText.Text = $"Geschwindigkeit: {TempoText(t)}";
        }
        finally { matrixSync = false; }

        if (speichern) { matrixSpeichern.Stop(); matrixSpeichern.Start(); }   // beim Ziehen nicht ständig schreiben
    }

    void NavMatrixAn_Click(object sender, RoutedEventArgs e) => SetzeMatrix(NavMatrixAn.IsChecked == true, fortschritt.MatrixStufe);

    void NavMatrixRegler_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (matrixSync || matrixSpeichern == null) return;
        SetzeMatrix(fortschritt.MatrixRegenAktiv(), NavMatrixRegler.Value);
    }

    void NavMatrixTempo_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (matrixSync || matrixSpeichern == null) return;
        SetzeMatrix(fortschritt.MatrixRegenAktiv(), fortschritt.MatrixStufe, NavMatrixTempo.Value);
    }

    /// <summary>Bei schmalem Editor: Knöpfe kompakter (nur Symbol), damit nichts abgeschnitten wird.</summary>
    void LaufLeiste_SizeChanged(object sender, SizeChangedEventArgs e) => LaufLeisteAnpassen();

    void LaufLeisteAnpassen()
    {
        double b = LaufLeiste.ActualWidth;
        if (b <= 0) return;
        // Stufe 0 = alles, 1 = ohne „F5“ und Stopp nur als Symbol, 2 = zusätzlich Prüfen als Symbol und kurzer Text
        for (int stufe = 0; stufe <= 2; stufe++)
        {
            AusfuehrenF5.Visibility = stufe >= 1 ? Visibility.Collapsed : Visibility.Visible;
            StoppText.Visibility = stufe >= 1 ? Visibility.Collapsed : Visibility.Visible;
            StoppIcon.Margin = new Thickness(0, 2, stufe >= 1 ? 0 : 8, 0);
            PruefenText.Visibility = stufe >= 2 ? Visibility.Collapsed : Visibility.Visible;
            PruefenIcon.Margin = new Thickness(0, 2, stufe >= 2 ? 0 : 8, 0);
            AusfuehrenText.Text = stufe >= 2 ? "Ausführen" : "Kompilieren & Ausführen";
            if (Bedarf(LaufLeiste) <= b) break;
        }
    }

    /// <summary>Gesamtbreite, die die sichtbaren Kinder eines Panels nebeneinander brauchen.</summary>
    static double Bedarf(Panel p)
    {
        double summe = 0;
        foreach (UIElement c in p.Children)
        {
            if (c.Visibility != Visibility.Visible) continue;
            c.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            summe += c.DesiredSize.Width;
        }
        return summe;
    }

    double navFortschrittHoehe = 118;

    /// <summary>Bei sehr niedrigem Fenster: Fortschritts-Karte links ausblenden, damit das Menü nicht abgeschnitten wird.</summary>
    void NavRaster_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        double frei = NavRaster.RowDefinitions[1].ActualHeight;
        NavListe.Measure(new Size(NavListe.ActualWidth > 0 ? NavListe.ActualWidth : double.PositiveInfinity, double.PositiveInfinity));
        double noetig = NavListe.DesiredSize.Height;
        if (NavFortschrittKarte.Visibility == Visibility.Visible)
        {
            if (noetig > frei + 0.5)
            {
                navFortschrittHoehe = NavFortschrittKarte.ActualHeight + 8;
                NavFortschrittKarte.Visibility = Visibility.Collapsed;
            }
        }
        else if (frei - noetig > navFortschrittHoehe + 4)
            NavFortschrittKarte.Visibility = Visibility.Visible;
    }

    void LektionSpalte_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        bool schmal = LektionSpalte.ActualWidth < 430;
        VorherigeText.Text = schmal ? "Zurück" : "Vorherige Lektion";
        NaechsteText.Text = schmal ? "Weiter" : "Nächste Lektion";
    }

    void EditorKopf_SizeChanged(object sender, SizeChangedEventArgs e) => EditorKopfAnpassen();

    void EditorKopfAnpassen()
    {
        double b = EditorKopf.ActualWidth;
        if (b <= 0) return;
        for (int stufe = 0; stufe <= 2; stufe++)
        {
            EditorKontext.Visibility = stufe >= 1 ? Visibility.Collapsed : Visibility.Visible;
            var v = stufe >= 2 ? Visibility.Collapsed : Visibility.Visible;
            KnopfKleiner.Visibility = v; KnopfGroesser.Visibility = v; KnopfOeffnen.Visibility = v;
            if (Bedarf(EditorKopf) + 8 <= b) break;
        }
    }
    void Stopp_Click(object sender, RoutedEventArgs e) => Stoppen();
    void Pruefen_Click(object sender, RoutedEventArgs e) => _ = Pruefen();

    void Stoppen()
    {
        if (!laeuft) return;
        try { laufCts?.Cancel(); } catch { }
        Status("■ Stoppe Programm …", "TextGedimmt");
    }

    int LaufStart()
    {
        int nr = ++laufNr;
        laeuft = true;
        laufCts = new CancellationTokenSource();
        eingabeWarteschlange = new BlockingCollection<string>();
        AusfuehrenKnopf.IsEnabled = false;
        PruefenKnopf.IsEnabled = false;
        StoppKnopf.IsEnabled = true;
        LaufMeldung.Visibility = Visibility.Collapsed;
        while (ausgabePuffer.TryDequeue(out _)) { }
        AusgabeBox.Clear();
        ausgabeTimer.Start();
        return nr;
    }

    void LaufEnde()
    {
        ausgabeTimer.Stop();
        AusgabeFlush();
        laeuft = false;
        AusfuehrenKnopf.IsEnabled = true;
        PruefenKnopf.IsEnabled = true;
        StoppKnopf.IsEnabled = false;
        EingabeZeile.Visibility = Visibility.Collapsed;
        eingabeWarteschlange?.CompleteAdding();
    }

    List<string> VorabEingaben()
    {
        var zeilen = EingabeBox.Text.Replace("\r\n", "\n").Split('\n').ToList();
        while (zeilen.Count > 0 && zeilen[^1].Length == 0) zeilen.RemoveAt(zeilen.Count - 1);
        return zeilen;
    }

    async Task Ausfuehren()
    {
        if (laeuft) return;
        SpeichereEditor();
        string code = Editor.Text;
        var eingaben = VorabEingaben();
        int nr = LaufStart();
        TabAusgabe.IsChecked = true;
        Status("⏳ Kompiliere …", "TextMittel");
        LogZeile($"Kompiliere {CodeAusfuehrer.Dateiname} ({Editor.Document.LineCount} Zeilen) · C# 12 · .NET 8 · implizite usings: System, System.Collections.Generic, System.IO, System.Linq, …");
        if (eingaben.Count > 0) LogZeile($"Vorab-Eingaben: {eingaben.Count} Zeile(n)");

        var uhr = Stopwatch.StartNew();
        Laufergebnis r;
        try
        {
            r = await CodeAusfuehrer.AusfuehrenAsync(code, new Laufoptionen
            {
                Eingabe = eingaben,
                InteraktiveEingabe = WarteAufEingabe,
                BeiAusgabe = s => { if (nr == laufNr) ausgabePuffer.Enqueue(s); },
                BeiLoeschen = () => { if (nr == laufNr) ausgabePuffer.Enqueue(LoeschMarke); },
                BeiStart = () => Dispatcher.BeginInvoke(() =>
                {
                    if (nr != laufNr) return;
                    Status("▶ Programm läuft … (Stopp mit Umschalt+F5)", "Akzent");
                    LogZeile($"Kompilierung erfolgreich nach {uhr.Elapsed.TotalSeconds:0.00} s – Programm startet");
                }),
                Zeitlimit = TimeSpan.FromSeconds(fortschritt.Zeitlimit),
                Abbruch = laufCts.Token,
            });
        }
        finally
        {
            LaufEnde();
        }
        // Inzwischen eine andere Aufgabe geöffnet? Dann gehört nichts davon in die (leere) Konsole.
        if (nr != laufNr) return;

        fortschritt.Ausfuehrungen++;
        if (editorSchluessel == "spielwiese") fortschritt.SpielwieseLaeufe++;
        ZeigeLaufergebnis(r, uhr.Elapsed);
        fortschritt.Speichern();
        PruefeErfolge();
    }

    string WarteAufEingabe(CancellationToken token)
    {
        var q = eingabeWarteschlange;
        Dispatcher.BeginInvoke(() =>
        {
            AusgabeFlush();
            TabAusgabe.IsChecked = true;
            EingabeZeile.Visibility = Visibility.Visible;
            Status("⌨ Dein Programm wartet auf eine Eingabe (Console.ReadLine) – tippe sie unten ein und drück Enter.", "Akzent");
            InteraktivBox.Focus();
        });
        try { return q.Take(token); }
        catch (OperationCanceledException) { return null; }
        catch (InvalidOperationException) { return null; }
        finally
        {
            Dispatcher.BeginInvoke(() =>
            {
                EingabeZeile.Visibility = Visibility.Collapsed;
                if (laeuft) Status("▶ Programm läuft … (Stopp mit Umschalt+F5)", "Akzent");
            });
        }
    }

    void EingabeSenden()
    {
        if (!laeuft || eingabeWarteschlange == null || eingabeWarteschlange.IsAddingCompleted) return;
        string text = InteraktivBox.Text;
        InteraktivBox.Clear();
        AusgabeFlush();
        AusgabeBox.AppendText(text + Environment.NewLine); // Echo wie in der echten Konsole
        AusgabeBox.ScrollToEnd();
        try { eingabeWarteschlange.Add(text); } catch (InvalidOperationException) { }
    }

    void InteraktivBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) { EingabeSenden(); e.Handled = true; }
    }

    void EingabeSenden_Click(object sender, RoutedEventArgs e) => EingabeSenden();

    void AusgabeFlush()
    {
        if (ausgabePuffer.IsEmpty) return;
        var sb = new StringBuilder();
        while (ausgabePuffer.TryDequeue(out var s))
        {
            if (s == LoeschMarke) { AusgabeBox.Clear(); sb.Clear(); continue; }
            sb.Append(s);
        }
        if (sb.Length == 0) return;
        AusgabeBox.AppendText(sb.ToString());
        if (AusgabeBox.Text.Length > 400_000)
            AusgabeBox.Text = "… (ältere Ausgabe gekürzt)\n" + AusgabeBox.Text[^300_000..];
        AusgabeBox.ScrollToEnd();
    }

    /// <summary>Leert die Konsole beim Wechsel der Aufgabe/Lektion/Seite. Läuft noch ein Programm (z. B. eines, das auf
    /// Console.ReadLine wartet), wird es gestoppt und seine restliche Ausgabe verworfen.</summary>
    void KonsoleZuruecksetzen()
    {
        bool gestoppt = false;
        if (laeuft)
        {
            laufNr++;
            try { laufCts?.Cancel(); } catch { }
            gestoppt = true;
        }
        while (ausgabePuffer.TryDequeue(out _)) { }
        EingabeZeile.Visibility = Visibility.Collapsed;
        InteraktivBox.Clear();
        KonsoleLeeren();
        if (gestoppt) Status("■ Das vorige Programm wurde gestoppt. Bereit für F5.", "TextGedimmt");
    }

    void KonsoleLeeren()
    {
        AusgabeBox.Clear();
        LaufMeldung.Visibility = Visibility.Collapsed;
        FehlerListe.Children.Clear();
        TabFehler.Content = "Fehler (0)";
        Status("Bereit. Klick auf „Kompilieren & Ausführen“ oder drück F5.", "TextGedimmt");
        if (TabAusgabe.IsChecked != true && TabEingabe.IsChecked != true) TabAusgabe.IsChecked = true;
    }

    void AusgabeLeeren_Click(object sender, RoutedEventArgs e)
    {
        AusgabeBox.Clear();
        LaufMeldung.Visibility = Visibility.Collapsed;
    }

    void Status(string text, string farbe)
    {
        StatusZeile.Text = text;
        StatusZeile.Foreground = Ui.B(farbe);
    }

    void LaufMeldungZeigen(string text, bool warnung)
    {
        LaufMeldungText.Text = text;
        LaufMeldung.Background = Ui.Farbe(warnung ? "#2A2210" : "#2A1414");
        LaufMeldung.BorderBrush = Ui.Farbe(warnung ? "#6B5520" : "#6B2B2B");
        LaufMeldungText.Foreground = Ui.Farbe(warnung ? "#FFE2A3" : "#FFC4C4");
        LaufMeldung.Visibility = Visibility.Visible;
    }

    void ZeigeLaufergebnis(Laufergebnis r, TimeSpan gesamt)
    {
        ZeigeMeldungen(r.Meldungen);
        foreach (var m in r.Meldungen)
            LogZeile($"{(m.IstWarnung ? "warning" : "error")} {m.Code} (Zeile {m.Zeile}, Spalte {m.Spalte}): {m.Text}");

        if (r.Ausgang == Ausgang.Kompilierfehler)
        {
            int n = r.Meldungen.Count(m => !m.IstWarnung);
            Status($"✗ Kompilierung fehlgeschlagen – {n} Fehler. Klick im Tab „Fehler“ auf einen Eintrag, um zur Zeile zu springen.", "Rot");
            LogZeile($"Kompilierung fehlgeschlagen: {n} Fehler ({gesamt.TotalSeconds:0.00} s)");
            TabFehler.IsChecked = true;
            letzterLaufKompilierfehler = true;
            return;
        }

        if (letzterLaufKompilierfehler) fortschritt.FehlerBehoben = true;
        letzterLaufKompilierfehler = false;
        string sek = $"{r.Dauer.TotalSeconds:0.00} s";
        string warnHinweis = r.Meldungen.Count > 0 ? $" · {r.Meldungen.Count} Warnung(en) im Tab „Fehler“" : "";

        switch (r.Ausgang)
        {
            case Ausgang.Erfolg:
                fortschritt.ErfolgreicheLaeufe++;
                Status($"✓ Kompilierung erfolgreich · Programm beendet nach {sek}{warnHinweis}", "Gruen");
                if (r.Ausgabe.Length == 0) AusgabeBox.Text = "(Dein Programm hat nichts ausgegeben.)";
                break;
            case Ausgang.Laufzeitfehler:
                Status($"⚠ Laufzeitfehler{(r.FehlerZeile is int z ? $" in Zeile {z}" : "")} – das Programm wurde abgebrochen.", "Gelb");
                LaufMeldungZeigen($"Laufzeitfehler{(r.FehlerZeile is int z2 ? $" in Zeile {z2}" : "")}: {r.FehlerText}" + (r.Tipp != null ? $"\nTipp: {r.Tipp}" : ""), false);
                if (r.FehlerZeile is int fz)
                {
                    markierer.Setzen(new[] { (fz, false) });
                    Editor.TextArea.TextView.InvalidateLayer(KnownLayer.Background);
                }
                break;
            case Ausgang.Zeitueberschreitung:
            case Ausgang.ZuVielAusgabe:
                fortschritt.EndlosschleifeGestoppt = true;
                Status("⏱ Programm gestoppt – vermutlich eine Endlosschleife.", "Gelb");
                LaufMeldungZeigen((r.FehlerText ?? "") + (r.Tipp != null ? "\nTipp: " + r.Tipp : ""), true);
                break;
            case Ausgang.Abgebrochen:
                Status("■ Programm gestoppt.", "TextGedimmt");
                break;
        }
        LogZeile($"Ergebnis: {r.Ausgang} · Laufzeit {sek} · gesamt {gesamt.TotalSeconds:0.00} s");
    }

    void ZeigeMeldungen(IReadOnlyList<CompilerMeldung> meldungen)
    {
        FehlerListe.Children.Clear();
        int fehler = meldungen.Count(m => !m.IstWarnung);
        TabFehler.Content = meldungen.Count > fehler ? $"Fehler ({fehler}) · ⚠ {meldungen.Count - fehler}" : $"Fehler ({fehler})";

        markierer.Setzen(meldungen.Select(m => (m.Zeile, m.IstWarnung)));
        Editor.TextArea.TextView.InvalidateLayer(KnownLayer.Background);

        if (meldungen.Count == 0)
        {
            FehlerListe.Children.Add(Ui.Text("✓ Keine Fehler – alles sauber!", 11.5, "Gruen", rand: new Thickness(4)));
            return;
        }
        foreach (var m in meldungen)
        {
            var raster = new Grid();
            raster.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(28) });
            raster.ColumnDefinitions.Add(new ColumnDefinition());
            raster.Children.Add(Ui.Icon(m.IstWarnung ? "\uE7BA" : "\uEA39", 16, m.IstWarnung ? "Gelb" : "Rot"));
            var sp = new StackPanel();
            sp.Children.Add(Ui.Text($"Zeile {m.Zeile}, Spalte {m.Spalte}  ·  {m.Code}", 11, m.IstWarnung ? "Gelb" : "Rot", FontWeights.SemiBold));
            sp.Children.Add(Ui.Text(m.Text, 11.5, "TextHell"));
            if (m.Tipp != null) sp.Children.Add(Ui.Text("Tipp: " + m.Tipp, 11.5, "Akzent", rand: new Thickness(0, 3, 0, 0)));
            Grid.SetColumn(sp, 1);
            raster.Children.Add(sp);
            var knopf = new Button { Style = (Style)FindResource("LektionKnopf"), Content = raster, Margin = new Thickness(0, 0, 4, 4), ToolTip = "Zur Zeile springen" };
            var meldung = m;
            knopf.Click += (_, _) => GeheZuZeile(meldung.Zeile, meldung.Spalte);
            FehlerListe.Children.Add(knopf);
        }
    }

    void LogZeile(string text)
    {
        LogBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {text}{Environment.NewLine}");
        LogBox.ScrollToEnd();
    }

    void UntenReiter_Checked(object sender, RoutedEventArgs e)
    {
        if (!geladen) return;
        SeiteAusgabe.Visibility = TabAusgabe.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        SeiteFehler.Visibility = TabFehler.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        LogBox.Visibility = TabLog.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        SeiteEingabe.Visibility = TabEingabe.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
    }

    void EingabeBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!geladen) return;
        EingabeZaehlerAktualisieren();
    }

    void EingabeZaehlerAktualisieren()
    {
        int n = VorabEingaben().Count;
        TabEingabe.Content = n > 0 ? $"Eingabe ({n})" : "Eingabe";
    }

    // ===================================================== Toast & Erfolge

    void Toast(string titel, string text, string icon, string farbe)
    {
        ToastTitel.Text = titel;
        ToastText.Text = text;
        ToastIcon.Text = icon;
        ToastIcon.Foreground = Ui.B(farbe);
        ToastRahmen.BeginAnimation(OpacityProperty, null);
        ToastRahmen.Opacity = 0;
        ToastRahmen.Visibility = Visibility.Visible;
        ToastRahmen.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200)));
        toastTimer.Stop();
        toastTimer.Start();
    }

    void ToastAusblenden()
    {
        toastTimer.Stop();
        var anim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(400));
        anim.Completed += (_, _) => ToastRahmen.Visibility = Visibility.Collapsed;
        ToastRahmen.BeginAnimation(OpacityProperty, anim);
    }

    void PruefeErfolge()
    {
        var jetzt = Erfolge.Erreicht(fortschritt, kurs);
        var neu = Erfolge.Alle.Where(e => jetzt.Contains(e.Id) && !bekannteErfolge.Contains(e.Id)).ToList();
        bekannteErfolge = jetzt;
        if (neu.Count == 1) Toast("Erfolg freigeschaltet: " + neu[0].Titel, neu[0].Beschreibung, "\uE734", "Gelb");
        else if (neu.Count > 1) Toast($"{neu.Count} neue Erfolge!", string.Join(", ", neu.Select(e => e.Titel)), "\uE734", "Gelb");
        if (neu.Count > 0 && seite == "erfolge") BaueSeite("erfolge");
    }
}
