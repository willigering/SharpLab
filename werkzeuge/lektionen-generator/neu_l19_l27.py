"""Zusätzliche Aufgaben Lektion 19–27 (bis 8 pro Lektion)."""
from neu import N, T, E

TRY = (r"TryParse\s*\(", "Prüfe sicher, ob die Eingabe eine Zahl ist – so stürzt nichts ab.")

# ---------------- l19 TryParse ----------------
N("l19", "Sicheres Alter", """
Lies ein Alter ein. Ist es eine ganze Zahl, gib aus, wie alt die Person in 10 Jahren ist. Sonst erscheint `Das war keine Zahl.` – das Programm darf nicht abstürzen. Bei `15`:

```
In 10 Jahren bist du 25.
```
""", r'''
if (int.TryParse(Console.ReadLine(), out int alter))
{
    Console.WriteLine($"In 10 Jahren bist du {alter + 10}.");
}
else
{
    Console.WriteLine("Das war keine Zahl.");
}
''', [T("In 10 Jahren bist du 25.", "15"), T("Das war keine Zahl.", "fünfzehn"), T("In 10 Jahren bist du 10.", "0")],
  "Die Lektion zeigt dir eine Umwandlung, die meldet, ob sie geklappt hat. Das Ergebnis landet in einer Variablen.", "lenient", anf=[TRY])

N("l19", "Kommazahl prüfen", """
Lies eine Kommazahl ein und gib das Doppelte aus. Ist die Eingabe keine Zahl, erscheint `Keine gültige Kommazahl.`. Bei `2,5`:

```
Das Doppelte ist 5
```
""", r'''
if (double.TryParse(Console.ReadLine(), out double zahl))
{
    Console.WriteLine($"Das Doppelte ist {zahl * 2}");
}
else
{
    Console.WriteLine("Keine gültige Kommazahl.");
}
''', [T("Das Doppelte ist 5", "2,5"), T("Das Doppelte ist 0,5", "0,25"), T("Keine gültige Kommazahl.", "abc")],
  "Die sichere Umwandlung gibt es auch für Kommazahlen. Der Aufbau bleibt gleich.", "lenient",
  anf=[(r"double\s*\.\s*TryParse\s*\(", "Prüfe sicher, ob die Eingabe eine Kommazahl ist.")])

N("l19", "Sicherer Mini-Rechner", """
Lies zwei Eingaben ein. Sind beide ganze Zahlen, gib die Summe aus, sonst `Ungültige Eingabe`. Bei `5` und `7`:

```
Summe: 12
```
""", r'''
bool ok1 = int.TryParse(Console.ReadLine(), out int a);
bool ok2 = int.TryParse(Console.ReadLine(), out int b);
if (ok1 && ok2)
{
    Console.WriteLine($"Summe: {a + b}");
}
else
{
    Console.WriteLine("Ungültige Eingabe");
}
''', [T("Summe: 12", "5", "7"), T("Ungültige Eingabe", "5", "x"), T("Ungültige Eingabe", "x", "7"), T("Summe: -1", "-4", "3")],
  "Prüfe jede Eingabe für sich und merk dir das Ergebnis. Gerechnet wird nur, wenn beide geklappt haben.", "lenient", anf=[TRY])

N("l19", "Zahl von 1 bis 10", """
Frage so lange nach einer Zahl, bis eine ganze Zahl von 1 bis 10 kommt. Bei Text erscheint `Keine Zahl!`, bei einer Zahl außerhalb `Bitte eine Zahl von 1 bis 10!`. Bei `abc`, `15`, `7`:

```
Keine Zahl!
Bitte eine Zahl von 1 bis 10!
Danke, du hast 7 gewählt.
```
""", r'''
int zahl = 0;
bool fertig = false;
while (!fertig)
{
    if (!int.TryParse(Console.ReadLine(), out zahl))
    {
        Console.WriteLine("Keine Zahl!");
    }
    else if (zahl < 1 || zahl > 10)
    {
        Console.WriteLine("Bitte eine Zahl von 1 bis 10!");
    }
    else
    {
        fertig = true;
    }
}
Console.WriteLine($"Danke, du hast {zahl} gewählt.");
''', [T("Keine Zahl!\nBitte eine Zahl von 1 bis 10!\nDanke, du hast 7 gewählt.", "abc", "15", "7"),
      T("Danke, du hast 10 gewählt.", "10"), T("Bitte eine Zahl von 1 bis 10!\nDanke, du hast 1 gewählt.", "0", "1")],
  "Es gibt zwei Gründe, warum eine Eingabe nicht passt – prüfe sie nacheinander. Die Schleife endet erst bei einer passenden Zahl.", "lenient",
  anf=[TRY, (r"\bwhile\s*\(|\bdo\s*\{|\bfor\s*\(", "Frage in einer Schleife so lange, bis die Eingabe passt.")])

N("l19", "Durchschnitt gültiger Eingaben", """
Lies so lange Eingaben ein, bis `fertig` kommt. Ganze Zahlen werden gesammelt, alles andere wird mit `Ungültig: ...` gemeldet und übersprungen. Gib am Ende den Durchschnitt der gültigen Zahlen mit zwei Nachkommastellen aus. Bei `3`, `abc`, `6`, `fertig`:

```
Ungültig: abc
Durchschnitt: 4,50
```
""", r'''
int summe = 0;
int anzahl = 0;
string eingabe = Console.ReadLine();
while (eingabe != "fertig")
{
    if (int.TryParse(eingabe, out int zahl))
    {
        summe = summe + zahl;
        anzahl++;
    }
    else
    {
        Console.WriteLine($"Ungültig: {eingabe}");
    }
    eingabe = Console.ReadLine();
}
Console.WriteLine($"Durchschnitt: {(double)summe / anzahl:F2}");
''', [T("Ungültig: abc\nDurchschnitt: 4,50", "3", "abc", "6", "fertig"), T("Durchschnitt: 10,00", "10", "fertig"),
      T("Ungültig: x\nUngültig: 2,5\nDurchschnitt: 2,00", "1", "x", "2,5", "3", "fertig")],
  "Speichere die Eingabe zuerst als Text, damit du sie mit fertig vergleichen und trotzdem umwandeln kannst. Zähle mit, wie viele Zahlen gültig waren.", "lenient",
  anf=[TRY])

# ---------------- l20 Math ----------------
def MATHE(name, was):
    return (rf"Math\s*\.\s*{name}\b", f"Für {was} hat die Mathe-Klasse eine fertige Methode – schau in die Lektion.")

N("l20", "Abstand zweier Zahlen", """
Lies zwei ganze Zahlen ein und gib ihren Abstand aus – der ist nie negativ, egal in welcher Reihenfolge die Zahlen kommen. Bei `3` und `10`:

```
Abstand: 7
```
""", r'''
int a = int.Parse(Console.ReadLine());
int b = int.Parse(Console.ReadLine());
Console.WriteLine($"Abstand: {Math.Abs(a - b)}");
''', [T("Abstand: 7", "3", "10"), T("Abstand: 7", "10", "3"), T("Abstand: 4", "-2", "2")],
  "Rechne die Differenz aus und entferne danach ein mögliches Minus. Dafür gibt es eine Methode in der Lektion.", "lenient",
  anf=[MATHE("Abs", "den Betrag einer Zahl")])

N("l20", "Größte von drei", """
Lies drei ganze Zahlen ein und gib die größte aus. Bei `4`, `9` und `2`:

```
Die größte Zahl ist 9.
```
""", r'''
int a = int.Parse(Console.ReadLine());
int b = int.Parse(Console.ReadLine());
int c = int.Parse(Console.ReadLine());
Console.WriteLine($"Die größte Zahl ist {Math.Max(Math.Max(a, b), c)}.");
''', [T("Die größte Zahl ist 9.", "4", "9", "2"), T("Die größte Zahl ist 12.", "12", "3", "5"), T("Die größte Zahl ist 7.", "1", "2", "7")],
  "Die fertige Methode vergleicht immer nur zwei Zahlen. Ihr Ergebnis kannst du aber direkt wieder hineingeben.", "lenient",
  anf=[MATHE("Max", "die größere von zwei Zahlen")])

