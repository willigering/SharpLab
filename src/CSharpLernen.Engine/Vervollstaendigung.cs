using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Tags;
using Microsoft.CodeAnalysis.Text;

namespace CSharpLernen.Engine;

/// <summary>Art eines Vorschlags (für Symbol und deutschen Kurztext in der Liste).</summary>
public enum VorschlagArt
{
    Methode, Erweiterungsmethode, Eigenschaft, Feld, Ereignis, Konstante, Klasse, Struktur, Schnittstelle,
    Aufzaehlung, Aufzaehlungswert, Delegat, Namensraum, Variable, Parameter, Typparameter, Schluesselwort, Sonstiges,
}

/// <summary>Ein einzelner Vorschlag der Autovervollständigung.</summary>
public sealed class Vorschlag
{
    internal Vorschlag(CompletionItem item, VervollstaendigungsListe liste)
    {
        Item = item;
        Liste = liste;
        Anzeige = item.DisplayTextPrefix + item.DisplayText + item.DisplayTextSuffix;
        FilterText = item.FilterText;
        Art = ArtAus(item.Tags);
        Einfuegen = item.Properties.TryGetValue("InsertionText", out var t) && !string.IsNullOrEmpty(t) ? t : item.DisplayText;
    }

    internal CompletionItem Item { get; }
    internal VervollstaendigungsListe Liste { get; }

    /// <summary>Text in der Liste, z. B. "WriteLine" oder "List&lt;&gt;".</summary>
    public string Anzeige { get; }
    /// <summary>Text, nach dem beim Tippen gefiltert wird.</summary>
    public string FilterText { get; }
    /// <summary>Text, der beim Übernehmen eingefügt wird (ohne Klammern).</summary>
    public string Einfuegen { get; }
    public VorschlagArt Art { get; }
    /// <summary>Roslyn empfiehlt diesen Eintrag zur Vorauswahl (z. B. passender Typ nach new).</summary>
    public bool Vorausgewaehlt => Item.Rules.MatchPriority > MatchPriority.Default;

    public string ArtText => Art switch
    {
        VorschlagArt.Methode => "Methode",
        VorschlagArt.Erweiterungsmethode => "Methode",
        VorschlagArt.Eigenschaft => "Eigenschaft",
        VorschlagArt.Feld => "Feld",
        VorschlagArt.Ereignis => "Ereignis",
        VorschlagArt.Konstante => "Konstante",
        VorschlagArt.Klasse => "Klasse",
        VorschlagArt.Struktur => "Struktur",
        VorschlagArt.Schnittstelle => "Schnittstelle",
        VorschlagArt.Aufzaehlung => "Enum",
        VorschlagArt.Aufzaehlungswert => "Enum-Wert",
        VorschlagArt.Delegat => "Delegat",
        VorschlagArt.Namensraum => "Namensraum",
        VorschlagArt.Variable => "Variable",
        VorschlagArt.Parameter => "Parameter",
        VorschlagArt.Typparameter => "Typparameter",
        VorschlagArt.Schluesselwort => "Schlüsselwort",
        _ => "",
    };

    /// <summary>Signatur/Beschreibung (Quickinfo), z. B. "void Console.WriteLine(string? value) (+ 17 Überladungen)".</summary>
    public Task<string> BeschreibungAsync(CancellationToken abbruch = default) => Liste.BeschreibungAsync(this, abbruch);

    static VorschlagArt ArtAus(ImmutableArray<string> tags)
    {
        foreach (var t in tags)
        {
            switch (t)
            {
                case WellKnownTags.ExtensionMethod: return VorschlagArt.Erweiterungsmethode;
                case WellKnownTags.Method: return VorschlagArt.Methode;
                case WellKnownTags.Property: return VorschlagArt.Eigenschaft;
                case WellKnownTags.Field: return VorschlagArt.Feld;
                case WellKnownTags.Event: return VorschlagArt.Ereignis;
                case WellKnownTags.Constant: return VorschlagArt.Konstante;
                case WellKnownTags.Class: return VorschlagArt.Klasse;
                case WellKnownTags.Structure: return VorschlagArt.Struktur;
                case WellKnownTags.Interface: return VorschlagArt.Schnittstelle;
                case WellKnownTags.Enum: return VorschlagArt.Aufzaehlung;
                case WellKnownTags.EnumMember: return VorschlagArt.Aufzaehlungswert;
                case WellKnownTags.Delegate: return VorschlagArt.Delegat;
                case WellKnownTags.Namespace: return VorschlagArt.Namensraum;
                case WellKnownTags.Local: case WellKnownTags.RangeVariable: return VorschlagArt.Variable;
                case WellKnownTags.Parameter: return VorschlagArt.Parameter;
                case WellKnownTags.TypeParameter: return VorschlagArt.Typparameter;
                case WellKnownTags.Keyword: return VorschlagArt.Schluesselwort;
            }
        }
        return VorschlagArt.Sonstiges;
    }

    public override string ToString() => Anzeige;
}

