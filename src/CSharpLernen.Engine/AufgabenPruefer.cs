using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CSharpLernen.Engine;

public sealed class Pruefergebnis
{
    public bool Bestanden { get; init; }
    /// <summary>Kurze Zusammenfassung, z. B. "Richtig!" oder "Noch nicht ganz …".</summary>
    public string Titel { get; init; } = "";
    /// <summary>Ausführliche Erklärung (mehrzeilig).</summary>
    public string Details { get; init; } = "";
    public Laufergebnis? Lauf { get; init; }
    public int? FehlerTest { get; init; }
    public string? ErwarteteAusgabe { get; init; }
    public string? TatsaechlicheAusgabe { get; init; }
    public IReadOnlyList<string>? Eingabe { get; init; }
}

/// <summary>Prüft eine Aufgabe: Code mit den Test-Eingaben ausführen und die Ausgabe vergleichen.</summary>
public static class AufgabenPruefer
{
    public static TimeSpan Zeitlimit { get; set; } = TimeSpan.FromSeconds(5);

    public static async Task<Pruefergebnis> PruefenAsync(Aufgabe aufgabe, string code, CancellationToken abbruch = default)
    {
        // 1. Anforderungen an den Code (z. B. "benutze eine for-Schleife")
        string ohneKommentare = OhneKommentare(code);
        foreach (var a in aufgabe.Anforderungen)
        {
            if (!Regex.IsMatch(a.MitKommentaren ? code : ohneKommentare, a.Muster))
                return new Pruefergebnis { Bestanden = false, Titel = "Noch nicht ganz", Details = a.Meldung };
        }

        var tests = aufgabe.Tests.Count > 0 ? aufgabe.Tests : new List<Testfall> { new() };
        for (int i = 0; i < tests.Count; i++)
        {
            var t = tests[i];
            var lauf = await CodeAusfuehrer.AusfuehrenAsync(code, new Laufoptionen
            {
                Eingabe = t.Eingabe,
                Zeitlimit = Zeitlimit,
                Abbruch = abbruch,
            }).ConfigureAwait(false);

            string testName = tests.Count > 1 ? $"Test {i + 1} von {tests.Count}" : "Test";
            string eingabeText = t.Eingabe.Count > 0 ? $"\nEingabe beim {testName}: {string.Join(" | ", t.Eingabe)}" : "";

            switch (lauf.Ausgang)
            {
                case Ausgang.Kompilierfehler:
                    return new Pruefergebnis
                    {
                        Bestanden = false, Titel = "Dein Code hat noch Fehler", Lauf = lauf,
                        Details = "Der Compiler meldet Fehler – siehe Liste unten. Klick auf einen Fehler, um zur Zeile zu springen.",
                    };
                case Ausgang.Abgebrochen:
                    return new Pruefergebnis { Bestanden = false, Titel = "Prüfung abgebrochen", Lauf = lauf };
                case Ausgang.Erfolg:
                    break;
                default:
                    return new Pruefergebnis
                    {
                        Bestanden = false, Titel = "Fehler beim Ausführen", Lauf = lauf, FehlerTest = i, Eingabe = t.Eingabe,
                        Details = (lauf.FehlerText ?? "") + (lauf.FehlerZeile is int z ? $" (Zeile {z})" : "")
                                  + (lauf.Tipp != null ? "\nTipp: " + lauf.Tipp : "") + eingabeText,
                        TatsaechlicheAusgabe = lauf.Ausgabe, ErwarteteAusgabe = t.Erwartet,
                    };
            }

            var vergleich = Vergleichen(t.Erwartet, lauf.Ausgabe, aufgabe.Vergleich == "lenient");
            if (!vergleich.Gleich)
            {
                return new Pruefergebnis
                {
                    Bestanden = false, Titel = "Noch nicht ganz", Lauf = lauf, FehlerTest = i, Eingabe = t.Eingabe,
                    Details = vergleich.Erklaerung + eingabeText,
                    ErwarteteAusgabe = Normalisieren(t.Erwartet), TatsaechlicheAusgabe = Normalisieren(lauf.Ausgabe),
                };
            }
        }

        return new Pruefergebnis
        {
            Bestanden = true,
            Titel = "Richtig! 🎉",
            Details = tests.Count > 1 ? $"Alle {tests.Count} Tests bestanden. Super gemacht!" : "Deine Ausgabe stimmt genau. Super gemacht!",
        };
    }

