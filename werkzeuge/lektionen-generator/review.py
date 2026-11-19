r"""Review-Korrekturen (Nov 2026): Anforderungen, Tipps, Quiz-Erklärungen und zusätzliche Quizfragen.

Wird von build.py nach den teil*-Modulen geladen und ergänzt gen.LESSONS.
Hinweis: Der Prüfer vergleicht die Muster mit dem Code ohne Kommentare, dessen Tokens durch Leerzeichen
getrennt sind (z. B. "liste . Add ("). Deshalb steht zwischen Tokens wie . < > ( ) immer \s*.

Text-Korrekturen (Pizzen, Versuche, Durchschnitt, Kreis, switch-Muster, Length) stehen direkt in teil*.py.
"""
import gen

# Punkt 1: Anforderungen gegen fest eingetippte Ausgaben.
# l13a2, l21a3, l22a2, l23a1, l23a3, l27a1, l27a3 wurden in 1.0.2 verschärft: Die Muster aus der Review
# standen schon im Startcode (Klasse, Aufrufe), jetzt prüfen sie Code, den man selbst schreiben muss.
REQUIREMENTS = {
    "l04a4": [(r'3\.0|\(\s*double\s*\)', "Teile durch 3.0 (oder wandle mit (double) um), damit eine Kommazahl herauskommt.")],
    "l13a1": [(r'\.\s*Length', "Benutze .Length für die Länge des Arrays – und auch für das letzte Element.")],
    "l13a2": [(r'quadrate\s*\[\s*\w+\s*\]\s*=', "Fülle die Fächer in der Schleife, z. B. mit quadrate[i] = …"),
              (r'for\s*\(', "Fülle das Array mit einer for-Schleife.")],
    "l16a2": [(r'static\s+void\s+Quadrat\s*\(\s*int', "Schreib die Methode static void Quadrat(int zahl).")],
    "l16a3": [(r'static\s+void\s+Rahmen\s*\(\s*string', "Schreib die Methode static void Rahmen(string text).")],
    "l21a1": [(r'public\s+string\s+Marke', "Ergänze das Feld public string Marke;"),
              (r'public\s+int\s+PS', "Ergänze das Feld public int PS;"),
              (r'void\s+Info\s*\(', "Ergänze die Methode public void Info().")],
    "l21a3": [(r'void\s+Erhoehen\s*\(', "Schreib in der Klasse Zaehler die Methode public void Erhoehen()."),
              (r'void\s+Zuruecksetzen\s*\(', "Schreib in der Klasse Zaehler die Methode public void Zuruecksetzen().")],
    "l22a1": [(r'public\s+Schueler\s*\(', "Schreib einen Konstruktor public Schueler(...) in der Klasse.")],
    "l22a2": [(r'public\s+Konto\s*\(', "Schreib einen Konstruktor public Konto(string inhaber, double startbetrag)."),
              (r'void\s+Einzahlen\s*\(', "Schreib in der Klasse die Methode public void Einzahlen(double betrag)."),
              (r'void\s+Abheben\s*\(', "Schreib in der Klasse die Methode public void Abheben(double betrag).")],
    "l23a1": [(r'double\s+Fahrenheit\s*(\{|=>)', "Ergänze in der Klasse die Eigenschaft public double Fahrenheit (nur mit get).")],
    "l23a3": [(r'void\s+Einzahlen\s*\(', "Schreib in der Klasse die Methode public void Einzahlen(int betrag).")],
    "l24a1": [(r'\.\s*Add\s*\(', "Füge die Schüler mit .Add(...) zur Liste hinzu.")],
    "l24a2": [(r'foreach|for\s*\(', "Suche das teuerste Produkt mit einer Schleife (foreach oder for).")],
    "l27a1": [(r'\.\s*Add\s*\(', "Füge neue Artikel mit liste.Add(...) zur Liste hinzu."),
              (r'\.\s*Count\b', "Prüfe mit liste.Count, ob die Liste leer ist.")],
    "l27a2": [(r'Contains', "Prüfe mit Contains, ob der Artikel schon auf der Liste steht."),
              (r'Remove', "Entferne Artikel mit Remove.")],
    "l27a3": [(r'new\s+Artikel\b', "Lege für jeden neuen Artikel ein Objekt mit new Artikel … an."),
              (r'Menge\s*(\+=|=\s*\w+\s*\.\s*Menge\s*\+)', "Ist der Artikel schon da, addiere die neue Menge zur vorhandenen (z. B. mit +=).")],
}

