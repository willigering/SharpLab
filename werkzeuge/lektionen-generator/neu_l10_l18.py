"""Zusätzliche Aufgaben Lektion 10–18 (bis 8 pro Lektion)."""
from neu import N, T, E

SWITCH = (r"\bswitch\b", "Löse die Auswahl mit der Mehrfachauswahl aus dieser Lektion.")

# ---------------- l10 switch ----------------
N("l10", "Ampel", """
Lies eine Ampelfarbe ein und gib aus, was zu tun ist:
- `rot`: `Stehen bleiben!`
- `gelb`: `Achtung!`
- `grün`: `Gehen!`
- alles andere: `Unbekannte Farbe`

Bei `rot`:

```
Stehen bleiben!
```
""", r'''
string farbe = Console.ReadLine();
switch (farbe)
{
    case "rot":
        Console.WriteLine("Stehen bleiben!");
        break;
    case "gelb":
        Console.WriteLine("Achtung!");
        break;
    case "grün":
        Console.WriteLine("Gehen!");
        break;
    default:
        Console.WriteLine("Unbekannte Farbe");
        break;
}
''', [T("Stehen bleiben!", "rot"), T("Achtung!", "gelb"), T("Gehen!", "grün"), T("Unbekannte Farbe", "blau")],
  "Auch Texte können die Fälle unterscheiden. Für alle anderen Eingaben gibt es einen Auffang-Fall.", "lenient", anf=[SWITCH])

N("l10", "Note als Wort", """
Lies eine Schulnote von 1 bis 6 ein und gib sie als Wort aus: sehr gut, gut, befriedigend, ausreichend, mangelhaft, ungenügend. Für andere Zahlen erscheint `Keine gültige Note`. Bei `3`:

```
befriedigend
```
""", r'''
int note = int.Parse(Console.ReadLine());
switch (note)
{
    case 1: Console.WriteLine("sehr gut"); break;
    case 2: Console.WriteLine("gut"); break;
    case 3: Console.WriteLine("befriedigend"); break;
    case 4: Console.WriteLine("ausreichend"); break;
    case 5: Console.WriteLine("mangelhaft"); break;
    case 6: Console.WriteLine("ungenügend"); break;
    default: Console.WriteLine("Keine gültige Note"); break;
}
''', [T("befriedigend", "3"), T("sehr gut", "1"), T("ungenügend", "6"), T("Keine gültige Note", "7")],
  "Jede Note von 1 bis 6 bekommt ihren eigenen Fall. Vergiss den Fall für ungültige Zahlen nicht.", "lenient", anf=[SWITCH])

N("l10", "Tage im Monat", """
Lies eine Monatszahl (1 bis 12) ein und gib aus, wie viele Tage der Monat hat. Der Februar hat hier 28 Tage. Für andere Zahlen erscheint `Keinen solchen Monat`. Monate mit gleich vielen Tagen sollen sich **einen** gemeinsamen Zweig teilen. Bei `4`:

```
30 Tage
```
""", r'''
int monat = int.Parse(Console.ReadLine());
switch (monat)
{
    case 1: case 3: case 5: case 7: case 8: case 10: case 12:
        Console.WriteLine("31 Tage");
        break;
    case 4: case 6: case 9: case 11:
        Console.WriteLine("30 Tage");
        break;
    case 2:
        Console.WriteLine("28 Tage");
        break;
    default:
        Console.WriteLine("Keinen solchen Monat");
        break;
}
''', [T("30 Tage", "4"), T("31 Tage", "1"), T("31 Tage", "12"), T("28 Tage", "2"), T("Keinen solchen Monat", "13")],
  "In der Lektion siehst du, wie mehrere Werte denselben Code-Block benutzen. So schreibst du jede Ausgabe nur einmal.", "lenient",
  anf=[SWITCH, (r"case\s+\d+\s*:\s*case|\bor\b", "Lass Monate mit gleich vielen Tagen einen gemeinsamen Zweig benutzen.")])

N("l10", "Getränkeautomat", """
Lies eine Nummer ein und gib das Getränk aus: `1` Wasser, `2` Saft, `3` Kakao, alles andere `Gibt es nicht`. Nutze diesmal die **moderne Kurzform** aus der Lektion, die direkt einen Wert liefert. Bei `2`:

```
Du bekommst: Saft
```
""", r'''
int nummer = int.Parse(Console.ReadLine());
string getraenk = nummer switch
{
    1 => "Wasser",
    2 => "Saft",
    3 => "Kakao",
    _ => "Gibt es nicht"
};
Console.WriteLine($"Du bekommst: {getraenk}");
''', [T("Du bekommst: Saft", "2"), T("Du bekommst: Wasser", "1"), T("Du bekommst: Kakao", "3"), T("Du bekommst: Gibt es nicht", "9")],
  "Bei der Kurzform steht die Variable vor dem Schlüsselwort, und jeder Fall liefert einen Text. Den Text speicherst du und gibst ihn danach aus.", "lenient",
  anf=[(r"\w\s+switch\s*\{", "Nutze die moderne Kurzform, bei der die Auswahl direkt einen Wert liefert.")])

