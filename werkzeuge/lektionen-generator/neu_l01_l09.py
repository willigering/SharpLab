"""Zusätzliche Aufgaben Lektion 1–9 (bis 8 pro Lektion)."""
from neu import N, T, E

# ---------------- l01 Dein erstes Programm ----------------
N("l01", "Countdown in einer Zeile", """
Gib den Countdown in **einer** Zeile aus – mit vier getrennten Ausgabe-Befehlen, einem für jedes Stück:

```
3... 2... 1... Start!
```
""", r'''
Console.Write("3... ");
Console.Write("2... ");
Console.Write("1... ");
Console.WriteLine("Start!");
''', [T("3... 2... 1... Start!")],
  "Die Lektion zeigt dir einen Befehl, nach dem die Ausgabe in derselben Zeile weitergeht. Achte auf die Leerzeichen am Ende der Stücke.",
  anf=[(r"(Console\s*\.\s*Write\s*\([\s\S]*){3}", "Teil den Countdown auf mehrere Befehle auf, die in derselben Zeile bleiben.")])

N("l01", "Hallo und Willkommen", """
Gib diese zwei Zeilen aus. Die erste Zeile setzt du aus **zwei** Ausgabe-Befehlen zusammen: einer für `Hallo, ` und einer für `Lena!`.

```
Hallo, Lena!
Schön, dass du da bist.
```
""", r'''
Console.Write("Hallo, ");
Console.WriteLine("Lena!");
Console.WriteLine("Schön, dass du da bist.");
''', [T("Hallo, Lena!\nSchön, dass du da bist.")],
  "Nach dem ersten Stück darf noch keine neue Zeile beginnen. Die neue Zeile kommt erst nach dem Namen.",
  anf=[(r"Console\s*\.\s*Write\s*\(", "Die erste Zeile soll aus zwei Befehlen bestehen – der erste darf noch keine neue Zeile anfangen.")])

N("l01", "Sternen-Dreieck", """
Gib dieses Dreieck aus Sternen aus:

```
*
**
***
****
```
""", r'''
Console.WriteLine("*");
Console.WriteLine("**");
Console.WriteLine("***");
Console.WriteLine("****");
''', [T("*\n**\n***\n****")],
  "Jede Zeile des Dreiecks ist ein eigener Ausgabe-Befehl. Zähl die Sterne pro Zeile genau.")

N("l01", "Visitenkarte", """
Gib diese kleine Visitenkarte aus:

```
==================
  Max Muster
  Schüler, 8b
  Lieblingsfach: IT
==================
```
""", r'''
Console.WriteLine("==================");
Console.WriteLine("  Max Muster");
Console.WriteLine("  Schüler, 8b");
Console.WriteLine("  Lieblingsfach: IT");
Console.WriteLine("==================");
''', [T("==================\n  Max Muster\n  Schüler, 8b\n  Lieblingsfach: IT\n==================")],
  "Auch Leerzeichen am Zeilenanfang gehören in die Anführungszeichen. Die Linien oben und unten sind gleich lang.")

N("l01", "Satz aus Bausteinen", """
Bau den Satz aus **vier** Ausgabe-Befehlen zusammen. Die ersten drei bleiben in derselben Zeile, der vierte beendet sie. Danach folgt eine zweite Zeile:

```
Ich lerne C#, Schritt für Schritt.
Und es macht Spaß!
```
""", r'''
Console.Write("Ich ");
Console.Write("lerne ");
Console.Write("C#, ");
Console.WriteLine("Schritt für Schritt.");
Console.WriteLine("Und es macht Spaß!");
''', [T("Ich lerne C#, Schritt für Schritt.\nUnd es macht Spaß!")],
  "Drei Befehle bleiben in der Zeile, erst der vierte beginnt danach eine neue. Vergiss die Leerzeichen zwischen den Wörtern nicht.",
  anf=[(r"(Console\s*\.\s*Write\s*\([\s\S]*){3}", "Die erste Zeile soll aus mehreren Befehlen bestehen, die in derselben Zeile bleiben.")])

# ---------------- l02 Kommentare ----------------
KOMMENTAR = r"//(?!\s*Schreib hier deinen Code)\s*\S"

N("l02", "Kommentar am Zeilenende", """
Gib `Programm startet` aus. Schreib in **dieselbe Zeile**, hinter den Befehl, einen kurzen Kommentar, der erklärt, was die Zeile macht.

```
Programm startet
```
""", r'''
Console.WriteLine("Programm startet"); // zeigt den Start an
''', [T("Programm startet")],
  "Ein Kommentar darf auch hinter Code stehen. Alles ab dem Kommentarzeichen bis zum Zeilenende ignoriert C#.",
  anf=[(r";[ \t]*//", "Der Kommentar soll in derselben Zeile hinter dem Befehl stehen.", True)])

