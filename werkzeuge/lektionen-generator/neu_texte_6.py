"""Neue Aufgabentexte Lektion 23–27."""
from neu_texte import X, Z, B, FRAGE

# ------------------------------------------------------------------ L23
X("l23a1", "Im Startcode gibt es schon die Klasse `Thermometer` mit der Eigenschaft `Celsius`. Ergänze zwei Dinge: Die Temperatur darf nie unter -273 Grad fallen (alles darunter wird als -273 gespeichert). Außerdem gibt es eine Eigenschaft `Fahrenheit`, die man nur lesen kann und die aus Celsius berechnet wird (mal 1,8, plus 32).\n\n" + Z("20 °C = 68,0 °F", "-273 °C = -459,4 °F"),
  "Beim Setzen der Eigenschaft kannst du den neuen Wert prüfen, bevor du ihn speicherst. Die Fahrenheit-Eigenschaft braucht nur einen Lesezugriff.",
  m=["Ergänze eine Eigenschaft `Fahrenheit`, die aus Celsius berechnet wird."])
X("l23a2", "Schreib eine Klasse `Produkt` mit den Eigenschaften `Name` (Text) und `Preis` (Kommazahl), die man beim Erzeugen angibt, und einer Methode `PreisMitRabatt`, die einen Prozentsatz (ganze Zahl) bekommt und den reduzierten Preis zurückliefert. Name und Preis sollen als kurze Eigenschaften geschrieben sein. Der Startcode benutzt die Klasse schon:\n\n" + Z("Kopfhörer: 80,00 Euro, mit 25 % Rabatt: 60,00 Euro"),
  "Die Kurzform einer Eigenschaft kommt ohne eigenes Feld aus.",
  m=["Name und Preis sollen als Eigenschaften in Kurzform geschrieben werden. Die Lektion zeigt wie."])
X("l23a3", "Im Startcode gibt es die Klasse `Sparschwein` mit dem Kontostand, den man von außen nur lesen, aber nicht ändern kann. Schreib die Methode `Einzahlen`, die einen Betrag (ganze Zahl) bekommt. Ist er positiv, wird er addiert. Sonst gibt sie `Ungültiger Betrag` aus und ändert nichts:\n\n" + Z("Ungültiger Betrag", "Kontostand: 75"),
  "Nur die Klasse selbst darf den Kontostand ändern. Deshalb muss die Methode das tun.",
  m=["Schreib in der Klasse eine Methode `Einzahlen`, die eine ganze Zahl annimmt."])
X("l23a4", "Schreib eine Klasse `Film` mit den Eigenschaften `Titel` (Text) und `Jahr` (ganze Zahl) in der kurzen Schreibweise. Erzeuge `Matrix` (1999) und `Oben` (2009) und gib beide aus:\n\n" + Z("Matrix (1999)", "Oben (2009)"),
  "Die Eigenschaften setzt du nach dem Erzeugen des Objekts.",
  m=["`Titel` soll eine Eigenschaft sein.", "`Jahr` soll eine Eigenschaft sein.", "Schreib die Eigenschaften in der kurzen Schreibweise aus der Lektion."])
X("l23a5", "Schreib eine Klasse `Kreis` mit der Eigenschaft `Radius` (ganze Zahl, in der kurzen Schreibweise). Dazu gibt es eine Eigenschaft `Durchmesser`, die man nur lesen kann und die aus dem Radius berechnet wird. Teste mit Radius 3 und 5:\n\n" + Z("Radius 3, Durchmesser 6", "Radius 5, Durchmesser 10"),
  "Der Durchmesser wird nicht gespeichert, sondern bei jedem Zugriff berechnet.",
  m=["Schreib `Radius` in der kurzen Schreibweise aus der Lektion.", "`Durchmesser` soll eine Eigenschaft sein, die man nur lesen kann."])
