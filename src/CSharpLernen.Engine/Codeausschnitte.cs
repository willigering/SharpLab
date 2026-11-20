using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CSharpLernen.Engine;

/// <summary>Ein Teil eines Codeausschnitts: fester Text, ein Feld zum Ausfüllen (mit Vorgabe), ein Verweis auf ein Feld
/// (wird beim Tippen mitgeändert) oder die Endposition des Cursors.</summary>
public abstract record AusschnittTeil;
public sealed record AusschnittText(string Text) : AusschnittTeil;
public sealed record AusschnittFeld(int Nummer, string Vorgabe) : AusschnittTeil;
public sealed record AusschnittVerweis(int Nummer) : AusschnittTeil;
public sealed record AusschnittEnde : AusschnittTeil;

/// <summary>
/// Ein Codeausschnitt (Snippet) wie in Visual Studio, z. B. „cw“ + Tab Tab → Console.WriteLine();
/// Vorlage: ${1:i} = Feld 1 mit Vorgabe „i“, $1 = noch einmal Feld 1, $0 = Cursor am Ende, \t = eine Einrückung.
/// </summary>
public sealed class Codeausschnitt
{
    public Codeausschnitt(string kuerzel, string beschreibung, string vorlage)
    {
        Kuerzel = kuerzel; Beschreibung = beschreibung; Vorlage = vorlage;
        Teile = Zerlege(vorlage);
    }

    public string Kuerzel { get; }
    public string Beschreibung { get; }
    public string Vorlage { get; }
    public IReadOnlyList<AusschnittTeil> Teile { get; }

    static readonly Regex Platzhalter = new(@"\$\{(\d+):([^}]*)\}|\$(\d+)", RegexOptions.Compiled);

    static List<AusschnittTeil> Zerlege(string vorlage)
    {
        var teile = new List<AusschnittTeil>();
        var gesehen = new HashSet<int>();
        int pos = 0;
        foreach (Match m in Platzhalter.Matches(vorlage))
        {
            if (m.Index > pos) teile.Add(new AusschnittText(vorlage[pos..m.Index]));
            if (m.Groups[1].Success)
            {
                int nr = int.Parse(m.Groups[1].Value);
                teile.Add(gesehen.Add(nr) ? new AusschnittFeld(nr, m.Groups[2].Value) : new AusschnittVerweis(nr));
            }
            else
            {
                int nr = int.Parse(m.Groups[3].Value);
                teile.Add(nr == 0 ? new AusschnittEnde() : gesehen.Contains(nr) ? new AusschnittVerweis(nr) : (gesehen.Add(nr) ? new AusschnittFeld(nr, "") : new AusschnittVerweis(nr)));
            }
            pos = m.Index + m.Length;
        }
        if (pos < vorlage.Length) teile.Add(new AusschnittText(vorlage[pos..]));
        return teile;
    }

    /// <summary>Der eingefügte Text (Felder mit ihren Vorgaben), Cursorposition und Bereiche der Felder (in Tab-Reihenfolge).
    /// <paramref name="einrueckung"/> wird nach jedem Zeilenumbruch eingefügt, \t durch <paramref name="tab"/> ersetzt.</summary>
    public (string Text, int Cursor, IReadOnlyList<(int Start, int Laenge)> Felder) Erweitere(string einrueckung = "", string tab = "    ")
    {
        var sb = new StringBuilder();
        var felder = new SortedDictionary<int, (int, int)>();
        var werte = new Dictionary<int, string>();
        int cursor = -1;
        foreach (var t in Teile)
        {
            switch (t)
            {
                case AusschnittText x: sb.Append(x.Text.Replace("\t", tab).Replace("\n", "\n" + einrueckung)); break;
                case AusschnittFeld f: felder[f.Nummer] = (sb.Length, f.Vorgabe.Length); werte[f.Nummer] = f.Vorgabe; sb.Append(f.Vorgabe); break;
                case AusschnittVerweis v: sb.Append(werte.GetValueOrDefault(v.Nummer, "")); break;
                case AusschnittEnde: cursor = sb.Length; break;
            }
        }
        if (cursor < 0) cursor = sb.Length;
        return (sb.ToString(), cursor, felder.Values.ToList());
    }
}

