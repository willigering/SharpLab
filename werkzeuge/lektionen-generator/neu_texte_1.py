"""Neue Aufgabentexte Lektion 1–10."""
from neu_texte import X, Z, B, FRAGE
import gen

T = gen.T

# ------------------------------------------------------------------ L01
X("l01a1", "Schreib ein Programm, das diesen Gruß ausgibt:\n\n" + Z("Hallo, Welt!"),
  "Schau dir das Beispiel oben in der Lektion an. Der Text kommt so in den Befehl, wie er später zu lesen sein soll.")
X("l01a2", "Stell dich in drei Zeilen vor. Jede Angabe steht in einer eigenen Zeile:\n\n" + Z(
    "Ich heiße Max.", "Ich bin 20 Jahre alt.", "Meine Lieblingsfarbe ist Blau."),
  "Eine Zeile, ein Befehl. Vergleiche Wörter und Satzzeichen Zeichen für Zeichen mit der Vorgabe.")
X("l01a3", "Gib diese Zeile aus, aber nicht mit einem Befehl, sondern mit drei: Jedes Wort kommt aus einem eigenen Befehl, und trotzdem soll alles in **einer** Zeile stehen.\n\n" + Z("Eins Zwei Drei"),
  "Die Wörter brauchen Abstand zueinander. Überleg, wo du ihn einbaust.",
  m=["Alle drei Wörter sollen in derselben Zeile landen. In der Lektion gibt es einen Befehl, der danach nicht in die nächste Zeile springt."])
X("l01a4", "Gib den Countdown in **einer** Zeile aus. Jeder der vier Teile (`3...`, `2...`, `1...` und `Start!`) soll von einem eigenen Befehl kommen:\n\n" + Z("3... 2... 1... Start!"),
  "Nur der letzte Teil darf die Zeile beenden.",
  m=["Teile den Countdown auf vier Befehle auf. Die ersten drei sollen in derselben Zeile bleiben."])
X("l01a5", "Gib zwei Zeilen aus. In der ersten Zeile soll der Gruß aus zwei Befehlen entstehen (erst `Hallo,`, dann `Lena!`). Die zweite Zeile ist ein eigener Satz:\n\n" + Z("Hallo, Lena!", "Schön, dass du da bist."),
  "Nach dem ersten Befehl geht es in derselben Zeile weiter. Erst nach dem zweiten beginnt eine neue.",
  m=["Die erste Zeile soll aus zwei Befehlen bestehen. Der erste darf noch keine neue Zeile beginnen."])
X("l01a6", "Gib ein Dreieck aus Sternen aus. Es hat vier Zeilen: Zeile 1 hat einen Stern, Zeile 2 zwei, Zeile 3 drei und Zeile 4 vier.\n\n" + Z("*", "**", "***", "****"),
  "Pro Zeile ein Befehl. Zähl die Sterne nach.")
X("l01a7", "Gib diese Visitenkarte aus. Die drei Zeilen zwischen den Linien sind eingerückt:\n\n" + Z(
    "==================", "  Max Muster", "  Schüler, 8b", "  Lieblingsfach: IT", "=================="),
  "Die beiden Linien sind gleich lang. Der Einzug gehört zum Text.")
X("l01a8", "Gib zwei Zeilen aus. Die erste Zeile soll aus vier Befehlen entstehen (`Ich`, `lerne`, `C#,` und `Schritt für Schritt.`). Die zweite ist ein eigener Satz:\n\n" + Z(
    "Ich lerne C#, Schritt für Schritt.", "Und es macht Spaß!"),
  "Nur der vierte Befehl beendet die erste Zeile. Die Abstände zwischen den Wörtern musst du selbst einbauen.",
  m=["Die erste Zeile soll aus mehreren Befehlen bestehen, die in derselben Zeile bleiben."])

# ------------------------------------------------------------------ L02
X("l02a1", "Das Programm gibt drei Grüße aus. Der mittlere soll nicht mehr erscheinen, die Zeile soll aber im Code stehen bleiben (du löschst sie also nicht). Danach lautet die Ausgabe:\n\n" + Z("Guten Morgen", "Gute Nacht"),
  "Du musst nichts löschen. Die Lektion zeigt, wie man C# sagt: Diese Zeile bitte überspringen.",
  m=["Die mittlere Zeile soll im Code bleiben. Lösch sie nicht, sondern sorg dafür, dass sie nicht ausgeführt wird."])