N("l02", "Notiz über drei Zeilen", """
Gib `Hausaufgaben erledigt!` aus. Schreib darüber eine Notiz, die über **drei Zeilen** geht – als **ein einziger** Kommentar, nicht drei einzelne.

```
Hausaufgaben erledigt!
```
""", r'''
/* Mein Programm
   Datum: heute
   Es zeigt eine Nachricht an */
Console.WriteLine("Hausaufgaben erledigt!");
''', [T("Hausaufgaben erledigt!")],
  "Für längere Notizen gibt es eine eigene Kommentar-Art mit Anfangs- und Endzeichen. Alles dazwischen wird ignoriert, auch über mehrere Zeilen.",
  anf=[(r"/\*[^\n]*\n[^\n]*\n[\s\S]*?\*/", "Die Notiz soll ein einziger Kommentar über drei Zeilen sein.", True)])

N("l02", "Kopf mit Notizen", """
Schreib ein Programm, das diese zwei Zeilen ausgibt. Ganz oben steht ein Kommentar über mehrere Zeilen, der das Programm beschreibt. Hinter **jeder** Ausgabezeile steht zusätzlich ein kurzer Kommentar.

```
Mein Programm
Version 1
```
""", r'''
/* Das ist mein Programm.
   Es zeigt Name und Version an. */
Console.WriteLine("Mein Programm"); // Name
Console.WriteLine("Version 1");     // Version
''', [T("Mein Programm\nVersion 1")],
  "Du brauchst beide Kommentar-Arten aus der Lektion. Hinter den Befehlen reicht jeweils ein Wort.",
  anf=[(r"/\*[\s\S]*?\*/", "Oben fehlt noch der Kommentar über mehrere Zeilen.", True),
       (r";[ \t]*//[^\n]*\n[\s\S]*;[ \t]*//", "Hinter jeder Ausgabezeile soll ein Kommentar stehen.", True)])

N("l02", "Beide Kommentar-Arten", """
Gib die drei Zeilen aus. Erkläre den Code mit genau einem einzeiligen Kommentar und genau einem Kommentar, der über mehrere Zeilen gehen darf – wo genau, entscheidest du.

```
Rot
Gelb
Grün
```
""", r'''
// Die Ampelfarben
Console.WriteLine("Rot");
/* erst Gelb,
   dann Grün */
Console.WriteLine("Gelb");
Console.WriteLine("Grün");
''', [T("Rot\nGelb\nGrün")],
  "Ein Kommentar beginnt mit zwei Zeichen und gilt bis zum Zeilenende. Der andere hat einen Anfang und ein Ende.",
  anf=[(KOMMENTAR, "Es fehlt noch ein einzeiliger Kommentar.", True),
       (r"/\*[\s\S]*?\*/", "Es fehlt noch ein Kommentar mit Anfang und Ende.", True)])

N("l02", "Ausgeblendete Zeile", """
Schreib ein Programm mit drei Ausgabe-Befehlen für `Eins`, `Zwei` und `Drei`. Der Befehl für `Zwei` soll im Code stehen bleiben, aber nicht ausgeführt werden. Ausgabe:

```
Eins
Drei
```
""", r'''
Console.WriteLine("Eins");
// Console.WriteLine("Zwei");
Console.WriteLine("Drei");
''', [T("Eins\nDrei")],
  "Code, der in einem Kommentar steht, wird nicht ausgeführt. So kannst du eine Zeile ausschalten, ohne sie zu löschen.",
  anf=[(r"Zwei", "Der Befehl für Zwei soll im Code stehen bleiben – nur ausgeschaltet.", True)])

# ---------------- l03 Variablen ----------------
N("l03", "Lieblingszahl und Buchstabe", """
Lege die Variable `lieblingszahl` mit dem Wert `7` und die Variable `anfangsbuchstabe` mit dem Zeichen `L` an. Gib beide über die Variablen aus:

```
Lieblingszahl: 7
Anfangsbuchstabe: L
```
""", r'''
int lieblingszahl = 7;
char anfangsbuchstabe = 'L';
Console.WriteLine("Lieblingszahl: " + lieblingszahl);
Console.WriteLine("Anfangsbuchstabe: " + anfangsbuchstabe);
''', [T("Lieblingszahl: 7\nAnfangsbuchstabe: L")],
  "Für ein einzelnes Zeichen gibt es einen eigenen Datentyp. Sein Wert steht in einfachen Anführungszeichen.",
  anf=[(r"int\s+lieblingszahl\s*=", "Lege die Variable lieblingszahl als ganze Zahl an."),
       (r"char\s+anfangsbuchstabe\s*=", "Lege anfangsbuchstabe mit dem Datentyp für ein einzelnes Zeichen an."),
       (r"\+\s*lieblingszahl", "Die 7 soll aus der Variablen kommen."),
       (r"\+\s*anfangsbuchstabe", "Das L soll aus der Variablen kommen.")])