X("l23a6", "Schreib eine Klasse `Person` mit der Eigenschaft `Alter` (ganze Zahl). Das Alter darf nie negativ sein: Wird ein negativer Wert gesetzt, wird stattdessen 0 gespeichert. Das Alter liegt dabei in einem privaten Feld.\n\n" + Z("Alter: 0", "Alter: 12"),
  "Beim Setzen bekommst du den neuen Wert und kannst entscheiden, was gespeichert wird.",
  m=["Das Alter soll in einem privaten Feld liegen.", "Die Eigenschaft `Alter` soll beim Setzen prüfen.", "Prüfe beim Setzen den neuen Wert."])
X("l23a7", "Schreib eine Klasse `Besucherzaehler`. Die Anzahl liegt in einem privaten Feld und ist von außen über die Eigenschaft `Anzahl` nur lesbar. Eine Methode `Eintreten` erhöht sie um 1. Lass drei Besucher eintreten:\n\n" + Z("Besucher: 3"),
  "Nur die Methode soll die Anzahl ändern dürfen, von außen kann man sie nur lesen.",
  m=["Die Anzahl soll in einem privaten Feld liegen.", "Die Klasse braucht eine Methode `Eintreten`.", "`Anzahl` soll von außen nur lesbar sein.", "Lass drei Besucher eintreten."])
X("l23a8", "Schreib eine Klasse `Benutzer` mit der Eigenschaft `Name` (Text). Beim Setzen werden Leerzeichen am Rand entfernt. Bleibt dabei nichts übrig, wird `Unbekannt` gespeichert. Anfangs ist der Name `Unbekannt`:\n\n" + Z("Name: Lisa", "Name: Unbekannt"),
  "Beim Setzen kannst du den Wert erst bereinigen und dann speichern.",
  m=["Die Eigenschaft `Name` soll beim Setzen aufräumen.", "Bearbeite beim Setzen den neuen Wert."])

# ------------------------------------------------------------------ L24
X("l24a1", "Im Startcode sind eine leere Liste und die Klasse `Schueler` mit Name und Note schon da. Nimm `Ana` (Note 2), `Ben` (Note 1) und `Cem` (Note 3) auf. Gib jeden mit seiner Note aus und danach den Notendurchschnitt mit zwei Nachkommastellen:\n\n" + Z("Ana: 2", "Ben: 1", "Cem: 3", "Durchschnitt: 2,00"),
  "Die Liste enthält Objekte. Jedes erzeugst du mit seinen Werten.",
  m=["Nimm die Schüler einzeln in die Liste auf."])
X("l24a2", "Im Startcode gibt es eine Produktliste mit der Klasse `Produkt`. Finde das teuerste Produkt und gib Name und Preis aus (zwei Nachkommastellen). Das Programm soll es finden, nicht du:\n\n" + Z("Teuerstes Produkt: Monitor (199,99 Euro)"),
  "Merk dir das bisher teuerste Produkt und vergleiche es mit jedem weiteren.",
  m=["Geh die Produkte selbst der Reihe nach durch."])
X("l24a3", "Das Programm liest zuerst die Anzahl der Teilnehmer ein, dann für jeden Namen und Punkte. Ab 50 Punkten gilt man als bestanden. Es listet alle Bestandenen auf und nennt am Ende, wie viele von allen bestanden haben. Im Startcode gibt es die Klasse `Teilnehmer`. " + FRAGE + "\n\n" + B(("3, Ana, 70, Ben, 40, Cem, 50", "Bestanden: Ana (70) / Bestanden: Cem (50) / 2 von 3 haben bestanden."), ("1, Zoe, 10", "0 von 1 haben bestanden.")),
  "Lies erst alle Teilnehmer in die Liste ein und werte sie dann aus.")
