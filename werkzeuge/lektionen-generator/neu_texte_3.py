"""Neue Aufgabentexte Lektion 11–14."""
from neu_texte import X, Z, B, FRAGE

S = "Löse es mit der Schleife aus dieser Lektion."
ZS = "Löse es mit der Zählschleife aus dieser Lektion."
FE = "Löse es mit der Schleife aus dieser Lektion, die jedes Element einzeln durchgeht."

# ------------------------------------------------------------------ L11
X("l11a1", "Gib die Zahlen von 1 bis 10 aus, jede in einer eigenen Zeile. " + S + "\n\n" + Z("1", "2", "3", "…", "10"),
  "Eine Variable zählt mit. Die Schleife läuft, solange sie klein genug ist.")
X("l11a2", "Starte bei 1 und verdopple die Zahl immer wieder. Gib jede Zahl aus, solange sie kleiner als 1000 ist. " + S + "\n\n" + Z("1", "2", "4", "8", "…", "512"),
  "Nach jeder Ausgabe wird die Zahl verdoppelt. Die Schleife endet von selbst, sobald sie zu groß wird.")
X("l11a3", "Das Programm fragt so lange nach einem Passwort, bis `geheim` eingegeben wird. Jede falsche Eingabe wird mit `Falsch!` quittiert, am Ende kommt `Willkommen!`. " + S + " " + FRAGE + "\n\n" + B(("abc, 123, geheim", "Falsch! / Falsch! / Willkommen!"), ("geheim", "Willkommen!")),
  "Die erste Eingabe musst du vor der Schleife lesen, die weiteren in der Schleife.")
X("l11a4", "Das Programm liest so lange Zahlen ein, bis `0` kommt, und gibt dann die Summe aller Zahlen aus. Die `0` selbst zählt nicht mit. " + S + " " + FRAGE + "\n\n" + B(("5, 10, 3, 0", "Summe: 18"), ("0", "Summe: 0"), ("4, -6, 0", "Summe: -2")),
  "Eine Variable sammelt die Summe. Die Schleife endet, wenn die Eingabe 0 ist.")
X("l11a5", "Gib alle geraden Zahlen von 2 bis 20 aus, jede in einer eigenen Zeile. " + S + "\n\n" + Z("2", "4", "6", "…", "20"),
  "Der Zähler springt nach jeder Runde um denselben Betrag.")
X("l11a6", "Das Programm liest eine positive ganze Zahl ein und gibt ihre Quersumme aus, also die Summe ihrer Ziffern. " + S + " " + FRAGE + "\n\n" + B(("1234", "Quersumme: 10"), ("9", "Quersumme: 9"), ("505", "Quersumme: 10"), ("999", "Quersumme: 27")),
  "Mit dem Rest einer Division durch 10 bekommst du die letzte Ziffer. Wenn du durch 10 teilst, fällt sie weg.")
X("l11a7", "Das Programm fragt immer wieder nach einem Wort. Zu jedem Wort sagt es `Du hast … gewählt.`, bis `q` eingegeben wird. Dann sagt es `Ende.`. Das `q` selbst wird nicht als Wahl gemeldet. Es wird mindestens einmal nach einem Wort gefragt. Nutze die Schleife, die erst am Ende jeder Runde prüft. " + FRAGE + "\n\n" + B(("Pizza, Nudeln, q", "Du hast Pizza gewählt. / Du hast Nudeln gewählt. / Ende."), ("q", "Ende.")),
  "Diese Schleife läuft immer mindestens einmal. Erst danach wird entschieden, ob es weitergeht.")
X("l11a8", "Das Programm liest eine Zahl ein und gibt die kleinste Zweierpotenz aus (1, 2, 4, 8, …), die größer als diese Zahl ist. " + S + " " + FRAGE + "\n\n" + B(("100", "Die erste Zweierpotenz über 100 ist 128."), ("1", "Die erste Zweierpotenz über 1 ist 2."), ("1000", "Die erste Zweierpotenz über 1000 ist 1024.")),
  "Starte bei 1 und verdopple, solange du die Grenze noch nicht überschritten hast.")

# ------------------------------------------------------------------ L12
X("l12a1", "Gib die Fünferreihe von `1 x 5` bis `10 x 5` aus. Die Ergebnisse soll das Programm berechnen. " + ZS + "\n\n" + Z("1 x 5 = 5", "2 x 5 = 10", "…", "10 x 5 = 50"),
  "Der Zähler läuft von 1 bis 10 und wird in jeder Zeile benutzt.")
X("l12a2", "Das Programm liest eine Zahl ein und zählt von ihr bis 1 herunter. Am Ende kommt `Abflug!`. " + ZS + " " + FRAGE + "\n\n" + B(("3", "3 / 2 / 1 / Abflug!"), ("1", "1 / Abflug!"), ("5", "5 / 4 / 3 / 2 / 1 / Abflug!")),
  "Die Schleife darf auch rückwärts zählen. Die Abflug-Zeile kommt erst nach der Schleife.")
