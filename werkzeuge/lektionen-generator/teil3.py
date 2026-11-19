# Stufe 3: Methoden
from gen import L, A, T, Q

L(3, "l16", "Methoden",
  intro="Mit Methoden gibst du einem Stück Code einen Namen. Dann kannst du es immer wieder aufrufen, statt es zu kopieren.",
  explanation=r'''
## Eine Methode schreiben
```
static void SagHallo()
{
    Console.WriteLine("Hallo!");
}
```
- `static void` – „gibt nichts zurück“ (`void` = leer). `static` erklären wir bei den Klassen.
- `SagHallo` – der Name. Methodennamen schreibt man groß (PascalCase).
- `()` – hier stehen später die **Parameter**.

## Aufrufen
`SagHallo();` – der Code in der Methode wird ausgeführt.

## Parameter
Mit Parametern gibst du der Methode Werte mit:
```
static void Begruesse(string name, int alter)
{
    Console.WriteLine($"Hallo {name}, du bist {alter}.");
}

Begruesse("Ana", 30);   // "Ana" und 30 sind die Argumente
```

## Wo steht die Methode?
Bei Top-Level-Code schreibst du Methoden **unter** deinen Hauptcode. Im klassischen Aufbau stehen sie in `class Program` neben `Main`.
''',
  example=r'''Trennlinie();
Begruesse("Ana");
Begruesse("Ben");
Trennlinie();
Zeichne('#', 10);

static void Trennlinie()
{
    Console.WriteLine("--------------");
}

static void Begruesse(string name)
{
    Console.WriteLine($"Hallo {name}, schön dass du da bist!");
}

static void Zeichne(char zeichen, int anzahl)
{
    for (int i = 0; i < anzahl; i++)
        Console.Write(zeichen);
    Console.WriteLine();
}
''',
  note="Eine Methode macht nichts, solange du sie nicht aufrufst! Vergiss nicht die Klammern beim Aufruf: SagHallo();",
  exercises=[
    A("Erste Methode", r'''Schreib eine Methode `static void Willkommen()`, die `Willkommen im Kurs!` ausgibt. Ruf sie **zweimal** auf:

```
Willkommen im Kurs!
Willkommen im Kurs!
```''',
      starter='// Rufe hier die Methode auf\n\n\n// Schreib hier die Methode\n',
      solution='Willkommen();\nWillkommen();\n\nstatic void Willkommen()\n{\n    Console.WriteLine("Willkommen im Kurs!");\n}\n',
      tests=[T("Willkommen im Kurs!\nWillkommen im Kurs!")],
      requirements=[(r'void\s+Willkommen\s*\(\s*\)', "Schreib eine Methode static void Willkommen().")]),
    A("Methode mit Parameter", r'''Schreib eine Methode `static void Quadrat(int zahl)`, die das Quadrat ausgibt, z. B. `5 zum Quadrat ist 25`. Ruf sie mit 3, 5 und 12 auf:

```
3 zum Quadrat ist 9
5 zum Quadrat ist 25
12 zum Quadrat ist 144
```''',
      starter='Quadrat(3);\nQuadrat(5);\nQuadrat(12);\n\n',
      solution='Quadrat(3);\nQuadrat(5);\nQuadrat(12);\n\nstatic void Quadrat(int zahl)\n{\n    Console.WriteLine($"{zahl} zum Quadrat ist {zahl * zahl}");\n}\n',
      tests=[T("3 zum Quadrat ist 9\n5 zum Quadrat ist 25\n12 zum Quadrat ist 144")]),
    A("Rahmen zeichnen", r'''Schreib eine Methode `static void Rahmen(string text)`, die den Text in einen Rahmen setzt. Die Linie ist so lang wie der Text plus 4. Bei `Rahmen("Hallo");`:

```
*********
* Hallo *
*********
```
Ruf sie mit `Hallo` und danach mit `C#` auf.''',
      starter='Rahmen("Hallo");\nRahmen("C#");\n\n',
      solution='Rahmen("Hallo");\nRahmen("C#");\n\nstatic void Rahmen(string text)\n{\n    string linie = new string(\'*\', text.Length + 4);\n    Console.WriteLine(linie);\n    Console.WriteLine("* " + text + " *");\n    Console.WriteLine(linie);\n}\n',
      tests=[T("*********\n* Hallo *\n*********\n******\n* C# *\n******")],
      hint="text.Length ist die Anzahl der Zeichen. Eine Linie geht mit einer for-Schleife oder mit new string('*', anzahl)."),
  ],
  quiz=[
    Q("Was bedeutet void bei einer Methode?", ["Sie ist leer", "Sie gibt nichts zurück", "Sie ist privat", "Sie ist fehlerhaft"], 1, ""),
    Q("Wie rufst du die Methode Drucken ohne Parameter auf?", ["Drucken;", "call Drucken;", "Drucken();", "void Drucken();"], 2, ""),
    Q("Bei Begruesse(\"Ana\") – was ist \"Ana\"?", ["Ein Parameter in der Definition", "Ein Argument", "Der Rückgabewert", "Ein Kommentar"], 1, "In der Definition heißt es Parameter, beim Aufruf Argument."),
  ])

