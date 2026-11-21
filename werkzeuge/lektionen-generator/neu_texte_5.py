"""Neue Aufgabentexte Lektion 19–22."""
from neu_texte import X, Z, B, FRAGE

SICHER = "Die Eingabe soll das Programm nie zum Absturz bringen, auch wenn jemand Unsinn eintippt."

# ------------------------------------------------------------------ L19
X("l19a1", "Das Programm liest eine Eingabe. Ist es eine ganze Zahl, gibt es ihr Doppeltes aus, sonst `Ungültige Eingabe`. " + SICHER + " " + FRAGE + "\n\n" + B(("7", "Das Doppelte ist 14"), ("sieben", "Ungültige Eingabe"), ("(nur Enter)", "Ungültige Eingabe")),
  "Es gibt eine Methode, die versucht umzuwandeln und dir sagt, ob es geklappt hat.")
X("l19a2", "Das Programm fragt nach dem Alter. Solange die Eingabe keine ganze Zahl ist, meldet es `Bitte eine Zahl eingeben!` und fragt erneut. Am Ende nennt es das Alter. " + FRAGE + "\n\n" + B(("dreißig, 30", "Bitte eine Zahl eingeben! / Du bist 30 Jahre alt."), ("5", "Du bist 5 Jahre alt."), ("x, y, 8", "Bitte eine Zahl eingeben! (zweimal) / Du bist 8 Jahre alt.")),
  "Die Schleife läuft, solange die Umwandlung nicht klappt.")
X("l19a3", "Das Programm liest genau fünf Eingaben. Die gültigen ganzen Zahlen werden addiert, die ungültigen nur gezählt. " + FRAGE + "\n\n" + B(("4, x, 10, zwei, 6", "Summe: 20 / Ungültig: 2"), ("1, 2, 3, 4, 5", "Summe: 15 / Ungültig: 0")),
  "Bei jeder Eingabe entscheidest du: klappt die Umwandlung oder nicht?")
X("l19a4", "Das Programm liest ein Alter ein und sagt, wie alt die Person in 10 Jahren ist. Ist die Eingabe keine ganze Zahl, sagt es `Das war keine Zahl.`. " + SICHER + " " + FRAGE + "\n\n" + B(("15", "In 10 Jahren bist du 25."), ("fünfzehn", "Das war keine Zahl."), ("0", "In 10 Jahren bist du 10.")),
  "Prüfe, ob die Umwandlung klappt, bevor du rechnest.")
X("l19a5", "Das Programm liest eine Kommazahl ein und gibt ihr Doppeltes aus. Ist die Eingabe keine Kommazahl, sagt es `Keine gültige Kommazahl.`. " + FRAGE + "\n\n" + B(("2,5", "Das Doppelte ist 5"), ("0,25", "Das Doppelte ist 0,5"), ("abc", "Keine gültige Kommazahl.")),
  "Es gibt die sichere Umwandlung auch für Kommazahlen.")
X("l19a6", "Das Programm liest zwei Eingaben und addiert sie, wenn beide ganze Zahlen sind. Ist mindestens eine ungültig, meldet es `Ungültige Eingabe`. " + FRAGE + "\n\n" + B(("5, 7", "Summe: 12"), ("5, x", "Ungültige Eingabe"), ("x, 7", "Ungültige Eingabe"), ("-4, 3", "Summe: -1")),
  "Beide Eingaben müssen gelesen werden und beide müssen stimmen.")
X("l19a7", "Das Programm verlangt eine Zahl von 1 bis 10 und fragt so lange nach, bis sie stimmt. Ist die Eingabe keine Zahl, meldet es `Keine Zahl!`. Liegt sie außerhalb von 1 bis 10, meldet es `Bitte eine Zahl von 1 bis 10!`. Am Ende bedankt es sich mit `Danke, du hast … gewählt.`. " + FRAGE + "\n\n" + B(("abc, 15, 7", "Keine Zahl! / Bitte eine Zahl von 1 bis 10! / Danke, du hast 7 gewählt."), ("10", "Danke, du hast 10 gewählt."), ("0, 1", "Bitte eine Zahl von 1 bis 10! / Danke, du hast 1 gewählt.")),
  "Zwei Dinge sind zu prüfen: erst, ob es eine Zahl ist, dann, ob sie im Bereich liegt.")
