r"""Überarbeitete Aufgabenstellungen (Nov 2026, Wunsch von Willi).

Regeln für die Texte:
- Kurz und klar: Was soll das Programm tun? Was kommt rein, was soll rauskommen?
- Kein Lösungsweg: keine Syntax, keine Operatoren, keine Methodennamen, keine Hinweise auf Prüfmuster.
  Nur Namen, die wirklich vorgegeben sein müssen (Variablen-/Methoden-/Klassennamen, die geprüft werden).
- Leerzeichen und Leerzeilen sind egal – die Prüfung drückt hier ein Auge zu.
- Tipps zeigen nur die Richtung. Meldungen bei fehlenden Bausteinen sind sanfte Stupser.

X(id, task, hint, m=[...])  überschreibt Text, Tipp und (optional) die Meldungen der Anforderungen in
                              gleicher Reihenfolge. m-Einträge mit None bleiben unverändert.
X(..., reqs=[(muster, meldung[, raw])])  ersetzt die Anforderungen komplett.
Wird von build.py ganz zuletzt geladen.
"""
import textwrap
import gen

_alle = {}


def _lektion(lid):
    return next(l for l in gen.LESSONS if l["id"] == lid)


def X(aid, task, hint=None, m=None, reqs=None, sol=None, tests=None):
    _alle[aid] = (task, hint, m, reqs, sol, tests)


def anwenden():
    gefunden = set()
    for l in gen.LESSONS:
        for a in l["exercises"]:
            if a["id"] not in _alle:
                continue
            task, hint, m, reqs, sol, tests = _alle[a["id"]]
            if sol is not None:
                a["solution"] = textwrap.dedent(sol).strip("\n") + "\n"
            if tests is not None:
                a["tests"] = tests
            a["task"] = textwrap.dedent(task).strip()
            if hint is not None:
                a["hint"] = hint
            if reqs is not None:
                a["requirements"] = [
                    ({"pattern": r[0], "message": r[1], "raw": True} if len(r) > 2 and r[2]
                     else {"pattern": r[0], "message": r[1]}) for r in reqs]
            if m is not None:
                assert len(m) == len(a["requirements"]), (a["id"], len(m), len(a["requirements"]))
                for r, msg in zip(a["requirements"], m):
                    if msg is not None:
                        r["message"] = msg
            gefunden.add(a["id"])
    fehlt = set(_alle) - gefunden
    assert not fehlt, f"Unbekannte Aufgaben-IDs: {sorted(fehlt)}"


def Z(*zeilen):
    """Ausgabe-Block für den Aufgabentext."""
    return "```\n" + "\n".join(zeilen) + "\n```"


def B(*paare):
    """Beispiele als Block: B(("4", "gerade"), ("7", "ungerade")) -> Eingabe → Ausgabe."""
    return "```\n" + "\n".join(f"{e}  →  {a}" for e, a in paare) + "\n```"


FRAGE = "Eine Frage vor der Eingabe (z. B. „Zahl:“) ist erlaubt."
