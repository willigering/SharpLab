# Stufe 1: Grundlagen
from gen import L, A, T, Q

L(1, "l01", "Dein erstes Programm",
  intro="Jedes Programm fängt klein an: Du lässt den Computer Text ausgeben. Das ist der erste Schritt zu allem anderen.",
  explanation=r'''
## Text ausgeben
Mit `Console.WriteLine("...")` schreibst du eine Zeile in das Ausgabefenster (die **Konsole**).

- `Console` ist das Textfenster.
- `.WriteLine` heißt „schreib eine Zeile“. Danach geht es in der nächsten Zeile weiter.
- Der Text steht in **Anführungszeichen** `" "`. So einen Text nennt man **String**.
- Das **Semikolon** `;` am Ende ist wie ein Punkt im Satz: Jeder Befehl endet damit.

## Write und WriteLine
`Console.Write("...")` schreibt Text **ohne** Zeilenumbruch. Der nächste Text kommt direkt dahinter.

## Groß- und Kleinschreibung
C# unterscheidet Groß- und Kleinbuchstaben! `Console.WriteLine` ist richtig, `console.writeline` ist falsch.
''',
  example=r'''Console.WriteLine("Hallo, Welt!");
Console.WriteLine("Ich lerne C#.");
Console.Write("Das steht ");
Console.Write("in einer Zeile.");
''',
  note="Klick auf „Kompilieren & Ausführen“ (oder drück F5), um den Code laufen zu lassen. Ändere ruhig den Text und probier es nochmal!",
  exercises=[
    A("Hallo, Welt!", r'''Gib genau diesen Text aus:

```
Hallo, Welt!
```''',
      starter='// Schreib hier deinen Code\n',
      solution='Console.WriteLine("Hallo, Welt!");\n',
      tests=[T("Hallo, Welt!")], hint='Console.WriteLine("Hallo, Welt!"); – achte auf das Komma, das Ausrufezeichen und das ; am Ende.'),
    A("Drei Zeilen", r'''Gib diese drei Zeilen aus (jede mit einem eigenen `Console.WriteLine`):

```
Ich heiße Max.
Ich bin 20 Jahre alt.
Meine Lieblingsfarbe ist Blau.
```''',
      starter='// Schreib hier deinen Code\n',
      solution='Console.WriteLine("Ich heiße Max.");\nConsole.WriteLine("Ich bin 20 Jahre alt.");\nConsole.WriteLine("Meine Lieblingsfarbe ist Blau.");\n',
      tests=[T("Ich heiße Max.\nIch bin 20 Jahre alt.\nMeine Lieblingsfarbe ist Blau.")]),
    A("Write ohne Zeilenumbruch", r'''Benutze **dreimal** `Console.Write` (nicht WriteLine!), sodass diese **eine** Zeile entsteht:

```
Eins Zwei Drei
```''',
      starter='// Schreib hier deinen Code\n',
      solution='Console.Write("Eins ");\nConsole.Write("Zwei ");\nConsole.Write("Drei");\n',
      tests=[T("Eins Zwei Drei")],
      requirements=[(r'Console\s*\.\s*Write\s*\(', "Benutze Console.Write (ohne Line).")],
      hint="Die Leerzeichen zwischen den Wörtern musst du selbst in die Anführungszeichen schreiben."),
  ],
  quiz=[
    Q("Womit endet in C# jeder Befehl?", [". (Punkt)", "; (Semikolon)", ": (Doppelpunkt)", "Mit gar nichts"], 1, "Das Semikolon ; beendet jeden Befehl."),
    Q("Was ist der Unterschied zwischen Write und WriteLine?", ["Es gibt keinen", "WriteLine macht danach einen Zeilenumbruch", "Write ist schneller", "WriteLine schreibt fett"], 1, "WriteLine springt nach dem Text in die nächste Zeile, Write nicht."),
    Q("Welche Zeile ist richtig?", ['console.writeline("Hi");', 'Console.WriteLine(Hi);', 'Console.WriteLine("Hi");', 'Console.WriteLine("Hi")'], 2, "Groß-/Kleinschreibung, Anführungszeichen um den Text und das ; am Ende müssen stimmen."),
  ])

