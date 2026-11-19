using System.Text.Json.Serialization;

namespace CSharpLernen.Engine;

/// <summary>Der komplette Kurs: alle Lektionen und das Begriffe-Lexikon.</summary>
public sealed class Kurs
{
    [JsonPropertyName("stages")] public List<Stufe> Stufen { get; set; } = new();
    [JsonPropertyName("lessons")] public List<Lektion> Lektionen { get; set; } = new();
    [JsonPropertyName("glossary")] public List<Begriff> Begriffe { get; set; } = new();
}

public sealed class Lektion
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
    [JsonPropertyName("title")] public string Titel { get; set; } = "";
    [JsonPropertyName("stage")] public int Stufe { get; set; } = 1;
    /// <summary>Ein, zwei Sätze Einleitung unter dem Titel.</summary>
    [JsonPropertyName("intro")] public string Einleitung { get; set; } = "";
    /// <summary>Hinweis-Kasten unter dem Beispiel.</summary>
    [JsonPropertyName("note")] public string Hinweis { get; set; } = "";
    [JsonPropertyName("quiz")] public List<Quizfrage> Quiz { get; set; } = new();
    /// <summary>Erklärung in einfachem Markdown (##, -, **fett**, `code`, ```-Blöcke).</summary>
    [JsonPropertyName("explanation")] public string Erklaerung { get; set; } = "";
    [JsonPropertyName("example")] public string Beispiel { get; set; } = "";
    [JsonPropertyName("exampleInput")] public List<string> BeispielEingabe { get; set; } = new();
    [JsonPropertyName("exercises")] public List<Aufgabe> Aufgaben { get; set; } = new();

    public override string ToString() => Titel;
}

public sealed class Aufgabe
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
    [JsonPropertyName("title")] public string Titel { get; set; } = "";
    /// <summary>Aufgabenstellung in einfachem Markdown.</summary>
    [JsonPropertyName("task")] public string Aufgabentext { get; set; } = "";
    [JsonPropertyName("starter")] public string Startcode { get; set; } = "";
    [JsonPropertyName("solution")] public string Loesung { get; set; } = "";
    [JsonPropertyName("hint")] public string Tipp { get; set; } = "";
    /// <summary>"exact" = Ausgabe muss genau passen, "lenient" = erwartete Zeilen müssen (am Zeilenende) vorkommen, Eingabeaufforderungen sind egal.</summary>
    [JsonPropertyName("compare")] public string Vergleich { get; set; } = "exact";
    [JsonPropertyName("tests")] public List<Testfall> Tests { get; set; } = new();
    [JsonPropertyName("requirements")] public List<Anforderung> Anforderungen { get; set; } = new();

    public override string ToString() => Titel;
}

public sealed class Testfall
{
    [JsonPropertyName("input")] public List<string> Eingabe { get; set; } = new();
    [JsonPropertyName("expected")] public string Erwartet { get; set; } = "";
}

/// <summary>Ein Regex, der im Code (ohne Kommentare) vorkommen muss, z. B. "\bfor\s*\(".</summary>
public sealed class Anforderung
{
    [JsonPropertyName("pattern")] public string Muster { get; set; } = "";
    [JsonPropertyName("message")] public string Meldung { get; set; } = "";
    /// <summary>true = im Originalcode suchen (mit Kommentaren), z. B. um Kommentare zu verlangen.</summary>
    [JsonPropertyName("raw")] public bool MitKommentaren { get; set; }
}

public sealed class Begriff
{
    [JsonPropertyName("term")] public string Wort { get; set; } = "";
    [JsonPropertyName("explanation")] public string Erklaerung { get; set; } = "";
    [JsonPropertyName("example")] public string Beispiel { get; set; } = "";

    public override string ToString() => Wort;
}

public sealed class Stufe
{
    [JsonPropertyName("number")] public int Nummer { get; set; }
    [JsonPropertyName("title")] public string Titel { get; set; } = "";
}

public sealed class Quizfrage
{
    [JsonPropertyName("question")] public string Frage { get; set; } = "";
    [JsonPropertyName("options")] public List<string> Antworten { get; set; } = new();
    /// <summary>Index (ab 0) der richtigen Antwort.</summary>
    [JsonPropertyName("correct")] public int Richtig { get; set; }
    [JsonPropertyName("explanation")] public string Erklaerung { get; set; } = "";
}