X("l02a2", "Im Code steht schon ein Ausgabe-Befehl. Schreib direkt darüber eine Notiz, die erklärt, was der Befehl macht. Die Notiz ist nur für Menschen und darf in der Ausgabe nicht erscheinen:\n\n" + Z("C# macht Spaß"),
  "Der Computer soll die Notiz überspringen. In der Lektion steht, wie du ihm das sagst.",
  reqs=[(r"//|/\*", "Über dem Befehl fehlt noch deine Notiz.", True)])
X("l02a3", "Im Code stehen vier Ausgabe-Befehle. `Fehler 1` und `Fehler 2` sollen nicht mehr laufen, aber im Code stehen bleiben. Das geht mit **einer** einzigen Markierung für beide Zeilen zusammen, du brauchst nicht für jede Zeile eine eigene. Ausgabe:\n\n" + Z("Start", "Ende"),
  "Es gibt eine Markierung, die einen ganzen Block umschließt. Sie hat einen Anfang und ein Ende.",
  m=["Hier fehlt noch eine Markierung, die beide Zeilen auf einmal überspringt.", "Lösch die Zeilen nicht. Sie sollen im Code stehen bleiben."])
X("l02a4", "Gib diese Zeile aus. In derselben Zeile **hinter** dem Befehl steht eine kurze Notiz, was der Befehl macht:\n\n" + Z("Programm startet"),
  "Die Notiz steht hinter dem Befehl, nicht in der Zeile darunter.",
  reqs=[(r";[ \t]*(//|/\*)", "Die Notiz soll in derselben Zeile hinter dem Befehl stehen.", True)])
X("l02a5", "Gib diese Zeile aus. Darüber steht **eine** zusammenhängende Notiz, die genau drei Zeilen lang ist, zum Beispiel mit Programmname, Datum und Zweck. Es sind nicht drei einzelne Notizen. Der Inhalt ist dir überlassen.\n\n" + Z("Hausaufgaben erledigt!"),
  "Eine Notiz über mehrere Zeilen hat einen Anfang und ein Ende. Dazwischen steht der Text.",
  m=["Die Notiz soll ein einziger Kommentar über drei Zeilen sein."])
X("l02a6", "Gib diese zwei Zeilen aus:\n\n" + Z("Mein Programm", "Version 1") + "\n\nGanz oben im Code steht eine mehrzeilige Notiz, die das Programm beschreibt. Außerdem steht hinter **jedem** der beiden Ausgabe-Befehle noch eine kurze Notiz in derselben Zeile.",
  "Zwei Arten von Notizen: oben die lange mit Anfang und Ende, hinter den Befehlen die kurzen.",
  m=["Oben fehlt noch die mehrzeilige Notiz.", "Hinter jedem Ausgabe-Befehl soll eine kurze Notiz stehen."])
X("l02a7", "Gib die drei Farben aus. Erkläre den Code mit **genau einer** kurzen Notiz (die nur bis zum Zeilenende gilt) und **genau einer** Notiz mit Anfang und Ende. Wo sie stehen, ist dir überlassen. Den vorgegebenen Platzhalter ersetzt du dabei.\n\n" + Z("Rot", "Gelb", "Grün"),
  "Eine Notiz gilt nur bis zum Zeilenende, die andere hat einen Anfang und ein Ende.",
  m=["Es fehlt noch eine kurze Notiz, die nur bis zum Zeilenende gilt.", "Es fehlt noch eine Notiz mit Anfang und Ende."])
X("l02a8", "Gib `Eins` und `Drei` aus. Der Befehl für `Zwei` soll trotzdem im Code stehen, aber nicht ausgeführt werden.\n\n" + Z("Eins", "Drei"),
  "Schreib den Befehl für Zwei ganz normal hin und schalte ihn dann aus. Löschen gilt nicht.",
  m=["Der Befehl für Zwei soll im Code stehen bleiben, nur eben nicht ausgeführt werden."])