/// <summary>Die eingebauten Codeausschnitte (wie die Standard-Snippets von Visual Studio für C#).</summary>
public static class Codeausschnitte
{
    public static readonly IReadOnlyList<Codeausschnitt> Alle = new List<Codeausschnitt>
    {
        new("cw", "Console.WriteLine();", "Console.WriteLine($0);"),
        new("cr", "Console.ReadLine();", "Console.ReadLine()$0;"),
        new("for", "for-Schleife", "for (int ${1:i} = 0; $1 < ${2:10}; $1++)\n{\n\t$0\n}"),
        new("forr", "for-Schleife rückwärts", "for (int ${1:i} = ${2:10} - 1; $1 >= 0; $1--)\n{\n\t$0\n}"),
        new("foreach", "foreach-Schleife", "foreach (var ${1:element} in ${2:liste})\n{\n\t$0\n}"),
        new("if", "if-Anweisung", "if (${1:true})\n{\n\t$0\n}"),
        new("else", "else-Block", "else\n{\n\t$0\n}"),
        new("while", "while-Schleife", "while (${1:true})\n{\n\t$0\n}"),
        new("do", "do-while-Schleife", "do\n{\n\t$0\n} while (${1:true});"),
        new("switch", "switch-Anweisung", "switch (${1:wert})\n{\n\tcase ${2:1}:\n\t\t$0\n\t\tbreak;\n\tdefault:\n\t\tbreak;\n}"),
        new("try", "try-catch-Block", "try\n{\n\t$0\n}\ncatch (${1:Exception} ex)\n{\n\tConsole.WriteLine(ex.Message);\n}"),
        new("class", "Klasse", "class ${1:MeineKlasse}\n{\n\t$0\n}"),
        new("prop", "Auto-Eigenschaft", "public ${1:int} ${2:Eigenschaft} { get; set; }$0"),
        new("ctor", "Konstruktor", "public ${1:Klasse}()\n{\n\t$0\n}"),
        new("svm", "static void Main", "static void Main(string[] args)\n{\n\t$0\n}"),
    };

    public static Codeausschnitt? Finde(string kuerzel) => Alle.FirstOrDefault(a => a.Kuerzel == kuerzel);

    static bool IstBezeichner(char c) => char.IsLetterOrDigit(c) || c == '_';

    /// <summary>
    /// Steht direkt vor <paramref name="position"/> ein Kürzel am Anfang einer Anweisung (vorne nur Leerraum,
    /// dahinter kein Buchstabe, nicht in String/Kommentar)? Dann liefert die Methode Ausschnitt und Startposition des Kürzels.
    /// </summary>
    public static bool KannErweitern(string code, int position, out Codeausschnitt? ausschnitt, out int start)
    {
        ausschnitt = null; start = position;
        code ??= "";
        if (position <= 0 || position > code.Length) return false;
        if (position < code.Length && IstBezeichner(code[position])) return false;
        start = position;
        while (start > 0 && IstBezeichner(code[start - 1])) start--;
        if (start == position) return false;
        ausschnitt = Finde(code[start..position]);
        if (ausschnitt == null) return false;
        int zeile = start;
        while (zeile > 0 && code[zeile - 1] != '\n') zeile--;
        if (code[zeile..start].Any(c => !char.IsWhiteSpace(c))) { ausschnitt = null; return false; }
        if (InStringOderKommentar(code, start)) { ausschnitt = null; return false; }
        return true;
    }

    /// <summary>Steht die Position in einem String oder Kommentar? (Dort gibt es keine Ausschnitte.)</summary>
    public static bool InStringOderKommentar(string code, int position)
    {
        var baum = CSharpSyntaxTree.ParseText(code, CodeAusfuehrer.ParseOptionen);
        var wurzel = baum.GetRoot();
        var trivia = wurzel.FindTrivia(position);
        if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) || trivia.IsKind(SyntaxKind.MultiLineCommentTrivia)
            || trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) || trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
            return trivia.Span.Start < position || trivia.Span.Contains(position);
        var token = wurzel.FindToken(position);
        return token.Span.Start < position && token.Span.End > position
               && (token.IsKind(SyntaxKind.StringLiteralToken) || token.IsKind(SyntaxKind.InterpolatedStringTextToken) || token.IsKind(SyntaxKind.CharacterLiteralToken));
    }

    /// <summary>Darf an dieser Stelle (Anfang einer Anweisung) ein Ausschnitt in der Vorschlagsliste erscheinen?</summary>
    public static bool AmAnweisungsanfang(string code, int wortStart)
    {
        int zeile = wortStart;
        while (zeile > 0 && code[zeile - 1] != '\n') zeile--;
        return code[zeile..wortStart].All(char.IsWhiteSpace);
    }
}