L(1, "l02", "Kommentare",
  intro="Kommentare sind Notizen für Menschen. Der Computer überliest sie einfach.",
  explanation=r'''
## Einzeilige Kommentare
Alles hinter `//` bis zum Zeilenende ist ein **Kommentar**. Der Compiler ignoriert es.

## Mehrzeilige Kommentare
Alles zwischen `/*` und `*/` ist ebenfalls ein Kommentar – auch über mehrere Zeilen.

## Wofür?
- Um zu erklären, **was** dein Code macht.
- Um Code kurz „auszuschalten“, ohne ihn zu löschen (**auskommentieren**).

In Visual Studio: Zeilen markieren und **Strg+K, Strg+C** kommentiert aus, **Strg+K, Strg+U** macht es rückgängig.
''',
  example=r'''// Das ist ein Kommentar
Console.WriteLine("Das wird ausgegeben"); // Kommentar am Zeilenende

/* Ein Kommentar
   über mehrere Zeilen */

// Console.WriteLine("Das wird NICHT ausgegeben");
''',
  note="Gute Kommentare erklären das „Warum“. Schreib lieber zu viele als zu wenige, solange du lernst.",
  exercises=[
    A("Auskommentieren", r'''Das Programm gibt drei Zeilen aus. Kommentiere die **mittlere** Zeile aus, sodass nur noch das hier erscheint:

```
Guten Morgen
Gute Nacht
```''',
      starter='Console.WriteLine("Guten Morgen");\nConsole.WriteLine("Guten Mittag");\nConsole.WriteLine("Gute Nacht");\n',
      solution='Console.WriteLine("Guten Morgen");\n// Console.WriteLine("Guten Mittag");\nConsole.WriteLine("Gute Nacht");\n',
      tests=[T("Guten Morgen\nGute Nacht")],
      requirements=[(r"Guten Mittag", "Lösch die mittlere Zeile nicht – setz // davor.", True)],
      hint="Setz // an den Anfang der mittleren Zeile."),
    A("Kommentar schreiben", r'''Schreib über die Ausgabe einen Kommentar (mit `//`), der erklärt, was die Zeile macht. Die Ausgabe soll sein:

```
C# macht Spaß
```''',
      starter='Console.WriteLine("C# macht Spaß");\n',
      solution='// Gibt einen Satz aus\nConsole.WriteLine("C# macht Spaß");\n',
      tests=[T("C# macht Spaß")],
      requirements=[(r'//', "Schreib einen Kommentar mit // über die Zeile.", True)]),
    A("Mehrzeiliger Kommentar", r'''Schalte die beiden mittleren Zeilen mit **einem** mehrzeiligen Kommentar `/* ... */` aus. Ausgabe:

```
Start
Ende
```''',
      starter='Console.WriteLine("Start");\nConsole.WriteLine("Fehler 1");\nConsole.WriteLine("Fehler 2");\nConsole.WriteLine("Ende");\n',
      solution='Console.WriteLine("Start");\n/*\nConsole.WriteLine("Fehler 1");\nConsole.WriteLine("Fehler 2");\n*/\nConsole.WriteLine("Ende");\n',
      tests=[T("Start\nEnde")],
      requirements=[(r'/\*[\s\S]*\*/', "Benutze einen mehrzeiligen Kommentar /* ... */.", True), (r'Fehler 1', "Lösch die Zeilen nicht, kommentiere sie aus.", True)]),
  ],
  quiz=[
    Q("Was macht der Compiler mit Kommentaren?", ["Er gibt sie aus", "Er ignoriert sie", "Er meldet einen Fehler", "Er führt sie aus"], 1, "Kommentare sind nur für Menschen da."),
    Q("Wie beginnt ein einzeiliger Kommentar?", ["#", "--", "//", "<!--"], 2, "// leitet in C# einen einzeiligen Kommentar ein."),
    Q("Was bedeutet „auskommentieren“?", ["Code löschen", "Code vorübergehend in einen Kommentar verwandeln", "Code kopieren", "Code schneller machen"], 1, "Auskommentierter Code bleibt stehen, wird aber nicht ausgeführt."),
  ])

