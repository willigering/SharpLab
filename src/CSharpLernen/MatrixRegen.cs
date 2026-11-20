using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CSharpLernen;

/// <summary>
/// Matrix-Regen „wie im Film“: Die Zeichen stehen in einem festen Raster und bewegen sich nie.
/// Pro Spalte schreibt ein Kopf Zeichen für Zeichen nach unten; das neueste Zeichen leuchtet fast weiß,
/// danach klingt jedes Zeichen an seinem Platz grün ab, bis es verschwunden ist. Sichtbare Zeichen
/// wechseln gelegentlich zufällig.
///
/// Technik (auf 60 FPS ausgelegt, keine Allokationen pro Bild):
/// <list type="bullet">
/// <item>Zellraster mit Zeichen, Schreibzeitpunkt und Abklingdauer je Zelle.</item>
/// <item>Glyphen-Atlas einmalig vorgerendert (Abdeckungsmasken normal + fett, in Bildschirmpixeln/DPI)
/// plus Farbtabelle je Helligkeitsstufe.</item>
/// <item>Zeichnen direkt in eine WriteableBitmap; nur Zellen, deren Stufe oder Zeichen sich geändert hat,
/// werden neu geschrieben (Dirty-Rects pro Spalte).</item>
/// <item>CompositionTarget.Rendering + Stopwatch, Pause bei minimiert/unsichtbar/aus.</item>
/// </list>
/// Vorlage: Vorschau film.html (gleiche Parameter).
/// </summary>
public sealed class MatrixRegen : FrameworkElement
{
    // ---- Parameter (wie in der Vorschau) ----
    const double ZelleDip = 18;
    const double TempoMin = 7, TempoMax = 13;            // Schritte (Zellen) pro Sekunde je Spalte
    const double AbklingenMin = 2.0, AbklingenMax = 4.0; // Sekunden bis ein Zeichen verschwunden ist
    const double Nachgluehen = 0.12;                     // Sekunden in Kopffarbe
    const double Mutation = 0.35;                        // Zeichenwechsel pro sichtbarem Zeichen und Sekunde
    const double PauseMin = 0.2, PauseMax = 1.8;         // Wartezeit bis eine Spalte neu startet
    const double Vorlauf = 6.0;                          // Sekunden Vorsimulation, damit der Bildschirm gleich gefüllt ist
    const int Stufen = 16;                               // Abklingstufen 0..15, Stufe 16 = Kopf
    const int AtlasSpalten = 16;

    static readonly Color KopfFarbe = Color.FromRgb(0xE2, 0xF5, 0xE7);
    static readonly Color Hell = Color.FromRgb(0x3F, 0xCB, 0x6E);
    static readonly Color Dunkel = Color.FromRgb(0x17, 0x53, 0x2E);

    static readonly string[] AlleZeichen = BaueZeichen();
    static readonly string[] Schriften = { "MS Gothic", "Yu Gothic", "Yu Gothic UI", "Meiryo", "MS UI Gothic", "Consolas", "Segoe UI" };

    readonly Random zufall = new();
    readonly Stopwatch uhr = Stopwatch.StartNew();
    readonly uint[] farbTabelle = BaueFarbTabelle();     // [(Stufe)*256 + Abdeckung] → Pbgra32

    // Atlas
    int anzahlZeichen;
    byte[] masken = Array.Empty<byte>();                // [(zeichen*2 + fett) * zp*zp + y*zp + x]
    int zp;                                              // Zellgröße in Pixeln
    double atlasSkala = -1;
    bool kaputt;

    // Raster
    WriteableBitmap bild;
    int spalten, zeilen, pw, ph;
    double sx = 1, sy = 1;
    short[] glyph = Array.Empty<short>();
    double[] geschrieben = Array.Empty<double>();
    float[] dauer = Array.Empty<float>();
    int[] gezeigt = Array.Empty<int>();                 // was gerade in der Bitmap steht: 0 = leer, sonst zeichen*(Stufen+1)+stufe+1
    int[] dirtyMin = Array.Empty<int>(), dirtyMax = Array.Empty<int>();
    Kopf[] koepfe = Array.Empty<Kopf>();
    int kopfAnzahl;

