using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using CSharpLernen.Engine;

namespace CSharpLernen;

/// <summary>
/// Auto-Update über GitHub-Releases: beim Start still prüfen (nicht blockierend, 5 s Timeout, Fehler nur ins fehler.log),
/// bei neuer Version eine Ebene im Matrix-Design mit Release-Notizen anbieten, auf Wunsch herunterladen, prüfen,
/// die laufende exe austauschen und neu starten. Logik ohne Oberfläche: <see cref="Aktualisierung"/>.
/// </summary>
public partial class MainWindow
{
    static readonly TimeSpan PruefTimeout = TimeSpan.FromSeconds(5);
    bool updateLaeuft;

    static Version EigeneVersion => typeof(MainWindow).Assembly.GetName().Version;

    /// <summary>Beim Start: Aufräumen nach einem Update, Hinweis „Aktualisiert auf …“, danach stille Prüfung.</summary>
    void UpdateBeimStartEinrichten()
    {
        string exe = Environment.ProcessPath;
        string ordner = exe == null ? null : Path.GetDirectoryName(exe);
        string nachUpdate = Aktualisierung.NachUpdateVersion(Environment.GetCommandLineArgs());
        if (ordner != null)
            _ = Task.Run(async () =>
            {
                // Der alte Prozess beendet sich gerade erst – .old ggf. ein paar Mal versuchen.
                for (int i = 0; i < 20 && Aktualisierung.AlteDateienLoeschen(ordner) > 0; i++) await Task.Delay(500);
            });
        UninstallVersionNachziehen(ordner);
        if (nachUpdate != null)
        {
            string v = string.IsNullOrEmpty(nachUpdate) ? Aktualisierung.Anzeige(EigeneVersion) : nachUpdate;
            Toast("Aktualisiert auf " + v, "SharpLab wurde erfolgreich aktualisiert. Dein Lernstand ist unverändert.", "\uE895", "Gruen");
            return;   // direkt nach einem Update nicht gleich wieder fragen
        }
        if (fortschritt.UpdateBeimStart)
            _ = Dispatcher.InvokeAsync(async () => { await Task.Delay(1500); await NachUpdatesSuchenAsync(manuell: false); });
    }

    /// <summary>Fragt GitHub nach dem neuesten Release. Still (manuell = false): Fehler nur ins Protokoll.</summary>
    async Task NachUpdatesSuchenAsync(bool manuell)
    {
        if (updateLaeuft) return;
        updateLaeuft = true;
        try
        {
            ReleaseInfo r;
            try
            {
                using var client = Aktualisierung.NeuerClient(Aktualisierung.Anzeige(EigeneVersion));
                r = await Task.Run(() => Aktualisierung.NeuestesReleaseAsync(client, PruefTimeout));
            }
            catch (Exception ex)
            {
                Fortschritt.Protokoll(ex, "Update-Prüfung");
                if (manuell)
                    await ReleaseSeiteAnbieten("Keine Verbindung", "Ob es eine neue Version gibt, konnte gerade nicht geprüft werden. Bist du online?", DialogKind.Warnung);
                return;
            }

            if (!Aktualisierung.SollAnbieten(r, EigeneVersion, fortschritt.UpdateUebersprungen, manuell))
            {
                if (manuell)
                    await DialogService.ShowAsync("Alles aktuell", $"Du benutzt SharpLab {Aktualisierung.Anzeige(EigeneVersion)}, das ist die neueste Version.", DialogKind.Erfolg, "OK");
                return;
            }
            await UpdateAnbieten(r);
        }
        finally { updateLaeuft = false; }
    }

    async Task ReleaseSeiteAnbieten(string titel, string text, DialogKind art, string seite = null)
    {
        seite ??= Aktualisierung.ReleaseSeite;
        if (await DialogService.ShowAsync(titel, text, art, "Release-Seite öffnen", cancelText: "Schließen", hint: seite) == DialogAntwort.Primaer)
            try { Process.Start(new ProcessStartInfo(seite) { UseShellExecute = true }); } catch (Exception ex) { Fortschritt.Protokoll(ex, "Release-Seite öffnen"); }
    }