N("l10", "Wochenplan", """
Lies einen Wochentag ein (`Montag` bis `Sonntag`). Von Montag bis Freitag erscheint `Schule`, am Samstag und Sonntag `Wochenende`, bei allem anderen `Unbekannter Tag`. Tage mit gleicher Ausgabe teilen sich einen Zweig. Bei `Samstag`:

```
Wochenende
```
""", r'''
string tag = Console.ReadLine();
switch (tag)
{
    case "Montag":
    case "Dienstag":
    case "Mittwoch":
    case "Donnerstag":
    case "Freitag":
        Console.WriteLine("Schule");
        break;
    case "Samstag":
    case "Sonntag":
        Console.WriteLine("Wochenende");
        break;
    default:
        Console.WriteLine("Unbekannter Tag");
        break;
}
''', [T("Wochenende", "Samstag"), T("Schule", "Montag"), T("Schule", "Freitag"), T("Wochenende", "Sonntag"), T("Unbekannter Tag", "Feiertag")],
  "Wie bei den Zahlen können sich auch Texte einen Zweig teilen. Der Auffang-Fall fängt alles Unbekannte ab.", "lenient",
  anf=[SWITCH, (r'case\s+"\w+"\s*:\s*case|\bor\b', "Lass die Tage mit gleicher Ausgabe einen gemeinsamen Zweig benutzen.")])

# ---------------- l11 while ----------------
WHILE = (r"\bwhile\s*\(", "Löse die Aufgabe mit der Schleife aus dieser Lektion.")

N("l11", "Gerade Zahlen", """
Gib alle geraden Zahlen von 2 bis 20 aus, jede in einer eigenen Zeile:

```
2
4
6
...
20
```

(Die Punkte stehen für die Zahlen dazwischen.)
""", r'''
int zahl = 2;
while (zahl <= 20)
{
    Console.WriteLine(zahl);
    zahl = zahl + 2;
}
''', [T("2\n4\n6\n8\n10\n12\n14\n16\n18\n20")],
  "Starte bei 2 und erhöhe in jeder Runde um 2. Die Bedingung sagt, wann Schluss ist.", anf=[WHILE])

N("l11", "Quersumme", """
Lies eine positive ganze Zahl ein und gib ihre Quersumme aus – die Summe aller Ziffern. Bei `1234`:

```
Quersumme: 10
```
""", r'''
int zahl = int.Parse(Console.ReadLine());
int summe = 0;
while (zahl > 0)
{
    summe = summe + zahl % 10;
    zahl = zahl / 10;
}
Console.WriteLine($"Quersumme: {summe}");
''', [T("Quersumme: 10", "1234"), T("Quersumme: 9", "9"), T("Quersumme: 10", "505"), T("Quersumme: 27", "999")],
  "Die letzte Ziffer einer Zahl ist der Rest beim Teilen durch 10. Teilst du die Zahl ganzzahlig durch 10, fällt diese Ziffer weg.", "lenient",
  anf=[WHILE])

N("l11", "Wörter bis q", """
Lies so lange Wörter ein, bis `q` eingegeben wird. Für jedes andere Wort erscheint `Du hast ... gewählt.`, am Schluss `Ende.`. Prüfe erst **am Ende** jeder Runde, ob es weitergeht. Bei `Pizza`, `Nudeln`, `q`:

```
Du hast Pizza gewählt.
Du hast Nudeln gewählt.
Ende.
```
""", r'''
string wort;
do
{
    wort = Console.ReadLine();
    if (wort != "q")
    {
        Console.WriteLine($"Du hast {wort} gewählt.");
    }
} while (wort != "q");
Console.WriteLine("Ende.");
''', [T("Du hast Pizza gewählt.\nDu hast Nudeln gewählt.\nEnde.", "Pizza", "Nudeln", "q"), T("Ende.", "q")],
  "Die Lektion zeigt eine Schleifenform, die immer mindestens einmal läuft. Die Variable für das Wort brauchst du schon vor der Schleife.", "lenient",
  anf=[(r"\bdo\s*\{", "Nutze die Schleifenform, die erst am Ende jeder Runde prüft.")])

N("l11", "Erste Zweierpotenz", """
Lies eine Grenze ein. Starte bei 1 und verdopple so lange, bis die Zahl **größer** als die Grenze ist. Bei `100`:

```
Die erste Zweierpotenz über 100 ist 128.
```
""", r'''
int grenze = int.Parse(Console.ReadLine());
int zahl = 1;
while (zahl <= grenze)
{
    zahl = zahl * 2;
}
Console.WriteLine($"Die erste Zweierpotenz über {grenze} ist {zahl}.");
''', [T("Die erste Zweierpotenz über 100 ist 128.", "100"), T("Die erste Zweierpotenz über 1 ist 2.", "1"), T("Die erste Zweierpotenz über 1000 ist 1024.", "1000")],
  "Die Schleife läuft, solange die Zahl noch nicht über der Grenze liegt. Ausgegeben wird erst danach.", "lenient",
  anf=[WHILE])

# ---------------- l12 for ----------------
FOR = (r"\bfor\s*\(", "Löse die Aufgabe mit der Zählschleife aus dieser Lektion.")

N("l12", "Einmaleins", """
Lies eine Zahl ein und gib ihr Einmaleins von 1 bis 10 aus. Bei `3`:

```
3 x 1 = 3
3 x 2 = 6
...
3 x 10 = 30
```

(Die Punkte stehen für die Zeilen dazwischen.)
""", r'''
int zahl = int.Parse(Console.ReadLine());
for (int i = 1; i <= 10; i++)
{
    Console.WriteLine($"{zahl} x {i} = {zahl * i}");
}
''', [T("\n".join(f"3 x {i} = {3*i}" for i in range(1, 11)), "3"), T("\n".join(f"7 x {i} = {7*i}" for i in range(1, 11)), "7")],
  "Die Zählvariable läuft von 1 bis 10. In jeder Runde rechnest du die Eingabe mal die Zählvariable.", "lenient", anf=[FOR])

