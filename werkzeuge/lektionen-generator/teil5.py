# Stufe 5: Projekte
from gen import L, A, T, Q

L(5, "l25", "Projekt: Notenrechner",
  intro="Dein erstes kleines Projekt: Ein Programm, das Noten einliest, auswertet und ein Zeugnis ausgibt. Hier kommt alles zusammen.",
  explanation=r'''
## Was das Programm können soll
1. Fragen, wie viele Noten eingegeben werden.
2. Die Noten einlesen und in einer **Liste** speichern.
3. Durchschnitt, beste und schlechteste Note berechnen.
4. Eine Bewertung ausgeben.

## Vorgehen wie ein Profi
- Teile das Problem in **kleine Schritte** – jede Aufgabe unten ist ein Schritt.
- Teste nach jedem Schritt, ob alles läuft.
- Lagere Berechnungen in **Methoden** aus.

## Was du brauchst
`List<int>`, Schleifen, `int.Parse`, `if`/`else`, Methoden mit Rückgabewert und String-Interpolation mit `:F2`.
''',
  example=r'''List<int> noten = new List<int> { 2, 1, 3, 2 };

Console.WriteLine("Noten: " + string.Join(", ", noten));
Console.WriteLine($"Durchschnitt: {Durchschnitt(noten):F2}");

static double Durchschnitt(List<int> liste)
{
    double summe = 0;
    foreach (int n in liste)
        summe += n;
    return summe / liste.Count;
}
''',
  note="Profis schreiben auch nicht alles auf einmal. Bau das Programm Stück für Stück und führe es zwischendurch immer wieder aus.",
  exercises=[
    A("Schritt 1: Noten einlesen", r'''Lies zuerst die Anzahl der Noten ein, dann die Noten selbst, und speichere sie in einer `List<int>`. Gib sie danach so aus (bei `3`, `2`, `1`, `4`):

```
Deine Noten: 2, 1, 4
```''',
      starter='List<int> noten = new List<int>();\n',
      solution='List<int> noten = new List<int>();\nint anzahl = int.Parse(Console.ReadLine());\nfor (int i = 0; i < anzahl; i++)\n{\n    noten.Add(int.Parse(Console.ReadLine()));\n}\nConsole.WriteLine("Deine Noten: " + string.Join(", ", noten));\n',
      tests=[T("Deine Noten: 2, 1, 4", "3", "2", "1", "4"), T("Deine Noten: 5", "1", "5")], compare="lenient",
      hint='string.Join(", ", noten) verbindet alle Noten mit Komma.'),
    A("Schritt 2: Auswertung", r'''Ergänze Methoden `Durchschnitt`, `Beste` (kleinste Zahl) und `Schlechteste` (größte Zahl) und gib die Auswertung aus. Bei `4`, `2`, `1`, `3`, `2`:

```
Durchschnitt: 2,00
Beste Note: 1
Schlechteste Note: 3
```''',
      starter='List<int> noten = new List<int>();\nint anzahl = int.Parse(Console.ReadLine());\nfor (int i = 0; i < anzahl; i++)\n{\n    noten.Add(int.Parse(Console.ReadLine()));\n}\n\n',
      solution='List<int> noten = new List<int>();\nint anzahl = int.Parse(Console.ReadLine());\nfor (int i = 0; i < anzahl; i++)\n{\n    noten.Add(int.Parse(Console.ReadLine()));\n}\n\nConsole.WriteLine($"Durchschnitt: {Durchschnitt(noten):F2}");\nConsole.WriteLine($"Beste Note: {Beste(noten)}");\nConsole.WriteLine($"Schlechteste Note: {Schlechteste(noten)}");\n\nstatic double Durchschnitt(List<int> liste)\n{\n    double summe = 0;\n    foreach (int n in liste) summe += n;\n    return summe / liste.Count;\n}\n\nstatic int Beste(List<int> liste)\n{\n    int beste = liste[0];\n    foreach (int n in liste) if (n < beste) beste = n;\n    return beste;\n}\n\nstatic int Schlechteste(List<int> liste)\n{\n    int schlechteste = liste[0];\n    foreach (int n in liste) if (n > schlechteste) schlechteste = n;\n    return schlechteste;\n}\n',
      tests=[T("Durchschnitt: 2,00\nBeste Note: 1\nSchlechteste Note: 3", "4", "2", "1", "3", "2"), T("Durchschnitt: 3,67\nBeste Note: 2\nSchlechteste Note: 5", "3", "5", "4", "2")], compare="lenient"),
    A("Schritt 3: Zeugnis", r'''Gib zusätzlich eine Bewertung nach dem Durchschnitt aus:
- bis 1,5 → `Ausgezeichnet!`
- bis 2,5 → `Gut gemacht!`
- bis 3,5 → `Befriedigend.`
- darüber → `Da geht noch was!`

Lies wie in Schritt 1 die Noten ein und gib aus (bei `2`, `1`, `2`):
```
Durchschnitt: 1,50
Ausgezeichnet!
```''',
      starter='List<int> noten = new List<int>();\nint anzahl = int.Parse(Console.ReadLine());\nfor (int i = 0; i < anzahl; i++)\n{\n    noten.Add(int.Parse(Console.ReadLine()));\n}\n',
      solution='List<int> noten = new List<int>();\nint anzahl = int.Parse(Console.ReadLine());\nfor (int i = 0; i < anzahl; i++)\n{\n    noten.Add(int.Parse(Console.ReadLine()));\n}\n\ndouble schnitt = noten.Average();\nConsole.WriteLine($"Durchschnitt: {schnitt:F2}");\nConsole.WriteLine(Bewertung(schnitt));\n\nstatic string Bewertung(double schnitt)\n{\n    if (schnitt <= 1.5) return "Ausgezeichnet!";\n    if (schnitt <= 2.5) return "Gut gemacht!";\n    if (schnitt <= 3.5) return "Befriedigend.";\n    return "Da geht noch was!";\n}\n',
      tests=[T("Durchschnitt: 1,50\nAusgezeichnet!", "2", "1", "2"), T("Durchschnitt: 2,50\nGut gemacht!", "2", "2", "3"), T("Durchschnitt: 3,00\nBefriedigend.", "1", "3"), T("Durchschnitt: 4,50\nDa geht noch was!", "2", "4", "5")], compare="lenient"),
  ],
  quiz=[
    Q("Was ist der beste erste Schritt bei einem größeren Programm?", ["Alles auf einmal schreiben", "Das Problem in kleine Schritte zerlegen", "Sofort optimieren", "Viele Kommentare löschen"], 1, ""),
    Q("Welche Methode liefert direkt den Durchschnitt einer List<int>?", [".Sum()", ".Average()", ".Mean()", ".Count"], 1, ""),
  ])