# ------------------------------------------------------------------ L03
X("l03a1", "Lege drei Variablen an und gib jede in einer eigenen Zeile aus, in dieser Reihenfolge:\n\n- `stadt`: der Text `Berlin`\n- `einwohner`: die ganze Zahl `3700000`\n- `istHauptstadt`: der Wahrheitswert `true`\n\nDie Variablen müssen genau so heißen.\n\n" + Z("Berlin", "3700000", "True"),
  "Überleg dir zuerst für jede Angabe den passenden Datentyp. Für Ja/Nein-Werte gibt es einen eigenen.",
  m=["Lege `einwohner` als ganze Zahl an.", "Lege `istHauptstadt` als Wahrheitswert an."])
X("l03a2", "Lege `tier` (Text: `Katze`) und `beine` (ganze Zahl: `4`) an. Setze daraus diesen Satz zusammen. `Katze` und `4` müssen aus den Variablen kommen und dürfen nicht von Hand im Satz stehen:\n\n" + Z("Eine Katze hat 4 Beine."),
  "Der Satz besteht aus Textstücken und Variablen. Achte auf die Abstände dazwischen.",
  reqs=[(r"\+\s*tier|tier\s*\+|\{\s*tier\s*\}", "`tier` fehlt noch in deinem Satz. Der Text soll aus der Variablen kommen."),
        (r"\+\s*beine|beine\s*\+|\{\s*beine\s*\}", "`beine` fehlt noch in deinem Satz. Die Zahl soll aus der Variablen kommen.")])
X("l03a3", "Lege eine ganzzahlige Variable `punkte` mit dem Wert `10` an und gib sie aus. Ändere danach **dieselbe** Variable auf `50` (keine zweite anlegen) und gib sie noch einmal aus:\n\n" + Z("Punkte: 10", "Punkte: 50"),
  "Es gibt nur eine Variable `punkte`. Sie bekommt einfach einen neuen Wert.",
  m=["`punkte` hat noch nicht den neuen Wert 50. Ändere die vorhandene Variable, statt eine neue anzulegen."],
  reqs=[(r"(?<!int\s)\bpunkte\s*=\s*50", "`punkte` hat noch nicht den neuen Wert 50. Ändere die vorhandene Variable, statt eine neue anzulegen.")])
X("l03a4", "Speichere zwei Angaben in Variablen und gib sie aus:\n\n- `lieblingszahl`: die ganze Zahl `7`\n- `anfangsbuchstabe`: das einzelne Zeichen `L`\n\nDie Werte in der Ausgabe müssen aus den Variablen kommen.\n\n" + Z("Lieblingszahl: 7", "Anfangsbuchstabe: L"),
  "Ein einzelnes Zeichen hat einen eigenen Datentyp. Sein Wert wird anders geschrieben als ein Text.",
  m=["Lege `lieblingszahl` als ganze Zahl an.", "`anfangsbuchstabe` braucht den Datentyp für ein einzelnes Zeichen.", "Die 7 soll aus der Variablen kommen.", "Das L soll aus der Variablen kommen."],
  reqs=[(r"int\s+lieblingszahl\s*=", "Lege `lieblingszahl` als ganze Zahl an."),
        (r"char\s+anfangsbuchstabe\s*=", "`anfangsbuchstabe` braucht den Datentyp für ein einzelnes Zeichen."),
        (r"\+\s*lieblingszahl|\{\s*lieblingszahl\s*\}", "Die 7 soll aus der Variablen kommen."),
        (r"\+\s*anfangsbuchstabe|\{\s*anfangsbuchstabe\s*\}", "Das L soll aus der Variablen kommen.")])
X("l03a5", "Ein Brot kostet 2,49 Euro. Speichere den Namen in `produkt` (Text) und den Preis in `preis` (Kommazahl) und gib damit diesen Satz aus. Name und Preis müssen aus den Variablen kommen:\n\n" + Z("Brot kostet 2,49 Euro."),
  "Denk an die Schreibweise von Kommazahlen im Code. Das Komma in der Ausgabe kommt von allein.",
  m=["Lege `produkt` als Text an.", "`preis` braucht den Datentyp für Kommazahlen.", "Brot soll aus der Variablen `produkt` kommen.", "Der Preis soll aus der Variablen `preis` kommen."],
  reqs=[(r"string\s+produkt\s*=", "Lege `produkt` als Text an."),
        (r"double\s+preis\s*=", "`preis` braucht den Datentyp für Kommazahlen."),
        (r"produkt\s*\+|\{\s*produkt\s*\}", "Brot soll aus der Variablen `produkt` kommen."),
        (r"\+\s*preis|\{\s*preis\s*\}", "Der Preis soll aus der Variablen `preis` kommen.")])
