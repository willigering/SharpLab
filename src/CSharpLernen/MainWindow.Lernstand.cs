using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CSharpLernen.Engine;

namespace CSharpLernen;

/// <summary>Einstellungen → „Lernstand“: wo fortschritt.json liegt (Standard, eigener Ordner oder OneDrive).</summary>
public partial class MainWindow
{
    StackPanel lernstandHost;

    static Button KleinerKnopf(string text, RoutedEventHandler klick, bool akzent, string icon)
    {
        var b = Ui.Knopf(text, klick, akzent, icon);
        b.Margin = new Thickness(0, 0, 8, 8);
        return b;
    }

    void BaueLernstandKarte()
    {
        var k = lernstandHost;
        if (k == null) return;
        k.Children.Clear();
        string ordner = Fortschritt.LernstandOrdner ?? Fortschritt.Ordner;
        bool standard = Speicherort.IstStandard(ordner);
        string oneDrive = Speicherort.OneDriveOrdner();
        bool aufOneDrive = oneDrive != null && Speicherort.IstGleich(ordner, Speicherort.LernstandOrdner(oneDrive));

        k.Children.Add(Ui.Text("Lernstand", 15.5, "TextHell", FontWeights.SemiBold, new Thickness(0, 0, 0, 10)));
        k.Children.Add(Ui.Text("Gelöste Aufgaben, Quiz-Ergebnisse und dein Code werden in diesem Ordner gespeichert:", 12.5, "TextMittel"));
        k.Children.Add(new TextBox
        {
            Text = Path.Combine(ordner, Speicherort.DateiName),
            IsReadOnly = true,
            TextWrapping = TextWrapping.Wrap,   // lange Pfade umbrechen statt abschneiden
            FontFamily = Ui.CodeSchrift,
            FontSize = 11.5,
            Margin = new Thickness(0, 6, 0, 4),
            BorderThickness = new Thickness(1),
        });
        k.Children.Add(Ui.Text(standard ? "Standard-Ordner (nur auf diesem Rechner)" : aufOneDrive ? "OneDrive – wird mit deinen anderen Rechnern synchronisiert" : "Eigener Ordner",
            11, standard ? "TextGedimmt" : "Akzent", FontWeights.SemiBold, new Thickness(0, 0, 0, 12)));

        var leiste = new WrapPanel();
        leiste.Children.Add(KleinerKnopf("Ordner wählen…", async (_, _) => await OrdnerWaehlen(), true, "\uE8DA"));
        if (oneDrive != null && !aufOneDrive)
            leiste.Children.Add(KleinerKnopf("OneDrive verwenden", async (_, _) => await WechsleSpeicherort(oneDrive), false, "\uE753"));
        if (!standard)
            leiste.Children.Add(KleinerKnopf("Standard", async (_, _) => await WechsleSpeicherort(null), false, "\uE777"));
        leiste.Children.Add(KleinerKnopf("Ordner öffnen", (_, _) =>
        {
            try
            {
                Directory.CreateDirectory(ordner);
                Process.Start(new ProcessStartInfo("explorer.exe", ordner) { UseShellExecute = true });
            }
            catch (Exception ex) { _ = DialogService.ShowAsync("Ordner nicht geöffnet", ex.Message, DialogKind.Gefahr, "OK"); }
        }, false, "\uE8B7"));
        k.Children.Add(leiste);

        string tipp = oneDrive != null
            ? $"Tipp: Mit „OneDrive verwenden“ liegt dein Lernstand in {Speicherort.LernstandOrdner(oneDrive)} – auf dem anderen Rechner genauso wählen und „Diesen übernehmen“ klicken."
            : "Tipp: Wähle einen Ordner in OneDrive, Dropbox oder auf einem USB-Stick – auf dem anderen Rechner denselben Ordner wählen und „Diesen übernehmen“ klicken.";
        k.Children.Add(Ui.Text(tipp, 11, "TextGedimmt", rand: new Thickness(0, 4, 0, 4)));
        k.Children.Add(Ui.Text("SharpLab legt dort einen Unterordner „SharpLab“ an. Der Stand wird beim Start geladen – nutze SharpLab nicht gleichzeitig auf zwei Rechnern. Einstellungen wie Schriftgröße, Matrixcode und Autovervollständigung bleiben auf jedem Rechner eigen.",
            11, "TextGedimmt", rand: new Thickness(0, 0, 0, 10)));
        k.Children.Add(Ui.Text($"Eigene Lektionen: Lege eine Datei „Lektionen.json“ (gleiches Format wie die eingebaute) in {Fortschritt.Ordner} – dann lädt die App beim nächsten Start deine Lektionen.",
            11, "TextGedimmt", rand: new Thickness(0, 0, 0, 10)));

        k.Children.Add(KleinerKnopf("Fortschritt zurücksetzen", async (_, _) => await FortschrittZuruecksetzen(), false, "\uE74D"));
    }

