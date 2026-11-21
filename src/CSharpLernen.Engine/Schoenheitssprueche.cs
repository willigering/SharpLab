namespace CSharpLernen.Engine;

/// <summary>
/// Freundliche Sätze, wenn eine Lösung stimmt, aber ein rein ästhetischer Fehler drin ist
/// (vergessenes Leerzeichen, überflüssige Leerzeile). Die Aufgabe gilt als gelöst – es gibt nur einen Hinweis.
/// </summary>
public static class Schoenheitssprueche
{
    static readonly string[] Einleitungen =
    {
        "Gelöst! Nur ein kleiner Schönheitsfehler:",
        "Richtig verstanden und richtig gelöst – eine Kleinigkeit noch:",
        "Das passt! Nur eine Winzigkeit am Rand:",
        "Stark, die Aufgabe sitzt. Ein Detail für die Optik:",
        "Sieht nach einer richtigen Lösung aus! Nur das hier ist dir durchgerutscht:",
        "Daumen hoch – du hast die Aufgabe gelöst. Kleiner Hinweis:",
        "Aufgabe geknackt! Fürs Auge noch ein Detail:",
    };

    static readonly string[] Abschluesse =
    {
        "Wir drücken ein Auge zu 😉",
        "Das lassen wir diesmal durchgehen – achte beim nächsten Mal drauf.",
        "Kein Drama, das zählt nicht als Fehler.",
        "Dafür gibt es keinen Punktabzug. Sauber gelöst!",
        "Rein optisch, deshalb zählt es als richtig. 👍",
        "Beide Augen zu – Hauptsache, du hast verstanden, worum es geht.",
        "Das ist nur Kosmetik. Weiter so!",
    };

    public static string Einleitung() => Einleitungen[Random.Shared.Next(Einleitungen.Length)];
    public static string Abschluss() => Abschluesse[Random.Shared.Next(Abschluesse.Length)];
    public static IReadOnlyList<string> AlleEinleitungen => Einleitungen;
    public static IReadOnlyList<string> AlleAbschluesse => Abschluesse;
}
