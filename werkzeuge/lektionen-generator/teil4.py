# Stufe 4: Klassen & Objekte
from gen import L, A, T, Q

L(4, "l21", "Deine erste Klasse",
  intro="Eine Klasse ist ein Bauplan. Aus dem Bauplan erzeugst du mit new beliebig viele Objekte.",
  explanation=r'''
## Bauplan: die Klasse
```
class Hund
{
    public string Name;
    public int Alter;

    public void Bellen()
    {
        Console.WriteLine(Name + " sagt: Wuff!");
    }
}
```
- **Felder** (`Name`, `Alter`) speichern die Daten eines Objekts.
- **Methoden** (`Bellen`) beschreiben, was ein Objekt kann.
- `public` heißt: Man darf von außen darauf zugreifen.

## Objekte erzeugen
```
Hund bello = new Hund();
bello.Name = "Bello";
bello.Alter = 3;
bello.Bellen();
```
Mit `new` entsteht ein neues **Objekt** (auch **Instanz** genannt). Mit dem **Punkt** greifst du auf Felder und Methoden zu.

## Wo steht die Klasse?
Bei Top-Level-Code kommen Klassen **ans Ende** der Datei, nach deinem Hauptcode. (Später legt man jede Klasse in eine eigene Datei.)

## Was ist static?
Methoden ohne `static` gehören zu einem **Objekt** (jeder Hund bellt mit seinem Namen). `static` gehört zur Klasse selbst – deshalb war `Main` immer `static`.
''',
  example=r'''Hund bello = new Hund();
bello.Name = "Bello";
bello.Alter = 3;

Hund luna = new Hund();
luna.Name = "Luna";
luna.Alter = 7;

bello.Bellen();
luna.Bellen();
Console.WriteLine($"{luna.Name} ist {luna.Alter - bello.Alter} Jahre älter.");

class Hund
{
    public string Name;
    public int Alter;

    public void Bellen()
    {
        Console.WriteLine(Name + " sagt: Wuff!");
    }
}
''',
  note="Klassennamen schreibt man groß (Hund), Objektvariablen meist klein (bello).",
  exercises=[
    A("Auto-Klasse", r'''Ergänze die Klasse `Auto` um die Felder `public string Marke;` und `public int PS;` sowie eine Methode `public void Info()`, die z. B. `VW mit 150 PS` ausgibt. Der Hauptcode ist fertig:

```
VW mit 150 PS
Tesla mit 400 PS
```''',
      starter='Auto a1 = new Auto();\na1.Marke = "VW";\na1.PS = 150;\nAuto a2 = new Auto();\na2.Marke = "Tesla";\na2.PS = 400;\na1.Info();\na2.Info();\n\nclass Auto\n{\n    // Felder und Methode hier\n}\n',
      solution='Auto a1 = new Auto();\na1.Marke = "VW";\na1.PS = 150;\nAuto a2 = new Auto();\na2.Marke = "Tesla";\na2.PS = 400;\na1.Info();\na2.Info();\n\nclass Auto\n{\n    public string Marke;\n    public int PS;\n\n    public void Info()\n    {\n        Console.WriteLine($"{Marke} mit {PS} PS");\n    }\n}\n',
      tests=[T("VW mit 150 PS\nTesla mit 400 PS")]),
    A("Rechteck-Objekt", r'''Schreib eine Klasse `Rechteck` mit den Feldern `Breite` und `Hoehe` (double) und einer Methode `public double Flaeche()`, die die Fläche **zurückgibt**. Erzeuge ein Rechteck 4 × 2,5 und gib aus:

```
Fläche: 10
```''',
      starter='\n\nclass Rechteck\n{\n}\n',
      solution='Rechteck r = new Rechteck();\nr.Breite = 4;\nr.Hoehe = 2.5;\nConsole.WriteLine("Fläche: " + r.Flaeche());\n\nclass Rechteck\n{\n    public double Breite;\n    public double Hoehe;\n\n    public double Flaeche()\n    {\n        return Breite * Hoehe;\n    }\n}\n',
      tests=[T("Fläche: 10")],
      requirements=[(r'new\s+Rechteck\s*\(', "Erzeuge ein Objekt mit new Rechteck().")]),
    A("Zähler-Objekt", r'''Schreib eine Klasse `Zaehler` mit einem Feld `public int Wert;` und den Methoden `Erhoehen()` (Wert + 1) und `Zuruecksetzen()` (Wert = 0). Teste sie so:

```
Zaehler z = new Zaehler();
z.Erhoehen(); z.Erhoehen(); z.Erhoehen();
Console.WriteLine(z.Wert);
z.Zuruecksetzen();
z.Erhoehen();
Console.WriteLine(z.Wert);
```
Ausgabe:
```
3
1
```''',
      starter='Zaehler z = new Zaehler();\nz.Erhoehen(); z.Erhoehen(); z.Erhoehen();\nConsole.WriteLine(z.Wert);\nz.Zuruecksetzen();\nz.Erhoehen();\nConsole.WriteLine(z.Wert);\n\nclass Zaehler\n{\n    public int Wert;\n}\n',
      solution='Zaehler z = new Zaehler();\nz.Erhoehen(); z.Erhoehen(); z.Erhoehen();\nConsole.WriteLine(z.Wert);\nz.Zuruecksetzen();\nz.Erhoehen();\nConsole.WriteLine(z.Wert);\n\nclass Zaehler\n{\n    public int Wert;\n\n    public void Erhoehen()\n    {\n        Wert++;\n    }\n\n    public void Zuruecksetzen()\n    {\n        Wert = 0;\n    }\n}\n',
      tests=[T("3\n1")]),
  ],
  quiz=[
    Q("Was ist eine Klasse?", ["Ein Objekt", "Ein Bauplan für Objekte", "Eine Variable", "Eine Schleife"], 1, ""),
    Q("Womit erzeugst du ein Objekt?", ["create", "new", "class", "make"], 1, ""),
    Q("Wie rufst du die Methode Starten des Objekts auto auf?", ["Starten(auto);", "auto.Starten();", "auto->Starten();", "Auto.Starten;"], 1, ""),
  ])