N("l20", "Runden in drei Richtungen", """
Lies eine Kommazahl ein und gib sie dreimal aus: auf eine Nachkommastelle gerundet, auf die ganze Zahl darunter und auf die ganze Zahl darüber. Bei `3,14159`:

```
Gerundet: 3,1
Abgerundet: 3
Aufgerundet: 4
```
""", r'''
double zahl = double.Parse(Console.ReadLine());
Console.WriteLine($"Gerundet: {Math.Round(zahl, 1)}");
Console.WriteLine($"Abgerundet: {Math.Floor(zahl)}");
Console.WriteLine($"Aufgerundet: {Math.Ceiling(zahl)}");
''', [T("Gerundet: 3,1\nAbgerundet: 3\nAufgerundet: 4", "3,14159"), T("Gerundet: 7,9\nAbgerundet: 7\nAufgerundet: 8", "7,86")],
  "Für jede der drei Richtungen gibt es eine eigene Methode in der Lektion. Beim normalen Runden gibst du die Anzahl der Stellen mit.", "lenient",
  anf=[MATHE("Round", "das Runden"), MATHE("Floor", "das Abrunden"), MATHE("Ceiling", "das Aufrunden")])

N("l20", "Potenzen", """
Lies eine Basis und einen Exponenten ein (ganze Zahlen) und gib die Potenz aus. Bei `2` und `10`:

```
2 hoch 10 = 1024
```
""", r'''
int basis = int.Parse(Console.ReadLine());
int exponent = int.Parse(Console.ReadLine());
Console.WriteLine($"{basis} hoch {exponent} = {Math.Pow(basis, exponent)}");
''', [T("2 hoch 10 = 1024", "2", "10"), T("3 hoch 4 = 81", "3", "4"), T("5 hoch 0 = 1", "5", "0")],
  "Für hoch gibt es eine fertige Methode, die zwei Werte bekommt. Die Reihenfolge der beiden ist wichtig.", "lenient",
  anf=[MATHE("Pow", "Potenzen")])

N("l20", "Kreisrechner", """
Lies den Radius eines Kreises ein (Kommazahl) und gib Fläche und Umfang mit zwei Nachkommastellen aus. Nimm für Pi den genauen Wert aus der Lektion. Bei `5`:

```
Fläche: 78,54
Umfang: 31,42
```
""", r'''
double radius = double.Parse(Console.ReadLine());
double flaeche = Math.PI * Math.Pow(radius, 2);
double umfang = 2 * Math.PI * radius;
Console.WriteLine($"Fläche: {flaeche:F2}");
Console.WriteLine($"Umfang: {umfang:F2}");
''', [T("Fläche: 78,54\nUmfang: 31,42", "5"), T("Fläche: 3,14\nUmfang: 6,28", "1"), T("Fläche: 19,63\nUmfang: 15,71", "2,5")],
  "Die Fläche ist Pi mal Radius mal Radius, der Umfang zwei mal Pi mal Radius. Pi steht fertig in der Mathe-Klasse.", "lenient",
  anf=[(r"Math\s*\.\s*PI\b", "Nimm für Pi den genauen Wert aus der Mathe-Klasse.")])

# ---------------- l21 Klassen ----------------
N("l21", "Bellende Hunde", """
Schreib eine Klasse `Hund` mit dem Feld `Name` und einer Methode `Bellen()`. Erzeuge zwei Hunde, `Bello` und `Rex`, und lass beide bellen:

```
Bello sagt: Wuff!
Rex sagt: Wuff!
```
""", r'''
Hund h1 = new Hund();
h1.Name = "Bello";
Hund h2 = new Hund();
h2.Name = "Rex";
h1.Bellen();
h2.Bellen();

class Hund
{
    public string Name;

    public void Bellen()
    {
        Console.WriteLine($"{Name} sagt: Wuff!");
    }
}
''', [T("Bello sagt: Wuff!\nRex sagt: Wuff!")],
  "Die Klasse ist der Bauplan, im Hauptcode erzeugst du daraus zwei Objekte. In der Methode kennst du den eigenen Namen ohne Objektnamen davor.",
  anf=[(r"class\s+Hund", "Schreib die Klasse Hund."),
       (r"public\s+string\s+Name", "Die Klasse braucht ein öffentliches Feld Name."),
       (r"void\s+Bellen\s*\(\s*\)", "Die Klasse braucht eine Methode Bellen."),
       (r"(new\s+Hund\s*\([\s\S]*){2}", "Erzeuge zwei Hunde.")])

N("l21", "Bücherregal", """
Schreib eine Klasse `Buch` mit den Feldern `Titel` (Text) und `Seiten` (ganze Zahl) und einer Methode `Info()`. Erzeuge die Bücher „Der Hobbit“ (310 Seiten) und „Momo“ (304 Seiten):

```
Der Hobbit hat 310 Seiten.
Momo hat 304 Seiten.
```
""", r'''
Buch b1 = new Buch();
b1.Titel = "Der Hobbit";
b1.Seiten = 310;
Buch b2 = new Buch();
b2.Titel = "Momo";
b2.Seiten = 304;
b1.Info();
b2.Info();

class Buch
{
    public string Titel;
    public int Seiten;

    public void Info()
    {
        Console.WriteLine($"{Titel} hat {Seiten} Seiten.");
    }
}
''', [T("Der Hobbit hat 310 Seiten.\nMomo hat 304 Seiten.")],
  "Jedes Objekt hat seine eigenen Werte in den Feldern. Die Methode gibt die Werte des Objekts aus, für das sie aufgerufen wird.",
  anf=[(r"class\s+Buch", "Schreib die Klasse Buch."),
       (r"public\s+string\s+Titel", "Die Klasse braucht ein öffentliches Feld Titel."),
       (r"public\s+int\s+Seiten", "Die Klasse braucht ein öffentliches Feld Seiten."),
       (r"void\s+Info\s*\(\s*\)", "Die Klasse braucht eine Methode Info.")])

N("l21", "Lichtschalter", """
Schreib eine Klasse `Lampe` mit dem Feld `An` (Wahrheitswert), einer Methode `Schalten()`, die die Lampe umschaltet, und einer Methode `Zustand()`. Erzeuge eine Lampe, zeig den Zustand, schalte, zeig ihn wieder, schalte noch einmal und zeig ihn ein drittes Mal:

```
Die Lampe ist aus.
Die Lampe ist an.
Die Lampe ist aus.
```
""", r'''
Lampe lampe = new Lampe();
lampe.Zustand();
lampe.Schalten();
lampe.Zustand();
lampe.Schalten();
lampe.Zustand();

class Lampe
{
    public bool An;

    public void Schalten()
    {
        An = !An;
    }

    public void Zustand()
    {
        if (An)
        {
            Console.WriteLine("Die Lampe ist an.");
        }
        else
        {
            Console.WriteLine("Die Lampe ist aus.");
        }
    }
}
''', [T("Die Lampe ist aus.\nDie Lampe ist an.\nDie Lampe ist aus.")],
  "Ein neues Objekt startet bei einem Wahrheitswert mit false. Umschalten heißt: aus wahr wird falsch und umgekehrt.",
  anf=[(r"class\s+Lampe", "Schreib die Klasse Lampe."),
       (r"public\s+bool\s+An", "Die Klasse braucht ein öffentliches Feld An."),
       (r"void\s+Schalten\s*\(\s*\)", "Die Klasse braucht eine Methode Schalten."),
       (r"void\s+Zustand\s*\(\s*\)", "Die Klasse braucht eine Methode Zustand.")])

N("l21", "Kreise vergleichen", """
Schreib eine Klasse `Kreis` mit dem Feld `Radius` (Kommazahl) und einer Methode `Flaeche()`, die die Fläche liefert (Pi mal Radius mal Radius). Erzeuge Kreise mit Radius 1 und 2 und gib die Flächen mit zwei Nachkommastellen aus:

```
Fläche: 3,14
Fläche: 12,57
```
""", r'''
Kreis k1 = new Kreis();
k1.Radius = 1;
Kreis k2 = new Kreis();
k2.Radius = 2;
Console.WriteLine($"Fläche: {k1.Flaeche():F2}");
Console.WriteLine($"Fläche: {k2.Flaeche():F2}");

class Kreis
{
    public double Radius;

    public double Flaeche()
    {
        return Math.PI * Radius * Radius;
    }
}
''', [T("Fläche: 3,14\nFläche: 12,57")],
  "Die Methode gibt ein Ergebnis zurück, statt selbst etwas auszugeben. Die Formatierung passiert im Hauptcode.",
  anf=[(r"class\s+Kreis", "Schreib die Klasse Kreis."),
       (r"public\s+double\s+Radius", "Die Klasse braucht ein öffentliches Feld Radius als Kommazahl."),
       (r"double\s+Flaeche\s*\(\s*\)", "Die Klasse braucht eine Methode Flaeche, die eine Kommazahl liefert.")])

