using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Tags;
using Microsoft.CodeAnalysis.Text;

namespace CSharpLernen.Engine;

/// <summary>Art eines Vorschlags (für Symbol und deutschen Kurztext in der Liste).</summary>
public enum VorschlagArt
{
    Methode, Erweiterungsmethode, Eigenschaft, Feld, Ereignis, Konstante, Klasse, Struktur, Schnittstelle,
    Aufzaehlung, Aufzaehlungswert, Delegat, Namensraum, Variable, Parameter, Typparameter, Schluesselwort, Codeausschnitt, Sonstiges,
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

    /// <summary>Vorschlag für einen Codeausschnitt (cw, for, prop …): Übernehmen fügt das Kürzel ein, Tab erweitert es.</summary>
    internal Vorschlag(Codeausschnitt ausschnitt, VervollstaendigungsListe liste)
    {
        Ausschnitt = ausschnitt;
        Liste = liste;
        Anzeige = ausschnitt.Kuerzel;
        FilterText = ausschnitt.Kuerzel;
        Art = VorschlagArt.Codeausschnitt;
        Einfuegen = ausschnitt.Kuerzel;
    }

    internal CompletionItem? Item { get; }
    /// <summary>Gesetzt, wenn dieser Vorschlag ein Codeausschnitt ist.</summary>
    public Codeausschnitt? Ausschnitt { get; }
    /// <summary>Sortierschlüssel wie in Visual Studio (Roslyns SortText, bei Ausschnitten das Kürzel).</summary>
    public string SortText => Item?.SortText ?? FilterText;
    internal VervollstaendigungsListe Liste { get; }

    /// <summary>Text in der Liste, z. B. "WriteLine" oder "List&lt;&gt;".</summary>
    public string Anzeige { get; }
    /// <summary>Text, nach dem beim Tippen gefiltert wird.</summary>
    public string FilterText { get; }
    /// <summary>Text, der beim Übernehmen eingefügt wird (ohne Klammern).</summary>
    public string Einfuegen { get; }
    public VorschlagArt Art { get; }
    /// <summary>Roslyn empfiehlt diesen Eintrag zur Vorauswahl (z. B. passender Typ nach new).</summary>
    public bool Vorausgewaehlt => Item != null && Item.Rules.MatchPriority > MatchPriority.Default;

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
        VorschlagArt.Codeausschnitt => "Ausschnitt",
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
/// <summary>Sichtbare Einträge (alphabetisch) und die Vorauswahl nach dem Filtern.</summary>
public sealed record Filterergebnis(IReadOnlyList<Vorschlag> Sichtbar, Vorschlag? Auswahl);

public sealed class VervollstaendigungsListe
{
    readonly Document dokument;
    readonly CompletionService dienst;

    internal VervollstaendigungsListe(Document dokument, CompletionService dienst, CompletionList liste, bool mitAusschnitten)
    {
        this.dokument = dokument;
        this.dienst = dienst;
        Start = liste.Span.Start;
        Laenge = liste.Span.Length;
        var alle = new List<Vorschlag>(liste.ItemsList.Count + Codeausschnitte.Alle.Count);
        foreach (var item in liste.ItemsList)
        {
            // Roslyns eigene Snippet-Einträge ersetzen wir durch unsere Codeausschnitte (mit Tab-Erweiterung).
            if (item.Tags.Contains("Snippet") || (Codeausschnitte.Finde(item.DisplayText) != null && !item.Tags.Contains(WellKnownTags.Keyword))) continue;
            alle.Add(new Vorschlag(item, this));
        }
        if (mitAusschnitten)
            foreach (var a in Codeausschnitte.Alle) alle.Add(new Vorschlag(a, this));
        // Wie IntelliSense in Visual Studio: eine alphabetische Liste (nach SortText, Groß/klein egal), Ausschnitte einsortiert.
        Vorschlaege = alle.Select((v, i) => (v, i))
            .OrderBy(t => t.v.SortText, StringComparer.OrdinalIgnoreCase)
            .ThenBy(t => t.v.SortText, StringComparer.Ordinal).ThenBy(t => t.i)
            .Select(t => t.v).ToList();
        Vorschlagsmodus = liste.SuggestionModeItem != null;
    }

    /// <summary>Beginn des Wortes, das ersetzt wird (z. B. direkt hinter dem Punkt).</summary>
    public int Start { get; }
    public int Laenge { get; }
    public IReadOnlyList<Vorschlag> Vorschlaege { get; }
    /// <summary>True, wenn hier eher etwas Neues getippt wird (z. B. ein neuer Variablenname) – dann nicht automatisch übernehmen.</summary>
    public bool Vorschlagsmodus { get; }

