import json, textwrap
LESSONS = []
STAGES = [
    {"number": 1, "title": "Grundlagen"},
    {"number": 2, "title": "Kontrollstrukturen"},
    {"number": 3, "title": "Methoden"},
    {"number": 4, "title": "Klassen & Objekte"},
    {"number": 5, "title": "Projekte"},
]
GLOSSARY = []

def T(expected, *inputs):
    return {"input": list(inputs), "expected": expected}

def Q(question, options, correct, explanation=""):
    return {"question": question, "options": options, "correct": correct, "explanation": explanation}

def A(title, task, starter, solution, tests, hint="", compare="exact", requirements=()):
    reqs = []
    for r in requirements:
        d = {"pattern": r[0], "message": r[1]}
        if len(r) > 2 and r[2]: d["raw"] = True
        reqs.append(d)
    return {"title": title, "task": task.strip(), "starter": starter, "solution": solution,
            "hint": hint, "compare": compare, "tests": tests, "requirements": reqs}

def L(stage, id, title, intro, explanation, example, note, exercises, quiz, exampleInput=()):
    for i, e in enumerate(exercises):
        e["id"] = f"{id}a{i+1}"
    LESSONS.append({"id": id, "title": title, "stage": stage, "intro": intro,
                    "explanation": textwrap.dedent(explanation).strip(), "example": example,
                    "exampleInput": list(exampleInput), "note": note, "exercises": exercises, "quiz": quiz})

def G(term, explanation, example=""):
    GLOSSARY.append({"term": term, "explanation": explanation, "example": example})