N("l21", "Warenkorb", """
Schreib eine Klasse `Warenkorb` mit den Feldern `Anzahl` (ganze Zahl) und `Summe` (Kommazahl) und einer Methode `Hinzufuegen(double preis)`, die beides passend erhöht. Lege Artikel für 2,99, 4,50 und 10 Euro hinein und gib aus:

```
3 Artikel, Summe: 17,49 Euro
```
""", r'''
Warenkorb korb = new Warenkorb();
korb.Hinzufuegen(2.99);
korb.Hinzufuegen(4.50);
korb.Hinzufuegen(10);
Console.WriteLine($"{korb.Anzahl} Artikel, Summe: {korb.Summe:F2} Euro");

class Warenkorb
{
    public int Anzahl;
    public double Summe;

    public void Hinzufuegen(double preis)
    {
        Anzahl = Anzahl + 1;
        Summe = Summe + preis;
    }
}
''', [T("3 Artikel, Summe: 17,49 Euro")],
  "Bei jedem Aufruf ändert die Methode beide Felder des Objekts. Im Hauptcode liest du am Ende nur noch die Felder aus.",
  anf=[(r"class\s+Warenkorb", "Schreib die Klasse Warenkorb."),
       (r"void\s+Hinzufuegen\s*\(\s*double", "Die Klasse braucht eine Methode Hinzufuegen mit einem Preis."),
       (r"(Hinzufuegen\s*\([\s\S]*){4}", "Lege alle drei Artikel über die Methode in den Warenkorb."),
       (r"\.\s*Summe", "Die Summe soll aus dem Warenkorb kommen.")])

# ---------------- l22 Konstruktoren ----------------
N("l22", "Haustiere", """
Schreib eine Klasse `Tier` mit den Feldern `Name` und `Art`, einem Konstruktor `Tier(string name, string art)` und einer Methode `Vorstellen()`. Erzeuge die Katze Mimi und die Schildkröte Rex:

```
Mimi ist eine Katze.
Rex ist eine Schildkröte.
```
""", r'''
Tier t1 = new Tier("Mimi", "Katze");
Tier t2 = new Tier("Rex", "Schildkröte");
t1.Vorstellen();
t2.Vorstellen();

class Tier
{
    public string Name;
    public string Art;

    public Tier(string name, string art)
    {
        Name = name;
        Art = art;
    }

    public void Vorstellen()
    {
        Console.WriteLine($"{Name} ist eine {Art}.");
    }
}
''', [T("Mimi ist eine Katze.\nRex ist eine Schildkröte.")],
  "Der Konstruktor heißt genau wie die Klasse und hat keinen Rückgabetyp. Er übernimmt die Werte in die Felder.",
  anf=[(r"public\s+Tier\s*\(\s*string\s+\w+\s*,\s*string\s+\w+\s*\)", "Die Klasse braucht einen Konstruktor mit Name und Art."),
       (r"void\s+Vorstellen\s*\(\s*\)", "Die Klasse braucht eine Methode Vorstellen."),
       (r"new\s+Tier\s*\(\s*\"Mimi\"", "Erzeuge Mimi direkt mit ihren Werten.")])

N("l22", "Rechteck mit Konstruktor", """
Schreib eine Klasse `Rechteck` mit einem Konstruktor `Rechteck(double breite, double hoehe)` und einer Methode `Flaeche()`, die die Fläche liefert. Gib die Flächen für 3 mal 4 und für 2,5 mal 3 aus:

```
Fläche: 12
Fläche: 7,5
```
""", r'''
Rechteck r1 = new Rechteck(3, 4);
Rechteck r2 = new Rechteck(2.5, 3);
Console.WriteLine($"Fläche: {r1.Flaeche()}");
Console.WriteLine($"Fläche: {r2.Flaeche()}");

class Rechteck
{
    public double Breite;
    public double Hoehe;

    public Rechteck(double breite, double hoehe)
    {
        Breite = breite;
        Hoehe = hoehe;
    }

    public double Flaeche()
    {
        return Breite * Hoehe;
    }
}
''', [T("Fläche: 12\nFläche: 7,5")],
  "Speichere die Werte aus dem Konstruktor in Feldern. Die Methode rechnet später mit diesen Feldern.",
  anf=[(r"public\s+Rechteck\s*\(\s*double\s+\w+\s*,\s*double\s+\w+\s*\)", "Die Klasse braucht einen Konstruktor mit Breite und Höhe."),
       (r"double\s+Flaeche\s*\(\s*\)", "Die Klasse braucht eine Methode Flaeche, die eine Kommazahl liefert.")])

N("l22", "Uhrzeit mit zwei Konstruktoren", """
Schreib eine Klasse `Uhrzeit` mit den Feldern `Stunde` und `Minute` und **zwei** Konstruktoren: einen mit Stunde und Minute und einen nur mit der Stunde (die Minute ist dann 0). Eine Methode `Anzeigen()` gibt die Zeit aus. Erzeuge 8:30 und 14 Uhr:

```
8 Uhr 30
14 Uhr 0
```
""", r'''
Uhrzeit u1 = new Uhrzeit(8, 30);
Uhrzeit u2 = new Uhrzeit(14);
u1.Anzeigen();
u2.Anzeigen();

class Uhrzeit
{
    public int Stunde;
    public int Minute;

    public Uhrzeit(int stunde, int minute)
    {
        Stunde = stunde;
        Minute = minute;
    }

    public Uhrzeit(int stunde)
    {
        Stunde = stunde;
        Minute = 0;
    }

    public void Anzeigen()
    {
        Console.WriteLine($"{Stunde} Uhr {Minute}");
    }
}
''', [T("8 Uhr 30\n14 Uhr 0")],
  "Eine Klasse darf mehrere Konstruktoren haben, wenn sich ihre Parameter unterscheiden. C# wählt beim Erzeugen den passenden aus.",
  anf=[(r"public\s+Uhrzeit\s*\(\s*int\s+\w+\s*,\s*int\s+\w+\s*\)", "Es fehlt der Konstruktor mit Stunde und Minute."),
       (r"public\s+Uhrzeit\s*\(\s*int\s+\w+\s*\)", "Es fehlt der Konstruktor nur mit der Stunde."),
       (r"new\s+Uhrzeit\s*\(\s*14\s*\)", "Erzeuge 14 Uhr mit dem Konstruktor, der nur die Stunde bekommt.")])

N("l22", "Konto mit Startbonus", """
Schreib eine Klasse `Konto` mit den Feldern `Inhaber` und `Kontostand` (ganze Zahl). Der Konstruktor `Konto(string inhaber)` setzt den Inhaber und schreibt als Startbonus 10 Euro gut. Dazu kommen `Einzahlen(int betrag)` und `Anzeigen()`. Für Ali: anzeigen, 25 einzahlen, wieder anzeigen:

```
Ali hat 10 Euro.
Ali hat 35 Euro.
```
""", r'''
Konto k = new Konto("Ali");
k.Anzeigen();
k.Einzahlen(25);
k.Anzeigen();

class Konto
{
    public string Inhaber;
    public int Kontostand;

    public Konto(string inhaber)
    {
        Inhaber = inhaber;
        Kontostand = 10;
    }

    public void Einzahlen(int betrag)
    {
        Kontostand = Kontostand + betrag;
    }

    public void Anzeigen()
    {
        Console.WriteLine($"{Inhaber} hat {Kontostand} Euro.");
    }
}
''', [T("Ali hat 10 Euro.\nAli hat 35 Euro.")],
  "Ein Konstruktor darf Felder auch mit festen Startwerten belegen. Der Bonus braucht deshalb keinen eigenen Parameter.",
  anf=[(r"public\s+Konto\s*\(\s*string\s+\w+\s*\)", "Die Klasse braucht einen Konstruktor, der nur den Inhaber bekommt."),
       (r"void\s+Einzahlen\s*\(\s*int", "Die Klasse braucht eine Methode Einzahlen."),
       (r"void\s+Anzeigen\s*\(\s*\)", "Die Klasse braucht eine Methode Anzeigen.")])