L(3, "l17", "Methoden mit Rückgabewert",
  intro="Viele Methoden rechnen etwas aus und geben das Ergebnis mit return zurück.",
  explanation=r'''
## return
```
static int Addiere(int a, int b)
{
    return a + b;
}

int summe = Addiere(3, 4);   // summe ist 7
```
- Statt `void` steht der **Rückgabetyp** vor dem Namen (`int`, `double`, `string`, `bool` …).
- `return` gibt den Wert zurück und **beendet** die Methode sofort.

## Ergebnis benutzen
Den Rückgabewert kannst du speichern, ausgeben oder direkt weiterverwenden:
```
Console.WriteLine(Addiere(10, 5));
if (IstGerade(8)) { ... }
```

## Jeder Weg braucht ein return
Bei `if`/`else` muss **jeder** Fall etwas zurückgeben – sonst meldet der Compiler CS0161.

## Kurzschreibweise
Bei einer Zeile geht auch: `static int Doppelt(int x) => x * 2;`
''',
  example=r'''Console.WriteLine(Addiere(3, 4));
Console.WriteLine(Flaeche(2.5, 4));
Console.WriteLine(IstGerade(10));
Console.WriteLine(Note(87));

static int Addiere(int a, int b)
{
    return a + b;
}

static double Flaeche(double breite, double hoehe) => breite * hoehe;

static bool IstGerade(int zahl)
{
    return zahl % 2 == 0;
}

static string Note(int punkte)
{
    if (punkte >= 90) return "sehr gut";
    if (punkte >= 75) return "gut";
    return "weiter üben";
}
''',
  note="Eine Methode sollte genau eine Sache gut machen. Lieber zwei kleine als eine riesige Methode.",
  exercises=[
    A("Maximum", r'''Schreib eine Methode `static int Max(int a, int b)`, die die größere Zahl **zurückgibt** (nicht ausgibt!). Der Hauptcode ist schon da.

```
9
12
5
```''',
      starter='Console.WriteLine(Max(9, 4));\nConsole.WriteLine(Max(3, 12));\nConsole.WriteLine(Max(5, 5));\n\n',
      solution='Console.WriteLine(Max(9, 4));\nConsole.WriteLine(Max(3, 12));\nConsole.WriteLine(Max(5, 5));\n\nstatic int Max(int a, int b)\n{\n    if (a > b)\n        return a;\n    else\n        return b;\n}\n',
      tests=[T("9\n12\n5")],
      requirements=[(r'\breturn\b', "Gib das Ergebnis mit return zurück.")]),
    A("Brutto-Preis", r'''Schreib `static double Brutto(double netto)`, die den Preis plus 19 % Mehrwertsteuer zurückgibt (`netto * 1.19`). Gib damit aus:

```
100 Euro netto = 119,00 Euro brutto
50 Euro netto = 59,50 Euro brutto
```''',
      starter='',
      solution='Console.WriteLine($"100 Euro netto = {Brutto(100):F2} Euro brutto");\nConsole.WriteLine($"50 Euro netto = {Brutto(50):F2} Euro brutto");\n\nstatic double Brutto(double netto)\n{\n    return netto * 1.19;\n}\n',
      tests=[T("100 Euro netto = 119,00 Euro brutto\n50 Euro netto = 59,50 Euro brutto")],
      requirements=[(r'double\s+Brutto\s*\(', "Schreib eine Methode static double Brutto(double netto).")]),
    A("Primzahl-Test", r'''Schreib `static bool IstPrimzahl(int n)`. Eine Primzahl ist größer als 1 und nur durch 1 und sich selbst teilbar. Lies eine Zahl ein und gib `17 ist eine Primzahl` bzw. `12 ist keine Primzahl` aus.''',
      starter='int zahl = int.Parse(Console.ReadLine());\n\n',
      solution='int zahl = int.Parse(Console.ReadLine());\n\nif (IstPrimzahl(zahl))\n    Console.WriteLine($"{zahl} ist eine Primzahl");\nelse\n    Console.WriteLine($"{zahl} ist keine Primzahl");\n\nstatic bool IstPrimzahl(int n)\n{\n    if (n < 2) return false;\n    for (int i = 2; i < n; i++)\n    {\n        if (n % i == 0) return false;\n    }\n    return true;\n}\n',
      tests=[T("17 ist eine Primzahl", "17"), T("12 ist keine Primzahl", "12"), T("2 ist eine Primzahl", "2"), T("1 ist keine Primzahl", "1"), T("97 ist eine Primzahl", "97")], compare="lenient",
      requirements=[(r'bool\s+IstPrimzahl\s*\(', "Schreib eine Methode static bool IstPrimzahl(int n).")],
      hint="Prüfe mit einer Schleife, ob n % i == 0 für ein i zwischen 2 und n-1. Dann ist es keine Primzahl."),
    A("Fakultät", r'''Schreib `static long Fakultaet(int n)`, die 1 · 2 · 3 · … · n berechnet. Gib die Fakultäten von 1 bis 10 aus:

```
1! = 1
2! = 2
3! = 6
...
10! = 3628800
```''',
      starter='',
      solution='for (int i = 1; i <= 10; i++)\n{\n    Console.WriteLine($"{i}! = {Fakultaet(i)}");\n}\n\nstatic long Fakultaet(int n)\n{\n    long ergebnis = 1;\n    for (int i = 2; i <= n; i++)\n        ergebnis *= i;\n    return ergebnis;\n}\n',
      tests=[T("1! = 1\n2! = 2\n3! = 6\n4! = 24\n5! = 120\n6! = 720\n7! = 5040\n8! = 40320\n9! = 362880\n10! = 3628800")],
      requirements=[(r'Fakultaet\s*\(', "Schreib eine Methode Fakultaet.")]),
  ],
  quiz=[
    Q("Was macht return?", ["Gibt einen Wert zurück und beendet die Methode", "Startet die Methode neu", "Gibt Text aus", "Springt zur nächsten Zeile"], 0, ""),
    Q("Welcher Rückgabetyp passt zu einer Methode, die true oder false liefert?", ["void", "int", "bool", "string"], 2, ""),
    Q("Was gibt Console.WriteLine(Doppelt(Doppelt(3))); aus, wenn Doppelt(x) => x * 2 ist?", ["6", "9", "12", "3"], 2, "Doppelt(3) = 6, Doppelt(6) = 12."),
  ])

