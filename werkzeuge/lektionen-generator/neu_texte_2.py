"""Neue Aufgabentexte Lektion 7–10."""
from neu_texte import X, Z, B, FRAGE

# ------------------------------------------------------------------ L07
X("l07a1", "Im Editor steht schon das Grundgerüst eines Programms. Ergänze nur den Befehl, der diese Zeile ausgibt, und lass das Gerüst unverändert:\n\n" + Z("Programm gestartet"),
  "Dein Code gehört in die Methode, mit der das Programm startet. Dort steht gerade nur eine Notiz.")
X("l07a2", "Dieses Programm läuft nicht, weil im Aufbau etwas fehlt. Finde den Fehler und repariere ihn, sodass es dies ausgibt:\n\n" + Z("Alle Klammern stimmen!"),
  "Zu jeder öffnenden geschweiften Klammer gehört eine schließende. Zähl nach, welche fehlt.")
X("l07a3", "Schreib das Programm im klassischen Aufbau mit der Klasse `Program` und der Startmethode `Main`, so wie in der Lektion. Die Stadt `Hamburg` kommt aus einer Variablen:\n\n" + Z("Ich wohne in Hamburg."),
  "Der Aufbau besteht aus einer Klasse, und darin liegt die Methode, in der das Programm startet.",
  m=["Das Programm braucht eine Klasse namens `Program`.", "Das Programm braucht die Startmethode `Main`."])
X("l07a4", "Gib diese zwei Zeilen aus. Schreib das Programm im klassischen Aufbau mit der Klasse `Program` und der Startmethode `Main`:\n\n" + Z("Guten Tag!", "Heute lernen wir C#."),
  "Beide Befehle gehören in die Startmethode.",
  m=["Das Programm braucht eine Klasse namens `Program`.", "Das Programm braucht die Startmethode `Main`."])
X("l07a5", "Speichere die Zahl `2026` in einer Variablen `jahr` und gib damit den Satz aus. Schreib das Programm im klassischen Aufbau (Klasse `Program`, Startmethode `Main`):\n\n" + Z("Wir haben 2026."),
  "Die Variable gehört in die Startmethode, genau wie die Ausgabe.",
  m=["Das Programm braucht eine Klasse namens `Program`.", "Das Programm braucht die Startmethode `Main`.", "Lege die Variable `jahr` als ganze Zahl an."])
X("l07a6", "Schreib das Programm im klassischen Aufbau (Klasse `Program`, Startmethode `Main`) und stecke alles in einen eigenen Namensraum mit dem Namen `Schule`:\n\n" + Z("Willkommen in der Schule!"),
  "Der Namensraum umschließt die Klasse, so wie im Beispiel der Lektion.",
  m=["Das Programm braucht eine Klasse namens `Program`.", "Das Programm braucht die Startmethode `Main`.", "Die Klasse soll in einem Namensraum namens `Schule` liegen."])
X("l07a7", "Das Programm liest einen Namen ein und begrüßt die Person. Schreib es im klassischen Aufbau (Klasse `Program`, Startmethode `Main`). " + FRAGE + "\n\n" + B(("Ben", "Schön, dich zu sehen, Ben!"), ("Aylin", "Schön, dich zu sehen, Aylin!")),
  "Alles gehört in die Startmethode.",
  m=["Das Programm braucht eine Klasse namens `Program`.", "Das Programm braucht die Startmethode `Main`."])
X("l07a8", "Das Programm liest zwei Preise ein (Kommazahlen) und gibt die Summe mit zwei Nachkommastellen aus. Schreib es im klassischen Aufbau (Klasse `Program`, Startmethode `Main`). " + FRAGE + "\n\n" + B(("2,49 und 4,99", "Summe: 7,48 Euro"), ("1 und 2,5", "Summe: 3,50 Euro")),
  "Alles gehört in die Startmethode.",
  m=["Das Programm braucht eine Klasse namens `Program`.", "Das Programm braucht die Startmethode `Main`."])