    public static string Normalisieren(string s) => string.Join("\n", Zeilen(s));

    /// <summary>Zeilenenden vereinheitlichen, Leerzeichen am Zeilenende und leere Zeilen am Anfang/Ende entfernen.</summary>
    public static List<string> Zeilen(string s)
    {
        var zeilen = (s ?? "").Replace("\r\n", "\n").Replace('\r', '\n').Split('\n').Select(z => z.TrimEnd()).ToList();
        while (zeilen.Count > 0 && zeilen[0].Length == 0) zeilen.RemoveAt(0);
        while (zeilen.Count > 0 && zeilen[^1].Length == 0) zeilen.RemoveAt(zeilen.Count - 1);
        return zeilen;
    }

    public sealed record Vergleichsergebnis(bool Gleich, string Erklaerung);

    public static Vergleichsergebnis Vergleichen(string erwartet, string tatsaechlich, bool nachsichtig)
    {
        var e = Zeilen(erwartet);
        var t = Zeilen(tatsaechlich);

        if (nachsichtig)
        {
            int pos = 0;
            for (int i = 0; i < e.Count; i++)
            {
                int gefunden = -1;
                for (int j = pos; j < t.Count; j++)
                    if (t[j].EndsWith(e[i], StringComparison.Ordinal) && (e[i].Length > 0 || t[j].Length == 0)) { gefunden = j; break; }
                if (gefunden < 0)
                {
                    string? aehnlich = t.Skip(pos).FirstOrDefault(z => z.Trim().EndsWith(e[i].Trim(), StringComparison.OrdinalIgnoreCase));
                    string extra = aehnlich != null ? $"\nÄhnlich ist deine Zeile «{aehnlich}» – achte auf Groß-/Kleinschreibung und Leerzeichen." : "";
                    return new(false, $"In deiner Ausgabe fehlt diese Zeile (Zeile {i + 1} der erwarteten Ausgabe):\n«{e[i]}»{extra}");
                }
                pos = gefunden + 1;
            }
            return new(true, "");
        }

        int n = Math.Max(e.Count, t.Count);
        for (int i = 0; i < n; i++)
        {
            if (i >= t.Count)
                return new(false, $"Deine Ausgabe ist zu kurz. Ab Zeile {i + 1} fehlt noch:\n«{e[i]}»" + (e.Count - i > 1 ? $" (und {e.Count - i - 1} weitere Zeile(n))" : ""));
            if (i >= e.Count)
                return new(false, $"Deine Ausgabe hat zu viele Zeilen. Zeile {i + 1} ist zu viel:\n«{t[i]}»");
            if (e[i] != t[i])
            {
                var sb = new StringBuilder();
                sb.Append($"Zeile {i + 1} stimmt nicht.\nErwartet:      «{e[i]}»\nDeine Ausgabe: «{t[i]}»");
                if (string.Equals(e[i], t[i], StringComparison.OrdinalIgnoreCase))
                    sb.Append("\nTipp: Achte auf Groß- und Kleinschreibung.");
                else if (OhneLeer(e[i]) == OhneLeer(t[i]))
                    sb.Append("\nTipp: Es unterscheiden sich nur Leerzeichen – achte genau auf die Abstände.");
                else if (string.Equals(OhneLeer(e[i]), OhneLeer(t[i]), StringComparison.OrdinalIgnoreCase))
                    sb.Append("\nTipp: Achte auf Leerzeichen sowie Groß- und Kleinschreibung.");
                else
                {
                    int k = 0;
                    while (k < e[i].Length && k < t[i].Length && e[i][k] == t[i][k]) k++;
                    if (k > 0) sb.Append($"\nDie ersten {k} Zeichen stimmen, danach weicht es ab.");
                }
                return new(false, sb.ToString());
            }
        }
        return new(true, "");
    }

    static string OhneLeer(string s) => new(s.Where(c => !char.IsWhiteSpace(c)).ToArray());

    /// <summary>Code ohne Kommentare (für die Anforderungs-Prüfung).</summary>
    public static string OhneKommentare(string code)
    {
        var baum = CSharpSyntaxTree.ParseText(code ?? "");
        var sb = new StringBuilder();
        foreach (var token in baum.GetRoot().DescendantTokens())
        {
            sb.Append(token.Text);
            sb.Append(' ');
        }
        return sb.ToString();
    }
}