X("l12a3", "Berechne die Summe aller geraden Zahlen von 2 bis 100 und gib sie aus. " + ZS + "\n\n" + Z("Summe: 2550"),
  "Der Zähler kann in größeren Schritten laufen. Eine Variable sammelt die Summe.")
X("l12a4", "Das Programm liest eine Zahl ein und gibt eine Sterne-Treppe mit so vielen Zeilen aus. Zeile 1 hat einen Stern, Zeile 2 zwei und so weiter. " + ZS + " " + FRAGE + "\n\n" + B(("4", "* / ** / *** / ****"), ("2", "* / **")),
  "Du brauchst eine Schleife für die Zeilen und eine zweite, die in jeder Zeile die Sterne ausgibt.")
X("l12a5", "Das Programm liest eine Zahl ein und gibt ihr kleines Einmaleins von 1 bis 10 aus. " + ZS + " " + FRAGE + "\n\n" + B(("3", "3 x 1 = 3 / 3 x 2 = 6 / … / 3 x 10 = 30")),
  "Genau wie die Fünferreihe, nur mit der eingegebenen Zahl.")
X("l12a6", "Gib die Zahlen 3, 6, 9 bis 30 in einer einzigen Zeile aus, durch Leerzeichen getrennt. " + ZS + "\n\n" + Z("3 6 9 12 15 18 21 24 27 30"),
  "Jede Zahl wird ohne Zeilenwechsel ausgegeben. Erst nach der Schleife endet die Zeile.")
X("l12a7", "Das Programm liest erst die Breite, dann die Höhe ein und gibt ein Rechteck aus lauter `#` aus. " + ZS + " " + FRAGE + "\n\n" + B(("5 und 3", "##### / ##### / #####"), ("2 und 4", "## / ## / ## / ##")),
  "Eine Schleife für die Zeilen, darin eine für die Zeichen einer Zeile.")
X("l12a8", "Du legst 1000 Euro mit 10 % Zinsen pro Jahr an. Das Programm liest die Anzahl der Jahre ein und gibt für jedes Jahr den Kontostand aus (auf ganze Euro abgerundet, die Zinsen werden jedes Jahr dazugerechnet). " + ZS + " " + FRAGE + "\n\n" + B(("3", "Jahr 1: 1100 Euro / Jahr 2: 1210 Euro / Jahr 3: 1331 Euro"), ("5", "… Jahr 4: 1464 Euro / Jahr 5: 1610 Euro")),
  "Mit ganzen Zahlen gerechnet: erst mal 110, dann durch 100. So bleiben es ganze Euro.")

# ------------------------------------------------------------------ L13
X("l13a1", "Lege ein Array mit den Tagen `Mo`, `Di`, `Mi`, `Do` und `Fr` an. Gib den ersten Tag, den letzten Tag und die Anzahl der Tage aus. Letzten Tag und Anzahl soll das Programm selbst ermitteln, nichts davon darf von Hand eingetippt werden.\n\n" + Z("Mo", "Fr", "5"),
  "Das Array kennt seine eigene Länge. Der letzte Platz hat eine Nummer, die sich daraus ergibt.")
X("l13a2", "Lege ein Array `quadrate` für 10 ganze Zahlen an. Fülle es so, dass an jedem Platz das Quadrat seiner Platznummer steht (Platz 0 hat 0, Platz 3 hat 9 usw.). Gib danach alle Werte in einer Zeile aus, durch Leerzeichen getrennt. Die Werte soll das Programm berechnen.\n\n" + Z("0 1 4 9 16 25 36 49 64 81"),
  "Die Platznummer läuft von 0 bis 9. Zum Füllen und zum Ausgeben brauchst du je eine Schleife.")
X("l13a3", "Im Startcode ist ein Array mit Zahlen vorgegeben. Finde den größten Wert und gib ihn aus. Du darfst ihn nicht selbst nachschauen und eintippen, das Programm soll ihn finden:\n\n" + Z("Maximum: 89"),
  "Merk dir den bisher größten Wert und vergleiche ihn mit jedem weiteren.")
X("l13a4", "Das Programm liest drei ganze Zahlen ein und gibt sie in umgekehrter Reihenfolge wieder aus, jede in einer eigenen Zeile. Speichere die Eingaben dafür in einem Array. " + FRAGE + "\n\n" + B(("1, 2, 3", "3 / 2 / 1"), ("7, -4, 9", "9 / -4 / 7")),
  "Du brauchst alle drei Werte, bevor du den ersten ausgeben kannst. Zähl rückwärts durch das Array.")
