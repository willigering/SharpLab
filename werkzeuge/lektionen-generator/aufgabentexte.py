r"""Aufgabentexte ohne Lösungsweg (Nov 2026, Wunsch von Willi):
„In der Aufgabe soll nicht stehen, was zu tun ist … In keiner Aufgabe soll offensichtlich auf die Lösung
hingewiesen werden.“

Wird von build.py NACH review.py geladen. Die Aufgabentexte nennen nur noch das ZIEL (gewünschte Ausgabe,
Eingaben, Namen/Signaturen, die geprüft werden) – nicht mehr die Syntax, den Operator, die Methode oder die
Schleifenart. Tipps zeigen höchstens die Richtung, Anforderungs-Meldungen (nur bei Fehlschlag sichtbar)
sind sanfte Stupser. IDs, Tests, Vergleichsmodus, Lösungen, Startcode und Anforderungs-Muster bleiben gleich.

Jeder Eintrag prüft beim Bauen, dass der alte Text wirklich noch der erwartete ist (Wortanfang), damit
nichts versehentlich überschrieben wird. vorher_nachher() liefert die Tabelle für die Doku.
"""
import gen

# ---------------------------------------------------------------- Aufgabentexte
TASKS = {
    "l01a2": """Gib diese drei Zeilen aus:

```
Ich heiße Max.
Ich bin 20 Jahre alt.
Meine Lieblingsfarbe ist Blau.
```""",
    "l01a3": """Gib die drei Wörter mit **drei getrennten** Ausgabe-Befehlen aus – trotzdem soll am Ende nur **eine** Zeile entstehen:

```
Eins Zwei Drei
```""",
    "l02a1": """Das Programm gibt drei Zeilen aus. Sorge dafür, dass die **mittlere** Zeile nicht mehr ausgegeben wird – lösch sie aber nicht, sie soll im Code stehen bleiben. Danach erscheint nur noch:

```
Guten Morgen
Gute Nacht
```""",
    "l02a2": """Schreib direkt über die Ausgabezeile einen einzeiligen Kommentar, der erklärt, was die Zeile macht. Die Ausgabe soll sein:

```
C# macht Spaß
```""",
    "l02a3": """Sorge dafür, dass die beiden mittleren Zeilen nicht mehr ausgegeben werden – ohne sie zu löschen. Schaff das mit **einem einzigen** Kommentar (nicht Zeile für Zeile). Ausgabe:

```
Start
Ende
```""",
    "l03a2": """Gegeben sind die Variablen `tier` (Wert `"Katze"`) und `beine` (Wert `4`). Bau daraus diesen Satz zusammen – `Katze` und `4` sollen aus den Variablen kommen, nicht von Hand eingetippt:

```
Eine Katze hat 4 Beine.
```""",
    "l03a3": """Lege eine ganzzahlige Variable `punkte` mit dem Wert `10` an und gib den Wert aus. Ändere danach den Wert **derselben** Variablen auf `50` (keine zweite Variable anlegen) und gib ihn nochmal aus:

```
Punkte: 10
Punkte: 50
```""",
    "l04a1": """Ein Rechteck ist 8 breit und 5 hoch (ganze Zahlen). Berechne Fläche (Breite × Höhe) und Umfang (2 × (Breite + Höhe)) und gib aus:

```
Fläche: 40
Umfang: 26
```""",
    "l04a2": """17 Kekse werden gerecht an 5 Kinder verteilt. Berechne, wie viele jedes Kind bekommt und wie viele übrig bleiben – lass beides vom Programm ausrechnen:

```
Jedes Kind: 3
Übrig: 2
```""",
    "l04a3": """Starte mit einer Variablen `level` mit dem Wert `1`. Erhöhe `level` zweimal um 1 und danach um 10 – jeweils mit der **Kurzschreibweise** aus dieser Lektion (nicht `level = level + …`). Gib nach jedem Schritt aus:

```
Level 1
Level 2
Level 3
Level 13
```""",
    "l04a4": """Berechne den Durchschnitt der Noten `2`, `3` und `2` als Kommazahl – achte darauf, dass dabei **nicht ganzzahlig** geteilt wird. Gib das Ergebnis mit 2 Nachkommastellen aus. Wie man Nachkommastellen festlegt, lernst du erst in der nächsten Lektion – für die Ausgabe darfst du deshalb diese Zeile übernehmen:

```
Console.WriteLine($"Durchschnitt: {schnitt:F2}");
```

Ausgabe:
```
Durchschnitt: 2,33
```""",
    "l05a1": """Lege Variablen für Name (`Tom`) und Alter (`17`) an und bau sie so in den Satz ein, wie du es in dieser Lektion gelernt hast – ohne Text mit Plus-Zeichen zusammenzukleben:

```
Ich heiße Tom und bin 17 Jahre alt.
```""",
    "l05a2": """Ein Kino-Ticket kostet `9.5` Euro. Berechne den Preis für `4` Tickets **direkt im Ausgabetext** (ohne Extra-Variable für das Ergebnis) und gib ihn mit 2 Nachkommastellen aus:

```
4 Tickets kosten 38,00 Euro.
```""",
    "l05a3": """Gib alle drei Zeilen mit **einer einzigen** Anweisung aus:

```
Rot
Gelb
Grün
```""",
    "l06a3": """Lies nacheinander zwei ganze Zahlen ein und gib ihre Summe aus. Bei `7` und `5`:

```
7 + 5 = 12
```""",
    "l06a4": """Lies einen Preis als Kommazahl und danach eine Menge als ganze Zahl ein. Gib den Gesamtpreis mit 2 Nachkommastellen aus. Bei `2,5` und `3`:

```
Gesamt: 7,50 Euro
```""",
    "l07a2": """Dieses Programm lässt sich nicht kompilieren – an seinem Aufbau fehlt etwas. Repariere es, sodass es ausgibt:

```
Alle Klammern stimmen!
```""",
    "l07a3": """Schreib dieses Top-Level-Programm in den **klassischen Aufbau** um – mit der Klasse `Program` und der Methode `Main`:

```
string stadt = "Hamburg";
Console.WriteLine($"Ich wohne in {stadt}.");
```

Ausgabe:
```
Ich wohne in Hamburg.
```""",
    "l08a1": """Lies eine ganze Zahl ein. Ist sie gerade, gib `gerade` aus, sonst `ungerade`.

Bei `4` → `gerade`, bei `7` → `ungerade`.""",
    "l09a1": """Lies eine Zahl ein. Liegt sie zwischen 1 und 100 (beide eingeschlossen), gib `im Bereich` aus, sonst `außerhalb`. Prüfe beide Grenzen in **einer einzigen** Bedingung.""",
    "l09a2": """Lies einen Wochentag ein (z. B. `Samstag`). Ist es `Samstag` oder `Sonntag`, gib `Wochenende!` aus, sonst `Arbeitstag`. Prüfe beide Tage in **einer einzigen** Bedingung.""",
    "l10a1": """Lies eine Zahl von 1 bis 7 ein und gib den Wochentag aus (1 = `Montag` … 7 = `Sonntag`). Bei allen anderen Zahlen: `Ungültig`. Löse die Auswahl mit der Mehrfachauswahl aus dieser Lektion statt mit einer Kette von Abfragen.""",
    "l10a2": """Lies zwei ganze Zahlen und dann ein Rechenzeichen (`+`, `-`, `*` oder `/`) ein. Wähle die passende Rechnung mit der Mehrfachauswahl aus dieser Lektion aus und gib das Ergebnis aus, z. B. bei `8`, `2`, `*`:

```
Ergebnis: 16
```
Bei einem unbekannten Zeichen: `Unbekanntes Zeichen`. (`/` teilt ganzzahlig.)""",
    "l10a3": """Lies eine Monatszahl (1–12) ein und gib mit der Mehrfachauswahl aus dieser Lektion die Jahreszeit aus. Monate mit derselben Jahreszeit sollen sich **denselben** Code teilen (nicht viermal dasselbe schreiben):
- 12, 1, 2 → `Winter`
- 3, 4, 5 → `Frühling`
- 6, 7, 8 → `Sommer`
- 9, 10, 11 → `Herbst`
- sonst → `Ungültiger Monat`""",
    "l11a1": """Gib die Zahlen von 1 bis 10 aus, jede in einer eigenen Zeile – lass eine Schleife die Arbeit machen, statt zehn Ausgaben zu schreiben.""",
    "l11a2": """Starte mit der Zahl 1 und verdopple sie immer wieder, solange sie kleiner als 1000 ist. Gib jede Zahl aus:

```
1
2
4
...
512
```""",
    "l11a3": """Frage so lange nach dem Passwort, bis `geheim` eingegeben wird. Bei falscher Eingabe gib `Falsch!` aus, am Ende `Willkommen!`.

Bei den Eingaben `abc`, `123`, `geheim`:
```
Falsch!
Falsch!
Willkommen!
```""",
    "l12a1": """Gib die Fünferreihe aus – mit der Schleifenart aus dieser Lektion, deren Zähler von 1 bis 10 läuft:

```
1 x 5 = 5
2 x 5 = 10
...
10 x 5 = 50
```""",
    "l12a2": """Lies eine Zahl `n` ein und zähle mit der Schleifenart aus dieser Lektion von `n` bis 1 rückwärts. Danach `Abflug!`. Bei `3`:

```
3
2
1
Abflug!
```""",
    "l12a3": """Berechne mit der Schleifenart aus dieser Lektion die Summe aller geraden Zahlen von 2 bis 100 (2 + 4 + … + 100):

```
Summe: 2550
```""",
    "l12a4": """Lies eine Zahl `n` ein und gib eine Treppe aus `n` Zeilen aus – mit der Schleifenart aus dieser Lektion. Bei `4`:

```
*
**
***
****
```""",
    "l13a1": """Lege ein Array mit den Tagen `Mo`, `Di`, `Mi`, `Do`, `Fr` an. Gib das **erste** und das **letzte** Element aus und dann die Länge. Die Länge und die Position des letzten Elements sollen vom Array kommen, nicht von Hand eingetippt sein:

```
Mo
Fr
5
```""",
    "l13a2": """Lege ein Array `quadrate` mit 10 Fächern an. Fülle es automatisch (nicht jedes Fach einzeln), sodass in Fach `i` der Wert `i · i` steht. Gib danach alle Werte in **einer** Zeile aus, getrennt durch Leerzeichen:

```
0 1 4 9 16 25 36 49 64 81
```""",
    "l13a3": """Finde im Array `{ 12, 45, 7, 89, 23, 56 }` selbst den größten Wert, indem du die Werte der Reihe nach durchgehst (ohne fertige Funktion wie `Max()`):

```
Maximum: 89
```""",
    "l14a1": """Gib alle Einträge des Arrays aus, jeweils mit einem `- ` davor – mit der Schleifenart aus dieser Lektion:

```
- Milch
- Brot
- Eier
- Äpfel
```""",
    "l14a2": """Berechne mit der Schleifenart aus dieser Lektion den Durchschnitt der Temperaturen und gib ihn mit **einer** Nachkommastelle aus:

```
Durchschnitt: 18,4 Grad
```""",
    "l14a3": """Lies ein Wort ein und zähle mit der Schleifenart aus dieser Lektion die Vokale (a, e, i, o, u – nur Kleinbuchstaben). Bei `programmieren`:

```
Vokale: 5
```""",
    "l15a1": """Lege eine leere Liste für Texte an, füge die Filme `Matrix`, `Shrek` und `Up` nacheinander hinzu und gib dann aus. Die Anzahl soll aus der Liste kommen, nicht von Hand eingetippt:

```
3 Filme:
Matrix
Shrek
Up
```""",
    "l15a2": """Lies Zahlen ein, bis `0` kommt, und speichere sie in einer Liste. Gib danach Anzahl, Summe und die sortierte Liste aus. Bei `5`, `2`, `8`, `0`:

```
Anzahl: 3
Summe: 15
Sortiert: 2 5 8
```""",
    "l17a2": """Schreib `static double Brutto(double netto)`, die den Preis plus 19 % Mehrwertsteuer zurückgibt. Gib damit aus:

```
100 Euro netto = 119,00 Euro brutto
50 Euro netto = 59,50 Euro brutto
```""",
    "l18a3": """Lies einen Satz ein, zerlege ihn in seine Wörter und gib die Anzahl der Wörter und das längste Wort aus. Bei `Ich lerne gerade programmieren`:

```
Wörter: 4
Längstes Wort: programmieren
```""",
    "l19a1": """Lies eine Eingabe. Ist es eine ganze Zahl, gib das Doppelte aus, z. B. `Das Doppelte ist 14` (bei `7`), sonst `Ungültige Eingabe`. Dein Programm darf bei einer falschen Eingabe **nicht abstürzen**.""",
    "l20a2": """Lies einen Radius (Kommazahl) ein und gib Umfang (2·π·r) und Fläche (π·r²) mit 2 Nachkommastellen aus. Tipp π nicht als Zahl ein. Bei `5`:

```
Umfang: 31,42
Fläche: 78,54
```""",
    "l20a3": """Für eine Party brauchst du pro Person 3 Pizzastücke. Eine Pizza hat 8 Stücke. Lies die Anzahl der Personen ein und berechne, wie viele ganze Pizzen du **mindestens** bestellen musst – angefangene Pizzen zählen voll. Bei `10`:

```
Anzahl Pizzen: 4
```""",
    "l22a3": """Schreib eine Klasse `Punkt` mit Feldern `X` und `Y` (int), einem Konstruktor, dessen Parameter **genauso heißen** wie die Felder (`X` und `Y`), und einer Methode `public string AlsText()`, die z. B. `(3|4)` zurückgibt. Gib aus:

```
(3|4)
(-1|7)
```""",
    "l23a1": """Ergänze die Eigenschaft `Celsius`: Werte unter -273 sollen auf -273 gesetzt werden (kälter geht nicht). Ergänze außerdem eine Eigenschaft `Fahrenheit`, die man nur **lesen** kann und die die Temperatur in °F liefert (°F = °C · 1,8 + 32).

```
20 °C = 68,0 °F
-273 °C = -459,4 °F
```""",
    "l24a2": """Finde in der Liste selbst das teuerste Produkt, indem du die Produkte der Reihe nach durchgehst, und gib aus:

```
Teuerstes Produkt: Monitor (199,99 Euro)
```""",
    "l25a1": """Lies zuerst die Anzahl der Noten ein, dann die Noten selbst, und speichere sie in einer Liste. Gib sie danach so aus (bei `3`, `2`, `1`, `4`):

```
Deine Noten: 2, 1, 4
```""",
    "l26a3": """Erweitere das Spiel:
- Ungültige Eingaben (keine Zahl) → `Das ist keine Zahl!` (zählt nicht als Versuch). Das Programm darf dabei **nicht abstürzen**.
- Nach **5** Versuchen ohne Treffer ist Schluss: `Verloren! Die Zahl war 37.`
- Bei Treffer: `Richtig! Versuche: 2`

Bei `50`, `abc`, `37`:
```
Zu groß!
Das ist keine Zahl!
Richtig! Versuche: 2
```""",
}

