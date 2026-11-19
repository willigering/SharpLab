# Stufe 2: Kontrollstrukturen
from gen import L, A, T, Q

L(2, "l08", "Entscheidungen mit if / else",
  intro="Programme müssen Entscheidungen treffen: Wenn etwas gilt, mach dies – sonst das.",
  explanation=r'''
## if
```
if (alter >= 18)
{
    Console.WriteLine("Du bist volljährig.");
}
```
In den runden Klammern steht eine **Bedingung**. Ist sie **wahr** (`true`), wird der Block in `{ }` ausgeführt.

## else
`else` heißt „sonst“: Dieser Block läuft, wenn die Bedingung **falsch** ist.

## else if
Mehrere Fälle nacheinander prüfen:
```
if (note == 1) { ... }
else if (note == 2) { ... }
else { ... }
```
C# nimmt den **ersten** passenden Fall und überspringt den Rest.

## Vergleiche
`==` gleich, `!=` ungleich, `<` kleiner, `>` größer, `<=` kleiner gleich, `>=` größer gleich.

**Achtung:** `=` weist zu, `==` vergleicht!
''',
  example=r'''Console.Write("Wie alt bist du? ");
int alter = int.Parse(Console.ReadLine());

if (alter >= 18)
{
    Console.WriteLine("Du darfst Auto fahren.");
}
else if (alter >= 16)
{
    Console.WriteLine("Du darfst Roller fahren.");
}
else
{
    Console.WriteLine("Noch etwas Geduld!");
}
''',
  exampleInput=["17"],
  note="Kein Semikolon hinter if (...)! Sonst gehört der Block nicht mehr zum if und wird immer ausgeführt.",
  exercises=[
    A("Gerade oder ungerade", r'''Lies eine ganze Zahl ein. Ist sie gerade (`zahl % 2 == 0`), gib `gerade` aus, sonst `ungerade`.

Bei `4` → `gerade`, bei `7` → `ungerade`.''',
      starter='int zahl = int.Parse(Console.ReadLine());\n',
      solution='int zahl = int.Parse(Console.ReadLine());\nif (zahl % 2 == 0)\n{\n    Console.WriteLine("gerade");\n}\nelse\n{\n    Console.WriteLine("ungerade");\n}\n',
      tests=[T("gerade", "4"), T("ungerade", "7"), T("gerade", "0")], compare="lenient",
      requirements=[(r'\bif\s*\(', "Benutze if."), (r'\belse\b', "Benutze else.")]),
    A("Temperatur", r'''Lies eine Temperatur (`int`) ein und gib aus:
- unter 10 → `kalt`
- 10 bis 24 → `angenehm`
- 25 oder mehr → `heiß`''',
      starter='int temp = int.Parse(Console.ReadLine());\n',
      solution='int temp = int.Parse(Console.ReadLine());\nif (temp < 10)\n{\n    Console.WriteLine("kalt");\n}\nelse if (temp < 25)\n{\n    Console.WriteLine("angenehm");\n}\nelse\n{\n    Console.WriteLine("heiß");\n}\n',
      tests=[T("kalt", "3"), T("angenehm", "10"), T("angenehm", "24"), T("heiß", "25"), T("kalt", "-5")], compare="lenient",
      requirements=[(r'else\s+if', "Benutze else if für den mittleren Fall.")]),
    A("Größere Zahl", r'''Lies zwei ganze Zahlen ein und gib die größere aus: `Die größere Zahl ist 9`. Sind beide gleich, gib `Beide sind gleich` aus.''',
      starter='int a = int.Parse(Console.ReadLine());\nint b = int.Parse(Console.ReadLine());\n',
      solution='int a = int.Parse(Console.ReadLine());\nint b = int.Parse(Console.ReadLine());\nif (a == b)\n{\n    Console.WriteLine("Beide sind gleich");\n}\nelse if (a > b)\n{\n    Console.WriteLine($"Die größere Zahl ist {a}");\n}\nelse\n{\n    Console.WriteLine($"Die größere Zahl ist {b}");\n}\n',
      tests=[T("Die größere Zahl ist 9", "9", "4"), T("Die größere Zahl ist 12", "3", "12"), T("Beide sind gleich", "5", "5")], compare="lenient"),
  ],
  quiz=[
    Q("Was ist der Unterschied zwischen = und ==?", ["Keiner", "= weist zu, == vergleicht", "== weist zu, = vergleicht", "== ist für Text"], 1, ""),
    Q("Wann wird der else-Block ausgeführt?", ["Immer", "Wenn die if-Bedingung wahr ist", "Wenn die if-Bedingung falsch ist", "Nie"], 2, ""),
    Q("Was gibt der Code aus, wenn x = 5 ist?\nif (x > 3) Ausgabe A, else if (x > 1) Ausgabe B", ["A", "B", "A und B", "Nichts"], 0, "Nur der erste passende Fall wird ausgeführt."),
  ])