/// <summary>Ergebnis einer Anfrage: Vorschläge und der Textbereich, den sie ersetzen.</summary>
public sealed class VervollstaendigungsListe
{
    readonly Document dokument;
    readonly CompletionService dienst;

    internal VervollstaendigungsListe(Document dokument, CompletionService dienst, CompletionList liste)
    {
        this.dokument = dokument;
        this.dienst = dienst;
        Start = liste.Span.Start;
        Laenge = liste.Span.Length;
        var l = new List<Vorschlag>(liste.ItemsList.Count);
        foreach (var item in liste.ItemsList) l.Add(new Vorschlag(item, this));
        Vorschlaege = l;
        Vorschlagsmodus = liste.SuggestionModeItem != null;
    }

    /// <summary>Beginn des Wortes, das ersetzt wird (z. B. direkt hinter dem Punkt).</summary>
    public int Start { get; }
    public int Laenge { get; }
    public IReadOnlyList<Vorschlag> Vorschlaege { get; }
    /// <summary>True, wenn hier eher etwas Neues getippt wird (z. B. ein neuer Variablenname) – dann nicht automatisch übernehmen.</summary>
    public bool Vorschlagsmodus { get; }

    internal async Task<string> BeschreibungAsync(Vorschlag v, CancellationToken abbruch)
    {
        try
        {
            var d = await dienst.GetDescriptionAsync(dokument, v.Item, abbruch).ConfigureAwait(false);
            return d?.Text ?? "";
        }
        catch (OperationCanceledException) { throw; }
        catch { return ""; }
    }
}

/// <summary>
/// Autovervollständigung wie IntelliSense in Visual Studio – mit Roslyns echtem CompletionService und
/// denselben Referenzen und globalen usings wie der Compiler der App. Thread-sicher: jede Anfrage
/// arbeitet auf einer eigenen, unveränderlichen Kopie der Lösung.
/// </summary>
public sealed class Vervollstaendigung : IDisposable
{
    static readonly Lazy<Vervollstaendigung> standard = new(() => new Vervollstaendigung(), LazyThreadSafetyMode.ExecutionAndPublication);
    public static Vervollstaendigung Standard => standard.Value;

    readonly AdhocWorkspace arbeitsbereich;
    readonly DocumentId dokumentId;

    public Vervollstaendigung()
    {
        arbeitsbereich = new AdhocWorkspace(MefHostServices.DefaultHost);
        var projektId = ProjectId.CreateNewId();
        var projekt = ProjectInfo.Create(projektId, VersionStamp.Create(), "Programm", "Programm", LanguageNames.CSharp,
            compilationOptions: CodeAusfuehrer.Optionen,
            parseOptions: CodeAusfuehrer.ParseOptionen,
            metadataReferences: CodeAusfuehrer.Referenzen.Value);
        var loesung = arbeitsbereich.CurrentSolution.AddProject(projekt);
        loesung = loesung.AddDocument(DocumentId.CreateNewId(projektId), "GlobalUsings.cs", SourceText.From(CodeAusfuehrer.GlobaleUsings));
        dokumentId = DocumentId.CreateNewId(projektId);
        loesung = loesung.AddDocument(dokumentId, CodeAusfuehrer.Dateiname, SourceText.From(""));
        if (!arbeitsbereich.TryApplyChanges(loesung)) throw new InvalidOperationException("Arbeitsbereich konnte nicht angelegt werden.");
    }

    /// <summary>Lädt Roslyn-Dienste und Referenzen vorab, damit der erste Vorschlag schnell erscheint.</summary>
    public static Task VorwaermenAsync() => Task.Run(async () =>
    {
        try { await Standard.VorschlaegeAsync("Console.", 8, '.').ConfigureAwait(false); } catch { }
    });

    /// <summary>
    /// Vorschläge an Position <paramref name="position"/>. <paramref name="zeichen"/> ist das gerade getippte Zeichen
    /// (z. B. '.') oder null bei Strg+Leertaste. Liefert null, wenn an dieser Stelle nichts vorgeschlagen werden soll
    /// (z. B. in einem String oder Kommentar).
    /// </summary>
    public async Task<VervollstaendigungsListe?> VorschlaegeAsync(string code, int position, char? zeichen = null, CancellationToken abbruch = default)
    {
        var text = SourceText.From(code ?? "");
        position = Math.Clamp(position, 0, text.Length);
        var dokument = arbeitsbereich.CurrentSolution.WithDocumentText(dokumentId, text).GetDocument(dokumentId)!;
        var dienst = CompletionService.GetService(dokument);
        if (dienst == null) return null;

        var ausloeser = zeichen is char c ? CompletionTrigger.CreateInsertionTrigger(c) : CompletionTrigger.Invoke;
        if (zeichen != null && !dienst.ShouldTriggerCompletion(text, position, ausloeser)) return null;

        var liste = await dienst.GetCompletionsAsync(dokument, position, ausloeser, cancellationToken: abbruch).ConfigureAwait(false);
        if (liste == null || liste.ItemsList.Count == 0) return null;
        return new VervollstaendigungsListe(dokument, dienst, liste);
    }

    public void Dispose() => arbeitsbereich.Dispose();
}