    async Task OrdnerWaehlen()
    {
        string aktuell = Fortschritt.LernstandOrdner ?? Fortschritt.Ordner;
        var dlg = new Microsoft.Win32.OpenFolderDialog
        {
            Title = "Ordner für deinen Lernstand wählen",
            Multiselect = false,
        };
        string start = Speicherort.IstStandard(aktuell) ? Speicherort.OneDriveOrdner() : Path.GetDirectoryName(aktuell);
        if (!string.IsNullOrEmpty(start) && Directory.Exists(start)) dlg.InitialDirectory = start;
        bool gewaehlt;
        try { gewaehlt = dlg.ShowDialog(this) == true; }
        catch (Exception ex)
        {
            Fortschritt.Protokoll(ex, "Ordnerauswahl");
            await DialogService.ShowAsync("Ordnerauswahl nicht möglich", ex.Message, DialogKind.Warnung, "OK");
            return;
        }
        if (gewaehlt && !string.IsNullOrWhiteSpace(dlg.FolderName))
            await WechsleSpeicherort(dlg.FolderName);
    }

    static string Beschreibe(Fortschritt f, DateTime? geaendert)
    {
        string zeit = geaendert is { } t ? $", zuletzt gespeichert {t:dd.MM.yyyy HH:mm}" : "";
        return $"{f.GeloesteAufgaben.Count} gelöste Aufgaben, {f.GeleseneLektionen.Count} gelesene Lektionen{zeit}";
    }

    /// <summary>Wechselt zum Basisordner <paramref name="basis"/> (null = Standard).</summary>
    async Task WechsleSpeicherort(string basis)
    {
        string ziel;
        try { ziel = Speicherort.LernstandOrdner(basis); }
        catch (Exception ex)
        {
            Fortschritt.Protokoll(ex, "Speicherort wählen");
            await DialogService.ShowAsync("Ungültiger Ordner", "Mit diesem Ordner kann SharpLab nichts anfangen. Bitte wähle einen anderen.", DialogKind.Warnung, "OK");
            return;
        }
        if (Speicherort.IstGleich(ziel, Fortschritt.LernstandOrdner ?? Fortschritt.Ordner))
        {
            Toast("Schon ausgewählt", "Dein Lernstand liegt bereits in diesem Ordner.", "\uE946", "Akzent");
            return;
        }
        if (!Speicherort.IstBeschreibbar(ziel, out var fehler))
        {
            Fortschritt.Protokoll(new IOException($"Lernstand-Ordner nicht beschreibbar: {ziel} – {fehler}"), "Speicherort wählen");
            await DialogService.ShowAsync("Ordner nicht nutzbar",
                $"In diesen Ordner kann SharpLab nicht schreiben:\n{ziel}\n\nDein Lernstand bleibt, wo er ist. Wähle bitte einen anderen Ordner.",
                DialogKind.Warnung, "OK", hint: "Details stehen in %AppData%\\CSharpLernen\\fehler.log");
            return;
        }

        SpeichereEditor();
        var art = Speicherort.Wechselart.MeinenKopieren;
        string zielDatei = Speicherort.LernstandDatei(ziel);
        Fortschritt dort = null;
        if (File.Exists(zielDatei))
        {
            dort = Fortschritt.LadeDatei(zielDatei);
            string dortText = dort != null ? Beschreibe(dort, File.GetLastWriteTime(zielDatei)) : "Die Datei ist beschädigt und kann nicht gelesen werden.";
            var antwort = await DialogService.ShowAsync("Dort gibt es schon einen Lernstand",
                $"Im Ordner\n{ziel}\nliegt bereits ein Lernstand.\n\nDort: {dortText}\nHier: {Beschreibe(fortschritt, null)}",
                DialogKind.Frage,
                dort != null ? "Diesen übernehmen" : "Meinen aktuellen dorthin kopieren",
                dort != null ? "Meinen aktuellen dorthin kopieren" : null,
                "Beim Kopieren wird der vorhandene Stand vorher als Sicherung (fortschritt.vorher-….json) aufgehoben.",
                cancelText: "Abbrechen");
            if (antwort == DialogAntwort.Abgebrochen) return;
            art = antwort == DialogAntwort.Primaer && dort != null ? Speicherort.Wechselart.VorhandenenUebernehmen : Speicherort.Wechselart.MeinenKopieren;
        }

        try
        {
            Speicherort.Wechseln(basis, fortschritt.LernstandJson(), art);
        }
        catch (Exception ex)
        {
            Fortschritt.Protokoll(ex, "Speicherort wechseln");
            await DialogService.ShowAsync("Wechsel nicht möglich",
                $"Der Lernstand konnte nicht nach\n{ziel}\ngeschrieben werden. Er bleibt am bisherigen Ort.\n\n{ex.Message}",
                DialogKind.Warnung, "OK", hint: "Details stehen in %AppData%\\CSharpLernen\\fehler.log");
            return;
        }

        Fortschritt.LernstandOrdner = ziel;
        if (art == Speicherort.Wechselart.VorhandenenUebernehmen && dort != null)
        {
            fortschritt.UebernimmLernstand(dort);
            bekannteErfolge = Erfolge.Erreicht(fortschritt, kurs);
            editorSchluessel = null;
            editorGeaendert = false;
            AktualisiereFortschritt();
            BaueLernpfad();
        }
        fortschritt.Speichern();
        BaueLernstandKarte();
        Toast("Speicherort geändert",
            art == Speicherort.Wechselart.VorhandenenUebernehmen ? "Der Lernstand aus dem neuen Ordner ist jetzt geladen." : "Dein Lernstand liegt jetzt im neuen Ordner.",
            "\uE73E", "Akzent");
    }

