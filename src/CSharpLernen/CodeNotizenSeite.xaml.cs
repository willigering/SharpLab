using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CSharpLernen.Engine;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Indentation.CSharp;

namespace CSharpLernen;

public partial class CodeNotizenSeite : UserControl
{
    readonly CodeNotizen ablage = new(Path.Combine(Fortschritt.Ordner, "CodeNotizen"));
    readonly Func<string> editorCode;
    readonly Action<string> inEditorLaden;
    readonly CodeVervollstaendiger vervollstaendiger;
    List<CodeNotiz> notizen = new();
    CodeNotiz aktuell;
    bool laden;

    public CodeNotizenSeite(Func<string> editorCode, Action<string> inEditorLaden,
        IHighlightingDefinition highlighting, Func<bool> autovervollstaendigungAktiv)
    {
        this.editorCode = editorCode;
        this.inEditorLaden = inEditorLaden;
        InitializeComponent();

        Code.SyntaxHighlighting = highlighting;
        Code.TextArea.SelectionBrush = Ui.B("EditorAuswahl");
        Code.TextArea.Caret.CaretBrush = Ui.B("Akzent");
        Code.Options.ConvertTabsToSpaces = true;
        Code.Options.EnableHyperlinks = false;
        Code.Options.EnableEmailHyperlinks = false;
        Code.TextArea.IndentationStrategy = new CSharpIndentationStrategy(Code.Options);
        vervollstaendiger = new CodeVervollstaendiger(Code,
            () => Code.IsVisible && Code.IsKeyboardFocusWithin && autovervollstaendigungAktiv());
        Code.IsVisibleChanged += (_, _) =>
        {
            if (!Code.IsVisible) vervollstaendiger.Schliessen();
        };

        int fehler = 0;
        try
        {
            notizen = ablage.Laden((datei, ex) =>
            {
                fehler++;
                Fortschritt.Protokoll(ex, "Code-Notiz lesen: " + datei);
            });
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            fehler++;
            Fortschritt.Protokoll(ex, "Code-Notizen öffnen");
        }

        FormularLaden(null);
        if (fehler > 0) Meldung.Text = "Einige Code-Notizen konnten nicht gelesen werden. Details stehen im Fehlerprotokoll.";
    }

    bool Ungespeichert => Titel.Text != (aktuell?.Titel ?? "") ||
                          Kommentar.Text != (aktuell?.Kommentar ?? "") ||
                          Code.Text != (aktuell?.Code ?? "");

    void FormularLaden(CodeNotiz notiz, string startcode = "")
    {
        laden = true;
        vervollstaendiger.Schliessen();
        aktuell = notiz;
        Titel.Text = notiz?.Titel ?? "";
        Kommentar.Text = notiz?.Kommentar ?? "";
        Code.Text = notiz?.Code ?? startcode;
        Code.Document.UndoStack.ClearAll();
        Code.ScrollToHome();
        LoeschenKnopf.IsEnabled = notiz != null;
        laden = false;
        Meldung.Text = Ungespeichert ? "Ungespeicherte Änderungen" : "";
        TrefferAktualisieren();
    }

    void TrefferAktualisieren()
    {
        var sichtbar = notizen.Where(n => n.PasstZu(Suche.Text)).OrderByDescending(n => n.Geaendert).ToList();
        laden = true;
        Treffer.ItemsSource = sichtbar;
        Treffer.SelectedItem = sichtbar.FirstOrDefault(n => n.Id == aktuell?.Id);
        laden = false;
        Trefferstatus.Text = sichtbar.Count == 0
            ? (string.IsNullOrWhiteSpace(Suche.Text) ? "Noch keine gespeicherten Notizen" : "Keine Treffer")
            : $"{sichtbar.Count} {(sichtbar.Count == 1 ? "Notiz" : "Notizen")}";
    }

    void Suche_Geaendert(object sender, TextChangedEventArgs e)
    {
        if (Treffer != null && Trefferstatus != null) TrefferAktualisieren();
    }

    void Feld_Geaendert(object sender, EventArgs e)
    {
        if (!laden && Meldung != null && Code != null) Meldung.Text = Ungespeichert ? "Ungespeicherte Änderungen" : "";
    }

    void Treffer_Gewaehlt(object sender, SelectionChangedEventArgs e)
    {
        if (laden || Treffer.SelectedItem is not CodeNotiz auswahl || auswahl.Id == aktuell?.Id) return;
        if (WechselErlaubt()) FormularLaden(auswahl);
        else TrefferAktualisieren();
    }

    bool WechselErlaubt()
    {
        if (!Ungespeichert) return true;
        var antwort = MessageBox.Show(Window.GetWindow(this), "Änderungen an dieser Code-Notiz speichern?",
            "Ungespeicherte Änderungen", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        return antwort == MessageBoxResult.No || (antwort == MessageBoxResult.Yes && Speichern());
    }

    bool Speichern()
    {
        if (string.IsNullOrWhiteSpace(Titel.Text))
        {
            Meldung.Text = "Gib der Code-Notiz einen Titel.";
            Titel.Focus();
            return false;
        }

        var notiz = new CodeNotiz(aktuell?.Id ?? Guid.NewGuid(), Titel.Text.Trim(), Kommentar.Text, Code.Text, DateTimeOffset.Now);
        try { ablage.Speichern(notiz); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Fortschritt.Protokoll(ex, "Code-Notiz speichern");
            Meldung.Text = "Speichern fehlgeschlagen. Deine Änderungen stehen weiterhin im Formular.";
            return false;
        }

        notizen.RemoveAll(n => n.Id == notiz.Id);
        notizen.Add(notiz);
        aktuell = notiz;
        Titel.Text = notiz.Titel;
        TrefferAktualisieren();
        Meldung.Text = "Code-Notiz gespeichert.";
        return true;
    }

    void Neu_Click(object sender, RoutedEventArgs e) => NeueNotiz("");
    void Uebernehmen_Click(object sender, RoutedEventArgs e) => EditorUebernehmen();
    public void EditorUebernehmen() => NeueNotiz(editorCode());
    void NeueNotiz(string code)
    {
        if (!WechselErlaubt()) return;
        FormularLaden(null, code);
        Titel.Focus();
    }

    void Speichern_Click(object sender, RoutedEventArgs e) => Speichern();

    void Loeschen_Click(object sender, RoutedEventArgs e)
    {
        if (aktuell == null || MessageBox.Show(Window.GetWindow(this), $"„{aktuell.Titel}“ löschen?", "Code-Notiz löschen",
                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) != MessageBoxResult.Yes) return;

        try { ablage.Loeschen(aktuell.Id); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Fortschritt.Protokoll(ex, "Code-Notiz löschen");
            Meldung.Text = "Die Code-Notiz konnte nicht gelöscht werden.";
            return;
        }
        notizen.RemoveAll(n => n.Id == aktuell.Id);
        FormularLaden(null);
        Meldung.Text = "Code-Notiz gelöscht.";
    }

    void InEditor_Click(object sender, RoutedEventArgs e) => inEditorLaden(Code.Text);

    public bool EntwurfAbschliessen()
    {
        if (!WechselErlaubt()) return false;
        if (Ungespeichert) FormularLaden(aktuell);
        return true;
    }

    void Seite_Taste(object sender, KeyEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S) Speichern();
        else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.F) Suche.Focus();
        else return;
        e.Handled = true;
    }
}