N("l22", "Brüche multiplizieren", """
Schreib eine Klasse `Bruch` mit den Feldern `Zaehler` und `Nenner`, einem Konstruktor `Bruch(int zaehler, int nenner)`, einer Methode `AlsText()`, die z. B. `3/4` liefert, und einer Methode `Mal(Bruch anderer)`, die einen **neuen** Bruch als Ergebnis liefert (Zähler mal Zähler, Nenner mal Nenner, ohne Kürzen). Rechne 1/2 mal 3/4:

```
1/2 * 3/4 = 3/8
```
""", r'''
Bruch a = new Bruch(1, 2);
Bruch b = new Bruch(3, 4);
Bruch ergebnis = a.Mal(b);
Console.WriteLine($"{a.AlsText()} * {b.AlsText()} = {ergebnis.AlsText()}");

class Bruch
{
    public int Zaehler;
    public int Nenner;

    public Bruch(int zaehler, int nenner)
    {
        Zaehler = zaehler;
        Nenner = nenner;
    }

    public string AlsText()
    {
        return $"{Zaehler}/{Nenner}";
    }

    public Bruch Mal(Bruch anderer)
    {
        return new Bruch(Zaehler * anderer.Zaehler, Nenner * anderer.Nenner);
    }
}
''', [T("1/2 * 3/4 = 3/8")],
  "Eine Methode kann auch ein Objekt der eigenen Klasse liefern. Darin erzeugst du das Ergebnis mit dem Konstruktor.",
  anf=[(r"public\s+Bruch\s*\(\s*int\s+\w+\s*,\s*int\s+\w+\s*\)", "Die Klasse braucht einen Konstruktor mit Zähler und Nenner."),
       (r"string\s+AlsText\s*\(\s*\)", "Die Klasse braucht eine Methode AlsText, die Text liefert."),
       (r"Bruch\s+Mal\s*\(\s*Bruch\s+\w+\s*\)", "Die Klasse braucht eine Methode Mal, die einen Bruch bekommt und einen Bruch liefert."),
       (r"\.\s*Mal\s*\(", "Lass das Ergebnis von der Methode Mal ausrechnen.")])

# ---------------- l23 Eigenschaften ----------------
AUTO = (r"\{\s*get\s*;\s*set\s*;\s*\}", "Nutze die kurzen Auto-Eigenschaften aus der Lektion.")

N("l23", "Filmsammlung", """
Schreib eine Klasse `Film` mit den Auto-Eigenschaften `Titel` (Text) und `Jahr` (ganze Zahl). Erzeuge die Filme „Matrix“ (1999) und „Oben“ (2009) und gib sie aus:

```
Matrix (1999)
Oben (2009)
```
""", r'''
Film f1 = new Film();
f1.Titel = "Matrix";
f1.Jahr = 1999;
Film f2 = new Film();
f2.Titel = "Oben";
f2.Jahr = 2009;
Console.WriteLine($"{f1.Titel} ({f1.Jahr})");
Console.WriteLine($"{f2.Titel} ({f2.Jahr})");

class Film
{
    public string Titel { get; set; }
    public int Jahr { get; set; }
}
''', [T("Matrix (1999)\nOben (2009)")],
  "Auto-Eigenschaften brauchen kein eigenes Feld. Benutzt werden sie im Hauptcode genau wie Felder.",
  anf=[(r"string\s+Titel\s*\{", "Titel soll eine Eigenschaft sein."), (r"int\s+Jahr\s*\{", "Jahr soll eine Eigenschaft sein."), AUTO])

N("l23", "Nur lesbarer Durchmesser", """
Schreib eine Klasse `Kreis` mit der Auto-Eigenschaft `Radius` (ganze Zahl) und einer Eigenschaft `Durchmesser`, die man nur **lesen** kann und die immer das Doppelte des Radius liefert. Setze den Radius auf 3 und danach auf 5:

```
Radius 3, Durchmesser 6
Radius 5, Durchmesser 10
```
""", r'''
Kreis k = new Kreis();
k.Radius = 3;
Console.WriteLine($"Radius {k.Radius}, Durchmesser {k.Durchmesser}");
k.Radius = 5;
Console.WriteLine($"Radius {k.Radius}, Durchmesser {k.Durchmesser}");

class Kreis
{
    public int Radius { get; set; }

    public int Durchmesser
    {
        get { return Radius * 2; }
    }
}
''', [T("Radius 3, Durchmesser 6\nRadius 5, Durchmesser 10")],
  "Eine Eigenschaft ohne Setzen kann man nur lesen. Ihr Wert wird jedes Mal neu aus dem Radius berechnet.",
  anf=[AUTO, (r"int\s+Durchmesser\s*(=>|\{\s*get\s*(\{|=>))", "Durchmesser soll eine Eigenschaft sein, die man nur lesen kann.")])

N("l23", "Kein negatives Alter", """
Schreib eine Klasse `Person` mit einem privaten Feld für das Alter und einer Eigenschaft `Alter`. Wird ein negativer Wert gesetzt, speichert sie 0. Setze zuerst -5 und dann 12:

```
Alter: 0
Alter: 12
```
""", r'''
Person p = new Person();
p.Alter = -5;
Console.WriteLine($"Alter: {p.Alter}");
p.Alter = 12;
Console.WriteLine($"Alter: {p.Alter}");

class Person
{
    private int alter;

    public int Alter
    {
        get { return alter; }
        set
        {
            if (value < 0)
            {
                alter = 0;
            }
            else
            {
                alter = value;
            }
        }
    }
}
''', [T("Alter: 0\nAlter: 12")],
  "Beim Setzen kommt der neue Wert in einem besonderen Schlüsselwort an. Prüfe ihn, bevor du ihn ins private Feld schreibst.",
  anf=[(r"private\s+int\s+\w+", "Das Alter soll in einem privaten Feld liegen."),
       (r"\bset\s*\{", "Die Eigenschaft Alter soll beim Setzen prüfen."),
       (r"\bvalue\b", "Prüfe beim Setzen den neuen Wert.")])

N("l23", "Besucherzähler", """
Schreib eine Klasse `Besucherzaehler` mit einem privaten Feld für die Anzahl, einer Methode `Eintreten()`, die um 1 erhöht, und einer Eigenschaft `Anzahl`, die man nur **lesen** kann. Lass drei Besucher eintreten:

```
Besucher: 3
```
""", r'''
Besucherzaehler z = new Besucherzaehler();
z.Eintreten();
z.Eintreten();
z.Eintreten();
Console.WriteLine($"Besucher: {z.Anzahl}");

class Besucherzaehler
{
    private int anzahl;

    public int Anzahl
    {
        get { return anzahl; }
    }

    public void Eintreten()
    {
        anzahl++;
    }
}
''', [T("Besucher: 3")],
  "Von außen kann man die Anzahl nur lesen, ändern darf sie nur die Klasse selbst. Das erledigt die Methode.",
  anf=[(r"private\s+int\s+\w+", "Die Anzahl soll in einem privaten Feld liegen."),
       (r"void\s+Eintreten\s*\(\s*\)", "Die Klasse braucht eine Methode Eintreten."),
       (r"int\s+Anzahl\s*(=>|\{\s*get\s*(\{|=>|;\s*private))", "Anzahl soll eine Eigenschaft sein, die man von außen nur lesen kann."),
       (r"(Eintreten\s*\(\s*\)\s*;[\s\S]*){3}", "Lass drei Besucher eintreten.")])

N("l23", "Aufgeräumter Name", """
Schreib eine Klasse `Benutzer` mit einer Eigenschaft `Name`. Beim Setzen werden Leerzeichen am Anfang und Ende entfernt. Bleibt dann nichts übrig, wird `Unbekannt` gespeichert. Setze zuerst `"  Lisa  "` und dann `"   "`:

```
Name: Lisa
Name: Unbekannt
```
""", r'''
Benutzer b = new Benutzer();
b.Name = "  Lisa  ";
Console.WriteLine($"Name: {b.Name}");
b.Name = "   ";
Console.WriteLine($"Name: {b.Name}");

class Benutzer
{
    private string name = "Unbekannt";

    public string Name
    {
        get { return name; }
        set
        {
            string sauber = value.Trim();
            if (sauber == "")
            {
                name = "Unbekannt";
            }
            else
            {
                name = sauber;
            }
        }
    }
}
''', [T("Name: Lisa\nName: Unbekannt")],
  "Fürs Entfernen der Leerzeichen kennst du eine String-Methode aus Lektion 18. Den bereinigten Wert prüfst du, bevor du ihn speicherst.",
  anf=[(r"\bset\s*\{", "Die Eigenschaft Name soll beim Setzen aufräumen."),
       (r"\bvalue\b", "Bearbeite beim Setzen den neuen Wert.")])

