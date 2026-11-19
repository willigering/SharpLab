using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace CSharpLernen;

public enum DialogKind { Info, Warnung, Erfolg, Gefahr, Frage, Gesperrt }
public enum DialogAntwort { Primaer, Sekundaer, Abgebrochen }

/// <summary>
/// Eigene, zum Design passende Dialoge statt der grauen Windows-MessageBox.
/// Standard: eine Ebene im Hauptfenster (abgedunkelter Hintergrund + Karte in der Mitte).
/// Ohne Hauptfenster (z. B. sehr früher Fehler) wird ein randloses, transparentes Fenster benutzt.
/// Tasten: Enter = Hauptknopf, Esc = Abbrechen/Schließen.
/// </summary>
public static class DialogService
{
    /// <summary>Das Panel, über das sich Dialoge legen (das Wurzel-Grid des Hauptfensters).</summary>
    public static Panel Host { get; set; }
    static int offen;
    public static bool IstOffen => offen > 0;

    public static Task<DialogAntwort> ShowAsync(string title, string message, DialogKind kind = DialogKind.Info,
        string primaryText = "OK", string secondaryText = null, string hint = null)
    {
        var tcs = new TaskCompletionSource<DialogAntwort>();
        void Starten()
        {
            try
            {
                if (Host != null) AlsEbene(title, message, kind, primaryText, secondaryText, hint, tcs);
                else AlsFenster(title, message, kind, primaryText, secondaryText, hint, tcs);
            }
            catch (Exception ex) { tcs.TrySetException(ex); }
        }
        var d = Application.Current?.Dispatcher;
        if (d == null || d.CheckAccess()) Starten(); else d.BeginInvoke(Starten);
        return tcs.Task;
    }

    // ------------------------------------------------------------------ Ebene im Fenster