X("l03a6", "Lege `hatHunger` als Wahrheitswert mit dem Startwert `false` an und gib ihn aus. Ändere ihn danach auf `true` (dieselbe Variable) und gib ihn noch einmal aus:\n\n" + Z("Hunger: False", "Hunger: True"),
  "Beim zweiten Mal bekommt die vorhandene Variable nur einen neuen Wert. Sonst wäre es eine neue.",
  m=["Lege `hatHunger` als Wahrheitswert mit dem Startwert false an.", "Ändere den Wert von `hatHunger` später auf true.", "Der Wert soll aus der Variablen kommen."],
  reqs=[(r"bool\s+hatHunger\s*=\s*false", "Lege `hatHunger` als Wahrheitswert mit dem Startwert false an."),
        (r"hatHunger\s*=\s*true", "Ändere den Wert von `hatHunger` später auf true."),
        (r"\+\s*hatHunger|\{\s*hatHunger\s*\}", "Der Wert soll aus der Variablen kommen.")])
X("l03a7", "Lege `a` mit dem Wert `3` und `b` mit dem Wert `8` an (ganze Zahlen). Vertausche danach die beiden Werte, ohne `3` oder `8` noch einmal einzutippen. Gib am Ende beide aus. Die Zahlen müssen aus den Variablen kommen:\n\n" + Z("a = 8", "b = 3"),
  "Wenn du `a` sofort überschreibst, ist der alte Wert weg. Du brauchst einen Zwischenspeicher.",
  m=["Lege `a` mit dem Wert 3 an.", "Lege `b` mit dem Wert 8 an.", "Merk dir einen der Werte in einer dritten Variablen, bevor du ihn überschreibst.", "Gib `a` über die Variable aus.", "Gib `b` über die Variable aus."],
  reqs=[(r"int\s+a\s*=\s*3\s*;", "Lege `a` mit dem Wert 3 an."),
        (r"int\s+b\s*=\s*8\s*;", "Lege `b` mit dem Wert 8 an."),
        (r"int\s+(?!a\b|b\b)\w+\s*=\s*[ab]\s*;", "Merk dir einen der Werte in einer dritten Variablen, bevor du ihn überschreibst."),
        (r"\+\s*a\b|\{\s*a\s*\}", "Gib `a` über die Variable aus."),
        (r"\+\s*b\b|\{\s*b\s*\}", "Gib `b` über die Variable aus.")])
X("l03a8", "Lege vier Variablen an und gib damit einen Steckbrief aus. Die Werte müssen aus den Variablen kommen:\n\n- `name`: der Text `Mia`\n- `alter`: die ganze Zahl `14`\n- `groesse`: die Kommazahl `1.75`\n- `schueler`: der Wahrheitswert `true`\n\n" + Z("Name: Mia", "Alter: 14", "Größe: 1,75 m", "Schüler: True"),
  "Vier Werte, vier Datentypen. Das „m“ hinter der Größe ist einfach Text, der dahinter steht.",
  m=["Lege `name` als Text an.", "Lege `alter` als ganze Zahl an.", "Lege `groesse` als Kommazahl an.", "Lege `schueler` als Wahrheitswert an.", "Die Größe soll aus der Variablen kommen."],
  reqs=[(r"string\s+name\s*=", "Lege `name` als Text an."),
        (r"int\s+alter\s*=", "Lege `alter` als ganze Zahl an."),
        (r"double\s+groesse\s*=", "Lege `groesse` als Kommazahl an."),
        (r"bool\s+schueler\s*=", "Lege `schueler` als Wahrheitswert an."),
        (r"\+\s*groesse|\{\s*groesse\s*\}", "Die Größe soll aus der Variablen kommen.")])

# ------------------------------------------------------------------ L04
X("l04a1", "Ein Rechteck ist 8 breit und 5 hoch. Das Programm soll Fläche und Umfang selbst ausrechnen (nicht von Hand eintippen) und ausgeben:\n\n" + Z("Fläche: 40", "Umfang: 26"),
  "Speichere Breite und Höhe in Variablen. Die Formeln kennst du aus der Schule.")
