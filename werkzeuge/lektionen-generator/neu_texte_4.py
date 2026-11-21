"""Neue Aufgabentexte Lektion 15–18."""
from neu_texte import X, Z, B, FRAGE

# ------------------------------------------------------------------ L15
X("l15a1", "Lege eine Liste deiner Lieblingsfilme an und nimm nacheinander `Matrix`, `Shrek` und `Up` auf. Gib zuerst aus, wie viele Filme es sind (das soll die Liste selbst sagen), und danach jeden Film in einer eigenen Zeile:\n\n" + Z("3 Filme:", "Matrix", "Shrek", "Up"),
  "Eine Liste wächst, wenn du etwas hinzufügst. Ihre Größe kennt sie selbst.")
X("l15a2", "Das Programm liest so lange ganze Zahlen ein, bis `0` kommt (die `0` selbst zählt nicht), und speichert sie in einer Liste. Danach gibt es Anzahl und Summe aus und die Zahlen aufsteigend sortiert in einer Zeile. " + FRAGE + "\n\n" + B(("5, 2, 8, 0", "Anzahl: 3 / Summe: 15 / Sortiert: 2 5 8"), ("4, 0", "Anzahl: 1 / Summe: 4 / Sortiert: 4")),
  "Die Liste kann sich selbst sortieren.")
X("l15a3", "Im Startcode ist eine Gästeliste mit `Ana`, `Ben` und `Cem` vorgegeben. Das Programm liest einen Namen ein. Steht er auf der Liste, wird der Gast ausgeladen (von der Liste gestrichen). Sonst meldet das Programm, dass er nicht auf der Liste steht. Zum Schluss steht, wie viele Gäste noch kommen. " + FRAGE + "\n\n" + B(("Ben", "Ben wurde ausgeladen. / Noch 2 Gäste"), ("Zoe", "Zoe steht nicht auf der Liste. / Noch 3 Gäste")),
  "Erst fragen, ob der Name vorkommt. Dann entscheiden.")
X("l15a4", "Lege eine Wunschliste an und nimm nacheinander `Fahrrad`, `Buch` und `Spiel` auf. Dann überlegst du es dir anders und streichst das Buch wieder. Gib die verbliebene Anzahl und die Wünsche aus. Die Liste soll `wuensche` heißen:\n\n" + Z("Wünsche: 2", "- Fahrrad", "- Spiel"),
  "Streichen geht über den Inhalt, du brauchst keine Platznummer.")
X("l15a5", "Das Programm liest vier Namen ein und gibt sie alphabetisch sortiert aus, jeden in einer eigenen Zeile. " + FRAGE + "\n\n" + B(("Tom, Ana, Mia, Ben", "Ana / Ben / Mia / Tom"), ("Lea, Emil, Dora, Cem", "Cem / Dora / Emil / Lea")),
  "Du musst nicht selbst sortieren. Die Liste kann das.")
X("l15a6", "Das Programm liest so lange ganze Zahlen ein, bis `0` kommt (zählt nicht mit), und sammelt sie in einer Liste. Danach gibt es die erste und die letzte Zahl sowie die Anzahl aus. " + FRAGE + "\n\n" + B(("4, 7, 9, 0", "Erste Zahl: 4 / Letzte Zahl: 9 / Anzahl: 3"), ("12, 5, 0", "Erste Zahl: 12 / Letzte Zahl: 5 / Anzahl: 2")),
  "Der Platz der letzten Zahl hängt von der Anzahl ab.")
X("l15a7", "Das Programm liest fünf Wörter ein, merkt sich aber jedes Wort nur einmal. Danach gibt es aus, wie viele verschiedene Wörter es gab, und listet sie in der Reihenfolge des ersten Auftretens auf. " + FRAGE + "\n\n" + B(("Hund, Katze, Hund, Maus, Katze", "Verschiedene Wörter: 3 / Hund / Katze / Maus"), ("A, B, C, D, E", "Verschiedene Wörter: 5 / A / B / C / D / E")),
  "Bevor du ein Wort aufnimmst, schau nach, ob es schon da ist.")
X("l15a8", "Im Startcode ist eine To-do-Liste mit `Einkaufen`, `Hausaufgaben` und `Zimmer aufräumen` vorgegeben (nummeriert ab 1). Das Programm liest die Nummer einer erledigten Aufgabe ein, meldet sie und streicht sie von der Liste. Danach gibt es die übrigen Aufgaben nummeriert ab 1 aus. " + FRAGE + "\n\n" + B(("2", "Erledigt: Hausaufgaben / 1. Einkaufen / 2. Zimmer aufräumen"), ("1", "Erledigt: Einkaufen / 1. Hausaufgaben / 2. Zimmer aufräumen")),
  "Die Nummer für den Benutzer beginnt bei 1, die Plätze der Liste bei 0. Streich die Aufgabe über ihren Platz.")

