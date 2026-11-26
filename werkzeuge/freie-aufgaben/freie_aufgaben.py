#!/usr/bin/env python3
"""Erzeugt src/CSharpLernen.Engine/Inhalte/FreieAufgaben.json (Reiter „Freie Aufgaben“).

Aufruf:  python3 werkzeuge/freie-aufgaben/freie_aufgaben.py

Regeln (wie bei den Lektionen, geprüft von den Tests):
- Aufgabentexte nennen nur das Ziel, nicht den Lösungsweg (keine verratenen Schlüsselwörter).
- Jede Musterlösung besteht alle Tests, jeder Startcode fällt durch.
- „falsch“ enthält typische Fehllösungen. Sie müssen durchfallen. So ist sichergestellt, dass die
  Tests trennscharf sind (die Prüfung verzeiht kleine Tippfehler, das darf keine falsche Lösung retten).
- Ausgaben mit Bezeichnung („Summe: 23“ statt „23“), damit der nachsichtige Vergleich nicht über
  Zeilenenden stolpert. Kommazahlen erscheinen deutsch formatiert (15,0), weil die App mit de-DE prüft.
"""
import json, os, textwrap

def c(s):
    """Code-Block: Einrückung entfernen, mit genau einem Zeilenumbruch enden."""
    return textwrap.dedent(s).strip("\n") + "\n"

START = "// Schreib hier deinen Code\n"
FRAGE = "Eine Frage vor der Eingabe ist erlaubt."
FRAGEN = "Fragen vor den Eingaben sind erlaubt."

def t(eingabe, erwartet):
    return {"input": [str(x) for x in eingabe], "expected": erwartet}

def a(id, titel, thema, text, loesung, tipp, tests, start=START, vergleich="lenient", anf=None, falsch=None):
    return {"id": id, "title": titel, "topic": thema, "task": text, "starter": start, "solution": loesung,
            "hint": tipp, "compare": vergleich, "tests": tests, "requirements": anf or [], "wrong": falsch or []}

# ============================================================== LEICHT
leicht = [
a("fl01", "Haus aus Zeichen", "Ausgabe",
  "Zeichne dieses kleine Haus Zeile für Zeile in die Konsole:\n\n```\n  /\\\n /  \\\n/____\\\n|    |\n|_[]_|\n```",
  c(r'''
    Console.WriteLine("  /\\");
    Console.WriteLine(" /  \\");
    Console.WriteLine("/____\\");
    Console.WriteLine("|    |");
    Console.WriteLine("|_[]_|");
  '''),
  "Der Backslash hat in C#-Texten eine Sonderbedeutung. Überleg, wie du ihn als ganz normales Zeichen ausgibst.",
  [t([], "  /\\\n /  \\\n/____\\\n|    |\n|_[]_|")], vergleich="exact",
  falsch=[c(r'''
    Console.WriteLine("  /");
    Console.WriteLine(" /  ");
    Console.WriteLine("/____");
    Console.WriteLine("|    |");
    Console.WriteLine("|_[]_|");
  ''')]),

a("fl02", "Blick in die Zukunft", "Eingabe & Rechnen",
  "Das Programm liest dein Geburtsjahr ein und verrät, wie alt du im Jahr 2030 wirst. " + FRAGE +
  "\n\n```\n2010  →  Im Jahr 2030 wirst du 20 Jahre alt.\n1985  →  Im Jahr 2030 wirst du 45 Jahre alt.\n```",
  c('''
    Console.Write("Geburtsjahr: ");
    int jahr = int.Parse(Console.ReadLine());
    Console.WriteLine($"Im Jahr 2030 wirst du {2030 - jahr} Jahre alt.");
  '''),
  "Eingaben kommen immer als Text an. Zum Rechnen brauchst du daraus erst eine Zahl.",
  [t([2010], "Im Jahr 2030 wirst du 20 Jahre alt."), t([1985], "Im Jahr 2030 wirst du 45 Jahre alt."),
   t([2024], "Im Jahr 2030 wirst du 6 Jahre alt.")],
  falsch=[c('''
    int jahr = int.Parse(Console.ReadLine());
    Console.WriteLine($"Im Jahr 2030 wirst du {2026 - jahr} Jahre alt.");
  ''')]),

a("fl03", "Pizza für alle", "Rechnen",
  "Eine Pizza hat 8 Stücke. Das Programm liest zuerst die Anzahl der Pizzen und dann die Anzahl der Personen ein. "
  "Alle bekommen gleich viele ganze Stücke. Was nicht aufgeht, bleibt übrig. " + FRAGEN +
  "\n\n```\n3 Pizzen, 10 Personen  →  Pro Person: 2 Stücke / Übrig: 4 Stücke\n4 Pizzen, 8 Personen  →  Pro Person: 4 Stücke / Übrig: 0 Stücke\n```",
  c('''
    Console.Write("Pizzen: ");
    int pizzen = int.Parse(Console.ReadLine());
    Console.Write("Personen: ");
    int personen = int.Parse(Console.ReadLine());
    int stuecke = pizzen * 8;
    Console.WriteLine($"Pro Person: {stuecke / personen} Stücke");
    Console.WriteLine($"Übrig: {stuecke % personen} Stücke");
  '''),
  "Rechne zuerst aus, wie viele Stücke es insgesamt gibt. Für das Teilen ganzer Zahlen und für den Rest hat C# je einen eigenen Rechenoperator.",
  [t([3, 10], "Pro Person: 2 Stücke\nÜbrig: 4 Stücke"), t([4, 8], "Pro Person: 4 Stücke\nÜbrig: 0 Stücke"),
   t([2, 7], "Pro Person: 2 Stücke\nÜbrig: 2 Stücke"), t([5, 6], "Pro Person: 6 Stücke\nÜbrig: 4 Stücke")],
  falsch=[c('''
    int pizzen = int.Parse(Console.ReadLine());
    int personen = int.Parse(Console.ReadLine());
    Console.WriteLine($"Pro Person: {pizzen * 8 / personen} Stücke");
    Console.WriteLine($"Übrig: {pizzen % personen} Stücke");
  ''')]),

a("fl04", "Achterbahn", "Entscheidungen",
  "Für die Achterbahn muss man mindestens 120 cm groß sein. Das Programm liest die Körpergröße in Zentimetern ein und sagt, ob man mitfahren darf. " + FRAGE +
  "\n\n```\n130  →  Einsteigen, viel Spaß!\n120  →  Einsteigen, viel Spaß!\n119  →  Leider noch zu klein.\n```",
  c('''
    Console.Write("Größe in cm: ");
    int groesse = int.Parse(Console.ReadLine());
    if (groesse >= 120)
        Console.WriteLine("Einsteigen, viel Spaß!");
    else
        Console.WriteLine("Leider noch zu klein.");
  '''),
  "Achte auf die Grenze: Wer genau 120 cm groß ist, darf schon mitfahren.",
  [t([130], "Einsteigen, viel Spaß!"), t([120], "Einsteigen, viel Spaß!"), t([119], "Leider noch zu klein."), t([95], "Leider noch zu klein.")],
  falsch=[c('''
    int groesse = int.Parse(Console.ReadLine());
    if (groesse > 120) Console.WriteLine("Einsteigen, viel Spaß!");
    else Console.WriteLine("Leider noch zu klein.");
  ''')]),

a("fl05", "Sternenreihe", "Schleifen",
  "Das Programm liest eine Zahl ein und gibt genau so viele Sterne in einer Zeile aus. " + FRAGE +
  "\n\n```\n5  →  Sterne: *****\n1  →  Sterne: *\n8  →  Sterne: ********\n```",
  c('''
    Console.Write("Anzahl: ");
    int anzahl = int.Parse(Console.ReadLine());
    string sterne = "";
    for (int i = 0; i < anzahl; i++)
    {
        sterne = sterne + "*";
    }
    Console.WriteLine($"Sterne: {sterne}");
  '''),
  "Häng so oft einen Stern an, wie die Zahl angibt, und gib die Reihe erst am Ende aus.",
  [t([5], "Sterne: *****"), t([1], "Sterne: *"), t([8], "Sterne: ********")],
  falsch=[c('''
    int anzahl = int.Parse(Console.ReadLine());
    string sterne = "";
    for (int i = 0; i <= anzahl; i++) sterne = sterne + "*";
    Console.WriteLine($"Sterne: {sterne}");
  ''')]),

a("fl06", "Parkhaus", "Rechnen",
  "Im Parkhaus kostet die erste Stunde 3 Euro, jede weitere Stunde 2 Euro. Das Programm liest die Parkdauer in ganzen Stunden ein (mindestens 1) und gibt die Gebühr aus. " + FRAGE +
  "\n\n```\n1  →  Parkgebühr: 3 Euro\n4  →  Parkgebühr: 9 Euro\n10  →  Parkgebühr: 21 Euro\n```",
  c('''
    Console.Write("Stunden: ");
    int stunden = int.Parse(Console.ReadLine());
    int gebuehr = 3 + (stunden - 1) * 2;
    Console.WriteLine($"Parkgebühr: {gebuehr} Euro");
  '''),
  "Rechne die erste Stunde getrennt von den übrigen.",
  [t([1], "Parkgebühr: 3 Euro"), t([4], "Parkgebühr: 9 Euro"), t([10], "Parkgebühr: 21 Euro")],
  falsch=[c('''
    int stunden = int.Parse(Console.ReadLine());
    Console.WriteLine($"Parkgebühr: {3 + stunden * 2} Euro");
  ''')]),

a("fl07", "Teilbar durch 3?", "Entscheidungen",
  "Das Programm liest eine ganze Zahl ein und prüft, ob sie ohne Rest durch 3 teilbar ist. " + FRAGE +
  "\n\n```\n27  →  27 ist durch 3 teilbar.\n28  →  28 ist nicht durch 3 teilbar.\n0  →  0 ist durch 3 teilbar.\n```",
  c('''
    Console.Write("Zahl: ");
    int zahl = int.Parse(Console.ReadLine());
    if (zahl % 3 == 0)
        Console.WriteLine($"{zahl} ist durch 3 teilbar.");
    else
        Console.WriteLine($"{zahl} ist nicht durch 3 teilbar.");
  '''),
  "Es gibt einen Rechenoperator, der dir den Rest einer Division liefert. Ist der Rest 0, geht die Division auf.",
  [t([27], "27 ist durch 3 teilbar."), t([28], "28 ist nicht durch 3 teilbar."), t([0], "0 ist durch 3 teilbar."),
   t([-9], "-9 ist durch 3 teilbar."), t([31], "31 ist nicht durch 3 teilbar.")],
  falsch=[c('''
    int zahl = int.Parse(Console.ReadLine());
    Console.WriteLine($"{zahl} ist durch 3 teilbar.");
  '''), c('''
    int zahl = int.Parse(Console.ReadLine());
    if (zahl / 3 == 0) Console.WriteLine($"{zahl} ist durch 3 teilbar.");
    else Console.WriteLine($"{zahl} ist nicht durch 3 teilbar.");
  ''')]),

a("fl08", "Zahlen summieren", "Schleifen",
  "Das Programm liest eine positive ganze Zahl n ein und addiert alle Zahlen von 1 bis n. " + FRAGE +
  "\n\n```\n4  →  Summe von 1 bis 4: 10\n10  →  Summe von 1 bis 10: 55\n1  →  Summe von 1 bis 1: 1\n```",
  c('''
    Console.Write("n: ");
    int n = int.Parse(Console.ReadLine());
    int summe = 0;
    for (int i = 1; i <= n; i++)
    {
        summe = summe + i;
    }
    Console.WriteLine($"Summe von 1 bis {n}: {summe}");
  '''),
  "Merk dir eine Zwischensumme und zähle nacheinander jede Zahl dazu.",
  [t([4], "Summe von 1 bis 4: 10"), t([10], "Summe von 1 bis 10: 55"), t([1], "Summe von 1 bis 1: 1"), t([100], "Summe von 1 bis 100: 5050")],
  falsch=[c('''
    int n = int.Parse(Console.ReadLine());
    int summe = 0;
    for (int i = 1; i < n; i++) summe = summe + i;
    Console.WriteLine($"Summe von 1 bis {n}: {summe}");
  ''')]),

a("fl09", "Wortlänge", "Texte",
  "Das Programm liest ein Wort ein und nennt die Anzahl seiner Buchstaben. " + FRAGE +
  "\n\n```\nHaus  →  Haus hat 4 Buchstaben.\nProgrammieren  →  Programmieren hat 13 Buchstaben.\n```",
  c('''
    Console.Write("Wort: ");
    string wort = Console.ReadLine();
    Console.WriteLine($"{wort} hat {wort.Length} Buchstaben.");
  '''),
  "Jeder Text weiß selbst, wie viele Zeichen er hat.",
  [t(["Haus"], "Haus hat 4 Buchstaben."), t(["Programmieren"], "Programmieren hat 13 Buchstaben."),
   t(["Ei"], "Ei hat 2 Buchstaben."), t(["Übung"], "Übung hat 5 Buchstaben.")],
  falsch=[c('''
    string wort = Console.ReadLine();
    Console.WriteLine($"{wort} hat {wort.Length - 1} Buchstaben.");
  ''')]),

a("fl10", "Passwort-Länge", "Texte",
  "Ein Passwort muss mindestens 8 Zeichen lang sein. Das Programm liest ein Passwort ein und gibt eine passende Rückmeldung. " + FRAGE +
  "\n\n```\ngeheim123  →  Das Passwort ist lang genug.\nabc  →  Zu kurz: 3 von 8 Zeichen.\n12345678  →  Das Passwort ist lang genug.\n```",
  c('''
    Console.Write("Passwort: ");
    string passwort = Console.ReadLine();
    if (passwort.Length >= 8)
        Console.WriteLine("Das Passwort ist lang genug.");
    else
        Console.WriteLine($"Zu kurz: {passwort.Length} von 8 Zeichen.");
  '''),
  "Vergleiche die Anzahl der Zeichen mit der Mindestlänge. Wer genau 8 Zeichen hat, ist schon dabei.",
  [t(["geheim123"], "Das Passwort ist lang genug."), t(["abc"], "Zu kurz: 3 von 8 Zeichen."),
   t(["12345678"], "Das Passwort ist lang genug."), t(["Sommer7"], "Zu kurz: 7 von 8 Zeichen.")],
  falsch=[c('''
    string passwort = Console.ReadLine();
    if (passwort.Length > 8) Console.WriteLine("Das Passwort ist lang genug.");
    else Console.WriteLine($"Zu kurz: {passwort.Length} von 8 Zeichen.");
  ''')]),

a("fl11", "Tagesgruß", "Entscheidungen",
  "Das Programm liest eine Uhrzeit als volle Stunde (0 bis 23) ein und grüßt passend: bis 11 Uhr mit „Guten Morgen!“, "
  "von 12 bis 17 Uhr mit „Guten Tag!“ und ab 18 Uhr mit „Guten Abend!“. " + FRAGE +
  "\n\n```\n8  →  Guten Morgen!\n12  →  Guten Tag!\n17  →  Guten Tag!\n18  →  Guten Abend!\n```",
  c('''
    Console.Write("Stunde: ");
    int stunde = int.Parse(Console.ReadLine());
    if (stunde < 12)
        Console.WriteLine("Guten Morgen!");
    else if (stunde < 18)
        Console.WriteLine("Guten Tag!");
    else
        Console.WriteLine("Guten Abend!");
  '''),
  "Prüfe die Bereiche der Reihe nach, vom frühen Morgen bis zum Abend. Achte genau auf die Grenzen 12 und 18.",
  [t([8], "Guten Morgen!"), t([12], "Guten Tag!"), t([17], "Guten Tag!"), t([18], "Guten Abend!"),
   t([0], "Guten Morgen!"), t([23], "Guten Abend!")],
  falsch=[c('''
    int stunde = int.Parse(Console.ReadLine());
    if (stunde <= 12) Console.WriteLine("Guten Morgen!");
    else if (stunde <= 18) Console.WriteLine("Guten Tag!");
    else Console.WriteLine("Guten Abend!");
  ''')]),

a("fl12", "Gerade Zahlen", "Schleifen",
  "Das Programm liest eine Zahl n ein und gibt alle geraden Zahlen von 2 bis n in einer Zeile aus, getrennt durch Leerzeichen. " + FRAGE +
  "\n\n```\n10  →  Gerade Zahlen: 2 4 6 8 10\n7  →  Gerade Zahlen: 2 4 6\n2  →  Gerade Zahlen: 2\n```",
  c('''
    Console.Write("n: ");
    int n = int.Parse(Console.ReadLine());
    string zeile = "Gerade Zahlen:";
    for (int i = 2; i <= n; i = i + 2)
    {
        zeile = zeile + " " + i;
    }
    Console.WriteLine(zeile);
  '''),
  "Starte bei 2 und geh immer in Zweierschritten weiter, solange du n nicht überschreitest.",
  [t([10], "Gerade Zahlen: 2 4 6 8 10"), t([7], "Gerade Zahlen: 2 4 6"), t([2], "Gerade Zahlen: 2"), t([15], "Gerade Zahlen: 2 4 6 8 10 12 14")],
  falsch=[c('''
    int n = int.Parse(Console.ReadLine());
    string zeile = "Gerade Zahlen:";
    for (int i = 2; i < n; i = i + 2) zeile = zeile + " " + i;
    Console.WriteLine(zeile);
  ''')]),
]