L(1, "l03", "Variablen & Datentypen",
  intro="Eine Variable ist wie eine beschriftete Schachtel, in der du einen Wert aufbewahrst.",
  explanation=r'''
## Variable anlegen
```
int alter = 25;
```
- `int` ist der **Datentyp**: Was für ein Wert passt in die Schachtel?
- `alter` ist der **Name** der Variable – den denkst du dir aus.
- `=` ist die **Zuweisung**: Der Wert rechts kommt in die Variable links.

Eine Variable anlegen heißt **deklarieren**. Ihr den ersten Wert geben heißt **initialisieren**.

## Die wichtigsten Datentypen
- `int` – ganze Zahlen: `42`, `-7`
- `double` – Kommazahlen: `3.14` (im Code mit **Punkt**!)
- `string` – Text: `"Hallo"`
- `char` – ein einzelnes Zeichen: `'A'` (einfache Anführungszeichen)
- `bool` – wahr oder falsch: `true` oder `false`

## Wert ändern
Eine Variable kann später einen neuen Wert bekommen – dann **ohne** Typ davor: `alter = 26;`

## Ausgeben
`Console.WriteLine(alter);` gibt den **Inhalt** aus. Mit `+` kannst du Text und Variablen verbinden: `"Alter: " + alter`.
''',
  example=r'''string name = "Anna";
int alter = 25;
double groesse = 1.68;
bool istStudentin = true;
char initiale = 'A';

Console.WriteLine(name);
Console.WriteLine("Alter: " + alter);
Console.WriteLine("Größe: " + groesse);
Console.WriteLine("Studentin: " + istStudentin);
Console.WriteLine("Initiale: " + initiale);

alter = 26; // neuer Wert, ohne "int" davor
Console.WriteLine("Nächstes Jahr: " + alter);
''',
  note="Kommazahlen schreibst du im Code mit Punkt (1.68). Ausgegeben werden sie auf einem deutschen PC aber mit Komma (1,68).",
  exercises=[
    A("Steckbrief", r'''Lege drei Variablen an: `string stadt` mit dem Wert `"Berlin"`, `int einwohner` mit `3700000` und `bool istHauptstadt` mit `true`. Gib dann aus:

```
Berlin
3700000
True
```''',
      starter='string stadt = "Berlin";\n',
      solution='string stadt = "Berlin";\nint einwohner = 3700000;\nbool istHauptstadt = true;\nConsole.WriteLine(stadt);\nConsole.WriteLine(einwohner);\nConsole.WriteLine(istHauptstadt);\n',
      tests=[T("Berlin\n3700000\nTrue")],
      requirements=[(r'\bint\s+einwohner\b', "Lege eine Variable int einwohner an."), (r'\bbool\s+istHauptstadt\b', "Lege eine Variable bool istHauptstadt an.")],
      hint="bool-Werte werden als True bzw. False ausgegeben (mit großem Anfangsbuchstaben)."),
    A("Text und Variable verbinden", r'''Gegeben ist `string tier = "Katze";` und `int beine = 4;`. Gib mit `+` aus:

```
Eine Katze hat 4 Beine.
```''',
      starter='string tier = "Katze";\nint beine = 4;\n\n',
      solution='string tier = "Katze";\nint beine = 4;\n\nConsole.WriteLine("Eine " + tier + " hat " + beine + " Beine.");\n',
      tests=[T("Eine Katze hat 4 Beine.")],
      requirements=[(r'\+\s*tier|tier\s*\+', "Benutze die Variable tier mit +."), (r'\+\s*beine|beine\s*\+', "Benutze die Variable beine mit +.")],
      hint="Leerzeichen gehören in die Anführungszeichen: \"Eine \" + tier + \" hat \" ..."),
    A("Wert ändern", r'''Lege `int punkte = 10;` an und gib den Wert aus. Ändere ihn dann auf `50` (ohne neues `int`!) und gib ihn nochmal aus:

```
Punkte: 10
Punkte: 50
```''',
      starter='int punkte = 10;\n',
      solution='int punkte = 10;\nConsole.WriteLine("Punkte: " + punkte);\npunkte = 50;\nConsole.WriteLine("Punkte: " + punkte);\n',
      tests=[T("Punkte: 10\nPunkte: 50")],
      requirements=[(r'(?<!int\s)\bpunkte\s*=\s*50', "Weise punkte den neuen Wert 50 zu: punkte = 50;")]),
  ],
  quiz=[
    Q("Welcher Datentyp passt für den Wert 3.75?", ["int", "double", "bool", "char"], 1, "Kommazahlen speichert man in double."),
    Q("Was bedeutet „initialisieren“?", ["Die Variable löschen", "Der Variable ihren ersten Wert geben", "Die Variable ausgeben", "Den Datentyp ändern"], 1, "Initialisieren = erster Wert. Deklarieren = anlegen."),
    Q("Welche Zeile ist falsch?", ['string s = "Hi";', "int x = 5;", 'int y = "5";', "bool b = false;"], 2, '"5" ist ein Text (string) und passt nicht in eine int-Variable.'),
  ])

