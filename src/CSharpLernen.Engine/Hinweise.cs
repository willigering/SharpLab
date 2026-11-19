namespace CSharpLernen.Engine;

/// <summary>Kurze Anfänger-Tipps zu häufigen Compiler- und Laufzeitfehlern.</summary>
public static class Hinweise
{
    static readonly Dictionary<string, string> CompilerTipps = new()
    {
        ["CS1002"] = "Am Ende des Befehls fehlt ein Semikolon ; – jeder Befehl endet mit ; (meistens am Ende der Zeile davor oder dieser Zeile).",
        ["CS0103"] = "Diesen Namen kennt C# nicht. Hast du dich vertippt (Groß-/Kleinschreibung zählt!) oder die Variable noch nicht deklariert, z. B. mit int zahl = 0; ? Oder wurde sie in einem anderen { }-Block angelegt?",
        ["CS0029"] = "Der Wert passt nicht zum Datentyp der Variable. Beispiel: string in int geht nicht direkt – nimm int.Parse(text). Zahl zu Text: zahl.ToString().",
        ["CS0266"] = "Hier würde Genauigkeit verloren gehen (z. B. double → int). Wenn du das willst, schreib eine Umwandlung davor: (int)wert. Oder nimm gleich double als Typ.",
        ["CS1525"] = "Hier steht etwas, das C# an dieser Stelle nicht erwartet. Oft fehlt eine Klammer ( ) oder { }, ein Wert nach = oder es ist ein Zeichen zu viel.",
        ["CS1513"] = "Es fehlt eine schließende geschweifte Klammer }. Zu jeder { gehört genau eine }. Zähl mal nach!",
        ["CS1514"] = "Es fehlt eine öffnende geschweifte Klammer {.",
        ["CS1022"] = "Hier ist eine } zu viel, oder Code steht außerhalb einer Methode/Klasse.",
        ["CS1026"] = "Es fehlt eine schließende runde Klammer ). Zu jeder ( gehört eine ).",
        ["CS1003"] = "Hier fehlt ein Zeichen. Sehr oft fehlt ein Semikolon ; am Ende der Zeile davor – oder eine Klammer bzw. ein Komma.",
        ["CS1010"] = "Ein Text (string) wurde nicht geschlossen. Text muss mit \" anfangen und mit \" aufhören – in derselben Zeile.",
        ["CS1012"] = "Ein einzelnes Zeichen (char) steht in ' '. Für längeren Text nimm \" \".",
        ["CS1001"] = "Hier wird ein Name (Bezeichner) erwartet, z. B. der Name einer Variable.",
        ["CS0165"] = "Die Variable hat noch keinen Wert. Gib ihr beim Anlegen einen Startwert, z. B. int summe = 0;",
        ["CS0128"] = "Diese Variable gibt es schon. Lass beim zweiten Mal den Typ weg: nur zahl = 5; statt int zahl = 5;",
        ["CS0136"] = "Diesen Namen benutzt schon eine andere Variable. Nimm einen anderen Namen.",
        ["CS0019"] = "Dieser Operator passt nicht zu diesen Datentypen, z. B. Text minus Zahl. Prüfe die Typen – vielleicht fehlt int.Parse(...)?",
        ["CS1503"] = "Der Wert, den du der Methode gibst, hat den falschen Datentyp.",
        ["CS1501"] = "Die Methode bekommt zu viele oder zu wenige Werte in den Klammern.",
        ["CS7036"] = "Der Methode fehlt ein Wert (Argument) in den Klammern.",
        ["CS0161"] = "Die Methode muss in jedem Fall etwas mit return zurückgeben – auch im else-Fall.",
        ["CS0117"] = "Das gibt es so nicht. Prüfe die Schreibweise – C# unterscheidet Groß- und Kleinschreibung (WriteLine, nicht Writeline).",
        ["CS1061"] = "Diese Methode/Eigenschaft gibt es bei diesem Typ nicht. Oft ein Tippfehler oder falsche Groß-/Kleinschreibung (z. B. Length statt length).",
        ["CS0246"] = "Diesen Typ kennt C# nicht. Tippfehler? (string, int, double, bool werden klein geschrieben, List und Console groß.)",
        ["CS0201"] = "Diese Zeile tut nichts. Vielleicht fehlt ein = oder Klammern beim Methodenaufruf, z. B. Console.WriteLine() statt Console.WriteLine.",
        ["CS0428"] = "Bei einem Methodenaufruf fehlen die runden Klammern ( ).",
        ["CS8803"] = "Der Hauptcode (Anweisungen) muss oberhalb von class-/namespace-Blöcken stehen. Schreib Klassen ans Ende der Datei.",
        ["CS0127"] = "Eine void-Methode gibt nichts zurück. Entferne den Wert hinter return – oder ändere void in den passenden Typ (z. B. int).",
        ["CS0126"] = "Hinter return fehlt der Wert, den die Methode zurückgeben soll.",
        ["CS0120"] = "Diese Methode/Variable gehört zu einem Objekt. Erstelle erst ein Objekt mit new – oder mach die Methode static.",
        ["CS0122"] = "Darauf darfst du von hier aus nicht zugreifen. Mach es public.",
        ["CS0021"] = "Eckige Klammern [ ] gehen nur bei Arrays, Listen und Strings.",
        ["CS0023"] = "Dieser Operator funktioniert mit diesem Typ nicht.",
        ["CS0030"] = "Diese Umwandlung ist nicht möglich. Für Text → Zahl nimm int.Parse(...).",
        ["CS0151"] = "In switch muss ein passender Wert stehen.",
        ["CS0163"] = "Jeder case-Block muss mit break; enden.",
        ["CS8070"] = "Jeder case-Block muss mit break; enden.",
        ["CS0152"] = "Diesen case-Wert gibt es doppelt.",
        ["CS0131"] = "Links vom = muss eine Variable stehen. Zum Vergleichen nimm == statt =.",
        ["CS0664"] = "Kommazahlen für decimal brauchen ein m am Ende, z. B. 1.5m – oder nimm double.",
        ["CS1620"] = "Hier fehlt ref oder out vor dem Argument.",
        ["CS0050"] = "Prüfe, ob alle Typen public sind.",
        ["CS0106"] = "Dieser Modifizierer (z. B. public/static) ist hier nicht erlaubt. In Top-Level-Code schreibt man Methoden ohne public, z. B. static void Gruss() { ... }",
        ["CS0116"] = "Code muss in einer Methode stehen.",
        ["CS1729"] = "Für diesen Konstruktor stimmt die Anzahl der Werte in den Klammern nicht.",
    };

