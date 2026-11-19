using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace CSharpLernen.Engine;

/// <summary>Ergebnis des Kompilierens (Übersetzen) des Nutzer-Codes.</summary>
public sealed class Kompilat
{
    public bool Ok { get; init; }
    public IReadOnlyList<CompilerMeldung> Meldungen { get; init; } = Array.Empty<CompilerMeldung>();
    internal byte[]? Pe { get; init; }
    internal byte[]? Pdb { get; init; }
}

/// <summary>
/// Übersetzt C#-Code mit Roslyn im Arbeitsspeicher und führt ihn aus – wie eine Konsolen-App in Visual Studio
/// (Top-Level-Anweisungen und implizite usings werden unterstützt).
/// </summary>
public static class CodeAusfuehrer
{
    public const string Dateiname = "Program.cs";

    internal static readonly CSharpParseOptions ParseOptionen = new(LanguageVersion.CSharp12, DocumentationMode.None);

    /// <summary>Wie die impliziten usings einer .NET-8-Konsolen-App in Visual Studio.</summary>
    internal const string GlobaleUsings = """
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Net.Http;
global using System.Threading;
global using System.Threading.Tasks;
""";

    internal static readonly Lazy<List<MetadataReference>> Referenzen =
        new(() => Basic.Reference.Assemblies.Net80.References.All.Cast<MetadataReference>().ToList());

    static readonly SyntaxTree UsingsBaum = CSharpSyntaxTree.ParseText(GlobaleUsings, ParseOptionen, "GlobalUsings.cs", Encoding.UTF8);
    static readonly SyntaxTree WaechterBaum = CSharpSyntaxTree.ParseText(WaechterEinbau.Quelltext, ParseOptionen, "Waechter.cs", Encoding.UTF8);

    public static readonly CultureInfo Deutsch = CultureInfo.GetCultureInfo("de-DE");

    /// <summary>Warnungen, die Anfängern wirklich helfen (alle anderen werden ausgeblendet).</summary>
    static readonly Dictionary<string, string> NuetzlicheWarnungen = new()
    {
        ["CS0642"] = "Achtung: Direkt hinter if/while/for steht ein Semikolon ; – dadurch gehört der folgende Block NICHT mehr dazu. Lösche das ; hinter der Klammer.",
        ["CS0665"] = "Hast du = statt == gemeint? = weist zu, == vergleicht.",
        ["CS1717"] = "Du weist eine Variable sich selbst zu – das bewirkt nichts.",
        ["CS0162"] = "Dieser Code wird nie erreicht (z. B. weil davor return oder break steht).",
        ["CS0168"] = "Diese Variable wird deklariert, aber nie benutzt.",
        ["CS0219"] = "Diese Variable bekommt einen Wert, wird aber nie benutzt.",
    };

    static readonly SemaphoreSlim Sperre = new(1, 1);

    internal static readonly CSharpCompilationOptions Optionen = new(
        OutputKind.ConsoleApplication,
        optimizationLevel: OptimizationLevel.Debug,
        nullableContextOptions: NullableContextOptions.Disable,
        allowUnsafe: false,
        concurrentBuild: true);

    /// <summary>Lädt Roslyn einmal vorab, damit der erste Klick auf „Ausführen“ schneller geht.</summary>
    public static void Vorwaermen()
    {
        try { Kompilieren("Console.WriteLine(\"Hallo\");"); } catch { /* egal */ }
    }

    static CompilerMeldung ZuMeldung(Diagnostic d, bool warnung)
    {
        var span = d.Location.GetLineSpan();
        int zeile = d.Location.IsInSource ? span.StartLinePosition.Line + 1 : 0;
        int spalte = d.Location.IsInSource ? span.StartLinePosition.Character + 1 : 0;
        string tipp = warnung ? NuetzlicheWarnungen.GetValueOrDefault(d.Id, "") : Hinweise.FuerCompilerFehler(d.Id) ?? "";
        return new CompilerMeldung(zeile, spalte, d.Id, d.GetMessage(Deutsch), tipp.Length > 0 ? tipp : null, warnung);
    }

    public static string RoslynVersion =>
        typeof(CSharpCompilation).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion?.Split('+')[0] ?? "?";