N("l03", "Preisschild", """
Lege `produkt` mit dem Wert `"Brot"` und `preis` mit dem Wert `2.49` an. Gib mit beiden Variablen aus:

```
Brot kostet 2,49 Euro.
```
""", r'''
string produkt = "Brot";
double preis = 2.49;
Console.WriteLine(produkt + " kostet " + preis + " Euro.");
''', [T("Brot kostet 2,49 Euro.")],
  "Für Kommazahlen brauchst du einen anderen Datentyp als für ganze Zahlen. Im Code schreibst du sie mit Punkt, ausgegeben werden sie mit Komma.",
  anf=[(r"string\s+produkt\s*=", "Lege produkt als Text-Variable an."),
       (r"double\s+preis\s*=", "Lege preis mit dem Datentyp für Kommazahlen an."),
       (r"produkt\s*\+", "Brot soll aus der Variablen produkt kommen."),
       (r"\+\s*preis", "Der Preis soll aus der Variablen preis kommen.")])

N("l03", "Hunger?", """
Lege `hatHunger` mit dem Wert `false` an und gib ihn aus. Ändere den Wert danach auf `true` und gib ihn noch einmal aus:

```
Hunger: False
Hunger: True
```
""", r'''
bool hatHunger = false;
Console.WriteLine("Hunger: " + hatHunger);
hatHunger = true;
Console.WriteLine("Hunger: " + hatHunger);
''', [T("Hunger: False\nHunger: True")],
  "Für wahr und falsch gibt es einen eigenen Datentyp. Beim zweiten Mal änderst du nur den Wert, ohne den Typ noch einmal davorzuschreiben.",
  anf=[(r"bool\s+hatHunger\s*=\s*false", "Lege hatHunger als Wahrheitswert mit false an."),
       (r"hatHunger\s*=\s*true", "Ändere den Wert von hatHunger später auf true."),
       (r"\+\s*hatHunger", "Der Wert soll aus der Variablen kommen.")])

N("l03", "Werte tauschen", """
Lege `a` mit dem Wert `3` und `b` mit dem Wert `8` an. Tausche danach die Werte der beiden Variablen, ohne die Zahlen noch einmal einzutippen, und gib sie aus:

```
a = 8
b = 3
```
""", r'''
int a = 3;
int b = 8;
int merker = a;
a = b;
b = merker;
Console.WriteLine("a = " + a);
Console.WriteLine("b = " + b);
''', [T("a = 8\nb = 3")],
  "Wenn du a sofort überschreibst, ist die 3 weg. Merk sie dir vorher woanders.",
  anf=[(r"int\s+a\s*=\s*3\s*;", "Lege a mit dem Wert 3 an."),
       (r"int\s+b\s*=\s*8\s*;", "Lege b mit dem Wert 8 an."),
       (r"int\s+(?!a\b|b\b)\w+\s*=\s*[ab]\s*;", "Merk dir einen der Werte in einer dritten Variablen, bevor du ihn überschreibst."),
       (r"\+\s*a\s*\)", "Gib a über die Variable aus."),
       (r"\+\s*b\s*\)", "Gib b über die Variable aus.")])

N("l03", "Steckbrief mit vier Datentypen", """
Lege diese vier Variablen an und gib damit den Steckbrief aus:
- `name` mit `"Mia"`
- `alter` mit `14`
- `groesse` mit `1.75`
- `schueler` mit `true`

```
Name: Mia
Alter: 14
Größe: 1,75 m
Schüler: True
```
""", r'''
string name = "Mia";
int alter = 14;
double groesse = 1.75;
bool schueler = true;
Console.WriteLine("Name: " + name);
Console.WriteLine("Alter: " + alter);
Console.WriteLine("Größe: " + groesse + " m");
Console.WriteLine("Schüler: " + schueler);
''', [T("Name: Mia\nAlter: 14\nGröße: 1,75 m\nSchüler: True")],
  "Jede Variable braucht den passenden Datentyp aus der Lektion. Die Einheit m hängst du als Text hinten an.",
  anf=[(r"string\s+name\s*=", "Lege name als Text an."),
       (r"int\s+alter\s*=", "Lege alter als ganze Zahl an."),
       (r"double\s+groesse\s*=", "Lege groesse als Kommazahl an."),
       (r"bool\s+schueler\s*=", "Lege schueler als Wahrheitswert an."),
       (r"\+\s*groesse", "Die Größe soll aus der Variablen kommen.")])

# ---------------- l04 Rechnen ----------------
N("l04", "Minuten umrechnen", """
Lege `minuten` mit dem Wert `135` an. Rechne aus, wie viele volle Stunden und übrige Minuten das sind:

```
135 Minuten sind 2 Stunden und 15 Minuten.
```
""", r'''
int minuten = 135;
Console.WriteLine(minuten + " Minuten sind " + minuten / 60 + " Stunden und " + minuten % 60 + " Minuten.");
''', [T("135 Minuten sind 2 Stunden und 15 Minuten.")],
  "Eine Stunde hat 60 Minuten. Für die vollen Stunden und für den Rest gibt es je einen Rechenoperator aus der Lektion.",
  anf=[(r"int\s+minuten\s*=\s*135", "Lege minuten mit dem Wert 135 an."),
       (r"minuten\s*/\s*60", "Die vollen Stunden sollst du ausrechnen lassen."),
       (r"minuten\s*%\s*60", "Die übrigen Minuten sollst du ausrechnen lassen – dafür gibt es einen eigenen Operator.")])