L(2, "l09", "Vergleichs- & logische Operatoren",
  intro="Mit && (und), || (oder) und ! (nicht) kombinierst du mehrere Bedingungen.",
  explanation=r'''
## Vergleichsoperatoren
Sie liefern immer einen **bool** (`true`/`false`):
- `a == b` gleich, `a != b` ungleich
- `a < b`, `a > b`, `a <= b`, `a >= b`

## Logische Operatoren
- `&&` – **UND**: beide Bedingungen müssen wahr sein.
- `||` – **ODER**: mindestens eine muss wahr sein.
- `!` – **NICHT**: dreht den Wert um (`!true` ist `false`).

```
if (alter >= 13 && alter <= 19)
    Console.WriteLine("Teenager");
```

## bool-Variablen
Das Ergebnis eines Vergleichs kannst du speichern:
```
bool istWochenende = tag == "Samstag" || tag == "Sonntag";
```

## Strings vergleichen
Texte vergleichst du ebenfalls mit `==`. Groß- und Kleinschreibung zählt: `"Ja" == "ja"` ist `false`.
''',
  example=r'''int alter = 15;
bool hatTicket = true;

bool istTeenager = alter >= 13 && alter <= 19;
Console.WriteLine("Teenager: " + istTeenager);

if (alter >= 12 || hatTicket)
{
    Console.WriteLine("Du darfst rein.");
}

if (!hatTicket)
{
    Console.WriteLine("Bitte Ticket kaufen.");
}
''',
  note="Bei && hört C# auf zu prüfen, sobald eine Bedingung falsch ist. Bei || hört es auf, sobald eine wahr ist.",
  exercises=[
    A("Im Bereich?", r'''Lies eine Zahl ein. Liegt sie zwischen 1 und 100 (beide eingeschlossen), gib `im Bereich` aus, sonst `außerhalb`. Benutze `&&`.''',
      starter='int zahl = int.Parse(Console.ReadLine());\n',
      solution='int zahl = int.Parse(Console.ReadLine());\nif (zahl >= 1 && zahl <= 100)\n    Console.WriteLine("im Bereich");\nelse\n    Console.WriteLine("außerhalb");\n',
      tests=[T("im Bereich", "1"), T("im Bereich", "100"), T("außerhalb", "0"), T("außerhalb", "101"), T("im Bereich", "50")], compare="lenient",
      requirements=[(r'&&', "Benutze && (UND).")]),
    A("Wochenende", r'''Lies einen Wochentag ein (z. B. `Samstag`). Ist es `Samstag` oder `Sonntag`, gib `Wochenende!` aus, sonst `Arbeitstag`. Benutze `||`.''',
      starter='string tag = Console.ReadLine();\n',
      solution='string tag = Console.ReadLine();\nif (tag == "Samstag" || tag == "Sonntag")\n    Console.WriteLine("Wochenende!");\nelse\n    Console.WriteLine("Arbeitstag");\n',
      tests=[T("Wochenende!", "Samstag"), T("Wochenende!", "Sonntag"), T("Arbeitstag", "Montag")], compare="lenient",
      requirements=[(r'\|\|', "Benutze || (ODER).")]),
    A("Kinokasse", r'''Lies Alter (`int`) und ob die Person in Begleitung ist (`ja` oder `nein`) ein. Sie darf in den Film, wenn sie **mindestens 16** ist **oder** mindestens 12 **und** in Begleitung. Gib `Viel Spaß im Film!` oder `Leider nicht erlaubt.` aus.''',
      starter='int alter = int.Parse(Console.ReadLine());\nstring begleitung = Console.ReadLine();\n',
      solution='int alter = int.Parse(Console.ReadLine());\nstring begleitung = Console.ReadLine();\nbool mitBegleitung = begleitung == "ja";\nif (alter >= 16 || (alter >= 12 && mitBegleitung))\n    Console.WriteLine("Viel Spaß im Film!");\nelse\n    Console.WriteLine("Leider nicht erlaubt.");\n',
      tests=[T("Viel Spaß im Film!", "16", "nein"), T("Viel Spaß im Film!", "13", "ja"), T("Leider nicht erlaubt.", "13", "nein"), T("Leider nicht erlaubt.", "10", "ja")], compare="lenient"),
  ],
  quiz=[
    Q("Was ergibt true && false?", ["true", "false"], 1, "Bei UND müssen beide wahr sein."),
    Q("Was ergibt false || true?", ["true", "false"], 0, "Bei ODER reicht eine wahre Bedingung."),
    Q("Was ergibt !(5 > 3)?", ["true", "false", "5", "Fehler"], 1, "5 > 3 ist true, ! dreht es zu false."),
  ])