X("l19a8", "Das Programm liest so lange Eingaben, bis `fertig` kommt. Gültige ganze Zahlen werden für den Durchschnitt gesammelt. Zu jeder ungültigen Eingabe meldet es `Ungültig: …` mit dem eingegebenen Text. Am Ende gibt es den Durchschnitt aus (zwei Nachkommastellen). " + FRAGE + "\n\n" + B(("3, abc, 6, fertig", "Ungültig: abc / Durchschnitt: 4,50"), ("10, fertig", "Durchschnitt: 10,00"), ("1, x, 2,5, 3, fertig", "Ungültig: x / Ungültig: 2,5 / Durchschnitt: 2,00")),
  "Eine Kommazahl wie 2,5 ist für den Zahlentyp ganze Zahl ungültig. Summe und Anzahl brauchst du beide.")

# ------------------------------------------------------------------ L20
X("l20a1", "Schreib eine Methode `Hypotenuse`, die die beiden Katheten eines rechtwinkligen Dreiecks (Kommazahlen) bekommt und die längste Seite zurückliefert (Satz des Pythagoras). Gib die Ergebnisse für 3 und 4 sowie für 5 und 12 aus:\n\n" + Z("5", "13"),
  "Die Wurzel aus der Summe der Quadrate. Die Mathe-Klasse hilft dir dabei.")
X("l20a2", "Das Programm liest den Radius eines Kreises ein (Kommazahl) und gibt Umfang und Fläche mit zwei Nachkommastellen aus. " + FRAGE + "\n\n" + B(("2", "Umfang: 12,57 / Fläche: 12,57"), ("5", "Umfang: 31,42 / Fläche: 78,54"), ("1,5", "Umfang: 9,42 / Fläche: 7,07")),
  "Du brauchst die Kreiszahl, und die Mathe-Klasse kennt sie schon genau.")
X("l20a3", "Jede Person isst drei Stücke Pizza, eine Pizza hat 8 Stücke. Das Programm liest die Anzahl der Personen ein und gibt aus, wie viele Pizzen bestellt werden müssen (es darf nichts fehlen, es wird also aufgerundet). " + FRAGE + "\n\n" + B(("10", "Anzahl Pizzen: 4"), ("8", "Anzahl Pizzen: 3"), ("1", "Anzahl Pizzen: 1")),
  "Wenn die Division nicht aufgeht, brauchst du eine volle Pizza mehr. Dafür gibt es eine Methode.")
X("l20a4", "Das Programm liest zwei ganze Zahlen ein und gibt ihren Abstand auf dem Zahlenstrahl aus. Die Reihenfolge der Eingabe spielt keine Rolle. " + FRAGE + "\n\n" + B(("3 und 10", "Abstand: 7"), ("10 und 3", "Abstand: 7"), ("-2 und 2", "Abstand: 4")),
  "Der Abstand ist nie negativ. Es gibt eine Methode, die das Vorzeichen entfernt.")
X("l20a5", "Das Programm liest drei ganze Zahlen ein und gibt die größte aus. " + FRAGE + "\n\n" + B(("4, 9, 2", "Die größte Zahl ist 9."), ("12, 3, 5", "Die größte Zahl ist 12."), ("1, 2, 7", "Die größte Zahl ist 7.")),
  "Die Mathe-Klasse vergleicht zwei Zahlen für dich. Wie kommst du von zwei auf drei?")
X("l20a6", "Das Programm liest eine Kommazahl ein und gibt sie auf drei Arten aus: auf eine Nachkommastelle gerundet, immer abgerundet und immer aufgerundet. " + FRAGE + "\n\n" + B(("3,14159", "Gerundet: 3,1 / Abgerundet: 3 / Aufgerundet: 4"), ("7,86", "Gerundet: 7,9 / Abgerundet: 7 / Aufgerundet: 8")),
  "Die Mathe-Klasse hat für jede Art des Rundens eine eigene Methode.")