N("l04", "Geschwindigkeit", """
Eine Radtour ist 150 km lang und dauert 2,5 Stunden. Lege `strecke` und `zeit` an und rechne die Geschwindigkeit aus:

```
Geschwindigkeit: 60 km/h
```
""", r'''
double strecke = 150;
double zeit = 2.5;
Console.WriteLine("Geschwindigkeit: " + strecke / zeit + " km/h");
''', [T("Geschwindigkeit: 60 km/h")],
  "Geschwindigkeit ist Strecke geteilt durch Zeit. Weil die Zeit eine Kommazahl ist, brauchst du den passenden Datentyp.",
  anf=[(r"double\s+zeit\s*=", "zeit ist eine Kommazahl – wähle den passenden Datentyp."),
       (r"strecke\s*/\s*zeit", "Lass das Programm strecke durch zeit teilen.")])

N("l04", "Temperatur ändern", """
Lege `temp` mit dem Wert `12.5` an. Erhöhe den Wert um 3,5 und gib ihn aus. Halbiere ihn danach und gib ihn wieder aus. Ändere `temp` beide Male mit der Kurzschreibweise aus der Lektion:

```
16
8
```
""", r'''
double temp = 12.5;
temp += 3.5;
Console.WriteLine(temp);
temp /= 2;
Console.WriteLine(temp);
''', [T("16\n8")],
  "Die Lektion zeigt dir für jede Grundrechenart eine Kurzform, die den Wert der Variablen direkt ändert.",
  anf=[(r"double\s+temp\s*=\s*12\.5", "Lege temp als Kommazahl mit 12.5 an."),
       (r"temp\s*\+=", "Erhöhe temp mit der Kurzschreibweise."),
       (r"temp\s*/=", "Halbiere temp mit der Kurzschreibweise.")])

N("l04", "Bakterien verdoppeln sich", """
In einer Schale leben 50 Bakterien. Jede Stunde verdoppelt sich ihre Zahl. Lege `bakterien` an und verdopple den Wert dreimal mit der Kurzschreibweise. Gib nach jedem Schritt aus:

```
Start: 50
Nach 1 Stunde: 100
Nach 2 Stunden: 200
Nach 3 Stunden: 400
```
""", r'''
int bakterien = 50;
Console.WriteLine("Start: " + bakterien);
bakterien *= 2;
Console.WriteLine("Nach 1 Stunde: " + bakterien);
bakterien *= 2;
Console.WriteLine("Nach 2 Stunden: " + bakterien);
bakterien *= 2;
Console.WriteLine("Nach 3 Stunden: " + bakterien);
''', [T("Start: 50\nNach 1 Stunde: 100\nNach 2 Stunden: 200\nNach 3 Stunden: 400")],
  "Verdoppeln heißt mal 2. Auch dafür gibt es eine Kurzform, die die Variable direkt ändert.",
  anf=[(r"int\s+bakterien\s*=\s*50", "Lege bakterien mit dem Wert 50 an."),
       (r"(bakterien\s*\*=[\s\S]*){3}", "Verdopple bakterien dreimal mit der Kurzschreibweise."),
       (r"\+\s*bakterien", "Die Zahlen sollen aus der Variablen kommen.")])

# ---------------- l05 String-Interpolation ----------------
N("l05", "Kontostand", """
Lege `betrag` mit dem Wert `1234.5` an und gib ihn mit zwei Nachkommastellen aus. Bau die Variable direkt in den Text ein:

```
Kontostand: 1234,50 Euro
```
""", r'''
double betrag = 1234.5;
Console.WriteLine($"Kontostand: {betrag:F2} Euro");
''', [T("Kontostand: 1234,50 Euro")],
  "Die Lektion zeigt dir, wie du Variablen in einen Text einbaust und wie viele Nachkommastellen angezeigt werden.",
  anf=[(r'\$"', "Bau die Variable direkt in den Text ein – so wie in der Lektion."),
       (r"\{\s*betrag", "Der Betrag soll aus der Variablen kommen.")])

N("l05", "Zitat", """
Lege `wort` mit dem Wert `Hallo!` an. Gib den Satz mit Anführungszeichen aus – das Wort kommt aus der Variablen:

```
Er sagte: "Hallo!"
```
""", r'''
string wort = "Hallo!";
Console.WriteLine($"Er sagte: \"{wort}\"");
''', [T('Er sagte: "Hallo!"')],
  "Anführungszeichen mitten im Text sind ein Sonderzeichen. In der Lektion steht, wie du sie schreibst.",
  anf=[(r'\$"', "Bau die Variable direkt in den Text ein."),
       (r"\{\s*wort\s*\}", "Das Wort soll aus der Variablen kommen.")])