L(2, "l10", "Mehrfachauswahl mit switch",
  intro="Wenn du eine Variable mit vielen festen Werten vergleichst, ist switch übersichtlicher als viele else if.",
  explanation=r'''
## Aufbau
```
switch (zahl)
{
    case 1:
        Console.WriteLine("Eins");
        break;
    case 2:
        Console.WriteLine("Zwei");
        break;
    default:
        Console.WriteLine("Andere Zahl");
        break;
}
```
- `case 1:` – dieser Fall gilt, wenn `zahl == 1` ist.
- `break;` – beendet den Fall. **Jeder** case braucht ein `break;`.
- `default:` – gilt, wenn kein case passt (wie `else`).

## Mehrere Werte, ein Fall
```
case "Samstag":
case "Sonntag":
    Console.WriteLine("Wochenende");
    break;
```

## switch-Ausdruck (modern)
```
string text = note switch
{
    1 => "sehr gut",
    2 => "gut",
    _ => "anders"
};
```
`_` steht für „alles andere“.
''',
  example=r'''string farbe = "grün";

switch (farbe)
{
    case "rot":
        Console.WriteLine("Stopp!");
        break;
    case "gelb":
        Console.WriteLine("Achtung!");
        break;
    case "grün":
        Console.WriteLine("Fahren!");
        break;
    default:
        Console.WriteLine("Unbekannte Farbe");
        break;
}

int tag = 6;
string art = tag switch
{
    6 or 7 => "Wochenende",
    _ => "Werktag"
};
Console.WriteLine(art);
''',
  note="Vergisst du ein break; meldet der Compiler einen Fehler – in C# kann man nicht versehentlich in den nächsten case „durchfallen“.",
  exercises=[
    A("Wochentag-Nummer", r'''Lies eine Zahl von 1 bis 7 ein und gib mit `switch` den Wochentag aus (1 = `Montag` … 7 = `Sonntag`). Bei allen anderen Zahlen: `Ungültig`.''',
      starter='int nummer = int.Parse(Console.ReadLine());\n\nswitch (nummer)\n{\n    case 1:\n        Console.WriteLine("Montag");\n        break;\n}\n',
      solution='int nummer = int.Parse(Console.ReadLine());\n\nswitch (nummer)\n{\n    case 1: Console.WriteLine("Montag"); break;\n    case 2: Console.WriteLine("Dienstag"); break;\n    case 3: Console.WriteLine("Mittwoch"); break;\n    case 4: Console.WriteLine("Donnerstag"); break;\n    case 5: Console.WriteLine("Freitag"); break;\n    case 6: Console.WriteLine("Samstag"); break;\n    case 7: Console.WriteLine("Sonntag"); break;\n    default: Console.WriteLine("Ungültig"); break;\n}\n',
      tests=[T("Montag", "1"), T("Mittwoch", "3"), T("Sonntag", "7"), T("Ungültig", "8"), T("Ungültig", "0")], compare="lenient",
      requirements=[(r'\bswitch\b', "Benutze switch."), (r'\bdefault\b', "Denk an den default-Fall.")]),
    A("Mini-Taschenrechner", r'''Lies zwei ganze Zahlen und dann ein Rechenzeichen (`+`, `-`, `*` oder `/`) ein. Gib das Ergebnis mit `switch` aus, z. B. bei `8`, `2`, `*`:

```
Ergebnis: 16
```
Bei einem unbekannten Zeichen: `Unbekanntes Zeichen`. (`/` teilt ganzzahlig.)''',
      starter='int a = int.Parse(Console.ReadLine());\nint b = int.Parse(Console.ReadLine());\nstring op = Console.ReadLine();\n',
      solution='int a = int.Parse(Console.ReadLine());\nint b = int.Parse(Console.ReadLine());\nstring op = Console.ReadLine();\n\nswitch (op)\n{\n    case "+": Console.WriteLine($"Ergebnis: {a + b}"); break;\n    case "-": Console.WriteLine($"Ergebnis: {a - b}"); break;\n    case "*": Console.WriteLine($"Ergebnis: {a * b}"); break;\n    case "/": Console.WriteLine($"Ergebnis: {a / b}"); break;\n    default: Console.WriteLine("Unbekanntes Zeichen"); break;\n}\n',
      tests=[T("Ergebnis: 16", "8", "2", "*"), T("Ergebnis: 10", "8", "2", "+"), T("Ergebnis: 6", "8", "2", "-"), T("Ergebnis: 4", "8", "2", "/"), T("Unbekanntes Zeichen", "8", "2", "?")], compare="lenient",
      requirements=[(r'\bswitch\b', "Benutze switch.")]),
    A("Jahreszeit", r'''Lies eine Monatszahl (1–12) ein und gib die Jahreszeit aus. Fasse mehrere `case` zusammen:
- 12, 1, 2 → `Winter`
- 3, 4, 5 → `Frühling`
- 6, 7, 8 → `Sommer`
- 9, 10, 11 → `Herbst`
- sonst → `Ungültiger Monat`''',
      starter='int monat = int.Parse(Console.ReadLine());\n',
      solution='int monat = int.Parse(Console.ReadLine());\nswitch (monat)\n{\n    case 12:\n    case 1:\n    case 2:\n        Console.WriteLine("Winter");\n        break;\n    case 3:\n    case 4:\n    case 5:\n        Console.WriteLine("Frühling");\n        break;\n    case 6:\n    case 7:\n    case 8:\n        Console.WriteLine("Sommer");\n        break;\n    case 9:\n    case 10:\n    case 11:\n        Console.WriteLine("Herbst");\n        break;\n    default:\n        Console.WriteLine("Ungültiger Monat");\n        break;\n}\n',
      tests=[T("Winter", "1"), T("Winter", "12"), T("Frühling", "4"), T("Sommer", "8"), T("Herbst", "11"), T("Ungültiger Monat", "13")], compare="lenient",
      requirements=[(r'\bswitch\b', "Benutze switch.")]),
  ],
  quiz=[
    Q("Wofür steht default in einem switch?", ["Den ersten Fall", "Den Fall, wenn kein case passt", "Einen Fehler", "Das Ende des Programms"], 1, ""),
    Q("Womit endet jeder case-Block?", ["end;", "stop;", "break;", "}"], 2, ""),
    Q("Was bedeutet _ in einem switch-Ausdruck?", ["Leerzeichen", "Alles andere", "Null", "Fehler"], 1, ""),
  ])

