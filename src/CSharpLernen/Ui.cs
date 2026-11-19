using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace CSharpLernen;

/// <summary>Kleine Helfer, um Oberflächen-Elemente im Code zu bauen (Texte, Karten, Code-Blöcke, Markdown).</summary>
public static class Ui
{
    public static Brush B(string key) => (Brush)Application.Current.Resources[key];
    public static FontFamily CodeSchrift => (FontFamily)Application.Current.Resources["CodeSchrift"];
    public static FontFamily Icons => (FontFamily)Application.Current.Resources["Icons"];
    public static Brush Farbe(string hex) { var b = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex)); b.Freeze(); return b; }

    public static TextBlock Text(string text, double groesse = 14, string farbe = "TextHell", FontWeight? gewicht = null, Thickness? rand = null)
    {
        return new TextBlock
        {
            Text = text,
            FontSize = groesse,
            Foreground = B(farbe),
            FontWeight = gewicht ?? FontWeights.Normal,
            TextWrapping = TextWrapping.Wrap,
            Margin = rand ?? new Thickness(0),
            LineHeight = groesse * 1.5,
        };
    }

    public static TextBlock Icon(string glyph, double groesse = 16, string farbe = "Akzent")
    {
        return new TextBlock { Text = glyph, FontFamily = Icons, FontSize = groesse, Foreground = B(farbe), VerticalAlignment = VerticalAlignment.Center };
    }

    public static Border Karte(UIElement inhalt, Thickness? rand = null, Thickness? innen = null, string hintergrund = "KarteHell")
    {
        var karte = new Border
        {
            Style = (Style)Application.Current.Resources["KartenRahmen"],
            CornerRadius = new CornerRadius(0),
            Padding = innen ?? new Thickness(16),
            Margin = rand ?? new Thickness(0, 0, 0, 14),
            Child = inhalt,
        };
        karte.SetResourceReference(Border.BackgroundProperty, hintergrund);   // folgt der Matrix-Deckkraft
        return karte;
    }

    public static Button Knopf(string text, RoutedEventHandler klick, bool akzent = false, string icon = null)
    {
        var sp = new StackPanel { Orientation = Orientation.Horizontal };
        var farbe = akzent ? Farbe("#0B180F") : B("TextHell");
        if (icon != null)
            sp.Children.Add(new TextBlock { Text = icon, FontFamily = Icons, Margin = new Thickness(0, 2, 8, 0), Foreground = farbe, FontSize = 11.5 });
        sp.Children.Add(new TextBlock { Text = text, Foreground = farbe });
        var b = new Button
        {
            Content = sp,
            Style = (Style)Application.Current.Resources[akzent ? "AkzentKnopf" : "RandKnopf"],
            Margin = new Thickness(0, 0, 8, 0),
        };
        b.Click += klick;
        return b;
    }

    /// <summary>Info-Kasten („Hinweis“, „Tipp“) mit farbigem Rand links.</summary>
    public static Border Hinweis(string titel, string text, string farbe = "Akzent", string icon = "\uE946")
    {
        var sp = new StackPanel();
        var kopf = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 4) };
        kopf.Children.Add(Icon(icon, 15, farbe));
        kopf.Children.Add(new TextBlock { Text = "  " + titel, FontWeight = FontWeights.SemiBold, Foreground = B(farbe), FontSize = 12.5 });
        sp.Children.Add(kopf);
        var tb = new TextBlock { TextWrapping = TextWrapping.Wrap, Foreground = B("TextMittel"), FontSize = 12.5, LineHeight = 19 };
        InlinesAusMarkdown(tb.Inlines, text);
        sp.Children.Add(tb);
        return new Border
        {
            Background = Farbe("#121F16"),
            BorderBrush = B(farbe),
            BorderThickness = new Thickness(3, 0, 0, 0),
            CornerRadius = new CornerRadius(0),
            Padding = new Thickness(14, 10, 14, 12),
            Margin = new Thickness(0, 4, 0, 16),
            Child = sp,
        };
    }

    // ---------------- Markdown (einfach) ----------------

    /// <summary>Wandelt einfaches Markdown (##, -, **fett**, `code`, ```-Blöcke) in WPF-Elemente um.</summary>
    public static void Markdown(Panel ziel, string md, Action<string> codeLaden = null)
    {
        var zeilen = (md ?? "").Replace("\r\n", "\n").Split('\n');
        var absatz = new List<string>();

        void AbsatzFertig()
        {
            if (absatz.Count == 0) return;
            var tb = new TextBlock { TextWrapping = TextWrapping.Wrap, FontSize = 13, LineHeight = 20.5, Margin = new Thickness(0, 0, 0, 12), Foreground = B("TextMittel") };
            InlinesAusMarkdown(tb.Inlines, string.Join(" ", absatz));
            ziel.Children.Add(tb);
            absatz.Clear();
        }

        for (int i = 0; i < zeilen.Length; i++)
        {
            string z = zeilen[i];
            string t = z.Trim();
            if (t.StartsWith("```"))
            {
                AbsatzFertig();
                var code = new List<string>();
                i++;
                while (i < zeilen.Length && !zeilen[i].Trim().StartsWith("```")) { code.Add(zeilen[i]); i++; }
                ziel.Children.Add(CodeKarte(string.Join("\n", code), null, codeLaden));
                continue;
            }
            if (t.StartsWith("## ") || t.StartsWith("# "))
            {
                AbsatzFertig();
                ziel.Children.Add(new TextBlock
                {
                    Text = t.TrimStart('#', ' '),
                    FontSize = 16, FontWeight = FontWeights.SemiBold, Foreground = B("TextHell"),
                    Margin = new Thickness(0, 10, 0, 8), TextWrapping = TextWrapping.Wrap,
                });
                continue;
            }
            if (t.StartsWith("- ") || t.StartsWith("* ") || Regex.IsMatch(t, @"^\d+\.\s"))
            {
                AbsatzFertig();
                bool nummer = char.IsDigit(t[0]);
                string zeichen = nummer ? t.Substring(0, t.IndexOf('.') + 1) : "•";
                string rest = nummer ? t.Substring(t.IndexOf('.') + 1).Trim() : t.Substring(2);
                var g = new Grid { Margin = new Thickness(6, 0, 0, 7) };
                g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(22) });
                g.ColumnDefinitions.Add(new ColumnDefinition());
                g.Children.Add(new TextBlock { Text = zeichen, Foreground = B("Akzent"), FontSize = 13, FontWeight = FontWeights.Bold });
                var tb = new TextBlock { TextWrapping = TextWrapping.Wrap, FontSize = 13, LineHeight = 20, Foreground = B("TextMittel") };
                InlinesAusMarkdown(tb.Inlines, rest);
                Grid.SetColumn(tb, 1);
                g.Children.Add(tb);
                ziel.Children.Add(g);
                continue;
            }
            if (t.Length == 0) { AbsatzFertig(); continue; }
            absatz.Add(t);
        }
        AbsatzFertig();
    }

    static readonly Regex InlineMuster = new(@"(\*\*.+?\*\*|`[^`]+`)", RegexOptions.Compiled);

    public static void InlinesAusMarkdown(InlineCollection inlines, string text)
    {
        foreach (string teil in InlineMuster.Split(text ?? ""))
        {
            if (teil.Length == 0) continue;
            if (teil.StartsWith("**") && teil.EndsWith("**") && teil.Length > 4)
                inlines.Add(new Run(teil[2..^2]) { FontWeight = FontWeights.SemiBold, Foreground = B("TextHell") });
            else if (teil.StartsWith("`") && teil.EndsWith("`") && teil.Length > 2)
                inlines.Add(new Run(" " + teil[1..^1] + " ") { FontFamily = CodeSchrift, Background = B("InlineCode"), Foreground = B("Akzent"), FontSize = 12 });
            else
                inlines.Add(new Run(teil));
        }
    }

    // ---------------- Code-Karte mit Zeilennummern und Hervorhebung ----------------

    public static Border CodeKarte(string code, string titel = null, Action<string> inEditorLaden = null, string ladenText = "In Editor laden")
    {
        code = (code ?? "").Replace("\r\n", "\n").TrimEnd('\n');
        int anzahl = code.Split('\n').Length;

        var kopf = new Grid { Margin = new Thickness(0, 0, 0, 8) };
        var links = new StackPanel { Orientation = Orientation.Horizontal };
        links.Children.Add(new Border
        {
            Background = Farbe("#5B2A86"), CornerRadius = new CornerRadius(0), Width = 22, Height = 16, Margin = new Thickness(0, 0, 8, 0),
            Child = new TextBlock { Text = "C#", FontSize = 8, FontWeight = FontWeights.Bold, Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center },
        });
        links.Children.Add(new TextBlock { Text = titel ?? "Code", FontSize = 11, Foreground = B("TextGedimmt"), VerticalAlignment = VerticalAlignment.Center });
        kopf.Children.Add(links);

        var rechts = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
        if (inEditorLaden != null)
        {
            var laden = KleinerKnopf("\uE768", ladenText);
            laden.Click += (_, _) => inEditorLaden(code + "\n");
            rechts.Children.Add(laden);
        }
        var kopieren = KleinerKnopf("\uE8C8", "Kopieren");
        kopieren.Click += (_, _) =>
        {
            try { Clipboard.SetText(code.Replace("\n", Environment.NewLine)); } catch { }
            if (kopieren.Content is StackPanel sp && sp.Children[1] is TextBlock tb) tb.Text = "Kopiert ✓";
        };
        rechts.Children.Add(kopieren);
        kopf.Children.Add(rechts);

        var nummern = new TextBlock
        {
            FontFamily = CodeSchrift, FontSize = 12, Foreground = Farbe("#476150"), TextAlignment = TextAlignment.Right,
            Margin = new Thickness(0, 0, 14, 0), Text = string.Join("\n", Enumerable.Range(1, anzahl)),
        };
        var codeText = new TextBlock { FontFamily = CodeSchrift, FontSize = 12, Foreground = Farbe("#B3CCBB") };
        Hervorheben(codeText.Inlines, code);

        var raster = new Grid();
        raster.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        raster.ColumnDefinitions.Add(new ColumnDefinition());
        raster.Children.Add(nummern);
        Grid.SetColumn(codeText, 1);
        raster.Children.Add(codeText);

        var scroller = new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
            Content = raster,
        };

        var sp2 = new StackPanel();
        sp2.Children.Add(kopf);
        sp2.Children.Add(scroller);
        return new Border
        {
            Background = B("CodeHintergrund"),
            BorderBrush = B("Rand"),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(0),
            Padding = new Thickness(14, 10, 14, 12),
            Margin = new Thickness(0, 4, 0, 16),
            Child = sp2,
        };
    }

    static Button KleinerKnopf(string icon, string text)
    {
        var sp = new StackPanel { Orientation = Orientation.Horizontal };
        sp.Children.Add(new TextBlock { Text = icon, FontFamily = Icons, FontSize = 10, Margin = new Thickness(0, 1, 6, 0), Foreground = B("TextMittel") });
        sp.Children.Add(new TextBlock { Text = text, FontSize = 11, Foreground = B("TextMittel") });
        return new Button
        {
            Content = sp,
            Style = (Style)Application.Current.Resources["RandKnopf"],
            Padding = new Thickness(8, 3, 8, 3),
            Margin = new Thickness(6, 0, 0, 0),
        };
    }

    static readonly HashSet<string> Steuerung = new() { "if", "else", "switch", "case", "default", "while", "do", "for", "foreach", "in", "break", "continue", "return", "goto", "try", "catch", "finally", "throw", "yield", "await" };
    static readonly HashSet<string> Typen = new() { "int", "long", "short", "byte", "double", "float", "decimal", "bool", "char", "string", "object", "void", "var", "uint", "ulong", "Console", "Math", "List", "Dictionary", "Random", "String", "Program", "DateTime", "Convert", "Exception" };
    static readonly HashSet<string> Werte = new() { "true", "false", "null", "this", "base", "value" };
    static readonly HashSet<string> Schluessel = new() { "using", "namespace", "class", "struct", "interface", "enum", "record", "public", "private", "protected", "internal", "static", "readonly", "const", "new", "override", "virtual", "abstract", "sealed", "get", "set", "init", "out", "ref", "params", "is", "as", "typeof", "nameof", "async", "and", "or", "not", "when" };

    static readonly Regex Token = new(
        @"(?<k>//[^\n]*|/\*[\s\S]*?\*/)|(?<s>\$?@?""(?:\\.|""""|[^""\\\n])*""|'(?:\\.|[^'\\\n])')|(?<n>\b\d+(?:\.\d+)?[fFdDmMlL]?\b)|(?<w>[A-Za-z_][A-Za-z0-9_]*)",
        RegexOptions.Compiled);

    public static void Hervorheben(InlineCollection inlines, string code)
    {
        int pos = 0;
        foreach (Match m in Token.Matches(code))
        {
            if (m.Index > pos) Hinzu(inlines, code[pos..m.Index], null);
            string t = m.Value;
            string farbe = null;
            if (m.Groups["k"].Success) farbe = "#76907F";
            else if (m.Groups["s"].Success) farbe = "#F2C185";
            else if (m.Groups["n"].Success) farbe = "#99C4A7";
            else if (Steuerung.Contains(t)) farbe = "#E08CF0";
            else if (Typen.Contains(t)) farbe = "#6CBC87";
            else if (Werte.Contains(t) || Schluessel.Contains(t)) farbe = "#5FB8FF";
            else
            {
                int nach = m.Index + m.Length;
                while (nach < code.Length && code[nach] == ' ') nach++;
                if (nach < code.Length && code[nach] == '(') farbe = "#F3E9A0";
            }
            Hinzu(inlines, t, farbe);
            pos = m.Index + m.Length;
        }
        if (pos < code.Length) Hinzu(inlines, code[pos..], null);
    }

    static void Hinzu(InlineCollection inlines, string text, string farbe)
    {
        var teile = text.Split('\n');
        for (int i = 0; i < teile.Length; i++)
        {
            if (i > 0) inlines.Add(new LineBreak());
            if (teile[i].Length == 0) continue;
            var r = new Run(teile[i]);
            if (farbe != null) r.Foreground = Farbe(farbe);
            inlines.Add(r);
        }
    }

    public static Effect Leuchten => (Effect)Application.Current.Resources["Leuchten"];
}