# ============================================================== MITTEL
mittel = [
a("fm01", "FizzBuzz", "Schleifen",
  "Ein Klassiker unter Programmierern: Gib die Zahlen von 1 bis 15 aus, jede in einer eigenen Zeile. "
  "Ist eine Zahl durch 3 teilbar, steht dort stattdessen `Fizz`. Ist sie durch 5 teilbar, steht dort `Buzz`. "
  "Ist sie durch beide teilbar, steht dort `FizzBuzz`.\n\n```\n1\n2\nFizz\n4\nBuzz\nFizz\n7\n…\n14\nFizzBuzz\n```",
  c('''
    for (int i = 1; i <= 15; i++)
    {
        if (i % 15 == 0)
            Console.WriteLine("FizzBuzz");
        else if (i % 3 == 0)
            Console.WriteLine("Fizz");
        else if (i % 5 == 0)
            Console.WriteLine("Buzz");
        else
            Console.WriteLine(i);
    }
  '''),
  "Die Reihenfolge der Prüfungen entscheidet: Was durch 3 und durch 5 teilbar ist, muss zuerst erkannt werden.",
  [t([], "1\n2\nFizz\n4\nBuzz\nFizz\n7\n8\nFizz\nBuzz\n11\nFizz\n13\n14\nFizzBuzz")], vergleich="exact",
  anf=[{"pattern": r"\b(for|while|foreach)\b", "message": "Lass das Programm zählen, statt jede Zeile einzeln hinzuschreiben."}],
  falsch=[c('''
    for (int i = 1; i <= 15; i++)
    {
        if (i % 3 == 0) Console.WriteLine("Fizz");
        else if (i % 5 == 0) Console.WriteLine("Buzz");
        else if (i % 15 == 0) Console.WriteLine("FizzBuzz");
        else Console.WriteLine(i);
    }
  ''')]),

a("fm02", "Collatz-Folge", "Schleifen",
  "Nimm eine positive ganze Zahl. Ist sie gerade, halbiere sie. Ist sie ungerade, nimm sie mal 3 und zähle 1 dazu. "
  "Wiederhole das, bis du bei 1 ankommst. Das Programm liest die Startzahl ein und gibt aus, wie viele Schritte es bis zur 1 braucht. " + FRAGE +
  "\n\n```\n6  →  Schritte bis zur 1: 8\n1  →  Schritte bis zur 1: 0\n27  →  Schritte bis zur 1: 111\n```",
  c('''
    Console.Write("Startzahl: ");
    int zahl = int.Parse(Console.ReadLine());
    int schritte = 0;
    while (zahl != 1)
    {
        if (zahl % 2 == 0)
            zahl = zahl / 2;
        else
            zahl = zahl * 3 + 1;
        schritte++;
    }
    Console.WriteLine($"Schritte bis zur 1: {schritte}");
  '''),
  "Zähle bei jedem Rechenschritt mit und hör auf, sobald die Zahl 1 ist. Startest du schon bei 1, sind es 0 Schritte.",
  [t([6], "Schritte bis zur 1: 8"), t([1], "Schritte bis zur 1: 0"), t([27], "Schritte bis zur 1: 111"), t([12], "Schritte bis zur 1: 9")],
  falsch=[c('''
    int zahl = int.Parse(Console.ReadLine());
    int schritte = 0;
    do
    {
        if (zahl % 2 == 0) zahl = zahl / 2; else zahl = zahl * 3 + 1;
        schritte++;
    } while (zahl != 1);
    Console.WriteLine($"Schritte bis zur 1: {schritte}");
  ''')]),

a("fm03", "Wetterstation", "Arrays",
  "Das Programm liest nacheinander 5 Temperaturen (ganze Zahlen) ein. Danach gibt es den höchsten Wert, den niedrigsten Wert "
  "und den Durchschnitt mit einer Nachkommastelle aus. " + FRAGEN +
  "\n\n```\n12, 15, 9, 21, 18  →  Höchstwert: 21 / Tiefstwert: 9 / Durchschnitt: 15,0\n7, 7, 8, 9, 10  →  Höchstwert: 10 / Tiefstwert: 7 / Durchschnitt: 8,2\n```",
  c('''
    int[] werte = new int[5];
    for (int i = 0; i < werte.Length; i++)
    {
        Console.Write($"Temperatur {i + 1}: ");
        werte[i] = int.Parse(Console.ReadLine());
    }
    int hoechst = werte[0];
    int tiefst = werte[0];
    int summe = 0;
    foreach (int w in werte)
    {
        if (w > hoechst) hoechst = w;
        if (w < tiefst) tiefst = w;
        summe += w;
    }
    double schnitt = (double)summe / werte.Length;
    Console.WriteLine($"Höchstwert: {hoechst}");
    Console.WriteLine($"Tiefstwert: {tiefst}");
    Console.WriteLine($"Durchschnitt: {schnitt:F1}");
  '''),
  "Achtung beim Durchschnitt: Teilst du zwei ganze Zahlen, fallen die Nachkommastellen einfach weg.",
  [t([12, 15, 9, 21, 18], "Höchstwert: 21\nTiefstwert: 9\nDurchschnitt: 15,0"),
   t([7, 7, 8, 9, 10], "Höchstwert: 10\nTiefstwert: 7\nDurchschnitt: 8,2"),
   t([-3, 0, 4, -8, 2], "Höchstwert: 4\nTiefstwert: -8\nDurchschnitt: -1,0")],
  falsch=[c('''
    int[] werte = new int[5];
    for (int i = 0; i < 5; i++) werte[i] = int.Parse(Console.ReadLine());
    Console.WriteLine($"Höchstwert: {werte.Max()}");
    Console.WriteLine($"Tiefstwert: {werte.Min()}");
    Console.WriteLine($"Durchschnitt: {werte.Sum() / 5:F1}");
  '''), c('''
    int[] werte = new int[5];
    for (int i = 0; i < 5; i++) werte[i] = int.Parse(Console.ReadLine());
    int tiefst = 0;
    foreach (int w in werte) if (w < tiefst) tiefst = w;
    Console.WriteLine($"Höchstwert: {werte.Max()}");
    Console.WriteLine($"Tiefstwert: {tiefst}");
    Console.WriteLine($"Durchschnitt: {werte.Average():F1}");
  ''')]),

a("fm04", "Regentropfen", "Methoden",
  "Das Programm macht aus einer Zahl Regengeräusche: Ist sie durch 3 teilbar, kommt `Plopp` dazu, durch 5 `Platsch` und durch 7 `Tropf`, "
  "immer in dieser Reihenfolge. Passt keiner der drei Teiler, erscheint die Zahl selbst. "
  "Vervollständige dafür die Methode `Regentropfen` im Startcode. Sie bekommt die Zahl und liefert den fertigen Text.\n\n"
  "```\n9  →  Ergebnis: Plopp\n28  →  Ergebnis: Tropf\n30  →  Ergebnis: PloppPlatsch\n34  →  Ergebnis: 34\n```",
  c('''
    int zahl = int.Parse(Console.ReadLine());
    Console.WriteLine($"Ergebnis: {Regentropfen(zahl)}");

    string Regentropfen(int n)
    {
        string text = "";
        if (n % 3 == 0) text += "Plopp";
        if (n % 5 == 0) text += "Platsch";
        if (n % 7 == 0) text += "Tropf";
        if (text == "") text = n.ToString();
        return text;
    }
  '''),
  "Bau den Text Stück für Stück zusammen. Ist er am Ende noch leer, gehört die Zahl hinein.",
  [t([9], "Ergebnis: Plopp"), t([28], "Ergebnis: Tropf"), t([30], "Ergebnis: PloppPlatsch"), t([34], "Ergebnis: 34"),
   t([105], "Ergebnis: PloppPlatschTropf"), t([25], "Ergebnis: Platsch")],
  start=c('''
    int zahl = int.Parse(Console.ReadLine());
    Console.WriteLine($"Ergebnis: {Regentropfen(zahl)}");

    string Regentropfen(int n)
    {
        // Schreib hier deinen Code
        return "";
    }
  '''),
  anf=[{"pattern": r"string\s+Regentropfen\s*\(", "message": "Lass die Methode `Regentropfen` im Code stehen und vervollständige sie."}],
  falsch=[c('''
    int zahl = int.Parse(Console.ReadLine());
    Console.WriteLine($"Ergebnis: {Regentropfen(zahl)}");
    string Regentropfen(int n)
    {
        if (n % 3 == 0) return "Plopp";
        if (n % 5 == 0) return "Platsch";
        if (n % 7 == 0) return "Tropf";
        return n.ToString();
    }
  ''')]),

a("fm05", "Summe der Vielfachen", "Schleifen",
  "Das Programm liest eine Zahl n ein und addiert alle Zahlen unter n, die durch 3 oder durch 5 teilbar sind. "
  "Jede Zahl zählt nur einmal, auch wenn sie durch beide teilbar ist. " + FRAGE +
  "\n\n```\n10  →  Summe: 23\n16  →  Summe: 60\n1  →  Summe: 0\n```",
  c('''
    Console.Write("n: ");
    int n = int.Parse(Console.ReadLine());
    int summe = 0;
    for (int i = 1; i < n; i++)
    {
        if (i % 3 == 0 || i % 5 == 0)
            summe += i;
    }
    Console.WriteLine($"Summe: {summe}");
  '''),
  "Prüfe jede Zahl einzeln. Achte darauf, dass n selbst nicht mehr dazugehört.",
  [t([10], "Summe: 23"), t([16], "Summe: 60"), t([1], "Summe: 0"), t([100], "Summe: 2318")],
  falsch=[c('''
    int n = int.Parse(Console.ReadLine());
    int summe = 0;
    for (int i = 1; i < n; i++)
    {
        if (i % 3 == 0) summe += i;
        if (i % 5 == 0) summe += i;
    }
    Console.WriteLine($"Summe: {summe}");
  '''), c('''
    int n = int.Parse(Console.ReadLine());
    int summe = 0;
    for (int i = 1; i <= n; i++) if (i % 3 == 0 || i % 5 == 0) summe += i;
    Console.WriteLine($"Summe: {summe}");
  ''')]),

a("fm06", "Großer Anfang", "Texte",
  "Das Programm liest einen Satz in Kleinbuchstaben ein und schreibt jedes Wort mit großem Anfangsbuchstaben. "
  "Die Wörter sind durch einzelne Leerzeichen getrennt. " + FRAGE +
  "\n\n```\nhallo schöne welt  →  Ergebnis: Hallo Schöne Welt\nc# macht spaß  →  Ergebnis: C# Macht Spaß\n```",
  c('''
    Console.Write("Satz: ");
    string[] woerter = Console.ReadLine().Split(' ');
    for (int i = 0; i < woerter.Length; i++)
    {
        if (woerter[i].Length > 0)
            woerter[i] = char.ToUpper(woerter[i][0]) + woerter[i].Substring(1);
    }
    Console.WriteLine("Ergebnis: " + string.Join(" ", woerter));
  '''),
  "Nimm den Satz Wort für Wort auseinander, ändere jeweils nur den ersten Buchstaben und setz danach alles wieder zusammen.",
  [t(["hallo schöne welt"], "Ergebnis: Hallo Schöne Welt"), t(["c# macht spaß"], "Ergebnis: C# Macht Spaß"),
   t(["übung macht den meister"], "Ergebnis: Übung Macht Den Meister"), t(["a"], "Ergebnis: A")],
  falsch=[c('''
    string satz = Console.ReadLine();
    Console.WriteLine("Ergebnis: " + char.ToUpper(satz[0]) + satz.Substring(1));
  ''')]),

a("fm07", "Einkaufswagen", "Listen",
  "Das Programm liest Artikel ein, bis `fertig` eingegeben wird. Danach gibt es die Anzahl der Artikel aus und listet sie alphabetisch sortiert auf. " + FRAGEN +
  "\n\nBeispiel mit den Eingaben `Milch`, `Brot`, `Eier` und `fertig`:\n\n```\nArtikel: 3\n- Brot\n- Eier\n- Milch\n```\n\n"
  "Wird gleich zu Beginn `fertig` eingegeben, erscheint nur `Artikel: 0`.",
  c('''
    List<string> artikel = new List<string>();
    while (true)
    {
        Console.Write("Artikel: ");
        string eingabe = Console.ReadLine();
        if (eingabe == "fertig") break;
        artikel.Add(eingabe);
    }
    artikel.Sort();
    Console.WriteLine($"Artikel: {artikel.Count}");
    foreach (string a in artikel)
    {
        Console.WriteLine($"- {a}");
    }
  '''),
  "Sammle die Artikel in einer Liste. Fürs Sortieren musst du nichts selbst programmieren.",
  [t(["Milch", "Brot", "Eier", "fertig"], "Artikel: 3\n- Brot\n- Eier\n- Milch"),
   t(["Zucker", "Apfel", "Mehl", "Butter", "fertig"], "Artikel: 4\n- Apfel\n- Butter\n- Mehl\n- Zucker"),
   t(["fertig"], "Artikel: 0")],
  falsch=[c('''
    List<string> artikel = new List<string>();
    while (true)
    {
        string eingabe = Console.ReadLine();
        if (eingabe == "fertig") break;
        artikel.Add(eingabe);
    }
    Console.WriteLine($"Artikel: {artikel.Count}");
    foreach (string a in artikel) Console.WriteLine($"- {a}");
  ''')]),

a("fm08", "Teiler finden", "Schleifen",
  "Das Programm liest eine positive ganze Zahl ein und listet alle ihre Teiler auf, getrennt durch Komma und Leerzeichen. "
  "Nach dem letzten Teiler steht kein Komma mehr. " + FRAGE +
  "\n\n```\n12  →  Teiler von 12: 1, 2, 3, 4, 6, 12\n7  →  Teiler von 7: 1, 7\n1  →  Teiler von 1: 1\n```",
  c('''
    Console.Write("Zahl: ");
    int zahl = int.Parse(Console.ReadLine());
    List<int> teiler = new List<int>();
    for (int i = 1; i <= zahl; i++)
    {
        if (zahl % i == 0)
            teiler.Add(i);
    }
    Console.WriteLine($"Teiler von {zahl}: {string.Join(", ", teiler)}");
  '''),
  "Probiere jede Zahl von 1 bis zur Zahl selbst. Für die Kommas hilft es, die Teiler erst zu sammeln und am Ende gemeinsam auszugeben.",
  [t([12], "Teiler von 12: 1, 2, 3, 4, 6, 12"), t([7], "Teiler von 7: 1, 7"), t([1], "Teiler von 1: 1"),
   t([36], "Teiler von 36: 1, 2, 3, 4, 6, 9, 12, 18, 36")],
  falsch=[c('''
    int zahl = int.Parse(Console.ReadLine());
    string text = $"Teiler von {zahl}: ";
    for (int i = 1; i <= zahl; i++) if (zahl % i == 0) text += i + ", ";
    Console.WriteLine(text);
  '''), c('''
    int zahl = int.Parse(Console.ReadLine());
    List<int> teiler = new List<int>();
    for (int i = 1; i < zahl; i++) if (zahl % i == 0) teiler.Add(i);
    Console.WriteLine($"Teiler von {zahl}: {string.Join(", ", teiler)}");
  ''')]),

a("fm09", "Würfelstatistik", "Arrays",
  "Das Programm liest zehn Würfelergebnisse (1 bis 6) ein und zählt, wie oft jede Augenzahl gefallen ist. " + FRAGEN +
  "\n\nBeispiel für die Würfe `3, 6, 1, 6, 2, 6, 5, 1, 3, 6`:\n\n```\n1: 2-mal\n2: 1-mal\n3: 2-mal\n4: 0-mal\n5: 1-mal\n6: 4-mal\n```",
  c('''
    int[] anzahl = new int[7];
    for (int i = 0; i < 10; i++)
    {
        Console.Write($"Wurf {i + 1}: ");
        int wurf = int.Parse(Console.ReadLine());
        anzahl[wurf]++;
    }
    for (int augen = 1; augen <= 6; augen++)
    {
        Console.WriteLine($"{augen}: {anzahl[augen]}-mal");
    }
  '''),
  "Ein Array mit einem Zähler pro Augenzahl spart dir sechs einzelne Variablen.",
  [t([3, 6, 1, 6, 2, 6, 5, 1, 3, 6], "1: 2-mal\n2: 1-mal\n3: 2-mal\n4: 0-mal\n5: 1-mal\n6: 4-mal"),
   t([1, 1, 1, 1, 1, 2, 2, 2, 4, 4], "1: 5-mal\n2: 3-mal\n3: 0-mal\n4: 2-mal\n5: 0-mal\n6: 0-mal"),
   t([6, 5, 4, 3, 2, 1, 6, 5, 4, 3], "1: 1-mal\n2: 1-mal\n3: 2-mal\n4: 2-mal\n5: 2-mal\n6: 2-mal")],
  falsch=[c('''
    int[] anzahl = new int[7];
    for (int i = 0; i < 10; i++) anzahl[int.Parse(Console.ReadLine())]++;
    for (int augen = 1; augen <= 6; augen++)
        if (anzahl[augen] > 0) Console.WriteLine($"{augen}: {anzahl[augen]}-mal");
  ''')]),

a("fm10", "Bestellmenge", "Sichere Eingabe",
  "Im Onlineshop darf man von einem Artikel 1 bis 10 Stück bestellen. Das Programm fragt so lange nach der Menge, bis eine gültige Zahl kommt. "
  "Ist die Eingabe keine ganze Zahl, meldet es `Das ist keine Zahl.` Liegt sie außerhalb des Bereichs, meldet es `Bitte 1 bis 10 Stück wählen.` "
  "Am Ende bestätigt es die Bestellung. " + FRAGEN +
  "\n\n```\ndrei, 3  →  Das ist keine Zahl. / Bestellt: 3 Stück\n0, 11, 10  →  Bitte 1 bis 10 Stück wählen. (zweimal) / Bestellt: 10 Stück\n5  →  Bestellt: 5 Stück\n```",
  c('''
    int menge;
    while (true)
    {
        Console.Write("Menge: ");
        if (!int.TryParse(Console.ReadLine(), out menge))
            Console.WriteLine("Das ist keine Zahl.");
        else if (menge < 1 || menge > 10)
            Console.WriteLine("Bitte 1 bis 10 Stück wählen.");
        else
            break;
    }
    Console.WriteLine($"Bestellt: {menge} Stück");
  '''),
  "Du brauchst eine Schleife, die erst endet, wenn die Eingabe eine Zahl ist und im Bereich liegt. Die Umwandlung darf dabei nicht abstürzen.",
  [t(["drei", "3"], "Das ist keine Zahl.\nBestellt: 3 Stück"),
   t(["0", "11", "10"], "Bitte 1 bis 10 Stück wählen.\nBitte 1 bis 10 Stück wählen.\nBestellt: 10 Stück"),
   t(["5"], "Bestellt: 5 Stück"),
   t(["-2", "2,5", "7"], "Bitte 1 bis 10 Stück wählen.\nDas ist keine Zahl.\nBestellt: 7 Stück")],
  falsch=[c('''
    int menge;
    while (!int.TryParse(Console.ReadLine(), out menge)) Console.WriteLine("Das ist keine Zahl.");
    Console.WriteLine($"Bestellt: {menge} Stück");
  ''')]),

a("fm11", "Spielfigur", "Klassen",
  "Der Startcode liest einen Namen und vier Zahlen ein und benutzt damit die Klasse `Spielfigur`, die du vervollständigst. "
  "Eine neue Figur hat 100 Lebenspunkte. `Schaden` zieht Punkte ab, `Heilen` gibt welche dazu. "
  "Die Lebenspunkte fallen nie unter 0 und steigen nie über 100. `Anzeigen` gibt den aktuellen Stand aus.\n\n"
  "Beispiel für die Eingaben `Lara`, `30`, `500`, `25` und `1000`:\n\n"
  "```\nLara hat 100 Lebenspunkte.\nLara hat 70 Lebenspunkte.\nLara hat 0 Lebenspunkte.\nLara hat 25 Lebenspunkte.\nLara hat 100 Lebenspunkte.\n```",
  c('''
    string name = Console.ReadLine();
    Spielfigur figur = new Spielfigur(name);
    figur.Anzeigen();
    figur.Schaden(int.Parse(Console.ReadLine()));
    figur.Anzeigen();
    figur.Schaden(int.Parse(Console.ReadLine()));
    figur.Anzeigen();
    figur.Heilen(int.Parse(Console.ReadLine()));
    figur.Anzeigen();
    figur.Heilen(int.Parse(Console.ReadLine()));
    figur.Anzeigen();

    class Spielfigur
    {
        string name;
        int lebenspunkte = 100;

        public Spielfigur(string name)
        {
            this.name = name;
        }

        public void Schaden(int punkte)
        {
            lebenspunkte = Math.Max(0, lebenspunkte - punkte);
        }

        public void Heilen(int punkte)
        {
            lebenspunkte = Math.Min(100, lebenspunkte + punkte);
        }

        public void Anzeigen()
        {
            Console.WriteLine($"{name} hat {lebenspunkte} Lebenspunkte.");
        }
    }
  '''),
  "Prüfe nach jeder Änderung, ob der Wert eine Grenze überschritten hat, und setz ihn in diesem Fall genau auf die Grenze.",
  [t(["Lara", 30, 500, 25, 1000], "Lara hat 100 Lebenspunkte.\nLara hat 70 Lebenspunkte.\nLara hat 0 Lebenspunkte.\nLara hat 25 Lebenspunkte.\nLara hat 100 Lebenspunkte."),
   t(["Kai", 10, 15, 5, 50], "Kai hat 100 Lebenspunkte.\nKai hat 90 Lebenspunkte.\nKai hat 75 Lebenspunkte.\nKai hat 80 Lebenspunkte.\nKai hat 100 Lebenspunkte."),
   t(["Mo", 100, 1, 99, 0], "Mo hat 100 Lebenspunkte.\nMo hat 0 Lebenspunkte.\nMo hat 0 Lebenspunkte.\nMo hat 99 Lebenspunkte.\nMo hat 99 Lebenspunkte.")],
  start=c('''
    string name = Console.ReadLine();
    Spielfigur figur = new Spielfigur(name);
    figur.Anzeigen();
    figur.Schaden(int.Parse(Console.ReadLine()));
    figur.Anzeigen();
    figur.Schaden(int.Parse(Console.ReadLine()));
    figur.Anzeigen();
    figur.Heilen(int.Parse(Console.ReadLine()));
    figur.Anzeigen();
    figur.Heilen(int.Parse(Console.ReadLine()));
    figur.Anzeigen();

    class Spielfigur
    {
        // Was muss sich eine Spielfigur merken?

        public Spielfigur(string name)
        {
        }

        public void Schaden(int punkte)
        {
        }

        public void Heilen(int punkte)
        {
        }

        public void Anzeigen()
        {
        }
    }
  '''),
  anf=[{"pattern": r"class\s+Spielfigur\b", "message": "Lass die Klasse `Spielfigur` im Code stehen und vervollständige sie."}],
  falsch=[c('''
    string name = Console.ReadLine();
    Spielfigur figur = new Spielfigur(name);
    figur.Anzeigen();
    figur.Schaden(int.Parse(Console.ReadLine()));
    figur.Anzeigen();
    figur.Schaden(int.Parse(Console.ReadLine()));
    figur.Anzeigen();
    figur.Heilen(int.Parse(Console.ReadLine()));
    figur.Anzeigen();
    figur.Heilen(int.Parse(Console.ReadLine()));
    figur.Anzeigen();
    class Spielfigur
    {
        string name; int lp = 100;
        public Spielfigur(string name) { this.name = name; }
        public void Schaden(int punkte) { lp = lp - punkte; }
        public void Heilen(int punkte) { lp = lp + punkte; }
        public void Anzeigen() { Console.WriteLine($"{name} hat {lp} Lebenspunkte."); }
    }
  ''')]),

a("fm12", "Längstes Wort", "Texte",
  "Das Programm liest einen Satz ein und findet das längste Wort. Die Wörter sind durch einzelne Leerzeichen getrennt, Satzzeichen kommen nicht vor. "
  "Haben mehrere Wörter die gleiche Länge, gewinnt das erste. " + FRAGE +
  "\n\n```\nIch lerne heute Programmieren  →  Längstes Wort: Programmieren (13 Buchstaben)\nEin Tag am See  →  Längstes Wort: Ein (3 Buchstaben)\n```",
  c('''
    Console.Write("Satz: ");
    string[] woerter = Console.ReadLine().Split(' ');
    string laengstes = woerter[0];
    foreach (string wort in woerter)
    {
        if (wort.Length > laengstes.Length)
            laengstes = wort;
    }
    Console.WriteLine($"Längstes Wort: {laengstes} ({laengstes.Length} Buchstaben)");
  '''),
  "Merk dir das bisher längste Wort. Ein späteres Wort ersetzt es nur, wenn es wirklich länger ist.",
  [t(["Ich lerne heute Programmieren"], "Längstes Wort: Programmieren (13 Buchstaben)"),
   t(["Ein Tag am See"], "Längstes Wort: Ein (3 Buchstaben)"),
   t(["Der Hund bellt laut"], "Längstes Wort: bellt (5 Buchstaben)"),
   t(["Hallo"], "Längstes Wort: Hallo (5 Buchstaben)")],
  falsch=[c('''
    string[] woerter = Console.ReadLine().Split(' ');
    string laengstes = woerter[0];
    foreach (string wort in woerter) if (wort.Length >= laengstes.Length) laengstes = wort;
    Console.WriteLine($"Längstes Wort: {laengstes} ({laengstes.Length} Buchstaben)");
  ''')]),
]

