using System.Windows;
using System.Windows.Controls;

namespace CSharpLernen;

public partial class MainWindow
{
    static string ProjektSchluessel(Guid id) => "projekt:" + id.ToString("N");

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
            if (code.Length > 0) fortschritt.Code[ProjektSchluessel(id)] = code;
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
            var knopf = Ui.Knopf(titel, (_, _) => ProjektOeffnen(id, titel), false, "\uE8E5");
            knopf.Margin = new Thickness(0, 6, 0, 0);
            knopf.HorizontalContentAlignment = HorizontalAlignment.Left;
            knopf.ToolTip = "Projekt öffnen";
            abschnitt.Children.Add(knopf);
        }
        SeitenInhalt.Children.Add(Ui.Karte(abschnitt));
    }

    void ProjektOeffnen(Guid id, string titel)
    {
        PruefenKnopf.Visibility = Visibility.Collapsed;
        SetzeEditorKontext(ProjektSchluessel(id), "", Array.Empty<string>(), titel);
        Editor.Focus();
    }
}