    // Ablauf
    double simZeit, letzteZeit;
    TimeSpan letzterFrame;
    bool laeuft, aktiv;
    double staerke = 0.7;
    Window fenster;

    struct Kopf
    {
        public int Spalte, Zeile;
        public double Tempo, Abklingen, Naechster;
    }

    public MatrixRegen()
    {
        IsHitTestVisible = false;
        Focusable = false;
        ClipToBounds = true;
        RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);

        Loaded += (_, _) =>
        {
            fenster = Window.GetWindow(this);
            if (fenster != null) fenster.StateChanged += FensterZustand;
            laeuft = true;
            BaueRaster();
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
        SizeChanged += (_, _) => BaueRaster();
    }

    /// <summary>0 … 1: steuert Deckkraft und Dichte (gleiche Formeln wie bisher).</summary>
    public double Staerke
    {
        get => staerke;
        set { staerke = Math.Clamp(value, 0, 1); Opacity = 0.10 + 0.70 * staerke; KoepfeAnpassen(); }
    }

    /// <summary>0,3 … 2,5: Geschwindigkeit. Skaliert die Zeit der ganzen Animation (Schreibtempo, Abklingen,
    /// Zeichenwechsel, Pausen) – so bleiben die Schweife gleich lang und es sieht bei jedem Tempo stimmig aus.</summary>
    public double Tempo
    {
        get => tempo;
        set => tempo = Math.Clamp(value, 0.3, 2.5);
    }
    double tempo = 1.0;

    public void Starten() { laeuft = true; Aktualisieren(); }
    public void Anhalten() { laeuft = false; Aktualisieren(); }

    void FensterZustand(object sender, EventArgs e) => Aktualisieren();

    void Aktualisieren()
    {
        bool soll = laeuft && !kaputt && IsVisible && (fenster == null || fenster.WindowState != WindowState.Minimized);
        if (soll == aktiv) return;
        aktiv = soll;
        if (soll)
        {
            letzteZeit = uhr.Elapsed.TotalSeconds;   // nach einer Pause nicht springen
            CompositionTarget.Rendering += BeiRendern;
        }
        else CompositionTarget.Rendering -= BeiRendern;
    }

    protected override void OnDpiChanged(DpiScale alt, DpiScale neu)
    {
        base.OnDpiChanged(alt, neu);
        BaueRaster();
    }

    // ===================================================== Zeichen & Atlas

    static string[] BaueZeichen()
    {
        var l = new List<string>();
        for (int c = 0xFF66; c <= 0xFF9D; c++) l.Add(((char)c).ToString());   // Halbbreiten-Katakana
        for (char c = '0'; c <= '9'; c++) l.Add(c.ToString());
        foreach (var t in new[] { "{", "}", ";", "=>", "()", "[]", "+=", "++", "==", "&&", "int", "var", "if", "for", "new", "<T>" }) l.Add(t);
        return l.ToArray();
    }

    /// <summary>Farbe je Stufe × Abdeckung, vormultipliziert (Pbgra32). Stufe 16 = Kopf.</summary>
    static uint[] BaueFarbTabelle()
    {
        var t = new uint[(Stufen + 1) * 256];
        for (int s = 0; s <= Stufen; s++)
        {
            Color c; double a;
            if (s == Stufen) { c = KopfFarbe; a = 1; }
            else
            {
                double f = s / (double)(Stufen - 1);                 // 0 = frisch, 1 = fast weg
                c = Color.FromRgb((byte)Math.Round(Hell.R + (Dunkel.R - Hell.R) * f),
                                  (byte)Math.Round(Hell.G + (Dunkel.G - Hell.G) * f),
                                  (byte)Math.Round(Hell.B + (Dunkel.B - Hell.B) * f));
                a = 1 - Math.Pow(f, 1.6);
            }
            for (int d = 0; d < 256; d++)
            {
                double A = a * d / 255.0;
                uint aa = (uint)Math.Round(A * 255), rr = (uint)Math.Round(c.R * A), gg = (uint)Math.Round(c.G * A), bb = (uint)Math.Round(c.B * A);
                t[s * 256 + d] = (aa << 24) | (rr << 16) | (gg << 8) | bb;
            }
        }
        return t;
    }