X("l20a7", "Das Programm liest eine Basis und einen Exponenten (ganze Zahlen) ein und gibt die Potenz aus. " + FRAGE + "\n\n" + B(("2 und 10", "2 hoch 10 = 1024"), ("3 und 4", "3 hoch 4 = 81"), ("5 und 0", "5 hoch 0 = 1")),
  "Ausprobieren in einer Schleife ist unnötig. Die Mathe-Klasse hat eine Methode fürs Potenzieren.")
X("l20a8", "Das Programm liest den Radius eines Kreises ein (Kommazahl) und gibt zuerst die Fläche, dann den Umfang aus, jeweils mit zwei Nachkommastellen. " + FRAGE + "\n\n" + B(("5", "Fläche: 78,54 / Umfang: 31,42"), ("1", "Fläche: 3,14 / Umfang: 6,28"), ("2,5", "Fläche: 19,63 / Umfang: 15,71")),
  "Beide Formeln brauchen die Kreiszahl. Du musst sie nicht selbst eintippen.")

# ------------------------------------------------------------------ L21
X("l21a1", "Im Startcode werden zwei `Auto`-Objekte benutzt, aber die Klasse ist noch leer. Ergänze sie: Ein Auto hat eine Marke (Text) und eine Leistung in PS (ganze Zahl) und kann sich mit einer Methode `Info` vorstellen. Beide Felder heißen `Marke` und `PS`.\n\n" + Z("VW mit 150 PS", "Tesla mit 400 PS"),
  "Alles, was die Hauptdatei benutzt, muss in der Klasse vorhanden und von außen erreichbar sein.",
  m=["Es fehlt das Feld `Marke` (Text).", "Es fehlt das Feld `PS` (ganze Zahl).", "Es fehlt die Methode `Info`."])
X("l21a2", "Schreib eine Klasse `Rechteck` mit den Feldern `Breite` und `Hoehe` (Kommazahlen) und einer Methode `Flaeche`, die die Fläche zurückliefert. Erzeuge ein Rechteck mit Breite 4 und Höhe 2,5 und gib die Fläche aus:\n\n" + Z("Fläche: 10"),
  "Erst die Klasse bauen, dann ein Objekt davon erzeugen und die Felder füllen.",
  m=["Erzeuge im Hauptprogramm ein Rechteck-Objekt."])
X("l21a3", "Im Startcode wird ein `Zaehler`-Objekt benutzt, aber die Klasse fehlt. Schreib sie: Sie hat ein Feld `Wert` (ganze Zahl), eine Methode `Erhoehen`, die den Wert um 1 erhöht, und eine Methode `Zuruecksetzen`, die ihn wieder auf 0 setzt.\n\n" + Z("3", "1"),
  "Der Wert gehört dem Objekt, nicht der Methode.",
  m=["Es fehlt die Methode `Erhoehen`.", "Es fehlt die Methode `Zuruecksetzen`."])
X("l21a4", "Schreib eine Klasse `Hund` mit einem Feld `Name` (Text) und einer Methode `Bellen`, die `<Name> sagt: Wuff!` ausgibt. Erzeuge zwei Hunde, `Bello` und `Rex`, und lass beide bellen:\n\n" + Z("Bello sagt: Wuff!", "Rex sagt: Wuff!"),
  "Jedes Objekt hat seinen eigenen Namen.",
  m=["Es fehlt die Klasse `Hund`.", "Die Klasse braucht ein Feld `Name`.", "Die Klasse braucht eine Methode `Bellen`.", "Erzeuge zwei Hund-Objekte."])
X("l21a5", "Schreib eine Klasse `Buch` mit den Feldern `Titel` (Text) und `Seiten` (ganze Zahl) und einer Methode `Info`. Erzeuge `Der Hobbit` (310 Seiten) und `Momo` (304 Seiten) und lass beide Info ausgeben:\n\n" + Z("Der Hobbit hat 310 Seiten.", "Momo hat 304 Seiten."),
  "Eine Klasse, zwei Objekte mit unterschiedlichen Werten.",
  m=["Es fehlt die Klasse `Buch`.", "Die Klasse braucht ein Feld `Titel`.", "Die Klasse braucht ein Feld `Seiten`.", "Die Klasse braucht eine Methode `Info`."])