N("l12", "Jede dritte Zahl", """
Gib die Zahlen 3, 6, 9 bis 30 in **einer** Zeile aus, getrennt durch Leerzeichen:

```
3 6 9 12 15 18 21 24 27 30
```
""", r'''
for (int i = 3; i <= 30; i = i + 3)
{
    Console.Write(i + " ");
}
Console.WriteLine();
''', [T("3 6 9 12 15 18 21 24 27 30")],
  "Die Zählvariable muss nicht immer um 1 wachsen. Damit alles in einer Zeile bleibt, nimmst du den Ausgabe-Befehl ohne Zeilenumbruch.",
  anf=[FOR])

N("l12", "Rechteck aus Zeichen", """
Lies eine Breite und eine Höhe ein und zeichne ein Rechteck aus `#`. Bei `5` und `3`:

```
#####
#####
#####
```
""", r'''
int breite = int.Parse(Console.ReadLine());
int hoehe = int.Parse(Console.ReadLine());
for (int zeile = 0; zeile < hoehe; zeile++)
{
    for (int spalte = 0; spalte < breite; spalte++)
    {
        Console.Write("#");
    }
    Console.WriteLine();
}
''', [T("#####\n#####\n#####", "5", "3"), T("##\n##\n##\n##", "2", "4")],
  "Eine Schleife zählt die Zeilen, eine zweite darin die Zeichen pro Zeile. Nach jeder Zeile beginnst du eine neue.", "lenient",
  anf=[(r"\bfor\s*\([\s\S]*\bfor\s*\(", "Nutze zwei ineinander verschachtelte Schleifen – eine für die Zeilen, eine für die Zeichen.")])

N("l12", "Zinsen", """
Auf einem Sparkonto liegen 1000 Euro, jedes Jahr kommen 10 Prozent Zinsen dazu (nur ganze Euro, Nachkommastellen fallen weg). Lies die Anzahl der Jahre ein und gib den Stand nach jedem Jahr aus. Bei `3`:

```
Jahr 1: 1100 Euro
Jahr 2: 1210 Euro
Jahr 3: 1331 Euro
```
""", r'''
int jahre = int.Parse(Console.ReadLine());
int kapital = 1000;
for (int jahr = 1; jahr <= jahre; jahr++)
{
    kapital = kapital * 110 / 100;
    Console.WriteLine($"Jahr {jahr}: {kapital} Euro");
}
''', [T("Jahr 1: 1100 Euro\nJahr 2: 1210 Euro\nJahr 3: 1331 Euro", "3"),
      T("Jahr 1: 1100 Euro\nJahr 2: 1210 Euro\nJahr 3: 1331 Euro\nJahr 4: 1464 Euro\nJahr 5: 1610 Euro", "5")],
  "Mit ganzen Zahlen fallen die Nachkommastellen beim Teilen von selbst weg. Rechne zuerst mal 110 und teile dann durch 100.", "lenient",
  anf=[FOR])

# ---------------- l13 Arrays ----------------
N("l13", "Farben im Array", """
Lege ein Array `farben` mit `Rot`, `Grün` und `Blau` an. Gib die erste Farbe, die letzte Farbe und die Anzahl aus. Letzte Farbe und Anzahl soll das Programm selbst herausfinden:

```
Erste Farbe: Rot
Letzte Farbe: Blau
Anzahl: 3
```
""", r'''
string[] farben = { "Rot", "Grün", "Blau" };
Console.WriteLine($"Erste Farbe: {farben[0]}");
Console.WriteLine($"Letzte Farbe: {farben[farben.Length - 1]}");
Console.WriteLine($"Anzahl: {farben.Length}");
''', [T("Erste Farbe: Rot\nLetzte Farbe: Blau\nAnzahl: 3")],
  "Die Zählung beginnt bei 0. Wie du an die Länge kommst und daraus den letzten Index ausrechnest, steht in der Lektion.",
  anf=[(r"string\s*\[\s*\]\s*farben", "Lege farben als Array von Texten an."),
       (r"farben\s*\.\s*Length", "Lass das Programm die Anzahl selbst herausfinden.")])

N("l13", "Warme Tage", """
Lege ein Array mit den Temperaturen `18`, `23`, `19`, `25` und `21` an. Gib alle Werte über 20 aus und danach, wie viele es waren:

```
23
25
21
3 Tage über 20 Grad
```
""", r'''
int[] temperaturen = { 18, 23, 19, 25, 21 };
int anzahl = 0;
for (int i = 0; i < temperaturen.Length; i++)
{
    if (temperaturen[i] > 20)
    {
        Console.WriteLine(temperaturen[i]);
        anzahl++;
    }
}
Console.WriteLine($"{anzahl} Tage über 20 Grad");
''', [T("23\n25\n21\n3 Tage über 20 Grad")],
  "Geh das Array mit einer Schleife durch und prüfe jeden Wert. Für die Anzahl brauchst du eine Zählvariable.",
  anf=[(r"int\s*\[\s*\]", "Lege die Temperaturen in einem Array an."),
       (r"\bfor\s*\(|\bwhile\s*\(", "Geh das Array mit einer Schleife durch.")])