L(1, "l04", "Rechnen & Operatoren",
  intro="Computer sind super im Rechnen. In C# rechnest du fast wie im Matheunterricht.",
  explanation=r'''
## Grundrechenarten
- `+` plus, `-` minus, `*` mal, `/` geteilt
- `%` **Modulo**: der **Rest** einer Division. `7 % 3` ist `1`.
- Klammern `( )` wie in Mathe: Punkt vor Strich gilt auch hier.

## Achtung bei Division mit int!
Wenn beide Zahlen `int` sind, wird **ganzzahlig** geteilt – der Rest fällt weg: `7 / 2` ergibt `3`.
Mit `double` gibt es Kommazahlen: `7.0 / 2` ergibt `3,5`.

## Kurzschreibweisen
- `x = x + 5;` kann man kürzer schreiben: `x += 5;` (auch `-=`, `*=`, `/=`)
- `x++;` erhöht um 1 – das nennt man **inkrementieren**.
- `x--;` verringert um 1 – **dekrementieren**.

## Rechnen in der Ausgabe
Setz Rechnungen in der Ausgabe in Klammern: `"Summe: " + (a + b)`. Sonst klebt C# die Zahlen als Text aneinander!
''',
  example=r'''int a = 7;
int b = 2;

Console.WriteLine(a + b);   // 9
Console.WriteLine(a - b);   // 5
Console.WriteLine(a * b);   // 14
Console.WriteLine(a / b);   // 3  (ganzzahlig!)
Console.WriteLine(a % b);   // 1  (Rest)
Console.WriteLine(7.0 / 2); // 3,5

int zaehler = 0;
zaehler++;      // jetzt 1
zaehler += 10;  // jetzt 11
Console.WriteLine("Zähler: " + zaehler);
Console.WriteLine("Summe: " + (a + b));
''',
  note='Ohne Klammern ergibt "Summe: " + 7 + 2 den Text "Summe: 72" – weil von links nach rechts zusammengeklebt wird.',
  exercises=[
    A("Rechteck", r'''Ein Rechteck ist `breite = 8` und `hoehe = 5` groß (beides `int`). Berechne Fläche (`breite * hoehe`) und Umfang (`2 * (breite + hoehe)`) und gib aus:

```
Fläche: 40
Umfang: 26
```''',
      starter='int breite = 8;\nint hoehe = 5;\n\n',
      solution='int breite = 8;\nint hoehe = 5;\n\nint flaeche = breite * hoehe;\nint umfang = 2 * (breite + hoehe);\nConsole.WriteLine("Fläche: " + flaeche);\nConsole.WriteLine("Umfang: " + umfang);\n',
      tests=[T("Fläche: 40\nUmfang: 26")]),
    A("Teilen mit Rest", r'''17 Kekse werden an 5 Kinder verteilt. Berechne mit `/` und `%`, wie viele jedes Kind bekommt und wie viele übrig bleiben:

```
Jedes Kind: 3
Übrig: 2
```''',
      starter='int kekse = 17;\nint kinder = 5;\n\n',
      solution='int kekse = 17;\nint kinder = 5;\n\nConsole.WriteLine("Jedes Kind: " + (kekse / kinder));\nConsole.WriteLine("Übrig: " + (kekse % kinder));\n',
      tests=[T("Jedes Kind: 3\nÜbrig: 2")],
      requirements=[(r'%', "Benutze den Modulo-Operator % für den Rest.")]),
    A("Inkrementieren", r'''Starte mit `int level = 1;`. Erhöhe `level` zweimal mit `++` und danach mit `+=` um 10. Gib nach jedem Schritt aus:

```
Level 1
Level 2
Level 3
Level 13
```''',
      starter='int level = 1;\n',
      solution='int level = 1;\nConsole.WriteLine("Level " + level);\nlevel++;\nConsole.WriteLine("Level " + level);\nlevel++;\nConsole.WriteLine("Level " + level);\nlevel += 10;\nConsole.WriteLine("Level " + level);\n',
      tests=[T("Level 1\nLevel 2\nLevel 3\nLevel 13")],
      requirements=[(r'level\s*\+\+', "Benutze level++."), (r'level\s*\+=', "Benutze level += 10.")]),
    A("Durchschnitt mit double", r'''Berechne den Durchschnitt der Noten `2`, `3` und `2` als Kommazahl. Tipp: Teile durch `3.0`, damit nicht ganzzahlig geteilt wird. Gib das Ergebnis mit **2 Nachkommastellen** aus (`:F2`):

```
Durchschnitt: 2,33
```''',
      starter='int n1 = 2;\nint n2 = 3;\nint n3 = 2;\n\n',
      solution='int n1 = 2;\nint n2 = 3;\nint n3 = 2;\n\ndouble schnitt = (n1 + n2 + n3) / 3.0;\nConsole.WriteLine($"Durchschnitt: {schnitt:F2}");\n',
      tests=[T("Durchschnitt: 2,33")],
      hint="(n1 + n2 + n3) / 3.0 – die Klammern sind wichtig (Punkt vor Strich)!"),
  ],
  quiz=[
    Q("Was ergibt 7 / 2 in C#, wenn beide Zahlen int sind?", ["3,5", "3", "4", "1"], 1, "Ganzzahlige Division schneidet den Rest ab."),
    Q("Was ergibt 10 % 4?", ["2", "2,5", "0", "6"], 0, "10 geteilt durch 4 ist 2 Rest 2. Modulo liefert den Rest."),
    Q("Was macht x++?", ["Verdoppelt x", "Erhöht x um 1", "Setzt x auf 0", "Verringert x um 1"], 1, "++ heißt inkrementieren: um 1 erhöhen."),
  ])