    static bool Deckt(Typeface tf, string text)
    {
        try
        {
            if (!tf.TryGetGlyphTypeface(out var gt)) return false;
            foreach (char ch in text)
                if (!gt.CharacterToGlyphMap.ContainsKey(ch)) return false;
            return true;
        }
        catch { return false; }
    }

    /// <summary>
    /// Rendert alle Zeichen (normal + fett) einmalig in einen Atlas und legt die Abdeckung je Pixel ab.
    /// Zeichen, die keine installierte Schrift darstellen kann, werden weggelassen – es wird nie ein
    /// leeres Kästchen gezeichnet und nichts kann hier eine Ausnahme nach außen werfen.
    /// </summary>
    void BaueAtlas(double skala)
    {
        zp = Math.Max(8, (int)Math.Round(ZelleDip * skala));
        atlasSkala = skala;

        var familien = new List<FontFamily>();
        try { familien.Add(new FontFamily(new Uri("pack://application:,,,/"), "./Ressourcen/Schrift/#JetBrains Mono NL")); } catch { }
        foreach (var name in Schriften)
            if (!string.IsNullOrWhiteSpace(name))
                try { familien.Add(new FontFamily(name)); } catch { }

        var texte = new List<string>();
        var schriften = new List<FontFamily>();
        foreach (var text in AlleZeichen)
        {
            foreach (var fam in familien)
            {
                if (Deckt(new Typeface(fam, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal), text))
                {
                    texte.Add(text); schriften.Add(fam); break;
                }
            }
        }
        if (texte.Count == 0) throw new InvalidOperationException("Keine Schrift für den Matrix-Regen gefunden.");

        int kacheln = texte.Count * 2;
        int atlasZeilen = (kacheln + AtlasSpalten - 1) / AtlasSpalten;
        int aw = AtlasSpalten * zp, ah = atlasZeilen * zp;

        var dv = new DrawingVisual();
        TextOptions.SetTextRenderingMode(dv, TextRenderingMode.Grayscale);
        using (var dc = dv.RenderOpen())
        {
            for (int z = 0; z < texte.Count; z++)
            {
                string text = texte[z];
                double groesse = (text.Length == 1 ? 15 : text.Length == 2 ? 11 : 9) * skala;
                for (int fett = 0; fett < 2; fett++)
                {
                    int k = z * 2 + fett;
                    double x0 = (k % AtlasSpalten) * zp, y0 = (k / AtlasSpalten) * zp;
                    var tf = new Typeface(schriften[z], FontStyles.Normal, fett == 1 ? FontWeights.Bold : FontWeights.Normal, FontStretches.Normal);
                    var ft = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, tf, groesse, Brushes.White, 1.0);
                    dc.PushClip(new RectangleGeometry(new Rect(x0, y0, zp, zp)));
                    dc.DrawText(ft, new Point(x0 + (zp - ft.Width) / 2, y0 + (zp - ft.Height) / 2));
                    dc.Pop();
                }
            }
        }
        var rtb = new RenderTargetBitmap(aw, ah, 96, 96, PixelFormats.Pbgra32);
        rtb.Render(dv);
        var px = new uint[aw * ah];
        rtb.CopyPixels(px, aw * 4, 0);