L(2, "l11", "Die while-Schleife",
  intro="Schleifen wiederholen Code. Die while-Schleife läuft, solange eine Bedingung wahr ist.",
  explanation=r'''
## Aufbau
```
int i = 0;
while (i < 5)
{
    Console.WriteLine(i);
    i++;
}
```
1. Die **Bedingung** `i < 5` wird geprüft.
2. Ist sie wahr, läuft der **Schleifenkörper** in `{ }`.
3. Danach geht es zurück zu 1.

`i` ist die **Laufvariable** (der Zähler). Das `i++` am Ende ist wichtig: Ohne es bliebe `i` immer 0 und die Schleife liefe **endlos**.

## do-while
Prüft die Bedingung erst **am Ende** – der Körper läuft also mindestens einmal:
```
do
{
    Console.Write("Passwort: ");
    eingabe = Console.ReadLine();
} while (eingabe != "geheim");
```

## break
`break;` beendet die Schleife sofort.
''',
  example=r'''int countdown = 5;
while (countdown > 0)
{
    Console.WriteLine(countdown);
    countdown--;
}
Console.WriteLine("Start!");

// Summe 1 + 2 + ... + 10
int summe = 0;
int zahl = 1;
while (zahl <= 10)
{
    summe += zahl;
    zahl++;
}
Console.WriteLine("Summe: " + summe);
''',
  note="Läuft dein Programm endlos? Diese App stoppt es nach ein paar Sekunden automatisch – oder klick auf „Stopp“.",
  exercises=[
    A("Zählen bis 10", r'''Gib mit einer `while`-Schleife die Zahlen von 1 bis 10 aus, jede in einer eigenen Zeile.''',
      starter='int i = 1;\n',
      solution='int i = 1;\nwhile (i <= 10)\n{\n    Console.WriteLine(i);\n    i++;\n}\n',
      tests=[T("\n".join(str(i) for i in range(1, 11)))],
      requirements=[(r'\bwhile\s*\(', "Benutze eine while-Schleife.")]),
    A("Verdoppeln bis 1000", r'''Starte mit `int zahl = 1;` und verdopple sie in einer `while`-Schleife, solange sie kleiner als 1000 ist. Gib jede Zahl aus:

```
1
2
4
...
512
```''',
      starter='int zahl = 1;\n',
      solution='int zahl = 1;\nwhile (zahl < 1000)\n{\n    Console.WriteLine(zahl);\n    zahl *= 2;\n}\n',
      tests=[T("\n".join(str(2**i) for i in range(10)))],
      requirements=[(r'\bwhile\s*\(', "Benutze eine while-Schleife.")]),
    A("Passwort-Abfrage", r'''Frage mit einer Schleife so lange nach dem Passwort, bis `geheim` eingegeben wird. Bei falscher Eingabe gib `Falsch!` aus, am Ende `Willkommen!`.

Bei den Eingaben `abc`, `123`, `geheim`:
```
Falsch!
Falsch!
Willkommen!
```''',
      starter='string eingabe = Console.ReadLine();\n',
      solution='string eingabe = Console.ReadLine();\nwhile (eingabe != "geheim")\n{\n    Console.WriteLine("Falsch!");\n    eingabe = Console.ReadLine();\n}\nConsole.WriteLine("Willkommen!");\n',
      tests=[T("Falsch!\nFalsch!\nWillkommen!", "abc", "123", "geheim"), T("Willkommen!", "geheim")], compare="lenient",
      requirements=[(r'\bwhile\s*\(', "Benutze eine while- oder do-while-Schleife.")]),
    A("Summe bis 0", r'''Lies so lange Zahlen ein, bis `0` eingegeben wird. Gib dann die Summe aus. Bei `5`, `10`, `3`, `0`:

```
Summe: 18
```''',
      starter='int summe = 0;\n',
      solution='int summe = 0;\nint zahl = int.Parse(Console.ReadLine());\nwhile (zahl != 0)\n{\n    summe += zahl;\n    zahl = int.Parse(Console.ReadLine());\n}\nConsole.WriteLine("Summe: " + summe);\n',
      tests=[T("Summe: 18", "5", "10", "3", "0"), T("Summe: 0", "0"), T("Summe: -2", "4", "-6", "0")], compare="lenient"),
  ],
  quiz=[
    Q("Was passiert, wenn in einer while-Schleife das i++ fehlt (und i < 5 die Bedingung ist)?", ["Die Schleife läuft einmal", "Sie läuft gar nicht", "Endlosschleife", "Compilerfehler"], 2, ""),
    Q("Wie oft läuft eine do-while-Schleife mindestens?", ["0-mal", "1-mal", "2-mal", "Unendlich"], 1, ""),
    Q("Was macht break; in einer Schleife?", ["Pausiert", "Beendet die Schleife sofort", "Startet sie neu", "Nichts"], 1, ""),
  ])