L(1, "l05", "String-Interpolation",
  intro="Mit dem $-Zeichen baust du Texte mit Variablen viel bequemer zusammen als mit +.",
  explanation=r'''
## Das $ vor dem String
```
string name = "Lea";
int alter = 30;
Console.WriteLine($"{name} ist {alter} Jahre alt.");
```
Steht ein `$` vor den Anführungszeichen, kannst du in **geschweiften Klammern** `{ }` Variablen oder Rechnungen einsetzen. Das heißt **String-Interpolation**.

## Rechnen in { }
`$"Doppelt: {alter * 2}"` – in den Klammern darf jeder Ausdruck stehen.

## Formatieren
- `{preis:F2}` – genau **2 Nachkommastellen**: `4,50`
- `{preis:C}` – als Währung: `4,50 €`
- `{zahl,5}` – rechtsbündig auf 5 Zeichen auffüllen

## Sonderzeichen
- `\n` – neue Zeile im Text
- `\t` – Tabulator
- `\"` – ein Anführungszeichen im Text
''',
  example=r'''string produkt = "Kaffee";
double preis = 3.5;
int menge = 3;

Console.WriteLine($"{menge}x {produkt} kostet {preis * menge:F2} Euro.");
Console.WriteLine($"Einzelpreis: {preis:F2}");
Console.WriteLine("Zeile 1\nZeile 2");
Console.WriteLine($"Er sagte: \"Hallo!\"");
''',
  note="Vergiss das $ nicht! Ohne $ wird {name} einfach als Text ausgegeben.",
  exercises=[
    A("Vorstellung", r'''Gib mit String-Interpolation (`$"..."`) aus:

```
Ich heiße Tom und bin 17 Jahre alt.
```''',
      starter='string name = "Tom";\nint alter = 17;\n\n',
      solution='string name = "Tom";\nint alter = 17;\n\nConsole.WriteLine($"Ich heiße {name} und bin {alter} Jahre alt.");\n',
      tests=[T("Ich heiße Tom und bin 17 Jahre alt.")],
      requirements=[(r'\$"', "Benutze String-Interpolation mit $\"...\".")]),
    A("Rechnung im Text", r'''Ein Kino-Ticket kostet `9.5` Euro. Berechne den Preis für `4` Tickets direkt in den `{ }` und gib ihn mit 2 Nachkommastellen aus:

```
4 Tickets kosten 38,00 Euro.
```''',
      starter='double ticketPreis = 9.5;\nint anzahl = 4;\n\n',
      solution='double ticketPreis = 9.5;\nint anzahl = 4;\n\nConsole.WriteLine($"{anzahl} Tickets kosten {ticketPreis * anzahl:F2} Euro.");\n',
      tests=[T("4 Tickets kosten 38,00 Euro.")],
      requirements=[(r'\$"', "Benutze String-Interpolation mit $\"...\".")],
      hint="{ticketPreis * anzahl:F2}"),
    A("Zeilenumbruch im Text", r'''Gib mit **einem einzigen** `Console.WriteLine` und `\n` drei Zeilen aus:

```
Rot
Gelb
Grün
```''',
      starter='',
      solution='Console.WriteLine("Rot\\nGelb\\nGrün");\n',
      tests=[T("Rot\nGelb\nGrün")],
      requirements=[(r'\\n', "Benutze \\n für die Zeilenumbrüche.")]),
  ],
  quiz=[
    Q("Was gibt Console.WriteLine($\"{3 + 4}\"); aus?", ["{3 + 4}", "3 + 4", "7", "34"], 2, "In den geschweiften Klammern wird gerechnet."),
    Q("Was bewirkt :F2 in {preis:F2}?", ["Fett schreiben", "Zwei Nachkommastellen", "Preis mal 2", "Zwei Leerzeichen"], 1, "F2 = Festkomma mit 2 Nachkommastellen."),
    Q("Was passiert ohne das $ vor dem String?", ["Fehler", "Die { } werden als normaler Text ausgegeben", "Nichts wird ausgegeben", "Das Programm stürzt ab"], 1, "Ohne $ gibt es keine Interpolation."),
  ])