X("l24a4", "Schreib eine Klasse `Tier` mit Name und Anzahl der Beine. Lege `Hund` (4), `Huhn` (2) und `Spinne` (8) in einer Liste ab. Gib jedes Tier aus und danach die Gesamtzahl der Beine:\n\n" + Z("Hund hat 4 Beine.", "Huhn hat 2 Beine.", "Spinne hat 8 Beine.", "Beine insgesamt: 14"),
  "Liste durchgehen und nebenbei die Beine aufsummieren.",
  m=["Es fehlt die Klasse `Tier`.", "Sammle die Tiere in einer Liste.", "Geh die Liste in einer Schleife durch."])
X("l24a5", "Schreib eine Klasse `Person` mit Name und Alter. Lege `Mia` (14), `Opa Karl` (81), `Mama` (45) und `Ben` (9) in einer Liste ab. Finde die älteste Person und gib sie aus:\n\n" + Z("Älteste Person: Opa Karl (81)"),
  "Merk dir die bisher älteste Person und vergleiche mit jeder weiteren.",
  m=["Sammle die Personen in einer Liste.", "Geh die Liste selbst durch."])
X("l24a6", "Schreib eine Klasse `Produkt` mit Name und Preis. Lege `Stift` (1,50), `Rucksack` (39,99), `Heft` (2,20), `Lineal` (0,99) und `Taschenrechner` (14,90) in einer Liste ab. Gib alle Produkte aus, die weniger als 10 Euro kosten (in der Reihenfolge der Liste, zwei Nachkommastellen), und danach, wie viele es sind:\n\n" + Z("Stift: 1,50 Euro", "Heft: 2,20 Euro", "Lineal: 0,99 Euro", "3 Produkte unter 10 Euro"),
  "Prüfe jedes Produkt und gib nur die günstigen aus. Nebenbei zählst du mit.",
  m=["Sammle die Produkte in einer Liste.", "Geh die Liste in einer Schleife durch."])
X("l24a7", "Schreib eine Klasse `Kontakt` mit Name und Telefonnummer und lege diese Kontakte in einer Liste ab: `Ana` (0151-111), `Ben` (0160-222), `Cem` (0176-333). Das Programm liest einen Namen ein und gibt die Nummer aus, oder sagt, dass es keinen Eintrag gibt. " + FRAGE + "\n\n" + B(("Ben", "Ben: 0160-222"), ("Cem", "Cem: 0176-333"), ("Dora", "Kein Eintrag für Dora.")),
  "Geh die Liste durch und merk dir den passenden Kontakt, falls es einen gibt.",
  m=["Sammle die Kontakte in einer Liste."])
X("l24a8", "Schreib eine Klasse `Spieler` mit Name und Punkten. Das Programm liest zuerst die Anzahl der Spieler und dann für jeden Namen und Punkte ein. Es nennt den Sieger (die meisten Punkte) und den Durchschnitt aller Punkte mit einer Nachkommastelle. " + FRAGE + "\n\n" + B(("3, Ana, 70, Cem, 95, Ben, 75", "Sieger: Cem mit 95 Punkten / Durchschnitt: 80,0 Punkte"), ("2, Lea, 12, Tim, 10", "Sieger: Lea mit 12 Punkten / Durchschnitt: 11,0 Punkte")),
  "Die Spieler liegen in einer Liste. Sieger und Summe findest du in einem Durchlauf.",
  m=["Es fehlt die Klasse `Spieler`.", "Speichere die Spieler in einer Liste."])

# ------------------------------------------------------------------ L25
X("l25a1", "**Schritt 1.** Das Programm liest zuerst die Anzahl der Noten und danach die Noten selbst ein. Es gibt alle Noten in einer Zeile aus, durch Komma und Leerzeichen getrennt. " + FRAGE + "\n\n" + B(("3, 2, 1, 4", "Deine Noten: 2, 1, 4"), ("1, 5", "Deine Noten: 5")),
  "Die Noten gehören in eine Liste. Es gibt eine Methode, die eine Liste mit einem Trennzeichen zu einem Text verbindet.")