L(4, "l22", "Konstruktoren",
  intro="Ein Konstruktor sorgt dafür, dass ein Objekt gleich beim Erzeugen mit new sinnvolle Werte bekommt.",
  explanation=r'''
## Der Konstruktor
```
class Person
{
    public string Name;
    public int Alter;

    public Person(string name, int alter)
    {
        Name = name;
        Alter = alter;
    }
}
```
- Er heißt **genauso wie die Klasse** und hat **keinen** Rückgabetyp (auch kein `void`).
- Er läuft automatisch bei `new`.

## Benutzen
```
Person p = new Person("Ana", 30);
```
Jetzt geht kein Objekt mehr „ohne Namen“ – du musst die Werte gleich mitgeben.

## this
Heißen Parameter und Feld gleich, unterscheidest du sie mit `this`:
```
public Person(string Name) { this.Name = Name; }
```
`this` bedeutet „dieses Objekt hier“.

## Mehrere Konstruktoren
Eine Klasse darf mehrere Konstruktoren mit unterschiedlichen Parametern haben (**Überladung**).
''',
  example=r'''Buch b1 = new Buch("Der Hobbit", "Tolkien", 310);
Buch b2 = new Buch("Unbekanntes Buch");

b1.Beschreibung();
b2.Beschreibung();

class Buch
{
    public string Titel;
    public string Autor;
    public int Seiten;

    public Buch(string titel, string autor, int seiten)
    {
        Titel = titel;
        Autor = autor;
        Seiten = seiten;
    }

    public Buch(string titel)
    {
        Titel = titel;
        Autor = "unbekannt";
        Seiten = 0;
    }

    public void Beschreibung()
    {
        Console.WriteLine($"„{Titel}“ von {Autor}, {Seiten} Seiten");
    }
}
''',
  note="Schreibst du einen eigenen Konstruktor mit Parametern, gibt es den leeren new Klasse() nicht mehr automatisch.",
  exercises=[
    A("Schüler mit Konstruktor", r'''Ergänze in der Klasse `Schueler` einen Konstruktor, der `Name` und `Klasse` setzt. Ausgabe:

```
Mia geht in die 7b
Tim geht in die 9a
```''',
      starter='Schueler s1 = new Schueler("Mia", "7b");\nSchueler s2 = new Schueler("Tim", "9a");\ns1.Vorstellen();\ns2.Vorstellen();\n\nclass Schueler\n{\n    public string Name;\n    public string Klasse;\n\n    // Konstruktor hier\n\n    public void Vorstellen()\n    {\n        Console.WriteLine($"{Name} geht in die {Klasse}");\n    }\n}\n',
      solution='Schueler s1 = new Schueler("Mia", "7b");\nSchueler s2 = new Schueler("Tim", "9a");\ns1.Vorstellen();\ns2.Vorstellen();\n\nclass Schueler\n{\n    public string Name;\n    public string Klasse;\n\n    public Schueler(string name, string klasse)\n    {\n        Name = name;\n        Klasse = klasse;\n    }\n\n    public void Vorstellen()\n    {\n        Console.WriteLine($"{Name} geht in die {Klasse}");\n    }\n}\n',
      tests=[T("Mia geht in die 7b\nTim geht in die 9a")]),
    A("Bankkonto", r'''Schreib eine Klasse `Konto` mit Feldern `Inhaber` (string) und `Kontostand` (double), einem Konstruktor `Konto(string inhaber, double startbetrag)` und den Methoden `Einzahlen(double betrag)` und `Abheben(double betrag)`. Beim Abheben: Reicht das Geld nicht, gib `Nicht genug Geld!` aus und ändere nichts.

```
Konto k = new Konto("Lea", 100);
k.Einzahlen(50);
k.Abheben(30);
k.Abheben(500);
Console.WriteLine($"{k.Inhaber}: {k.Kontostand:F2} Euro");
```
Ausgabe:
```
Nicht genug Geld!
Lea: 120,00 Euro
```''',
      starter='Konto k = new Konto("Lea", 100);\nk.Einzahlen(50);\nk.Abheben(30);\nk.Abheben(500);\nConsole.WriteLine($"{k.Inhaber}: {k.Kontostand:F2} Euro");\n\nclass Konto\n{\n}\n',
      solution='Konto k = new Konto("Lea", 100);\nk.Einzahlen(50);\nk.Abheben(30);\nk.Abheben(500);\nConsole.WriteLine($"{k.Inhaber}: {k.Kontostand:F2} Euro");\n\nclass Konto\n{\n    public string Inhaber;\n    public double Kontostand;\n\n    public Konto(string inhaber, double startbetrag)\n    {\n        Inhaber = inhaber;\n        Kontostand = startbetrag;\n    }\n\n    public void Einzahlen(double betrag)\n    {\n        Kontostand += betrag;\n    }\n\n    public void Abheben(double betrag)\n    {\n        if (betrag > Kontostand)\n        {\n            Console.WriteLine("Nicht genug Geld!");\n            return;\n        }\n        Kontostand -= betrag;\n    }\n}\n',
      tests=[T("Nicht genug Geld!\nLea: 120,00 Euro")]),
    A("Punkt mit this", r'''Schreib eine Klasse `Punkt` mit Feldern `X` und `Y` (int), einem Konstruktor mit Parametern `X` und `Y` (gleiche Namen – benutze `this`!) und einer Methode `public string AlsText()`, die `(3|4)` zurückgibt. Gib aus:

```
(3|4)
(-1|7)
```''',
      starter='Console.WriteLine(new Punkt(3, 4).AlsText());\nConsole.WriteLine(new Punkt(-1, 7).AlsText());\n\n',
      solution='Console.WriteLine(new Punkt(3, 4).AlsText());\nConsole.WriteLine(new Punkt(-1, 7).AlsText());\n\nclass Punkt\n{\n    public int X;\n    public int Y;\n\n    public Punkt(int X, int Y)\n    {\n        this.X = X;\n        this.Y = Y;\n    }\n\n    public string AlsText()\n    {\n        return $"({X}|{Y})";\n    }\n}\n',
      tests=[T("(3|4)\n(-1|7)")],
      requirements=[(r'\bthis\s*\.', "Benutze this, um Feld und Parameter zu unterscheiden.")]),
  ],
  quiz=[
    Q("Wie heißt ein Konstruktor?", ["Immer Konstruktor", "Wie die Klasse", "Init", "Main"], 1, ""),
    Q("Welchen Rückgabetyp hat ein Konstruktor?", ["void", "Den der Klasse", "Gar keinen", "int"], 2, ""),
    Q("Wann wird der Konstruktor ausgeführt?", ["Beim Programmstart", "Bei new", "Bei jedem Methodenaufruf", "Nie automatisch"], 1, ""),
  ])