N("l13", "Fünf Zahlen speichern", """
Lies fünf ganze Zahlen ein und speichere sie in einem Array. Gib danach die Summe und den Durchschnitt mit einer Nachkommastelle aus. Bei `1`, `2`, `3`, `4`, `6`:

```
Summe: 16
Durchschnitt: 3,2
```
""", r'''
int[] zahlen = new int[5];
for (int i = 0; i < 5; i++)
{
    zahlen[i] = int.Parse(Console.ReadLine());
}
int summe = 0;
for (int i = 0; i < zahlen.Length; i++)
{
    summe = summe + zahlen[i];
}
Console.WriteLine($"Summe: {summe}");
Console.WriteLine($"Durchschnitt: {(double)summe / zahlen.Length:F1}");
''', [T("Summe: 16\nDurchschnitt: 3,2", "1", "2", "3", "4", "6"), T("Summe: 25\nDurchschnitt: 5,0", "2", "4", "4", "5", "10")],
  "Lege zuerst ein leeres Array mit fünf Plätzen an und fülle es in einer Schleife. Beim Durchschnitt denk an das Teilen mit ganzen Zahlen.", "lenient",
  anf=[(r"\w+\s*\[\s*\w+\s*\]\s*=", "Speichere die Eingaben in den Plätzen des Arrays.")])

N("l13", "Array umdrehen", """
Lege ein Array mit `1`, `2`, `3`, `4`, `5` an. Fülle ein **zweites** Array mit denselben Zahlen in umgekehrter Reihenfolge und gib es in einer Zeile aus:

```
5 4 3 2 1
```
""", r'''
int[] zahlen = { 1, 2, 3, 4, 5 };
int[] umgedreht = new int[zahlen.Length];
for (int i = 0; i < zahlen.Length; i++)
{
    umgedreht[i] = zahlen[zahlen.Length - 1 - i];
}
for (int i = 0; i < umgedreht.Length; i++)
{
    Console.Write(umgedreht[i] + " ");
}
Console.WriteLine();
''', [T("5 4 3 2 1")],
  "Der erste Platz des neuen Arrays bekommt den letzten Wert des alten. Rechne aus, welcher Index zu welchem gehört.",
  anf=[(r"new\s+int\s*\[", "Lege ein zweites Array für die umgedrehten Zahlen an."),
       (r"\w+\s*\[\s*\w+\s*\]\s*=", "Fülle das zweite Array Platz für Platz.")])

# ---------------- l14 foreach ----------------
FOREACH = (r"\bforeach\s*\(", "Durchlaufe die Werte mit der Schleife aus dieser Lektion.")

N("l14", "Alle begrüßen", """
Lege ein Array mit den Namen `Ana`, `Ben` und `Cem` an und begrüße jeden Namen ohne Index:

```
Hallo Ana!
Hallo Ben!
Hallo Cem!
```
""", r'''
string[] namen = { "Ana", "Ben", "Cem" };
foreach (string name in namen)
{
    Console.WriteLine($"Hallo {name}!");
}
''', [T("Hallo Ana!\nHallo Ben!\nHallo Cem!")],
  "Die Schleife aus dieser Lektion gibt dir jeden Namen nacheinander, ganz ohne Zählvariable.", anf=[FOREACH])

N("l14", "Kleinste Zahl", """
Lege ein Array mit `12`, `5`, `8`, `3` und `9` an. Finde die kleinste Zahl, indem du alle Werte durchgehst (ohne fertige Funktion wie `Min()`):

```
Kleinste Zahl: 3
```
""", r'''
int[] zahlen = { 12, 5, 8, 3, 9 };
int kleinste = zahlen[0];
foreach (int zahl in zahlen)
{
    if (zahl < kleinste)
    {
        kleinste = zahl;
    }
}
Console.WriteLine($"Kleinste Zahl: {kleinste}");
''', [T("Kleinste Zahl: 3")],
  "Merk dir die bisher kleinste Zahl und vergleiche jede weitere damit. Als Startwert eignet sich der erste Wert des Arrays.",
  anf=[FOREACH])

N("l14", "Positiv und negativ", """
Lege ein Array mit `3`, `-1`, `4`, `-5`, `9` und `0` an. Zähle, wie viele Zahlen positiv und wie viele negativ sind (die 0 ist keins von beiden):

```
Positiv: 3
Negativ: 2
```
""", r'''
int[] zahlen = { 3, -1, 4, -5, 9, 0 };
int positiv = 0;
int negativ = 0;
foreach (int zahl in zahlen)
{
    if (zahl > 0)
    {
        positiv++;
    }
    else if (zahl < 0)
    {
        negativ++;
    }
}
Console.WriteLine($"Positiv: {positiv}");
Console.WriteLine($"Negativ: {negativ}");
''', [T("Positiv: 3\nNegativ: 2")],
  "Du brauchst zwei Zähler. In der Schleife entscheidest du für jede Zahl, welcher Zähler wächst.", anf=[FOREACH])

N("l14", "Buchstaben zählen", """
Lies einen Satz ein und zähle, wie oft der kleine Buchstabe `e` darin vorkommt. Bei `Lesen ist schön`:

```
Anzahl e: 2
```
""", r'''
string satz = Console.ReadLine();
int anzahl = 0;
foreach (char zeichen in satz)
{
    if (zeichen == 'e')
    {
        anzahl++;
    }
}
Console.WriteLine($"Anzahl e: {anzahl}");
''', [T("Anzahl e: 2", "Lesen ist schön"), T("Anzahl e: 0", "Hallo"), T("Anzahl e: 4", "eeee")],
  "Auch einen Text kann die Schleife Zeichen für Zeichen durchgehen. Ein einzelnes Zeichen steht in einfachen Anführungszeichen.", "lenient",
  anf=[FOREACH])