# ------------------------------------------------------------------ L08
X("l08a1", "Das Programm liest eine ganze Zahl ein und sagt, ob sie gerade oder ungerade ist. " + FRAGE + "\n\n" + B(("4", "gerade"), ("7", "ungerade"), ("0", "gerade")),
  "Eine Zahl ist gerade, wenn beim Teilen durch 2 nichts übrig bleibt. Danach entscheidet das Programm zwischen zwei Möglichkeiten.",
  m=["Das Programm soll hier eine Entscheidung treffen. Dafür gibt es in dieser Lektion eine Anweisung.", "Für den anderen Fall gibt es ein eigenes Schlüsselwort. Schau in die Lektion."])
X("l08a2", "Das Programm liest eine Temperatur ein (ganze Zahl) und bewertet sie: unter 10 Grad ist es `kalt`, von 10 bis 24 Grad `angenehm`, ab 25 Grad `heiß`. " + FRAGE + "\n\n" + B(("3", "kalt"), ("10", "angenehm"), ("24", "angenehm"), ("25", "heiß"), ("-5", "kalt")),
  "Du hast drei Fälle. Die Lektion zeigt, wie man mehr als zwei Fälle nacheinander prüft.",
  m=["Drei Fälle brauchen mehr als eine einfache Entscheidung. Die Lektion zeigt, wie man weitere Bedingungen anhängt."])
X("l08a3", "Das Programm liest zwei ganze Zahlen ein und sagt, welche größer ist. Sind beide gleich, sagt es das auch. " + FRAGE + "\n\n" + B(("9 und 4", "Die größere Zahl ist 9"), ("3 und 12", "Die größere Zahl ist 12"), ("5 und 5", "Beide sind gleich")),
  "Drei mögliche Ergebnisse: erste größer, zweite größer, gleich.")
X("l08a4", "Das Programm liest ein Alter ein. Ab 18 ist man volljährig. " + FRAGE + "\n\n" + B(("17", "Du bist minderjährig."), ("18", "Du bist volljährig."), ("30", "Du bist volljährig.")),
  "Achte darauf, was bei genau 18 passiert.")
X("l08a5", "Das Programm liest eine ganze Zahl ein und sagt, ob sie positiv, negativ oder null ist. " + FRAGE + "\n\n" + B(("-3", "Die Zahl ist negativ."), ("0", "Die Zahl ist null."), ("5", "Die Zahl ist positiv.")),
  "Es gibt drei Fälle. Überleg dir, in welcher Reihenfolge du sie prüfst.")
X("l08a6", "Das Programm liest erreichte Punkte ein und bewertet sie: ab 90 `sehr gut`, ab 75 `gut`, ab 50 `bestanden`, darunter `nicht bestanden`. " + FRAGE + "\n\n" + B(("80", "Ergebnis: gut"), ("90", "Ergebnis: sehr gut"), ("50", "Ergebnis: bestanden"), ("49", "Ergebnis: nicht bestanden")),
  "Prüf von der höchsten Grenze nach unten. Dann musst du nicht auch noch die obere Grenze jedes Bereichs prüfen.")
X("l08a7", "In einem Online-Shop gibt es ab 50 Euro Einkaufswert 10 % Rabatt. Das Programm liest den Betrag ein (Kommazahl) und gibt aus, was zu zahlen ist, mit zwei Nachkommastellen. " + FRAGE + "\n\n" + B(("60", "Zu zahlen: 54,00 Euro"), ("40", "Zu zahlen: 40,00 Euro"), ("50", "Zu zahlen: 45,00 Euro")),
  "Der Rabatt greift nur in einem Fall. Danach wird der (eventuell geänderte) Betrag ausgegeben.")
