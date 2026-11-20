r"""Startcode ohne Lösungszeilen (Nov 2026, Willi: „ich habe noch nichts eingetragen, aber die Konsole ist vorausgefüllt“).

Wird von build.py NACH aufgabentexte.py geladen. Jede Aufgabe beginnt mit STANDARD – außer die Zeilen sind zwingend
Material der Aufgabe (siehe AUSNAHMEN, gleiche Liste wie im Test ReviewAbnahmeTests.StartcodeMaterial):
Kommentar-/Reparatur-Übungen, vorgegebener Testcode für eine zu schreibende Methode/Klasse, gegebene Daten
(Arrays/Listen), eine vorgegebene Klasse, die ergänzt wird, und die Fortsetzung l27a2.
Bei Aufgaben mit Ausnahme werden Lösungszeilen gezielt entfernt (BEREINIGT).
"""
import gen

STANDARD = "// Schreib hier deinen Code\n"

AUSNAHMEN = {
    "l02a1": "Kommentar-Übung: die Zeilen sind das Material",
    "l02a2": "Kommentar-Übung: die Zeile ist das Material",
    "l02a3": "Kommentar-Übung: die Zeilen sind das Material",
    "l07a1": "Der klassische Aufbau (Klasse, Main) ist das Material – ergänzt wird nur der Befehl",
    "l07a2": "Reparatur-Übung: das kaputte Programm ist das Material",
    "l13a3": "gegebene Daten (Array)",
    "l14a1": "gegebene Daten (Array)",
    "l14a2": "gegebene Daten (Array)",
    "l15a3": "gegebene Daten (Gästeliste)",
    "l24a2": "gegebene Daten (Produktliste mit Klasse)",
    "l16a2": "Testcode ruft die zu schreibende Methode auf",
    "l16a3": "Testcode ruft die zu schreibende Methode auf",
    "l17a1": "Testcode ruft die zu schreibende Methode auf",
    "l21a1": "Testcode nutzt die zu schreibende Klasse",
    "l21a3": "Testcode nutzt die zu schreibende Klasse",
    "l22a2": "Testcode nutzt die zu schreibende Klasse",
    "l22a3": "Testcode nutzt die zu schreibende Klasse",
    "l23a2": "Testcode nutzt die zu schreibende Klasse",
    "l22a1": "vorgegebene Klasse, die ergänzt wird",
    "l23a1": "vorgegebene Klasse, die ergänzt wird",
    "l23a3": "vorgegebene Klasse, die ergänzt wird",
    "l24a1": "vorgegebene Klasse und Liste, die befüllt wird",
    "l24a3": "vorgegebene Klasse und Liste, die befüllt wird",
    "l27a3": "vorgegebene Klasse und Liste",
    "l27a2": "Fortsetzung von l27a1",
}

# Bei Ausnahmen: nur die Lösungszeilen entfernen, Material bleibt.
BEREINIGT = {
    "l13a3": "int[] werte = { 12, 45, 7, 89, 23, 56 };\n",
    "l15a3": 'List<string> gaeste = new List<string> { "Ana", "Ben", "Cem" };\n',
    "l21a3": "Zaehler z = new Zaehler();\nz.Erhoehen(); z.Erhoehen(); z.Erhoehen();\nConsole.WriteLine(z.Wert);\nz.Zuruecksetzen();\nz.Erhoehen();\nConsole.WriteLine(z.Wert);\n",
}

# Texte, die bisher auf Startcode verwiesen haben
TEXTE = {
    "l03a2": ('Gegeben sind die Variablen', 'Lege die Variablen `tier` mit dem Wert `"Katze"` und `beine` mit dem Wert `4` an. Bau daraus diesen Satz zusammen – `Katze` und `4` sollen aus den Variablen kommen, nicht von Hand eingetippt:\n\n```\nEine Katze hat 4 Beine.\n```'),
    "l25a2": ('Ergänze Methoden', 'Lies wie in Schritt 1 zuerst die Anzahl der Noten und dann die Noten selbst ein. Ergänze Methoden `Durchschnitt`, `Beste` (kleinste Zahl) und `Schlechteste` (größte Zahl) und gib die Auswertung aus. Bei `4`, `2`, `1`, `3`, `2`:\n\n```\nDurchschnitt: 2,00\nBeste Note: 1\nSchlechteste Note: 3\n```'),
}


def anwenden():
    aufgaben = {a["id"]: a for l in gen.LESSONS for a in l["exercises"]}
    for aid, a in aufgaben.items():
        if aid in BEREINIGT:
            a["starter"] = BEREINIGT[aid]
        elif aid not in AUSNAHMEN:
            a["starter"] = STANDARD
    for aid, (anfang, text) in TEXTE.items():
        a = aufgaben[aid]
        assert a["task"].startswith(anfang), aid
        a["task"] = text


anwenden()
