using System.Diagnostics;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using CSharpLernen.Engine;

namespace CSharpLernen;

/// <summary>Feedback an Gering Digital: eigene Ebene im Matrix-Design, Senden per HTTPS (JSON), Ausweichweg per E-Mail-Programm.
/// Der Betreff „SharpLab: “ wird unsichtbar vom Server davorgesetzt. Logik: <see cref="Feedback"/>.</summary>
public partial class MainWindow
{
    // Entwurf bleibt erhalten, wenn man abbricht oder das Senden scheitert – erst nach Erfolg wird er geleert.
    string entwurfBetreff = "", entwurfNachricht = "", entwurfEmail = "";
    bool feedbackOffen;

    void Feedback_Click(object sender, RoutedEventArgs e) => _ = FeedbackOeffnen();

    static TextBlock Beschriftung(string text, string zusatz = null)
    {
        var t = new TextBlock { FontSize = 12, FontWeight = FontWeights.SemiBold, Foreground = Ui.B("TextHell"), Margin = new Thickness(0, 12, 0, 5), TextWrapping = TextWrapping.Wrap };
        t.Inlines.Add(text);
        if (zusatz != null) t.Inlines.Add(new System.Windows.Documents.Run("  " + zusatz) { FontWeight = FontWeights.Normal, Foreground = Ui.B("TextGedimmt") });
        return t;
    }