# ---------------- l24 Listen von Objekten ----------------
N("l24", "Tierliste", """
Schreib eine Klasse `Tier` mit `Name` und `Beine`. Lege eine Liste mit Hund (4 Beine), Huhn (2) und Spinne (8) an. Gib jedes Tier aus und am Ende alle Beine zusammen:

```
Hund hat 4 Beine.
Huhn hat 2 Beine.
Spinne hat 8 Beine.
Beine insgesamt: 14
```
""", r'''
List<Tier> tiere = new List<Tier>();
tiere.Add(new Tier("Hund", 4));
tiere.Add(new Tier("Huhn", 2));
tiere.Add(new Tier("Spinne", 8));

int summe = 0;
foreach (Tier t in tiere)
{
    Console.WriteLine($"{t.Name} hat {t.Beine} Beine.");
    summe = summe + t.Beine;
}
Console.WriteLine($"Beine insgesamt: {summe}");

class Tier
{
    public string Name { get; }
    public int Beine { get; }

    public Tier(string name, int beine)
    {
        Name = name;
        Beine = beine;
    }
}
''', [T("Hund hat 4 Beine.\nHuhn hat 2 Beine.\nSpinne hat 8 Beine.\nBeine insgesamt: 14")],
  "Die Liste nimmt Objekte deiner Klasse auf. Beim Durchlaufen bekommst du jedes Tier mit allen seinen Werten.",
  anf=[(r"class\s+Tier", "Schreib die Klasse Tier."),
       (r"List\s*<\s*Tier\s*>", "Sammle die Tiere in einer Liste."),
       (r"\bforeach\s*\(|\bfor\s*\(", "Geh die Liste in einer Schleife durch.")])

N("l24", "Älteste Person", """
Schreib eine Klasse `Person` mit `Name` und `Alter`. Lege eine Liste mit Mia (14), Opa Karl (81), Mama (45) und Ben (9) an. Finde die älteste Person, indem du die Liste durchgehst:

```
Älteste Person: Opa Karl (81)
```
""", r'''
List<Person> familie = new List<Person>
{
    new Person("Mia", 14),
    new Person("Opa Karl", 81),
    new Person("Mama", 45),
    new Person("Ben", 9),
};

Person aelteste = familie[0];
foreach (Person p in familie)
{
    if (p.Alter > aelteste.Alter)
    {
        aelteste = p;
    }
}
Console.WriteLine($"Älteste Person: {aelteste.Name} ({aelteste.Alter})");

class Person
{
    public string Name { get; }
    public int Alter { get; }

    public Person(string name, int alter)
    {
        Name = name;
        Alter = alter;
    }
}
''', [T("Älteste Person: Opa Karl (81)")],
  "Merk dir die bisher älteste Person als ganzes Objekt. So hast du am Ende Name und Alter zusammen.",
  anf=[(r"List\s*<\s*Person\s*>", "Sammle die Personen in einer Liste."),
       (r"\bforeach\s*\(|\bfor\s*\(", "Geh die Liste selbst durch.")])

N("l24", "Günstige Schulsachen", """
Schreib eine Klasse `Produkt` mit `Name` und `Preis`. Lege eine Liste an mit Stift (1,50), Rucksack (39,99), Heft (2,20), Lineal (0,99) und Taschenrechner (14,90). Gib nur die Produkte unter 10 Euro aus und danach ihre Anzahl:

```
Stift: 1,50 Euro
Heft: 2,20 Euro
Lineal: 0,99 Euro
3 Produkte unter 10 Euro
```
""", r'''
List<Produkt> produkte = new List<Produkt>
{
    new Produkt("Stift", 1.50),
    new Produkt("Rucksack", 39.99),
    new Produkt("Heft", 2.20),
    new Produkt("Lineal", 0.99),
    new Produkt("Taschenrechner", 14.90),
};

int anzahl = 0;
foreach (Produkt p in produkte)
{
    if (p.Preis < 10)
    {
        Console.WriteLine($"{p.Name}: {p.Preis:F2} Euro");
        anzahl++;
    }
}
Console.WriteLine($"{anzahl} Produkte unter 10 Euro");

class Produkt
{
    public string Name { get; }
    public double Preis { get; }

    public Produkt(string name, double preis)
    {
        Name = name;
        Preis = preis;
    }
}
''', [T("Stift: 1,50 Euro\nHeft: 2,20 Euro\nLineal: 0,99 Euro\n3 Produkte unter 10 Euro")],
  "Prüfe beim Durchlaufen für jedes Produkt den Preis. Gezählt und ausgegeben wird nur, was passt.",
  anf=[(r"List\s*<\s*Produkt\s*>", "Sammle die Produkte in einer Liste."),
       (r"\bforeach\s*\(|\bfor\s*\(", "Geh die Liste in einer Schleife durch.")])

N("l24", "Telefonbuch", """
Schreib eine Klasse `Kontakt` mit `Name` und `Telefon`. Lege eine Liste an mit Ana (0151-111), Ben (0160-222) und Cem (0176-333). Lies einen Namen ein und gib die Nummer aus oder `Kein Eintrag für ...`. Bei `Ben`:

```
Ben: 0160-222
```
""", r'''
List<Kontakt> kontakte = new List<Kontakt>
{
    new Kontakt("Ana", "0151-111"),
    new Kontakt("Ben", "0160-222"),
    new Kontakt("Cem", "0176-333"),
};

string gesucht = Console.ReadLine();
Kontakt gefunden = null;
foreach (Kontakt k in kontakte)
{
    if (k.Name == gesucht)
    {
        gefunden = k;
    }
}
if (gefunden != null)
{
    Console.WriteLine($"{gefunden.Name}: {gefunden.Telefon}");
}
else
{
    Console.WriteLine($"Kein Eintrag für {gesucht}.");
}

class Kontakt
{
    public string Name { get; }
    public string Telefon { get; }

    public Kontakt(string name, string telefon)
    {
        Name = name;
        Telefon = telefon;
    }
}
''', [T("Ben: 0160-222", "Ben"), T("Cem: 0176-333", "Cem"), T("Kein Eintrag für Dora.", "Dora")],
  "Geh die Liste durch und vergleiche jeden Namen mit der Eingabe. Merk dir, ob du einen Treffer hattest.", "lenient",
  anf=[(r"List\s*<\s*Kontakt\s*>", "Sammle die Kontakte in einer Liste.")])

N("l24", "Siegerehrung", """
Lies die Anzahl der Spieler ein, dann für jeden Namen und Punkte. Speichere sie als Objekte einer Klasse `Spieler` in einer Liste. Gib den Sieger und den Punkte-Durchschnitt mit einer Nachkommastelle aus. Bei `3`, `Ana`, `70`, `Cem`, `95`, `Ben`, `75`:

```
Sieger: Cem mit 95 Punkten
Durchschnitt: 80,0 Punkte
```
""", r'''
List<Spieler> spieler = new List<Spieler>();
int anzahl = int.Parse(Console.ReadLine());
for (int i = 0; i < anzahl; i++)
{
    Spieler s = new Spieler();
    s.Name = Console.ReadLine();
    s.Punkte = int.Parse(Console.ReadLine());
    spieler.Add(s);
}

Spieler sieger = spieler[0];
int summe = 0;
foreach (Spieler s in spieler)
{
    if (s.Punkte > sieger.Punkte)
    {
        sieger = s;
    }
    summe = summe + s.Punkte;
}
Console.WriteLine($"Sieger: {sieger.Name} mit {sieger.Punkte} Punkten");
Console.WriteLine($"Durchschnitt: {(double)summe / spieler.Count:F1} Punkte");

class Spieler
{
    public string Name { get; set; }
    public int Punkte { get; set; }
}
''', [T("Sieger: Cem mit 95 Punkten\nDurchschnitt: 80,0 Punkte", "3", "Ana", "70", "Cem", "95", "Ben", "75"),
      T("Sieger: Lea mit 12 Punkten\nDurchschnitt: 11,0 Punkte", "2", "Lea", "12", "Tim", "10")],
  "Erst einlesen und speichern, dann auswerten. Beim Durchlaufen kannst du Sieger und Summe gleichzeitig bestimmen.", "lenient",
  anf=[(r"class\s+Spieler", "Schreib die Klasse Spieler."), (r"List\s*<\s*Spieler\s*>", "Speichere die Spieler in einer Liste.")])

# ---------------- l25 Notenrechner ----------------
EINLESEN = "Lies wie in Schritt 1 zuerst die Anzahl der Noten und dann die Noten selbst ein."