# Punkt 7: Tipps (zeigen den Weg, verraten nicht die Lösung)
HINTS = {
    "l01a2": "Jede Zeile bekommt ihr eigenes Console.WriteLine(...); mit dem Text in Anführungszeichen. Achte auf den Punkt am Satzende.",
    "l02a2": "Ein einzeiliger Kommentar beginnt mit // und gehört in eine eigene Zeile über den Befehl. Der Computer ignoriert alles dahinter.",
    "l02a3": "Setz /* vor die erste und */ hinter die zweite Zeile, die verschwinden soll. Alles dazwischen wird übersprungen.",
    "l03a3": "Beim Ändern schreibst du nur den Namen, ein = und den neuen Wert. Ein zweites int davor würde eine neue Variable anlegen wollen.",
    "l04a1": "Lege für Fläche und Umfang je eine int-Variable an und rechne mit den Formeln aus der Aufgabe. Danach baust du die Ausgabe mit + zusammen.",
    "l04a2": "Ganzzahliges Teilen mit / liefert, wie viele jedes Kind bekommt, % liefert den Rest. Setz die Rechnung in der Ausgabe in Klammern.",
    "l04a3": "Gib nach jeder Änderung den Wert erneut aus. ++ erhöht um genau 1, += erhöht um eine beliebige Zahl.",
    "l05a1": "Lege Variablen für Name und Alter an. Im $\"...\"-Text setzt du sie mit geschweiften Klammern ein.",
    "l05a3": "\\n innerhalb der Anführungszeichen erzeugt einen Zeilenumbruch. Du brauchst also nur einen einzigen Text.",
    "l06a1": "Console.ReadLine() liefert die Eingabe als string – speichere sie in einer Variablen. Danach gibst du den Gruß mit dem Namen aus.",
    "l06a2": "ReadLine liefert Text; mit int.Parse(...) wird daraus eine Zahl. Setz die Rechnung in der Ausgabe in Klammern.",
    "l06a3": "Lies zwei Mal ein und wandle jede Eingabe mit int.Parse um. Mit $\"...\" kannst du beide Zahlen und die Summe in einen Text setzen.",
    "l06a4": "double.Parse versteht das Komma, weil die App deutsch rechnet. Für 2 Nachkommastellen schreibst du :F2 hinter den Wert in den geschweiften Klammern.",
    "l07a1": "Der Befehl gehört zwischen die geschweiften Klammern von Main. Es ist ein ganz normales Console.WriteLine.",
    "l07a3": "Du brauchst class Program mit static void Main(string[] args) darin. Die beiden Zeilen wandern unverändert in Main.",
    "l08a1": "Der Rest beim Teilen durch 2 verrät, ob eine Zahl gerade ist. Verwende if für den einen Fall und else für den anderen.",
    "l08a2": "Prüfe die Grenzen der Reihe nach mit if, else if und else. Überleg genau, ob < oder <= passt.",
    "l08a3": "Prüf zuerst, ob beide gleich sind. Danach entscheidest du mit else if/else, welche größer ist.",
    "l09a1": "Die Zahl muss zwei Bedingungen gleichzeitig erfüllen: mindestens 1 und höchstens 100. Verbinde sie mit &&.",
    "l09a2": "Vergleiche den Text einmal mit \"Samstag\" und einmal mit \"Sonntag\". Es reicht, wenn einer der Vergleiche stimmt – dafür gibt es ||.",
    "l09a3": "Wandle ja/nein zuerst in ein bool um. Setz den Teil mit && in Klammern und verbinde ihn mit || mit der Altersgrenze 16.",
    "l10a1": "Schreib für jede Zahl von 1 bis 7 einen eigenen case mit break. Alles andere fängt default ab.",
    "l10a2": "Mach das Rechenzeichen zum Wert in switch (...), und jeder case ist ein Zeichen als Text, z. B. case \"+\":. Vergiss default nicht.",
    "l10a3": "Mehrere case-Zeilen direkt untereinander teilen sich denselben Code. Nur nach dem letzten case einer Gruppe kommen Ausgabe und break.",
    "l11a1": "Du brauchst eine Zählvariable, die vor der Schleife startet. Erhöhe sie in der Schleife, sonst läuft sie endlos.",
    "l11a2": "Die Bedingung prüft, ob die Zahl noch kleiner als 1000 ist. In der Schleife erst ausgeben, dann verdoppeln.",
    "l11a3": "Lies einmal vor der Schleife ein und dann noch einmal am Ende jedes Durchlaufs. Die Schleife läuft, solange die Eingabe nicht stimmt.",
    "l11a4": "Lege eine Summen-Variable mit 0 an. Lies vor der Schleife und in jedem Durchlauf neu ein, bis die 0 kommt.",
    "l12a1": "Die Zählvariable läuft von 1 bis 10. Mit $\"...\" kannst du sie und das Ergebnis in die Zeile einsetzen.",
    "l12a3": "Starte die Schleife bei 2 und geh in Zweierschritten weiter. Addiere jede Zahl zu einer Summe, die du vorher anlegst.",
    "l13a1": "Das erste Element hat den Index 0. Den letzten Index bekommst du aus der Länge minus 1.",
    "l13a3": "Merk dir am Anfang das erste Element als bisheriges Maximum. Vergleiche in der Schleife jedes weitere Element damit.",
    "l14a1": "foreach gibt dir jedes Element nacheinander in eine Variable. Davor setzt du einfach \"- \".",
    "l14a2": "Addiere alle Werte in der foreach-Schleife zu einer Summe. Teile danach durch die Anzahl und formatiere mit :F1.",
    "l15a1": "Erst die leere Liste anlegen, dann dreimal Add aufrufen. Die Anzahl liefert .Count, die Filme gibst du mit foreach aus.",
    "l15a3": "Mit Contains prüfst du, ob der Name auf der Liste steht, mit Remove entfernst du ihn. Die Anzahl gibst du ganz am Ende aus – in beiden Fällen.",
    "l16a1": "Die Methode steht unter dem Hauptcode und hat keine Parameter. Rufst du sie auf, brauchst du die runden Klammern.",
    "l16a2": "Der Parameter zahl ist innerhalb der Methode wie eine Variable nutzbar. Die Ausgabe passiert in der Methode selbst.",
    "l17a1": "Vergleiche a und b mit if. Statt etwas auszugeben, lieferst du die größere Zahl mit return zurück.",
    "l17a2": "Die Methode rechnet den Bruttopreis aus und gibt ihn mit return zurück. Die Formatierung :F2 machst du erst beim Ausgeben.",
    "l17a4": "Starte mit einem Ergebnis von 1 und multipliziere in einer Schleife nacheinander mit 2, 3, … bis n. long ist nötig, weil die Zahlen schnell groß werden.",
    "l18a3": "Split liefert ein Array – seine Length ist die Anzahl der Wörter. Fürs längste Wort vergleichst du in einer Schleife die Length jedes Wortes.",
    "l19a1": "int.TryParse liefert true oder false und schreibt die Zahl über out in eine Variable. Benutze den Rückgabewert direkt im if.",
    "l19a2": "Setz int.TryParse in die Bedingung einer while-Schleife. Solange es nicht klappt, gibst du die Bitte aus und liest neu ein.",
    "l19a3": "Eine for-Schleife mit 5 Durchläufen liest jede Eingabe. Je nach Ergebnis von TryParse addierst du zur Summe oder zählst die ungültigen.",
    "l20a1": "Math.Sqrt zieht die Wurzel. Die Methode bekommt a und b und gibt das Ergebnis mit return zurück.",
    "l20a2": "Math.PI ist die Kreiszahl π, r² rechnest du mit r * r oder Math.Pow. Für 2 Nachkommastellen nimmst du :F2.",
    "l21a1": "Felder schreibst du direkt in die Klasse, die Methode Info auch. In der Methode kannst du Marke und PS ohne Objektnamen benutzen.",
    "l21a2": "Die Methode Flaeche hat den Rückgabetyp double und liefert Breite mal Hoehe mit return. Breite und Hoehe setzt du im Hauptcode.",
    "l21a3": "Beide Methoden ändern nur das Feld Wert. Sie sind public void und brauchen keine Parameter.",
    "l22a1": "Ein Konstruktor heißt genau wie die Klasse und hat keinen Rückgabetyp. Er bekommt zwei Parameter und speichert sie in den Feldern.",
    "l22a2": "Im Konstruktor setzt du Inhaber und Startbetrag. In Abheben prüfst du zuerst mit if, ob genug Geld da ist.",
    "l22a3": "Heißen Parameter und Feld gleich, meint this.X das Feld und X den Parameter. AlsText baut den Text mit $\"...\" zusammen.",
    "l23a2": "Auto-Eigenschaften schreibst du kurz mit { get; set; }. Für den Rabatt ziehst du den Prozentanteil vom Preis ab – denk an das Teilen durch 100.0.",
    "l23a3": "Einzahlen gehört in die Klasse und darf Kontostand ändern, weil private set innerhalb der Klasse erlaubt ist. Prüfe vorher mit if, ob der Betrag größer als 0 ist.",
    "l24a1": "Erzeuge jeden Schüler mit new Schueler(...) und füge ihn mit Add hinzu. Für den Durchschnitt addierst du in der foreach-Schleife die Noten und teilst danach.",
    "l24a2": "Merk dir das erste Produkt als bisher teuerstes. Vergleiche in der Schleife den Preis jedes Produkts damit.",
    "l24a3": "Lies in einer for-Schleife Name und Punkte ein und lege dafür je ein Objekt an. Danach gibst du in einer zweiten Schleife nur die mit mindestens 50 Punkten aus und zählst mit.",
    "l25a2": "Jede Methode bekommt die Liste als Parameter und gibt einen Wert zurück. Für Beste und Schlechteste gehst du die Noten mit einer Schleife durch.",
    "l25a3": "Schreib eine Methode, die den Durchschnitt bekommt und den passenden Text zurückgibt. Prüfe die Grenzen von klein nach groß mit if.",
    "l26a1": "Lies den Tipp mit int.Parse ein. Mit if, else if und else deckst du alle drei Fälle ab.",
    "l26a2": "Eine while-Schleife läuft, solange der Tipp nicht die geheime Zahl ist. Zähle in jedem Durchlauf die Versuche hoch und gib das Ergebnis erst nach der Schleife aus.",
    "l27a1": "Die Schleife läuft, solange eine bool-Variable true ist – Befehl 0 setzt sie auf false. Mit switch auf den eingelesenen Befehl wählst du den Fall aus.",
}