N("l05", "Rechenaufgaben", """
Lege `a` mit dem Wert `7` und `b` mit dem Wert `3` an. Gib diese drei Rechnungen aus – Zahlen und Ergebnisse kommen aus dem Programm, das Teilen mit zwei Nachkommastellen:

```
7 + 3 = 10
7 * 3 = 21
7 / 3 = 2,33
```
""", r'''
int a = 7;
int b = 3;
Console.WriteLine($"{a} + {b} = {a + b}");
Console.WriteLine($"{a} * {b} = {a * b}");
Console.WriteLine($"{a} / {b} = {(double)a / b:F2}");
''', [T("7 + 3 = 10\n7 * 3 = 21\n7 / 3 = 2,33")],
  "Zwischen den geschweiften Klammern darf auch gerechnet werden. Denk beim Teilen an die Falle mit ganzen Zahlen aus Lektion 4.",
  anf=[(r"\{\s*a\s*\+\s*b\s*\}", "Lass die Summe im Text ausrechnen."),
       (r"\{\s*a\s*\*\s*b\s*\}", "Lass das Produkt im Text ausrechnen."),
       (r"\{[^}]*/[^}]*\}", "Lass auch das Teilen im Text ausrechnen.")])

N("l05", "Rechtsbündige Spalte", """
Lege die Variablen `x` mit `7`, `y` mit `42` und `z` mit `1234` an. Gib sie **rechtsbündig** in einer Spalte aus, die 5 Zeichen breit ist:

```
    7
   42
 1234
```
""", r'''
int x = 7;
int y = 42;
int z = 1234;
Console.WriteLine($"{x,5}");
Console.WriteLine($"{y,5}");
Console.WriteLine($"{z,5}");
''', [T("    7\n   42\n 1234")],
  "In der Lektion steht, wie du die Breite eines eingebauten Werts festlegst. Die Leerzeichen davor entstehen dann von allein.",
  anf=[(r"\{\s*x\s*,\s*5\s*\}", "Gib x in einer 5 Zeichen breiten Spalte aus."),
       (r"\{\s*y\s*,\s*5\s*\}", "Gib y in einer 5 Zeichen breiten Spalte aus."),
       (r"\{\s*z\s*,\s*5\s*\}", "Gib z in einer 5 Zeichen breiten Spalte aus.")])

N("l05", "Einkaufszettel mit Summe", """
Lege `preisBrot` mit `2.5` und `preisKaese` mit `3.99` an. Gib beide Preise und die Summe mit zwei Nachkommastellen aus – die Summe rechnet das Programm:

```
Brot: 2,50 Euro
Käse: 3,99 Euro
Summe: 6,49 Euro
```
""", r'''
double preisBrot = 2.5;
double preisKaese = 3.99;
Console.WriteLine($"Brot: {preisBrot:F2} Euro");
Console.WriteLine($"Käse: {preisKaese:F2} Euro");
Console.WriteLine($"Summe: {preisBrot + preisKaese:F2} Euro");
''', [T("Brot: 2,50 Euro\nKäse: 3,99 Euro\nSumme: 6,49 Euro")],
  "Jede Zeile baut einen Wert in den Text ein, die letzte rechnet dabei. Die Nachkommastellen stellst du wie in der Lektion ein.",
  anf=[(r'\$"', "Bau die Preise direkt in den Text ein."),
       (r"\{\s*preisBrot", "Der Brotpreis soll aus der Variablen kommen."),
       (r"\{\s*preisKaese", "Der Käsepreis soll aus der Variablen kommen."),
       (r"preisBrot\s*\+\s*preisKaese|preisKaese\s*\+\s*preisBrot", "Lass die Summe vom Programm ausrechnen.")])

# ---------------- l06 Eingabe ----------------
N("l06", "Wohnort", """
Lies zuerst einen Namen und dann eine Stadt ein und gib einen Satz aus. Bei `Lena` und `Bonn`:

```
Lena wohnt in Bonn.
```
""", r'''
string name = Console.ReadLine();
string stadt = Console.ReadLine();
Console.WriteLine($"{name} wohnt in {stadt}.");
''', [T("Lena wohnt in Bonn.", "Lena", "Bonn"), T("Tom wohnt in Wien.", "Tom", "Wien")],
  "Jede Eingabe bekommt ihre eigene Variable. Danach baust du beide in den Satz ein.", "lenient")

N("l06", "Alter in Tagen", """
Lies ein Alter in Jahren ein und gib aus, wie viele Tage das ungefähr sind (ein Jahr hat hier 365 Tage). Bei `12`:

```
Du bist ungefähr 4380 Tage alt.
```
""", r'''
int alter = int.Parse(Console.ReadLine());
Console.WriteLine($"Du bist ungefähr {alter * 365} Tage alt.");
''', [T("Du bist ungefähr 4380 Tage alt.", "12"), T("Du bist ungefähr 10950 Tage alt.", "30")],
  "Mit Text kann man nicht rechnen. Wandle die Eingabe zuerst in eine Zahl um.", "lenient")

N("l06", "Fläche und Umfang", """
Lies die Breite und dann die Höhe eines Rechtecks ein (ganze Zahlen). Gib Fläche und Umfang aus. Bei `6` und `4`:

```
Fläche: 24
Umfang: 20
```
""", r'''
int breite = int.Parse(Console.ReadLine());
int hoehe = int.Parse(Console.ReadLine());
Console.WriteLine($"Fläche: {breite * hoehe}");
Console.WriteLine($"Umfang: {2 * (breite + hoehe)}");
''', [T("Fläche: 24\nUmfang: 20", "6", "4"), T("Fläche: 9\nUmfang: 12", "3", "3")],
  "Die Fläche ist Breite mal Höhe. Für den Umfang zählst du alle vier Seiten zusammen.", "lenient")

