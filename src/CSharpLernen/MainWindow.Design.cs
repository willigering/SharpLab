using System.Windows;
using System.Windows.Controls;

namespace CSharpLernen;

public partial class MainWindow
{
    void SeiteDesign()
    {
        SeitenKopf("Design", "Wähle das Farbschema für SharpLab.", "\uE790");
        var auswahl = new StackPanel();
        var matrixOptionen = MatrixOptionen();
        foreach (var (id, name) in new[] { ("matrix", "Matrix"), ("silber", "Dark Silver"), ("gold", "Dark Gold") })
        {
            var reiter = new RadioButton
            {
                GroupName = "Farbschema",
                Style = (Style)FindResource("NavKnopf"),
                Content = name,
                Tag = "\uE790",
                IsChecked = fortschritt.Design == id,
            };
            reiter.Checked += (_, _) =>
            {
                fortschritt.Design = id;
                Farbdesign.Anwenden(id);
                SetzeMatrix(fortschritt.MatrixRegen, fortschritt.MatrixStufe, speichern: false);
                matrixOptionen.Visibility = id == "matrix" ? Visibility.Visible : Visibility.Collapsed;
                fortschritt.Speichern();
            };
            auswahl.Children.Add(reiter);
        }
        SeitenInhalt.Children.Add(Ui.Karte(auswahl));
        SeitenInhalt.Children.Add(matrixOptionen);
    }

    Border MatrixOptionen()
    {
        var optik = new StackPanel();
        optik.Children.Add(Ui.Text("Matrixcode", 13, "TextHell", FontWeights.SemiBold, new Thickness(0, 0, 0, 6)));
        var regenAn = new CheckBox { Content = "Matrixcode im Hintergrund anzeigen", IsChecked = fortschritt.MatrixRegen, FontSize = 12.5 };
        var staerkeText = Ui.Text($"Helligkeit: {fortschritt.MatrixStufe * 100:0} %", 12.5, "TextMittel", rand: new Thickness(0, 12, 0, 0));
        var staerke = new Slider { Minimum = 0, Maximum = 1, Value = fortschritt.MatrixStufe, SmallChange = 0.05, LargeChange = 0.1, Margin = new Thickness(0, 4, 0, 4), Width = 420, HorizontalAlignment = HorizontalAlignment.Left, IsEnabled = fortschritt.MatrixRegen };
        var tempoText = Ui.Text($"Geschwindigkeit: {TempoText(fortschritt.MatrixTempo)}", 12.5, "TextMittel", rand: new Thickness(0, 8, 0, 0));
        var tempo = new Slider { Minimum = 0.3, Maximum = 2.5, Value = fortschritt.MatrixTempo, SmallChange = 0.1, LargeChange = 0.25, Margin = new Thickness(0, 4, 0, 4), Width = 420, HorizontalAlignment = HorizontalAlignment.Left, IsEnabled = fortschritt.MatrixRegen };
        einstMatrixAn = regenAn; einstMatrixRegler = staerke; einstMatrixText = staerkeText; einstMatrixTempo = tempo; einstMatrixTempoText = tempoText;
        regenAn.Click += (_, _) => SetzeMatrix(regenAn.IsChecked == true, fortschritt.MatrixStufe);
        staerke.ValueChanged += (_, _) => { if (!matrixSync) SetzeMatrix(fortschritt.MatrixRegen, staerke.Value); };
        tempo.ValueChanged += (_, _) => { if (!matrixSync) SetzeMatrix(fortschritt.MatrixRegen, fortschritt.MatrixStufe, tempo.Value); };
        optik.Children.Add(regenAn);
        optik.Children.Add(staerkeText);
        optik.Children.Add(staerke);
        optik.Children.Add(tempoText);
        optik.Children.Add(tempo);
        optik.Children.Add(Ui.Text("Zeichen bauen sich im Hintergrund auf und klingen ab – auch links im Menü regelbar. Ab 50 % Helligkeit werden die Flächen etwas durchsichtiger. Wird angehalten, wenn das Fenster minimiert ist.", 11, "TextGedimmt", rand: new Thickness(0, 0, 0, 0)));
        var karte = Ui.Karte(optik);
        karte.Visibility = fortschritt.Design == "matrix" ? Visibility.Visible : Visibility.Collapsed;
        return karte;

    }
}