X("l04a2", "17 Kekse werden gerecht an 5 Kinder verteilt. Das Programm soll ausrechnen, wie viele Kekse jedes Kind bekommt und wie viele übrig bleiben:\n\n" + Z("Jedes Kind: 3", "Übrig: 2"),
  "Ganzzahlig teilen liefert die Kekse pro Kind. Für den Rest gibt es einen eigenen Rechenoperator.",
  m=["Für den Rest, der übrig bleibt, gibt es einen eigenen Operator. Schau in der Lektion nach."])
X("l04a3", "Ein Spiel startet bei Level 1. Erhöhe das Level zweimal um je 1 und danach um 10. Nutze dafür die Kurzschreibweisen aus der Lektion. Gib das Level am Anfang und nach jedem Schritt aus:\n\n" + Z("Level 1", "Level 2", "Level 3", "Level 13"),
  "Es gibt eine Kurzschreibweise für „plus eins“ und eine für „plus eine beliebige Zahl“.",
  m=["Nutze für das Erhöhen um 1 die Kurzschreibweise aus der Lektion.", "Nutze für das Erhöhen um 10 die Kurzschreibweise aus der Lektion."])
X("l04a4", "Die vier Noten 2, 3, 3 und 2 sollen gemittelt werden. Das Ergebnis ist eine Kommazahl, es darf also **nicht** ganzzahlig geteilt werden:\n\n" + Z("Durchschnitt: 2,5"),
  "Beim Teilen zweier ganzer Zahlen fällt der Rest weg. Mindestens eine Zahl muss eine Kommazahl sein.",
  reqs=[(r"\d\.\d|\d[dD]\b|\(\s*double\s*\)|double\s+\w+\s*=\s*\d+\s*;", "Beim Teilen muss mindestens eine Zahl eine Kommazahl sein, sonst fällt der Rest weg.")],
  sol='''
  int n1 = 2;
  int n2 = 3;
  int n3 = 3;
  int n4 = 2;

  double schnitt = (n1 + n2 + n3 + n4) / 4.0;
  Console.WriteLine("Durchschnitt: " + schnitt);''',
  tests=[T("Durchschnitt: 2,5")])
X("l04a5", "Ein Film dauert 135 Minuten. Speichere die Zahl in `minuten` und lass das Programm ausrechnen, wie viele volle Stunden und wie viele Minuten das sind:\n\n" + Z("135 Minuten sind 2 Stunden und 15 Minuten."),
  "Die vollen Stunden bekommst du durch Teilen, die übrigen Minuten sind der Rest.",
  m=["Lege `minuten` als ganze Zahl mit 135 an.", "Die Stunden soll das Programm ausrechnen, nicht von Hand eintippen.", "Die übrigen Minuten soll das Programm ausrechnen, nicht von Hand eintippen."])
X("l04a6", "Ein Auto fährt 150 km in 2,5 Stunden. Lege `strecke` und `zeit` als Kommazahlen an und lass das Programm die Geschwindigkeit ausrechnen:\n\n" + Z("Geschwindigkeit: 60 km/h"),
  "Geschwindigkeit ist Strecke durch Zeit.",
  m=["Lege `zeit` als Kommazahl an.", "Die Geschwindigkeit soll das Programm aus `strecke` und `zeit` ausrechnen."])
X("l04a7", "Lege die Kommazahl `temp` mit dem Wert `12.5` an. Erhöhe sie um 3,5, gib sie aus, halbiere sie danach und gib sie noch einmal aus. Benutze dafür die Kurzschreibweisen aus der Lektion:\n\n" + Z("16", "8"),
  "Die Kurzschreibweisen verändern die Variable direkt. Es braucht keinen neuen Wert und keine zweite Variable.",
  m=["Lege `temp` als Kommazahl mit dem Wert 12.5 an.", "Erhöhe `temp` mit der Kurzschreibweise für Plus.", "Halbiere `temp` mit der passenden Kurzschreibweise."])