L(5, "l26", "Projekt: Zahlenraten",
  intro="Ein echtes kleines Spiel: Der Computer denkt sich eine Zahl aus, du rätst – und bekommst Tipps „zu groß“ oder „zu klein“.",
  explanation=r'''
## Spielablauf
1. Der Computer wählt eine geheime Zahl zwischen 1 und 100.
2. Der Spieler rät.
3. Das Programm sagt `Zu klein!` oder `Zu groß!`.
4. Bei einem Treffer: Glückwunsch und die Anzahl der Versuche.

## Zufallszahlen
Im echten Spiel nimmst du eine Zufallszahl:
```
int geheim = Random.Shared.Next(1, 101);   // 1 bis 100
```
Die obere Grenze (101) ist **nicht** mit dabei.

Damit die App deine Lösung automatisch prüfen kann, ist die geheime Zahl in den Aufgaben **fest vorgegeben**. Tausch sie in der Spielwiese gern gegen `Random.Shared.Next(1, 101)` aus!
''',
  example=r'''int geheim = Random.Shared.Next(1, 101);
Console.WriteLine("Ich habe mir eine Zahl zwischen 1 und 100 gedacht.");
Console.WriteLine("(Psst, es ist die " + geheim + ")");
''',
  note="Probier das fertige Spiel danach in der Spielwiese mit einer echten Zufallszahl aus – und lass Freunde raten!",
  exercises=[
    A("Ein Versuch", r'''Die geheime Zahl ist `37`. Lies **einen** Tipp ein und gib `Zu klein!`, `Zu groß!` oder `Richtig!` aus.''',
      starter='int geheim = 37;\n',
      solution='int geheim = 37;\nint tipp = int.Parse(Console.ReadLine());\nif (tipp < geheim)\n    Console.WriteLine("Zu klein!");\nelse if (tipp > geheim)\n    Console.WriteLine("Zu groß!");\nelse\n    Console.WriteLine("Richtig!");\n',
      tests=[T("Zu klein!", "10"), T("Zu groß!", "80"), T("Richtig!", "37")], compare="lenient"),
    A("Raten bis zum Treffer", r'''Lass so lange raten, bis die Zahl `37` getroffen ist. Gib nach jedem Tipp `Zu klein!`/`Zu groß!` aus und am Ende `Richtig! Du hast 3 Versuche gebraucht.` Bei `50`, `25`, `37`:

```
Zu groß!
Zu klein!
Richtig! Du hast 3 Versuche gebraucht.
```''',
      starter='int geheim = 37;\nint versuche = 0;\n',
      solution='int geheim = 37;\nint versuche = 0;\nint tipp = 0;\nwhile (tipp != geheim)\n{\n    tipp = int.Parse(Console.ReadLine());\n    versuche++;\n    if (tipp < geheim)\n        Console.WriteLine("Zu klein!");\n    else if (tipp > geheim)\n        Console.WriteLine("Zu groß!");\n}\nConsole.WriteLine($"Richtig! Du hast {versuche} Versuche gebraucht.");\n',
      tests=[T("Zu groß!\nZu klein!\nRichtig! Du hast 3 Versuche gebraucht.", "50", "25", "37"), T("Richtig! Du hast 1 Versuche gebraucht.", "37")], compare="lenient"),
    A("Mit Schutz und Limit", r'''Erweitere das Spiel:
- Ungültige Eingaben (keine Zahl) → `Das ist keine Zahl!` (zählt nicht als Versuch). Benutze `int.TryParse`.
- Nach **5** Versuchen ohne Treffer ist Schluss: `Verloren! Die Zahl war 37.`
- Bei Treffer: `Richtig! Du hast 2 Versuche gebraucht.`

Bei `50`, `abc`, `37`:
```
Zu groß!
Das ist keine Zahl!
Richtig! Du hast 2 Versuche gebraucht.
```''',
      starter='int geheim = 37;\nint versuche = 0;\nconst int maxVersuche = 5;\n',
      solution='int geheim = 37;\nint versuche = 0;\nconst int maxVersuche = 5;\nbool gewonnen = false;\n\nwhile (versuche < maxVersuche && !gewonnen)\n{\n    if (!int.TryParse(Console.ReadLine(), out int tipp))\n    {\n        Console.WriteLine("Das ist keine Zahl!");\n        continue;\n    }\n    versuche++;\n    if (tipp < geheim) Console.WriteLine("Zu klein!");\n    else if (tipp > geheim) Console.WriteLine("Zu groß!");\n    else gewonnen = true;\n}\n\nif (gewonnen)\n    Console.WriteLine($"Richtig! Du hast {versuche} Versuche gebraucht.");\nelse\n    Console.WriteLine($"Verloren! Die Zahl war {geheim}.");\n',
      tests=[T("Zu groß!\nDas ist keine Zahl!\nRichtig! Du hast 2 Versuche gebraucht.", "50", "abc", "37"),
             T("Zu klein!\nZu klein!\nZu klein!\nZu klein!\nZu klein!\nVerloren! Die Zahl war 37.", "1", "2", "3", "4", "5", "37")], compare="lenient",
      requirements=[(r'TryParse', "Benutze int.TryParse.")],
      hint="continue; springt sofort zum nächsten Schleifendurchlauf."),
  ],
  quiz=[
    Q("Welche Zahlen liefert Random.Shared.Next(1, 7)?", ["0 bis 7", "1 bis 7", "1 bis 6", "0 bis 6"], 2, "Die obere Grenze ist nicht enthalten."),
    Q("Was macht continue; in einer Schleife?", ["Beendet die Schleife", "Springt zum nächsten Durchlauf", "Pausiert", "Startet das Programm neu"], 1, ""),
  ])