N("l25", "Schritt 4: Ungültige Noten", f"""
{EINLESEN} Noten außer 1 bis 6 werden mit `Ungültige Note: ...` gemeldet und nicht gespeichert. Gib danach die gültigen Noten aus. Bei `4`, `2`, `7`, `1`, `0`:

```
Ungültige Note: 7
Ungültige Note: 0
Deine Noten: 2, 1
```
""", r'''
List<int> noten = new List<int>();
int anzahl = int.Parse(Console.ReadLine());
for (int i = 0; i < anzahl; i++)
{
    int note = int.Parse(Console.ReadLine());
    if (note >= 1 && note <= 6)
    {
        noten.Add(note);
    }
    else
    {
        Console.WriteLine($"Ungültige Note: {note}");
    }
}
Console.WriteLine("Deine Noten: " + string.Join(", ", noten));
''', [T("Ungültige Note: 7\nUngültige Note: 0\nDeine Noten: 2, 1", "4", "2", "7", "1", "0"), T("Deine Noten: 3, 3", "2", "3", "3")],
  "Prüfe jede Note gleich nach dem Einlesen. Nur gültige Noten kommen in die Liste.", "lenient")

N("l25", "Schritt 5: Notenspiegel", f"""
{EINLESEN} Gib dann für jede Note von 1 bis 6 aus, wie oft sie vorkommt. Bei `5`, `2`, `1`, `2`, `3`, `2`:

```
Note 1: 1x
Note 2: 3x
Note 3: 1x
Note 4: 0x
Note 5: 0x
Note 6: 0x
```
""", r'''
List<int> noten = new List<int>();
int anzahl = int.Parse(Console.ReadLine());
for (int i = 0; i < anzahl; i++)
{
    noten.Add(int.Parse(Console.ReadLine()));
}
for (int note = 1; note <= 6; note++)
{
    int wieOft = 0;
    foreach (int n in noten)
    {
        if (n == note)
        {
            wieOft++;
        }
    }
    Console.WriteLine($"Note {note}: {wieOft}x");
}
''', [T("Note 1: 1x\nNote 2: 3x\nNote 3: 1x\nNote 4: 0x\nNote 5: 0x\nNote 6: 0x", "5", "2", "1", "2", "3", "2"),
      T("Note 1: 0x\nNote 2: 0x\nNote 3: 0x\nNote 4: 1x\nNote 5: 0x\nNote 6: 1x", "2", "6", "4")],
  "Eine äußere Schleife geht die Noten 1 bis 6 durch. Für jede zählst du, wie oft sie in der Liste steht.", "lenient")

N("l25", "Schritt 6: Tendenz", f"""
{EINLESEN} Vergleiche die erste mit der letzten Note: Ist die letzte besser (kleiner), erscheint `Du wirst besser!`, ist sie schlechter `Du wirst schlechter.`, sonst `Gleich geblieben.`. Bei `3`, `4`, `3`, `2`:

```
Du wirst besser!
```
""", r'''
List<int> noten = new List<int>();
int anzahl = int.Parse(Console.ReadLine());
for (int i = 0; i < anzahl; i++)
{
    noten.Add(int.Parse(Console.ReadLine()));
}
int erste = noten[0];
int letzte = noten[noten.Count - 1];
if (letzte < erste)
{
    Console.WriteLine("Du wirst besser!");
}
else if (letzte > erste)
{
    Console.WriteLine("Du wirst schlechter.");
}
else
{
    Console.WriteLine("Gleich geblieben.");
}
''', [T("Du wirst besser!", "3", "4", "3", "2"), T("Du wirst schlechter.", "2", "1", "3"), T("Gleich geblieben.", "3", "2", "5", "2")],
  "Bei Schulnoten ist die kleinere Zahl die bessere. Die letzte Note findest du über die Anzahl der Liste.", "lenient")

N("l25", "Schritt 7: Fächer", """
Lies die Anzahl der Fächer ein, dann für jedes Fach den Namen und die Note. Gib alle Fächer aus und danach das beste Fach (bei gleicher Note das zuerst eingegebene). Bei `3`, `Mathe`, `2`, `Deutsch`, `1`, `Sport`, `1`:

```
Mathe: 2
Deutsch: 1
Sport: 1
Bestes Fach: Deutsch (1)
```
""", r'''
List<string> faecher = new List<string>();
List<int> noten = new List<int>();
int anzahl = int.Parse(Console.ReadLine());
for (int i = 0; i < anzahl; i++)
{
    faecher.Add(Console.ReadLine());
    noten.Add(int.Parse(Console.ReadLine()));
}
int beste = 0;
for (int i = 0; i < anzahl; i++)
{
    Console.WriteLine($"{faecher[i]}: {noten[i]}");
    if (noten[i] < noten[beste])
    {
        beste = i;
    }
}
Console.WriteLine($"Bestes Fach: {faecher[beste]} ({noten[beste]})");
''', [T("Mathe: 2\nDeutsch: 1\nSport: 1\nBestes Fach: Deutsch (1)", "3", "Mathe", "2", "Deutsch", "1", "Sport", "1"),
      T("Kunst: 3\nBio: 4\nBestes Fach: Kunst (3)", "2", "Kunst", "3", "Bio", "4")],
  "Fach und Note gehören zusammen – speichere sie so, dass du sie wiederfindest. Beim besten Fach zählt nur eine echt kleinere Note.", "lenient")

N("l25", "Schritt 8: Das ganze Zeugnis", f"""
{EINLESEN} Schreib Methoden `Durchschnitt`, `Beste`, `Schlechteste` und `Bewertung` (Bewertung wie in Schritt 3) und gib das ganze Zeugnis aus. Bei `3`, `2`, `1`, `3`:

```
Deine Noten: 2, 1, 3
Durchschnitt: 2,00
Beste Note: 1
Schlechteste Note: 3
Gut gemacht!
```
""", r'''
List<int> noten = new List<int>();
int anzahl = int.Parse(Console.ReadLine());
for (int i = 0; i < anzahl; i++)
{
    noten.Add(int.Parse(Console.ReadLine()));
}
double schnitt = Durchschnitt(noten);
Console.WriteLine("Deine Noten: " + string.Join(", ", noten));
Console.WriteLine($"Durchschnitt: {schnitt:F2}");
Console.WriteLine($"Beste Note: {Beste(noten)}");
Console.WriteLine($"Schlechteste Note: {Schlechteste(noten)}");
Console.WriteLine(Bewertung(schnitt));

static double Durchschnitt(List<int> noten)
{
    int summe = 0;
    foreach (int n in noten)
    {
        summe = summe + n;
    }
    return (double)summe / noten.Count;
}

static int Beste(List<int> noten)
{
    int beste = noten[0];
    foreach (int n in noten)
    {
        if (n < beste) beste = n;
    }
    return beste;
}

static int Schlechteste(List<int> noten)
{
    int schlechteste = noten[0];
    foreach (int n in noten)
    {
        if (n > schlechteste) schlechteste = n;
    }
    return schlechteste;
}

static string Bewertung(double schnitt)
{
    if (schnitt <= 1.5) return "Ausgezeichnet!";
    if (schnitt <= 2.5) return "Gut gemacht!";
    if (schnitt <= 3.5) return "Befriedigend.";
    return "Da geht noch was!";
}
''', [T("Deine Noten: 2, 1, 3\nDurchschnitt: 2,00\nBeste Note: 1\nSchlechteste Note: 3\nGut gemacht!", "3", "2", "1", "3"),
      T("Deine Noten: 4, 5\nDurchschnitt: 4,50\nBeste Note: 4\nSchlechteste Note: 5\nDa geht noch was!", "2", "4", "5")],
  "Jede Methode bekommt die Liste als Parameter und liefert ein Ergebnis. Die Bewertung braucht nur den Durchschnitt.", "lenient",
  anf=[(r"static\s+double\s+Durchschnitt\s*\(", "Schreib eine Methode Durchschnitt, die eine Kommazahl liefert."),
       (r"static\s+int\s+Beste\s*\(", "Schreib eine Methode Beste."),
       (r"static\s+int\s+Schlechteste\s*\(", "Schreib eine Methode Schlechteste."),
       (r"static\s+string\s+Bewertung\s*\(", "Schreib eine Methode Bewertung, die Text liefert.")])

# ---------------- l26 Zahlenraten ----------------
N("l26", "Zwei Spieler", """
Spieler 1 gibt zuerst die geheime Zahl ein, danach rät Spieler 2. Nach jedem falschen Tipp erscheint `Zu klein!` oder `Zu groß!`, beim Treffer `Richtig nach ... Versuchen!`. Bei `42`, `50`, `40`, `42`:

```
Zu groß!
Zu klein!
Richtig nach 3 Versuchen!
```
""", r'''
int geheim = int.Parse(Console.ReadLine());
int versuche = 0;
int tipp = 0;
while (tipp != geheim)
{
    tipp = int.Parse(Console.ReadLine());
    versuche++;
    if (tipp < geheim)
    {
        Console.WriteLine("Zu klein!");
    }
    else if (tipp > geheim)
    {
        Console.WriteLine("Zu groß!");
    }
}
Console.WriteLine($"Richtig nach {versuche} Versuchen!");
''', [T("Zu groß!\nZu klein!\nRichtig nach 3 Versuchen!", "42", "50", "40", "42"), T("Richtig nach 1 Versuchen!", "7", "7"),
      T("Zu klein!\nRichtig nach 2 Versuchen!", "90", "10", "90")],
  "Die erste Eingabe ist die geheime Zahl, alle weiteren sind Tipps. Der Rest läuft wie in Schritt 2.", "lenient")

