using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CSharpLernen.Engine;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace CSharpLernen;

/// <summary>
/// Autovervollständigung wie IntelliSense in Visual Studio für den AvalonEdit-Editor.
/// Die Vorschläge kommen von Roslyn (Engine: <see cref="Vervollstaendigung"/>) und werden im Hintergrund
/// berechnet – jede neue Anfrage bricht die vorige ab, damit das Tippen nie hakt.
/// Außerdem: automatisches Schließen von ( [ " und Überschreiben der schließenden Zeichen.
/// </summary>
public sealed class CodeVervollstaendiger
{
    readonly TextEditor editor;
    readonly Func<bool> aktiv;
    CompletionWindow fenster;
    CancellationTokenSource abbruch;
    bool weichAusgewaehlt;     // Roslyn-„Vorschlagsmodus“ (z. B. neuer Variablenname): Enter übernimmt erst nach Pfeiltaste
    int letzterWortStart = -1;

    static readonly char[] UebernahmeZeichen = { '.', '(', ')', '[', ']', ';', ',', '<' };

    public CodeVervollstaendiger(TextEditor editor, Func<bool> aktiv)
    {
        this.editor = editor;
        this.aktiv = aktiv;
        var ta = editor.TextArea;
        ta.TextEntering += BeimTippen;
        ta.TextEntered += NachDemTippen;
        ta.PreviewKeyDown += Tasten;
        _ = Vervollstaendigung.VorwaermenAsync();
    }

    public void Schliessen()
    {
        abbruch?.Cancel();
        fenster?.Close();
    }

    static bool IstBezeichner(char c) => char.IsLetterOrDigit(c) || c == '_';

    // ============================================================ Tastatur

    void Tasten(object sender, KeyEventArgs e)
    {
        if (!aktiv()) return;
        var doc = editor.Document;
        int caret = editor.CaretOffset;

        if (e.Key == Key.Space && Keyboard.Modifiers == ModifierKeys.Control)
        {
            e.Handled = true;
            _ = AnfragenAsync(null, explizit: true);
            return;
        }
        if (fenster != null)
        {
            if (e.Key is Key.Up or Key.Down or Key.PageUp or Key.PageDown) weichAusgewaehlt = false;
            if ((e.Key == Key.Enter || e.Key == Key.Tab) && (weichAusgewaehlt || fenster.CompletionList.SelectedItem == null))
            {
                // nichts Passendes ausgewählt: Enter/Tab wirkt ganz normal
                fenster.Close();
                return;
            }
            return;   // Pfeile, Enter, Tab, Esc erledigt das Vervollständigungsfenster
        }
        // Backspace zwischen einem leeren Paar ( ) [ ] " " löscht beide Zeichen
        if (e.Key == Key.Back && editor.SelectionLength == 0 && caret > 0 && caret < doc.TextLength)
        {
            char vor = doc.GetCharAt(caret - 1), nach = doc.GetCharAt(caret);
            if ((vor == '(' && nach == ')') || (vor == '[' && nach == ']') || (vor == '"' && nach == '"'))
            {
                doc.Remove(caret - 1, 2);
                e.Handled = true;
            }
        }
    }

    void BeimTippen(object sender, TextCompositionEventArgs e)
    {
        if (!aktiv() || string.IsNullOrEmpty(e.Text)) return;
        char c = e.Text[0];
        var doc = editor.Document;
        int caret = editor.CaretOffset;

        if (fenster != null && !IstBezeichner(c))
        {
            var auswahl = fenster.CompletionList.SelectedItem as Eintrag;
            string getippt = doc.GetText(fenster.StartOffset, Math.Max(0, caret - fenster.StartOffset));
            bool passt = auswahl != null && !weichAusgewaehlt && getippt.Length > 0
                         && auswahl.Text.StartsWith(getippt, StringComparison.OrdinalIgnoreCase);
            if (passt && Array.IndexOf(UebernahmeZeichen, c) >= 0) fenster.CompletionList.RequestInsertion(e);
            else fenster.Close();
            caret = editor.CaretOffset;
        }

        // schließendes Zeichen überschreiben, wenn es schon dasteht
        if ((c == ')' || c == ']' || c == '"') && editor.SelectionLength == 0 && caret < doc.TextLength && doc.GetCharAt(caret) == c)
        {
            editor.CaretOffset = caret + 1;
            e.Handled = true;
        }
    }