L(4, "l23", "Eigenschaften & Kapselung",
  intro="Mit private und Eigenschaften (Properties) schützt du die Daten eines Objekts vor falschen Werten.",
  explanation=r'''
## private
Ein `private` Feld kann nur **innerhalb** der Klasse benutzt werden. Von außen kommt niemand direkt dran. Das nennt man **Kapselung**.

## Eigenschaften (Properties)
Eine Eigenschaft sieht von außen aus wie ein Feld, kann aber prüfen, was hineinkommt:
```
class Spieler
{
    private int leben;

    public int Leben
    {
        get { return leben; }
        set
        {
            if (value < 0) leben = 0;
            else leben = value;
        }
    }
}
```
- `get` – wird beim **Lesen** ausgeführt.
- `set` – beim **Schreiben**. Der neue Wert heißt `value`.

## Auto-Eigenschaften
Wenn nichts geprüft werden muss, gibt es die Kurzform:
```
public string Name { get; set; }
public int Punkte { get; private set; }   // nur die Klasse darf schreiben
```
In C# benutzt man für öffentliche Daten fast immer Eigenschaften statt öffentlicher Felder.
''',
  example=r'''Spieler s = new Spieler("Kai");
s.Leben = 50;
Console.WriteLine($"{s.Name}: {s.Leben} Leben");
s.Leben = -20;
Console.WriteLine($"{s.Name}: {s.Leben} Leben");
s.PunkteSammeln(10);
s.PunkteSammeln(5);
Console.WriteLine("Punkte: " + s.Punkte);

class Spieler
{
    private int leben = 100;

    public string Name { get; set; }
    public int Punkte { get; private set; }

    public int Leben
    {
        get { return leben; }
        set { leben = value < 0 ? 0 : value; }
    }

    public Spieler(string name)
    {
        Name = name;
    }

    public void PunkteSammeln(int anzahl)
    {
        Punkte += anzahl;
    }
}
''',
  note="Konvention: private Felder klein (leben), öffentliche Eigenschaften groß (Leben).",
  exercises=[
    A("Temperatur mit Grenze", r'''Ergänze die Eigenschaft `Celsius`: Werte unter -273 sollen auf -273 gesetzt werden (kälter geht nicht). Ergänze außerdem eine Eigenschaft `Fahrenheit` nur mit `get`, die `Celsius * 1.8 + 32` liefert.

```
20 °C = 68,0 °F
-273 °C = -459,4 °F
```''',
      starter='Thermometer t = new Thermometer();\nt.Celsius = 20;\nConsole.WriteLine($"{t.Celsius} °C = {t.Fahrenheit:F1} °F");\nt.Celsius = -500;\nConsole.WriteLine($"{t.Celsius} °C = {t.Fahrenheit:F1} °F");\n\nclass Thermometer\n{\n    private double celsius;\n\n    public double Celsius\n    {\n        get { return celsius; }\n        set { celsius = value; }\n    }\n}\n',
      solution='Thermometer t = new Thermometer();\nt.Celsius = 20;\nConsole.WriteLine($"{t.Celsius} °C = {t.Fahrenheit:F1} °F");\nt.Celsius = -500;\nConsole.WriteLine($"{t.Celsius} °C = {t.Fahrenheit:F1} °F");\n\nclass Thermometer\n{\n    private double celsius;\n\n    public double Celsius\n    {\n        get { return celsius; }\n        set\n        {\n            if (value < -273) celsius = -273;\n            else celsius = value;\n        }\n    }\n\n    public double Fahrenheit\n    {\n        get { return celsius * 1.8 + 32; }\n    }\n}\n',
      tests=[T("20 °C = 68,0 °F\n-273 °C = -459,4 °F")],
      hint="-273 * 1.8 + 32 ergibt -459,4. :F1 rundet auf eine Nachkommastelle."),
    A("Produkt mit Auto-Eigenschaften", r'''Schreib eine Klasse `Produkt` mit den Auto-Eigenschaften `Name` (string) und `Preis` (double) sowie einem Konstruktor. Ergänze die Methode `public double PreisMitRabatt(int prozent)`. Gib aus:

```
Kopfhörer: 80,00 Euro, mit 25 % Rabatt: 60,00 Euro
```''',
      starter='Produkt p = new Produkt("Kopfhörer", 80);\nConsole.WriteLine($"{p.Name}: {p.Preis:F2} Euro, mit 25 % Rabatt: {p.PreisMitRabatt(25):F2} Euro");\n\n',
      solution='Produkt p = new Produkt("Kopfhörer", 80);\nConsole.WriteLine($"{p.Name}: {p.Preis:F2} Euro, mit 25 % Rabatt: {p.PreisMitRabatt(25):F2} Euro");\n\nclass Produkt\n{\n    public string Name { get; set; }\n    public double Preis { get; set; }\n\n    public Produkt(string name, double preis)\n    {\n        Name = name;\n        Preis = preis;\n    }\n\n    public double PreisMitRabatt(int prozent)\n    {\n        return Preis * (100 - prozent) / 100;\n    }\n}\n',
      tests=[T("Kopfhörer: 80,00 Euro, mit 25 % Rabatt: 60,00 Euro")],
      requirements=[(r'\{\s*get\s*;\s*set\s*;\s*\}', "Benutze Auto-Eigenschaften { get; set; }.")]),
    A("Geschütztes Konto", r'''Die Eigenschaft `Kontostand` hat einen `private set` – von außen darf man sie nur lesen. Ergänze `Einzahlen`, das nur positive Beträge annimmt (sonst `Ungültiger Betrag`).

```
Ungültiger Betrag
Kontostand: 75
```''',
      starter='Sparschwein s = new Sparschwein();\ns.Einzahlen(50);\ns.Einzahlen(-10);\ns.Einzahlen(25);\nConsole.WriteLine("Kontostand: " + s.Kontostand);\n\nclass Sparschwein\n{\n    public int Kontostand { get; private set; }\n\n}\n',
      solution='Sparschwein s = new Sparschwein();\ns.Einzahlen(50);\ns.Einzahlen(-10);\ns.Einzahlen(25);\nConsole.WriteLine("Kontostand: " + s.Kontostand);\n\nclass Sparschwein\n{\n    public int Kontostand { get; private set; }\n\n    public void Einzahlen(int betrag)\n    {\n        if (betrag <= 0)\n        {\n            Console.WriteLine("Ungültiger Betrag");\n            return;\n        }\n        Kontostand += betrag;\n    }\n}\n',
      tests=[T("Ungültiger Betrag\nKontostand: 75")]),
  ],
  quiz=[
    Q("Was bedeutet private?", ["Nur innerhalb der Klasse sichtbar", "Überall sichtbar", "Konstant", "Geheim verschlüsselt"], 0, ""),
    Q("Wie heißt der neue Wert im set-Teil einer Eigenschaft?", ["neu", "wert", "value", "this"], 2, ""),
    Q("Was ist public int X { get; private set; }?", ["Von außen lesbar, aber nur in der Klasse änderbar", "Gar nicht lesbar", "Überall änderbar", "Ein Fehler"], 0, ""),
  ])

