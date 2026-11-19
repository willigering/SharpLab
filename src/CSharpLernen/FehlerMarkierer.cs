using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;

namespace CSharpLernen;

/// <summary>Färbt im Editor die Zeilen mit Fehlern rot (und Warnungen gelb) ein.</summary>
public sealed class FehlerMarkierer : IBackgroundRenderer
{
    readonly Dictionary<int, bool> zeilen = new(); // Zeile -> istWarnung
    static readonly Brush FehlerPinsel = Gefroren(Color.FromArgb(0x38, 0xFF, 0x50, 0x50));
    static readonly Brush WarnPinsel = Gefroren(Color.FromArgb(0x28, 0xFF, 0xC8, 0x57));
    static readonly Brush FehlerRand = Gefroren(Color.FromRgb(0xFF, 0x6B, 0x6B));
    static readonly Brush WarnRand = Gefroren(Color.FromRgb(0xFF, 0xC8, 0x57));

    static Brush Gefroren(Color c) { var b = new SolidColorBrush(c); b.Freeze(); return b; }

    public KnownLayer Layer => KnownLayer.Background;

    public void Setzen(IEnumerable<(int Zeile, bool Warnung)> eintraege)
    {
        zeilen.Clear();
        foreach (var (z, w) in eintraege)
        {
            if (z <= 0) continue;
            if (zeilen.TryGetValue(z, out bool schonWarnung)) zeilen[z] = schonWarnung && w;
            else zeilen[z] = w;
        }
    }

    public void Leeren() => zeilen.Clear();

    public void Draw(TextView textView, DrawingContext dc)
    {
        if (zeilen.Count == 0 || !textView.VisualLinesValid) return;
        foreach (var vl in textView.VisualLines)
        {
            int nr = vl.FirstDocumentLine.LineNumber;
            if (!zeilen.TryGetValue(nr, out bool warnung)) continue;
            double y = vl.VisualTop - textView.ScrollOffset.Y;
            var rect = new Rect(0, y, Math.Max(textView.ActualWidth, 10), vl.Height);
            dc.DrawRectangle(warnung ? WarnPinsel : FehlerPinsel, null, rect);
            dc.DrawRectangle(warnung ? WarnRand : FehlerRand, null, new Rect(0, y, 3, vl.Height));
        }
    }
}