N("l26", "Heiß oder kalt", """
Die geheime Zahl ist `37`. Lass so lange raten, bis sie getroffen ist. Nach jedem Tipp erscheint: `Treffer!` bei der richtigen Zahl, `Heiß!` bei höchstens 5 daneben, `Warm.` bei höchstens 15 daneben, sonst `Kalt.`. Bei `10`, `30`, `36`, `37`:

```
Kalt.
Warm.
Heiß!
Treffer!
```
""", r'''
int geheim = 37;
int tipp = 0;
while (tipp != geheim)
{
    tipp = int.Parse(Console.ReadLine());
    int abstand = Math.Abs(tipp - geheim);
    if (abstand == 0)
    {
        Console.WriteLine("Treffer!");
    }
    else if (abstand <= 5)
    {
        Console.WriteLine("Heiß!");
    }
    else if (abstand <= 15)
    {
        Console.WriteLine("Warm.");
    }
    else
    {
        Console.WriteLine("Kalt.");
    }
}
''', [T("Kalt.\nWarm.\nHeiß!\nTreffer!", "10", "30", "36", "37"), T("Treffer!", "37"), T("Heiß!\nWarm.\nTreffer!", "42", "52", "37")],
  "Wie weit ein Tipp danebenliegt, ist der Abstand ohne Vorzeichen. Die Mathe-Klasse aus Lektion 20 hilft dir dabei.", "lenient")

N("l26", "Der Bereich wird kleiner", """
Die geheime Zahl ist `37` und liegt zwischen 1 und 100. Nach jedem falschen Tipp erscheint der neue Bereich, in dem die Zahl noch liegen kann. Beim Treffer erscheint `Richtig!`. Bei `50`, `25`, `37`:

```
Die Zahl liegt zwischen 1 und 49.
Die Zahl liegt zwischen 26 und 49.
Richtig!
```
""", r'''
int geheim = 37;
int unten = 1;
int oben = 100;
int tipp = 0;
while (tipp != geheim)
{
    tipp = int.Parse(Console.ReadLine());
    if (tipp == geheim)
    {
        Console.WriteLine("Richtig!");
    }
    else
    {
        if (tipp > geheim)
        {
            oben = tipp - 1;
        }
        else
        {
            unten = tipp + 1;
        }
        Console.WriteLine($"Die Zahl liegt zwischen {unten} und {oben}.");
    }
}
''', [T("Die Zahl liegt zwischen 1 und 49.\nDie Zahl liegt zwischen 26 und 49.\nRichtig!", "50", "25", "37"),
      T("Die Zahl liegt zwischen 31 und 100.\nDie Zahl liegt zwischen 31 und 39.\nRichtig!", "30", "40", "37")],
  "Merk dir die untere und die obere Grenze in zwei Variablen. Ein zu großer Tipp verschiebt die obere Grenze, ein zu kleiner die untere.", "lenient")