L(5, "l27", "Projekt: Einkaufsliste mit Menü",
  intro="Zum Abschluss baust du eine kleine Anwendung mit Menü – so wie viele echte Konsolenprogramme aufgebaut sind.",
  explanation=r'''
## Das Menü-Muster
Viele Programme laufen in einer **Hauptschleife**: Menü zeigen, Auswahl lesen, Aktion ausführen – bis der Benutzer beendet.
```
bool laeuft = true;
while (laeuft)
{
    string befehl = Console.ReadLine();
    switch (befehl)
    {
        case "1": ... break;
        case "0": laeuft = false; break;
        default: Console.WriteLine("Unbekannt"); break;
    }
}
```

## Befehle dieser App
- `1` – Artikel hinzufügen (danach wird der Artikelname eingelesen)
- `2` – Liste anzeigen
- `3` – Artikel entfernen (danach Name einlesen)
- `0` – Beenden

Damit die Prüfung klappt, gibst du **kein Menü** aus, sondern nur die Rückmeldungen aus der Aufgabe. In deiner eigenen Version in Visual Studio darfst du natürlich ein schönes Menü anzeigen!
''',
  example=r'''List<string> liste = new List<string>();
liste.Add("Milch");
liste.Add("Brot");

Console.WriteLine("=== Einkaufsliste ===");
for (int i = 0; i < liste.Count; i++)
{
    Console.WriteLine($"{i + 1}. {liste[i]}");
}
''',
  note="Diese Struktur (Schleife + switch) kannst du für fast jedes Konsolenprogramm wiederverwenden: Vokabeltrainer, Kassensystem, Spiele …",
  exercises=[
    A("Hinzufügen & Anzeigen", r'''Baue die Hauptschleife mit den Befehlen `1` (hinzufügen), `2` (anzeigen) und `0` (beenden).
- Bei `1`: Namen einlesen, hinzufügen, `Milch hinzugefügt.` ausgeben.
- Bei `2`: alle Artikel nummeriert ausgeben (`1. Milch`). Ist die Liste leer: `Die Liste ist leer.`
- Bei `0`: `Tschüss!` und beenden.

Bei `2`, `1`, `Milch`, `1`, `Brot`, `2`, `0`:
```
Die Liste ist leer.
Milch hinzugefügt.
Brot hinzugefügt.
1. Milch
2. Brot
Tschüss!
```''',
      starter='List<string> liste = new List<string>();\nbool laeuft = true;\n\nwhile (laeuft)\n{\n    string befehl = Console.ReadLine();\n    // switch (befehl) ...\n    laeuft = false;\n}\n',
      solution='List<string> liste = new List<string>();\nbool laeuft = true;\n\nwhile (laeuft)\n{\n    string befehl = Console.ReadLine();\n    switch (befehl)\n    {\n        case "1":\n            string artikel = Console.ReadLine();\n            liste.Add(artikel);\n            Console.WriteLine($"{artikel} hinzugefügt.");\n            break;\n        case "2":\n            if (liste.Count == 0)\n                Console.WriteLine("Die Liste ist leer.");\n            for (int i = 0; i < liste.Count; i++)\n                Console.WriteLine($"{i + 1}. {liste[i]}");\n            break;\n        case "0":\n            Console.WriteLine("Tschüss!");\n            laeuft = false;\n            break;\n    }\n}\n',
      tests=[T("Die Liste ist leer.\nMilch hinzugefügt.\nBrot hinzugefügt.\n1. Milch\n2. Brot\nTschüss!", "2", "1", "Milch", "1", "Brot", "2", "0")], compare="lenient"),
    A("Entfernen & Fehler", r'''Erweitere dein Programm um:
- `3`: Namen einlesen und entfernen → `Brot entfernt.` – gibt es den Artikel nicht: `Brot ist nicht auf der Liste.`
- Unbekannte Befehle → `Unbekannter Befehl.`
- Beim Hinzufügen doppelte Artikel ablehnen: `Milch ist schon auf der Liste.`

Bei `1`, `Milch`, `1`, `Milch`, `3`, `Brot`, `9`, `3`, `Milch`, `2`, `0`:
```
Milch hinzugefügt.
Milch ist schon auf der Liste.
Brot ist nicht auf der Liste.
Unbekannter Befehl.
Milch entfernt.
Die Liste ist leer.
Tschüss!
```''',
      starter='List<string> liste = new List<string>();\nbool laeuft = true;\n\nwhile (laeuft)\n{\n    string befehl = Console.ReadLine();\n    switch (befehl)\n    {\n        case "0":\n            Console.WriteLine("Tschüss!");\n            laeuft = false;\n            break;\n    }\n}\n',
      solution='List<string> liste = new List<string>();\nbool laeuft = true;\n\nwhile (laeuft)\n{\n    string befehl = Console.ReadLine();\n    switch (befehl)\n    {\n        case "1":\n            string neu = Console.ReadLine();\n            if (liste.Contains(neu))\n                Console.WriteLine($"{neu} ist schon auf der Liste.");\n            else\n            {\n                liste.Add(neu);\n                Console.WriteLine($"{neu} hinzugefügt.");\n            }\n            break;\n        case "2":\n            if (liste.Count == 0)\n                Console.WriteLine("Die Liste ist leer.");\n            for (int i = 0; i < liste.Count; i++)\n                Console.WriteLine($"{i + 1}. {liste[i]}");\n            break;\n        case "3":\n            string weg = Console.ReadLine();\n            if (liste.Remove(weg))\n                Console.WriteLine($"{weg} entfernt.");\n            else\n                Console.WriteLine($"{weg} ist nicht auf der Liste.");\n            break;\n        case "0":\n            Console.WriteLine("Tschüss!");\n            laeuft = false;\n            break;\n        default:\n            Console.WriteLine("Unbekannter Befehl.");\n            break;\n    }\n}\n',
      tests=[T("Milch hinzugefügt.\nMilch ist schon auf der Liste.\nBrot ist nicht auf der Liste.\nUnbekannter Befehl.\nMilch entfernt.\nDie Liste ist leer.\nTschüss!", "1", "Milch", "1", "Milch", "3", "Brot", "9", "3", "Milch", "2", "0")], compare="lenient",
      hint="liste.Remove(x) gibt true zurück, wenn etwas entfernt wurde, sonst false."),
    A("Mit Klasse und Menge", r'''Profi-Version: Speichere Artikel als Objekte der Klasse `Artikel` mit `Name` und `Menge`. Befehl `1` liest Name **und** Menge ein. Ist der Artikel schon da, wird die Menge **addiert** (`Milch: jetzt 3`), sonst `2x Milch hinzugefügt.`. Befehl `2` zeigt `Milch (3)`. `0` beendet mit `Tschüss!`.

Bei `1`, `Milch`, `2`, `1`, `Eier`, `10`, `1`, `Milch`, `1`, `2`, `0`:
```
2x Milch hinzugefügt.
10x Eier hinzugefügt.
Milch: jetzt 3
Milch (3)
Eier (10)
Tschüss!
```''',
      starter='List<Artikel> liste = new List<Artikel>();\n\n\nclass Artikel\n{\n    public string Name { get; set; }\n    public int Menge { get; set; }\n}\n',
      solution='List<Artikel> liste = new List<Artikel>();\nbool laeuft = true;\n\nwhile (laeuft)\n{\n    switch (Console.ReadLine())\n    {\n        case "1":\n            string name = Console.ReadLine();\n            int menge = int.Parse(Console.ReadLine());\n            Artikel vorhanden = Suche(liste, name);\n            if (vorhanden != null)\n            {\n                vorhanden.Menge += menge;\n                Console.WriteLine($"{name}: jetzt {vorhanden.Menge}");\n            }\n            else\n            {\n                liste.Add(new Artikel { Name = name, Menge = menge });\n                Console.WriteLine($"{menge}x {name} hinzugefügt.");\n            }\n            break;\n        case "2":\n            foreach (Artikel a in liste)\n                Console.WriteLine($"{a.Name} ({a.Menge})");\n            break;\n        case "0":\n            Console.WriteLine("Tschüss!");\n            laeuft = false;\n            break;\n    }\n}\n\nstatic Artikel Suche(List<Artikel> liste, string name)\n{\n    foreach (Artikel a in liste)\n        if (a.Name == name) return a;\n    return null;\n}\n\nclass Artikel\n{\n    public string Name { get; set; }\n    public int Menge { get; set; }\n}\n',
      tests=[T("2x Milch hinzugefügt.\n10x Eier hinzugefügt.\nMilch: jetzt 3\nMilch (3)\nEier (10)\nTschüss!", "1", "Milch", "2", "1", "Eier", "10", "1", "Milch", "1", "2", "0")], compare="lenient",
      hint="Eine Hilfsmethode, die den Artikel mit dem Namen sucht (oder null zurückgibt), macht es viel übersichtlicher."),
  ],
  quiz=[
    Q("Wie hält man ein Menü-Programm am Laufen?", ["Mit einer Hauptschleife", "Mit vielen if ohne Schleife", "Mit Thread.Sleep", "Gar nicht"], 0, ""),
    Q("Was gibt liste.Remove(\"X\") zurück, wenn X nicht in der Liste ist?", ["null", "false", "-1", "Es stürzt ab"], 1, ""),
    Q("Was bedeutet null bei Objekten?", ["Die Zahl 0", "Kein Objekt vorhanden", "Ein leerer String", "Ein Fehler"], 1, ""),
  ])