    void NachDemTippen(object sender, TextCompositionEventArgs e)
    {
        if (!aktiv() || string.IsNullOrEmpty(e.Text)) return;
        char c = e.Text[0];
        var doc = editor.Document;
        int caret = editor.CaretOffset;

        // Klammern/Anführungszeichen automatisch schließen
        char? zu = c switch { '(' => ')', '[' => ']', '"' => '"', _ => null };
        if (zu != null)
        {
            char nach = caret < doc.TextLength ? doc.GetCharAt(caret) : '\n';
            bool frei = char.IsWhiteSpace(nach) || nach is ')' or ']' or ';' or ',';
            if (c == '"')
            {
                var zeile = doc.GetLineByOffset(caret);
                string davor = doc.GetText(zeile.Offset, caret - zeile.Offset);
                int anzahl = davor.Count(ch => ch == '"');
                bool escaped = davor.Length >= 2 && davor[^2] == '\\';
                frei = frei && anzahl % 2 == 1 && !escaped && !(davor.Length >= 2 && IstBezeichner(davor[^2]));
            }
            if (frei)
            {
                doc.Insert(caret, zu.Value.ToString());
                editor.CaretOffset = caret;
            }
        }

        if (fenster != null) return;   // offenes Fenster filtert selbst
        if (c == '.') { _ = AnfragenAsync(c, false); return; }
        if (IstBezeichner(c) && !char.IsDigit(c))
        {
            int start = caret;
            while (start > 0 && IstBezeichner(doc.GetCharAt(start - 1))) start--;
            int laenge = caret - start;
            if (laenge == 1) { letzterWortStart = start; _ = AnfragenAsync(c, false); }
            else if (laenge == 2 && letzterWortStart != start) { letzterWortStart = start; _ = AnfragenAsync(null, false); }
        }
    }

    // ============================================================ Anfrage

    async Task AnfragenAsync(char? zeichen, bool explizit)
    {
        abbruch?.Cancel();
        var cts = abbruch = new CancellationTokenSource();
        string code = editor.Document.Text;
        int pos = editor.CaretOffset;
        VervollstaendigungsListe liste;
        try
        {
            liste = await Task.Run(() => Vervollstaendigung.Standard.VorschlaegeAsync(code, pos, zeichen, cts.Token), cts.Token);
        }
        catch (OperationCanceledException) { return; }
        catch { return; }
        if (cts.IsCancellationRequested || liste == null || fenster != null || !aktiv()) return;

        // Passt das Ergebnis noch zum aktuellen Text? (Der Nutzer kann inzwischen weitergetippt haben.)
        var doc = editor.Document;
        int caret = editor.CaretOffset;
        if (doc.TextLength - caret != code.Length - pos || liste.Start > caret || liste.Start > doc.TextLength) return;
        for (int i = liste.Start; i < caret; i++) if (!IstBezeichner(doc.GetCharAt(i))) return;
        if (!explizit && zeichen == null && caret - liste.Start == 0) return;

        Oeffnen(liste, caret);
    }

    void Oeffnen(VervollstaendigungsListe liste, int caret)
    {
        var w = new CompletionWindow(editor.TextArea)
        {
            StartOffset = liste.Start,
            EndOffset = caret,
            CloseWhenCaretAtBeginning = true,
            CloseAutomatically = true,
            Width = 390,
            MaxHeight = 300,
            Background = (Brush)Application.Current.FindResource("Hintergrund"),
            BorderBrush = (Brush)Application.Current.FindResource("Rand"),
            BorderThickness = new Thickness(1),
            FontFamily = (FontFamily)Application.Current.FindResource("CodeSchrift"),
            FontSize = 12,
            Foreground = (Brush)Application.Current.FindResource("TextHell"),
            ResizeMode = ResizeMode.NoResize,
        };
        w.CompletionList.ApplyTemplate();
        var lb = w.CompletionList.ListBox;
        if (lb != null)
        {
            lb.Background = Brushes.Transparent;
            lb.BorderThickness = new Thickness(0);
            lb.Foreground = w.Foreground;
            lb.ItemContainerStyle = (Style)Application.Current.FindResource("VervollstaendigungEintrag");
            lb.Padding = new Thickness(0);
        }

        var daten = w.CompletionList.CompletionData;
        Eintrag vorausgewaehlt = null;
        foreach (var v in liste.Vorschlaege)
        {
            var e = new Eintrag(v);
            daten.Add(e);
            if (vorausgewaehlt == null && v.Vorausgewaehlt) vorausgewaehlt = e;
        }
        weichAusgewaehlt = liste.Vorschlagsmodus;

        fenster = w;
        w.Closed += (_, _) => { if (fenster == w) fenster = null; };
        string getippt = editor.Document.GetText(liste.Start, caret - liste.Start);
        w.Show();
        w.CompletionList.SelectItem(getippt);
        if (getippt.Length == 0 && vorausgewaehlt != null) w.CompletionList.SelectedItem = vorausgewaehlt;
    }