N("l14", "Preisliste mit Summe", """
Lege ein Array mit den Preisen `1.99`, `4.5`, `0.99` und `12` an. Gib jeden Preis mit zwei Nachkommastellen aus und am Ende die Summe:

```
1,99 Euro
4,50 Euro
0,99 Euro
12,00 Euro
Summe: 19,48 Euro
```
""", r'''
double[] preise = { 1.99, 4.5, 0.99, 12 };
double summe = 0;
foreach (double preis in preise)
{
    Console.WriteLine($"{preis:F2} Euro");
    summe = summe + preis;
}
Console.WriteLine($"Summe: {summe:F2} Euro");
''', [T("1,99 Euro\n4,50 Euro\n0,99 Euro\n12,00 Euro\nSumme: 19,48 Euro")],
  "In derselben Schleife kannst du ausgeben und die Summe sammeln. Die Summe gibst du erst nach der Schleife aus.",
  anf=[FOREACH, (r"double\s*\[\s*\]", "Lege die Preise in einem Array von Kommazahlen an.")])

# ---------------- l15 Listen ----------------
N("l15", "Wunschliste", """
Lege eine leere Liste `wuensche` an. Nimm `Fahrrad`, `Buch` und `Spiel` auf und streiche danach das `Buch` wieder. Gib die Anzahl und alle Wünsche aus:

```
Wünsche: 2
- Fahrrad
- Spiel
```
""", r'''
List<string> wuensche = new List<string>();
wuensche.Add("Fahrrad");
wuensche.Add("Buch");
wuensche.Add("Spiel");
wuensche.Remove("Buch");
Console.WriteLine($"Wünsche: {wuensche.Count}");
foreach (string wunsch in wuensche)
{
    Console.WriteLine($"- {wunsch}");
}
''', [T("Wünsche: 2\n- Fahrrad\n- Spiel")],
  "Die Lektion zeigt dir die wichtigsten Befehle zum Hinzufügen und Entfernen. Die Anzahl kennt die Liste selbst.",
  anf=[(r"wuensche\s*\.\s*Add\s*\(", "Nimm die Wünsche nacheinander in die Liste auf."),
       (r"wuensche\s*\.\s*Remove\s*\(", "Streiche das Buch wieder aus der Liste."),
       (r"wuensche\s*\.\s*Count", "Die Anzahl soll aus der Liste kommen.")])

N("l15", "Namen sortieren", """
Lies vier Namen ein und gib sie alphabetisch sortiert aus. Bei `Tom`, `Ana`, `Mia`, `Ben`:

```
Ana
Ben
Mia
Tom
```
""", r'''
List<string> namen = new List<string>();
for (int i = 0; i < 4; i++)
{
    namen.Add(Console.ReadLine());
}
namen.Sort();
foreach (string name in namen)
{
    Console.WriteLine(name);
}
''', [T("Ana\nBen\nMia\nTom", "Tom", "Ana", "Mia", "Ben"), T("Cem\nDora\nEmil\nLea", "Lea", "Emil", "Dora", "Cem")],
  "Sammle die Namen zuerst in einer Liste. Fürs Sortieren hat die Liste einen eigenen Befehl.", "lenient",
  anf=[(r"\.\s*Sort\s*\(", "Lass die Liste selbst sortieren.")])

N("l15", "Erste und letzte Zahl", """
Lies so lange Zahlen ein, bis `0` kommt (die 0 zählt nicht mit). Gib danach die erste und die letzte eingegebene Zahl und die Anzahl aus. Bei `4`, `7`, `9`, `0`:

```
Erste Zahl: 4
Letzte Zahl: 9
Anzahl: 3
```
""", r'''
List<int> zahlen = new List<int>();
int zahl = int.Parse(Console.ReadLine());
while (zahl != 0)
{
    zahlen.Add(zahl);
    zahl = int.Parse(Console.ReadLine());
}
Console.WriteLine($"Erste Zahl: {zahlen[0]}");
Console.WriteLine($"Letzte Zahl: {zahlen[zahlen.Count - 1]}");
Console.WriteLine($"Anzahl: {zahlen.Count}");
''', [T("Erste Zahl: 4\nLetzte Zahl: 9\nAnzahl: 3", "4", "7", "9", "0"), T("Erste Zahl: 12\nLetzte Zahl: 5\nAnzahl: 2", "12", "5", "0")],
  "Wie viele Zahlen kommen, weißt du vorher nicht – dafür ist eine Liste ideal. Der letzte Index ist eins kleiner als die Anzahl.", "lenient",
  anf=[(r"List\s*<\s*int\s*>", "Sammle die Zahlen in einer Liste."),
       (r"\.\s*Count", "Lass die Liste die Anzahl liefern.")])

N("l15", "Ohne Doppelte", """
Lies fünf Wörter ein. Nimm jedes Wort nur auf, wenn es noch nicht in der Liste steht. Gib danach aus, wie viele verschiedene Wörter es sind, und die Wörter selbst. Bei `Hund`, `Katze`, `Hund`, `Maus`, `Katze`:

```
Verschiedene Wörter: 3
Hund
Katze
Maus
```
""", r'''
List<string> woerter = new List<string>();
for (int i = 0; i < 5; i++)
{
    string wort = Console.ReadLine();
    if (!woerter.Contains(wort))
    {
        woerter.Add(wort);
    }
}
Console.WriteLine($"Verschiedene Wörter: {woerter.Count}");
foreach (string wort in woerter)
{
    Console.WriteLine(wort);
}
''', [T("Verschiedene Wörter: 3\nHund\nKatze\nMaus", "Hund", "Katze", "Hund", "Maus", "Katze"),
      T("Verschiedene Wörter: 5\nA\nB\nC\nD\nE", "A", "B", "C", "D", "E")],
  "Bevor du ein Wort aufnimmst, frag die Liste, ob sie es schon hat. Der passende Befehl steht in der Lektion.", "lenient",
  anf=[(r"\.\s*Contains\s*\(", "Prüfe vor dem Aufnehmen, ob das Wort schon in der Liste steht.")])