    public static string? FuerCompilerFehler(string code)
    {
        return CompilerTipps.TryGetValue(code, out var tipp) && tipp.Length > 0 ? tipp : null;
    }

    public static (string Text, string? Tipp) FuerLaufzeitFehler(Exception ex, bool eingabeAufgebraucht)
    {
        string typ = ex.GetType().Name;
        switch (ex)
        {
            case FormatException:
                return ($"Format-Fehler: Ein Text konnte nicht in eine Zahl umgewandelt werden. ({ex.Message})",
                    "int.Parse bzw. double.Parse brauchen eine gültige Zahl. Bei Kommazahlen: 3,5 statt 3.5 eingeben (deutsche Einstellung). Sicherer ist int.TryParse.");
            case ArgumentNullException when eingabeAufgebraucht:
                return ("Dein Programm wollte mehr Eingaben lesen, als vorhanden waren (Console.ReadLine lieferte null).",
                    "Schreib die Eingaben vorab ins Feld „Eingabe“ – eine Zeile pro Console.ReadLine().");
            case ArgumentNullException:
                return ($"Ein Wert war null (leer), obwohl einer gebraucht wurde. ({ex.Message})", null);
            case IndexOutOfRangeException:
                return ("Index außerhalb des Bereichs: Du hast auf ein Fach im Array zugegriffen, das es nicht gibt.",
                    "Indizes beginnen bei 0. Bei einem Array mit 5 Elementen ist der letzte Index 4. In Schleifen: i < array.Length (nicht <=).");
            case ArgumentOutOfRangeException:
                return ($"Index oder Wert außerhalb des erlaubten Bereichs. ({ex.Message})",
                    "Bei Listen geht der Index von 0 bis Count - 1. Bei Substring: Start + Länge darf nicht über das Textende hinausgehen.");
            case DivideByZeroException:
                return ("Division durch 0! Ganze Zahlen kann man nicht durch 0 teilen.", "Prüfe vorher mit if, ob der Teiler 0 ist.");
            case NullReferenceException:
                return ("Null-Referenz: Du benutzt etwas, das null (leer) ist – z. B. ein Objekt, das nie mit new erzeugt wurde.", null);
            case OverflowException:
                return ("Überlauf: Die Zahl ist zu groß oder zu klein für diesen Datentyp.", "Ein int geht nur bis etwa 2,1 Milliarden. Für größere Zahlen nimm long.");
            case InvalidCastException:
                return ($"Ungültige Typumwandlung. ({ex.Message})", null);
            case InsufficientExecutionStackException:
                return ("Zu tiefe Rekursion: Eine Methode ruft sich (fast) endlos selbst auf.", "Jede rekursive Methode braucht eine Abbruchbedingung, z. B. if (n <= 1) return 1;");
            case KeyNotFoundException:
                return ($"Schlüssel nicht gefunden. ({ex.Message})", null);
            case InvalidOperationException when ex.Message.Contains("ReadKey", StringComparison.OrdinalIgnoreCase) || ex.Message.Contains("console", StringComparison.OrdinalIgnoreCase):
                return ($"Diese Konsolen-Funktion geht in der Lern-App nicht. ({ex.Message})", "Nimm Console.ReadLine() für Eingaben.");
            case IOException when ex.Message.Contains("handle", StringComparison.OrdinalIgnoreCase):
                return ($"Diese Konsolen-Funktion geht in der Lern-App nicht. ({ex.Message})", null);
            default:
                return ($"{typ}: {ex.Message}", null);
        }
    }
}