    // ============================================================ Eintrag

    sealed class Eintrag : ICompletionData
    {
        static readonly Dictionary<VorschlagArt, (string Zeichen, Brush Farbe)> Symbole = BaueSymbole();
        readonly Vorschlag v;
        object inhalt, beschreibung;

        public Eintrag(Vorschlag v) { this.v = v; }

        public ImageSource Image => null;
        public string Text => v.FilterText;
        public double Priority => v.Vorausgewaehlt ? 1 : 0;

        public object Content => inhalt ??= BaueInhalt();

        public object Description
        {
            get
            {
                if (beschreibung != null) return beschreibung;
                var tb = new TextBlock { Text = v.ArtText, TextWrapping = TextWrapping.Wrap, MaxWidth = 460, FontSize = 11.5 };
                beschreibung = tb;
                _ = LadeBeschreibungAsync(tb);
                return tb;
            }
        }

        async Task LadeBeschreibungAsync(TextBlock tb)
        {
            try
            {
                string text = await Task.Run(() => v.BeschreibungAsync());
                if (!string.IsNullOrWhiteSpace(text)) tb.Text = text.Trim();
            }
            catch { }
        }

        public void Complete(TextArea textArea, ISegment segment, EventArgs e)
        {
            textArea.Document.Replace(segment, v.Einfuegen);
        }

        object BaueInhalt()
        {
            var (zeichen, farbe) = Symbole.TryGetValue(v.Art, out var s) ? s : ("·", Gedimmt);
            var g = new Grid();
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(22) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            var symbol = new Border
            {
                Width = 15, Height = 15, BorderBrush = farbe, BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(0),
                HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center,
                Child = new TextBlock { Text = zeichen, Foreground = farbe, FontSize = 9.5, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center },
            };
            g.Children.Add(symbol);
            var name = new TextBlock { Text = v.Anzeige, VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.None };
            Grid.SetColumn(name, 1);
            g.Children.Add(name);
            var art = new TextBlock { Text = v.ArtText, Foreground = Gedimmt, FontSize = 10.5, Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(art, 2);
            g.Children.Add(art);
            return g;
        }

        static readonly Brush Gedimmt = Frieren("#64756A");
        static Brush Frieren(string farbe) { var b = new SolidColorBrush((Color)ColorConverter.ConvertFromString(farbe)); b.Freeze(); return b; }

        static Dictionary<VorschlagArt, (string, Brush)> BaueSymbole()
        {
            var gruen = Frieren("#3FCB6E"); var gelb = Frieren("#DDB25E"); var blau = Frieren("#6FB7D9"); var lila = Frieren("#B48EE0"); var grau = Frieren("#93A597");
            return new()
            {
                [VorschlagArt.Methode] = ("M", lila),
                [VorschlagArt.Erweiterungsmethode] = ("M", lila),
                [VorschlagArt.Eigenschaft] = ("E", gelb),
                [VorschlagArt.Feld] = ("F", blau),
                [VorschlagArt.Ereignis] = ("Ev", gelb),
                [VorschlagArt.Konstante] = ("C", blau),
                [VorschlagArt.Klasse] = ("K", gruen),
                [VorschlagArt.Struktur] = ("S", gruen),
                [VorschlagArt.Schnittstelle] = ("I", gruen),
                [VorschlagArt.Aufzaehlung] = ("En", gruen),
                [VorschlagArt.Aufzaehlungswert] = ("W", blau),
                [VorschlagArt.Delegat] = ("D", gruen),
                [VorschlagArt.Namensraum] = ("N", grau),
                [VorschlagArt.Variable] = ("V", blau),
                [VorschlagArt.Parameter] = ("P", blau),
                [VorschlagArt.Typparameter] = ("T", gruen),
                [VorschlagArt.Schluesselwort] = ("#", grau),
            };
        }
    }
}