N("l26", "Punkte sammeln", """
Die geheime Zahl ist `37`. Du startest mit 100 Punkten, jeder falsche Tipp kostet 10 Punkte (weniger als 0 gibt es nicht). Nach falschen Tipps erscheint `Zu klein!` oder `Zu groß!`, beim Treffer `Richtig! Punkte: ...`. Bei `50`, `25`, `37`:

```
Zu groß!
Zu klein!
Richtig! Punkte: 80
```
""", r'''
int geheim = 37;
int punkte = 100;
int tipp = int.Parse(Console.ReadLine());
while (tipp != geheim)
{
    if (tipp < geheim)
    {
        Console.WriteLine("Zu klein!");
    }
    else
    {
        Console.WriteLine("Zu groß!");
    }
    punkte = Math.Max(punkte - 10, 0);
    tipp = int.Parse(Console.ReadLine());
}
Console.WriteLine($"Richtig! Punkte: {punkte}");
''', [T("Zu groß!\nZu klein!\nRichtig! Punkte: 80", "50", "25", "37"), T("Richtig! Punkte: 100", "37"),
      T("Richtig! Punkte: 0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "37")],
  "Zieh die Punkte nur bei falschen Tipps ab. Achte darauf, dass der Punktestand nie unter 0 fällt.", "lenient")

N("l26", "Mehrere Runden", """
Jede Runde beginnt damit, dass die geheime Zahl eingegeben wird. Danach wird geraten (`Zu klein!`/`Zu groß!`), bis sie getroffen ist: `Runde 1: Richtig nach 2 Versuchen!`. Dann wird `j` oder `n` eingegeben – bei `j` folgt eine neue Runde. Am Ende erscheint `Danke fürs Spielen! Runden: ...`. Bei `5`, `3`, `5`, `j`, `8`, `8`, `n`:

```
Zu klein!
Runde 1: Richtig nach 2 Versuchen!
Runde 2: Richtig nach 1 Versuchen!
Danke fürs Spielen! Runden: 2
```
""", r'''
int runde = 0;
string nochmal = "j";
while (nochmal == "j")
{
    runde++;
    int geheim = int.Parse(Console.ReadLine());
    int versuche = 0;
    int tipp = 0;
    while (tipp != geheim)
    {
        tipp = int.Parse(Console.ReadLine());
        versuche++;
        if (tipp < geheim)
        {
            Console.WriteLine("Zu klein!");
        }
        else if (tipp > geheim)
        {
            Console.WriteLine("Zu groß!");
        }
    }
    Console.WriteLine($"Runde {runde}: Richtig nach {versuche} Versuchen!");
    nochmal = Console.ReadLine();
}
Console.WriteLine($"Danke fürs Spielen! Runden: {runde}");
''', [T("Zu klein!\nRunde 1: Richtig nach 2 Versuchen!\nRunde 2: Richtig nach 1 Versuchen!\nDanke fürs Spielen! Runden: 2", "5", "3", "5", "j", "8", "8", "n"),
      T("Zu groß!\nZu groß!\nRunde 1: Richtig nach 3 Versuchen!\nDanke fürs Spielen! Runden: 1", "10", "50", "20", "10", "n")],
  "Eine äußere Schleife zählt die Runden, eine innere erledigt das Raten. Die Frage nach einer neuen Runde kommt nach jedem Treffer.", "lenient")

# ---------------- l27 Einkaufsliste ----------------
GRUND = "Die Befehle `1` (hinzufügen: `Milch hinzugefügt.`), `2` (nummeriert anzeigen, bei leerer Liste `Die Liste ist leer.`) und `0` (`Tschüss!`) funktionieren wie in Schritt 1."

N("l27", "Befehl 4: Liste leeren", f"""
{GRUND} Neu ist Befehl `4`: Er leert die ganze Liste und meldet `Liste geleert.`. Bei `1`, `Milch`, `4`, `2`, `0`:

```
Milch hinzugefügt.
Liste geleert.
Die Liste ist leer.
Tschüss!
```
""", r'''
List<string> liste = new List<string>();
bool laeuft = true;
while (laeuft)
{
    string befehl = Console.ReadLine();
    switch (befehl)
    {
        case "1":
            string artikel = Console.ReadLine();
            liste.Add(artikel);
            Console.WriteLine($"{artikel} hinzugefügt.");
            break;
        case "2":
            if (liste.Count == 0)
                Console.WriteLine("Die Liste ist leer.");
            for (int i = 0; i < liste.Count; i++)
                Console.WriteLine($"{i + 1}. {liste[i]}");
            break;
        case "4":
            liste.Clear();
            Console.WriteLine("Liste geleert.");
            break;
        case "0":
            Console.WriteLine("Tschüss!");
            laeuft = false;
            break;
    }
}
''', [T("Milch hinzugefügt.\nListe geleert.\nDie Liste ist leer.\nTschüss!", "1", "Milch", "4", "2", "0"),
      T("Brot hinzugefügt.\nListe geleert.\nEier hinzugefügt.\n1. Eier\nTschüss!", "1", "Brot", "4", "1", "Eier", "2", "0")],
  "Ergänze im Menü einen weiteren Fall. Die Liste hat einen fertigen Befehl, der alles auf einmal entfernt.", "lenient")

N("l27", "Befehl 5: Anzahl", f"""
{GRUND} Neu ist Befehl `5`: Er zeigt, wie viele Artikel auf der Liste stehen. Bei `1`, `Milch`, `1`, `Brot`, `5`, `0`:

```
Milch hinzugefügt.
Brot hinzugefügt.
2 Artikel auf der Liste.
Tschüss!
```
""", r'''
List<string> liste = new List<string>();
bool laeuft = true;
while (laeuft)
{
    string befehl = Console.ReadLine();
    switch (befehl)
    {
        case "1":
            string artikel = Console.ReadLine();
            liste.Add(artikel);
            Console.WriteLine($"{artikel} hinzugefügt.");
            break;
        case "2":
            if (liste.Count == 0)
                Console.WriteLine("Die Liste ist leer.");
            for (int i = 0; i < liste.Count; i++)
                Console.WriteLine($"{i + 1}. {liste[i]}");
            break;
        case "5":
            Console.WriteLine($"{liste.Count} Artikel auf der Liste.");
            break;
        case "0":
            Console.WriteLine("Tschüss!");
            laeuft = false;
            break;
    }
}
''', [T("Milch hinzugefügt.\nBrot hinzugefügt.\n2 Artikel auf der Liste.\nTschüss!", "1", "Milch", "1", "Brot", "5", "0"),
      T("0 Artikel auf der Liste.\nTschüss!", "5", "0")],
  "Die Liste weiß selbst, wie viele Einträge sie hat. Du musst nicht mitzählen.", "lenient")

N("l27", "Sortiert anzeigen", f"""
{GRUND} Ändere Befehl `2` so, dass die Artikel **alphabetisch sortiert** angezeigt werden. Bei `1`, `Milch`, `1`, `Brot`, `1`, `Eier`, `2`, `0`:

```
Milch hinzugefügt.
Brot hinzugefügt.
Eier hinzugefügt.
1. Brot
2. Eier
3. Milch
Tschüss!
```
""", r'''
List<string> liste = new List<string>();
bool laeuft = true;
while (laeuft)
{
    string befehl = Console.ReadLine();
    switch (befehl)
    {
        case "1":
            string artikel = Console.ReadLine();
            liste.Add(artikel);
            Console.WriteLine($"{artikel} hinzugefügt.");
            break;
        case "2":
            if (liste.Count == 0)
                Console.WriteLine("Die Liste ist leer.");
            liste.Sort();
            for (int i = 0; i < liste.Count; i++)
                Console.WriteLine($"{i + 1}. {liste[i]}");
            break;
        case "0":
            Console.WriteLine("Tschüss!");
            laeuft = false;
            break;
    }
}
''', [T("Milch hinzugefügt.\nBrot hinzugefügt.\nEier hinzugefügt.\n1. Brot\n2. Eier\n3. Milch\nTschüss!", "1", "Milch", "1", "Brot", "1", "Eier", "2", "0"),
      T("Zucker hinzugefügt.\nApfel hinzugefügt.\n1. Apfel\n2. Zucker\nTschüss!", "1", "Zucker", "1", "Apfel", "2", "0")],
  "Sortiere die Liste, bevor du sie ausgibst. Der Befehl dafür stammt aus Lektion 15.", "lenient")

N("l27", "Befehl 6: Suchen", f"""
{GRUND} Neu ist Befehl `6`: Er liest einen Namen ein und meldet, an welcher Stelle der Artikel steht, oder dass er fehlt. Bei `1`, `Milch`, `1`, `Brot`, `6`, `Brot`, `6`, `Käse`, `0`:

```
Milch hinzugefügt.
Brot hinzugefügt.
Brot steht auf Platz 2.
Käse ist nicht auf der Liste.
Tschüss!
```
""", r'''
List<string> liste = new List<string>();
bool laeuft = true;
while (laeuft)
{
    string befehl = Console.ReadLine();
    switch (befehl)
    {
        case "1":
            string artikel = Console.ReadLine();
            liste.Add(artikel);
            Console.WriteLine($"{artikel} hinzugefügt.");
            break;
        case "2":
            if (liste.Count == 0)
                Console.WriteLine("Die Liste ist leer.");
            for (int i = 0; i < liste.Count; i++)
                Console.WriteLine($"{i + 1}. {liste[i]}");
            break;
        case "6":
            string gesucht = Console.ReadLine();
            int platz = -1;
            for (int i = 0; i < liste.Count; i++)
            {
                if (liste[i] == gesucht)
                {
                    platz = i + 1;
                    break;
                }
            }
            if (platz > 0)
                Console.WriteLine($"{gesucht} steht auf Platz {platz}.");
            else
                Console.WriteLine($"{gesucht} ist nicht auf der Liste.");
            break;
        case "0":
            Console.WriteLine("Tschüss!");
            laeuft = false;
            break;
    }
}
''', [T("Milch hinzugefügt.\nBrot hinzugefügt.\nBrot steht auf Platz 2.\nKäse ist nicht auf der Liste.\nTschüss!", "1", "Milch", "1", "Brot", "6", "Brot", "6", "Käse", "0"),
      T("Eier hinzugefügt.\nEier steht auf Platz 1.\nTschüss!", "1", "Eier", "6", "Eier", "0")],
  "Geh die Liste mit Index durch, dann kennst du die Position. Für Menschen beginnt die Zählung bei 1.", "lenient")

N("l27", "Mengen ändern", """
Speichere Artikel als Objekte einer Klasse `Artikel` mit `Name` und `Menge`. Befehle:
- `1`: Name und Menge einlesen → `2x Milch hinzugefügt.`
- `2`: alle anzeigen → `Milch (2)`
- `4`: Name und neue Menge einlesen → `Milch: jetzt 5`, fehlt der Artikel: `Käse ist nicht auf der Liste.`
- `0`: `Tschüss! 2 Artikel, 15 Stück insgesamt.` und beenden

Bei `1`, `Milch`, `2`, `1`, `Eier`, `10`, `4`, `Milch`, `5`, `4`, `Käse`, `1`, `2`, `0`:

```
2x Milch hinzugefügt.
10x Eier hinzugefügt.
Milch: jetzt 5
Käse ist nicht auf der Liste.
Milch (5)
Eier (10)
Tschüss! 2 Artikel, 15 Stück insgesamt.
```
""", r'''
List<Artikel> liste = new List<Artikel>();
bool laeuft = true;
while (laeuft)
{
    string befehl = Console.ReadLine();
    switch (befehl)
    {
        case "1":
        {
            Artikel a = new Artikel();
            a.Name = Console.ReadLine();
            a.Menge = int.Parse(Console.ReadLine());
            liste.Add(a);
            Console.WriteLine($"{a.Menge}x {a.Name} hinzugefügt.");
            break;
        }
        case "2":
            foreach (Artikel a in liste)
                Console.WriteLine($"{a.Name} ({a.Menge})");
            break;
        case "4":
        {
            string name = Console.ReadLine();
            int menge = int.Parse(Console.ReadLine());
            Artikel gefunden = null;
            foreach (Artikel a in liste)
            {
                if (a.Name == name) gefunden = a;
            }
            if (gefunden != null)
            {
                gefunden.Menge = menge;
                Console.WriteLine($"{name}: jetzt {menge}");
            }
            else
            {
                Console.WriteLine($"{name} ist nicht auf der Liste.");
            }
            break;
        }
        case "0":
            int stueck = 0;
            foreach (Artikel a in liste)
                stueck = stueck + a.Menge;
            Console.WriteLine($"Tschüss! {liste.Count} Artikel, {stueck} Stück insgesamt.");
            laeuft = false;
            break;
    }
}

class Artikel
{
    public string Name { get; set; }
    public int Menge { get; set; }
}
''', [T("2x Milch hinzugefügt.\n10x Eier hinzugefügt.\nMilch: jetzt 5\nKäse ist nicht auf der Liste.\nMilch (5)\nEier (10)\nTschüss! 2 Artikel, 15 Stück insgesamt.",
        "1", "Milch", "2", "1", "Eier", "10", "4", "Milch", "5", "4", "Käse", "1", "2", "0"),
      T("Tschüss! 0 Artikel, 0 Stück insgesamt.", "0")],
  "Beim Ändern suchst du zuerst das passende Objekt in der Liste. Die Gesamtmenge sammelst du erst beim Beenden.", "lenient",
  anf=[(r"class\s+Artikel", "Schreib die Klasse Artikel."), (r"List\s*<\s*Artikel\s*>", "Speichere die Artikel als Objekte in einer Liste.")])