X("l25a2", "**Schritt 2.** Lies wie in Schritt 1 die Noten ein und gib dann den Durchschnitt (zwei Nachkommastellen), die beste und die schlechteste Note aus. Die beste Note ist die kleinste, die schlechteste die größte. " + FRAGE + "\n\n" + B(("4, 2, 1, 3, 2", "Durchschnitt: 2,00 / Beste Note: 1 / Schlechteste Note: 3"), ("3, 5, 4, 2", "Durchschnitt: 3,67 / Beste Note: 2 / Schlechteste Note: 5")),
  "Drei Auswertungen, die du am besten in eigene Methoden steckst.")
X("l25a3", "**Schritt 3.** Lies wie in Schritt 1 die Noten ein. Gib den Durchschnitt (zwei Nachkommastellen) und danach einen Kommentar aus: bis 1,5 `Ausgezeichnet!`, bis 2,5 `Gut gemacht!`, bis 3,5 `Befriedigend.`, darüber `Da geht noch was!`. " + FRAGE + "\n\n" + B(("2, 1, 2", "Durchschnitt: 1,50 / Ausgezeichnet!"), ("2, 2, 3", "Durchschnitt: 2,50 / Gut gemacht!"), ("1, 3", "Durchschnitt: 3,00 / Befriedigend."), ("2, 4, 5", "Durchschnitt: 4,50 / Da geht noch was!")),
  "Der Kommentar hängt allein vom Durchschnitt ab.")
X("l25a4", "**Schritt 4.** Lies wie in Schritt 1 die Noten ein, aber nur Noten von 1 bis 6 sind gültig. Zu jeder ungültigen Note gibt das Programm sofort `Ungültige Note: …` aus und nimmt sie nicht auf. Am Ende kommen die gültigen Noten wie in Schritt 1. " + FRAGE + "\n\n" + B(("4, 2, 7, 1, 0", "Ungültige Note: 7 / Ungültige Note: 0 / Deine Noten: 2, 1"), ("2, 3, 3", "Deine Noten: 3, 3")),
  "Prüf jede Note, bevor du sie in die Liste aufnimmst.")
X("l25a5", "**Schritt 5.** Lies wie in Schritt 1 die Noten ein und zähle, wie oft jede Note von 1 bis 6 vorkommt. Gib für jede Note eine Zeile aus, auch wenn sie 0-mal vorkommt. " + FRAGE + "\n\n" + B(("5, 2, 1, 2, 3, 2", "Note 1: 1x / Note 2: 3x / Note 3: 1x / Note 4: 0x / Note 5: 0x / Note 6: 0x"), ("2, 6, 4", "Note 1: 0x / … / Note 4: 1x / Note 5: 0x / Note 6: 1x")),
  "Für jede der sechs Noten gehst du die Liste durch und zählst.")
X("l25a6", "**Schritt 6.** Lies wie in Schritt 1 die Noten ein und vergleiche die erste mit der letzten. Ist die letzte besser (kleiner), sagt das Programm `Du wirst besser!`. Ist sie schlechter, `Du wirst schlechter.`. Sind beide gleich, `Gleich geblieben.`. " + FRAGE + "\n\n" + B(("3, 4, 3, 2", "Du wirst besser!"), ("2, 1, 3", "Du wirst schlechter."), ("3, 2, 5, 2", "Gleich geblieben.")),
  "Eine kleinere Note ist besser. Erste und letzte Note stehen an festen Plätzen der Liste.")
X("l25a7", "**Schritt 7.** Das Programm liest zuerst die Anzahl der Fächer ein, dann für jedes Fach den Namen und die Note. Es gibt jedes Fach mit Note aus und danach das beste Fach (kleinste Note). Bei Gleichstand gewinnt das erste. " + FRAGE + "\n\n" + B(("3, Mathe, 2, Deutsch, 1, Sport, 1", "Mathe: 2 / Deutsch: 1 / Sport: 1 / Bestes Fach: Deutsch (1)"), ("2, Kunst, 3, Bio, 4", "Kunst: 3 / Bio: 4 / Bestes Fach: Kunst (3)")),
  "Fach und Note gehören zusammen. Zwei Listen mit gleichen Plätzen oder eine eigene Klasse helfen dir dabei.")