    async Task FortschrittZuruecksetzen()
    {
        if (await DialogService.ShowAsync("Fortschritt zurücksetzen?",
                "Wirklich ALLES zurücksetzen? Gelöste Aufgaben, Quiz-Ergebnisse und dein gespeicherter Code werden gelöscht.",
                DialogKind.Gefahr, "Alles löschen", "Abbrechen", "Das kann nicht rückgängig gemacht werden.") != DialogAntwort.Primaer) return;
        fortschritt.GeloesteAufgaben.Clear();
        fortschritt.Code.Clear();
        fortschritt.QuizPunkte.Clear();
        fortschritt.GeleseneLektionen.Clear();
        fortschritt.ErledigteFreieAufgaben.Clear();
        fortschritt.LetzteFreieAufgabe = null;
        freieStufe = null;
        freieAufgabe = null;
        fortschritt.Ausfuehrungen = fortschritt.ErfolgreicheLaeufe = fortschritt.SpielwieseLaeufe = 0;
        fortschritt.FehlerBehoben = fortschritt.EndlosschleifeGestoppt = false;
        fortschritt.ErsterStart = DateTime.Now;
        fortschritt.Speichern();
        bekannteErfolge = Erfolge.Erreicht(fortschritt, kurs);
        editorSchluessel = null;
        editorGeaendert = false;
        AktualisiereFortschritt();
        BaueLernpfad();
        Toast("Zurückgesetzt", "Alles auf Anfang – viel Spaß beim Lernen!", "\uE72C", "Akzent");
    }

    /// <summary>Hinweise rund ums Speichern: Fallback beim Start, Fehler beim Schreiben während der Sitzung.</summary>
    void SpeicherortHinweiseEinrichten()
    {
        Fortschritt.Speicherfehler += ex => Dispatcher.BeginInvoke(() =>
            _ = DialogService.ShowAsync("Lernstand nicht gespeichert",
                $"Dein Lernstand konnte gerade nicht gespeichert werden:\n{Fortschritt.Datei}\n\n{ex.Message}\n\nPrüfe, ob der Ordner erreichbar ist, oder wähle in den Einstellungen wieder den Standard-Ordner.",
                DialogKind.Warnung, "OK", hint: "Details stehen in %AppData%\\CSharpLernen\\fehler.log"));
        if (Fortschritt.StartHinweis is { } h)
            Loaded += (_, _) => Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle,
                () => _ = DialogService.ShowAsync("Speicherort nicht erreichbar", h, DialogKind.Warnung, "OK", hint: "Details stehen in %AppData%\\CSharpLernen\\fehler.log"));
    }
}
