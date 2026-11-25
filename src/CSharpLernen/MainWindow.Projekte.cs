using System.Windows;
using System.Windows.Controls;

namespace CSharpLernen;

public partial class MainWindow
{
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
            var projekt = new StackPanel();
            projekt.Children.Add(Ui.Text(titel, 13, "TextHell", FontWeights.SemiBold));
            var projektAktionen = new WrapPanel { Margin = new Thickness(0, 6, 0, 0) };
            projektAktionen.Children.Add(Ui.Knopf("Öffnen", (_, _) => ProjektOeffnen(id, titel), false, "\uE8E5"));
            projektAktionen.Children.Add(Ui.Knopf("Editorcode als neue Version speichern",
                (_, _) => ProjektVersionSpeichern(id, titel), true, "\uE74E"));
            projekt.Children.Add(projektAktionen);

            if (fortschritt.ProjektVersionen.TryGetValue(id, out var versionen) && versionen.Count > 0)
            {
                var verlauf = new StackPanel();
                for (int i = versionen.Count - 1; i >= 0; i--)
                {
                    string code = versionen[i];
                    var knopf = Ui.Knopf($"Version {i + 1} in Editor übernehmen", (_, _) =>
                    {
                        ProjektOeffnen(id, titel);
                        CodeInEditor(code);
                    });
                    knopf.Margin = new Thickness(0, 4, 0, 0);
                    knopf.ToolTip = "Kopiert diese Version in den aktuellen Arbeitsstand. Mit Strg+Z rückgängig machen.";
                    verlauf.Children.Add(knopf);
                }
                projekt.Children.Add(new Expander
                {
                    Header = $"Versionsverlauf ({versionen.Count})",
                    Foreground = Ui.B("TextMittel"),
                    Content = verlauf,
                    Margin = new Thickness(0, 6, 0, 0),
                });
            }

            abschnitt.Children.Add(Ui.Karte(projekt, rand: new Thickness(0, 8, 0, 0)));
        }
        SeitenInhalt.Children.Add(Ui.Karte(abschnitt));
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