N("l06", "Tankrechner", """
Lies ein, wie viele Liter getankt werden und was ein Liter kostet (beides darf eine Kommazahl sein). Gib die Kosten mit zwei Nachkommastellen aus. Bei `40` und `1,85`:

```
Tanken kostet 74,00 Euro.
```
""", r'''
double liter = double.Parse(Console.ReadLine());
double preis = double.Parse(Console.ReadLine());
Console.WriteLine($"Tanken kostet {liter * preis:F2} Euro.");
''', [T("Tanken kostet 74,00 Euro.", "40", "1,85"), T("Tanken kostet 25,00 Euro.", "12,5", "2")],
  "Kommazahlen wandelst du mit dem passenden Datentyp um. Zwei Nachkommastellen kennst du aus Lektion 5.", "lenient")

# ---------------- l07 Program & Main ----------------
KLASSE = [(r"class\s+Program", "Schreib das Programm im klassischen Aufbau mit der Klasse Program."),
          (r"static\s+void\s+Main\s*\(", "Im klassischen Aufbau fehlt noch die Main-Methode.")]

N("l07", "Zwei Zeilen klassisch", """
Schreib ein Programm im **klassischen Aufbau** (mit Klasse und Main-Methode), das diese zwei Zeilen ausgibt:

```
Guten Tag!
Heute lernen wir C#.
```
""", r'''
using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Guten Tag!");
        Console.WriteLine("Heute lernen wir C#.");
    }
}
''', [T("Guten Tag!\nHeute lernen wir C#.")],
  "Die Lektion zeigt dir den Aufbau Schritt für Schritt. Deine Befehle gehören zwischen die geschweiften Klammern der Main-Methode.",
  anf=KLASSE)

N("l07", "Variable im Main", """
Schreib ein Programm im klassischen Aufbau. In der Main-Methode legst du `jahr` mit dem Wert `2026` an und gibst damit aus:

```
Wir haben 2026.
```
""", r'''
using System;

class Program
{
    static void Main(string[] args)
    {
        int jahr = 2026;
        Console.WriteLine($"Wir haben {jahr}.");
    }
}
''', [T("Wir haben 2026.")],
  "Variablen legst du innerhalb der Main-Methode genauso an wie bisher. Nur der Rahmen drumherum ist neu.",
  anf=KLASSE + [(r"int\s+jahr\s*=", "Lege jahr als ganze Zahl in der Main-Methode an.")])

N("l07", "Eigener Namensraum", """
Schreib ein Programm im klassischen Aufbau, bei dem die Klasse `Program` im Namensraum `Schule` steht:

```
Willkommen in der Schule!
```
""", r'''
using System;

namespace Schule
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Willkommen in der Schule!");
        }
    }
}
''', [T("Willkommen in der Schule!")],
  "Der Namensraum ist die äußerste Klammer um die Klasse. Achte darauf, dass jede geöffnete Klammer wieder geschlossen wird.",
  anf=KLASSE + [(r"namespace\s+Schule", "Die Klasse soll im Namensraum Schule stehen.")])

N("l07", "Begrüßung im Main", """
Schreib ein Programm im klassischen Aufbau, das einen Namen einliest und begrüßt. Bei `Ben`:

```
Schön, dich zu sehen, Ben!
```
""", r'''
using System;

class Program
{
    static void Main(string[] args)
    {
        string name = Console.ReadLine();
        Console.WriteLine($"Schön, dich zu sehen, {name}!");
    }
}
''', [T("Schön, dich zu sehen, Ben!", "Ben"), T("Schön, dich zu sehen, Aylin!", "Aylin")],
  "Einlesen funktioniert in der Main-Methode genauso wie in Lektion 6.", "lenient", anf=KLASSE)

N("l07", "Kassenzettel klassisch", """
Schreib ein Programm im klassischen Aufbau. Es liest zwei Preise ein (Kommazahlen) und gibt die Summe mit zwei Nachkommastellen aus. Bei `2,49` und `4,99`:

```
Summe: 7,48 Euro
```
""", r'''
using System;

class Program
{
    static void Main(string[] args)
    {
        double preis1 = double.Parse(Console.ReadLine());
        double preis2 = double.Parse(Console.ReadLine());
        Console.WriteLine($"Summe: {preis1 + preis2:F2} Euro");
    }
}
''', [T("Summe: 7,48 Euro", "2,49", "4,99"), T("Summe: 3,50 Euro", "1", "2,5")],
  "Alles, was du in Lektion 5 und 6 gelernt hast, funktioniert auch in der Main-Methode.", "lenient", anf=KLASSE)