    /// <summary>
    /// Filtert wie Visual Studio beim Tippen: sichtbar bleibt alles, was als Präfix, CamelCase-Kürzel (SB → StringBuilder,
    /// wl → WriteLine) oder Wortanfang im Inneren (Line → WriteLine) passt – in alphabetischer Reihenfolge.
    /// Die Vorauswahl bestimmt Roslyn (CompletionService.FilterItems: Trefferart, Groß/klein, MatchPriority);
    /// bei Gleichstand gewinnt der zuletzt übernommene, sonst der kürzeste Eintrag.
    /// </summary>
    public Filterergebnis Filtern(string getippt)
    {
        getippt ??= "";
        if (getippt.Length == 0)
        {
            var vor = Vorschlaege.FirstOrDefault(v => v.Vorausgewaehlt)
                      ?? Vervollstaendigung.ZuletztBenutzter(Vorschlaege)
                      ?? Vorschlaege.FirstOrDefault();
            return new Filterergebnis(Vorschlaege, vor);
        }
        // Präfix- und CamelCase-Treffer; Treffer mitten im Namen (Line → WriteLine) nur, wenn es sonst nichts gibt –
        // so bleibt die Liste für Einsteiger übersichtlich.
        var raenge = Vorschlaege.Select(v => (v, r: Trefferart(v.FilterText, getippt))).Where(t => t.r != null).ToList();
        int grenze = raenge.Any(t => t.r <= 4) ? 4 : int.MaxValue;
        var sichtbar = raenge.Where(t => t.r <= grenze).Select(t => t.v).ToList();
        if (sichtbar.Count == 0) return new Filterergebnis(sichtbar, null);

        Vorschlag? auswahl = null;
        var roslyn = sichtbar.Where(v => v.Item != null).ToList();
        if (roslyn.Count > 0)
        {
            ImmutableArray<CompletionItem> beste;
            try { beste = dienst.FilterItems(dokument, roslyn.Select(v => v.Item!).ToImmutableArray(), getippt); }
            catch { beste = ImmutableArray<CompletionItem>.Empty; }
            var besteSet = beste.ToHashSet();
            var kandidaten = roslyn.Where(v => besteSet.Contains(v.Item!)).ToList();
            if (kandidaten.Count > 0)
            {
                // Gleichstand: Trefferqualität (exakt > Präfix mit gleicher Schreibweise > …), dann zuletzt benutzt, dann der kürzeste.
                int bestRang = kandidaten.Min(v => Trefferart(v.FilterText, getippt)!.Value);
                var gleich = kandidaten.Where(v => Trefferart(v.FilterText, getippt) == bestRang).ToList();
                // wie VS: zuletzt benutzt, sonst der kürzeste (Console vor ConsoleColor, stadt vor stackalloc)
                auswahl = Vervollstaendigung.ZuletztBenutzter(gleich)
                          ?? gleich.OrderBy(v => v.FilterText.Length).ThenBy(v => v.SortText, StringComparer.OrdinalIgnoreCase).First();
            }
        }
        // Ein Codeausschnitt, dessen Kürzel genau getippt wurde (cw, prop …), gewinnt – außer ein Schlüsselwort heißt genauso (for, if …).
        var ausschnitt = sichtbar.FirstOrDefault(v => v.Ausschnitt != null && string.Equals(v.FilterText, getippt, StringComparison.OrdinalIgnoreCase));
        if (ausschnitt != null && (auswahl == null || !string.Equals(auswahl.FilterText, getippt, StringComparison.OrdinalIgnoreCase)))
            auswahl = ausschnitt;
        auswahl ??= sichtbar.OrderBy(v => Trefferart(v.FilterText, getippt)).First();
        return new Filterergebnis(sichtbar, auswahl);
    }