L(3, "l18", "String-Methoden",
  intro="Strings bringen viele praktische Werkzeuge mit: Länge, Groß-/Kleinschreibung, Suchen, Ersetzen, Zerlegen …",
  explanation=r'''
## Die wichtigsten
- `text.Length` – Anzahl der Zeichen (ohne Klammern – es ist eine **Eigenschaft**)
- `text.ToUpper()` / `text.ToLower()` – alles groß / klein
- `text.Trim()` – Leerzeichen am Anfang und Ende entfernen
- `text.Contains("ab")` – kommt „ab“ vor? (`true`/`false`)
- `text.StartsWith("Ha")` / `text.EndsWith("!")`
- `text.IndexOf("l")` – an welcher Stelle steht „l“ zum ersten Mal? (`-1` = gar nicht)
- `text.Replace("alt", "neu")` – ersetzen
- `text.Substring(start, länge)` – ein Stück herausschneiden
- `text.Split(' ')` – zerlegen, Ergebnis ist ein **Array**
- `text[0]` – das erste Zeichen (ein `char`)

## Wichtig: Strings ändern sich nicht
`text.ToUpper();` allein bewirkt nichts! Die Methode gibt einen **neuen** String zurück:
```
text = text.ToUpper();
```

## Zusammenfügen
`string.Join(", ", liste)` verbindet alle Elemente mit `, ` dazwischen.
''',
  example=r'''string satz = "  C# macht richtig Spaß  ";
satz = satz.Trim();

Console.WriteLine(satz);
Console.WriteLine("Länge: " + satz.Length);
Console.WriteLine(satz.ToUpper());
Console.WriteLine(satz.Replace("Spaß", "Freude"));
Console.WriteLine("Enthält 'macht': " + satz.Contains("macht"));
Console.WriteLine("Erstes Zeichen: " + satz[0]);
Console.WriteLine("Die ersten 7: " + satz.Substring(0, 7));

string[] woerter = satz.Split(' ');
Console.WriteLine("Wörter: " + woerter.Length);
Console.WriteLine(string.Join(" | ", woerter));
''',
  note="Substring(start, länge): Der Start ist ein Index (beginnt bei 0). Schneidest du über das Ende hinaus, gibt es eine ArgumentOutOfRangeException.",
  exercises=[
    A("Namen formatieren", r'''Lies einen Namen ein, der unordentlich geschrieben ist (z. B. `  mAx  `). Entferne die Leerzeichen und gib ihn so aus: erster Buchstabe groß, Rest klein:

```
Max
```''',
      starter='string name = Console.ReadLine();\n',
      solution='string name = Console.ReadLine();\nname = name.Trim();\nstring schoen = name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower();\nConsole.WriteLine(schoen);\n',
      tests=[T("Max", "  mAx  "), T("Anna", "ANNA"), T("Lu", "lu")], compare="exact",
      hint="Trim(), dann Substring(0, 1).ToUpper() + Substring(1).ToLower(). Gib keine Eingabeaufforderung aus."),
    A("Palindrom", r'''Ein Palindrom liest sich vorwärts und rückwärts gleich (z. B. `Anna`, `Otto`). Lies ein Wort ein, ignoriere Groß-/Kleinschreibung und gib `Palindrom` oder `kein Palindrom` aus.''',
      starter='string wort = Console.ReadLine();\n',
      solution='string wort = Console.ReadLine().ToLower();\nstring rueckwaerts = "";\nfor (int i = wort.Length - 1; i >= 0; i--)\n{\n    rueckwaerts += wort[i];\n}\nif (wort == rueckwaerts)\n    Console.WriteLine("Palindrom");\nelse\n    Console.WriteLine("kein Palindrom");\n',
      tests=[T("Palindrom", "Anna"), T("Palindrom", "Reliefpfeiler"), T("kein Palindrom", "Hallo")], compare="lenient",
      hint="Bau den umgedrehten Text mit einer Schleife, die bei wort.Length - 1 startet."),
    A("Wörter zählen", r'''Lies einen Satz ein, zerlege ihn mit `Split(' ')` und gib die Anzahl der Wörter und das längste Wort aus. Bei `Ich lerne gerade programmieren`:

```
Wörter: 4
Längstes Wort: programmieren
```''',
      starter='string satz = Console.ReadLine();\n',
      solution='string satz = Console.ReadLine();\nstring[] woerter = satz.Split(\' \');\nstring laengstes = "";\nforeach (string w in woerter)\n{\n    if (w.Length > laengstes.Length)\n        laengstes = w;\n}\nConsole.WriteLine("Wörter: " + woerter.Length);\nConsole.WriteLine("Längstes Wort: " + laengstes);\n',
      tests=[T("Wörter: 4\nLängstes Wort: programmieren", "Ich lerne gerade programmieren"), T("Wörter: 3\nLängstes Wort: Banane", "Apfel Banane Kiwi")], compare="lenient",
      requirements=[(r'\.\s*Split\s*\(', "Benutze Split.")]),
  ],
  quiz=[
    Q("Was gibt \"Hallo\".Length zurück?", ["4", "5", "6", "\"Hallo\""], 1, ""),
    Q("text = \"abc\"; text.ToUpper(); Console.WriteLine(text); – was wird ausgegeben?", ["ABC", "abc", "Abc", "Fehler"], 1, "Strings ändern sich nicht: Das Ergebnis von ToUpper() wurde nicht gespeichert."),
    Q("Was liefert \"Hallo Welt\".Split(' ')?", ["Einen string", "Ein Array mit 2 Elementen", "Die Zahl 2", "true"], 1, ""),
  ])