# ---------------- l08 if / else ----------------
N("l08", "Volljährig?", """
Lies ein Alter ein. Ab 18 gilt man als volljährig. Bei `17`:

```
Du bist minderjährig.
```

Bei `18`:

```
Du bist volljährig.
```
""", r'''
int alter = int.Parse(Console.ReadLine());
if (alter >= 18)
{
    Console.WriteLine("Du bist volljährig.");
}
else
{
    Console.WriteLine("Du bist minderjährig.");
}
''', [T("Du bist minderjährig.", "17"), T("Du bist volljährig.", "18"), T("Du bist volljährig.", "30")],
  "Überleg genau, ob 18 schon dazugehört. Der passende Vergleich steht in der Tabelle der Lektion.", "lenient")

N("l08", "Positiv, negativ oder null", """
Lies eine ganze Zahl ein und sag, ob sie positiv, negativ oder null ist. Bei `-3`:

```
Die Zahl ist negativ.
```

Bei `0` erscheint `Die Zahl ist null.`, bei `5` erscheint `Die Zahl ist positiv.`
""", r'''
int zahl = int.Parse(Console.ReadLine());
if (zahl > 0)
{
    Console.WriteLine("Die Zahl ist positiv.");
}
else if (zahl < 0)
{
    Console.WriteLine("Die Zahl ist negativ.");
}
else
{
    Console.WriteLine("Die Zahl ist null.");
}
''', [T("Die Zahl ist negativ.", "-3"), T("Die Zahl ist null.", "0"), T("Die Zahl ist positiv.", "5")],
  "Es gibt drei Möglichkeiten, also brauchst du mehr als zwei Zweige. Die Lektion zeigt dir, wie du Bedingungen aneinanderhängst.", "lenient")

N("l08", "Punkte bewerten", """
Lies eine Punktzahl von 0 bis 100 ein und gib die Bewertung aus:
- ab 90 Punkten: `sehr gut`
- ab 75 Punkten: `gut`
- ab 50 Punkten: `bestanden`
- darunter: `nicht bestanden`

Bei `80`:

```
Ergebnis: gut
```
""", r'''
int punkte = int.Parse(Console.ReadLine());
if (punkte >= 90)
{
    Console.WriteLine("Ergebnis: sehr gut");
}
else if (punkte >= 75)
{
    Console.WriteLine("Ergebnis: gut");
}
else if (punkte >= 50)
{
    Console.WriteLine("Ergebnis: bestanden");
}
else
{
    Console.WriteLine("Ergebnis: nicht bestanden");
}
''', [T("Ergebnis: gut", "80"), T("Ergebnis: sehr gut", "90"), T("Ergebnis: bestanden", "50"), T("Ergebnis: nicht bestanden", "49")],
  "Prüfe von der höchsten Grenze nach unten. Sobald eine Bedingung passt, werden die übrigen übersprungen.", "lenient")

N("l08", "Rabatt ab 50 Euro", """
Lies einen Einkaufsbetrag ein (Kommazahl). Ab 50 Euro gibt es 10 Prozent Rabatt, darunter keinen. Gib den Betrag mit zwei Nachkommastellen aus. Bei `60`:

```
Zu zahlen: 54,00 Euro
```

Bei `40` erscheint `Zu zahlen: 40,00 Euro`.
""", r'''
double betrag = double.Parse(Console.ReadLine());
if (betrag >= 50)
{
    betrag = betrag * 0.9;
}
Console.WriteLine($"Zu zahlen: {betrag:F2} Euro");
''', [T("Zu zahlen: 54,00 Euro", "60"), T("Zu zahlen: 40,00 Euro", "40"), T("Zu zahlen: 45,00 Euro", "50")],
  "Die Ausgabe ist in beiden Fällen gleich aufgebaut, nur der Betrag ändert sich. 10 Prozent weniger heißt: 90 Prozent des Betrags.", "lenient")

N("l08", "Schaltjahr", """
Lies eine Jahreszahl ein und prüfe, ob es ein Schaltjahr ist. Die Regel: Ein Jahr ist ein Schaltjahr, wenn es durch 4 teilbar ist – außer es ist auch durch 100 teilbar. Ist es durch 400 teilbar, ist es trotzdem ein Schaltjahr. Bei `2024`:

```
2024 ist ein Schaltjahr.
```

Bei `1900`:

```
1900 ist kein Schaltjahr.
```
""", r'''
int jahr = int.Parse(Console.ReadLine());
bool schaltjahr;
if (jahr % 400 == 0)
{
    schaltjahr = true;
}
else if (jahr % 100 == 0)
{
    schaltjahr = false;
}
else if (jahr % 4 == 0)
{
    schaltjahr = true;
}
else
{
    schaltjahr = false;
}
if (schaltjahr)
{
    Console.WriteLine($"{jahr} ist ein Schaltjahr.");
}
else
{
    Console.WriteLine($"{jahr} ist kein Schaltjahr.");
}
''', [T("2024 ist ein Schaltjahr.", "2024"), T("1900 ist kein Schaltjahr.", "1900"), T("2000 ist ein Schaltjahr.", "2000"), T("2023 ist kein Schaltjahr.", "2023")],
  "Ob eine Zahl teilbar ist, erkennst du am Rest der Division aus Lektion 4. Prüfe die speziellste Regel zuerst.", "lenient")