# ---------------------------------------------------------------- Tipps (höchstens 2 Sätze, nur die Richtung)
HINTS = {
    "l01a1": "Text, der genau so erscheinen soll, steht im Code in Anführungszeichen. Achte auf das Komma, das Ausrufezeichen und das Semikolon am Ende.",
    "l01a2": "Jeder Satz steht in einer eigenen Zeile – überleg, welcher Ausgabe-Befehl danach in die nächste Zeile springt. Achte auf den Punkt am Satzende.",
    "l02a1": "In dieser Lektion hast du gelernt, wie man den Computer eine Zeile überspringen lässt, ohne sie zu löschen.",
    "l02a2": "Ein Kommentar ist eine Notiz für Menschen, der Computer ignoriert sie. Schreib sie in eine eigene Zeile direkt über den Befehl.",
    "l02a3": "Schau in der Theorie nach, wie ein Kommentar über mehrere Zeilen geht. Er hat einen Anfang und ein Ende.",
    "l03a2": "Denk an die Leerzeichen zwischen Text und Variablen – sie gehören mit in die Anführungszeichen.",
    "l03a3": "Eine Variable legst du nur einmal an. Beim Ändern bekommt sie nur einen neuen Wert.",
    "l04a2": "Teilst du zwei ganze Zahlen, ist das Ergebnis wieder eine ganze Zahl. Für den Rest beim Teilen kennst du aus der Lektion einen eigenen Operator.",
    "l04a3": "Für „um 1 erhöhen“ und „um eine bestimmte Zahl erhöhen“ gibt es in der Lektion je eine Kurzschreibweise. Gib nach jeder Änderung den Wert erneut aus.",
    "l04a4": "Teilst du nur ganze Zahlen, schneidet C# die Nachkommastellen ab – mindestens eine Zahl in der Rechnung muss eine Kommazahl sein. Denk an Punkt vor Strich.",
    "l05a1": "Lege Variablen für Name und Alter an. Die Lektion zeigt dir, wie du Variablen direkt in einen Text einbaust.",
    "l05a2": "Im eingebauten Teil des Textes darf auch eine Rechnung stehen. Wie du die Anzahl der Nachkommastellen festlegst, steht in der Lektion.",
    "l05a3": "Es gibt ein Sonderzeichen, das mitten im Text eine neue Zeile beginnt. Schau in der Lektion nach.",
    "l06a4": "Eine Kommazahl wandelst du mit einer anderen Methode um als eine ganze Zahl. Wie man auf 2 Nachkommastellen formatiert, kennst du aus der Lektion zur String-Interpolation.",
    "l07a2": "Zähle, wie viele Blöcke geöffnet werden – und wie viele davon wieder geschlossen werden.",
    "l07a3": "Schau dir in der Theorie an, wie der klassische Aufbau aussieht: die Klasse außen, Main innen. Die beiden Zeilen wandern unverändert hinein.",
    "l09a1": "Die Zahl muss zwei Bedingungen gleichzeitig erfüllen: mindestens 1 und höchstens 100. Wie man zwei Bedingungen verknüpft, steht in der Lektion.",
    "l09a2": "Vergleiche den Text einmal mit \"Samstag\" und einmal mit \"Sonntag\". Es reicht, wenn einer der beiden Vergleiche stimmt.",
    "l09a3": "Wandle ja/nein zuerst in einen Wahrheitswert um. Überleg, welche Teile der Regel zusammengehören, und setz sie in Klammern.",
    "l10a1": "Jede Zahl von 1 bis 7 bekommt ihren eigenen Fall. Für alle anderen Zahlen gibt es einen Auffang-Fall.",
    "l10a2": "Das Rechenzeichen ist Text, und jedes mögliche Zeichen bekommt seinen eigenen Fall. Vergiss den Auffang-Fall nicht.",
    "l10a3": "In der Lektion siehst du, wie sich mehrere Fälle einen gemeinsamen Code-Block teilen.",
    "l14a1": "Die Schleife aus dieser Lektion liefert dir jedes Element nacheinander, ganz ohne Index. Davor setzt du einfach den Strich.",
    "l14a2": "Addiere alle Werte zu einer Summe und teile danach durch die Anzahl. Eine Nachkommastelle formatierst du genauso wie zwei, nur mit einer anderen Zahl.",
    "l14a3": "Ein einzelnes Zeichen schreibst du in einfache Anführungszeichen, nicht in doppelte.",
    "l15a1": "Erst die leere Liste anlegen, dann die Filme einzeln hinzufügen. Wie viele Elemente drin sind, kann dir die Liste selbst sagen.",
    "l15a3": "Prüf zuerst, ob der Name auf der Liste steht – die Liste hat dafür eine eigene Methode, genauso fürs Entfernen. Die Anzahl gibst du in beiden Fällen ganz am Ende aus.",
    "l16a3": "Wie lang die Linie wird, hängt von der Anzahl der Zeichen im Text ab. Eine Linie aus Sternen kannst du Stück für Stück in einer Schleife bauen.",
    "l17a2": "Die Methode rechnet nur und liefert den Wert zurück. Die 2 Nachkommastellen kommen erst beim Ausgeben dazu.",
    "l17a3": "Probiere alle möglichen Teiler zwischen 2 und n-1 durch. Lässt sich n durch einen davon ohne Rest teilen, ist es keine Primzahl.",
    "l18a1": "Entferne zuerst die Leerzeichen am Rand und behandle dann den ersten Buchstaben und den Rest getrennt. Gib keine Eingabeaufforderung aus.",
    "l18a2": "Bau dir den Text rückwärts zusammen und vergleiche ihn mit dem Original. Beide müssen dafür in derselben Schreibweise sein.",
    "l18a3": "Zerlegst du den Satz, bekommst du ein Array mit den Wörtern. Fürs längste Wort vergleichst du die Wörter in einer Schleife nacheinander.",
    "l19a1": "Die sichere Umwandlung aus dieser Lektion meldet dir, ob es geklappt hat. Das kannst du direkt in der Bedingung benutzen.",
    "l19a2": "Wiederhole das Einlesen, solange die sichere Umwandlung nicht klappt. Bei jedem Fehlschlag gibst du die Bitte aus.",
    "l19a3": "Lies die 5 Eingaben in einer Schleife ein. Je nachdem, ob die Umwandlung klappt, addierst du zur Summe oder zählst die ungültigen.",
    "l20a1": "Die Math-Klasse hat eine fertige Methode für die Quadratwurzel. Deine Methode bekommt a und b und gibt das Ergebnis zurück.",
    "l20a2": "Die Kreiszahl kennt die Math-Klasse schon, und r² ist einfach r mal r. Die 2 Nachkommastellen kommen erst bei der Ausgabe dazu.",
    "l20a3": "Achte darauf, dass beim Teilen eine Kommazahl herauskommt – sonst wird schon vorher abgerundet! Zum Aufrunden hilft dir die Math-Klasse.",
    "l22a3": "Heißen Parameter und Feld gleich, musst du C# sagen, wann du das Feld des Objekts meinst. AlsText baut den Text aus X und Y zusammen.",
    "l23a1": "Zur Kontrolle: -273 °C sind -459,4 °F.",
    "l23a2": "Auto-Eigenschaften brauchen kein eigenes Feld – die Lektion zeigt dir die Kurzschreibweise. Für den Rabatt ziehst du den Prozentanteil vom Preis ab und teilst dabei durch eine Kommazahl.",
    "l24a1": "Erzeuge für jeden Schüler ein Objekt und füge es der Liste hinzu. Für den Durchschnitt addierst du in einer Schleife die Noten und teilst danach.",
    "l24a3": "Lies Name und Punkte in einer Schleife ein und lege für jeden ein Objekt an. Danach gibst du nur die mit mindestens 50 Punkten aus und zählst mit.",
    "l25a1": "Für die Ausgabe gibt es eine fertige Methode, die alle Elemente mit einem Trennzeichen zu einem Text verbindet. Du kannst den Text aber auch in einer Schleife bauen.",
    "l26a3": "Bei einer ungültigen Eingabe kannst du den Rest des Schleifendurchlaufs überspringen und gleich neu fragen.",
    "l27a2": "Die Methode zum Entfernen verrät dir mit ihrem Rückgabewert, ob wirklich etwas entfernt wurde.",
    "l06a1": "Die Lektion zeigt dir, wie dein Programm auf eine Eingabe wartet – speichere das Ergebnis in einer Variablen. Danach gibst du den Gruß mit dem Namen aus.",
    "l06a2": "Eine Eingabe ist immer Text – zum Rechnen musst du sie erst in eine Zahl umwandeln. Setz die Rechnung in der Ausgabe in Klammern.",
    "l06a3": "Lies zweimal ein und wandle jede Eingabe in eine Zahl um. Beide Zahlen und die Summe kannst du dann in einen Text einbauen.",
    "l07a1": "Der Befehl gehört zwischen die geschweiften Klammern von Main. Es ist ein ganz normaler Ausgabe-Befehl.",
    "l08a1": "Der Rest beim Teilen durch 2 verrät, ob eine Zahl gerade ist. Für den einen Fall und den „sonst“-Fall brauchst du je einen Zweig.",
    "l08a2": "Prüfe die Grenzen der Reihe nach, von unten nach oben. Überleg genau, ob „kleiner“ oder „kleiner gleich“ passt.",
    "l08a3": "Prüf zuerst, ob beide gleich sind. Danach entscheidest du, welche der beiden größer ist.",
    "l17a1": "Vergleiche a und b miteinander. Statt etwas auszugeben, lieferst du die größere Zahl an den Aufrufer zurück.",
    "l21a2": "Die Methode Flaeche rechnet Breite mal Hoehe und liefert das Ergebnis zurück. Breite und Hoehe setzt du im Hauptcode.",
    "l26a2": "Wiederhole das Raten, solange der Tipp nicht die geheime Zahl ist. Zähle dabei die Versuche mit und gib das Ergebnis erst nach der Schleife aus.",
    "l27a1": "Die Hauptschleife läuft, solange eine Variable sagt „weitermachen“ – Befehl 0 ändert das. Welcher Befehl eingegeben wurde, entscheidest du mit einer Mehrfachauswahl.",
}