    static void AlsEbene(string titel, string text, DialogKind art, string primaer, string sekundaer, string hinweis, TaskCompletionSource<DialogAntwort> tcs)
    {
        var vorherFokus = Keyboard.FocusedElement as IInputElement;
        var ebene = new Grid
        {
            Background = new SolidColorBrush(Color.FromArgb(0xB4, 0x05, 0x08, 0x06)),
            Opacity = 0,
            Focusable = false,
        };
        Grid.SetColumnSpan(ebene, 99); Grid.SetRowSpan(ebene, 99);
        Panel.SetZIndex(ebene, 1000 + offen);
        KeyboardNavigation.SetTabNavigation(ebene, KeyboardNavigationMode.Cycle);
        KeyboardNavigation.SetControlTabNavigation(ebene, KeyboardNavigationMode.Cycle);
        KeyboardNavigation.SetDirectionalNavigation(ebene, KeyboardNavigationMode.Cycle);

        bool fertig = false;
        Button primaerKnopf = null;
        void Schliessen(DialogAntwort r)
        {
            if (fertig) return;
            fertig = true;
            var aus = new DoubleAnimation(0, TimeSpan.FromMilliseconds(90));
            aus.Completed += (_, _) =>
            {
                Host.Children.Remove(ebene);
                offen--;
                if (vorherFokus != null && offen == 0) vorherFokus.Focus();
                tcs.TrySetResult(r);
            };
            ebene.IsHitTestVisible = false;
            ebene.BeginAnimation(UIElement.OpacityProperty, aus);
        }

        var karte = BaueKarte(titel, text, art, primaer, sekundaer, hinweis, Schliessen, out primaerKnopf);
        ebene.Children.Add(karte);

        ebene.PreviewKeyDown += (_, e) =>
        {
            if (e.Key == Key.Escape) { Schliessen(sekundaer != null ? DialogAntwort.Sekundaer : DialogAntwort.Abgebrochen); e.Handled = true; }
            else if (e.Key == Key.Enter) { Schliessen(DialogAntwort.Primaer); e.Handled = true; }
        };

        offen++;
        Host.Children.Add(ebene);
        ebene.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(120)));
        // Fokus erst setzen, wenn der Knopf im Baum ist (auch wenn das Fenster noch nicht angezeigt wird)
        primaerKnopf.Loaded += (_, _) => { primaerKnopf.Focus(); Keyboard.Focus(primaerKnopf); };
    }

    // ------------------------------------------------------------------ Eigenes Fenster (Rückfall)

    static void AlsFenster(string titel, string text, DialogKind art, string primaer, string sekundaer, string hinweis, TaskCompletionSource<DialogAntwort> tcs)
    {
        var fenster = new Window
        {
            WindowStyle = WindowStyle.None,
            AllowsTransparency = true,
            Background = Brushes.Transparent,
            SizeToContent = SizeToContent.WidthAndHeight,
            ResizeMode = ResizeMode.NoResize,
            ShowInTaskbar = false,
            Topmost = true,
            Title = titel,
            FontFamily = (FontFamily)Application.Current.Resources["UiSchrift"],
            Foreground = Ui.B("TextHell"),
        };
        var haupt = Application.Current?.MainWindow;
        if (haupt != null && haupt.IsVisible) { fenster.Owner = haupt; fenster.WindowStartupLocation = WindowStartupLocation.CenterOwner; }
        else fenster.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        DialogAntwort ergebnis = DialogAntwort.Abgebrochen;
        var karte = BaueKarte(titel, text, art, primaer, sekundaer, hinweis, r => { ergebnis = r; fenster.Close(); }, out var primaerKnopf);
        karte.Margin = new Thickness(30);
        fenster.Content = karte;
        fenster.PreviewKeyDown += (_, e) =>
        {
            if (e.Key == Key.Escape) { ergebnis = sekundaer != null ? DialogAntwort.Sekundaer : DialogAntwort.Abgebrochen; fenster.Close(); e.Handled = true; }
            else if (e.Key == Key.Enter) { ergebnis = DialogAntwort.Primaer; fenster.Close(); e.Handled = true; }
        };
        karte.MouseLeftButtonDown += (_, e) => { if (e.OriginalSource is not Button) try { fenster.DragMove(); } catch { } };
        fenster.Loaded += (_, _) => primaerKnopf.Focus();
        fenster.Closed += (_, _) => { offen--; tcs.TrySetResult(ergebnis); };
        offen++;
        fenster.Show();
    }

    // ------------------------------------------------------------------ Karte

    static (string glyph, string farbe) Symbol(DialogKind art) => art switch
    {
        DialogKind.Gesperrt => ("\uE72E", "Akzent"),
        DialogKind.Warnung => ("\uE7BA", "Gelb"),
        DialogKind.Erfolg => ("\uE73E", "Gruen"),
        DialogKind.Gefahr => ("\uE74D", "Rot"),
        DialogKind.Frage => ("\uE9CE", "Akzent"),
        _ => ("\uE946", "Akzent"),
    };

    static FrameworkElement BaueKarte(string titel, string text, DialogKind art, string primaer, string sekundaer, string hinweis,
        Action<DialogAntwort> schliessen, out Button primaerKnopf)
    {
        var (glyph, farbe) = Symbol(art);
        var akzent = (SolidColorBrush)Ui.B(farbe);
        var c = akzent.Color;

        var raster = new Grid();
        raster.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        raster.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        raster.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        raster.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var symbol = new Border
        {
            Width = 44, Height = 44, BorderThickness = new Thickness(1),
            Background = new SolidColorBrush(Color.FromArgb(0x22, c.R, c.G, c.B)),
            BorderBrush = new SolidColorBrush(Color.FromArgb(0x99, c.R, c.G, c.B)),
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(0, 2, 18, 0),
            Child = new TextBlock { Text = glyph, FontFamily = Ui.Icons, FontSize = 17, Foreground = akzent, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center },
        };
        raster.Children.Add(symbol);

        var inhalt = new StackPanel();
        Grid.SetColumn(inhalt, 1);
        inhalt.Children.Add(new TextBlock { Text = titel, FontSize = 17, FontWeight = FontWeights.Bold, Foreground = Ui.B("TextHell"), TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 8) });
        inhalt.Children.Add(new TextBlock { Text = text, FontSize = 13, Foreground = Ui.B("TextHell"), Opacity = 0.92, TextWrapping = TextWrapping.Wrap, LineHeight = 20 });
        if (!string.IsNullOrWhiteSpace(hinweis))
        {
            var h = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 12, 0, 0) };
            h.Children.Add(new TextBlock { Text = "\uEA80", FontFamily = Ui.Icons, FontSize = 11, Foreground = Ui.B("TextGedimmt"), Margin = new Thickness(0, 2, 7, 0), VerticalAlignment = VerticalAlignment.Top });
            h.Children.Add(new TextBlock { Text = hinweis, FontSize = 11, Foreground = Ui.B("TextGedimmt"), TextWrapping = TextWrapping.Wrap, MaxWidth = 380 });
            inhalt.Children.Add(h);
        }
        raster.Children.Add(inhalt);

        var leiste = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 24, 0, 0) };
        Grid.SetRow(leiste, 1); Grid.SetColumnSpan(leiste, 2);
        if (sekundaer != null)
        {
            var sek = new Button { Content = sekundaer, Style = (Style)Application.Current.Resources["GeisterKnopf"], Margin = new Thickness(0, 0, 10, 0), MinWidth = 110, IsCancel = false };
            sek.Click += (_, _) => schliessen(DialogAntwort.Sekundaer);
            leiste.Children.Add(sek);
        }
        var prim = new Button
        {
            Content = primaer,
            Style = (Style)Application.Current.Resources[art == DialogKind.Gefahr ? "GefahrKnopf" : "AkzentKnopf"],
            MinWidth = 130,
        };
        prim.Click += (_, _) => schliessen(DialogAntwort.Primaer);
        leiste.Children.Add(prim);
        raster.Children.Add(leiste);
        primaerKnopf = prim;

        var karte = new Border
        {
            Padding = new Thickness(26, 24, 26, 22),
            Background = Ui.Farbe("#121914"),
            BorderBrush = Ui.B("RandHell"),
            BorderThickness = new Thickness(1),
            Effect = new DropShadowEffect { Color = Colors.Black, BlurRadius = 24, ShadowDepth = 4, Direction = 270, Opacity = 0.5 },
            MinWidth = 400, MaxWidth = 540,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Child = raster,
        };
        // schmale Akzentlinie oben
        var deko = new Grid { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        deko.Children.Add(karte);
        deko.Children.Add(new Border { Height = 2, Background = akzent, VerticalAlignment = VerticalAlignment.Top, IsHitTestVisible = false });
        return deko;
    }
}