L(2, "l12", "Die for-Schleife",
  intro="Wenn du vorher weißt, wie oft etwas wiederholt werden soll, ist die for-Schleife ideal.",
  explanation=r'''
## Aufbau
```
for (int i = 0; i < 5; i++)
{
    Console.WriteLine(i);
}
```
In den Klammern stehen **drei Teile**, getrennt durch `;`:
1. `int i = 0` – **Start**: Laufvariable anlegen und initialisieren.
2. `i < 5` – **Bedingung**: Solange sie wahr ist, läuft die Schleife.
3. `i++` – **Schritt**: passiert nach jedem Durchlauf.

Das ist genau dasselbe wie die while-Schleife – nur kompakter.

## Andere Schritte
- Rückwärts: `for (int i = 10; i >= 1; i--)`
- In Zweierschritten: `for (int i = 0; i <= 10; i += 2)`

## Verschachtelte Schleifen
Eine Schleife in einer Schleife – z. B. für Tabellen. Die innere Laufvariable heißt dann meist `j`.
''',
  example=r'''for (int i = 1; i <= 5; i++)
{
    Console.WriteLine($"Durchlauf {i}");
}

// Rückwärts in Zweierschritten
for (int i = 10; i > 0; i -= 2)
{
    Console.Write(i + " ");
}
Console.WriteLine();

// Kleines Dreieck
for (int zeile = 1; zeile <= 3; zeile++)
{
    for (int j = 0; j < zeile; j++)
    {
        Console.Write("*");
    }
    Console.WriteLine();
}
''',
  note="Die Laufvariable aus dem for-Kopf gibt es nur innerhalb der Schleife. Danach ist i „weg“.",
  exercises=[
    A("Fünferreihe", r'''Gib mit einer `for`-Schleife die Fünferreihe aus:

```
1 x 5 = 5
2 x 5 = 10
...
10 x 5 = 50
```''',
      starter='',
      solution='for (int i = 1; i <= 10; i++)\n{\n    Console.WriteLine($"{i} x 5 = {i * 5}");\n}\n',
      tests=[T("\n".join(f"{i} x 5 = {i*5}" for i in range(1, 11)))],
      requirements=[(r'\bfor\s*\(', "Benutze eine for-Schleife.")]),
    A("Countdown", r'''Lies eine Zahl `n` ein und zähle mit einer `for`-Schleife von `n` bis 1 rückwärts. Danach `Abflug!`. Bei `3`:

```
3
2
1
Abflug!
```''',
      starter='int n = int.Parse(Console.ReadLine());\n',
      solution='int n = int.Parse(Console.ReadLine());\nfor (int i = n; i >= 1; i--)\n{\n    Console.WriteLine(i);\n}\nConsole.WriteLine("Abflug!");\n',
      tests=[T("3\n2\n1\nAbflug!", "3"), T("1\nAbflug!", "1"), T("5\n4\n3\n2\n1\nAbflug!", "5")], compare="exact",
      requirements=[(r'\bfor\s*\(', "Benutze eine for-Schleife.")],
      hint="Bei dieser Aufgabe bitte keine Eingabeaufforderung ausgeben – nur die Zahlen."),
    A("Summe der geraden Zahlen", r'''Berechne mit einer `for`-Schleife die Summe aller geraden Zahlen von 2 bis 100 (2 + 4 + … + 100):

```
Summe: 2550
```''',
      starter='int summe = 0;\n',
      solution='int summe = 0;\nfor (int i = 2; i <= 100; i += 2)\n{\n    summe += i;\n}\nConsole.WriteLine("Summe: " + summe);\n',
      tests=[T("Summe: 2550")],
      requirements=[(r'\bfor\s*\(', "Benutze eine for-Schleife.")]),
    A("Sterne-Treppe", r'''Lies eine Zahl `n` ein und gib eine Treppe aus `n` Zeilen aus (verschachtelte Schleifen). Bei `4`:

```
*
**
***
****
```''',
      starter='int n = int.Parse(Console.ReadLine());\n',
      solution='int n = int.Parse(Console.ReadLine());\nfor (int zeile = 1; zeile <= n; zeile++)\n{\n    for (int j = 0; j < zeile; j++)\n    {\n        Console.Write("*");\n    }\n    Console.WriteLine();\n}\n',
      tests=[T("*\n**\n***\n****", "4"), T("*\n**", "2")], compare="exact",
      requirements=[(r'\bfor\s*\(', "Benutze for-Schleifen.")],
      hint="Gib keine Eingabeaufforderung aus, nur die Sterne."),
  ],
  quiz=[
    Q("Wie oft läuft for (int i = 0; i < 3; i++)?", ["2-mal", "3-mal", "4-mal", "Endlos"], 1, "i = 0, 1, 2 – also dreimal."),
    Q("Welcher Teil des for-Kopfs wird nach jedem Durchlauf ausgeführt?", ["Der Start", "Die Bedingung", "Der Schritt (z. B. i++)", "Keiner"], 2, ""),
    Q("Was gibt for (int i = 10; i > 0; i -= 5) Console.Write(i + \" \"); aus?", ["10 5", "10 5 0", "5 0", "10"], 0, ""),
  ])