L(1, "l06", "Eingabe & int.Parse",
  intro="Jetzt wird dein Programm interaktiv: Es fragt den Benutzer etwas und reagiert auf die Antwort.",
  explanation=r'''
## Text einlesen
```
Console.Write("Wie heißt du? ");
string name = Console.ReadLine();
```
`Console.ReadLine()` wartet, bis der Benutzer etwas eintippt und Enter drückt. Das Ergebnis ist **immer ein string**.

## Zahlen einlesen
Weil `ReadLine` Text liefert, musst du ihn in eine Zahl **umwandeln**:
```
int alter = int.Parse(Console.ReadLine());
double preis = double.Parse(Console.ReadLine());
```
Gibt der Benutzer keine gültige Zahl ein (z. B. „abc“), stürzt `int.Parse` mit einer **FormatException** ab. Wie man das verhindert, lernst du später bei `TryParse`.

## In dieser App
Wenn dein Programm etwas einliest, erscheint unten ein Eingabefeld. Du kannst Eingaben auch vorab ins Feld **„Eingabe“** schreiben – eine Zeile pro `ReadLine`. Beim Prüfen setzt die App selbst Test-Eingaben ein.
''',
  example=r'''Console.Write("Wie heißt du? ");
string name = Console.ReadLine();

Console.Write("Wie alt bist du? ");
int alter = int.Parse(Console.ReadLine());

Console.WriteLine($"Hallo {name}! In 10 Jahren bist du {alter + 10}.");
''',
  exampleInput=["Anna", "25"],
  note="Bei Kommazahlen tippst du auf einem deutschen PC ein Komma ein: 3,5 – im Code schreibst du aber 3.5.",
  exercises=[
    A("Begrüßung", r'''Lies einen Namen ein und begrüße die Person. Bei der Eingabe `Lena` soll erscheinen:

```
Hallo Lena!
```
(Eine Frage wie „Name: “ davor darfst du gern ausgeben.)''',
      starter='// Schreib hier deinen Code\n',
      solution='Console.Write("Wie heißt du? ");\nstring name = Console.ReadLine();\nConsole.WriteLine($"Hallo {name}!");\n',
      tests=[T("Hallo Lena!", "Lena"), T("Hallo Ben!", "Ben")], compare="lenient"),
    A("Verdoppeln", r'''Lies eine ganze Zahl ein und gib das Doppelte aus. Bei Eingabe `21`:

```
Das Doppelte ist 42
```''',
      starter='// Schreib hier deinen Code\n',
      solution='Console.Write("Zahl: ");\nint zahl = int.Parse(Console.ReadLine());\nConsole.WriteLine("Das Doppelte ist " + (zahl * 2));\n',
      tests=[T("Das Doppelte ist 42", "21"), T("Das Doppelte ist -10", "-5")], compare="lenient",
      requirements=[(r'int\s*\.\s*Parse', "Wandle die Eingabe mit int.Parse(...) in eine Zahl um.")]),
    A("Taschenrechner Plus", r'''Lies zwei ganze Zahlen ein (zwei `ReadLine`) und gib ihre Summe aus. Bei `7` und `5`:

```
7 + 5 = 12
```''',
      starter='',
      solution='Console.Write("Erste Zahl: ");\nint a = int.Parse(Console.ReadLine());\nConsole.Write("Zweite Zahl: ");\nint b = int.Parse(Console.ReadLine());\nConsole.WriteLine($"{a} + {b} = {a + b}");\n',
      tests=[T("7 + 5 = 12", "7", "5"), T("100 + 23 = 123", "100", "23")], compare="lenient"),
    A("Preis mit Komma", r'''Lies einen Preis als Kommazahl (`double.Parse`) und eine Menge (`int`) ein. Gib den Gesamtpreis mit 2 Nachkommastellen aus. Bei `2,5` und `3`:

```
Gesamt: 7,50 Euro
```''',
      starter='',
      solution='Console.Write("Preis: ");\ndouble preis = double.Parse(Console.ReadLine());\nConsole.Write("Menge: ");\nint menge = int.Parse(Console.ReadLine());\nConsole.WriteLine($"Gesamt: {preis * menge:F2} Euro");\n',
      tests=[T("Gesamt: 7,50 Euro", "2,5", "3"), T("Gesamt: 1,98 Euro", "0,99", "2")], compare="lenient"),
  ],
  quiz=[
    Q("Welchen Datentyp liefert Console.ReadLine()?", ["int", "string", "double", "Das hängt von der Eingabe ab"], 1, "ReadLine liefert immer Text (string)."),
    Q("Was passiert bei int.Parse(\"abc\")?", ["Ergebnis 0", "Ergebnis -1", "Absturz mit FormatException", "Ergebnis \"abc\""], 2, "\"abc\" ist keine Zahl – int.Parse wirft eine FormatException."),
    Q("Wie wandelst du die Eingabe in eine Kommazahl um?", ["int.Parse(...)", "double.Parse(...)", "string.Parse(...)", "Console.ReadDouble()"], 1, "double.Parse wandelt Text in eine Kommazahl um."),
  ])