X("l21a6", "Schreib eine Klasse `Lampe` mit einem Feld `An` (Wahrheitswert) und zwei Methoden: `Schalten` kehrt den Zustand um (an wird aus, aus wird an), `Zustand` gibt ihn aus. Die Lampe ist anfangs aus. Rufe `Zustand` auf, schalte, rufe `Zustand` auf, schalte, rufe `Zustand` auf:\n\n" + Z("Die Lampe ist aus.", "Die Lampe ist an.", "Die Lampe ist aus."),
  "Ein Wahrheitswert kann sich umkehren.",
  m=["Es fehlt die Klasse `Lampe`.", "Die Klasse braucht ein Feld `An`.", "Die Klasse braucht eine Methode `Schalten`.", "Die Klasse braucht eine Methode `Zustand`."])
X("l21a7", "Schreib eine Klasse `Kreis` mit dem Feld `Radius` (Kommazahl) und einer Methode `Flaeche`, die die Fläche zurückliefert. Erzeuge zwei Kreise mit Radius 1 und 2 und gib ihre Flächen mit zwei Nachkommastellen aus:\n\n" + Z("Fläche: 3,14", "Fläche: 12,57"),
  "Die Methode rechnet mit dem Radius des Objekts, zu dem sie gehört.",
  m=["Es fehlt die Klasse `Kreis`.", "Die Klasse braucht ein Feld `Radius` (Kommazahl).", "Die Klasse braucht eine Methode `Flaeche`, die eine Kommazahl zurückliefert."])
X("l21a8", "Schreib eine Klasse `Warenkorb` mit den Feldern `Anzahl` und `Summe` und einer Methode `Hinzufuegen`, die einen Preis (Kommazahl) bekommt: Sie erhöht die Anzahl um 1 und die Summe um den Preis. Lege Artikel für 2,99, 4,50 und 10 Euro hinein und gib Anzahl und Summe aus (zwei Nachkommastellen). Die Werte müssen aus dem Warenkorb kommen:\n\n" + Z("3 Artikel, Summe: 17,49 Euro"),
  "Das Objekt merkt sich selbst, wie viele Artikel schon drin sind.",
  m=["Es fehlt die Klasse `Warenkorb`.", "Die Klasse braucht eine Methode `Hinzufuegen` mit einem Preis.", "Lege alle drei Artikel über die Methode in den Warenkorb.", "Die Summe soll aus dem Warenkorb kommen."])

# ------------------------------------------------------------------ L22
X("l22a1", "Im Startcode werden zwei `Schueler`-Objekte gleich mit Name und Klasse erzeugt. Ergänze die Klasse um das, was dafür nötig ist, damit die Werte beim Erzeugen gesetzt werden:\n\n" + Z("Mia geht in die 7b", "Tim geht in die 9a"),
  "Beim Erzeugen eines Objekts läuft eine besondere Methode ab, die so heißt wie die Klasse.",
  m=["Die Klasse braucht einen Konstruktor, der Name und Klasse annimmt."])
X("l22a2", "Im Startcode wird ein `Konto` benutzt, die Klasse ist aber leer. Schreib sie: Beim Erzeugen bekommt das Konto einen Inhaber (Text) und einen Startbetrag (Kommazahl). `Einzahlen` erhöht den Kontostand. `Abheben` verringert ihn, aber nur, wenn genug Geld da ist, sonst gibt es `Nicht genug Geld!` aus. Die Felder heißen `Inhaber` und `Kontostand`.\n\n" + Z("Nicht genug Geld!", "Lea: 120,00 Euro"),
  "Vor dem Abheben prüfst du, ob der Betrag überhaupt gedeckt ist.",
  m=["Die Klasse braucht einen Konstruktor mit Inhaber und Startbetrag.", "Es fehlt die Methode `Einzahlen`.", "Es fehlt die Methode `Abheben`."])
X("l22a3", "Im Startcode werden zwei `Punkt`-Objekte erzeugt. Schreib die Klasse: Sie hat die Felder `X` und `Y` (ganze Zahlen), einen Konstruktor, dessen Parameter genauso heißen wie die Felder, und eine Methode `AlsText`, die den Punkt als Text zurückliefert:\n\n" + Z("(3|4)", "(-1|7)"),
  "Wenn Parameter und Feld gleich heißen, musst du dem Computer sagen, welches du meinst.",
  m=["Feld und Parameter heißen gleich. Die Lektion zeigt, wie du das Feld des Objekts trotzdem ansprichst."])
