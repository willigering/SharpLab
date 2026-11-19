using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace CSharpLernen;

/// <summary>
/// Animierter „Matrix-Regen“ im Hintergrund: fallende Zeichenspalten mit hellem Kopf und
/// verblassendem grünen Schweif.
///
/// Auf flüssige Darstellung (Bildwiederholrate des Monitors, i. d. R. 60 FPS) ausgelegt:
/// <list type="bullet">
/// <item>Jedes Zeichen wird pro Helligkeitsstufe genau einmal als eingefrorene Zeichnung
/// (GlyphRun) vorbereitet.</item>
/// <item>Jeder Tropfen ist eine fest aufgebaute Zeichnungsgruppe; pro Bild wird nur deren
/// Verschiebung (TranslateTransform) gesetzt – keine neuen Objekte, kein Neuzeichnen, das
/// Verschieben erledigt die GPU.</item>
/// <item>Bewegung ist zeitbasiert (Stopwatch) mit Sub-Pixel-Positionen, gekoppelt an
/// CompositionTarget.Rendering, ohne Bildbremse.</item>
/// </list>
/// </summary>
public sealed class MatrixRegen : FrameworkElement
{
    const double Zelle = 18;          // Spaltenbreite und Zeilenhöhe in DIP
    const int Stufen = 8;             // Helligkeitsstufen des Schweifs
    const int MaxLaenge = 30;         // längster Schweif (Zeichen)

    static readonly string[] Zeichen = BaueZeichen();

    readonly Random zufall = new();
    readonly Stopwatch uhr = Stopwatch.StartNew();
    readonly List<Tropfen> tropfen = new();
    readonly Stack<Tropfen> reserve = new();
    readonly DrawingGroup wurzel = new();          // einmal in OnRender gezeichnet, danach nur noch verändert
    Drawing[,] cache;                             // [Zeichen, Stufe]; Stufe 0 = heller Kopf
    double letzteZeit;
    TimeSpan letzterFrame;
    double dpi = 1;
    bool laeuft;
    bool aktiv;                                    // Rendering-Ereignis abonniert
    double staerke = 0.7;
    Window fenster;

    sealed class Tropfen
    {
        public double Y;                                   // Kopfposition in Zellen (Sub-Pixel)
        public double Tempo;                               // Zellen pro Sekunde
        public int Laenge;
        public double Wechsel;                             // Zeit bis zum nächsten Zeichenwechsel
        public readonly TranslateTransform Lage = new();
        public readonly DrawingGroup Gruppe = new();
        public readonly DrawingGroup[] Plaetze = new DrawingGroup[MaxLaenge];
        public readonly int[] Glyphen = new int[MaxLaenge];

        public Tropfen()
        {
            Gruppe.Transform = Lage;
            for (int k = 0; k < MaxLaenge; k++)
            {
                var versatz = new TranslateTransform(0, -k * Zelle);
                versatz.Freeze();
                Plaetze[k] = new DrawingGroup { Transform = versatz };
            }
        }
    }

    public MatrixRegen()
    {
        IsHitTestVisible = false;
        Focusable = false;
        ClipToBounds = true;
        // Text, der sich bewegt: Hinting für Animation, damit Glyphen nicht von Pixel zu Pixel springen.
        TextOptions.SetTextHintingMode(this, TextHintingMode.Animated);
        TextOptions.SetTextRenderingMode(this, TextRenderingMode.Grayscale);
        RenderOptions.SetEdgeMode(this, EdgeMode.Unspecified);
        RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.LowQuality);