# ---------------- l09 logische Operatoren ----------------
N("l09", "Anmeldung", """
Lies einen Benutzernamen und ein Passwort ein. Nur wenn der Name `admin` **und** das Passwort `1234` ist, erscheint `Zugang erlaubt`, sonst `Zugang verweigert`. Prüfe beides in **einer** Bedingung.

Bei `admin` und `1234`:

```
Zugang erlaubt
```
""", r'''
string name = Console.ReadLine();
string passwort = Console.ReadLine();
if (name == "admin" && passwort == "1234")
{
    Console.WriteLine("Zugang erlaubt");
}
else
{
    Console.WriteLine("Zugang verweigert");
}
''', [T("Zugang erlaubt", "admin", "1234"), T("Zugang verweigert", "admin", "0000"), T("Zugang verweigert", "gast", "1234")],
  "Beide Angaben müssen gleichzeitig stimmen. Die Lektion zeigt dir, wie du zwei Bedingungen verknüpfst.", "lenient",
  anf=[(r"&&", "Verknüpfe die zwei Bedingungen so, dass beide stimmen müssen.")])

N("l09", "Sommerferien", """
Lies eine Monatszahl ein. Im Juli (`7`) oder August (`8`) erscheint `Sommerferien!`, sonst `Schulzeit.`. Prüfe beide Monate in **einer** Bedingung.

```
Sommerferien!
```
""", r'''
int monat = int.Parse(Console.ReadLine());
if (monat == 7 || monat == 8)
{
    Console.WriteLine("Sommerferien!");
}
else
{
    Console.WriteLine("Schulzeit.");
}
''', [T("Sommerferien!", "7"), T("Sommerferien!", "8"), T("Schulzeit.", "9")],
  "Hier reicht es, wenn eine der beiden Bedingungen stimmt. Dafür gibt es einen eigenen logischen Operator.", "lenient",
  anf=[(r"\|\|", "Verknüpfe die Monate so, dass einer von beiden reicht.")])

N("l09", "Leere Eingabe", """
Lies einen Text ein. Hat jemand etwas eingegeben, erscheint `Danke für: ` und der Text. Bei einer leeren Eingabe (nur Enter) erscheint `Du hast nichts eingegeben.`

Bei `Hallo`:

```
Danke für: Hallo
```
""", r'''
string text = Console.ReadLine();
if (text != "")
{
    Console.WriteLine($"Danke für: {text}");
}
else
{
    Console.WriteLine("Du hast nichts eingegeben.");
}
''', [T("Danke für: Hallo", "Hallo"), T("Du hast nichts eingegeben.", "")],
  "Eine leere Eingabe ist ein Text ohne Zeichen, also zwei Anführungszeichen direkt hintereinander. Vergleiche die Eingabe damit.", "lenient")

N("l09", "Teenager", """
Lies ein Alter ein. Von 13 bis einschließlich 19 erscheint `Du bist ein Teenager.`, sonst `Du bist kein Teenager.`. Prüfe den Bereich in **einer** Bedingung.

Bei `15`:

```
Du bist ein Teenager.
```
""", r'''
int alter = int.Parse(Console.ReadLine());
if (alter >= 13 && alter <= 19)
{
    Console.WriteLine("Du bist ein Teenager.");
}
else
{
    Console.WriteLine("Du bist kein Teenager.");
}
''', [T("Du bist ein Teenager.", "15"), T("Du bist ein Teenager.", "13"), T("Du bist ein Teenager.", "19"),
      T("Du bist kein Teenager.", "12"), T("Du bist kein Teenager.", "20")],
  "Das Alter muss über der unteren und unter der oberen Grenze liegen. Achte darauf, dass 13 und 19 mitzählen.", "lenient",
  anf=[(r"&&", "Prüfe die untere und die obere Grenze in einer Bedingung.")])

N("l09", "Wetter-Berater", """
Lies die Temperatur (ganze Zahl) und dann `ja` oder `nein` für Regen ein. Speichere in einer Wahrheitswert-Variablen `regen`, ob es regnet. Dann gilt:
- Regnet es: `Regenschirm nicht vergessen!`
- Kein Regen und mindestens 20 Grad: `Ab ins Freibad!`
- Sonst: `Jacke an und los!`

Bei `25` und `nein`:

```
Ab ins Freibad!
```
""", r'''
int temperatur = int.Parse(Console.ReadLine());
bool regen = Console.ReadLine() == "ja";
if (regen)
{
    Console.WriteLine("Regenschirm nicht vergessen!");
}
else if (temperatur >= 20 && !regen)
{
    Console.WriteLine("Ab ins Freibad!");
}
else
{
    Console.WriteLine("Jacke an und los!");
}
''', [T("Ab ins Freibad!", "25", "nein"), T("Regenschirm nicht vergessen!", "25", "ja"), T("Jacke an und los!", "12", "nein")],
  "Ein Vergleich liefert selbst schon true oder false – das kannst du direkt in der Variablen speichern.", "lenient",
  anf=[(r"bool\s+regen\s*=", "Speichere in einer Variablen regen, ob es regnet.")])