L(2, "l13", "Arrays",
  intro="Ein Array speichert viele Werte gleichen Typs unter einem Namen – wie eine Reihe nummerierter Fächer.",
  explanation=r'''
## Array anlegen
```
int[] zahlen = new int[5];          // 5 Fächer, alle 0
string[] namen = { "Ana", "Ben" };  // gleich mit Werten
```
- `int[]` heißt „Array von int“.
- `new int[5]` legt **5** Fächer an.

## Index
Jedes Fach hat eine Nummer, den **Index**. Er beginnt bei **0**!
```
zahlen[0] = 42;      // erstes Fach
zahlen[4] = 7;       // letztes Fach (bei Länge 5)
Console.WriteLine(namen[1]);  // Ben
```
Greifst du auf `zahlen[5]` zu, stürzt das Programm ab (**IndexOutOfRangeException**).

## Länge
`zahlen.Length` ist die Anzahl der Fächer. (`zahlen.Count()` geht auch – das benutzt LINQ.)

## Array mit Schleife durchlaufen
```
for (int i = 0; i < zahlen.Length; i++)
{
    Console.WriteLine(zahlen[i]);
}
```
Deshalb `<` und nicht `<=`: Der letzte Index ist `Length - 1`.
''',
  example=r'''int[] noten = { 2, 1, 3, 2, 4 };

Console.WriteLine("Erste Note: " + noten[0]);
Console.WriteLine("Anzahl: " + noten.Length);

int summe = 0;
for (int i = 0; i < noten.Length; i++)
{
    Console.WriteLine($"Note {i + 1}: {noten[i]}");
    summe += noten[i];
}
Console.WriteLine("Summe: " + summe);

int[] anzahl3 = new int[100];
int j = 0;
while (j < anzahl3.Count())
{
    anzahl3[j] = j;
    j++;
}
Console.WriteLine("Letztes Fach: " + anzahl3[99]);
''',
  note="Die Länge eines Arrays ist fest. Brauchst du eine Liste, die wachsen kann, nimm List<T> (kommt gleich).",
  exercises=[
    A("Wochentage", r'''Lege ein `string`-Array mit den Tagen `Mo`, `Di`, `Mi`, `Do`, `Fr` an. Gib das **erste** und das **letzte** Element aus und dann die Länge:

```
Mo
Fr
5
```''',
      starter='string[] tage = { };\n',
      solution='string[] tage = { "Mo", "Di", "Mi", "Do", "Fr" };\nConsole.WriteLine(tage[0]);\nConsole.WriteLine(tage[tage.Length - 1]);\nConsole.WriteLine(tage.Length);\n',
      tests=[T("Mo\nFr\n5")]),
    A("Quadratzahlen", r'''Lege ein `int`-Array mit 10 Fächern an. Fülle es mit einer Schleife so, dass in Fach `i` der Wert `i * i` steht. Gib danach alle Werte in **einer** Zeile aus, getrennt durch Leerzeichen:

```
0 1 4 9 16 25 36 49 64 81
```''',
      starter='int[] quadrate = new int[10];\n',
      solution='int[] quadrate = new int[10];\nfor (int i = 0; i < quadrate.Length; i++)\n{\n    quadrate[i] = i * i;\n}\nfor (int i = 0; i < quadrate.Length; i++)\n{\n    Console.Write(quadrate[i] + " ");\n}\nConsole.WriteLine();\n',
      tests=[T("0 1 4 9 16 25 36 49 64 81")],
      hint="Leerzeichen am Zeilenende sind egal."),
    A("Größter Wert", r'''Finde im Array `{ 12, 45, 7, 89, 23, 56 }` mit einer Schleife den größten Wert (ohne `Max()`!):

```
Maximum: 89
```''',
      starter='int[] werte = { 12, 45, 7, 89, 23, 56 };\nint max = werte[0];\n',
      solution='int[] werte = { 12, 45, 7, 89, 23, 56 };\nint max = werte[0];\nfor (int i = 1; i < werte.Length; i++)\n{\n    if (werte[i] > max)\n        max = werte[i];\n}\nConsole.WriteLine("Maximum: " + max);\n',
      tests=[T("Maximum: 89")],
      requirements=[(r'\bfor\s*\(|\bforeach\s*\(|\bwhile\s*\(', "Benutze eine Schleife.")]),
    A("Eingaben speichern", r'''Lies 3 Zahlen ein und speichere sie in einem Array. Gib sie danach in **umgekehrter** Reihenfolge aus. Bei `1`, `2`, `3`:

```
3
2
1
```''',
      starter='int[] zahlen = new int[3];\n',
      solution='int[] zahlen = new int[3];\nfor (int i = 0; i < 3; i++)\n{\n    zahlen[i] = int.Parse(Console.ReadLine());\n}\nfor (int i = zahlen.Length - 1; i >= 0; i--)\n{\n    Console.WriteLine(zahlen[i]);\n}\n',
      tests=[T("3\n2\n1", "1", "2", "3"), T("9\n-4\n7", "7", "-4", "9")], compare="exact",
      hint="Gib keine Eingabeaufforderung aus – nur die Zahlen."),
  ],
  quiz=[
    Q("Welchen Index hat das erste Element eines Arrays?", ["1", "0", "-1", "Das kommt drauf an"], 1, ""),
    Q("Ein Array hat Length 5. Was ist der letzte gültige Index?", ["5", "4", "6", "0"], 1, ""),
    Q("Was passiert bei zahlen[10], wenn das Array nur 5 Fächer hat?", ["Ergebnis 0", "IndexOutOfRangeException", "Das Array wächst", "Nichts"], 1, ""),
  ])