# ---------------------------------------------------------------- Anforderungs-Meldungen (Index wie in der Aufgabe)
MESSAGES = {
    ("l01a3", 0): "Deine Ausgabe springt noch nach jedem Befehl in die nächste Zeile – in der Lektion gibt es eine Variante, die das nicht tut.",
    ("l02a1", 0): "Die mittlere Zeile soll im Code bleiben – lösch sie nicht, sondern sorg dafür, dass sie übersprungen wird.",
    ("l02a2", 0): "Über der Ausgabezeile fehlt noch ein einzeiliger Kommentar.",
    ("l02a3", 0): "Hier fehlt noch ein Kommentar, der beide Zeilen auf einmal ausblendet.",
    ("l02a3", 1): "Lösch die Zeilen nicht – blende sie aus, sodass sie im Code stehen bleiben.",
    ("l03a2", 0): "Die Variable tier fehlt noch in deinem Satz – verbinde sie mit dem Text.",
    ("l03a2", 1): "Die Variable beine fehlt noch in deinem Satz – verbinde sie mit dem Text.",
    ("l03a3", 0): "Die Variable punkte bekommt noch nicht den neuen Wert 50 – ändere die vorhandene Variable, statt eine neue anzulegen.",
    ("l04a2", 0): "Der Rest soll ausgerechnet werden – dafür gibt es in dieser Lektion einen eigenen Rechen-Operator.",
    ("l04a3", 0): "Für das Erhöhen um 1 fehlt noch die Kurzschreibweise aus der Lektion.",
    ("l04a3", 1): "Für das Erhöhen um 10 fehlt noch die Kurzschreibweise aus der Lektion.",
    ("l04a4", 0): "Hier wird noch ganzzahlig geteilt – sorg dafür, dass beim Teilen eine Kommazahl im Spiel ist.",
    ("l05a1", 0): "Bau die Variablen direkt in den Text ein, so wie in dieser Lektion – ohne Plus-Zeichen.",
    ("l05a2", 0): "Bau die Rechnung direkt in den Text ein, so wie in dieser Lektion – ohne Plus-Zeichen.",
    ("l05a3", 0): "Die Zeilenumbrüche sollen mitten im Text entstehen – in der Lektion gibt es dafür ein Sonderzeichen.",
    ("l06a2", 0): "Die Eingabe ist noch Text – wandle sie zuerst so in eine ganze Zahl um, wie es die Lektion zeigt.",
    ("l07a1", 0): "Lass die Main-Methode stehen und schreib den Befehl hinein.",
    ("l08a1", 0): "Triff die Entscheidung mit einer Verzweigung aus dieser Lektion.",
    ("l08a1", 1): "Für den Fall „sonst“ fehlt noch der passende Zweig.",
    ("l08a2", 0): "Für den mittleren Fall fehlt noch ein weiterer Zweig zwischen dem ersten und dem letzten.",
    ("l09a1", 0): "Beide Grenzen müssen gleichzeitig stimmen – verknüpfe die zwei Vergleiche mit dem passenden logischen Operator.",
    ("l09a2", 0): "Es reicht, wenn einer der beiden Vergleiche stimmt – verknüpfe sie mit dem passenden logischen Operator.",
    ("l10a1", 0): "Löse die Auswahl mit der Mehrfachauswahl aus dieser Lektion statt mit einer Kette von Abfragen.",
    ("l10a1", 1): "Es fehlt noch der Auffang-Fall für alle anderen Zahlen.",
    ("l10a2", 0): "Wähle die Rechnung mit der Mehrfachauswahl aus dieser Lektion aus.",
    ("l10a3", 0): "Löse die Auswahl mit der Mehrfachauswahl aus dieser Lektion.",
    ("l11a1", 0): "Lass die Schleife aus dieser Lektion die Arbeit machen.",
    ("l11a2", 0): "Lass die Schleife aus dieser Lektion das Verdoppeln wiederholen.",
    ("l11a3", 0): "Frag mit der Schleife aus dieser Lektion immer wieder nach.",
    ("l12a1", 0): "Nutze die Schleifenart aus dieser Lektion mit Zähler, Bedingung und Schritt.",
    ("l12a2", 0): "Nutze die Schleifenart aus dieser Lektion mit Zähler, Bedingung und Schritt.",
    ("l12a3", 0): "Nutze die Schleifenart aus dieser Lektion mit Zähler, Bedingung und Schritt.",
    ("l12a4", 0): "Nutze die Schleifenart aus dieser Lektion mit Zähler, Bedingung und Schritt.",
    ("l13a1", 0): "Tipp die Länge nicht von Hand ein – das Array kann dir seine Länge selbst verraten, auch für das letzte Element.",
    ("l13a2", 0): "Die Fächer von quadrate werden noch nicht in der Schleife befüllt.",
    ("l13a2", 1): "Fülle das Array mit einer Zählschleife, wie du sie aus der vorigen Lektion kennst.",
    ("l13a3", 0): "Geh die Werte selbst der Reihe nach durch.",
    ("l14a1", 0): "Nutze die Schleifenart aus dieser Lektion, die ganz ohne Index auskommt.",
    ("l14a2", 0): "Nutze die Schleifenart aus dieser Lektion, die ganz ohne Index auskommt.",
    ("l14a3", 0): "Nutze die Schleifenart aus dieser Lektion, die ganz ohne Index auskommt.",
    ("l15a1", 0): "Die Filme sollen nacheinander zur leeren Liste hinzugefügt werden.",
    ("l15a1", 1): "Die Anzahl soll aus der Liste kommen, nicht fest eingetippt sein.",
    ("l15a3", 0): "Prüf mit der passenden Listen-Methode, ob der Name auf der Liste steht.",
    ("l15a3", 1): "Entferne den Namen mit der passenden Listen-Methode.",
    ("l17a1", 0): "Die Methode soll die größere Zahl zurückliefern, nicht selbst ausgeben.",
    ("l18a3", 0): "Zerleg den Satz mit der passenden String-Methode aus dieser Lektion in einzelne Wörter.",
    ("l19a1", 0): "Wandle die Eingabe mit der sicheren Methode aus dieser Lektion um, damit nichts abstürzt.",
    ("l19a2", 0): "Wandle die Eingabe mit der sicheren Methode aus dieser Lektion um, damit nichts abstürzt.",
    ("l19a3", 0): "Wandle die Eingaben mit der sicheren Methode aus dieser Lektion um, damit nichts abstürzt.",
    ("l20a1", 0): "Für die Wurzel gibt es eine fertige Methode in der Math-Klasse.",
    ("l20a2", 0): "Tipp π nicht als Zahl ein – die Math-Klasse kennt die Kreiszahl schon.",
    ("l20a3", 0): "Zum Aufrunden gibt es eine fertige Methode in der Math-Klasse.",
    ("l21a2", 0): "Erzeuge im Hauptcode ein Rechteck-Objekt.",
    ("l22a3", 0): "Feld und Parameter heißen gleich – in der Lektion steht, wie du im Konstruktor das Feld eindeutig ansprichst.",
    ("l23a2", 0): "Name und Preis sollen Auto-Eigenschaften sein – nimm die Kurzschreibweise aus der Lektion.",
    ("l23a1", 0): "Ergänze in der Klasse die Eigenschaft public double Fahrenheit, die man nur lesen kann.",
    ("l24a1", 0): "Die Schüler sollen einzeln zur Liste hinzugefügt werden.",
    ("l24a2", 0): "Geh die Produkte selbst der Reihe nach durch.",
    ("l26a3", 0): "Wandle die Eingabe mit der sicheren Methode aus Lektion 19 um, damit nichts abstürzt.",
    ("l27a1", 0): "Neue Artikel sollen in der Liste gespeichert werden.",
    ("l27a1", 1): "Ob die Liste leer ist, kann sie dir selbst sagen – frag ihre Anzahl ab.",
    ("l27a2", 0): "Prüf mit der passenden Listen-Methode, ob der Artikel schon auf der Liste steht.",
    ("l27a2", 1): "Entferne Artikel mit der passenden Listen-Methode.",
    ("l27a3", 0): "Jeder neue Artikel soll ein eigenes Artikel-Objekt werden.",
    ("l27a3", 1): "Ist der Artikel schon da, soll die neue Menge zur vorhandenen dazukommen.",
}