# ============================================================== SCHWER
schwer = [
a("fs01", "Caesar-Verschlüsselung", "Texte",
  "Die Caesar-Verschlüsselung verschiebt jeden Buchstaben um eine feste Anzahl von Stellen im Alphabet. Nach `Z` geht es wieder bei `A` los. "
  "Das Programm liest zuerst einen Text und dann die Verschiebung (0 bis 25) ein. Groß- und Kleinbuchstaben von A bis Z werden verschoben "
  "und behalten ihre Schreibweise. Alle anderen Zeichen (Leerzeichen, Ziffern, Satzzeichen, Umlaute und ß) bleiben, wie sie sind. " + FRAGEN +
  "\n\n- `Hallo Welt` und `3` ergibt `Geheim: Kdoor Zhow`\n- `xyz ABC` und `2` ergibt `Geheim: zab CDE`\n- `C# ist super!` und `13` ergibt `Geheim: P# vfg fhcre!`",
  c('''
    Console.Write("Text: ");
    string text = Console.ReadLine();
    Console.Write("Verschiebung: ");
    int verschiebung = int.Parse(Console.ReadLine());
    string geheim = "";
    foreach (char z in text)
    {
        if (z >= 'a' && z <= 'z')
            geheim += (char)('a' + (z - 'a' + verschiebung) % 26);
        else if (z >= 'A' && z <= 'Z')
            geheim += (char)('A' + (z - 'A' + verschiebung) % 26);
        else
            geheim += z;
    }
    Console.WriteLine($"Geheim: {geheim}");
  '''),
  "Rechne mit der Position des Buchstabens im Alphabet (0 bis 25). Mit dem Rest einer Division durch 26 landest du nach dem Z wieder vorn.",
  [t(["Hallo Welt", 3], "Geheim: Kdoor Zhow"), t(["xyz ABC", 2], "Geheim: zab CDE"), t(["C# ist super!", 13], "Geheim: P# vfg fhcre!"),
   t(["Grüße an Ö", 1], "Geheim: Hsüßf bo Ö"), t(["Abc", 0], "Geheim: Abc")],
  falsch=[c('''
    string text = Console.ReadLine();
    int v = int.Parse(Console.ReadLine());
    string geheim = "";
    foreach (char z in text) geheim += char.IsLetter(z) ? (char)(z + v) : z;
    Console.WriteLine($"Geheim: {geheim}");
  '''), c('''
    string text = Console.ReadLine();
    int v = int.Parse(Console.ReadLine());
    string geheim = "";
    foreach (char z in text)
    {
        if (char.IsLetter(z) && z < 128) geheim += (char)('a' + (char.ToLower(z) - 'a' + v) % 26);
        else geheim += z;
    }
    Console.WriteLine($"Geheim: {geheim}");
  ''')]),

a("fs02", "Primzahlen bis n", "Methoden",
  "Das Programm liest eine Zahl n ein und listet alle Primzahlen von 2 bis n auf, getrennt durch Komma und Leerzeichen. Gibt es keine, erscheint `keine`. "
  "Eine Primzahl ist größer als 1 und nur durch 1 und sich selbst teilbar. " + FRAGE +
  "\n\n```\n30  →  Primzahlen bis 30: 2, 3, 5, 7, 11, 13, 17, 19, 23, 29\n2  →  Primzahlen bis 2: 2\n1  →  Primzahlen bis 1: keine\n```",
  c('''
    Console.Write("n: ");
    int n = int.Parse(Console.ReadLine());
    List<int> primzahlen = new List<int>();
    for (int i = 2; i <= n; i++)
    {
        if (IstPrimzahl(i))
            primzahlen.Add(i);
    }
    string liste = primzahlen.Count > 0 ? string.Join(", ", primzahlen) : "keine";
    Console.WriteLine($"Primzahlen bis {n}: {liste}");

    static bool IstPrimzahl(int zahl)
    {
        if (zahl < 2) return false;
        for (int teiler = 2; teiler * teiler <= zahl; teiler++)
        {
            if (zahl % teiler == 0) return false;
        }
        return true;
    }
  '''),
  "Schreib dir eine eigene Methode, die für eine einzelne Zahl prüft, ob sie eine Primzahl ist. Dann musst du sie nur noch für jede Zahl aufrufen.",
  [t([30], "Primzahlen bis 30: 2, 3, 5, 7, 11, 13, 17, 19, 23, 29"), t([2], "Primzahlen bis 2: 2"), t([1], "Primzahlen bis 1: keine"),
   t([50], "Primzahlen bis 50: 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47"),
   t([97], "Primzahlen bis 97: 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97")],
  falsch=[c('''
    int n = int.Parse(Console.ReadLine());
    List<int> p = new List<int>();
    for (int i = 2; i <= n; i++)
    {
        bool prim = true;
        for (int t = 2; t * t < i; t++) if (i % t == 0) prim = false;
        if (prim) p.Add(i);
    }
    Console.WriteLine($"Primzahlen bis {n}: {(p.Count > 0 ? string.Join(", ", p) : "keine")}");
  '''), c('''
    int n = int.Parse(Console.ReadLine());
    List<int> p = new List<int>();
    for (int i = 2; i < n; i++)
    {
        bool prim = true;
        for (int t = 2; t < i; t++) if (i % t == 0) prim = false;
        if (prim) p.Add(i);
    }
    Console.WriteLine($"Primzahlen bis {n}: {(p.Count > 0 ? string.Join(", ", p) : "keine")}");
  ''')]),

a("fs03", "Klammern prüfen", "Algorithmen",
  "Das Programm liest eine Zeile ein und prüft, ob alle runden, eckigen und geschweiften Klammern richtig gepaart und verschachtelt sind. "
  "Jede öffnende Klammer braucht eine passende schließende, und zwar in der richtigen Reihenfolge. Alle anderen Zeichen spielen keine Rolle. " + FRAGE +
  "\n\n```\n(a[b]{c})  →  Die Klammern sind korrekt.\n([)]  →  Die Klammern sind fehlerhaft.\n((  →  Die Klammern sind fehlerhaft.\n```",
  c('''
    Console.Write("Zeile: ");
    string zeile = Console.ReadLine();
    Stack<char> offen = new Stack<char>();
    bool korrekt = true;
    foreach (char z in zeile)
    {
        if (z == '(' || z == '[' || z == '{')
        {
            offen.Push(z);
        }
        else if (z == ')' || z == ']' || z == '}')
        {
            char erwartet = z == ')' ? '(' : z == ']' ? '[' : '{';
            if (offen.Count == 0 || offen.Pop() != erwartet)
            {
                korrekt = false;
                break;
            }
        }
    }
    if (offen.Count > 0) korrekt = false;
    Console.WriteLine(korrekt ? "Die Klammern sind korrekt." : "Die Klammern sind fehlerhaft.");
  '''),
  "Merk dir die offenen Klammern in der Reihenfolge, in der sie kommen. Jede schließende Klammer muss zur zuletzt geöffneten passen, und am Ende darf nichts offen bleiben.",
  [t(["(a[b]{c})"], "Die Klammern sind korrekt."), t(["([)]"], "Die Klammern sind fehlerhaft."), t(["(("], "Die Klammern sind fehlerhaft."),
   t(["Console.WriteLine(werte[0]);"], "Die Klammern sind korrekt."), t(["}{"], "Die Klammern sind fehlerhaft."),
   t(["{[()()]}"], "Die Klammern sind korrekt."), t(["abc"], "Die Klammern sind korrekt."), t(["(]"], "Die Klammern sind fehlerhaft.")],
  falsch=[c('''
    string zeile = Console.ReadLine();
    int rund = 0, eckig = 0, geschweift = 0;
    foreach (char z in zeile)
    {
        if (z == '(') rund++; if (z == ')') rund--;
        if (z == '[') eckig++; if (z == ']') eckig--;
        if (z == '{') geschweift++; if (z == '}') geschweift--;
    }
    Console.WriteLine(rund == 0 && eckig == 0 && geschweift == 0 ? "Die Klammern sind korrekt." : "Die Klammern sind fehlerhaft.");
  '''), c('''
    string zeile = Console.ReadLine();
    Stack<char> offen = new Stack<char>();
    bool ok = true;
    foreach (char z in zeile)
    {
        if ("([{".IndexOf(z) >= 0) offen.Push(z);
        else if (")]}".IndexOf(z) >= 0)
        {
            if (offen.Count == 0) { ok = false; break; }
            char o = offen.Pop();
            if ("([{".IndexOf(o) != ")]}".IndexOf(z)) { ok = false; break; }
        }
    }
    Console.WriteLine(ok ? "Die Klammern sind korrekt." : "Die Klammern sind fehlerhaft.");
  ''')]),

a("fs04", "Prüfziffer nach Luhn", "Algorithmen",
  "Viele Kartennummern enthalten eine Prüfziffer nach dem Luhn-Verfahren. So prüfst du eine Nummer:\n\n"
  "- Leerzeichen werden ignoriert. Eine Nummer mit nur einer Ziffer oder mit anderen Zeichen als Ziffern ist ungültig.\n"
  "- Von rechts gezählt wird jede zweite Ziffer verdoppelt, also die zweite, vierte, sechste und so weiter. Ist das Ergebnis größer als 9, zieh 9 davon ab.\n"
  "- Zähle alle Ziffern zusammen, die veränderten und die unveränderten. Ist die Summe ohne Rest durch 10 teilbar, ist die Nummer gültig.\n\n"
  + FRAGE +
  "\n\n```\n4539 3195 0343 6467  →  Die Nummer ist gültig.\n8273 1232 7352 0569  →  Die Nummer ist ungültig.\n059  →  Die Nummer ist gültig.\n```",
  c('''
    Console.Write("Nummer: ");
    string nummer = Console.ReadLine().Replace(" ", "");
    bool gueltig = nummer.Length > 1 && nummer.All(char.IsDigit);
    if (gueltig)
    {
        int summe = 0;
        for (int i = 0; i < nummer.Length; i++)
        {
            int ziffer = nummer[nummer.Length - 1 - i] - '0';
            if (i % 2 == 1)
            {
                ziffer *= 2;
                if (ziffer > 9) ziffer -= 9;
            }
            summe += ziffer;
        }
        gueltig = summe % 10 == 0;
    }
    Console.WriteLine(gueltig ? "Die Nummer ist gültig." : "Die Nummer ist ungültig.");
  '''),
  "Geh die Ziffern von hinten nach vorn durch und merk dir, ob die aktuelle Stelle verdoppelt werden muss.",
  [t(["4539 3195 0343 6467"], "Die Nummer ist gültig."), t(["8273 1232 7352 0569"], "Die Nummer ist ungültig."),
   t(["059"], "Die Nummer ist gültig."), t(["0"], "Die Nummer ist ungültig."), t(["055 444 285"], "Die Nummer ist gültig."),
   t(["1234-5678"], "Die Nummer ist ungültig."), t(["091"], "Die Nummer ist gültig."), t(["0000 0"], "Die Nummer ist gültig."),
   t(["059a"], "Die Nummer ist ungültig.")],
  falsch=[c('''
    string nummer = Console.ReadLine().Replace(" ", "");
    int summe = 0;
    for (int i = 0; i < nummer.Length; i++)
    {
        int ziffer = nummer[i] - '0';
        if (i % 2 == 0) { ziffer *= 2; if (ziffer > 9) ziffer -= 9; }
        summe += ziffer;
    }
    bool ok = nummer.Length > 1 && nummer.All(char.IsDigit) && summe % 10 == 0;
    Console.WriteLine(ok ? "Die Nummer ist gültig." : "Die Nummer ist ungültig.");
  '''), c('''
    string nummer = Console.ReadLine().Replace(" ", "");
    int summe = 0;
    for (int i = 0; i < nummer.Length; i++)
    {
        int ziffer = nummer[nummer.Length - 1 - i] - '0';
        if (i % 2 == 1) { ziffer *= 2; if (ziffer > 9) ziffer -= 9; }
        summe += ziffer;
    }
    Console.WriteLine(summe % 10 == 0 ? "Die Nummer ist gültig." : "Die Nummer ist ungültig.");
  ''')]),

a("fs05", "Anagramme", "Texte",
  "Zwei Wörter sind Anagramme, wenn sie aus genau denselben Buchstaben bestehen, nur in anderer Reihenfolge. Groß- und Kleinschreibung spielt keine Rolle. "
  "Ein Wort ist kein Anagramm von sich selbst. Das Programm liest zwei Wörter ein und gibt das Ergebnis aus. " + FRAGEN +
  "\n\n- `Regal` und `Lager` ergibt `Regal und Lager sind Anagramme.`\n- `Hallo` und `Halle` ergibt `Hallo und Halle sind keine Anagramme.`\n"
  "- `Regal` und `regal` ergibt `Regal und regal sind keine Anagramme.`",
  c('''
    Console.Write("Erstes Wort: ");
    string erstes = Console.ReadLine();
    Console.Write("Zweites Wort: ");
    string zweites = Console.ReadLine();
    string a = erstes.ToLower();
    string b = zweites.ToLower();
    char[] buchstabenA = a.ToCharArray();
    char[] buchstabenB = b.ToCharArray();
    Array.Sort(buchstabenA);
    Array.Sort(buchstabenB);
    bool anagramm = a != b && new string(buchstabenA) == new string(buchstabenB);
    Console.WriteLine(anagramm
        ? $"{erstes} und {zweites} sind Anagramme."
        : $"{erstes} und {zweites} sind keine Anagramme.");
  '''),
  "Wenn du die Buchstaben beider Wörter sortierst, kannst du sie leicht vergleichen. Denk vorher an die Groß- und Kleinschreibung.",
  [t(["Regal", "Lager"], "Regal und Lager sind Anagramme."), t(["Hallo", "Halle"], "Hallo und Halle sind keine Anagramme."),
   t(["Regal", "regal"], "Regal und regal sind keine Anagramme."), t(["Listen", "Silent"], "Listen und Silent sind Anagramme."),
   t(["Tor", "Tore"], "Tor und Tore sind keine Anagramme."), t(["Ampel", "Lampe"], "Ampel und Lampe sind Anagramme."),
   t(["Otto", "Toto"], "Otto und Toto sind Anagramme.")],
  falsch=[c('''
    string erstes = Console.ReadLine();
    string zweites = Console.ReadLine();
    char[] x = erstes.ToLower().ToCharArray(); Array.Sort(x);
    char[] y = zweites.ToLower().ToCharArray(); Array.Sort(y);
    bool ok = new string(x) == new string(y);
    Console.WriteLine(ok ? $"{erstes} und {zweites} sind Anagramme." : $"{erstes} und {zweites} sind keine Anagramme.");
  '''), c('''
    string erstes = Console.ReadLine();
    string zweites = Console.ReadLine();
    char[] x = erstes.ToCharArray(); Array.Sort(x);
    char[] y = zweites.ToCharArray(); Array.Sort(y);
    bool ok = erstes != zweites && new string(x) == new string(y);
    Console.WriteLine(ok ? $"{erstes} und {zweites} sind Anagramme." : $"{erstes} und {zweites} sind keine Anagramme.");
  ''')]),

a("fs06", "Worthäufigkeit", "Sammlungen",
  "Das Programm liest einen Satz ein und zählt, wie oft jedes Wort vorkommt. Groß- und Kleinschreibung spielt keine Rolle. "
  "Ausgegeben werden die Wörter kleingeschrieben, in der Reihenfolge, in der sie zum ersten Mal vorkommen. "
  "Die Wörter sind durch einzelne Leerzeichen getrennt, Satzzeichen kommen nicht vor. " + FRAGE +
  "\n\nBeispiel für `Die Katze und der Vogel und die Maus`:\n\n```\n- die: 2\n- katze: 1\n- und: 2\n- der: 1\n- vogel: 1\n- maus: 1\n```",
  c('''
    Console.Write("Satz: ");
    string[] woerter = Console.ReadLine().ToLower().Split(' ');
    List<string> reihenfolge = new List<string>();
    Dictionary<string, int> anzahl = new Dictionary<string, int>();
    foreach (string wort in woerter)
    {
        if (anzahl.ContainsKey(wort))
        {
            anzahl[wort]++;
        }
        else
        {
            anzahl[wort] = 1;
            reihenfolge.Add(wort);
        }
    }
    foreach (string wort in reihenfolge)
    {
        Console.WriteLine($"- {wort}: {anzahl[wort]}");
    }
  '''),
  "Zu jedem Wort brauchst du einen Zähler. C# hat dafür eine passende Sammlung, die zu jedem Schlüssel einen Wert speichert.",
  [t(["Die Katze und der Vogel und die Maus"], "- die: 2\n- katze: 1\n- und: 2\n- der: 1\n- vogel: 1\n- maus: 1"),
   t(["eins zwei Eins ZWEI drei"], "- eins: 2\n- zwei: 2\n- drei: 1"),
   t(["Hallo"], "- hallo: 1")],
  falsch=[c('''
    string[] woerter = Console.ReadLine().Split(' ');
    Dictionary<string, int> anzahl = new Dictionary<string, int>();
    foreach (string w in woerter) anzahl[w] = anzahl.GetValueOrDefault(w) + 1;
    foreach (var paar in anzahl) Console.WriteLine($"- {paar.Key.ToLower()}: {paar.Value}");
  ''')]),

a("fs07", "Lauflängenkodierung", "Texte",
  "Die Lauflängenkodierung fasst gleiche Zeichen zusammen, die direkt hintereinander stehen: Vor jedes Zeichen kommt die Anzahl seiner Wiederholungen. "
  "Groß- und Kleinbuchstaben gelten als verschiedene Zeichen. Das Programm liest einen Text ohne Ziffern ein und gibt ihn komprimiert aus. " + FRAGE +
  "\n\n```\nAAABBC  →  Komprimiert: 3A2B1C\nx  →  Komprimiert: 1x\naaAA  →  Komprimiert: 2a2A\n```",
  c('''
    Console.Write("Text: ");
    string text = Console.ReadLine();
    string ergebnis = "";
    int i = 0;
    while (i < text.Length)
    {
        char zeichen = text[i];
        int anzahl = 0;
        while (i < text.Length && text[i] == zeichen)
        {
            anzahl++;
            i++;
        }
        ergebnis += anzahl.ToString() + zeichen;
    }
    Console.WriteLine($"Komprimiert: {ergebnis}");
  '''),
  "Geh den Text Zeichen für Zeichen durch und zähle, solange das nächste Zeichen gleich bleibt. Vergiss die letzte Gruppe am Ende nicht.",
  [t(["AAABBC"], "Komprimiert: 3A2B1C"), t(["x"], "Komprimiert: 1x"), t(["aaAA"], "Komprimiert: 2a2A"),
   t(["WWWWWWWWWWWWBWWWWWWWWWWWWBBB"], "Komprimiert: 12W1B12W3B"), t(["ABAB"], "Komprimiert: 1A1B1A1B")],
  falsch=[c('''
    string text = Console.ReadLine();
    string ergebnis = "";
    int anzahl = 1;
    for (int i = 1; i < text.Length; i++)
    {
        if (text[i] == text[i - 1]) anzahl++;
        else { ergebnis += anzahl.ToString() + text[i - 1]; anzahl = 1; }
    }
    Console.WriteLine($"Komprimiert: {ergebnis}");
  '''), c('''
    string text = Console.ReadLine();
    string ergebnis = "";
    foreach (char z in text.Distinct()) ergebnis += text.Count(x => x == z).ToString() + z;
    Console.WriteLine($"Komprimiert: {ergebnis}");
  ''')]),

a("fs08", "Brüche addieren", "Klassen",
  "Der Startcode liest zwei Brüche ein (jeweils erst den Zähler, dann den Nenner) und benutzt die Klasse `Bruch`, die du vervollständigst. "
  "`Addiere` liefert die Summe zweier Brüche als neuen, vollständig gekürzten Bruch. Als Text sieht ein Bruch so aus: `Zähler/Nenner`. "
  "Die beiden Ausgangsbrüche erscheinen so, wie sie eingegeben wurden. Nur das Ergebnis wird gekürzt. Auch ein Ergebnis wie `1/1` bleibt in dieser Form. "
  "Alle Eingaben sind positive ganze Zahlen.\n\n"
  "- `1`, `2`, `1`, `3` ergibt `1/2 + 1/3 = 5/6`\n- `2`, `3`, `4`, `6` ergibt `2/3 + 4/6 = 4/3`\n- `3`, `8`, `5`, `8` ergibt `3/8 + 5/8 = 1/1`",
  c('''
    int z1 = int.Parse(Console.ReadLine());
    int n1 = int.Parse(Console.ReadLine());
    int z2 = int.Parse(Console.ReadLine());
    int n2 = int.Parse(Console.ReadLine());

    Bruch a = new Bruch(z1, n1);
    Bruch b = new Bruch(z2, n2);
    Console.WriteLine($"{a} + {b} = {a.Addiere(b)}");

    class Bruch
    {
        public int Zaehler { get; }
        public int Nenner { get; }

        public Bruch(int zaehler, int nenner)
        {
            Zaehler = zaehler;
            Nenner = nenner;
        }

        public Bruch Addiere(Bruch anderer)
        {
            int zaehler = Zaehler * anderer.Nenner + anderer.Zaehler * Nenner;
            int nenner = Nenner * anderer.Nenner;
            int teiler = Ggt(zaehler, nenner);
            return new Bruch(zaehler / teiler, nenner / teiler);
        }

        static int Ggt(int a, int b)
        {
            while (b != 0)
            {
                int rest = a % b;
                a = b;
                b = rest;
            }
            return a;
        }

        public override string ToString()
        {
            return $"{Zaehler}/{Nenner}";
        }
    }
  '''),
  "Bring beide Brüche auf denselben Nenner. Danach teilst du Zähler und Nenner durch ihren größten gemeinsamen Teiler.",
  [t([1, 2, 1, 3], "1/2 + 1/3 = 5/6"), t([1, 4, 1, 4], "1/4 + 1/4 = 1/2"), t([2, 3, 4, 6], "2/3 + 4/6 = 4/3"),
   t([5, 12, 1, 4], "5/12 + 1/4 = 2/3"), t([3, 8, 5, 8], "3/8 + 5/8 = 1/1"), t([7, 10, 2, 15], "7/10 + 2/15 = 5/6")],
  start=c('''
    int z1 = int.Parse(Console.ReadLine());
    int n1 = int.Parse(Console.ReadLine());
    int z2 = int.Parse(Console.ReadLine());
    int n2 = int.Parse(Console.ReadLine());

    Bruch a = new Bruch(z1, n1);
    Bruch b = new Bruch(z2, n2);
    Console.WriteLine($"{a} + {b} = {a.Addiere(b)}");

    class Bruch
    {
        public int Zaehler { get; }
        public int Nenner { get; }

        public Bruch(int zaehler, int nenner)
        {
            Zaehler = zaehler;
            Nenner = nenner;
        }

        public Bruch Addiere(Bruch anderer)
        {
            // Schreib hier deinen Code
            return this;
        }

        public override string ToString()
        {
            // Schreib hier deinen Code
            return "";
        }
    }
  '''),
  anf=[{"pattern": r"class\s+Bruch\b", "message": "Lass die Klasse `Bruch` im Code stehen und vervollständige sie."}],
  falsch=[c('''
    int z1 = int.Parse(Console.ReadLine());
    int n1 = int.Parse(Console.ReadLine());
    int z2 = int.Parse(Console.ReadLine());
    int n2 = int.Parse(Console.ReadLine());
    Bruch a = new Bruch(z1, n1);
    Bruch b = new Bruch(z2, n2);
    Console.WriteLine($"{a} + {b} = {a.Addiere(b)}");
    class Bruch
    {
        public int Zaehler { get; } public int Nenner { get; }
        public Bruch(int z, int n) { Zaehler = z; Nenner = n; }
        public Bruch Addiere(Bruch o) => new Bruch(Zaehler * o.Nenner + o.Zaehler * Nenner, Nenner * o.Nenner);
        public override string ToString() => $"{Zaehler}/{Nenner}";
    }
  '''), c('''
    int z1 = int.Parse(Console.ReadLine());
    int n1 = int.Parse(Console.ReadLine());
    int z2 = int.Parse(Console.ReadLine());
    int n2 = int.Parse(Console.ReadLine());
    Bruch a = new Bruch(z1, n1);
    Bruch b = new Bruch(z2, n2);
    Console.WriteLine($"{a} + {b} = {a.Addiere(b)}");
    class Bruch
    {
        public int Zaehler { get; } public int Nenner { get; }
        public Bruch(int z, int n) { Zaehler = z; Nenner = n; }
        public Bruch Addiere(Bruch o) => new Bruch(Zaehler + o.Zaehler, Nenner + o.Nenner);
        public override string ToString() => $"{Zaehler}/{Nenner}";
    }
  ''')]),

a("fs09", "Binärzahlen", "Algorithmen",
  "Das Programm liest eine ganze Zahl ab 0 ein und gibt sie im Binärsystem (Zweiersystem) aus. Rechne die Ziffern selbst aus, ohne eine fertige Umwandlungsfunktion. " + FRAGE +
  "\n\n```\n13  →  Binär: 1101\n0  →  Binär: 0\n255  →  Binär: 11111111\n```",
  c('''
    Console.Write("Zahl: ");
    int zahl = int.Parse(Console.ReadLine());
    string binaer = "";
    do
    {
        binaer = (zahl % 2) + binaer;
        zahl = zahl / 2;
    } while (zahl > 0);
    Console.WriteLine($"Binär: {binaer}");
  '''),
  "Teil die Zahl immer wieder durch 2, bis nichts mehr übrig ist. Die Reste ergeben von hinten gelesen die Binärziffern. Denk auch an die 0.",
  [t([13], "Binär: 1101"), t([0], "Binär: 0"), t([255], "Binär: 11111111"), t([1], "Binär: 1"), t([6], "Binär: 110"), t([1024], "Binär: 10000000000")],
  anf=[{"pattern": r"^(?![\s\S]*Convert\s*\.\s*ToString\s*\([^;]*,\s*2\s*\))(?![\s\S]*ToString\s*\(\s*\"[bB]\d*\"\s*\))(?![\s\S]*:\s*[bB]\d*\s*\})",
        "message": "Rechne die Binärziffern selbst aus, ohne fertige Umwandlung."}],
  falsch=[c('''
    int zahl = int.Parse(Console.ReadLine());
    string binaer = "";
    while (zahl > 0) { binaer = (zahl % 2) + binaer; zahl = zahl / 2; }
    Console.WriteLine($"Binär: {binaer}");
  '''), c('''
    int zahl = int.Parse(Console.ReadLine());
    string binaer = "";
    do { binaer = binaer + (zahl % 2); zahl = zahl / 2; } while (zahl > 0);
    Console.WriteLine($"Binär: {binaer}");
  '''), c('''
    int zahl = int.Parse(Console.ReadLine());
    Console.WriteLine($"Binär: {Convert.ToString(zahl, 2)}");
  '''), c('''
    int zahl = int.Parse(Console.ReadLine());
    Console.WriteLine($"Binär: {zahl.ToString("B")}");
  '''), c('''
    int zahl = int.Parse(Console.ReadLine());
    Console.WriteLine($"Binär: {zahl:B}");
  ''')]),

a("fs10", "Tic-Tac-Toe", "Arrays & Logik",
  "Das Programm liest ein fertiges Tic-Tac-Toe-Feld ein: drei Zeilen mit je drei Zeichen. `X` steht für Spieler 1, `O` für Spieler 2 und `.` für ein leeres Feld. "
  "Wer drei eigene Zeichen in einer Reihe, einer Spalte oder einer Diagonale hat, gewinnt. Gibt es keinen Gewinner, meldet das Programm ein Unentschieden. "
  "Jedes Feld hat höchstens einen Gewinner. " + FRAGEN +
  "\n\n- `XXO`, `OXO` und `OOX` ergibt `Gewinner: Spieler 1`\n- `XOX`, `.O.` und `XOX` ergibt `Gewinner: Spieler 2`\n- `XOX`, `XOO` und `OXX` ergibt `Unentschieden`",
  c('''
    char[,] feld = new char[3, 3];
    for (int zeile = 0; zeile < 3; zeile++)
    {
        Console.Write($"Zeile {zeile + 1}: ");
        string eingabe = Console.ReadLine();
        for (int spalte = 0; spalte < 3; spalte++)
            feld[zeile, spalte] = eingabe[spalte];
    }

    char gewinner = '.';
    for (int i = 0; i < 3; i++)
    {
        if (Gleich(feld[i, 0], feld[i, 1], feld[i, 2])) gewinner = feld[i, 0];
        if (Gleich(feld[0, i], feld[1, i], feld[2, i])) gewinner = feld[0, i];
    }
    if (Gleich(feld[0, 0], feld[1, 1], feld[2, 2])) gewinner = feld[1, 1];
    if (Gleich(feld[0, 2], feld[1, 1], feld[2, 0])) gewinner = feld[1, 1];

    if (gewinner == 'X') Console.WriteLine("Gewinner: Spieler 1");
    else if (gewinner == 'O') Console.WriteLine("Gewinner: Spieler 2");
    else Console.WriteLine("Unentschieden");

    static bool Gleich(char a, char b, char c) => a != '.' && a == b && b == c;
  '''),
  "Es gibt genau acht mögliche Gewinnlinien: drei Reihen, drei Spalten und zwei Diagonalen. Prüfe jede einzelne. Drei leere Felder sind keine Gewinnlinie.",
  [t(["XXO", "OXO", "OOX"], "Gewinner: Spieler 1"), t(["XOX", ".O.", "XOX"], "Gewinner: Spieler 2"), t(["XOX", "XOO", "OXX"], "Unentschieden"),
   t(["OOO", "XX.", "X.."], "Gewinner: Spieler 2"), t(["O.X", "OX.", "X.."], "Gewinner: Spieler 1"), t(["XO.", "X.O", "X.."], "Gewinner: Spieler 1"),
   t(["...", "...", "..."], "Unentschieden")],
  falsch=[c('''
    string[] z = { Console.ReadLine(), Console.ReadLine(), Console.ReadLine() };
    char g = '.';
    for (int i = 0; i < 3; i++)
    {
        if (z[i][0] == z[i][1] && z[i][1] == z[i][2]) g = z[i][0];
        if (z[0][i] == z[1][i] && z[1][i] == z[2][i]) g = z[0][i];
    }
    if (z[0][0] == z[1][1] && z[1][1] == z[2][2]) g = z[1][1];
    Console.WriteLine(g == 'X' ? "Gewinner: Spieler 1" : g == 'O' ? "Gewinner: Spieler 2" : "Unentschieden");
  '''), c('''
    string[] z = { Console.ReadLine(), Console.ReadLine(), Console.ReadLine() };
    char g = '.';
    for (int i = 0; i < 3; i++)
    {
        if (z[i][0] == z[i][1] && z[i][1] == z[i][2]) g = z[i][0];
    }
    if (z[0][0] == z[1][1] && z[1][1] == z[2][2]) g = z[1][1];
    if (z[0][2] == z[1][1] && z[1][1] == z[2][0]) g = z[1][1];
    Console.WriteLine(g == 'X' ? "Gewinner: Spieler 1" : g == 'O' ? "Gewinner: Spieler 2" : "Unentschieden");
  ''')]),

a("fs11", "Rangliste", "Listen & Objekte",
  "Das Programm liest Ergebnisse im Format `Name Punkte` ein, bis eine leere Zeile kommt. Danach gibt es eine Rangliste aus: "
  "die meisten Punkte zuerst, bei Gleichstand alphabetisch nach dem Namen. Die Plätze werden einfach durchnummeriert. " + FRAGEN +
  "\n\nBeispiel für die Eingaben `Mia 95`, `Ben 80`, `Anna 80`, `Tom 99` und eine leere Zeile:\n\n"
  "```\n1. Tom (99 Punkte)\n2. Mia (95 Punkte)\n3. Anna (80 Punkte)\n4. Ben (80 Punkte)\n```",
  c('''
    List<Ergebnis> liste = new List<Ergebnis>();
    while (true)
    {
        Console.Write("Name und Punkte: ");
        string zeile = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(zeile)) break;
        string[] teile = zeile.Split(' ');
        liste.Add(new Ergebnis(teile[0], int.Parse(teile[1])));
    }

    liste.Sort((a, b) => a.Punkte != b.Punkte
        ? b.Punkte.CompareTo(a.Punkte)
        : string.Compare(a.Name, b.Name, StringComparison.Ordinal));

    for (int i = 0; i < liste.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {liste[i].Name} ({liste[i].Punkte} Punkte)");
    }

    class Ergebnis
    {
        public string Name { get; }
        public int Punkte { get; }

        public Ergebnis(string name, int punkte)
        {
            Name = name;
            Punkte = punkte;
        }
    }
  '''),
  "Speichere jeden Eintrag als kleines Objekt mit Name und Punkten. Beim Sortieren vergleichst du erst die Punkte und nur bei Gleichstand die Namen.",
  [t(["Mia 95", "Ben 80", "Anna 80", "Tom 99", ""], "1. Tom (99 Punkte)\n2. Mia (95 Punkte)\n3. Anna (80 Punkte)\n4. Ben (80 Punkte)"),
   t(["Zoe 50", "Ole 50", "Uwe 70", ""], "1. Uwe (70 Punkte)\n2. Ole (50 Punkte)\n3. Zoe (50 Punkte)"),
   t(["Solo 7", ""], "1. Solo (7 Punkte)"),
   t(["Lea 12", "Kim 30", "Jan 12", "Ida 30", "Max 5", ""], "1. Ida (30 Punkte)\n2. Kim (30 Punkte)\n3. Jan (12 Punkte)\n4. Lea (12 Punkte)\n5. Max (5 Punkte)")],
  falsch=[c('''
    List<(string Name, int Punkte)> liste = new List<(string, int)>();
    while (true)
    {
        string zeile = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(zeile)) break;
        string[] t = zeile.Split(' ');
        liste.Add((t[0], int.Parse(t[1])));
    }
    liste = liste.OrderByDescending(e => e.Punkte).ToList();
    for (int i = 0; i < liste.Count; i++) Console.WriteLine($"{i + 1}. {liste[i].Name} ({liste[i].Punkte} Punkte)");
  ''')]),

a("fs12", "Zeitrechner", "Rechnen & Texte",
  "Das Programm liest eine Start- und eine Endzeit im Format `HH:MM` ein und berechnet die Dauer dazwischen. "
  "Liegt die Endzeit vor der Startzeit, geht es über Mitternacht. Gleiche Zeiten ergeben eine Dauer von 0. " + FRAGEN +
  "\n\n```\n08:15 bis 11:00  →  Dauer: 2 Std. 45 Min.\n22:30 bis 01:10  →  Dauer: 2 Std. 40 Min.\n12:00 bis 12:00  →  Dauer: 0 Std. 0 Min.\n```",
  c('''
    Console.Write("Start: ");
    int start = InMinuten(Console.ReadLine());
    Console.Write("Ende: ");
    int ende = InMinuten(Console.ReadLine());
    int dauer = ende - start;
    if (dauer < 0) dauer += 24 * 60;
    Console.WriteLine($"Dauer: {dauer / 60} Std. {dauer % 60} Min.");

    static int InMinuten(string zeit)
    {
        string[] teile = zeit.Split(':');
        return int.Parse(teile[0]) * 60 + int.Parse(teile[1]);
    }
  '''),
  "Rechne beide Zeiten in Minuten seit Mitternacht um. Dann ist es nur noch eine Differenz, und für den Weg über Mitternacht kommt ein ganzer Tag dazu.",
  [t(["08:15", "11:00"], "Dauer: 2 Std. 45 Min."), t(["22:30", "01:10"], "Dauer: 2 Std. 40 Min."), t(["12:00", "12:00"], "Dauer: 0 Std. 0 Min."),
   t(["00:00", "23:59"], "Dauer: 23 Std. 59 Min."), t(["09:50", "10:05"], "Dauer: 0 Std. 15 Min."), t(["23:59", "00:00"], "Dauer: 0 Std. 1 Min.")],
  falsch=[c('''
    string[] s = Console.ReadLine().Split(':');
    string[] e = Console.ReadLine().Split(':');
    int stunden = int.Parse(e[0]) - int.Parse(s[0]);
    int minuten = int.Parse(e[1]) - int.Parse(s[1]);
    Console.WriteLine($"Dauer: {stunden} Std. {minuten} Min.");
  '''), c('''
    string[] s = Console.ReadLine().Split(':');
    string[] e = Console.ReadLine().Split(':');
    int dauer = int.Parse(e[0]) * 60 + int.Parse(e[1]) - (int.Parse(s[0]) * 60 + int.Parse(s[1]));
    dauer = Math.Abs(dauer);
    Console.WriteLine($"Dauer: {dauer / 60} Std. {dauer % 60} Min.");
  ''')]),
]

katalog = {
    "levels": [
        {"id": "leicht", "title": "Leicht",
         "description": "Ausgabe, Rechnen, erste Entscheidungen und Schleifen. Ideal zum Aufwärmen.",
         "exercises": leicht},
        {"id": "mittel", "title": "Mittel",
         "description": "Schleifen, Arrays, Listen, Methoden und Texte. Hier wird kombiniert.",
         "exercises": mittel},
        {"id": "schwer", "title": "Schwer",
         "description": "Kleine Algorithmen, eigene Klassen und knifflige Texte. Für alle, die es wissen wollen.",
         "exercises": schwer},
    ]
}

if __name__ == "__main__":
    ziel = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "..", "src", "CSharpLernen.Engine", "Inhalte", "FreieAufgaben.json")
    with open(os.path.normpath(ziel), "w", encoding="utf-8", newline="\n") as f:
        json.dump(katalog, f, ensure_ascii=False, indent=1)
        f.write("\n")
    print("geschrieben:", os.path.normpath(ziel), "–", ", ".join(f"{l['title']}: {len(l['exercises'])}" for l in katalog["levels"]))
