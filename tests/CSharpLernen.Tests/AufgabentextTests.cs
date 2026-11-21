using System.Text;
using System.Text.RegularExpressions;
using CSharpLernen.Engine;
using Xunit;

namespace CSharpLernen.Tests;

/// <summary>
/// Aufgabentexte nennen nur das Ziel, nicht den Lösungsweg (Wunsch von Willi, Nov 2026):
/// keine Syntax, Operatoren, Methoden, Format-Angaben oder Schleifenarten, die die Lösung verraten.
/// Ausnahmen stehen mit Begründung in <see cref="Erlaubt"/>.
/// </summary>
public class AufgabentextTests
{
    static readonly Kurs Kurs = Lade();

    static Kurs Lade()
    {
        using var s = typeof(KursLader).Assembly.GetManifestResourceStream(KursLader.RessourcenName)!;
        using var r = new StreamReader(s, Encoding.UTF8);
        return KursLader.AusJson(r.ReadToEnd());
    }

    static IEnumerable<Aufgabe> Alle => Kurs.Lektionen.SelectMany(l => l.Aufgaben);

    /// <summary>Verräterische Bausteine (Name → Muster).</summary>
    static readonly (string Name, string Muster)[] Verraeter =
    {
        ("/*", @"/\*"), ("*/", @"\*/"), (":F", @":F\d"), ("++", @"\+\+"), ("+=", @"\+="),
        ("foreach", @"\bforeach\b"), ("TryParse", @"TryParse"), ("Parse", @"\bParse\b"), (@"\n", @"\\n"),
        ("&&", "&&"), ("||", @"\|\|"), (".Length", @"\.Length"), (".Count", @"\.Count"), (".Add", @"\.Add\b"),
        ("Contains", @"\bContains\b"), ("Remove", @"\bRemove\b"), ("Split", @"\bSplit\b"), ("Join", @"\bJoin\b"),
        ("Math.", @"Math\."), ("switch", @"\bswitch\b"), ("case", @"\bcase\b"), ("default", @"\bdefault\b"),
        ("for-Schleife", @"\bfor\b[`-]*\s*-?Schleife|`for`|\bfor\s*\("), ("while-Schleife", @"\bwhile\b[`-]*\s*-?Schleife|`while`|\bwhile\s*\("),
        ("this", @"\bthis\b"), ("return", @"\breturn\b"), ("Modulo", @"`%`|%\s*\d"), ("get; set;", @"get\s*;"),
        ("Console.Write", @"Console\.Write\b(?!Line)"), ("if", @"\bif\b"), ("else", @"\belse\b"),
        ("Trim/Substring", @"\b(Trim|Substring|ToUpper|ToLower)\b"), ("Ceiling/Sqrt", @"\b(Ceiling|Sqrt|Pow)\b"),
    };

    /// <summary>Ausnahmen mit Begründung (aktuell keine: alle Aufgabentexte kommen ohne verratene Bausteine aus).</summary>
    static readonly Dictionary<string, (string[] Namen, string Grund)> Erlaubt = new()
    {
    };

    [Fact]
    public void Aufgabentexte_verraten_keine_Loesung()
    {
        var fehler = new List<string>();
        foreach (var a in Alle)
            foreach (var (name, muster) in Verraeter)
            {
                if (!Regex.IsMatch(a.Aufgabentext, muster)) continue;
                if (Erlaubt.TryGetValue(a.Id, out var e) && e.Namen.Contains(name)) continue;
                fehler.Add($"{a.Id}: „{name}“ im Aufgabentext");
            }
        Assert.True(fehler.Count == 0, string.Join(Environment.NewLine, fehler));
    }

    [Fact]
    public void Ausnahmen_sind_noch_noetig()
    {
        foreach (var (id, (namen, grund)) in Erlaubt)
        {
            Assert.False(string.IsNullOrWhiteSpace(grund));
            var a = Alle.Single(x => x.Id == id);
            foreach (var n in namen)
                Assert.Matches(Verraeter.Single(v => v.Name == n).Muster, a.Aufgabentext);
        }
    }

    /// <summary>Tipps zeigen die Richtung, verraten aber nicht die Lösung; Meldungen sind Stupser ohne fertigen Code.</summary>
    [Fact]
    public void Tipps_und_Meldungen_ohne_fertige_Syntax()
    {
        string[] tippMuster = { @"/\*", @"\*/", @":F\d", @"\+\+", @"\+=", @"\\n", @"\bforeach\b", @"TryParse", @"\.Length", @"\.Count", @"Math\.", @"\bthis\.", @"get\s*;\s*set" };
        string[] meldungMuster = tippMuster.Concat(new[] { @"\.Add\s*\(", @"\bnew\s+\w+\s*(\(|…)", @"\w\s*=\s*\d+\s*;", @"\[\s*i\s*\]", @"\bBenutze\b" }).ToArray();
        var fehler = new List<string>();
        foreach (var a in Alle)
        {
            foreach (var m in tippMuster)
                if (Regex.IsMatch(a.Tipp, m)) fehler.Add($"{a.Id}: Tipp enthält {m}");
            foreach (var r in a.Anforderungen)
                foreach (var m in meldungMuster)
                    if (Regex.IsMatch(r.Meldung, m)) fehler.Add($"{a.Id}: Meldung „{r.Meldung}“ enthält {m}");
        }
        Assert.True(fehler.Count == 0, string.Join(Environment.NewLine, fehler));
    }
}