# ---------------------------------------------------------------- Titel, die den Weg verraten
TITLES = {
    "l01a3": "Drei Wörter, eine Zeile",
    "l02a1": "Zeile ausblenden",
    "l02a3": "Zwei Zeilen ausblenden",
    "l04a3": "Level-Aufstieg",
    "l04a4": "Notendurchschnitt",
    "l05a3": "Drei Farben, eine Anweisung",
    "l22a3": "Punkt mit gleichen Namen",
    "l23a2": "Produkt mit Rabatt",
}

VORHER = {"tasks": {}, "hints": {}, "messages": {}, "titles": {}}


def anwenden():
    aufgaben = {a["id"]: a for l in gen.LESSONS for a in l["exercises"]}
    for aid, text in TASKS.items():
        a = aufgaben[aid]
        assert a["task"].strip() != text.strip(), f"{aid}: Text unverändert"
        VORHER["tasks"][aid] = a["task"]
        a["task"] = text.strip()
    for aid, text in TITLES.items():
        a = aufgaben[aid]
        assert a["title"] != text, f"{aid}: Titel unverändert"
        VORHER["titles"][aid] = a["title"]
        a["title"] = text
    for aid, text in HINTS.items():
        a = aufgaben[aid]
        assert a["hint"].strip() and a["hint"] != text, f"{aid}: Tipp leer oder unverändert"
        VORHER["hints"][aid] = a["hint"]
        a["hint"] = text
    for (aid, i), text in MESSAGES.items():
        r = aufgaben[aid]["requirements"][i]
        assert r["message"] != text, f"{aid}/{i}: Meldung unverändert"
        VORHER["messages"][(aid, i)] = r["message"]
        r["message"] = text


anwenden()