L(2, "l14", "foreach",
  intro="foreach geht jedes Element einer Sammlung durch – ganz ohne Index und Zähler.",
  explanation=r'''
## Aufbau
```
string[] namen = { "Ana", "Ben", "Cem" };
foreach (string name in namen)
{
    Console.WriteLine(name);
}
```
Lies es so: „**Für jeden** `name` **in** `namen` …“. Bei jedem Durchlauf steht in `name` das nächste Element.

## for oder foreach?
- `foreach` – wenn du einfach **alle** Elemente durchgehen willst. Kürzer und ohne Fehlerquelle beim Index.
- `for` – wenn du den **Index** brauchst oder Elemente **ändern** willst.

In der foreach-Schleife darfst du die Laufvariable nicht ändern (`name = "x";` geht nicht).

## var
Statt den Typ hinzuschreiben, kannst du `var` benutzen – C# erkennt den Typ selbst:
`foreach (var name in namen)`
''',
  example=r'''int[] punkte = { 10, 25, 7, 40 };
int summe = 0;

foreach (int p in punkte)
{
    Console.WriteLine("Punkte: " + p);
    summe += p;
}
Console.WriteLine("Gesamt: " + summe);

foreach (char buchstabe in "Hallo")
{
    Console.Write(buchstabe + "-");
}
Console.WriteLine();
''',
  note="foreach funktioniert mit Arrays, Listen und sogar Strings (Zeichen für Zeichen).",
  exercises=[
    A("Einkaufsliste", r'''Gib alle Einträge des Arrays mit `foreach` aus, jeweils mit einem `- ` davor:

```
- Milch
- Brot
- Eier
- Äpfel
```''',
      starter='string[] einkauf = { "Milch", "Brot", "Eier", "Äpfel" };\n',
      solution='string[] einkauf = { "Milch", "Brot", "Eier", "Äpfel" };\nforeach (string artikel in einkauf)\n{\n    Console.WriteLine("- " + artikel);\n}\n',
      tests=[T("- Milch\n- Brot\n- Eier\n- Äpfel")],
      requirements=[(r'\bforeach\s*\(', "Benutze foreach.")]),
    A("Durchschnitt", r'''Berechne mit `foreach` den Durchschnitt der Temperaturen und gib ihn mit einer Nachkommastelle aus (`:F1`):

```
Durchschnitt: 18,4 Grad
```''',
      starter='double[] temperaturen = { 15.5, 18.0, 21.5, 17.0, 20.0 };\n',
      solution='double[] temperaturen = { 15.5, 18.0, 21.5, 17.0, 20.0 };\ndouble summe = 0;\nforeach (double t in temperaturen)\n{\n    summe += t;\n}\nConsole.WriteLine($"Durchschnitt: {summe / temperaturen.Length:F1} Grad");\n',
      tests=[T("Durchschnitt: 18,4 Grad")],
      requirements=[(r'\bforeach\s*\(', "Benutze foreach.")]),
    A("Vokale zählen", r'''Lies ein Wort ein und zähle mit `foreach` die Vokale (a, e, i, o, u – nur Kleinbuchstaben). Bei `programmieren`:

```
Vokale: 5
```''',
      starter='string wort = Console.ReadLine();\nint vokale = 0;\n',
      solution='string wort = Console.ReadLine();\nint vokale = 0;\nforeach (char c in wort)\n{\n    if (c == \'a\' || c == \'e\' || c == \'i\' || c == \'o\' || c == \'u\')\n        vokale++;\n}\nConsole.WriteLine("Vokale: " + vokale);\n',
      tests=[T("Vokale: 5", "programmieren"), T("Vokale: 0", "xyz"), T("Vokale: 2", "hallo")], compare="lenient",
      requirements=[(r'\bforeach\s*\(', "Benutze foreach.")],
      hint="Ein einzelnes Zeichen vergleichst du mit einfachen Anführungszeichen: c == 'a'"),
  ],
  quiz=[
    Q("Was steht bei foreach (int x in zahlen) in x?", ["Der Index", "Das aktuelle Element", "Die Länge", "Immer 0"], 1, ""),
    Q("Wann ist eine for-Schleife besser als foreach?", ["Wenn man den Index braucht", "Nie", "Bei Strings", "Bei kurzen Arrays"], 0, ""),
    Q("Kann man mit foreach durch einen string laufen?", ["Nein", "Ja, Zeichen für Zeichen", "Nur mit var", "Nur rückwärts"], 1, ""),
  ])