L(4, "l24", "Listen von Objekten",
  intro="Richtig stark wird es, wenn du viele Objekte in einer Liste verwaltest – z. B. alle Schüler einer Klasse.",
  explanation=r'''
## Liste mit eigenen Objekten
```
List<Schueler> klasse = new List<Schueler>();
klasse.Add(new Schueler("Ana", 2));
klasse.Add(new Schueler("Ben", 1));
```

## Durchlaufen und auswerten
```
foreach (Schueler s in klasse)
{
    Console.WriteLine($"{s.Name}: Note {s.Note}");
}
```

## Suchen und Filtern
Mit Schleife und `if` findest du z. B. den Besten:
```
Schueler bester = klasse[0];
foreach (Schueler s in klasse)
    if (s.Note < bester.Note) bester = s;
```

## Kleiner Ausblick: LINQ
C# hat dafür auch fertige Werkzeuge:
- `klasse.Where(s => s.Note <= 2)` – filtern
- `klasse.OrderBy(s => s.Name)` – sortieren
- `klasse.Average(s => s.Note)` – Durchschnitt

Das `s => ...` heißt **Lambda**: „für jedes s nimm …“.
''',
  example=r'''List<Film> filme = new List<Film>
{
    new Film("Matrix", 1999, 8.7),
    new Film("Up", 2009, 8.3),
    new Film("Dune", 2021, 8.0),
};

foreach (Film f in filme)
    Console.WriteLine(f);

Film bester = filme[0];
foreach (Film f in filme)
    if (f.Bewertung > bester.Bewertung) bester = f;
Console.WriteLine("Bester Film: " + bester.Titel);

Console.WriteLine("Schnitt: " + filme.Average(f => f.Bewertung).ToString("F2"));

class Film
{
    public string Titel { get; }
    public int Jahr { get; }
    public double Bewertung { get; }

    public Film(string titel, int jahr, double bewertung)
    {
        Titel = titel;
        Jahr = jahr;
        Bewertung = bewertung;
    }

    public override string ToString() => $"{Titel} ({Jahr}) – {Bewertung}";
}
''',
  note="Mit public override string ToString() legst du fest, wie ein Objekt aussieht, wenn du es mit Console.WriteLine ausgibst.",
  exercises=[
    A("Klassenliste", r'''Füge der Liste drei Schüler hinzu: `Ana` (Note 2), `Ben` (Note 1), `Cem` (Note 3). Gib alle aus und danach den Notendurchschnitt mit 2 Nachkommastellen:

```
Ana: 2
Ben: 1
Cem: 3
Durchschnitt: 2,00
```''',
      starter='List<Schueler> klasse = new List<Schueler>();\n\n\nclass Schueler\n{\n    public string Name { get; }\n    public int Note { get; }\n\n    public Schueler(string name, int note)\n    {\n        Name = name;\n        Note = note;\n    }\n}\n',
      solution='List<Schueler> klasse = new List<Schueler>();\nklasse.Add(new Schueler("Ana", 2));\nklasse.Add(new Schueler("Ben", 1));\nklasse.Add(new Schueler("Cem", 3));\n\ndouble summe = 0;\nforeach (Schueler s in klasse)\n{\n    Console.WriteLine($"{s.Name}: {s.Note}");\n    summe += s.Note;\n}\nConsole.WriteLine($"Durchschnitt: {summe / klasse.Count:F2}");\n\nclass Schueler\n{\n    public string Name { get; }\n    public int Note { get; }\n\n    public Schueler(string name, int note)\n    {\n        Name = name;\n        Note = note;\n    }\n}\n',
      tests=[T("Ana: 2\nBen: 1\nCem: 3\nDurchschnitt: 2,00")]),
    A("Teuerstes Produkt", r'''Finde in der Liste das teuerste Produkt (mit einer Schleife) und gib aus:

```
Teuerstes Produkt: Monitor (199,99 Euro)
```''',
      starter='List<Produkt> produkte = new List<Produkt>\n{\n    new Produkt("Maus", 19.99),\n    new Produkt("Monitor", 199.99),\n    new Produkt("Tastatur", 49.5),\n};\n\n\nclass Produkt\n{\n    public string Name { get; }\n    public double Preis { get; }\n    public Produkt(string name, double preis) { Name = name; Preis = preis; }\n}\n',
      solution='List<Produkt> produkte = new List<Produkt>\n{\n    new Produkt("Maus", 19.99),\n    new Produkt("Monitor", 199.99),\n    new Produkt("Tastatur", 49.5),\n};\n\nProdukt teuerstes = produkte[0];\nforeach (Produkt p in produkte)\n{\n    if (p.Preis > teuerstes.Preis)\n        teuerstes = p;\n}\nConsole.WriteLine($"Teuerstes Produkt: {teuerstes.Name} ({teuerstes.Preis:F2} Euro)");\n\nclass Produkt\n{\n    public string Name { get; }\n    public double Preis { get; }\n    public Produkt(string name, double preis) { Name = name; Preis = preis; }\n}\n',
      tests=[T("Teuerstes Produkt: Monitor (199,99 Euro)")]),
    A("Bestanden-Filter", r'''Lies erst die Anzahl der Teilnehmer, dann für jeden Namen und Punktzahl ein. Gib nur die aus, die **mindestens 50** Punkte haben, und am Ende die Anzahl. Bei `3`, `Ana`, `70`, `Ben`, `40`, `Cem`, `50`:

```
Bestanden: Ana (70)
Bestanden: Cem (50)
2 von 3 haben bestanden.
```''',
      starter='List<Teilnehmer> liste = new List<Teilnehmer>();\n\n\nclass Teilnehmer\n{\n    public string Name { get; set; }\n    public int Punkte { get; set; }\n}\n',
      solution='List<Teilnehmer> liste = new List<Teilnehmer>();\nint anzahl = int.Parse(Console.ReadLine());\nfor (int i = 0; i < anzahl; i++)\n{\n    Teilnehmer t = new Teilnehmer();\n    t.Name = Console.ReadLine();\n    t.Punkte = int.Parse(Console.ReadLine());\n    liste.Add(t);\n}\n\nint bestanden = 0;\nforeach (Teilnehmer t in liste)\n{\n    if (t.Punkte >= 50)\n    {\n        Console.WriteLine($"Bestanden: {t.Name} ({t.Punkte})");\n        bestanden++;\n    }\n}\nConsole.WriteLine($"{bestanden} von {liste.Count} haben bestanden.");\n\nclass Teilnehmer\n{\n    public string Name { get; set; }\n    public int Punkte { get; set; }\n}\n',
      tests=[T("Bestanden: Ana (70)\nBestanden: Cem (50)\n2 von 3 haben bestanden.", "3", "Ana", "70", "Ben", "40", "Cem", "50"), T("0 von 1 haben bestanden.", "1", "Zoe", "10")], compare="lenient"),
  ],
  quiz=[
    Q("Wie legst du eine Liste für Objekte der Klasse Auto an?", ["List<Auto> autos = new List<Auto>();", "Auto[] autos = new List();", "List autos = Auto;", "new Auto<List>();"], 0, ""),
    Q("Was macht ToString() bei Console.WriteLine(objekt)?", ["Nichts", "Bestimmt, welcher Text ausgegeben wird", "Löscht das Objekt", "Wandelt in int um"], 1, ""),
    Q("Was bedeutet s => s.Note in LINQ?", ["s ist größer als Note", "Für jedes s nimm s.Note", "Ein Vergleich", "Ein Kommentar"], 1, ""),
  ])