X("l25a8", "**Schritt 8: das ganze Zeugnis.** Lies die Noten wie in Schritt 1 ein und gib alles aus: Noten, Durchschnitt (zwei Nachkommastellen), beste und schlechteste Note und den Kommentar (bis 1,5 `Ausgezeichnet!`, bis 2,5 `Gut gemacht!`, bis 3,5 `Befriedigend.`, darüber `Da geht noch was!`). Schreib dafür vier Methoden: `Durchschnitt` (liefert eine Kommazahl), `Beste` und `Schlechteste` (liefern ganze Zahlen) und `Bewertung` (liefert Text). " + FRAGE + "\n\n" + B(("3, 2, 1, 3", "Deine Noten: 2, 1, 3 / Durchschnitt: 2,00 / Beste Note: 1 / Schlechteste Note: 3 / Gut gemacht!"), ("2, 4, 5", "Deine Noten: 4, 5 / Durchschnitt: 4,50 / Beste Note: 4 / Schlechteste Note: 5 / Da geht noch was!")),
  "Setz die Schritte 1 bis 3 zusammen. Jede Auswertung bekommt ihre eigene Methode.",
  m=["Schreib eine Methode `Durchschnitt`, die eine Kommazahl zurückliefert.", "Schreib eine Methode `Beste`, die eine ganze Zahl zurückliefert.", "Schreib eine Methode `Schlechteste`, die eine ganze Zahl zurückliefert.", "Schreib eine Methode `Bewertung`, die einen Text zurückliefert."])

# ------------------------------------------------------------------ L26
X("l26a1", "Die geheime Zahl ist `37` (fest im Programm). Das Programm liest einen Tipp ein und sagt, ob er zu klein, zu groß oder richtig ist. " + FRAGE + "\n\n" + B(("10", "Zu klein!"), ("80", "Zu groß!"), ("37", "Richtig!")),
  "Drei Fälle: kleiner, größer, gleich.")
X("l26a2", "Die geheime Zahl ist wieder `37`. Das Programm liest so lange Tipps ein, bis sie stimmt. Nach jedem falschen Tipp sagt es `Zu klein!` oder `Zu groß!`. Am Ende nennt es die Anzahl der Versuche. " + FRAGE + "\n\n" + B(("50, 25, 37", "Zu groß! / Zu klein! / Richtig! Versuche: 3"), ("37", "Richtig! Versuche: 1")),
  "Ein Zähler zählt mit, die Schleife läuft bis zum Treffer.")
X("l26a3", "Wie vorher, aber robuster: Ist der Tipp keine Zahl, gibt das Programm `Das ist keine Zahl!` aus. Das zählt nicht als Versuch. Nach 5 Versuchen ohne Treffer endet das Spiel mit `Verloren! Die Zahl war 37.`. Beim Treffer: `Richtig! Versuche: …`. " + FRAGE + "\n\n" + B(("50, abc, 37", "Zu groß! / Das ist keine Zahl! / Richtig! Versuche: 2"), ("1, 2, 3, 4, 5, 37", "Zu klein! (fünfmal) / Verloren! Die Zahl war 37.")),
  "Die Schleife muss zwei Dinge beachten: einen Treffer und das Versuchslimit.",
  m=["Wandle die Eingabe sicher um, damit Buchstaben das Programm nicht zum Absturz bringen."])
X("l26a4", "Zwei Spieler: Der erste Spieler gibt die geheime Zahl ein, der zweite rät. Nach jedem falschen Tipp kommt `Zu klein!` oder `Zu groß!`. Am Ende sagt das Programm, nach wie vielen Versuchen der zweite Spieler richtig lag. " + FRAGE + "\n\n" + B(("42, dann 50, 40, 42", "Zu groß! / Zu klein! / Richtig nach 3 Versuchen!"), ("7, dann 7", "Richtig nach 1 Versuchen!"), ("90, dann 10, 90", "Zu klein! / Richtig nach 2 Versuchen!")),
  "Die erste Eingabe ist die geheime Zahl. Alle weiteren sind Tipps.")