X("l08a8", "Das Programm liest eine Jahreszahl ein und sagt, ob es ein Schaltjahr ist. Ein Jahr ist ein Schaltjahr, wenn es durch 4 teilbar ist. Ausnahme: Jahre, die durch 100 teilbar sind, sind keine Schaltjahre, außer sie sind auch durch 400 teilbar. " + FRAGE + "\n\n" + B(("2024", "2024 ist ein Schaltjahr."), ("1900", "1900 ist kein Schaltjahr."), ("2000", "2000 ist ein Schaltjahr."), ("2023", "2023 ist kein Schaltjahr.")),
  "Die Regeln haben eine Reihenfolge. Fang mit der strengsten Ausnahme an.")

# ------------------------------------------------------------------ L09
X("l09a1", "Das Programm liest eine ganze Zahl ein und sagt, ob sie zwischen 1 und 100 liegt (beide Grenzen gehören dazu). " + FRAGE + "\n\n" + B(("1", "im Bereich"), ("100", "im Bereich"), ("0", "außerhalb"), ("101", "außerhalb"), ("50", "im Bereich")),
  "Die Zahl muss zwei Bedingungen gleichzeitig erfüllen.",
  m=["Die Zahl muss zwei Bedingungen gleichzeitig erfüllen. Dafür gibt es in dieser Lektion einen eigenen Operator."])
X("l09a2", "Das Programm liest einen Wochentag ein. Samstag und Sonntag sind Wochenende, jeder andere Tag ist ein Arbeitstag. " + FRAGE + "\n\n" + B(("Samstag", "Wochenende!"), ("Sonntag", "Wochenende!"), ("Montag", "Arbeitstag")),
  "Es reicht, wenn eine von zwei Bedingungen stimmt.",
  m=["Hier reicht es, wenn eine von zwei Bedingungen stimmt. Dafür gibt es in dieser Lektion einen eigenen Operator."])
X("l09a3", "An der Kinokasse gilt: Ab 16 darf man allein in den Film. Ab 12 darf man nur mit Begleitung einer erwachsenen Person hinein. Das Programm liest das Alter und danach die Antwort `ja` oder `nein` auf die Frage nach der Begleitung ein. " + FRAGE + "\n\n" + B(("16 und nein", "Viel Spaß im Film!"), ("13 und ja", "Viel Spaß im Film!"), ("13 und nein", "Leider nicht erlaubt."), ("10 und ja", "Leider nicht erlaubt.")),
  "Zwei Wege führen zum Ziel: alt genug, oder alt genug mit Begleitung. Überleg dir, welche Bedingungen du kombinierst.")
X("l09a4", "Das Programm liest einen Benutzernamen und danach ein Passwort ein. Nur der Benutzer `admin` mit dem Passwort `1234` bekommt Zugang. " + FRAGE + "\n\n" + B(("admin und 1234", "Zugang erlaubt"), ("admin und 0000", "Zugang verweigert"), ("gast und 1234", "Zugang verweigert")),
  "Beide Angaben müssen stimmen.",
  m=["Beide Angaben müssen gleichzeitig stimmen. Dafür gibt es in dieser Lektion einen eigenen Operator."])
X("l09a5", "Das Programm liest eine Monatsnummer ein. Juli und August sind Sommerferien, alle anderen Monate Schulzeit. " + FRAGE + "\n\n" + B(("7", "Sommerferien!"), ("8", "Sommerferien!"), ("9", "Schulzeit.")),
  "Es reicht, wenn einer der beiden Monate stimmt.",
  m=["Hier reicht es, wenn eine von zwei Bedingungen stimmt. Dafür gibt es in dieser Lektion einen eigenen Operator."])
X("l09a6", "Das Programm liest einen Text ein. Ist er nicht leer, bedankt es sich dafür. Wurde einfach nur Enter gedrückt, sagt es das. " + FRAGE + "\n\n" + B(("Hallo", "Danke für: Hallo"), ("(nur Enter)", "Du hast nichts eingegeben.")),
  "Ein leerer Text ist einfach ein Text ohne Zeichen. Den kannst du vergleichen.")