# ------------------------------------------------------------------ L16
X("l16a1", "Schreib eine eigene Methode `Willkommen`, die `Willkommen im Kurs!` ausgibt (sie braucht keine Werte und liefert keine zurück). Rufe sie im Hauptprogramm zweimal auf:\n\n" + Z("Willkommen im Kurs!", "Willkommen im Kurs!"),
  "Der Text steht nur einmal im Code, in der Methode. Ruf sie danach zweimal auf.",
  m=["Schreib eine Methode namens `Willkommen`, die keine Werte annimmt."])
X("l16a2", "Im Startcode wird eine Methode `Quadrat` schon dreimal aufgerufen. Schreib sie: Sie bekommt eine ganze Zahl und gibt die Zeile aus.\n\n" + Z("3 zum Quadrat ist 9", "5 zum Quadrat ist 25", "12 zum Quadrat ist 144"),
  "Die Methode bekommt die Zahl und berechnet das Ergebnis selbst.",
  m=["Schreib eine Methode namens `Quadrat`, die eine ganze Zahl annimmt."])
X("l16a3", "Im Startcode wird die Methode `Rahmen` mit je einem Text aufgerufen. Schreib sie: Sie zeichnet den Text mit einem Rahmen aus Sternen. Der Rahmen passt sich der Länge des Textes an:\n\n" + Z("*********", "* Hallo *", "*********", "******", "* C# *", "******"),
  "Die Rahmenlinie ist so lang wie der Text plus 4 Zeichen. Du kannst Text auch mehrfach hintereinander setzen lassen.",
  m=["Schreib eine Methode namens `Rahmen`, die einen Text annimmt."])
X("l16a4", "Schreib eine Methode `Linie` ohne Werte, die eine Trennlinie aus 10 Strichen ausgibt. Rufe sie zweimal auf, mit der Zeile `Inhalt` dazwischen:\n\n" + Z("----------", "Inhalt", "----------"),
  "Dieselbe Methode darf beliebig oft aufgerufen werden.",
  m=["Schreib eine Methode namens `Linie`, die keine Werte annimmt.", "Ruf `Linie` zweimal auf."])
X("l16a5", "Schreib eine Methode `Begruesse`, die einen Namen (Text, Parameter `name`) bekommt und diesen begrüßt. Rufe sie für `Ana` und `Ben` auf:\n\n" + Z("Hallo, Ana!", "Hallo, Ben!"),
  "Der Name wird beim Aufruf übergeben und in der Methode benutzt.",
  m=["Schreib eine Methode `Begruesse`, die einen Text als Parameter `name` annimmt.", "Ruf die Methode für Ana auf.", "Ruf die Methode für Ben auf."])
X("l16a6", "Schreib eine Methode `Rechteck`, die Breite und Höhe (zwei ganze Zahlen) bekommt und einen Satz mit der Fläche ausgibt. Rufe sie mit 3 und 4 sowie mit 5 und 2 auf:\n\n" + Z("Ein 3 x 4 Rechteck hat die Fläche 12.", "Ein 5 x 2 Rechteck hat die Fläche 10."),
  "Zwei Parameter, durch Komma getrennt.",
  m=["Schreib eine Methode `Rechteck`, die zwei ganze Zahlen annimmt.", "Ruf die Methode mit 3 und 4 auf.", "Ruf die Methode mit 5 und 2 auf."])
X("l16a7", "Schreib eine Methode `Wiederhole`, die einen Text und eine Anzahl (ganze Zahl) bekommt und den Text so oft in einer Zeile ausgibt, durch Leerzeichen getrennt. Rufe sie mit `Hey` und 3 sowie `Ho` und 2 auf:\n\n" + Z("Hey Hey Hey", "Ho Ho"),
  "In der Methode brauchst du eine Schleife, die so oft läuft, wie die Anzahl sagt.",
  m=["Schreib eine Methode `Wiederhole` mit einem Text und einer ganzen Zahl als Parameter.", "Ruf die Methode mit Hey und 3 auf.", "Ruf die Methode mit Ho und 2 auf."])
X("l16a8", "Schreib zwei Methoden: `Linie` (ohne Werte) gibt eine Trennlinie aus 10 Strichen aus. `Kopfzeile` bekommt einen Titel und ruft `Linie` auf, gibt den Titel aus und ruft `Linie` noch einmal auf. Rufe `Kopfzeile` für `Einkauf` und `Termine` auf:\n\n" + Z("----------", "Einkauf", "----------", "----------", "Termine", "----------"),
  "Eine Methode darf eine andere aufrufen.",
  m=["Schreib eine Methode namens `Linie`.", "Schreib eine Methode `Kopfzeile`, die einen Text annimmt.", "Ruf `Kopfzeile` für Einkauf auf.", "Ruf `Kopfzeile` für Termine auf."])