X("l26a5", "Die geheime Zahl ist `37`. Das Programm liest Tipps ein und sagt nach jedem, wie nah er dran ist: genau getroffen `Treffer!`, höchstens 5 daneben `Heiß!`, höchstens 15 daneben `Warm.`, sonst `Kalt.`. Es endet beim Treffer. " + FRAGE + "\n\n" + B(("10, 30, 36, 37", "Kalt. / Warm. / Heiß! / Treffer!"), ("37", "Treffer!"), ("42, 52, 37", "Heiß! / Warm. / Treffer!")),
  "Entscheidend ist der Abstand zur geheimen Zahl, egal in welche Richtung.")
X("l26a6", "Die geheime Zahl ist `37`, sie liegt immer zwischen 1 und 100. Nach jedem falschen Tipp meldet das Programm den noch möglichen Bereich: `Die Zahl liegt zwischen … und ….`. Der Bereich wird mit jedem Tipp kleiner. Beim Treffer: `Richtig!`. " + FRAGE + "\n\n" + B(("50, 25, 37", "Die Zahl liegt zwischen 1 und 49. / Die Zahl liegt zwischen 26 und 49. / Richtig!"), ("30, 40, 37", "Die Zahl liegt zwischen 31 und 100. / Die Zahl liegt zwischen 31 und 39. / Richtig!")),
  "Ein falscher Tipp schließt alles darüber oder darunter inklusive des Tipps selbst aus.")
X("l26a7", "Die geheime Zahl ist `37`. Man startet mit 100 Punkten. Jeder falsche Tipp (`Zu klein!` oder `Zu groß!`) kostet 10 Punkte, aber die Punkte fallen nie unter 0. Beim Treffer: `Richtig! Punkte: …`. " + FRAGE + "\n\n" + B(("50, 25, 37", "Zu groß! / Zu klein! / Richtig! Punkte: 80"), ("37", "Richtig! Punkte: 100"), ("zwölf falsche Tipps, dann 37", "Richtig! Punkte: 0")),
  "Nach mehr als zehn Fehlern wäre der Wert negativ. Es gibt eine Methode, die das verhindert.")
X("l26a8", "Mehrere Runden: In jeder Runde gibt zuerst jemand die geheime Zahl ein, dann wird geraten (`Zu klein!` oder `Zu groß!` nach falschen Tipps). Danach heißt es `Runde …: Richtig nach … Versuchen!`. Dann fragt das Programm, ob noch eine Runde gespielt wird (`j` oder `n`). Am Ende steht `Danke fürs Spielen! Runden: …`. " + FRAGE + "\n\n" + B(("5, 3, 5, j, 8, 8, n", "Zu klein! / Runde 1: Richtig nach 2 Versuchen! / Runde 2: Richtig nach 1 Versuchen! / Danke fürs Spielen! Runden: 2"), ("10, 50, 20, 10, n", "Zu groß! / Zu groß! / Runde 1: Richtig nach 3 Versuchen! / Danke fürs Spielen! Runden: 1")),
  "Eine äußere Schleife für die Runden, eine innere fürs Raten.")

# ------------------------------------------------------------------ L27
MENUE = ("Das Programm liest in einer Schleife Befehle ein (jeder Befehl und jede Angabe steht in einer eigenen Eingabezeile) und endet bei `0` mit `Tschüss!`. Ein Menütext ist nicht nötig. " + FRAGE)