X("l09a7", "Das Programm liest ein Alter ein und sagt, ob die Person ein Teenager ist. Teenager sind 13 bis 19 Jahre alt. " + FRAGE + "\n\n" + B(("15", "Du bist ein Teenager."), ("13", "Du bist ein Teenager."), ("19", "Du bist ein Teenager."), ("12", "Du bist kein Teenager."), ("20", "Du bist kein Teenager.")),
  "Das Alter muss zwei Grenzen gleichzeitig einhalten.",
  m=["Das Alter muss zwei Bedingungen gleichzeitig erfüllen. Dafür gibt es in dieser Lektion einen eigenen Operator."])
X("l09a8", "Der Wetter-Berater liest eine Temperatur ein und danach `ja` oder `nein` auf die Frage, ob es regnet. Speichere das Regenwetter in einer Variablen `regen`, die wahr oder falsch ist. Dann gilt: Bei Regen gibt es einen Schirm-Tipp. Ohne Regen und ab 20 Grad geht es ins Freibad. Sonst gibt es den Jacken-Tipp. " + FRAGE + "\n\n" + B(("25 und nein", "Ab ins Freibad!"), ("25 und ja", "Regenschirm nicht vergessen!"), ("12 und nein", "Jacke an und los!")),
  "Die Antwort `ja` oder `nein` kannst du direkt in einen Wahrheitswert verwandeln, indem du sie mit `ja` vergleichst.",
  m=["Speichere in `regen` als Wahrheitswert, ob es regnet."])

# ------------------------------------------------------------------ L10
X("l10a1", "Das Programm liest eine Zahl von 1 bis 7 ein und gibt den passenden Wochentag aus (1 ist Montag, 7 ist Sonntag). Jede andere Zahl ergibt `Ungültig`. Löse es mit der Mehrfachauswahl aus dieser Lektion. " + FRAGE + "\n\n" + B(("1", "Montag"), ("3", "Mittwoch"), ("7", "Sonntag"), ("8", "Ungültig"), ("0", "Ungültig")),
  "Jede Zahl ist ein eigener Fall. Für alle übrigen Zahlen gibt es einen Auffangfall.",
  m=["Nutze die Mehrfachauswahl aus dieser Lektion.", "Für alle anderen Zahlen braucht die Mehrfachauswahl einen Auffangfall."])
X("l10a2", "Mini-Taschenrechner: Das Programm liest zwei ganze Zahlen und danach ein Rechenzeichen (`+`, `-`, `*` oder `/`) ein und gibt das Ergebnis aus. Bei jedem anderen Zeichen meldet es `Unbekanntes Zeichen`. Löse es mit der Mehrfachauswahl. " + FRAGE + "\n\n" + B(("8, 2, *", "Ergebnis: 16"), ("8, 2, +", "Ergebnis: 10"), ("8, 2, -", "Ergebnis: 6"), ("8, 2, /", "Ergebnis: 4"), ("8, 2, ?", "Unbekanntes Zeichen")),
  "Die Mehrfachauswahl kann auch Text vergleichen, nicht nur Zahlen.",
  m=["Nutze die Mehrfachauswahl aus dieser Lektion."])
X("l10a3", "Das Programm liest eine Monatsnummer ein und nennt die Jahreszeit: Dezember bis Februar Winter, März bis Mai Frühling, Juni bis August Sommer, September bis November Herbst. Alles andere ergibt `Ungültiger Monat`. Löse es mit der Mehrfachauswahl. " + FRAGE + "\n\n" + B(("1", "Winter"), ("12", "Winter"), ("4", "Frühling"), ("8", "Sommer"), ("11", "Herbst"), ("13", "Ungültiger Monat")),
  "Mehrere Fälle dürfen zu derselben Ausgabe führen.",
  m=["Nutze die Mehrfachauswahl aus dieser Lektion."])