# ------------------------------------------------------------------ L17
X("l17a1", "Im Startcode wird eine Methode `Max` mit je zwei ganzen Zahlen aufgerufen und das Ergebnis ausgegeben. Schreib die Methode: Sie liefert die größere der beiden Zahlen zurück (sie gibt selbst nichts aus):\n\n" + Z("9", "12", "5"),
  "Die Methode braucht einen Rückgabetyp, und sie gibt ihr Ergebnis an die aufrufende Stelle zurück.")
X("l17a2", "Schreib eine Methode `Brutto`, die einen Nettopreis (Kommazahl) bekommt und den Bruttopreis mit 19 % Steuer zurückliefert. Gib damit beide Zeilen aus (zwei Nachkommastellen):\n\n" + Z("100 Euro netto = 119,00 Euro brutto", "50 Euro netto = 59,50 Euro brutto"),
  "Der Faktor für 19 % Aufschlag ist 1,19.",
  m=["Schreib eine Methode `Brutto`, die eine Kommazahl annimmt und eine Kommazahl zurückliefert."])
X("l17a3", "Schreib eine Methode `IstPrimzahl`, die eine ganze Zahl bekommt und `true` oder `false` zurückliefert. Eine Primzahl ist größer als 1 und nur durch 1 und sich selbst teilbar. Das Hauptprogramm liest eine Zahl ein und meldet das Ergebnis. " + FRAGE + "\n\n" + B(("17", "17 ist eine Primzahl"), ("12", "12 ist keine Primzahl"), ("2", "2 ist eine Primzahl"), ("1", "1 ist keine Primzahl"), ("97", "97 ist eine Primzahl")),
  "Probiere, ob eine kleinere Zahl ohne Rest in n passt. Sobald das einmal klappt, ist es keine Primzahl.",
  m=["Schreib eine Methode `IstPrimzahl`, die eine ganze Zahl annimmt und einen Wahrheitswert zurückliefert."])
X("l17a4", "Schreib eine Methode `Fakultaet`, die eine ganze Zahl n bekommt und das Produkt aller Zahlen von 1 bis n zurückliefert. Gib die Fakultäten von 1 bis 10 aus:\n\n" + Z("1! = 1", "2! = 2", "3! = 6", "…", "10! = 3628800"),
  "Multipliziere in einer Schleife immer weiter. Ein ganzzahliger Typ mit großem Wertebereich schadet nicht.",
  m=["Schreib eine Methode `Fakultaet`."])
X("l17a5", "Schreib eine Methode `Quadrat`, die eine ganze Zahl bekommt und deren Quadrat zurückliefert. Berechne damit die Summe der Quadrate von 3 und 4 und gib sie aus:\n\n" + Z("3² + 4² = 25"),
  "Die Methode liefert ein Ergebnis. Du kannst zwei Aufrufe direkt zusammenrechnen.",
  m=["Schreib eine Methode `Quadrat`, die eine ganze Zahl annimmt und eine zurückliefert.", "Die Methode soll ihr Ergebnis zurückgeben, nicht selbst ausgeben.", "Berechne die Summe aus zwei Aufrufen von `Quadrat`."])
X("l17a6", "Schreib eine Methode `DarfFahren`, die ein Alter (ganze Zahl) bekommt und zurückliefert, ob die Person Auto fahren darf (ab 18). Das Hauptprogramm liest das Alter ein und gibt das Ergebnis aus. " + FRAGE + "\n\n" + B(("17", "Darf Auto fahren: False"), ("18", "Darf Auto fahren: True"), ("40", "Darf Auto fahren: True")),
  "Ein Vergleich ergibt selbst schon einen Wahrheitswert.",
  m=["Schreib eine Methode `DarfFahren`, die eine ganze Zahl annimmt und einen Wahrheitswert zurückliefert.", "Die Methode soll ihr Ergebnis zurückgeben, nicht selbst ausgeben."])
X("l17a7", "Schreib eine Methode `ZuFahrenheit`, die eine Temperatur in Celsius (Kommazahl) bekommt und die Fahrenheit-Temperatur zurückliefert (Celsius mal 1,8 plus 32). Gib damit drei Zeilen aus, mit einer Nachkommastelle:\n\n" + Z("0 °C = 32,0 °F", "37 °C = 98,6 °F", "100 °C = 212,0 °F"),
  "Die Methode wird für alle drei Temperaturen aufgerufen.",
  m=["Schreib eine Methode `ZuFahrenheit`, die eine Kommazahl annimmt und eine zurückliefert.", "Die Methode soll ihr Ergebnis zurückgeben, nicht selbst ausgeben.", "Ruf die Methode für alle drei Temperaturen auf."])