        var m = new byte[kacheln * zp * zp];
        for (int k = 0; k < kacheln; k++)
        {
            int x0 = (k % AtlasSpalten) * zp, y0 = (k / AtlasSpalten) * zp, ziel = k * zp * zp;
            for (int y = 0; y < zp; y++)
                for (int x = 0; x < zp; x++)
                    m[ziel + y * zp + x] = (byte)(px[(y0 + y) * aw + x0 + x] >> 24);
        }
        masken = m;
        anzahlZeichen = texte.Count;
    }

    // ===================================================== Raster

    void BaueRaster()
    {
        if (!IsLoaded || ActualWidth <= 0 || ActualHeight <= 0) return;
        var dpi = VisualTreeHelper.GetDpi(this);
        sx = dpi.DpiScaleX; sy = dpi.DpiScaleY;

        if (Math.Abs(atlasSkala - sx) > 0.001 || masken.Length == 0)
        {
            try { BaueAtlas(sx); }
            catch (Exception ex)
            {
                Debug.WriteLine("Matrix-Regen deaktiviert: " + ex.Message);
                kaputt = true; bild = null; Aktualisieren(); InvalidateVisual();
                return;
            }
        }

        pw = Math.Max(1, (int)Math.Ceiling(ActualWidth * sx));
        ph = Math.Max(1, (int)Math.Ceiling(ActualHeight * sy));
        bild = new WriteableBitmap(pw, ph, 96 * sx, 96 * sy, PixelFormats.Pbgra32, null);
        spalten = (pw + zp - 1) / zp;
        zeilen = (ph + zp - 1) / zp;
        int n = spalten * zeilen;
        glyph = new short[n];
        geschrieben = new double[n];
        Array.Fill(geschrieben, -1e9);
        dauer = new float[n];
        Array.Fill(dauer, 1f);
        gezeigt = new int[n];
        dirtyMin = new int[spalten];
        dirtyMax = new int[spalten];
        koepfe = new Kopf[Math.Max(4, spalten)];
        kopfAnzahl = 0;
        simZeit = 0;
        KoepfeAnpassen();
        for (int i = 0; i < (int)(Vorlauf * 60); i++) Simulieren(1.0 / 60);
        letzteZeit = uhr.Elapsed.TotalSeconds;
        Zeichnen();
        InvalidateVisual();
    }

    void KoepfeAnpassen()
    {
        if (koepfe.Length == 0) return;
        int ziel = Math.Min(koepfe.Length, Math.Max(4, (int)(spalten * (0.15 + 0.4 * staerke))));
        for (int i = kopfAnzahl; i < ziel; i++) NeuerKopf(ref koepfe[i], simZeit, erstesMal: true);
        kopfAnzahl = ziel;
    }

    void NeuerKopf(ref Kopf k, double jetzt, bool erstesMal)
    {
        k.Spalte = zufall.Next(spalten);
        k.Zeile = erstesMal ? (int)(zufall.NextDouble() * zeilen * 0.8) - 1
                            : (zufall.NextDouble() < 0.6 ? -1 : (int)(zufall.NextDouble() * zeilen * 0.5) - 1);
        k.Tempo = TempoMin + zufall.NextDouble() * (TempoMax - TempoMin);
        k.Abklingen = AbklingenMin + zufall.NextDouble() * (AbklingenMax - AbklingenMin);
        k.Naechster = jetzt + (erstesMal ? zufall.NextDouble() * 2 : PauseMin + zufall.NextDouble() * (PauseMax - PauseMin));
    }

    void Simulieren(double dt)
    {
        simZeit += dt;
        for (int i = 0; i < kopfAnzahl; i++)
        {
            ref var k = ref koepfe[i];
            while (simZeit >= k.Naechster)
            {
                k.Zeile++;
                if (k.Zeile >= zeilen) { NeuerKopf(ref k, k.Naechster, erstesMal: false); break; }
                if (k.Zeile >= 0)
                {
                    int z = k.Zeile * spalten + k.Spalte;
                    glyph[z] = (short)zufall.Next(anzahlZeichen);
                    geschrieben[z] = k.Naechster;
                    dauer[z] = (float)k.Abklingen;
                }
                k.Naechster += 1 / k.Tempo;
            }
        }
        double p = Mutation * dt;
        for (int z = 0; z < glyph.Length; z++)
            if (simZeit - geschrieben[z] < dauer[z] && zufall.NextDouble() < p)
                glyph[z] = (short)zufall.Next(anzahlZeichen);
    }

    // ===================================================== Zeichnen

    unsafe void Zeichnen()
    {
        var b = bild;
        if (b == null) return;
        for (int c = 0; c < spalten; c++) { dirtyMin[c] = int.MaxValue; dirtyMax[c] = -1; }

        b.Lock();
        try
        {
            uint* basis = (uint*)b.BackBuffer;
            int stride = b.BackBufferStride / 4;
            int zz = zp * zp;
            fixed (byte* maskeBasis = masken)
            fixed (uint* tabelle = farbTabelle)
            {
                for (int z = 0; z < glyph.Length; z++)
                {
                    double alter = simZeit - geschrieben[z];
                    int stufe = -1;
                    if (alter < dauer[z])
                    {
                        if (alter < Nachgluehen) stufe = Stufen;
                        else stufe = Math.Min(Stufen - 1, (int)((alter - Nachgluehen) / (dauer[z] - Nachgluehen) * Stufen));
                    }
                    int schluessel = stufe < 0 ? 0 : glyph[z] * (Stufen + 1) + stufe + 1;
                    if (schluessel == gezeigt[z]) continue;
                    gezeigt[z] = schluessel;

                    int spalte = z % spalten, zeile = z / spalten;
                    int x0 = spalte * zp, y0 = zeile * zp;
                    int w = Math.Min(zp, pw - x0), h = Math.Min(zp, ph - y0);
                    if (w <= 0 || h <= 0) continue;

                    if (stufe < 0)
                    {
                        for (int y = 0; y < h; y++)
                            new Span<uint>(basis + (y0 + y) * stride + x0, w).Clear();
                    }
                    else
                    {
                        byte* maske = maskeBasis + (glyph[z] * 2 + (stufe == Stufen ? 1 : 0)) * zz;
                        uint* farbe = tabelle + stufe * 256;
                        for (int y = 0; y < h; y++)
                        {
                            uint* ziel = basis + (y0 + y) * stride + x0;
                            byte* quelle = maske + y * zp;
                            for (int x = 0; x < w; x++) ziel[x] = farbe[quelle[x]];
                        }
                    }
                    if (zeile < dirtyMin[spalte]) dirtyMin[spalte] = zeile;
                    if (zeile > dirtyMax[spalte]) dirtyMax[spalte] = zeile;
                }
            }
            for (int c = 0; c < spalten; c++)
            {
                if (dirtyMax[c] < 0) continue;
                int x0 = c * zp, y0 = dirtyMin[c] * zp;
                int w = Math.Min(zp, pw - x0), h = Math.Min((dirtyMax[c] - dirtyMin[c] + 1) * zp, ph - y0);
                if (w > 0 && h > 0) b.AddDirtyRect(new Int32Rect(x0, y0, w, h));
            }
        }
        finally { b.Unlock(); }
    }

    void BeiRendern(object sender, EventArgs e)
    {
        // Rendering kann pro Bild mehrfach feuern – nur einmal pro tatsächlichem Frame rechnen.
        if (e is RenderingEventArgs re)
        {
            if (re.RenderingTime == letzterFrame) return;
            letzterFrame = re.RenderingTime;
        }
        if (bild == null) return;
        double jetzt = uhr.Elapsed.TotalSeconds;
        double dt = Math.Min(jetzt - letzteZeit, 0.1);
        letzteZeit = jetzt;
        if (dt <= 0) return;
        Simulieren(dt * tempo);
        Zeichnen();
    }

    protected override void OnRender(DrawingContext dc)
    {
        if (bild != null) dc.DrawImage(bild, new Rect(0, 0, pw / sx, ph / sy));
    }
}