N("l15", "Aufgabe erledigt", """
Lege eine Liste mit `Einkaufen`, `Hausaufgaben` und `Zimmer aufräumen` an. Lies die Nummer der erledigten Aufgabe ein (1 ist die erste), entferne sie und gib den Rest nummeriert aus. Bei `2`:

```
Erledigt: Hausaufgaben
1. Einkaufen
2. Zimmer aufräumen
```
""", r'''
List<string> aufgaben = new List<string> { "Einkaufen", "Hausaufgaben", "Zimmer aufräumen" };
int nummer = int.Parse(Console.ReadLine());
Console.WriteLine($"Erledigt: {aufgaben[nummer - 1]}");
aufgaben.RemoveAt(nummer - 1);
for (int i = 0; i < aufgaben.Count; i++)
{
    Console.WriteLine($"{i + 1}. {aufgaben[i]}");
}
''', [T("Erledigt: Hausaufgaben\n1. Einkaufen\n2. Zimmer aufräumen", "2"), T("Erledigt: Einkaufen\n1. Hausaufgaben\n2. Zimmer aufräumen", "1")],
  "Die Nummer für Menschen beginnt bei 1, der Index bei 0. Zum Entfernen an einer Stelle gibt es einen eigenen Befehl.", "lenient",
  anf=[(r"\.\s*RemoveAt\s*\(", "Entferne die Aufgabe über ihre Position in der Liste.")])

# ---------------- l16 Methoden ----------------
N("l16", "Trennlinie", """
Schreib eine Methode `static void Linie()`, die `----------` (zehn Striche) ausgibt. Benutze sie, um diese Ausgabe zu erzeugen:

```
----------
Inhalt
----------
```
""", r'''
Linie();
Console.WriteLine("Inhalt");
Linie();

static void Linie()
{
    Console.WriteLine("----------");
}
''', [T("----------\nInhalt\n----------")],
  "Die Methode steht unter dem Hauptcode. Im Hauptcode rufst du sie zweimal auf.",
  anf=[(r"static\s+void\s+Linie\s*\(\s*\)", "Schreib die Methode Linie ohne Parameter."),
       (r"(Linie\s*\(\s*\)\s*;[\s\S]*){2}", "Ruf die Methode Linie zweimal auf.")])

N("l16", "Begrüßen mit Namen", """
Schreib eine Methode `static void Begruesse(string name)`, die einen Namen begrüßt. Ruf sie für `Ana` und `Ben` auf:

```
Hallo, Ana!
Hallo, Ben!
```
""", r'''
Begruesse("Ana");
Begruesse("Ben");

static void Begruesse(string name)
{
    Console.WriteLine($"Hallo, {name}!");
}
''', [T("Hallo, Ana!\nHallo, Ben!")],
  "Den Parameter kannst du in der Methode wie eine Variable benutzen. Beim Aufruf steht der Name in den Klammern.",
  anf=[(r"static\s+void\s+Begruesse\s*\(\s*string\s+name\s*\)", "Schreib die Methode Begruesse mit einem Text-Parameter name."),
       (r"Begruesse\s*\(\s*\"Ana\"\s*\)", "Ruf die Methode für Ana auf."),
       (r"Begruesse\s*\(\s*\"Ben\"\s*\)", "Ruf die Methode für Ben auf.")])

N("l16", "Rechteck-Methode", """
Schreib eine Methode `static void Rechteck(int breite, int hoehe)`, die die Fläche ausrechnet und einen Satz ausgibt. Ruf sie mit 3 und 4 und mit 5 und 2 auf:

```
Ein 3 x 4 Rechteck hat die Fläche 12.
Ein 5 x 2 Rechteck hat die Fläche 10.
```
""", r'''
Rechteck(3, 4);
Rechteck(5, 2);

static void Rechteck(int breite, int hoehe)
{
    Console.WriteLine($"Ein {breite} x {hoehe} Rechteck hat die Fläche {breite * hoehe}.");
}
''', [T("Ein 3 x 4 Rechteck hat die Fläche 12.\nEin 5 x 2 Rechteck hat die Fläche 10.")],
  "Mehrere Parameter trennst du mit Komma, beim Aufruf genauso. Die Rechnung passiert in der Methode.",
  anf=[(r"static\s+void\s+Rechteck\s*\(\s*int\s+\w+\s*,\s*int\s+\w+\s*\)", "Schreib die Methode Rechteck mit zwei Zahl-Parametern."),
       (r"Rechteck\s*\(\s*3\s*,\s*4\s*\)", "Ruf die Methode mit 3 und 4 auf."),
       (r"Rechteck\s*\(\s*5\s*,\s*2\s*\)", "Ruf die Methode mit 5 und 2 auf.")])