    public static Kompilat Kompilieren(string code)
    {
        var baum = CSharpSyntaxTree.ParseText(code ?? "", ParseOptionen, Dateiname, Encoding.UTF8);
        var compilation = CSharpCompilation.Create("Programm_" + Guid.NewGuid().ToString("N"),
            new[] { baum, UsingsBaum, WaechterBaum }, Referenzen.Value, Optionen);

        var diagnosen = compilation.GetDiagnostics();
        var fehler = diagnosen.Where(d => d.Severity == DiagnosticSeverity.Error)
            .OrderBy(d => d.Location.SourceSpan.Start)
            .Select(d => ZuMeldung(d, false))
            .DistinctBy(m => (m.Zeile, m.Spalte, m.Code))
            .Take(30)
            .ToList();
        var warnungen = diagnosen
            .Where(d => d.Severity == DiagnosticSeverity.Warning && NuetzlicheWarnungen.ContainsKey(d.Id) && d.Location.SourceTree == baum)
            .OrderBy(d => d.Location.SourceSpan.Start)
            .Select(d => ZuMeldung(d, true))
            .ToList();

        if (fehler.Count > 0)
            return new Kompilat { Ok = false, Meldungen = fehler.Concat(warnungen).ToList() };

        // Wächter einbauen (keine neuen Zeilen -> Zeilennummern bleiben gleich)
        var neueWurzel = new WaechterEinbau().Visit(baum.GetRoot());
        var neuerBaum = CSharpSyntaxTree.Create((Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode)neueWurzel, ParseOptionen, Dateiname, Encoding.UTF8);
        compilation = compilation.ReplaceSyntaxTree(baum, neuerBaum);

        using var pe = new MemoryStream();
        using var pdb = new MemoryStream();
        EmitResult emit = compilation.Emit(pe, pdb, options: new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb));
        if (!emit.Success)
        {
            var emitFehler = emit.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)
                .Select(d => ZuMeldung(d, false)).ToList();
            return new Kompilat { Ok = false, Meldungen = emitFehler };
        }
        return new Kompilat { Ok = true, Meldungen = warnungen, Pe = pe.ToArray(), Pdb = pdb.ToArray() };
    }

    public static Task<Laufergebnis> AusfuehrenAsync(string code, Laufoptionen? optionen = null)
    {
        optionen ??= new Laufoptionen();
        return Task.Run(async () =>
        {
            var gesamt = Stopwatch.StartNew();
            Kompilat k;
            try { k = Kompilieren(code); }
            catch (Exception ex)
            {
                return new Laufergebnis { Ausgang = Ausgang.Kompilierfehler, Meldungen = new[] { new CompilerMeldung(0, 0, "INTERN", "Interner Fehler beim Übersetzen: " + ex.Message, null) } };
            }
            if (!k.Ok)
                return new Laufergebnis { Ausgang = Ausgang.Kompilierfehler, Meldungen = k.Meldungen, Dauer = gesamt.Elapsed };

            try { await Sperre.WaitAsync(optionen.Abbruch).ConfigureAwait(false); }
            catch (OperationCanceledException) { return new Laufergebnis { Ausgang = Ausgang.Abgebrochen, Meldungen = k.Meldungen }; }
            try
            {
                optionen.BeiStart?.Invoke();
                return Ausfuehren(k, optionen);
            }
            finally { Sperre.Release(); }
        });
    }

    static Laufergebnis Ausfuehren(Kompilat k, Laufoptionen o)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(o.Abbruch);
        var zeit = new Rechenzeit();
        var schreiber = new AusgabeSchreiber(o.BeiAusgabe, o.BeiLoeschen, o.MaxAusgabeZeichen, () => { try { cts.Cancel(); } catch (ObjectDisposedException) { } });
        var leser = new EingabeLeser(o.Eingabe, o.InteraktiveEingabe, cts.Token, zeit);

        var alc = new AssemblyLoadContext("Nutzerprogramm", isCollectible: true);
        Assembly asm;
        using (var pe = new MemoryStream(k.Pe!))
        using (var pdb = new MemoryStream(k.Pdb!))
            asm = alc.LoadFromStream(pe, pdb);
        asm.GetType("__Waechter")!.GetField("Token")!.SetValue(null, cts.Token);
        MethodInfo einstieg = asm.EntryPoint ?? throw new InvalidOperationException("Kein Einstiegspunkt (Main) gefunden.");

        Exception? fehler = null;
        int? fehlerZeile = null;
        var thread = new Thread(() =>
        {
            Thread.CurrentThread.CurrentCulture = Deutsch;
            Thread.CurrentThread.CurrentUICulture = Deutsch;
            try
            {
                object?[]? args = einstieg.GetParameters().Length == 0 ? null : new object?[] { Array.Empty<string>() };
                object? r = einstieg.Invoke(null, args);
                if (r is Task t) t.GetAwaiter().GetResult();
            }
            catch (TargetInvocationException tie) { fehler = tie.InnerException ?? tie; fehlerZeile = FindeZeile(fehler); }
            catch (Exception ex) { fehler = ex; fehlerZeile = FindeZeile(ex); }
            try { Console.Out.Flush(); } catch { }
        }, 2 * 1024 * 1024)
        { IsBackground = true, Name = "Nutzerprogramm" };

        var altOut = Console.Out;
        var altIn = Console.In;
        Console.SetOut(schreiber);
        Console.SetIn(leser);

        bool zeitUeberschritten = false;
        bool verwaist = false;
        zeit.Start();
        var dauer = Stopwatch.StartNew();
        try
        {
            thread.Start();
            while (!thread.Join(20))
            {
                if (!zeitUeberschritten && zeit.Vergangen > o.Zeitlimit)
                {
                    zeitUeberschritten = true;
                    cts.Cancel();
                }
                if (cts.IsCancellationRequested)
                {
                    if (!thread.Join(2000)) verwaist = true;
                    break;
                }
            }
        }
        finally
        {
            schreiber.Schliessen();
            Console.SetOut(altOut);
            Console.SetIn(altIn);
            if (!verwaist) alc.Unload();
        }

        while (fehler is AggregateException { InnerExceptions.Count: 1 } ag) fehler = ag.InnerException;
        if (fehler?.GetType().Name == "__ProgrammEnde") fehler = null;

        string ausgabe = schreiber.Text;
        if (schreiber.LimitErreicht)
            return new Laufergebnis
            {
                Ausgang = Ausgang.ZuVielAusgabe, Ausgabe = ausgabe, Meldungen = k.Meldungen, Dauer = dauer.Elapsed,
                FehlerText = $"Dein Programm hat sehr viel ausgegeben (über {o.MaxAusgabeZeichen:N0} Zeichen) und wurde gestoppt.",
                Tipp = "Vielleicht eine Endlosschleife? Prüfe, ob sich die Schleifenbedingung irgendwann ändert (z. B. i++ vergessen?).",
            };
        if (zeitUeberschritten)
            return new Laufergebnis
            {
                Ausgang = Ausgang.Zeitueberschreitung, Ausgabe = ausgabe, Meldungen = k.Meldungen, Dauer = dauer.Elapsed,
                FehlerText = $"Dein Programm lief länger als {o.Zeitlimit.TotalSeconds:0} Sekunden und wurde gestoppt.",
                Tipp = "Wahrscheinlich eine Endlosschleife: Ändert sich die Bedingung deiner Schleife irgendwann? Hast du z. B. i++ vergessen?",
            };
        if (o.Abbruch.IsCancellationRequested)
            return new Laufergebnis { Ausgang = Ausgang.Abgebrochen, Ausgabe = ausgabe, Meldungen = k.Meldungen, Dauer = dauer.Elapsed, FehlerText = "Programm gestoppt." };

        if (fehler != null)
        {
            var (text, tipp) = Hinweise.FuerLaufzeitFehler(fehler, leser.Aufgebraucht);
            return new Laufergebnis
            {
                Ausgang = Ausgang.Laufzeitfehler, Ausgabe = ausgabe, Meldungen = k.Meldungen, Dauer = dauer.Elapsed,
                FehlerText = text, Tipp = tipp, FehlerTyp = fehler.GetType().Name, FehlerZeile = fehlerZeile,
                EingabeAufgebraucht = leser.Aufgebraucht,
            };
        }
        return new Laufergebnis { Ausgang = Ausgang.Erfolg, Ausgabe = ausgabe, Meldungen = k.Meldungen, Dauer = dauer.Elapsed, EingabeAufgebraucht = leser.Aufgebraucht };
    }

    static int? FindeZeile(Exception ex)
    {
        try
        {
            var frames = new StackTrace(ex, true).GetFrames();
            foreach (var f in frames)
            {
                string? datei = f.GetFileName();
                if (datei != null && datei.EndsWith(Dateiname, StringComparison.OrdinalIgnoreCase) && f.GetFileLineNumber() > 0)
                    return f.GetFileLineNumber();
            }
        }
        catch { }
        return null;
    }
}
