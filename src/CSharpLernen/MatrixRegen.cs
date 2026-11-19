using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace CSharpLernen;

/// <summary>
/// Animierter „Matrix-Regen“ im Hintergrund: fallende Zeichenspalten mit hellem Kopf und
/// verblassendem grünen Schweif. Zeichnet mit zwischengespeicherten FormattedText-Objekten
/// (pro Zeichen und Helligkeitsstufe) und begrenzt sich auf ca. 30 Bilder pro Sekunde.
/// </summary>
public sealed class MatrixRegen : FrameworkElement
{
    const double Zelle = 18;          // Spaltenbreite und Zeilenhöhe in Pixel
    const int Stufen = 8;             // Helligkeitsstufen des Schweifs
    const double BildIntervall = 1.0 / 24;

    static readonly string[] Zeichen = BaueZeichen();

    readonly Random zufall = new();
    readonly Stopwatch uhr = Stopwatch.StartNew();
    readonly List<Tropfen> tropfen = new();
    FormattedText[,] cache;           // [Zeichen, Stufe]; Stufe 0 = heller Kopf
    double letzteZeit;
    double dpi = 1;
    bool laeuft;
    double staerke = 0.7;
    readonly DrawingGroup bild = new();   // wird pro Bild neu befüllt – ohne Layout-Durchlauf

    sealed class Tropfen
    {
        public double X;
        public double Y;              // Kopfposition in Zellen
        public double Tempo;          // Zellen pro Sekunde
        public int Laenge;
        public int[] Glyphen = Array.Empty<int>();
        public double Wechsel;        // Zeit bis zum nächsten Zeichenwechsel
    }

    public MatrixRegen()
    {
        IsHitTestVisible = false;
        Focusable = false;
        Loaded += (_, _) => { dpi = VisualTreeHelper.GetDpi(this).PixelsPerDip; Starten(); };
        Unloaded += (_, _) => Anhalten();
        IsVisibleChanged += (_, _) => { if (IsVisible) Starten(); else Anhalten(); };
        SizeChanged += (_, _) => Verteilen();
    }

    /// <summary>0 … 1: steuert Deckkraft und Dichte.</summary>
    public double Staerke
    {
        get => staerke;
        set { staerke = Math.Clamp(value, 0, 1); Opacity = 0.10 + 0.70 * staerke; Verteilen(); }
    }

    public void Starten()
    {
        if (laeuft || !IsVisible) return;
        laeuft = true;
        letzteZeit = uhr.Elapsed.TotalSeconds;
        CompositionTarget.Rendering += BeiRendern;
    }

    public void Anhalten()
    {
        if (!laeuft) return;
        laeuft = false;
        CompositionTarget.Rendering -= BeiRendern;
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
        cache = new FormattedText[Zeichen.Length, Stufen];
        var schrift = new Typeface(new FontFamily("Consolas, MS Gothic, Yu Gothic, Segoe UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        var schriftFett = new Typeface(new FontFamily("Consolas, MS Gothic, Yu Gothic, Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
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
                cache[z, s] = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
                    s == 0 ? schriftFett : schrift, groesse, pinsel[s], dpi);
        }
    }

    static Brush Gefroren(Color c) { var b = new SolidColorBrush(c); b.Freeze(); return b; }

    void Verteilen()
    {
        if (ActualWidth <= 0 || ActualHeight <= 0) return;
        int spalten = (int)(ActualWidth / Zelle);
        int ziel = Math.Max(4, (int)(spalten * (0.15 + 0.4 * staerke)));
        while (tropfen.Count > ziel) tropfen.RemoveAt(tropfen.Count - 1);
        while (tropfen.Count < ziel) tropfen.Add(NeuerTropfen(true));
    }

    Tropfen NeuerTropfen(bool irgendwo)
    {
        int spalten = Math.Max(1, (int)(ActualWidth / Zelle));
        var t = new Tropfen
        {
            X = zufall.Next(spalten) * Zelle + 2,
            Tempo = 2.5 + zufall.NextDouble() * 5,
            Laenge = 8 + zufall.Next(22),
        };
        t.Y = irgendwo ? zufall.NextDouble() * (ActualHeight / Zelle + t.Laenge) : -zufall.Next(20);
        t.Glyphen = new int[t.Laenge];
        for (int i = 0; i < t.Laenge; i++) t.Glyphen[i] = zufall.Next(Zeichen.Length);
        return t;
    }

    void BeiRendern(object sender, EventArgs e)
    {
        var fenster = Window.GetWindow(this);
        if (fenster != null && fenster.WindowState == WindowState.Minimized) { letzteZeit = uhr.Elapsed.TotalSeconds; return; }
        double jetzt = uhr.Elapsed.TotalSeconds;
        double dt = jetzt - letzteZeit;
        if (dt < BildIntervall) return;
        letzteZeit = jetzt;
        dt = Math.Min(dt, 0.1);

        double zeilen = ActualHeight / Zelle;
        for (int i = 0; i < tropfen.Count; i++)
        {
            var t = tropfen[i];
            t.Y += t.Tempo * dt;
            t.Wechsel -= dt;
            if (t.Wechsel <= 0)
            {
                t.Glyphen[zufall.Next(t.Laenge)] = zufall.Next(Zeichen.Length);
                t.Wechsel = 0.4 + zufall.NextDouble() * 1.2;
            }
            if (t.Y - t.Laenge > zeilen) tropfen[i] = NeuerTropfen(false);
        }
        using var dc = bild.Open();
        Zeichnen(dc);
    }

    protected override void OnRender(DrawingContext dc)
    {
        using (var g = bild.Open()) Zeichnen(g);
        dc.DrawDrawing(bild);
    }

    void Zeichnen(DrawingContext dc)
    {
        if (ActualWidth <= 0 || ActualHeight <= 0) return;
        if (cache == null) BaueCache();
        if (tropfen.Count == 0) Verteilen();
        double zeilen = ActualHeight / Zelle;
        foreach (var t in tropfen)
        {
            int kopf = (int)t.Y;
            for (int k = 0; k < t.Laenge; k++)
            {
                int zeile = kopf - k;
                if (zeile < 0) break;
                if (zeile > zeilen) continue;
                int stufe = k == 0 ? 0 : 1 + (int)((k - 1) * (Stufen - 1) / (double)t.Laenge);
                var ft = cache[t.Glyphen[(zeile & 0x7FFFFFFF) % t.Laenge], Math.Min(stufe, Stufen - 1)];
                dc.DrawText(ft, new Point(t.X + (Zelle - ft.Width) / 2, zeile * Zelle));
            }
        }
    }
}