X("l10a4", "Eine Ampel: Das Programm liest eine Farbe ein. Bei `rot` kommt `Stehen bleiben!`, bei `gelb` `Achtung!`, bei `grün` `Gehen!`. Jede andere Farbe ergibt `Unbekannte Farbe`. Löse es mit der Mehrfachauswahl. " + FRAGE + "\n\n" + B(("rot", "Stehen bleiben!"), ("gelb", "Achtung!"), ("grün", "Gehen!"), ("blau", "Unbekannte Farbe")),
  "Jede Farbe ist ein Fall. Alles andere landet im Auffangfall.",
  m=["Nutze die Mehrfachauswahl aus dieser Lektion."])
X("l10a5", "Das Programm liest eine Schulnote von 1 bis 6 ein und gibt sie als Wort aus (1 sehr gut, 2 gut, 3 befriedigend, 4 ausreichend, 5 mangelhaft, 6 ungenügend). Alles andere ergibt `Keine gültige Note`. Löse es mit der Mehrfachauswahl. " + FRAGE + "\n\n" + B(("3", "befriedigend"), ("1", "sehr gut"), ("6", "ungenügend"), ("7", "Keine gültige Note")),
  "Sechs Fälle plus der Auffangfall für ungültige Zahlen.",
  m=["Nutze die Mehrfachauswahl aus dieser Lektion."])
X("l10a6", "Das Programm liest eine Monatsnummer ein und sagt, wie viele Tage der Monat hat (der Februar zählt 28). Alles andere ergibt `Keinen solchen Monat`. Mehrere Monate mit gleicher Antwort sollen sich einen Fall teilen. " + FRAGE + "\n\n" + B(("4", "30 Tage"), ("1", "31 Tage"), ("12", "31 Tage"), ("2", "28 Tage"), ("13", "Keinen solchen Monat")),
  "Mehrere Fälle dürfen untereinander stehen und teilen sich dann dieselben Befehle.",
  m=["Nutze die Mehrfachauswahl aus dieser Lektion.", "Monate mit gleicher Antwort sollen sich einen Fall teilen. Die Lektion zeigt, wie man Fälle zusammenlegt."])
X("l10a7", "Ein Getränkeautomat: Das Programm liest eine Nummer ein. 1 ist Wasser, 2 Saft, 3 Kakao, alles andere `Gibt es nicht`. Speichere das Getränk zuerst in einer Variablen und gib dann den Satz aus. Löse es mit der kompakten Variante der Mehrfachauswahl, die direkt einen Wert liefert. " + FRAGE + "\n\n" + B(("2", "Du bekommst: Saft"), ("1", "Du bekommst: Wasser"), ("3", "Du bekommst: Kakao"), ("9", "Du bekommst: Gibt es nicht")),
  "In der Lektion gibt es eine zweite, kürzere Schreibweise, die ihr Ergebnis direkt einer Variablen zuweist.",
  m=["Nutze die kompakte Variante der Mehrfachauswahl, die direkt einen Wert liefert."])
X("l10a8", "Das Programm liest einen Wochentag ein. Montag bis Freitag ist `Schule`, Samstag und Sonntag ist `Wochenende`. Alles andere ergibt `Unbekannter Tag`. Mehrere Tage mit gleicher Antwort sollen sich einen Fall teilen. " + FRAGE + "\n\n" + B(("Samstag", "Wochenende"), ("Montag", "Schule"), ("Freitag", "Schule"), ("Sonntag", "Wochenende"), ("Feiertag", "Unbekannter Tag")),
  "Fünf Tage führen zur selben Ausgabe, zwei andere zu einer anderen.",
  m=["Nutze die Mehrfachauswahl aus dieser Lektion.", "Tage mit gleicher Antwort sollen sich einen Fall teilen."])