X("l27a1", "Baue eine Einkaufsliste mit Menü. " + MENUE + "\n\n- `1` liest einen Artikel ein und nimmt ihn auf: `Milch hinzugefügt.`\n- `2` zeigt die Liste nummeriert ab 1 (`1. Milch`). Ist sie leer, kommt nur `Die Liste ist leer.`\n- `0` beendet das Programm.\n\n**Beispiel** (Eingaben: 2, 1, Milch, 1, Brot, 2, 0):\n\n" + Z("Die Liste ist leer.", "Milch hinzugefügt.", "Brot hinzugefügt.", "1. Milch", "2. Brot", "Tschüss!"),
  "Die Liste kann selbst sagen, ob sie leer ist.",
  m=["Neue Artikel sollen in der Liste gespeichert werden.", "Ob die Liste leer ist, kann die Liste dir selbst sagen."])
X("l27a2", "Im Startcode sind Liste, Schleife und Befehl `0` schon da. Ergänze den Rest:\n\n- `1` liest einen Artikel ein. Steht er schon auf der Liste: `Milch ist schon auf der Liste.` Sonst wird er aufgenommen: `Milch hinzugefügt.`\n- `2` zeigt die Liste nummeriert ab 1, oder `Die Liste ist leer.`\n- `3` liest einen Artikel ein und entfernt ihn: `Milch entfernt.` Stand er nicht drauf: `Brot ist nicht auf der Liste.`\n- Jeder andere Befehl ergibt `Unbekannter Befehl.`\n\n**Beispiel** (Eingaben: 1, Milch, 1, Milch, 3, Brot, 9, 3, Milch, 2, 0):\n\n" + Z(
    "Milch hinzugefügt.", "Milch ist schon auf der Liste.", "Brot ist nicht auf der Liste.", "Unbekannter Befehl.", "Milch entfernt.", "Die Liste ist leer.", "Tschüss!") + "\n\n" + FRAGE,
  "Frag vor dem Aufnehmen und vor dem Entfernen, ob der Artikel auf der Liste steht.",
  m=["Prüf, ob der Artikel schon auf der Liste steht, bevor du ihn aufnimmst.", "Entferne Artikel mit einer passenden Listen-Methode."])
X("l27a3", "Im Startcode sind eine leere Liste und die Klasse `Artikel` (mit `Name` und `Menge`) schon da. Jeder neue Artikel ist ein eigenes Objekt. " + MENUE + "\n\n- `1` liest Name und Menge ein. Gibt es den Namen schon, wird die Menge dazugezählt: `Milch: jetzt 3`. Sonst wird ein neuer Artikel aufgenommen: `2x Milch hinzugefügt.`\n- `2` zeigt jeden Artikel als `Milch (3)` in der Reihenfolge der Aufnahme.\n\n**Beispiel** (Eingaben: 1, Milch, 2, 1, Eier, 10, 1, Milch, 1, 2, 0):\n\n" + Z(
    "2x Milch hinzugefügt.", "10x Eier hinzugefügt.", "Milch: jetzt 3", "Milch (3)", "Eier (10)", "Tschüss!"),
  "Such zuerst in der Liste nach dem Namen. Gefunden oder nicht: das entscheidet, was passiert.",
  m=["Jeder neue Artikel soll als eigenes Objekt entstehen.", "Ist der Artikel schon da, soll die neue Menge zur vorhandenen dazukommen."])
X("l27a4", "Erweitere die Einkaufsliste um einen Befehl. " + MENUE + "\n\n- `1` liest einen Artikel ein und nimmt ihn auf: `Milch hinzugefügt.`\n- `2` zeigt die Liste nummeriert ab 1, oder `Die Liste ist leer.`\n- `4` leert die ganze Liste: `Liste geleert.` Danach kann man wieder etwas aufnehmen.\n\n**Beispiel** (Eingaben: 1, Brot, 4, 1, Eier, 2, 0):\n\n" + Z("Brot hinzugefügt.", "Liste geleert.", "Eier hinzugefügt.", "1. Eier", "Tschüss!"),
  "Die Liste kann sich selbst leeren.")