L(2, "l15", "Listen (List<T>)",
  intro="Eine Liste ist wie ein Array, das wachsen und schrumpfen kann. In der Praxis benutzt man sie sehr oft.",
  explanation=r'''
## Liste anlegen
```
List<string> namen = new List<string>();
List<int> zahlen = new List<int> { 3, 1, 4 };
```
In den spitzen Klammern `< >` steht der Typ der Elemente.

## Wichtige Befehle
- `namen.Add("Ana");` – hinten anhängen
- `namen.Remove("Ana");` – ein Element entfernen
- `namen.RemoveAt(0);` – das Element an Index 0 entfernen
- `namen.Count` – Anzahl der Elemente (bei Arrays heißt das `Length`)
- `namen[0]` – Zugriff per Index, wie beim Array
- `namen.Contains("Ben")` – ist das Element drin? (`true`/`false`)
- `namen.Sort();` – sortieren
- `namen.Clear();` – alles löschen

## Durchlaufen
Mit `foreach` oder mit `for (int i = 0; i < namen.Count; i++)`.
''',
  example=r'''List<string> aufgaben = new List<string>();
aufgaben.Add("Einkaufen");
aufgaben.Add("Lernen");
aufgaben.Add("Sport");

Console.WriteLine("Anzahl: " + aufgaben.Count);

aufgaben.Remove("Sport");
aufgaben.Insert(0, "Aufstehen");

foreach (string a in aufgaben)
{
    Console.WriteLine("- " + a);
}

Console.WriteLine("Lernen dabei? " + aufgaben.Contains("Lernen"));
''',
  note="Arrays: .Length – Listen: .Count. Das verwechselt anfangs fast jeder!",
  exercises=[
    A("Lieblingsfilme", r'''Lege eine leere `List<string>` an, füge mit `Add` die Filme `Matrix`, `Shrek` und `Up` hinzu und gib dann aus:

```
3 Filme:
Matrix
Shrek
Up
```''',
      starter='List<string> filme = new List<string>();\n',
      solution='List<string> filme = new List<string>();\nfilme.Add("Matrix");\nfilme.Add("Shrek");\nfilme.Add("Up");\nConsole.WriteLine(filme.Count + " Filme:");\nforeach (string f in filme)\n    Console.WriteLine(f);\n',
      tests=[T("3 Filme:\nMatrix\nShrek\nUp")],
      requirements=[(r'\.\s*Add\s*\(', "Benutze Add."), (r'\.\s*Count\b', "Benutze Count für die Anzahl.")]),
    A("Zahlen sammeln", r'''Lies Zahlen ein, bis `0` kommt, und speichere sie in einer `List<int>`. Gib danach Anzahl, Summe und die sortierte Liste aus. Bei `5`, `2`, `8`, `0`:

```
Anzahl: 3
Summe: 15
Sortiert: 2 5 8
```''',
      starter='List<int> zahlen = new List<int>();\n',
      solution='List<int> zahlen = new List<int>();\nint z = int.Parse(Console.ReadLine());\nwhile (z != 0)\n{\n    zahlen.Add(z);\n    z = int.Parse(Console.ReadLine());\n}\nint summe = 0;\nforeach (int x in zahlen) summe += x;\nzahlen.Sort();\nConsole.WriteLine("Anzahl: " + zahlen.Count);\nConsole.WriteLine("Summe: " + summe);\nConsole.Write("Sortiert:");\nforeach (int x in zahlen) Console.Write(" " + x);\nConsole.WriteLine();\n',
      tests=[T("Anzahl: 3\nSumme: 15\nSortiert: 2 5 8", "5", "2", "8", "0"), T("Anzahl: 1\nSumme: 4\nSortiert: 4", "4", "0")], compare="lenient",
      hint="Tipp: string.Join(\" \", zahlen) baut alle Elemente mit Leerzeichen zu einem Text zusammen."),
    A("Gästeliste", r'''Die Gästeliste enthält `Ana`, `Ben`, `Cem`. Lies einen Namen ein. Steht er auf der Liste, entferne ihn und gib `Ben wurde ausgeladen.` aus – sonst `Ben steht nicht auf der Liste.`. Gib danach die Anzahl aus: `Noch 2 Gäste`.''',
      starter='List<string> gaeste = new List<string> { "Ana", "Ben", "Cem" };\nstring name = Console.ReadLine();\n',
      solution='List<string> gaeste = new List<string> { "Ana", "Ben", "Cem" };\nstring name = Console.ReadLine();\nif (gaeste.Contains(name))\n{\n    gaeste.Remove(name);\n    Console.WriteLine($"{name} wurde ausgeladen.");\n}\nelse\n{\n    Console.WriteLine($"{name} steht nicht auf der Liste.");\n}\nConsole.WriteLine($"Noch {gaeste.Count} Gäste");\n',
      tests=[T("Ben wurde ausgeladen.\nNoch 2 Gäste", "Ben"), T("Zoe steht nicht auf der Liste.\nNoch 3 Gäste", "Zoe")], compare="lenient",
      requirements=[(r'\.\s*Contains\s*\(', "Benutze Contains."), (r'\.\s*Remove\s*\(', "Benutze Remove.")]),
  ],
  quiz=[
    Q("Wie fügst du einer Liste ein Element hinzu?", ["liste.Push(x)", "liste.Add(x)", "liste[liste.Count] = x", "liste + x"], 1, ""),
    Q("Wie bekommst du die Anzahl der Elemente einer List?", [".Length", ".Count", ".Size", ".Anzahl"], 1, ""),
    Q("Was ist der Hauptvorteil einer Liste gegenüber einem Array?", ["Sie ist schneller", "Sie kann wachsen und schrumpfen", "Sie beginnt bei Index 1", "Sie kann nur Text speichern"], 1, ""),
  ])