N("l16", "Wiederholer", """
Schreib eine Methode `static void Wiederhole(string text, int anzahl)`, die den Text so oft in **einer** Zeile ausgibt, getrennt durch Leerzeichen. Ruf sie mit `Hey` und 3 und mit `Ho` und 2 auf:

```
Hey Hey Hey
Ho Ho
```
""", r'''
Wiederhole("Hey", 3);
Wiederhole("Ho", 2);

static void Wiederhole(string text, int anzahl)
{
    for (int i = 0; i < anzahl; i++)
    {
        Console.Write(text + " ");
    }
    Console.WriteLine();
}
''', [T("Hey Hey Hey\nHo Ho")],
  "In der Methode darf auch eine Schleife stehen. Erst nach der Schleife beginnt eine neue Zeile.",
  anf=[(r"static\s+void\s+Wiederhole\s*\(\s*string\s+\w+\s*,\s*int\s+\w+\s*\)", "Schreib die Methode Wiederhole mit einem Text- und einem Zahl-Parameter."),
       (r"Wiederhole\s*\(\s*\"Hey\"\s*,\s*3\s*\)", "Ruf die Methode mit Hey und 3 auf."),
       (r"Wiederhole\s*\(\s*\"Ho\"\s*,\s*2\s*\)", "Ruf die Methode mit Ho und 2 auf.")])

N("l16", "Methode ruft Methode", """
Schreib die Methode `static void Linie()` für `----------` und eine Methode `static void Kopfzeile(string titel)`, die den Titel zwischen zwei Linien ausgibt und dafür `Linie` benutzt. Ruf `Kopfzeile` für `Einkauf` und `Termine` auf:

```
----------
Einkauf
----------
----------
Termine
----------
```
""", r'''
Kopfzeile("Einkauf");
Kopfzeile("Termine");

static void Kopfzeile(string titel)
{
    Linie();
    Console.WriteLine(titel);
    Linie();
}

static void Linie()
{
    Console.WriteLine("----------");
}
''', [T("----------\nEinkauf\n----------\n----------\nTermine\n----------")],
  "Eine Methode darf eine andere aufrufen. Im Hauptcode brauchst du dann nur noch zwei Zeilen.",
  anf=[(r"static\s+void\s+Linie\s*\(\s*\)", "Schreib die Methode Linie."),
       (r"static\s+void\s+Kopfzeile\s*\(\s*string\s+\w+\s*\)", "Schreib die Methode Kopfzeile mit einem Text-Parameter."),
       (r"Kopfzeile\s*\(\s*\"Einkauf\"\s*\)", "Ruf Kopfzeile für Einkauf auf."),
       (r"Kopfzeile\s*\(\s*\"Termine\"\s*\)", "Ruf Kopfzeile für Termine auf.")])

# ---------------- l17 Rückgabewert ----------------
RET = (r"\breturn\b", "Die Methode soll ihr Ergebnis zurückgeben, nicht selbst ausgeben.")

N("l17", "Quadrat-Methode", """
Schreib eine Methode `static int Quadrat(int x)`, die das Quadrat einer Zahl **zurückgibt**. Gib damit aus, was 3 zum Quadrat plus 4 zum Quadrat ergibt:

```
3² + 4² = 25
```
""", r'''
Console.WriteLine($"3² + 4² = {Quadrat(3) + Quadrat(4)}");

static int Quadrat(int x)
{
    return x * x;
}
''', [T("3² + 4² = 25")],
  "Ein Ergebnis, das zurückkommt, kannst du direkt weiterrechnen. Die Ausgabe passiert im Hauptcode.",
  anf=[(r"static\s+int\s+Quadrat\s*\(\s*int\s+\w+\s*\)", "Schreib die Methode Quadrat mit einer ganzen Zahl als Rückgabe."),
       RET, (r"Quadrat\s*\(\s*3\s*\)\s*\+\s*Quadrat\s*\(\s*4\s*\)", "Lass die Summe aus zwei Aufrufen von Quadrat berechnen.")])

N("l17", "Darf Auto fahren?", """
Schreib eine Methode `static bool DarfFahren(int alter)`, die `true` liefert, wenn jemand mindestens 18 ist. Lies ein Alter ein und gib das Ergebnis aus. Bei `17`:

```
Darf Auto fahren: False
```
""", r'''
int alter = int.Parse(Console.ReadLine());
Console.WriteLine($"Darf Auto fahren: {DarfFahren(alter)}");

static bool DarfFahren(int alter)
{
    return alter >= 18;
}
''', [T("Darf Auto fahren: False", "17"), T("Darf Auto fahren: True", "18"), T("Darf Auto fahren: True", "40")],
  "Ein Vergleich ist schon selbst true oder false. Das Ergebnis kann die Methode direkt zurückgeben.", "lenient",
  anf=[(r"static\s+bool\s+DarfFahren\s*\(\s*int\s+\w+\s*\)", "Schreib die Methode DarfFahren mit einem Wahrheitswert als Rückgabe."), RET])

N("l17", "Celsius in Fahrenheit", """
Schreib eine Methode `static double ZuFahrenheit(double celsius)`, die Celsius in Fahrenheit umrechnet (mal 1,8 plus 32). Gib damit 0, 37 und 100 Grad mit einer Nachkommastelle aus:

```
0 °C = 32,0 °F
37 °C = 98,6 °F
100 °C = 212,0 °F
```
""", r'''
Console.WriteLine($"0 °C = {ZuFahrenheit(0):F1} °F");
Console.WriteLine($"37 °C = {ZuFahrenheit(37):F1} °F");
Console.WriteLine($"100 °C = {ZuFahrenheit(100):F1} °F");

static double ZuFahrenheit(double celsius)
{
    return celsius * 1.8 + 32;
}
''', [T("0 °C = 32,0 °F\n37 °C = 98,6 °F\n100 °C = 212,0 °F")],
  "Die Methode rechnet nur und gibt das Ergebnis zurück. Wie viele Nachkommastellen erscheinen, legst du in der Ausgabe fest.",
  anf=[(r"static\s+double\s+ZuFahrenheit\s*\(\s*double\s+\w+\s*\)", "Schreib die Methode ZuFahrenheit mit einer Kommazahl als Rückgabe."), RET,
       (r"(ZuFahrenheit\s*\([\s\S]*){3}", "Ruf die Methode für alle drei Temperaturen auf.")])

