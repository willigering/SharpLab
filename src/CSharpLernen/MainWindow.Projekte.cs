using System.Windows;
using System.Windows.Controls;
using System.Windows.Automation;

namespace CSharpLernen;

public partial class MainWindow
{
    readonly HashSet<Guid> offeneProjekte = new();
    void EigeneProjekteAnzeigen()
    {
        var abschnitt = new StackPanel();
        abschnitt.Children.Add(Ui.Text("Eigene Projekte", 15.5, "TextHell", FontWeights.SemiBold));
        var name = new TextBox { MaxLength = 100, Margin = new Thickness(0, 4, 0, 8) };
        var label = new Label { Content = "Projektname", Target = name, Foreground = Ui.B("TextMittel"), Padding = new Thickness(0, 4, 0, 0) };
        abschnitt.Children.Add(label);
        abschnitt.Children.Add(name);
        var aktionen = new WrapPanel();
        void Anlegen(string code)
        {
            string titel = name.Text.Trim();
            if (titel.Length == 0)
            {
                name.Focus();
                return;
            }

            var id = Guid.NewGuid();
            fortschritt.EigeneProjekte.Add(id, titel);
            if (code.Length > 0) fortschritt.Code[Fortschritt.ProjektSchluessel(id)] = code;
            ProjektOeffnen(id, titel);
            fortschritt.Speichern();
            BaueSeite("projekte");
        }

        var neu = Ui.Knopf("Neues Projekt", (_, _) => Anlegen(""), true, "\uE710");
        var kopie = Ui.Knopf("Editorcode als Projekt speichern", (_, _) => Anlegen(Editor.Text), false, "\uE74E");
        neu.Margin = new Thickness(0, 0, 8, 4);
        kopie.Margin = new Thickness(0, 0, 0, 4);
        neu.IsEnabled = kopie.IsEnabled = false;
        name.TextChanged += (_, _) => neu.IsEnabled = kopie.IsEnabled = !string.IsNullOrWhiteSpace(name.Text);
        aktionen.Children.Add(neu);
        aktionen.Children.Add(kopie);
        abschnitt.Children.Add(aktionen);

        foreach (var (id, titel) in fortschritt.EigeneProjekte.OrderBy(p => p.Value, StringComparer.CurrentCultureIgnoreCase))
        {
            abschnitt.Children.Add(ProjektKarte(id, titel));
        }
        SeitenInhalt.Children.Add(Ui.Karte(abschnitt));
    }

    Border ProjektKarte(Guid id, string titel)
    {
        var inhalt = new StackPanel { Margin = new Thickness(0, 8, 0, 0) };
        var aktionen = new WrapPanel();
        aktionen.Children.Add(Ui.Knopf("Öffnen", (_, _) => ProjektOeffnen(id, titel), false, "\uE8E5"));
        aktionen.Children.Add(Ui.Knopf("Editorcode als neue Version speichern",
            (_, _) => ProjektVersionSpeichern(id, titel), true, "\uE74E"));
        inhalt.Children.Add(aktionen);

        if (fortschritt.ProjektVersionen.TryGetValue(id, out var versionen) && versionen.Any(code => code != null))
        {
            inhalt.Children.Add(Ui.Text("Versionsverlauf", 12, "TextMittel", rand: new Thickness(0, 8, 0, 4)));
            for (int i = versionen.Count - 1; i >= 0; i--)
            {
                string code = versionen[i];
                if (code == null) continue;
                int nummer = i + 1;
                var version = new DockPanel { Margin = new Thickness(0, 4, 0, 0) };
                var papierkorb = Papierkorb($"Version {nummer} löschen", async (_, _) =>
                    await ProjektVersionLoeschen(id, titel, nummer));
                DockPanel.SetDock(papierkorb, Dock.Right);
                version.Children.Add(papierkorb);

                var laden = Ui.Knopf($"Version {nummer} in Editor übernehmen", (_, _) =>
                {
                    ProjektOeffnen(id, titel);
                    CodeInEditor(code);
                });
                laden.Margin = new Thickness(0);
                laden.HorizontalContentAlignment = HorizontalAlignment.Left;
                laden.ToolTip = "Kopiert diese Version in den aktuellen Arbeitsstand. Mit Strg+Z rückgängig machen.";
                version.Children.Add(laden);
                inhalt.Children.Add(version);
            }
        }

        var zeile = new DockPanel();
        var projektPapierkorb = Papierkorb($"Projekt „{titel}“ löschen", async (_, _) => await ProjektLoeschen(id, titel));
        DockPanel.SetDock(projektPapierkorb, Dock.Right);
        zeile.Children.Add(projektPapierkorb);

        var einklappen = new Expander
        {
            Header = Ui.Text(titel, 13, "TextHell", FontWeights.SemiBold),
            Foreground = Ui.B("TextMittel"),
            Content = inhalt,
            IsExpanded = offeneProjekte.Contains(id),
        };
        einklappen.Expanded += (_, _) => offeneProjekte.Add(id);
        einklappen.Collapsed += (_, _) => offeneProjekte.Remove(id);
        zeile.Children.Add(einklappen);
        return Ui.Karte(zeile, rand: new Thickness(0, 8, 0, 0), innen: new Thickness(8, 4, 6, 4));
    }

    Button Papierkorb(string beschriftung, RoutedEventHandler klick)
    {
        var knopf = new Button
        {
            Style = (Style)FindResource("IconKnopf"),
            Content = "\uE74D",
            ToolTip = beschriftung,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(6, 0, 0, 0),
        };
        AutomationProperties.SetName(knopf, beschriftung);
        knopf.Click += klick;
        return knopf;
    }

    async Task ProjektLoeschen(Guid id, string titel)
    {
        if (await DialogService.ShowAsync("Projekt löschen?", $"„{titel}“ wird mit Arbeitsstand und allen Versionen gelöscht.",
                DialogKind.Gefahr, "Löschen", "Abbrechen") != DialogAntwort.Primaer) return;

        if (editorSchluessel == Fortschritt.ProjektSchluessel(id))
            SetzeEditorKontext("spielwiese", SpielwieseStart, Array.Empty<string>(), "Spielwiese");
        fortschritt.ProjektLoeschen(id);
        offeneProjekte.Remove(id);
        fortschritt.Speichern();
        if (seite == "projekte") BaueSeite("projekte");
    }

    async Task ProjektVersionLoeschen(Guid id, string titel, int nummer)
    {
        if (await DialogService.ShowAsync("Version löschen?", $"Version {nummer} von „{titel}“ wird aus dem Verlauf gelöscht.",
                DialogKind.Gefahr, "Löschen", "Abbrechen") != DialogAntwort.Primaer) return;

        fortschritt.ProjektVersionLoeschen(id, nummer);
        fortschritt.Speichern();
        if (seite == "projekte") BaueSeite("projekte");
    }

    void ProjektVersionSpeichern(Guid id, string titel)
    {
        // Vor dem Kontextwechsel festhalten. Im aktiven Zielprojekt steht die neue Version bereits im Editor.
        int version = fortschritt.ProjektVersionAnlegen(id, Editor.Text);
        ProjektOeffnen(id, titel);
        if (editorGeaendert) SpeichereEditor();
        else fortschritt.Speichern();
        BaueSeite("projekte");
        Status($"{titel} · Version {version}", "TextMittel");
    }

    void ProjektOeffnen(Guid id, string titel)
    {
        PruefenKnopf.Visibility = Visibility.Collapsed;
        SetzeEditorKontext(Fortschritt.ProjektSchluessel(id), "", Array.Empty<string>(), titel);
        Editor.Focus();
    }
}