# Punkt 8: Erklärungen für Quizfragen ohne Erklärung, Schlüssel (Lektion, Index)
QUIZ_EXPLANATIONS = {
    ("l08", 0): "Mit = speicherst du einen Wert in einer Variablen, mit == vergleichst du zwei Werte und bekommst true oder false.",
    ("l08", 1): "else ist der „sonst“-Fall: Er läuft genau dann, wenn die Bedingung im if falsch ist.",
    ("l10", 0): "default ist der Auffang-Fall und läuft, wenn keiner der case-Werte passt.",
    ("l10", 1): "break; beendet den case – ohne break meldet C# einen Fehler, damit nicht versehentlich der nächste case mitläuft.",
    ("l10", 2): "Der Unterstrich _ passt auf jeden Wert und steht deshalb für „alles andere“.",
    ("l11", 0): "Ohne i++ bleibt i immer gleich, die Bedingung i < 5 bleibt wahr – die Schleife hört nie auf.",
    ("l11", 1): "do-while prüft die Bedingung erst am Ende, deshalb läuft der Block immer mindestens einmal.",
    ("l11", 2): "break; verlässt die Schleife sofort, und das Programm macht nach der Schleife weiter.",
    ("l12", 1): "Der dritte Teil im for-Kopf, der Schritt wie i++, wird nach jedem Durchlauf ausgeführt – danach wird die Bedingung neu geprüft.",
    ("l12", 2): "i ist erst 10, dann 5; danach ist i 0, und 0 > 0 ist falsch – also erscheinen nur 10 und 5.",
    ("l13", 0): "In C# beginnt die Zählung bei 0, das erste Element ist also zahlen[0].",
    ("l13", 1): "Bei 5 Fächern gehen die Indizes von 0 bis 4 – der letzte ist immer Length - 1.",
    ("l13", 2): "Ein Array hat eine feste Größe; ein Zugriff außerhalb löst beim Ausführen eine IndexOutOfRangeException aus.",
    ("l14", 0): "foreach legt bei jedem Durchlauf das nächste Element in x – den Index bekommst du damit nicht.",
    ("l14", 1): "for hat eine Zählvariable; die brauchst du, wenn du den Index wissen oder Elemente gezielt ansprechen willst.",
    ("l14", 2): "Ein string besteht aus einzelnen Zeichen (char), und foreach geht sie der Reihe nach durch.",
    ("l15", 0): "Add hängt ein neues Element hinten an die Liste an.",
    ("l15", 1): "Listen haben .Count, Arrays dagegen .Length.",
    ("l15", 2): "Ein Array hat eine feste Größe, eine Liste wächst mit Add und schrumpft mit Remove.",
    ("l16", 0): "void heißt „leer“ im Sinne von: Die Methode liefert keinen Wert zurück.",
    ("l16", 1): "Beim Aufruf schreibst du den Namen mit runden Klammern und einem Semikolon – auch wenn es keine Parameter gibt.",
    ("l17", 0): "return liefert den Wert an die Stelle zurück, an der die Methode aufgerufen wurde, und beendet sie sofort.",
    ("l17", 1): "true und false sind Wahrheitswerte, und deren Typ heißt bool.",
    ("l18", 0): "Length zählt die Zeichen: H-a-l-l-o sind 5.",
    ("l18", 2): "Split zerlegt den Text am Leerzeichen und liefert ein string-Array: \"Hallo\" und \"Welt\".",
    ("l19", 0): "TryParse meldet mit true oder false, ob die Umwandlung geklappt hat; die Zahl selbst kommt über out.",
    ("l19", 1): "Klappt die Umwandlung nicht, setzt TryParse die out-Variable auf 0 und gibt false zurück.",
    ("l19", 2): "Parse stürzt bei einer falschen Eingabe mit einem Fehler ab, TryParse gibt einfach false zurück.",
    ("l20", 0): "Math.Pow(3, 2) bedeutet 3 hoch 2, also 3 · 3 = 9.",
    ("l20", 2): "Math.Ceiling rundet immer zur nächsten ganzen Zahl nach oben, Math.Floor immer nach unten.",
    ("l21", 0): "Die Klasse beschreibt, welche Felder und Methoden ein Objekt hat – wie ein Bauplan für viele Häuser.",
    ("l21", 1): "Mit new und dem Klassennamen, z. B. new Auto(), entsteht ein neues Objekt.",
    ("l21", 2): "Du schreibst den Objektnamen, einen Punkt und den Methodennamen mit Klammern.",
    ("l22", 0): "Ein Konstruktor trägt genau den Namen seiner Klasse, daran erkennt C# ihn.",
    ("l22", 1): "Ein Konstruktor hat keinen Rückgabetyp, nicht einmal void.",
    ("l22", 2): "Der Konstruktor läuft automatisch, sobald du mit new ein Objekt erzeugst.",
    ("l23", 0): "private Felder und Methoden kann nur Code innerhalb derselben Klasse benutzen.",
    ("l23", 1): "Im set-Teil steht der zugewiesene Wert immer im Schlüsselwort value.",
    ("l23", 2): "get ist öffentlich, set ist private – lesen darf jeder, ändern nur die Klasse selbst.",
    ("l24", 0): "In den spitzen Klammern steht der Typ der Elemente, hier Auto.",
    ("l24", 1): "Console.WriteLine ruft ToString() auf; überschreibst du die Methode, bestimmst du den ausgegebenen Text.",
    ("l24", 2): "Das ist ein Lambda-Ausdruck: Für jedes Element s wird s.Note genommen, z. B. für Average.",
    ("l25", 0): "Kleine Schritte kannst du einzeln testen – so findest du Fehler viel schneller.",
    ("l25", 1): "Average() berechnet direkt den Durchschnitt aller Zahlen der Liste – .Sum() liefert nur die Summe.",
    ("l26", 1): "continue; überspringt den Rest des aktuellen Durchlaufs und macht mit dem nächsten weiter.",
    ("l27", 0): "Eine Hauptschleife fragt immer wieder nach dem nächsten Befehl, bis der Benutzer beendet.",
    ("l27", 1): "Remove gibt true zurück, wenn etwas entfernt wurde, sonst false – es stürzt nicht ab.",
    ("l27", 2): "null bedeutet, dass die Variable auf kein Objekt zeigt – zum Beispiel, wenn eine Suche nichts gefunden hat.",
}