X("l04a8", "Eine Bakterienkultur startet mit 50 Bakterien (`bakterien`) und verdoppelt sich jede Stunde. Gib den Start und die ersten drei Stunden aus. Lass dafür das Programm rechnen:\n\n" + Z(
    "Start: 50", "Nach 1 Stunde: 100", "Nach 2 Stunden: 200", "Nach 3 Stunden: 400"),
  "Die Variable wird immer wieder mit demselben Faktor verändert. Dafür gibt es eine Kurzschreibweise.",
  m=["Lege `bakterien` als ganze Zahl mit dem Wert 50 an.", "Lass das Programm die Verdopplung dreimal selbst ausrechnen, mit der Kurzschreibweise aus der Lektion.", "Der Wert in der Ausgabe soll aus der Variablen kommen."])

# ------------------------------------------------------------------ L05
X("l05a1", "Speichere einen Namen (`Tom`) und ein Alter (`17`) in Variablen. Gib damit **einen** Satz aus, in dem beide Werte direkt in den Text eingebaut sind:\n\n" + Z("Ich heiße Tom und bin 17 Jahre alt."),
  "Es gibt eine Schreibweise, bei der Variablen mitten im Text stehen dürfen. Schau in die Lektion.",
  m=["Baue die Variablen direkt in den Text ein, so wie in der Lektion gezeigt."])
X("l05a2", "Ein Ticket kostet 9,50 Euro. Lass das Programm ausrechnen, was 4 Tickets kosten, und gib den Preis mit zwei Nachkommastellen aus. Die Rechnung steht direkt im Satz:\n\n" + Z("4 Tickets kosten 38,00 Euro."),
  "In die Textschreibweise der Lektion darf auch eine Rechnung hinein. Die Nachkommastellen legst du dort ebenfalls fest.",
  m=["Baue Werte und Rechnung direkt in den Text ein, so wie in der Lektion gezeigt."])
X("l05a3", "Gib die drei Farben in drei Zeilen aus, aber mit **einem einzigen** Ausgabe-Befehl:\n\n" + Z("Rot", "Gelb", "Grün"),
  "Es gibt ein Sonderzeichen, das mitten im Text einen Zeilenumbruch erzeugt.",
  m=["Mit einem Befehl geht es, wenn du im Text selbst für die Zeilenumbrüche sorgst. Die Lektion zeigt wie."])
X("l05a4", "Dein Kontostand beträgt 1234,50 Euro. Speichere ihn in der Kommazahl `betrag` und gib ihn mit zwei Nachkommastellen aus:\n\n" + Z("Kontostand: 1234,50 Euro"),
  "Der Wert steht mitten im Text. Dort legst du auch die Nachkommastellen fest.",
  m=["Baue den Betrag direkt in den Text ein, so wie in der Lektion gezeigt.", "Der Betrag soll aus der Variablen `betrag` kommen."])
X("l05a5", "Speichere das Wort `Hallo!` in `wort` und gib damit diesen Satz aus. Die Anführungszeichen gehören zur Ausgabe:\n\n" + Z('Er sagte: "Hallo!"'),
  "Anführungszeichen im Text brauchen eine besondere Schreibweise, sonst endet der Text zu früh.",
  m=["Baue die Variable direkt in den Text ein, so wie in der Lektion gezeigt.", "Das Wort soll aus der Variablen `wort` kommen."])
X("l05a6", "Lege `a` mit `7` und `b` mit `3` an. Gib drei Rechnungen aus. Zahlen und Ergebnisse kommen aus den Variablen, die Division auf zwei Nachkommastellen:\n\n" + Z("7 + 3 = 10", "7 * 3 = 21", "7 / 3 = 2,33"),
  "Rechnungen dürfen direkt im Text stehen. Bei der Division darf nicht ganzzahlig geteilt werden.",
  m=["Das Ergebnis von a + b soll direkt im Text berechnet werden.", "Das Ergebnis von a * b soll direkt im Text berechnet werden.", "Auch die Division soll direkt im Text berechnet werden."])
X("l05a7", "Gib die drei Zahlen `7`, `42` und `1234` untereinander aus, rechtsbündig in einer Spalte, die 5 Zeichen breit ist. Speichere sie vorher in `x`, `y` und `z`:\n\n" + Z("    7", "   42", " 1234"),
  "In der Lektion steht, wie man einem Wert im Text eine feste Breite gibt.",
  m=["Gib `x` mit fester Breite aus.", "Gib `y` mit fester Breite aus.", "Gib `z` mit fester Breite aus."])
