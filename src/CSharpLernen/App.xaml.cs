using System.Windows;
using CSharpLernen.Engine;
using System.Windows.Media;
using System.Windows.Threading;

namespace CSharpLernen;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        DispatcherUnhandledException += BeiUnbehandeltemFehler;
        AppDomain.CurrentDomain.UnhandledException += (_, a) =>
        {
            if (a.ExceptionObject is Exception ex) Protokollieren(ex, "AppDomain.UnhandledException");
        };
        TaskScheduler.UnobservedTaskException += (_, a) =>
        {
            Protokollieren(a.Exception, "TaskScheduler.UnobservedTaskException");
            a.SetObserved();
        };
        base.OnStartup(e);
    }

    static readonly (string Key, double Normal, double Minimum)[] Flaechen =
    {
        ("Panel", 0.85, 0.75), ("Karte", 0.85, 0.75), ("KarteHell", 0.90, 0.80), ("KarteHover", 0.90, 0.82),
    };
    static readonly Dictionary<string, Color> Grundfarben = new();

    /// <summary>
    /// Passt die Deckkraft der Panels/Karten an die Matrix-Intensität an: bis 50 % unverändert,
    /// darüber werden sie bis 75 % (Panels) durchsichtiger, damit der Regen sichtbar bleibt.
    /// Editor und Ausgabe sind davon nicht betroffen.
    /// </summary>
    public static void SetzeFlaechenDeckkraft(double staerke, bool regenAn)
    {
        var res = Current.Resources;
        double t = regenAn ? Math.Clamp((staerke - 0.5) / 0.5, 0, 1) : 0;
        foreach (var (key, normal, minimum) in Flaechen)
        {
            if (!Grundfarben.TryGetValue(key, out var c))
                Grundfarben[key] = c = ((SolidColorBrush)res[key]).Color;
            double a = normal + (minimum - normal) * t;
            var b = new SolidColorBrush(Color.FromArgb((byte)Math.Round(a * 255), c.R, c.G, c.B));
            b.Freeze();
            res[key] = b;
        }
    }

    /// <summary>Fehlerprotokoll mit Stacktrace: %AppData%\CSharpLernen\fehler.log</summary>
    public static string FehlerDatei => System.IO.Path.Combine(Fortschritt.Ordner, "fehler.log");

    static void Protokollieren(Exception ex, string quelle) =>
        FehlerProtokoll.Schreiben(FehlerDatei, ex, quelle, typeof(App).Assembly.GetName().Version?.ToString(3));

    void BeiUnbehandeltemFehler(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        e.Handled = true;
        Protokollieren(e.Exception, "Oberfläche");
        try
        {
            _ = DialogService.ShowAsync("Ups, ein Fehler", "In der App ist ein Fehler aufgetreten:\n" + e.Exception.Message,
                DialogKind.Warnung, "Weitermachen",
                hint: "Dein Fortschritt ist gespeichert. Du kannst einfach weitermachen. Details: %AppData%\\CSharpLernen\\fehler.log");
        }
        catch { }
    }
}
