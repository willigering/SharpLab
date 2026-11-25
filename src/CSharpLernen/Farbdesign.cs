using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace CSharpLernen;

static class Farbdesign
{
    sealed class Farbton(Color matrix) : INotifyPropertyChanged
    {
        public Color Matrix { get; } = matrix;
        public Color Wert { get; private set; } = matrix;
        public event PropertyChangedEventHandler PropertyChanged;

        public void Setzen(Color wert)
        {
            if (Wert == wert) return;
            Wert = wert;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Wert)));
        }
    }

    static readonly Dictionary<string, Farbton> Farbtoene = new();

    public static void Vorbereiten(ResourceDictionary ressourcen)
    {
        var palette = ressourcen.MergedDictionaries[0];
        foreach (string key in palette.Keys.OfType<string>().ToArray())
        {
            if (palette[key] is not SolidColorBrush brush || key is "Gruen" or "Rot" or "Gelb") continue;
            var farbton = new Farbton(brush.Color);
            var pinsel = new SolidColorBrush();
            BindingOperations.SetBinding(pinsel, SolidColorBrush.ColorProperty,
                new Binding(nameof(Farbton.Wert)) { Source = farbton, Mode = BindingMode.OneWay });
            Farbtoene.Add(key, farbton);
            palette[key] = pinsel;
        }
        ressourcen[SystemColors.HighlightBrushKey] = palette["DialogAuswahl"];
        ressourcen[SystemColors.HighlightTextBrushKey] = palette["AkzentZart"];
    }

    public static void Anwenden(string design)
    {
        foreach (var (key, farbton) in Farbtoene)
            farbton.Setzen(design == "matrix" ? farbton.Matrix : Farbe(key, farbton.Matrix, design == "gold"));
        Application.Current.Resources["AkzentFarbe"] = Farbtoene["Akzent"].Wert;
    }

    static Color Farbe(string key, Color original, bool gold)
    {
        string hex = key switch
        {
            "Akzent" or "AkzentVerlauf" => gold ? "#E4BA63" : "#D1D5DC",
            "AkzentHell" or "AkzentRegler" => gold ? "#F4D18A" : "#EDF0F5",
            "AkzentGedrueckt" => gold ? "#C49849" : "#AEB5C0",
            "AkzentDunkel" => gold ? "#BB934F" : "#929CA9",
            "AkzentZart" => gold ? "#F5D99E" : "#E7EAF0",
            "AkzentText" or "PrimaerText" or "Kontrollhaken" => gold ? "#181107" : "#15181D",
            "AkzentTextDunkel" => gold ? "#4E3818" : "#3D4652",
            "Rand" => gold ? "#504027" : "#363B43",
            "RandHell" or "AuswahlRahmen" or "MenueRahmen" => gold ? "#776038" : "#616B79",
            "TextHell" or "EditorText" or "AusgabeText" => gold ? "#E9DEC8" : "#E0E4EA",
            "TextMittel" or "MenueText" or "AuswahlText" => gold ? "#C3B598" : "#B4BEC9",
            "TextGedimmt" or "EditorZeilen" or "NotizZeilen" => gold ? "#A09680" : "#949EAC",
            "AkzentHintergrund" or "AuswahlFuellung" or "MenueZeileAktiv" => gold ? "#382C1A" : "#292F39",
            "IconHover" or "IconGedrueckt" or "MenueAuswahl" or "EditorAuswahl" or "DialogAuswahl" => gold ? "#E4BA63" : "#A9B6C7",
            _ => null,
        };
        if (hex != null)
        {
            var farbe = (Color)ColorConverter.ConvertFromString(hex);
            farbe.A = original.A;
            return farbe;
        }

        // Preserve the brightness hierarchy of borders, inactive controls and panels.
        int hell = (original.R * 21 + original.G * 72 + original.B * 7) / 100;
        int abstand = Math.Min(hell / 6, 8);
        return gold
            ? Color.FromArgb(original.A, (byte)Math.Min(255, hell + abstand), (byte)hell, (byte)(hell - abstand))
            : Color.FromArgb(original.A, (byte)Math.Max(0, hell - 1), (byte)hell, (byte)Math.Min(255, hell + 3));
    }
}