# Punkt 9: je eine dritte Quizfrage für l25 und l26
EXTRA_QUIZ = {
    "l25": gen.Q("Was liefert noten.Count bei einer Liste mit den Noten 2, 1 und 3?",
                 ["1", "2", "3", "6"], 2,
                 "Count zählt die Elemente der Liste – hier sind es drei Noten."),
    "l26": gen.Q("Wie zählst du in einer Schleife die Versuche mit?",
                 ["versuche == 1;", "versuche++;", "versuche = versuche;", "int versuche++;"], 1,
                 "versuche++ erhöht die Zählvariable bei jedem Durchlauf um 1."),
}


def anwenden():
    lektionen = {l["id"]: l for l in gen.LESSONS}
    aufgaben = {a["id"]: a for l in gen.LESSONS for a in l["exercises"]}

    for aid, reqs in REQUIREMENTS.items():
        a = aufgaben[aid]
        assert not a["requirements"], f"{aid} hat schon Anforderungen"
        a["requirements"] = [{"pattern": p, "message": m} for p, m in reqs]

    for aid, tipp in HINTS.items():
        a = aufgaben[aid]
        assert not a["hint"].strip(), f"{aid} hat schon einen Tipp"
        a["hint"] = tipp

    for (lid, i), text in QUIZ_EXPLANATIONS.items():
        q = lektionen[lid]["quiz"][i]
        assert not q["explanation"].strip(), f"{lid}/{i} hat schon eine Erklärung"
        q["explanation"] = text

    for lid, q in EXTRA_QUIZ.items():
        lektionen[lid]["quiz"].append(q)


anwenden()
