"""Hilfen für die zusätzlichen Aufgaben (8 pro Lektion, Nov 2026)."""
import textwrap
import gen
from startcode import STANDARD


def E(text):
    """Erwartete Ausgabe aus einem eingerückten Block."""
    return textwrap.dedent(text).strip("\n")


def N(lid, titel, text, loesung, tests, tipp, vergleich="exact", anf=()):
    lektion = next(l for l in gen.LESSONS if l["id"] == lid)
    a = gen.A(titel, textwrap.dedent(text), STANDARD, textwrap.dedent(loesung).strip("\n") + "\n",
              tests, tipp, vergleich, anf)
    a["id"] = f"{lid}a{len(lektion['exercises']) + 1}"
    lektion["exercises"].append(a)
    return a


T = gen.T