N("l17", "Bewertung als Text", """
Schreib eine Methode `static string Bewertung(int punkte)`, die einen Text liefert: ab 90 `sehr gut`, ab 50 `bestanden`, darunter `nicht bestanden`. Lies Punkte ein und gib die Bewertung aus. Bei `95`:

```
Bewertung: sehr gut
```
""", r'''
int punkte = int.Parse(Console.ReadLine());
Console.WriteLine($"Bewertung: {Bewertung(punkte)}");

static string Bewertung(int punkte)
{
    if (punkte >= 90)
    {
        return "sehr gut";
    }
    if (punkte >= 50)
    {
        return "bestanden";
    }
    return "nicht bestanden";
}
''', [T("Bewertung: sehr gut", "95"), T("Bewertung: bestanden", "50"), T("Bewertung: nicht bestanden", "20")],
  "Jeder mögliche Weg durch die Methode braucht ein Ergebnis. Die Lektion erklärt, warum.", "lenient",
  anf=[(r"static\s+string\s+Bewertung\s*\(\s*int\s+\w+\s*\)", "Schreib die Methode Bewertung mit einem Text als Rückgabe."), RET])

# ---------------- l18 String-Methoden ----------------
N("l18", "Laut rufen", """
Lies ein Wort ein und gib es in Großbuchstaben aus, zusammen mit der Anzahl seiner Buchstaben. Bei `Hallo`:

```
HALLO hat 5 Buchstaben.
```
""", r'''
string wort = Console.ReadLine();
Console.WriteLine($"{wort.ToUpper()} hat {wort.Length} Buchstaben.");
''', [T("HALLO hat 5 Buchstaben.", "Hallo"), T("PIZZA hat 5 Buchstaben.", "pizza"), T("C# hat 2 Buchstaben.", "c#")],
  "Für beides gibt es fertige Bausteine aus der Lektion. Das Original-Wort bleibt dabei unverändert.", "lenient")

N("l18", "Beginnt mit A?", """
Lies einen Namen ein und prüfe, ob er mit einem großen `A` beginnt. Bei `Ana`:

```
Ana beginnt mit A.
```

Bei `Ben`:

```
Ben beginnt nicht mit A.
```
""", r'''
string name = Console.ReadLine();
if (name.StartsWith("A"))
{
    Console.WriteLine($"{name} beginnt mit A.");
}
else
{
    Console.WriteLine($"{name} beginnt nicht mit A.");
}
''', [T("Ana beginnt mit A.", "Ana"), T("Ben beginnt nicht mit A.", "Ben"), T("Anton beginnt mit A.", "Anton")],
  "Für den Anfang eines Texts gibt es eine eigene Prüfung in der Lektion. Sie liefert true oder false.", "lenient")

N("l18", "Gute Laune", """
Lies einen Satz ein und ersetze darin jedes `schlecht` durch `gut`. Bei `Das Wetter ist schlecht.`:

```
Das Wetter ist gut.
```
""", r'''
string satz = Console.ReadLine();
Console.WriteLine(satz.Replace("schlecht", "gut"));
''', [T("Das Wetter ist gut.", "Das Wetter ist schlecht."), T("Mir geht es gut, gut, gut!", "Mir geht es schlecht, schlecht, gut!")],
  "Strings ändern sich nicht selbst, du bekommst einen neuen Text zurück. Den gibst du aus.", "lenient")

N("l18", "Initialen", """
Lies einen Vor- und Nachnamen in **einer** Zeile ein (getrennt durch ein Leerzeichen) und gib die Initialen aus. Bei `Max Mustermann`:

```
Initialen: M.M.
```
""", r'''
string name = Console.ReadLine();
string[] teile = name.Split(' ');
Console.WriteLine($"Initialen: {teile[0][0]}.{teile[1][0]}.");
''', [T("Initialen: M.M.", "Max Mustermann"), T("Initialen: L.S.", "Lena Sommer")],
  "Zerleg den Namen am Leerzeichen in seine Teile. Von jedem Teil brauchst du das erste Zeichen.", "lenient")

N("l18", "Rückwärts", """
Lies ein Wort ein und gib es rückwärts aus. Geh dafür die Zeichen selbst durch. Bei `August`:

```
Rückwärts: tsuguA
```
""", r'''
string wort = Console.ReadLine();
string rueckwaerts = "";
for (int i = wort.Length - 1; i >= 0; i--)
{
    rueckwaerts = rueckwaerts + wort[i];
}
Console.WriteLine($"Rückwärts: {rueckwaerts}");
''', [T("Rückwärts: tsuguA", "August"), T("Rückwärts: otto", "otto"), T("Rückwärts: lebaK", "Kabel")],
  "Ein Text funktioniert wie ein Array aus Zeichen. Fang beim letzten Zeichen an und hänge eins nach dem anderen an.", "lenient",
  anf=[(r"\bfor\s*\(|\bwhile\s*\(|\bforeach\s*\(", "Geh die Zeichen des Worts in einer Schleife selbst durch.")])