        Loaded += (_, _) =>
        {
            NeueDpi(VisualTreeHelper.GetDpi(this).PixelsPerDip);
            fenster = Window.GetWindow(this);
            if (fenster != null) fenster.StateChanged += FensterZustand;
            laeuft = true;
            Aktualisieren();
        };
        Unloaded += (_, _) =>
        {
            if (fenster != null) fenster.StateChanged -= FensterZustand;
            fenster = null;
            laeuft = false;
            Aktualisieren();
        };
        IsVisibleChanged += (_, _) => Aktualisieren();
        SizeChanged += (_, _) => Verteilen();
    }

    /// <summary>0 … 1: steuert Deckkraft und Dichte.</summary>
    public double Staerke
    {
        get => staerke;
        set { staerke = Math.Clamp(value, 0, 1); Opacity = 0.10 + 0.70 * staerke; Verteilen(); }
    }

    public void Starten() { laeuft = true; Aktualisieren(); }
    public void Anhalten() { laeuft = false; Aktualisieren(); }

    void FensterZustand(object sender, EventArgs e) => Aktualisieren();

    /// <summary>Abonniert das Rendering-Ereignis nur, wenn wirklich etwas zu sehen ist.</summary>
    void Aktualisieren()
    {
        bool soll = laeuft && IsVisible && (fenster == null || fenster.WindowState != WindowState.Minimized);
        if (soll == aktiv) return;
        aktiv = soll;
        if (soll)
        {
            letzteZeit = uhr.Elapsed.TotalSeconds;   // nach Pause nicht springen
            CompositionTarget.Rendering += BeiRendern;
        }
        else CompositionTarget.Rendering -= BeiRendern;
    }

    static string[] BaueZeichen()
    {
        var l = new List<string>();
        for (int c = 0xFF66; c <= 0xFF9D; c++) l.Add(((char)c).ToString());   // Halbbreiten-Katakana
        for (char c = '0'; c <= '9'; c++) l.Add(c.ToString());
        foreach (var t in new[] { "{", "}", ";", "=>", "()", "[]", "+=", "++", "==", "&&", "int", "var", "if", "for", "new", "<T>" }) l.Add(t);
        return l.ToArray();
    }

    void BaueCache()
    {
        var c = new Drawing[Zeichen.Length, Stufen];
        var familie = new FontFamily("Consolas, MS Gothic, Yu Gothic, Segoe UI");
        var schrift = new Typeface(familie, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        var schriftFett = new Typeface(familie, FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
        var pinsel = new Brush[Stufen];
        pinsel[0] = Gefroren(Color.FromRgb(0xE2, 0xF5, 0xE7));                       // Kopf: fast weiß
        for (int s = 1; s < Stufen; s++)
        {
            double t = (s - 1) / (double)(Stufen - 1);                                // 0 = hell, 1 = dunkel
            byte a = (byte)(255 * (1 - t * 0.75));
            pinsel[s] = Gefroren(Color.FromArgb(a, (byte)(0x3F - t * 0x28), (byte)(0xCB - t * 0x78), (byte)(0x6E - t * 0x40)));
        }
        for (int z = 0; z < Zeichen.Length; z++)
        {
            string text = Zeichen[z];
            double groesse = text.Length == 1 ? 15 : (text.Length == 2 ? 11 : 9);
            for (int s = 0; s < Stufen; s++)
            {
                var ft = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
                    s == 0 ? schriftFett : schrift, groesse, pinsel[s], dpi);
                var g = new DrawingGroup();
                using (var dc = g.Open()) dc.DrawText(ft, new Point((Zelle - ft.Width) / 2, (Zelle - ft.Height) / 2));
                g.Freeze();
                c[z, s] = g;
            }
        }
        cache = c;
    }

    protected override void OnDpiChanged(DpiScale alt, DpiScale neu)
    {
        base.OnDpiChanged(alt, neu);
        NeueDpi(neu.PixelsPerDip);
    }

    /// <summary>Glyphen für die tatsächliche Bildschirmskalierung (z. B. 125 %) neu vorbereiten.</summary>
    void NeueDpi(double neu)
    {
        if (Math.Abs(neu - dpi) < 0.001 && cache != null) return;
        dpi = neu;
        if (cache == null) return;      // wird beim ersten Verteilen mit der richtigen DPI gebaut
        BaueCache();
        foreach (var t in tropfen) for (int k = 0; k < t.Laenge; k++) SetzeGlyphe(t, k);
    }

    static Brush Gefroren(Color c) { var b = new SolidColorBrush(c); b.Freeze(); return b; }

    void Verteilen()
    {
        if (ActualWidth <= 0 || ActualHeight <= 0) return;
        if (cache == null) BaueCache();
        int spalten = (int)(ActualWidth / Zelle);
        int ziel = Math.Max(4, (int)(spalten * (0.15 + 0.4 * staerke)));
        while (tropfen.Count > ziel)
        {
            var t = tropfen[^1];
            tropfen.RemoveAt(tropfen.Count - 1);
            wurzel.Children.Remove(t.Gruppe);
            reserve.Push(t);
        }
        while (tropfen.Count < ziel)
        {
            var t = reserve.Count > 0 ? reserve.Pop() : new Tropfen();
            Neu(t, irgendwo: true);
            tropfen.Add(t);
            wurzel.Children.Add(t.Gruppe);
        }
        // Tropfen außerhalb einer schmaler gewordenen Fläche neu setzen
        foreach (var t in tropfen) if (t.Lage.X > ActualWidth) Neu(t, irgendwo: true);
    }

    /// <summary>Setzt einen Tropfen neu auf (wiederverwendet alle Objekte).</summary>
    void Neu(Tropfen t, bool irgendwo)
    {
        int spalten = Math.Max(1, (int)(ActualWidth / Zelle));
        t.Tempo = 2.5 + zufall.NextDouble() * 5;
        int laenge = 8 + zufall.Next(MaxLaenge - 8 + 1);
        t.Y = irgendwo ? zufall.NextDouble() * (ActualHeight / Zelle + laenge) : -zufall.Next(20);
        t.Wechsel = zufall.NextDouble() * 1.2;
        t.Lage.X = zufall.Next(spalten) * Zelle;
        t.Lage.Y = t.Y * Zelle;

        if (laenge != t.Laenge)
        {
            var kinder = t.Gruppe.Children;
            while (kinder.Count > laenge) kinder.RemoveAt(kinder.Count - 1);
            while (kinder.Count < laenge) kinder.Add(t.Plaetze[kinder.Count]);
            t.Laenge = laenge;
        }
        for (int k = 0; k < laenge; k++)
        {
            t.Glyphen[k] = zufall.Next(Zeichen.Length);
            SetzeGlyphe(t, k);
        }
    }

    void SetzeGlyphe(Tropfen t, int k)
    {
        int stufe = k == 0 ? 0 : Math.Min(Stufen - 1, 1 + (int)((k - 1) * (Stufen - 1) / (double)t.Laenge));
        var platz = t.Plaetze[k].Children;
        var d = cache[t.Glyphen[k], stufe];
        if (platz.Count == 0) platz.Add(d); else platz[0] = d;
    }

    void BeiRendern(object sender, EventArgs e)
    {
        // Rendering kann pro Bild mehrfach feuern – nur einmal pro tatsächlichem Frame rechnen.
        if (e is RenderingEventArgs re)
        {
            if (re.RenderingTime == letzterFrame) return;
            letzterFrame = re.RenderingTime;
        }
        if (cache == null || ActualHeight <= 0) return;

        double jetzt = uhr.Elapsed.TotalSeconds;
        double dt = Math.Min(jetzt - letzteZeit, 0.1);   // nach Hängern nicht springen
        letzteZeit = jetzt;
        if (dt <= 0) return;

        double zeilen = ActualHeight / Zelle;
        for (int i = 0; i < tropfen.Count; i++)
        {
            var t = tropfen[i];
            t.Y += t.Tempo * dt;
            t.Wechsel -= dt;
            if (t.Wechsel <= 0)
            {
                int k = zufall.Next(t.Laenge);
                t.Glyphen[k] = zufall.Next(Zeichen.Length);
                SetzeGlyphe(t, k);
                t.Wechsel = 0.4 + zufall.NextDouble() * 1.2;
            }
            if (t.Y - t.Laenge > zeilen) Neu(t, irgendwo: false);
            else t.Lage.Y = t.Y * Zelle;               // Sub-Pixel-Position, Verschiebung macht die GPU
        }
    }

    protected override void OnRender(DrawingContext dc)
    {
        if (cache == null) Verteilen();
        dc.DrawDrawing(wurzel);
    }
}