X("l27a5", "Erweitere die Einkaufsliste um einen Befehl. " + MENUE + "\n\n- `1` liest einen Artikel ein und nimmt ihn auf: `Milch hinzugefügt.`\n- `2` zeigt die Liste nummeriert ab 1, oder `Die Liste ist leer.`\n- `5` nennt die Anzahl der Artikel: `2 Artikel auf der Liste.` Auch bei null Artikeln: `0 Artikel auf der Liste.`\n\n**Beispiel** (Eingaben: 1, Milch, 1, Brot, 5, 0):\n\n" + Z("Milch hinzugefügt.", "Brot hinzugefügt.", "2 Artikel auf der Liste.", "Tschüss!"),
  "Die Liste kennt ihre Anzahl.")
X("l27a6", "Erweitere die Einkaufsliste: Befehl `2` zeigt die Artikel jetzt alphabetisch sortiert. " + MENUE + "\n\n- `1` liest einen Artikel ein und nimmt ihn auf: `Milch hinzugefügt.` (in der Reihenfolge der Eingabe)\n- `2` zeigt die Liste alphabetisch sortiert und nummeriert ab 1, oder `Die Liste ist leer.`\n\n**Beispiel** (Eingaben: 1, Milch, 1, Brot, 1, Eier, 2, 0):\n\n" + Z(
    "Milch hinzugefügt.", "Brot hinzugefügt.", "Eier hinzugefügt.", "1. Brot", "2. Eier", "3. Milch", "Tschüss!"),
  "Sortieren kann die Liste selbst.")
X("l27a7", "Erweitere die Einkaufsliste um eine Suche. " + MENUE + "\n\n- `1` liest einen Artikel ein und nimmt ihn auf: `Milch hinzugefügt.`\n- `2` zeigt die Liste nummeriert ab 1 in der Reihenfolge der Aufnahme.\n- `6` liest einen Namen ein und sucht ihn: `Brot steht auf Platz 2.` (der erste Platz ist 1) oder `Käse ist nicht auf der Liste.`\n\n**Beispiel** (Eingaben: 1, Milch, 1, Brot, 6, Brot, 6, Käse, 0):\n\n" + Z(
    "Milch hinzugefügt.", "Brot hinzugefügt.", "Brot steht auf Platz 2.", "Käse ist nicht auf der Liste.", "Tschüss!"),
  "Die Liste kann dir den Platz eines Eintrags nennen. Denk daran, dass sie bei 0 zu zählen beginnt.")
X("l27a8", "Speichere Artikel als Objekte einer Klasse `Artikel` (mit `Name` und `Menge`) in einer Liste. Das Programm liest Befehle ein (jeder Befehl und jede Angabe in einer eigenen Eingabezeile). Ein Menütext ist nicht nötig. " + FRAGE + "\n\n- `1` liest Name und Menge ein und nimmt immer einen neuen Artikel auf: `2x Milch hinzugefügt.`\n- `2` zeigt jeden Artikel als `Milch (5)`, ohne Nummer.\n- `4` liest einen Namen und eine neue Menge ein (immer beide). Gefunden: die Menge wird ersetzt, nicht addiert: `Milch: jetzt 5`. Nicht gefunden: `Käse ist nicht auf der Liste.`\n- `0` beendet mit `Tschüss! 2 Artikel, 15 Stück insgesamt.` (Anzahl der Artikel und Summe aller Mengen).\n\n**Beispiel** (Eingaben: 1, Milch, 2, 1, Eier, 10, 4, Milch, 5, 4, Käse, 1, 2, 0):\n\n" + Z(
    "2x Milch hinzugefügt.", "10x Eier hinzugefügt.", "Milch: jetzt 5", "Käse ist nicht auf der Liste.", "Milch (5)", "Eier (10)", "Tschüss! 2 Artikel, 15 Stück insgesamt."),
  "Bei Befehl 4 liest du immer beide Angaben, auch wenn der Name nicht gefunden wird, sonst gerät die Eingabe durcheinander.",
  m=["Schreib die Klasse `Artikel`.", "Speichere die Artikel als Objekte in einer Liste."])