    /// <summary>
    /// Trefferart wie beim Visual-Studio-Filter (kleiner = besser), null = passt nicht:
    /// 0 exakt, 1 exakt ohne Groß/klein, 2 Präfix, 3 Präfix ohne Groß/klein, 4 CamelCase ab Wortanfang,
    /// 5 Wortanfang im Inneren (Line → WriteLine), 6 CamelCase ab einem inneren Wortteil.
    /// </summary>
    public static int? Trefferart(string kandidat, string muster)
    {
        if (string.IsNullOrEmpty(muster)) return 0;
        if (string.IsNullOrEmpty(kandidat)) return null;
        if (kandidat == muster) return 0;
        if (string.Equals(kandidat, muster, StringComparison.OrdinalIgnoreCase)) return 1;
        if (kandidat.StartsWith(muster, StringComparison.Ordinal)) return 2;
        if (kandidat.StartsWith(muster, StringComparison.OrdinalIgnoreCase)) return 3;
        var anfaenge = Wortanfaenge(kandidat);
        if (CamelCase(kandidat, muster, 0, 0, anfaenge)) return 4;
        foreach (int a in anfaenge)
            if (a > 0 && string.Compare(kandidat, a, muster, 0, muster.Length, StringComparison.OrdinalIgnoreCase) == 0) return 5;
        foreach (int a in anfaenge)
            if (a > 0 && CamelCase(kandidat, muster, a, 0, anfaenge)) return 6;
        return null;
    }

    /// <summary>Positionen, an denen ein Wortteil beginnt: Anfang, Großbuchstabe nach Kleinbuchstabe/Ziffer, Zeichen nach _ .</summary>
    static List<int> Wortanfaenge(string s)
    {
        var l = new List<int>();
        for (int i = 0; i < s.Length; i++)
        {
            if (!char.IsLetterOrDigit(s[i])) continue;
            if (i == 0 || !char.IsLetterOrDigit(s[i - 1])
                || (char.IsUpper(s[i]) && !char.IsUpper(s[i - 1]))
                || (char.IsUpper(s[i]) && i + 1 < s.Length && char.IsLower(s[i + 1]) && char.IsUpper(s[i - 1])))
                l.Add(i);
        }
        return l;
    }

    /// <summary>Muster ab <paramref name="m"/> passt ab Wortanfang <paramref name="k"/>: Zeichen laufen im Wortteil weiter
    /// oder springen an einen späteren Wortanfang (SB → StringBuilder, SBuil → StringBuilder, wl → WriteLine).</summary>
    static bool CamelCase(string k, string muster, int pos, int m, List<int> anfaenge)
    {
        if (m == muster.Length) return true;
        if (pos >= k.Length || char.ToLowerInvariant(k[pos]) != char.ToLowerInvariant(muster[m])) return false;
        // im selben Wortteil weiter
        if (m + 1 == muster.Length) return true;
        if (pos + 1 < k.Length && !anfaenge.Contains(pos + 1) && CamelCase(k, muster, pos + 1, m + 1, anfaenge)) return true;
        foreach (int a in anfaenge)
            if (a > pos && CamelCase(k, muster, a, m + 1, anfaenge)) return true;
        return false;
    }

    internal async Task<string> BeschreibungAsync(Vorschlag v, CancellationToken abbruch)
    {
        if (v.Ausschnitt is { } a) return $"Codeausschnitt „{a.Kuerzel}“: {a.Beschreibung}\nTab drücken zum Einfügen.";
        if (v.Item == null) return "";
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
        bool ausschnitte = liste.Span.Start <= text.Length && Codeausschnitte.AmAnweisungsanfang(code ?? "", liste.Span.Start)
                           && !Codeausschnitte.InStringOderKommentar(code ?? "", liste.Span.Start);
        return new VervollstaendigungsListe(dokument, dienst, liste, ausschnitte);
    }

    static readonly object mruSperre = new();
    static readonly LinkedList<string> zuletzt = new();

    /// <summary>Merkt sich einen übernommenen Eintrag (wie Visual Studio: bei Gleichstand wird er bevorzugt).</summary>
    public static void Benutzt(Vorschlag v)
    {
        lock (mruSperre)
        {
            zuletzt.Remove(v.FilterText);
            zuletzt.AddFirst(v.FilterText);
            while (zuletzt.Count > 100) zuletzt.RemoveLast();
        }
    }

    /// <summary>Vergisst die zuletzt benutzten Einträge (für Tests).</summary>
    public static void ZuletztVergessen() { lock (mruSperre) zuletzt.Clear(); }

    internal static Vorschlag? ZuletztBenutzter(IReadOnlyList<Vorschlag> kandidaten)
    {
        lock (mruSperre)
        {
            if (zuletzt.Count == 0 || kandidaten.Count == 0) return null;
            Vorschlag? best = null; int bestRang = int.MaxValue;
            foreach (var v in kandidaten)
            {
                int rang = 0;
                foreach (var z in zuletzt) { if (z == v.FilterText) break; rang++; }
                if (rang < zuletzt.Count && rang < bestRang) { best = v; bestRang = rang; }
            }
            return best;
        }
    }

    public void Dispose() => arbeitsbereich.Dispose();
}