L(1, "l07", "Program & Main – der klassische Aufbau",
  intro="In vielen Kursen und Büchern sieht ein C#-Programm länger aus: mit namespace, class Program und static void Main. Hier lernst du, was das bedeutet.",
  explanation=r'''
## Zwei Schreibweisen, ein Programm
Seit .NET 6 erstellt Visual Studio Konsolen-Apps mit **Top-Level-Anweisungen**: Du schreibst deine Befehle einfach in die Datei. Früher (und bei vielen Dozenten heute noch) sah das so aus:

```
using System;

namespace MeinProgramm
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hallo!");
        }
    }
}
```

## Was bedeuten die Teile?
- `using System;` – „Ich benutze Dinge aus dem Bereich System“ (z. B. `Console`).
- `namespace MeinProgramm` – ein **Namensraum**, wie ein Ordner für deinen Code.
- `class Program` – eine **Klasse**. In C# steht Code immer in Klassen.
- `static void Main(string[] args)` – die **Main-Methode**: Hier startet das Programm.
- `{ }` – geschweifte Klammern umschließen einen **Block**. Jede `{` braucht eine `}`.

## Und die Top-Level-Version?
Bei Top-Level-Anweisungen erzeugt der Compiler `class Program` und `Main` unsichtbar für dich. Beide Varianten funktionieren in dieser App – und in Visual Studio.
''',
  example=r'''using System;

namespace ErstesProgramm
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hallo aus Main!");
            int zahl = 5;
            Console.WriteLine($"Die Zahl ist {zahl}.");
        }
    }
}
''',
  note="In Visual Studio kannst du beim Erstellen einer Konsolen-App den Haken „Keine Anweisungen der obersten Ebene verwenden“ setzen – dann bekommst du den klassischen Aufbau mit Main.",
  exercises=[
    A("Main ergänzen", r'''In der `Main`-Methode fehlt noch der Befehl. Lass das Programm ausgeben:

```
Programm gestartet
```''',
      starter='using System;\n\nnamespace Uebung\n{\n    class Program\n    {\n        static void Main(string[] args)\n        {\n            // Hier kommt dein Code hin\n        }\n    }\n}\n',
      solution='using System;\n\nnamespace Uebung\n{\n    class Program\n    {\n        static void Main(string[] args)\n        {\n            Console.WriteLine("Programm gestartet");\n        }\n    }\n}\n',
      tests=[T("Programm gestartet")],
      requirements=[(r'static\s+void\s+Main', "Lass die Main-Methode stehen und schreib den Code hinein.")]),
    A("Klammern reparieren", r'''Hier fehlen **zwei** schließende geschweifte Klammern `}`. Repariere das Programm, sodass es ausgibt:

```
Alle Klammern stimmen!
```''',
      starter='using System;\n\nnamespace Reparatur\n{\n    class Program\n    {\n        static void Main(string[] args)\n        {\n            Console.WriteLine("Alle Klammern stimmen!");\n        }\n',
      solution='using System;\n\nnamespace Reparatur\n{\n    class Program\n    {\n        static void Main(string[] args)\n        {\n            Console.WriteLine("Alle Klammern stimmen!");\n        }\n    }\n}\n',
      tests=[T("Alle Klammern stimmen!")],
      hint="Zähle: namespace {, class {, Main { – also braucht es am Ende drei }."),
    A("Von Top-Level zu Main", r'''Schreib dieses Top-Level-Programm in den **klassischen Aufbau** mit `class Program` und `static void Main` um:

```
string stadt = "Hamburg";
Console.WriteLine($"Ich wohne in {stadt}.");
```

Ausgabe:
```
Ich wohne in Hamburg.
```''',
      starter='using System;\n\n',
      solution='using System;\n\nclass Program\n{\n    static void Main(string[] args)\n    {\n        string stadt = "Hamburg";\n        Console.WriteLine($"Ich wohne in {stadt}.");\n    }\n}\n',
      tests=[T("Ich wohne in Hamburg.")],
      requirements=[(r'class\s+Program', "Schreib eine class Program."), (r'static\s+void\s+Main', "Schreib eine static void Main-Methode.")]),
  ],
  quiz=[
    Q("Wo startet ein klassisches C#-Programm?", ["In der ersten Zeile", "In der Main-Methode", "Im namespace", "Bei using"], 1, "Main ist der Einstiegspunkt."),
    Q("Was macht using System; ?", ["Startet das System", "Erlaubt Console statt System.Console zu schreiben", "Installiert .NET", "Nichts"], 1, "using macht Namen aus einem Namensraum direkt verfügbar."),
    Q("Was gehört zu jeder öffnenden geschweiften Klammer { ?", ["Ein ;", "Eine schließende }", "Ein Kommentar", "Nichts"], 1, "Klammern kommen immer paarweise."),
  ])