    // ------------------------------------------------------------------ Ebene „Neue Version“

    async Task UpdateAnbieten(ReleaseInfo r)
    {
        string neu = Aktualisierung.Anzeige(r.Version);
        var fertig = new TaskCompletionSource<bool>();
        var vorherFokus = Keyboard.FocusedElement as IInputElement;

        var ebene = new Grid { Background = new SolidColorBrush(Color.FromArgb(0xB4, 0x05, 0x08, 0x06)), Opacity = 0 };
        Grid.SetColumnSpan(ebene, 99); Grid.SetRowSpan(ebene, 99);
        Panel.SetZIndex(ebene, 1500);
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
            Child = new TextBlock { Text = "\uE896", FontFamily = Ui.Icons, FontSize = 17, Foreground = akzent, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center },
        });
        var titel = new StackPanel();
        Grid.SetColumn(titel, 1);
        titel.Children.Add(new TextBlock { Text = $"SharpLab {neu} ist verfügbar", FontSize = 17, FontWeight = FontWeights.Bold, Foreground = Ui.B("TextHell"), Margin = new Thickness(0, 0, 0, 4) });
        titel.Children.Add(new TextBlock { Text = $"Du benutzt {Aktualisierung.Anzeige(EigeneVersion)}. Dein Lernstand bleibt beim Aktualisieren erhalten.", FontSize = 12.5, Foreground = Ui.B("TextMittel"), TextWrapping = TextWrapping.Wrap });
        kopf.Children.Add(titel);
        inhalt.Children.Add(kopf);

        inhalt.Children.Add(new TextBlock { Text = "Neu in dieser Version", FontSize = 11.5, FontWeight = FontWeights.SemiBold, Foreground = Ui.B("TextGedimmt"), Margin = new Thickness(0, 16, 0, 6) });
        inhalt.Children.Add(new Border
        {
            BorderBrush = Ui.B("Rand"), BorderThickness = new Thickness(1), Background = Ui.B("Hintergrund"),
            Child = new ScrollViewer
            {
                MaxHeight = 220, VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Padding = new Thickness(12, 10, 12, 10),
                Content = new TextBlock
                {
                    Text = string.IsNullOrWhiteSpace(r.Notizen) ? "Keine Release-Notizen vorhanden." : r.Notizen,
                    FontFamily = Ui.CodeSchrift, FontSize = 12, LineHeight = 19, Foreground = Ui.B("TextHell"), TextWrapping = TextWrapping.Wrap,
                },
            },
        });

        var status = new TextBlock { FontSize = 11.5, Foreground = Ui.B("TextMittel"), Margin = new Thickness(0, 14, 0, 6), Visibility = Visibility.Collapsed, TextWrapping = TextWrapping.Wrap };
        var balken = new ProgressBar { Height = 6, Minimum = 0, Maximum = 1, Visibility = Visibility.Collapsed };
        inhalt.Children.Add(status);
        inhalt.Children.Add(balken);

        var leiste = new WrapPanel { HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 22, 0, 0) };
        var ueberspringen = new Button { Content = "Diese Version überspringen", Style = (Style)Application.Current.Resources["RandKnopf"], FontSize = 12.5, Padding = new Thickness(16, 10, 16, 10), Margin = new Thickness(0, 0, 10, 0) };
        var spaeter = new Button { Content = "Später", Style = (Style)Application.Current.Resources["GeisterKnopf"], MinWidth = 100, Margin = new Thickness(0, 0, 10, 0) };
        var jetzt = new Button { Content = "Jetzt aktualisieren", Style = (Style)Application.Current.Resources["AkzentKnopf"], MinWidth = 160 };
        leiste.Children.Add(ueberspringen); leiste.Children.Add(spaeter); leiste.Children.Add(jetzt);
        inhalt.Children.Add(leiste);

        var karte = new Border
        {
            Padding = new Thickness(26, 24, 26, 22), Background = Ui.B("DialogFlaeche"), BorderBrush = Ui.B("RandHell"), BorderThickness = new Thickness(1),
            Effect = new DropShadowEffect { Color = Colors.Black, BlurRadius = 24, ShadowDepth = 4, Direction = 270, Opacity = 0.5 },
            Width = 600, Child = inhalt,
        };
        var deko = new Grid { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        deko.Children.Add(karte);
        deko.Children.Add(new Border { Height = 2, Background = akzent, VerticalAlignment = VerticalAlignment.Top, IsHitTestVisible = false });
        ebene.Children.Add(deko);

        CancellationTokenSource laden = null;
        bool zu = false;
        void Schliessen()
        {
            if (zu) return;
            zu = true;
            laden?.Cancel();
            var aus = new DoubleAnimation(0, TimeSpan.FromMilliseconds(90));
            aus.Completed += (_, _) =>
            {
                Wurzel.Children.Remove(ebene);
                DialogService.EbeneGeschlossen();
                vorherFokus?.Focus();
                fertig.TrySetResult(true);
            };
            ebene.IsHitTestVisible = false;
            ebene.BeginAnimation(OpacityProperty, aus);
        }

        spaeter.Click += (_, _) => Schliessen();
        ueberspringen.Click += (_, _) =>
        {
            fortschritt.UpdateUebersprungen = neu;
            fortschritt.Speichern();
            Schliessen();
        };
        ebene.PreviewKeyDown += (_, e) => { if (e.Key == Key.Escape) { Schliessen(); e.Handled = true; } };
        jetzt.Click += async (_, _) =>
        {
            if (r.AssetUrl == null)
            {
                Schliessen();
                await ReleaseSeiteAnbieten("Kein Download gefunden", $"Im Release {neu} fehlt die Datei „{Aktualisierung.AssetName}“. Du kannst die neue Version auf der Release-Seite herunterladen.", DialogKind.Warnung, r.SeitenUrl);
                return;
            }
            if (notizSeite?.EntwurfAbschliessen() == false) return;
            jetzt.IsEnabled = false; ueberspringen.IsEnabled = false;
            spaeter.Content = "Abbrechen";
            status.Visibility = balken.Visibility = Visibility.Visible;
            laden = new CancellationTokenSource();
            try
            {
                await Installieren(r, status, balken, laden.Token);
            }
            catch (OperationCanceledException) when (laden.IsCancellationRequested) { }
            catch (Exception ex)
            {
                Fortschritt.Protokoll(ex, "Update installieren");
                Schliessen();
                string grund = ex is AktualisierungsFehler ? ex.Message : "Beim Herunterladen ist ein Fehler aufgetreten: " + ex.Message;
                await ReleaseSeiteAnbieten("Update nicht möglich", grund + "\n\nSharpLab läuft unverändert weiter. Du kannst die neue Version auch selbst von der Release-Seite laden und den Ordner ersetzen.", DialogKind.Warnung, r.SeitenUrl);
            }
        };

        DialogService.EbeneGeoeffnet();
        Wurzel.Children.Add(ebene);
        ebene.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(120)));
        jetzt.Loaded += (_, _) => { jetzt.Focus(); Keyboard.Focus(jetzt); };
        await fertig.Task;
    }

    /// <summary>Herunterladen → prüfen → entpacken → austauschen → neu starten. Wirft bei Fehlern (alles zurückgerollt).</summary>
    async Task Installieren(ReleaseInfo r, TextBlock status, ProgressBar balken, CancellationToken ct)
    {
        string exe = Environment.ProcessPath;
        if (exe == null || !Path.GetFileName(exe).EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
            || Path.GetFileName(exe).Equals("dotnet.exe", StringComparison.OrdinalIgnoreCase))
            throw new AktualisierungsFehler("Automatisch aktualisieren geht nur mit der SharpLab.exe aus dem Download.");
        string programmOrdner = Path.GetDirectoryName(exe);
        if (!Aktualisierung.IstBeschreibbar(programmOrdner))
            throw new AktualisierungsFehler($"SharpLab darf im Ordner „{programmOrdner}“ nichts ändern (fehlende Schreibrechte, z. B. unter „Programme“).");

        string neu = Aktualisierung.Anzeige(r.Version);
        string basis = Path.Combine(Path.GetTempPath(), "SharpLab-Update-" + neu);
        string zip = basis + ".zip";
        using var client = Aktualisierung.NeuerClient(Aktualisierung.Anzeige(EigeneVersion));
        double mb = r.AssetGroesse / 1048576.0;
        var anzeige = new Progress<double>(p =>
        {
            if (p < 0) { balken.IsIndeterminate = true; status.Text = "Lade herunter …"; return; }
            balken.IsIndeterminate = false;
            balken.Value = p;
            status.Text = mb > 0 ? $"Lade herunter … {p * 100:0} % ({p * mb:0.0} von {mb:0.0} MB)" : $"Lade herunter … {p * 100:0} %";
        });
        status.Text = "Lade herunter …";
        await Task.Run(() => Aktualisierung.HerunterladenAsync(client, r.AssetUrl, zip, anzeige, ct), ct);

        // Prüfsumme, falls das Release eine angibt (im Text oder als .sha256-Datei)
        string soll = r.Sha256;
        if (soll == null && r.Sha256AssetUrl != null)
        {
            string text = await client.GetStringAsync(r.Sha256AssetUrl, ct);
            soll = Aktualisierung.HashAusText(text, r.AssetName);
        }
        if (string.IsNullOrWhiteSpace(soll))
            throw new AktualisierungsFehler("Das Release enthält keine überprüfbare SHA-256-Prüfsumme. Aus Sicherheitsgründen wurde das automatische Update abgebrochen. Bitte prüfe die Release-Seite.");
        status.Text = "Prüfe Prüfsumme (SHA-256) …";
        string exeOrdner = await Task.Run(() =>
        {
            if (!string.Equals(Aktualisierung.Sha256(zip), soll, StringComparison.OrdinalIgnoreCase))
                throw new AktualisierungsFehler("Die Prüfsumme des Downloads stimmt nicht (Datei beschädigt oder verändert). Es wurde nichts geändert.");
            return Aktualisierung.Entpacken(zip, basis);
        }, ct);
        ct.ThrowIfCancellationRequested();

        status.Text = "Installiere …";
        SpeichereEditor();
        fortschritt.Speichern();
        var tausch = Aktualisierung.Austauschen(exe, exeOrdner);
        try
        {
            Process.Start(new ProcessStartInfo(tausch.NeueExe) { UseShellExecute = false, WorkingDirectory = programmOrdner, ArgumentList = { Aktualisierung.NachUpdateArgument, neu } });
        }
        catch (Exception ex)
        {
            tausch.Rueckgaengig();
            throw new AktualisierungsFehler("Die neue Version ließ sich nicht starten (" + ex.Message + "). Die alte Version wurde wiederhergestellt.", ex);
        }
        try { File.Delete(zip); } catch { }
        Application.Current.Shutdown();
    }

    /// <summary>Wurde SharpLab per Setup installiert und danach per Auto-Update (oder ZIP) ausgetauscht,
    /// zeigt „Apps &amp; Features“ sonst noch die alte Version. Still, Fehler nur ins Protokoll.</summary>
    static void UninstallVersionNachziehen(string exeOrdner)
    {
        try
        {
            using var k = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(Aktualisierung.UninstallSchluessel, writable: true);
            if (k == null || !Aktualisierung.GehoertZurInstallation(k.GetValue("InstallLocation") as string, exeOrdner)) return;
            string v = Aktualisierung.Anzeige(EigeneVersion);
            if (!string.Equals(k.GetValue("DisplayVersion") as string, v, StringComparison.Ordinal)) k.SetValue("DisplayVersion", v);
        }
        catch (Exception ex) { Fortschritt.Protokoll(ex, "Uninstall-Eintrag aktualisieren"); }
    }
}