L(3, "l19", "Sichere Eingabe mit TryParse",
  intro="int.Parse stürzt bei falschen Eingaben ab. Mit int.TryParse prüfst du vorher, ob die Eingabe eine Zahl ist.",
  explanation=r'''
## Das Problem
```
int zahl = int.Parse("abc");   // FormatException – Absturz!
```

## Die Lösung: TryParse
```
string eingabe = Console.ReadLine();
if (int.TryParse(eingabe, out int zahl))
{
    Console.WriteLine("Danke, " + zahl);
}
else
{
    Console.WriteLine("Das war keine Zahl!");
}
```
- `TryParse` versucht die Umwandlung und gibt **true** (geklappt) oder **false** (nicht geklappt) zurück.
- Das Ergebnis landet in der Variable hinter `out`. Klappt es nicht, ist sie `0`.

## Wiederholen bis es passt
```
int alter;
Console.Write("Alter: ");
while (!int.TryParse(Console.ReadLine(), out alter))
{
    Console.Write("Bitte eine Zahl eingeben: ");
}
```
Das gibt es auch für Kommazahlen: `double.TryParse(...)`.
''',
  example=r'''string[] eingaben = { "42", "abc", "-7", "3,5" };

foreach (string e in eingaben)
{
    if (int.TryParse(e, out int zahl))
        Console.WriteLine($"'{e}' ist die Zahl {zahl}");
    else
        Console.WriteLine($"'{e}' ist keine ganze Zahl");
}

if (double.TryParse("3,5", out double d))
    Console.WriteLine("Als double: " + d);
''',
  note="out bedeutet: Die Methode schreibt einen Wert in diese Variable hinein. Du kannst sie direkt im Aufruf deklarieren: out int zahl.",
  exercises=[
    A("Zahl oder nicht?", r'''Lies eine Eingabe. Ist es eine ganze Zahl, gib `Das Doppelte ist 14` (bei `7`) aus, sonst `Ungültige Eingabe`. Benutze `int.TryParse`.''',
      starter='string eingabe = Console.ReadLine();\n',
      solution='string eingabe = Console.ReadLine();\nif (int.TryParse(eingabe, out int zahl))\n    Console.WriteLine("Das Doppelte ist " + (zahl * 2));\nelse\n    Console.WriteLine("Ungültige Eingabe");\n',
      tests=[T("Das Doppelte ist 14", "7"), T("Ungültige Eingabe", "sieben"), T("Ungültige Eingabe", "")], compare="lenient",
      requirements=[(r'int\s*\.\s*TryParse', "Benutze int.TryParse.")]),
    A("Nachfragen bis es passt", r'''Frage so lange nach dem Alter, bis eine gültige Zahl eingegeben wird. Bei ungültiger Eingabe: `Bitte eine Zahl eingeben!`. Am Ende: `Du bist 30 Jahre alt.`

Bei `dreißig`, `30`:
```
Bitte eine Zahl eingeben!
Du bist 30 Jahre alt.
```''',
      starter='int alter;\n',
      solution='int alter;\nwhile (!int.TryParse(Console.ReadLine(), out alter))\n{\n    Console.WriteLine("Bitte eine Zahl eingeben!");\n}\nConsole.WriteLine($"Du bist {alter} Jahre alt.");\n',
      tests=[T("Bitte eine Zahl eingeben!\nDu bist 30 Jahre alt.", "dreißig", "30"), T("Du bist 5 Jahre alt.", "5"), T("Bitte eine Zahl eingeben!\nBitte eine Zahl eingeben!\nDu bist 8 Jahre alt.", "x", "y", "8")], compare="lenient",
      requirements=[(r'TryParse', "Benutze int.TryParse.")]),
    A("Gültige Zahlen summieren", r'''Lies 5 Eingaben. Addiere nur die, die gültige ganze Zahlen sind, und zähle die ungültigen. Bei `4`, `x`, `10`, `zwei`, `6`:

```
Summe: 20
Ungültig: 2
```''',
      starter='int summe = 0;\nint ungueltig = 0;\n',
      solution='int summe = 0;\nint ungueltig = 0;\nfor (int i = 0; i < 5; i++)\n{\n    if (int.TryParse(Console.ReadLine(), out int z))\n        summe += z;\n    else\n        ungueltig++;\n}\nConsole.WriteLine("Summe: " + summe);\nConsole.WriteLine("Ungültig: " + ungueltig);\n',
      tests=[T("Summe: 20\nUngültig: 2", "4", "x", "10", "zwei", "6"), T("Summe: 15\nUngültig: 0", "1", "2", "3", "4", "5")], compare="lenient",
      requirements=[(r'TryParse', "Benutze int.TryParse.")]),
  ],
  quiz=[
    Q("Was gibt int.TryParse zurück?", ["Die Zahl", "true oder false", "Einen string", "Nichts"], 1, ""),
    Q("Welchen Wert hat zahl nach int.TryParse(\"abc\", out int zahl)?", ["null", "-1", "0", "\"abc\""], 2, ""),
    Q("Warum ist TryParse besser als Parse bei Benutzereingaben?", ["Es ist kürzer", "Das Programm stürzt bei falschen Eingaben nicht ab", "Es rundet", "Es kann Text speichern"], 1, ""),
  ])