X("l17a8", "Schreib eine Methode `Bewertung`, die Punkte (ganze Zahl) bekommt und einen Text zurückliefert: ab 90 `sehr gut`, ab 50 `bestanden`, sonst `nicht bestanden`. Das Hauptprogramm liest die Punkte ein und gibt das Ergebnis aus. " + FRAGE + "\n\n" + B(("95", "Bewertung: sehr gut"), ("50", "Bewertung: bestanden"), ("20", "Bewertung: nicht bestanden")),
  "Die Methode liefert einen Text. Das Hauptprogramm baut ihn in den Satz ein.",
  m=["Schreib eine Methode `Bewertung`, die eine ganze Zahl annimmt und einen Text zurückliefert.", "Die Methode soll ihr Ergebnis zurückgeben, nicht selbst ausgeben."])

# ------------------------------------------------------------------ L18
X("l18a1", "Das Programm liest einen Namen ein, der unordentlich geschrieben sein kann (Leerzeichen am Rand, falsche Groß- und Kleinschreibung), und gibt ihn ordentlich aus: erster Buchstabe groß, Rest klein, ohne Leerzeichen am Rand. " + FRAGE + "\n\n" + B(("  mAx  ", "Max"), ("ANNA", "Anna"), ("lu", "Lu")),
  "Teile den Namen in den ersten Buchstaben und den Rest auf und bearbeite beide Teile getrennt.")
X("l18a2", "Das Programm liest ein Wort ein und sagt, ob es ein Palindrom ist, also vorwärts und rückwärts gleich gelesen wird. Groß- und Kleinschreibung spielt keine Rolle. " + FRAGE + "\n\n" + B(("Anna", "Palindrom"), ("Reliefpfeiler", "Palindrom"), ("Hallo", "kein Palindrom")),
  "Drehe das Wort um und vergleiche es mit dem Original. Vorher beides gleich schreiben.")
X("l18a3", "Das Programm liest einen Satz ein und gibt aus, wie viele Wörter er hat und welches das längste ist. Die Wörter sind durch Leerzeichen getrennt. " + FRAGE + "\n\n" + B(("Ich lerne gerade programmieren", "Wörter: 4 / Längstes Wort: programmieren"), ("Apfel Banane Kiwi", "Wörter: 3 / Längstes Wort: Banane")),
  "Zerlege den Satz zuerst in seine Wörter. Dann geh sie durch.",
  m=["Zerlege den Satz mit einer String-Methode aus dieser Lektion in einzelne Wörter."])
X("l18a4", "Das Programm liest ein Wort ein und gibt es in Großbuchstaben aus, gefolgt von seiner Länge. " + FRAGE + "\n\n" + B(("Hallo", "HALLO hat 5 Buchstaben."), ("pizza", "PIZZA hat 5 Buchstaben."), ("c#", "C# hat 2 Buchstaben.")),
  "Ein Text kennt seine Länge und kann sich in Großbuchstaben verwandeln.")
X("l18a5", "Das Programm liest einen Namen ein und sagt, ob er mit dem Buchstaben A beginnt. " + FRAGE + "\n\n" + B(("Ana", "Ana beginnt mit A."), ("Ben", "Ben beginnt nicht mit A."), ("Anton", "Anton beginnt mit A.")),
  "Ein Text kann selbst sagen, womit er beginnt.")
X("l18a6", "Das Programm liest einen Satz ein und ersetzt darin jedes Vorkommen des Wortes `schlecht` durch `gut`. " + FRAGE + "\n\n" + B(("Das Wetter ist schlecht.", "Das Wetter ist gut."), ("Mir geht es schlecht, schlecht, gut!", "Mir geht es gut, gut, gut!")),
  "Dafür gibt es eine fertige Methode, die Textstücke austauscht.")
X("l18a7", "Das Programm liest Vor- und Nachnamen (durch ein Leerzeichen getrennt) ein und gibt die Initialen mit Punkten aus. " + FRAGE + "\n\n" + B(("Max Mustermann", "Initialen: M.M."), ("Lena Sommer", "Initialen: L.S.")),
  "Zerlege den Namen in zwei Teile. Vom ersten Zeichen jedes Teils willst du den Buchstaben.")
X("l18a8", "Das Programm liest ein Wort ein und gibt es rückwärts aus. Gehe dafür die Zeichen selbst durch. " + FRAGE + "\n\n" + B(("August", "Rückwärts: tsuguA"), ("otto", "Rückwärts: otto"), ("Kabel", "Rückwärts: lebaK")),
  "Fang hinten an und bau das neue Wort Zeichen für Zeichen zusammen.",
  m=["Geh die Zeichen des Worts in einer Schleife selbst durch."])