X("l05a8", "Ein Einkauf: Brot kostet 2,50 Euro (`preisBrot`) und Käse 3,99 Euro (`preisKaese`). Gib beide Preise und die Summe aus, jeweils mit zwei Nachkommastellen. Die Summe soll das Programm berechnen:\n\n" + Z("Brot: 2,50 Euro", "Käse: 3,99 Euro", "Summe: 6,49 Euro"),
  "Die Summe kann direkt im Text berechnet werden.",
  m=["Baue die Werte direkt in den Text ein, so wie in der Lektion gezeigt.", "Der Brotpreis soll aus `preisBrot` kommen.", "Der Käsepreis soll aus `preisKaese` kommen.", "Die Summe soll das Programm aus beiden Preisen berechnen."])

# ------------------------------------------------------------------ L06
X("l06a1", "Das Programm fragt nach dem Namen und begrüßt die Person. " + FRAGE + "\n\n" + B(("Lena", "Hallo Lena!"), ("Ben", "Hallo Ben!")),
  "Der eingegebene Text muss erst in einer Variablen landen, bevor du ihn benutzen kannst.")
X("l06a2", "Das Programm liest eine ganze Zahl ein und gibt ihr Doppeltes aus. " + FRAGE + "\n\n" + B(("21", "Das Doppelte ist 42"), ("-5", "Das Doppelte ist -10")),
  "Eingaben sind immer Text. Zum Rechnen musst du sie erst in eine Zahl umwandeln.",
  m=["Die Eingabe ist ein Text. Wandle sie in eine Zahl um, bevor du rechnest."])
X("l06a3", "Das Programm liest zwei ganze Zahlen ein und gibt die Rechnung samt Ergebnis aus. " + FRAGE + "\n\n" + B(("7 und 5", "7 + 5 = 12"), ("100 und 23", "100 + 23 = 123")),
  "Beide Eingaben müssen vor dem Rechnen zu Zahlen werden.")
X("l06a4", "Das Programm liest einen Preis (Kommazahl) und eine Menge (ganze Zahl) ein und gibt den Gesamtpreis mit zwei Nachkommastellen aus. " + FRAGE + "\n\n" + B(("2,5 und 3", "Gesamt: 7,50 Euro"), ("0,99 und 2", "Gesamt: 1,98 Euro")),
  "Preis und Menge brauchen verschiedene Datentypen. Beim Umwandeln musst du darauf achten.")
X("l06a5", "Das Programm liest einen Namen und eine Stadt ein (in dieser Reihenfolge) und gibt einen Satz aus. " + FRAGE + "\n\n" + B(("Lena, Bonn", "Lena wohnt in Bonn."), ("Tom, Wien", "Tom wohnt in Wien.")),
  "Zwei Eingaben, zwei Variablen. Beide sind Text, du musst nichts umwandeln.")
X("l06a6", "Das Programm liest ein Alter in Jahren ein und schätzt, wie viele Tage das sind (ein Jahr hat 365 Tage). " + FRAGE + "\n\n" + B(("12", "Du bist ungefähr 4380 Tage alt."), ("30", "Du bist ungefähr 10950 Tage alt.")),
  "Erst umwandeln, dann rechnen.")
X("l06a7", "Das Programm liest Breite und Höhe eines Rechtecks ein (ganze Zahlen, in dieser Reihenfolge) und gibt Fläche und Umfang aus. " + FRAGE + "\n\n" + B(("6 und 4", "Fläche: 24 / Umfang: 20 (zwei Zeilen)"), ("3 und 3", "Fläche: 9 / Umfang: 12 (zwei Zeilen)")),
  "Zwei Eingaben, zwei Ergebnisse. Du kannst sie direkt im Text berechnen.")
X("l06a8", "Das Programm liest die getankten Liter und den Preis pro Liter ein (beides Kommazahlen, in dieser Reihenfolge) und gibt die Kosten mit zwei Nachkommastellen aus. " + FRAGE + "\n\n" + B(("40 und 1,85", "Tanken kostet 74,00 Euro."), ("12,5 und 2", "Tanken kostet 25,00 Euro.")),
  "Beide Eingaben sind Kommazahlen.")
