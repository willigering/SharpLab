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
        Laufergebnis? letzterLauf = null;
        var schoenheit = new List<string>();
        for (int i = 0; i < tests.Count; i++)
        {
            var t = tests[i];
            var lauf = await CodeAusfuehrer.AusfuehrenAsync(code, new Laufoptionen
            {
                Eingabe = t.Eingabe,
                Zeitlimit = Zeitlimit,
                Abbruch = abbruch,
            }).ConfigureAwait(false);
            letzterLauf = lauf;

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
            if (vergleich.Schoenheitshinweis != null && !schoenheit.Contains(vergleich.Schoenheitshinweis))
                schoenheit.Add(vergleich.Schoenheitshinweis);
        }

        string lob = tests.Count > 1 ? $"Alle {tests.Count} Tests bestanden. Super gemacht!" : "Deine Ausgabe stimmt. Super gemacht!";
        if (schoenheit.Count > 0)
            lob = Schoenheitssprueche.Einleitung() + " " + schoenheit[0] + "\n" + Schoenheitssprueche.Abschluss();

        return new Pruefergebnis
        {
            Bestanden = true,
            Titel = "Richtig! 🎉",
            Details = lob,
            // Die Konsole zeigt genau diesen Lauf. Ohne Lauf bliebe die Ausgabe nach „Richtig!“ leer.
            Lauf = letzterLauf,
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

    /// <summary>
    /// Gleich = bestanden. Schoenheitshinweis ist gesetzt, wenn die Ausgabe nur in Leerzeichen oder Leerzeilen
    /// vom Erwarteten abweicht: das zählt als richtig, der Nutzer wird nur freundlich darauf hingewiesen.
    /// </summary>
    public sealed record Vergleichsergebnis(bool Gleich, string Erklaerung, string? Schoenheitshinweis = null);

    public static Vergleichsergebnis Vergleichen(string erwartet, string tatsaechlich, bool nachsichtig)
    {
        var e = Zeilen(erwartet);
        var t = Zeilen(tatsaechlich);

        if (nachsichtig)
        {
            int pos = 0;
            string? schoenheit = null;
            for (int i = 0; i < e.Count; i++)
            {
                int gefunden = -1;
                for (int j = pos; j < t.Count; j++)
                    if (t[j].EndsWith(e[i], StringComparison.Ordinal) && (e[i].Length > 0 || t[j].Length == 0)) { gefunden = j; break; }
                if (gefunden < 0)
                {
                    // Nur Kleinigkeiten anders (Leerzeichen, Satzzeichen am Ende, Tippfehler)? Dann gilt es als richtig.
                    int locker = -1; string? lockerHinweis = null;
                    for (int j = pos; j < t.Count && locker < 0; j++)
                        if (LockerEndet(e[i], t[j], out lockerHinweis)) locker = j;
                    if (locker >= 0)
                    {
                        schoenheit ??= $"In Zeile {i + 1} {lockerHinweis}";
                        pos = locker + 1;
                        continue;
                    }
                    string? aehnlich = t.Skip(pos).FirstOrDefault(z => z.Trim().EndsWith(e[i].Trim(), StringComparison.OrdinalIgnoreCase));
                    string extra = aehnlich != null ? $"\nÄhnlich ist deine Zeile «{aehnlich}» – achte auf Groß-/Kleinschreibung und Leerzeichen." : "";
                    return new(false, $"In deiner Ausgabe fehlt diese Zeile (Zeile {i + 1} der erwarteten Ausgabe):\n«{e[i]}»{extra}");
                }
                pos = gefunden + 1;
            }
            return new(true, "", schoenheit);
        }

        // Exakter Vergleich. Rein ästhetische Abweichungen (Leerzeichen, Leerzeilen, . statt !, kleine Tippfehler) zählen nicht als Fehler.
        if (!e.SequenceEqual(t))
        {
            var eOrig = e.Where(z => z.Length > 0).ToList();
            var tOrig = t.Where(z => z.Length > 0).ToList();
            if (eOrig.Count == tOrig.Count)
            {
                string? erste = null; bool alleOk = true;
                for (int i = 0; i < eOrig.Count && alleOk; i++)
                {
                    if (eOrig[i] == tOrig[i]) continue;
                    if (ZeileAehnlich(eOrig[i], tOrig[i], out string? h)) erste ??= $"In Zeile {i + 1} {h}";
                    else alleOk = false;
                }
                if (alleOk) return new(true, "", erste ?? "Bei den Leerzeilen weicht deine Ausgabe vom Beispiel ab.");
            }
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

    /// <summary>
    /// Ist die Zeile „fast richtig“? Erlaubt: Leerzeichen anders, am Ende . statt ! (oder umgekehrt, oder weggelassen)
    /// und ein kleiner Tippfehler in Wörtern. Streng bleiben: Zahlen, Groß-/Kleinschreibung, kurze Zeilen.
    /// </summary>
    internal static bool ZeileAehnlich(string erwartet, string tatsaechlich, out string? hinweis, bool mitBeispiel = true)
    {
        hinweis = null;
        string e = OhneLeer(erwartet), t = OhneLeer(tatsaechlich);
        int ve = erwartet.Count(char.IsWhiteSpace), vt = tatsaechlich.Count(char.IsWhiteSpace);
        string leerHinweis = vt < ve ? "fehlt ein Leerzeichen" : vt > ve ? "ist ein Leerzeichen zu viel" : "sitzt ein Leerzeichen an einer anderen Stelle";
        string beispiel = mitBeispiel ? $" (Beispiel: «{erwartet}», bei dir: «{tatsaechlich}»)." : ".";
        if (e == t) { hinweis = leerHinweis + beispiel; return true; }

        // Satzzeichen am Ende: . und ! austauschbar oder weggelassen
        string e2 = SatzzeichenWeg(e), t2 = SatzzeichenWeg(t);
        bool satzzeichenAnders = e2 != e || t2 != t;
        if (satzzeichenAnders && e2 == t2 && e2.Length > 0)
        {
            hinweis = "weicht das Satzzeichen am Ende ab (. und ! verwechselt oder vergessen)" + beispiel;
            return true;
        }

        // Kleiner Tippfehler: Zahlen müssen exakt gleich sein, Groß-/Kleinschreibung zählt, sehr kurze Zeilen sind streng.
        if (e2.Length >= 5 && KeineBuchstaben(e2) == KeineBuchstaben(t2) && !string.Equals(e2, t2, StringComparison.OrdinalIgnoreCase))
        {
            int erlaubt = e2.Length > 24 ? 2 : 1;
            if (Abstand(e2, t2, erlaubt) <= erlaubt)
            {
                hinweis = "steckt ein kleiner Tippfehler" + beispiel;
                return true;
            }
        }
        return false;
    }

    /// <summary>Für den nachsichtigen Vergleich: endet die Zeile (vor einer Eingabeaufforderung) „fast“ wie erwartet?</summary>
    static bool LockerEndet(string erwartet, string zeile, out string? hinweis)
    {
        hinweis = null;
        string e = OhneLeer(erwartet);
        if (e.Length == 0) return false;
        string t = OhneLeer(zeile);
        for (int laenge = Math.Min(t.Length, e.Length + 1); laenge >= Math.Max(1, e.Length - 2); laenge--)
        {
            string ende = t[^laenge..];
            if (ZeileAehnlich(erwartet, ende, out hinweis, mitBeispiel: false)) return true;
        }
        return false;
    }

    static string SatzzeichenWeg(string s) => s.TrimEnd('.', '!');
    /// <summary>Alles außer Buchstaben (Zahlen, Rechenzeichen, Satzzeichen): muss bei Tippfehler-Toleranz identisch sein.</summary>
    static string KeineBuchstaben(string s) => new(s.Where(c => !char.IsLetter(c)).ToArray());

    /// <summary>Editierabstand (Levenshtein), bricht ab, sobald er sicher über der Grenze liegt.</summary>
    static int Abstand(string a, string b, int grenze)
    {
        if (Math.Abs(a.Length - b.Length) > grenze) return grenze + 1;
        var vor = new int[b.Length + 1];
        var akt = new int[b.Length + 1];
        for (int j = 0; j <= b.Length; j++) vor[j] = j;
        for (int i = 1; i <= a.Length; i++)
        {
            akt[0] = i; int min = akt[0];
            for (int j = 1; j <= b.Length; j++)
            {
                int kosten = a[i - 1] == b[j - 1] ? 0 : 1;
                akt[j] = Math.Min(Math.Min(akt[j - 1] + 1, vor[j] + 1), vor[j - 1] + kosten);
                min = Math.Min(min, akt[j]);
            }
            if (min > grenze) return grenze + 1;
            (vor, akt) = (akt, vor);
        }
        return vor[b.Length];
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