X("l22a4", "Schreib eine Klasse `Tier` mit den Feldern `Name` und `Art` (Text), einem Konstruktor, der beide bekommt, und einer Methode `Vorstellen`. Erzeuge `Mimi` (Katze) und `Rex` (Schildkröte) direkt mit ihren Werten:\n\n" + Z("Mimi ist eine Katze.", "Rex ist eine Schildkröte."),
  "Die Werte werden schon beim Erzeugen übergeben. Du musst sie nicht danach einzeln setzen.",
  m=["Die Klasse braucht einen Konstruktor mit Name und Art.", "Die Klasse braucht eine Methode `Vorstellen`.", "Erzeuge Mimi direkt mit ihren Werten."])
X("l22a5", "Schreib eine Klasse `Rechteck`, die beim Erzeugen Breite und Höhe (Kommazahlen) bekommt, und eine Methode `Flaeche`, die die Fläche zurückliefert. Erzeuge ein 3 x 4 und ein 2,5 x 3 Rechteck:\n\n" + Z("Fläche: 12", "Fläche: 7,5"),
  "Die Werte gehören in den Konstruktor, nicht in einzelne Befehle danach.",
  m=["Die Klasse braucht einen Konstruktor mit Breite und Höhe.", "Die Klasse braucht eine Methode `Flaeche`, die eine Kommazahl zurückliefert."])
X("l22a6", "Schreib eine Klasse `Uhrzeit` mit den Feldern `Stunde` und `Minute` (ganze Zahlen) und einer Methode `Anzeigen`. Man kann sie auf zwei Arten erzeugen: mit Stunde und Minute oder nur mit der Stunde (dann ist die Minute 0). Erzeuge 8:30 und 14 Uhr:\n\n" + Z("8 Uhr 30", "14 Uhr 0"),
  "Eine Klasse darf mehrere Konstruktoren mit unterschiedlichen Parametern haben.",
  m=["Es fehlt der Konstruktor mit Stunde und Minute.", "Es fehlt der Konstruktor nur mit der Stunde.", "Erzeuge 14 Uhr mit dem Konstruktor, der nur die Stunde bekommt."])
X("l22a7", "Schreib eine Klasse `Konto` für ein Taschengeldkonto. Beim Erzeugen bekommt es nur den Inhaber (Text) und startet automatisch mit 10 Euro (ganze Zahl). `Einzahlen` erhöht den Betrag, `Anzeigen` gibt ihn aus. Erzeuge ein Konto für `Ali`, zeige es an, zahle 25 Euro ein und zeige es noch einmal an:\n\n" + Z("Ali hat 10 Euro.", "Ali hat 35 Euro."),
  "Der Startbetrag kommt nicht von außen, der Konstruktor legt ihn selbst fest.",
  m=["Die Klasse braucht einen Konstruktor, der nur den Inhaber bekommt.", "Es fehlt die Methode `Einzahlen`.", "Es fehlt die Methode `Anzeigen`."])
X("l22a8", "Schreib eine Klasse `Bruch` mit Zähler und Nenner (ganze Zahlen), die beim Erzeugen gesetzt werden. Sie hat eine Methode `AlsText`, die den Bruch als `Zähler/Nenner` zurückliefert, und eine Methode `Mal`, die einen anderen Bruch bekommt und einen neuen Bruch zurückliefert (Zähler mal Zähler, Nenner mal Nenner, nicht gekürzt). Multipliziere 1/2 mit 3/4:\n\n" + Z("1/2 * 3/4 = 3/8"),
  "Die Methode `Mal` liefert ein neues Objekt der eigenen Klasse zurück.",
  m=["Die Klasse braucht einen Konstruktor mit Zähler und Nenner.", "Die Klasse braucht eine Methode `AlsText`, die einen Text zurückliefert.", "Die Klasse braucht eine Methode `Mal`, die einen Bruch annimmt und einen Bruch zurückliefert.", "Lass das Ergebnis von der Methode `Mal` ausrechnen."])