L(3, "l20", "Die Math-Klasse",
  intro="Für Wurzeln, Potenzen, Runden und mehr gibt es die fertige Klasse Math.",
  explanation=r'''
## Häufig gebraucht
- `Math.Sqrt(16)` – Wurzel → `4`
- `Math.Pow(2, 10)` – Potenz 2¹⁰ → `1024`
- `Math.Abs(-5)` – Betrag → `5`
- `Math.Max(3, 8)` / `Math.Min(3, 8)` – größere / kleinere Zahl
- `Math.Round(3.14159, 2)` – auf 2 Stellen runden → `3,14`
- `Math.Floor(3.9)` – abrunden → `3`, `Math.Ceiling(3.1)` – aufrunden → `4`
- `Math.PI` – die Zahl π

## double und int
Die meisten Math-Methoden liefern `double`. Brauchst du einen `int`, wandelst du um:
```
int x = (int)Math.Floor(3.7);   // 3
```
Das `(int)` davor heißt **Cast** (Typumwandlung). Dabei werden Nachkommastellen abgeschnitten.
''',
  example=r'''Console.WriteLine(Math.Sqrt(81));
Console.WriteLine(Math.Pow(2, 8));
Console.WriteLine(Math.Abs(-12));
Console.WriteLine(Math.Max(7, 3));
Console.WriteLine(Math.Round(2.71828, 2));
Console.WriteLine(Math.Ceiling(4.01));

double radius = 3;
double flaeche = Math.PI * Math.Pow(radius, 2);
Console.WriteLine($"Kreisfläche: {flaeche:F2}");

int ganz = (int)7.9;
Console.WriteLine(ganz);
''',
  note="Math.Round rundet bei .5 standardmäßig zur nächsten geraden Zahl (2,5 → 2). Für kaufmännisches Runden: Math.Round(x, MidpointRounding.AwayFromZero).",
  exercises=[
    A("Satz des Pythagoras", r'''Berechne die Länge der Hypotenuse c = √(a² + b²) für a = 3 und b = 4 sowie für a = 5 und b = 12. Schreib dafür eine Methode `static double Hypotenuse(double a, double b)`:

```
5
13
```''',
      starter='',
      solution='Console.WriteLine(Hypotenuse(3, 4));\nConsole.WriteLine(Hypotenuse(5, 12));\n\nstatic double Hypotenuse(double a, double b)\n{\n    return Math.Sqrt(a * a + b * b);\n}\n',
      tests=[T("5\n13")],
      requirements=[(r'Math\s*\.\s*Sqrt', "Benutze Math.Sqrt.")]),
    A("Kreis", r'''Lies einen Radius (Kommazahl) ein und gib Umfang (2·π·r) und Fläche (π·r²) mit 2 Nachkommastellen aus. Bei `5`:

```
Umfang: 31,42
Fläche: 78,54
```''',
      starter='double r = double.Parse(Console.ReadLine());\n',
      solution='double r = double.Parse(Console.ReadLine());\nConsole.WriteLine($"Umfang: {2 * Math.PI * r:F2}");\nConsole.WriteLine($"Fläche: {Math.PI * Math.Pow(r, 2):F2}");\n',
      tests=[T("Umfang: 12,57\nFläche: 12,57", "2"), T("Umfang: 31,42\nFläche: 78,54", "5"), T("Umfang: 9,42\nFläche: 7,07", "1,5")], compare="lenient",
      requirements=[(r'Math\s*\.\s*PI', "Benutze Math.PI.")]),
    A("Pizza-Rechner", r'''Für eine Party brauchst du pro Person 3 Pizzastücke. Eine Pizza hat 8 Stücke. Lies die Anzahl der Personen ein und berechne mit `Math.Ceiling`, wie viele Pizzen du **mindestens** bestellen musst. Bei `10`:

```
Anzahl Pizzen: 4
```''',
      starter='int personen = int.Parse(Console.ReadLine());\n',
      solution='int personen = int.Parse(Console.ReadLine());\nint stuecke = personen * 3;\nint pizzen = (int)Math.Ceiling(stuecke / 8.0);\nConsole.WriteLine($"Anzahl Pizzen: {pizzen}");\n',
      tests=[T("Anzahl Pizzen: 4", "10"), T("Anzahl Pizzen: 3", "8"), T("Anzahl Pizzen: 1", "1")], compare="lenient",
      requirements=[(r'Math\s*\.\s*Ceiling', "Benutze Math.Ceiling.")],
      hint="Teile durch 8.0 (nicht 8), sonst wird ganzzahlig geteilt und vorher schon abgerundet!"),
  ],
  quiz=[
    Q("Was ergibt Math.Pow(3, 2)?", ["6", "9", "5", "1,5"], 1, ""),
    Q("Was ergibt (int)4.99?", ["5", "4", "4,99", "Fehler"], 1, "Ein Cast nach int schneidet die Nachkommastellen ab."),
    Q("Welche Methode rundet immer auf?", ["Math.Floor", "Math.Round", "Math.Ceiling", "Math.Abs"], 2, ""),
  ])