    async Task FeedbackOeffnen()
    {
        if (feedbackOffen) return;
        feedbackOffen = true;
        var fertig = new TaskCompletionSource<bool>();
        var vorherFokus = Keyboard.FocusedElement as IInputElement;
        string version = Aktualisierung.Anzeige(EigeneVersion);

        var ebene = new Grid { Background = new SolidColorBrush(Color.FromArgb(0xB4, 0x05, 0x08, 0x06)), Opacity = 0 };
        Grid.SetColumnSpan(ebene, 99); Grid.SetRowSpan(ebene, 99);
        Panel.SetZIndex(ebene, 1400);
        KeyboardNavigation.SetTabNavigation(ebene, KeyboardNavigationMode.Cycle);

        var akzent = (SolidColorBrush)Ui.B("Akzent");
        var c = akzent.Color;
        var inhalt = new StackPanel();

        var kopf = new Grid();
        kopf.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        kopf.ColumnDefinitions.Add(new ColumnDefinition());
        kopf.Children.Add(new Border
        {
            Width = 44, Height = 44, BorderThickness = new Thickness(1), Margin = new Thickness(0, 2, 18, 0), VerticalAlignment = VerticalAlignment.Top,
            Background = new SolidColorBrush(Color.FromArgb(0x22, c.R, c.G, c.B)), BorderBrush = new SolidColorBrush(Color.FromArgb(0x99, c.R, c.G, c.B)),
            Child = new TextBlock { Text = "\uED15", FontFamily = Ui.Icons, FontSize = 17, Foreground = akzent, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center },
        });
        var titel = new StackPanel();
        Grid.SetColumn(titel, 1);
        titel.Children.Add(new TextBlock { Text = "Feedback senden", FontSize = 17, FontWeight = FontWeights.Bold, Foreground = Ui.B("TextHell"), Margin = new Thickness(0, 0, 0, 4) });
        titel.Children.Add(new TextBlock { Text = "Lob, Fehler, Wünsche – alles hilft, SharpLab besser zu machen.", FontSize = 12.5, Foreground = Ui.B("TextMittel"), TextWrapping = TextWrapping.Wrap });
        kopf.Children.Add(titel);
        inhalt.Children.Add(kopf);

        inhalt.Children.Add(Beschriftung("Betreff", "(optional)"));
        var betreff = new TextBox { Text = entwurfBetreff, MaxLength = Feedback.MaxBetreff, FontSize = 13, Padding = new Thickness(8, 6, 8, 6) };
        inhalt.Children.Add(betreff);

        var nachrichtKopf = new Grid();
        nachrichtKopf.ColumnDefinitions.Add(new ColumnDefinition());
        nachrichtKopf.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        nachrichtKopf.Children.Add(Beschriftung("Nachricht"));
        var zaehler = new TextBlock { FontSize = 11, Foreground = Ui.B("TextGedimmt"), VerticalAlignment = VerticalAlignment.Bottom, Margin = new Thickness(0, 0, 0, 5) };
        Grid.SetColumn(zaehler, 1);
        nachrichtKopf.Children.Add(zaehler);
        inhalt.Children.Add(nachrichtKopf);
        var nachricht = new TextBox
        {
            Text = entwurfNachricht, MaxLength = Feedback.MaxNachricht, AcceptsReturn = true, AcceptsTab = false, TextWrapping = TextWrapping.Wrap,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Height = 150, FontSize = 13, Padding = new Thickness(8, 6, 8, 6), VerticalContentAlignment = VerticalAlignment.Top,
        };
        void Zaehlen() => zaehler.Text = $"{nachricht.Text.Length} / {Feedback.MaxNachricht}";
        nachricht.TextChanged += (_, _) => Zaehlen();
        Zaehlen();
        inhalt.Children.Add(nachricht);

        inhalt.Children.Add(Beschriftung("Deine E-Mail für eine Antwort", "(optional)"));
        var email = new TextBox { Text = entwurfEmail, MaxLength = Feedback.MaxEmail, FontSize = 13, Padding = new Thickness(8, 6, 8, 6) };
        var platzhalter = new TextBlock { Text = "z. B. max@beispiel.de", FontSize = 13, Foreground = Ui.B("TextGedimmt"), Margin = new Thickness(11, 7, 0, 0), IsHitTestVisible = false };
        void Platzhalter() => platzhalter.Visibility = email.Text.Length == 0 ? Visibility.Visible : Visibility.Collapsed;
        email.TextChanged += (_, _) => Platzhalter();
        Platzhalter();
        var emailFeld = new Grid();
        emailFeld.Children.Add(email);
        emailFeld.Children.Add(platzhalter);
        inhalt.Children.Add(emailFeld);

        string protokoll = Feedback.ProtokollEnde(App.FehlerDatei);
        var mitProtokoll = new CheckBox
        {
            Content = protokoll == null ? "Fehlerprotokoll anhängen (es gibt noch keins)" : "Fehlerprotokoll anhängen (letzte Einträge aus fehler.log, hilft bei Fehlern)",
            IsChecked = false, IsEnabled = protokoll != null, FontSize = 12, Foreground = Ui.B("TextHell"), Margin = new Thickness(0, 12, 0, 0),
        };
        inhalt.Children.Add(mitProtokoll);

        var hinweis = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 12, 0, 0) };
        hinweis.Children.Add(new TextBlock { Text = "\uEA80", FontFamily = Ui.Icons, FontSize = 11, Foreground = Ui.B("TextGedimmt"), Margin = new Thickness(0, 2, 7, 0), VerticalAlignment = VerticalAlignment.Top });
        hinweis.Children.Add(new TextBlock
        {
            Text = $"Deine Nachricht geht an Gering Digital ({Feedback.Empfaenger}). Zusätzlich zu deiner Nachricht werden die SharpLab-Version ({version}) und deine Windows-Version übertragen. Deine E-Mail ist freiwillig. Ein Fehlerprotokoll wird nur mit deiner Auswahl angehängt und kann persönliche Daten oder Codeausschnitte enthalten. Prüfe es vor dem Senden. Dein gespeicherter Lernstand wird nicht automatisch übertragen. Datenschutz: geringdigital.de/datenschutz.php#sharplab.",
            FontSize = 11, Foreground = Ui.B("TextGedimmt"), TextWrapping = TextWrapping.Wrap, MaxWidth = 510,
        });
        inhalt.Children.Add(hinweis);

        // Meldungen: Prüffehler (rot) bzw. Sendefehler mit Ausweichweg
        var meldung = new TextBlock { FontSize = 12, Foreground = Ui.B("Rot"), TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 12, 0, 0), Visibility = Visibility.Collapsed };
        inhalt.Children.Add(meldung);
        var mailKnopf = new Button { Content = "Stattdessen per E-Mail-Programm senden", Style = (Style)Application.Current.Resources["GeisterKnopf"], HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(0, 10, 0, 0), Visibility = Visibility.Collapsed };
        inhalt.Children.Add(mailKnopf);

        var leiste = new WrapPanel { HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 22, 0, 0) };
        var abbrechen = new Button { Content = "Abbrechen", Style = (Style)Application.Current.Resources["GeisterKnopf"], MinWidth = 110, Margin = new Thickness(0, 0, 10, 0) };
        var senden = new Button { Content = "Senden", Style = (Style)Application.Current.Resources["AkzentKnopf"], MinWidth = 130 };
        leiste.Children.Add(abbrechen); leiste.Children.Add(senden);
        inhalt.Children.Add(leiste);

        var karte = new Border
        {
            Background = Ui.B("DialogFlaeche"), BorderBrush = Ui.B("RandHell"), BorderThickness = new Thickness(1),
            Effect = new DropShadowEffect { Color = Colors.Black, BlurRadius = 24, ShadowDepth = 4, Direction = 270, Opacity = 0.5 },
            Width = 600,
            // Bei kleinen Fenstern scrollt der Inhalt, statt abgeschnitten zu werden.
            Child = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Padding = new Thickness(26, 24, 26, 22), Content = inhalt },
        };
        var deko = new Grid { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(20) };
        deko.Children.Add(karte);
        deko.Children.Add(new Border { Height = 2, Background = akzent, VerticalAlignment = VerticalAlignment.Top, IsHitTestVisible = false });
        ebene.Children.Add(deko);
        void Hoehe() => deko.MaxHeight = Math.Max(300, Wurzel.ActualHeight - 40);
        Hoehe();
        SizeChangedEventHandler groesse = (_, _) => Hoehe();
        Wurzel.SizeChanged += groesse;

        void EntwurfMerken() { entwurfBetreff = betreff.Text; entwurfNachricht = nachricht.Text; entwurfEmail = email.Text; }
        FeedbackDaten Daten() => Feedback.Erstellen(betreff.Text, nachricht.Text, email.Text, version, mitProtokoll.IsChecked == true ? protokoll : null);

        bool zu = false;
        CancellationTokenSource sendenCts = null;
        void Schliessen()
        {
            if (zu) return;
            zu = true;
            sendenCts?.Cancel();
            EntwurfMerken();
            Wurzel.SizeChanged -= groesse;
            var aus = new DoubleAnimation(0, TimeSpan.FromMilliseconds(90));
            aus.Completed += (_, _) =>
            {
                Wurzel.Children.Remove(ebene);
                DialogService.EbeneGeschlossen();
                feedbackOffen = false;
                vorherFokus?.Focus();
                fertig.TrySetResult(true);
            };
            ebene.IsHitTestVisible = false;
            ebene.BeginAnimation(OpacityProperty, aus);
        }

        void Melden(string text, bool mitMail)
        {
            meldung.Text = text;
            meldung.Visibility = Visibility.Visible;
            mailKnopf.Visibility = mitMail ? Visibility.Visible : Visibility.Collapsed;
            meldung.BringIntoView();
        }

        abbrechen.Click += (_, _) => Schliessen();
        ebene.PreviewKeyDown += (_, e) =>
        {
            if (e.Key == Key.Escape) { Schliessen(); e.Handled = true; }
            else if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control) { senden.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent)); e.Handled = true; }
        };
        mailKnopf.Click += (_, _) =>
        {
            try
            {
                Process.Start(new ProcessStartInfo(Feedback.MailtoLink(Daten())) { UseShellExecute = true });
                Toast("E-Mail-Programm geöffnet", "Prüfe die Nachricht dort und schick sie ab.", "\uE715", "Akzent");
                Schliessen();
            }
            catch (Exception ex)
            {
                Fortschritt.Protokoll(ex, "Feedback per mailto");
                Melden($"Es ist kein E-Mail-Programm eingerichtet. Schreib einfach an {Feedback.Empfaenger} – dein Text ist noch hier und lässt sich kopieren.", false);
            }
        };
        senden.Click += async (_, _) =>
        {
            if (!senden.IsEnabled) return;
            var fehler = Feedback.Pruefen(betreff.Text, nachricht.Text, email.Text);
            if (fehler.Count > 0) { Melden(string.Join("\n", fehler), false); (string.IsNullOrWhiteSpace(nachricht.Text) ? nachricht : email).Focus(); return; }
            meldung.Visibility = mailKnopf.Visibility = Visibility.Collapsed;
            senden.IsEnabled = false;
            senden.Content = "Wird gesendet …";
            sendenCts = new CancellationTokenSource();
            FeedbackErgebnis r;
            try
            {
                var daten = Daten();
                using var client = new HttpClient { Timeout = Timeout.InfiniteTimeSpan };
                r = await Task.Run(() => Feedback.SendenAsync(client, daten, ct: sendenCts.Token));
            }
            catch (OperationCanceledException) { return; }
            catch (Exception ex) { r = new FeedbackErgebnis(false, ex.Message); }
            if (zu) return;
            senden.IsEnabled = true;
            senden.Content = "Senden";
            if (r.Erfolg)
            {
                bool mitAntwort = !string.IsNullOrWhiteSpace(email.Text);
                betreff.Text = nachricht.Text = email.Text = "";
                Schliessen();
                entwurfBetreff = entwurfNachricht = entwurfEmail = "";
                _ = DialogService.ShowAsync("Danke für dein Feedback!", "Deine Nachricht ist bei Gering Digital angekommen." + (!mitAntwort ? "" : " Wenn nötig, kommt die Antwort an deine E-Mail-Adresse."), DialogKind.Erfolg, "Gern geschehen");
            }
            else
            {
                Fortschritt.Protokoll(new HttpRequestException("Feedback senden fehlgeschlagen: " + r.Fehler), "Feedback senden");
                Melden("Das Senden hat gerade nicht geklappt – bist du online? Dein Text bleibt erhalten, du kannst es gleich nochmal versuchen.\n(" + r.Fehler + ")", true);
            }
        };

        DialogService.EbeneGeoeffnet();
        Wurzel.Children.Add(ebene);
        ebene.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(120)));
        (string.IsNullOrEmpty(entwurfNachricht) ? betreff : nachricht).Loaded += (s, _) => { var t = (TextBox)s; t.Focus(); Keyboard.Focus(t); t.CaretIndex = t.Text.Length; };
        await fertig.Task;
    }
}