X("l13a5", "Lege ein Array `farben` mit `Rot`, `Grün` und `Blau` an. Gib die erste Farbe, die letzte Farbe und die Anzahl aus. Die Anzahl und den Platz der letzten Farbe soll das Programm ermitteln.\n\n" + Z("Erste Farbe: Rot", "Letzte Farbe: Blau", "Anzahl: 3"),
  "Der letzte Platz hängt von der Länge des Arrays ab.")
X("l13a6", "Lege die Temperaturen 18, 23, 19, 25 und 21 in einem Array ab. Gib alle Temperaturen aus, die höher als 20 Grad sind, und danach, wie viele es waren:\n\n" + Z("23", "25", "21", "3 Tage über 20 Grad"),
  "Geh alle Plätze durch. Ein Zähler merkt sich, wie viele passen.")
X("l13a7", "Das Programm liest fünf ganze Zahlen ein, speichert sie in einem Array und gibt Summe und Durchschnitt aus (Durchschnitt mit einer Nachkommastelle). " + FRAGE + "\n\n" + B(("1, 2, 3, 4, 6", "Summe: 16 / Durchschnitt: 3,2"), ("2, 4, 4, 5, 10", "Summe: 25 / Durchschnitt: 5,0")),
  "Erst alles einlesen und speichern, dann in einer zweiten Runde summieren. Beim Durchschnitt darf nicht ganzzahlig geteilt werden.")
X("l13a8", "Im Startcode ist kein Array vorgegeben. Lege selbst ein Array mit den Zahlen 1 bis 5 an. Erzeuge daraus ein **zweites** Array mit denselben Zahlen in umgekehrter Reihenfolge und gib dieses in einer Zeile aus, durch Leerzeichen getrennt:\n\n" + Z("5 4 3 2 1"),
  "Das zweite Array hat dieselbe Länge. Der erste Platz des neuen Arrays bekommt den letzten des alten.")

# ------------------------------------------------------------------ L14
X("l14a1", "Im Startcode ist eine Einkaufsliste vorgegeben. Gib jeden Artikel in einer eigenen Zeile mit einem Strich davor aus. " + FE + "\n\n" + Z("- Milch", "- Brot", "- Eier", "- Äpfel"),
  "Du brauchst keine Platznummern, die Schleife liefert dir jeden Artikel direkt.")
X("l14a2", "Im Startcode sind fünf Temperaturen vorgegeben. Berechne ihren Durchschnitt und gib ihn mit einer Nachkommastelle aus. " + FE + "\n\n" + Z("Durchschnitt: 18,4 Grad"),
  "Erst alle Werte aufsummieren, dann durch ihre Anzahl teilen.")
X("l14a3", "Das Programm liest ein Wort ein und zählt, wie viele Vokale (a, e, i, o, u) es enthält. Das Wort ist klein geschrieben. " + FE + " " + FRAGE + "\n\n" + B(("programmieren", "Vokale: 5"), ("xyz", "Vokale: 0"), ("hallo", "Vokale: 2")),
  "Ein Text besteht aus einzelnen Zeichen. Die Schleife kann sie nacheinander durchgehen.")
X("l14a4", "Begrüße die Personen `Ana`, `Ben` und `Cem` jeweils in einer eigenen Zeile. Lege die Namen in einem Array ab. " + FE + "\n\n" + Z("Hallo Ana!", "Hallo Ben!", "Hallo Cem!"),
  "Ein Array, eine Schleife, ein Befehl in der Schleife.")
X("l14a5", "Finde in der Zahlenreihe 12, 5, 8, 3, 9 die kleinste Zahl. Das Programm soll sie finden, nicht du. " + FE + "\n\n" + Z("Kleinste Zahl: 3"),
  "Merk dir die bisher kleinste Zahl und vergleiche sie mit jeder weiteren.")
X("l14a6", "Zähle in der Zahlenreihe 3, -1, 4, -5, 9, 0, wie viele Zahlen positiv und wie viele negativ sind. Die Null zählt weder noch. " + FE + "\n\n" + Z("Positiv: 3", "Negativ: 2"),
  "Zwei Zähler, und in jeder Runde kommt höchstens einer dazu.")
X("l14a7", "Das Programm liest einen Satz ein und zählt, wie oft der Buchstabe `e` darin vorkommt. " + FE + " " + FRAGE + "\n\n" + B(("Lesen ist schön", "Anzahl e: 2"), ("Hallo", "Anzahl e: 0"), ("eeee", "Anzahl e: 4")),
  "Ein Text besteht aus einzelnen Zeichen. Die Schleife prüft jedes davon.")
X("l14a8", "Lege vier Preise 1,99, 4,50, 0,99 und 12 in einem Array von Kommazahlen an. Gib jeden Preis in einer eigenen Zeile aus (zwei Nachkommastellen) und danach die Summe. " + FE + "\n\n" + Z("1,99 Euro", "4,50 Euro", "0,99 Euro", "12,00 Euro", "Summe: 19,48 Euro"),
  "Gib in der Schleife jeden Preis aus und sammle gleichzeitig die Summe.")
