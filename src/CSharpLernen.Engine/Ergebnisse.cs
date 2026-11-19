namespace CSharpLernen.Engine;

public enum Ausgang
{
    Erfolg,
    Kompilierfehler,
    Laufzeitfehler,
    Zeitueberschreitung,
    Abgebrochen,
    ZuVielAusgabe,
}

/// <summary>Eine Fehlermeldung des Compilers, mit Zeilennummer und Anfänger-Tipp.</summary>
public sealed record CompilerMeldung(int Zeile, int Spalte, string Code, string Text, string? Tipp, bool IstWarnung = false)
{
    public string Anzeige => $"Zeile {Zeile}, Spalte {Spalte}: {Code} – {Text}";
}

public sealed class Laufergebnis
{
    public Ausgang Ausgang { get; init; }
    public IReadOnlyList<CompilerMeldung> Meldungen { get; init; } = Array.Empty<CompilerMeldung>();
    public string Ausgabe { get; init; } = "";
    /// <summary>Bei Laufzeitfehlern: der Fehler in Anfängersprache.</summary>
    public string? FehlerText { get; init; }
    public string? FehlerTyp { get; init; }
    public int? FehlerZeile { get; init; }
    public string? Tipp { get; init; }
    public TimeSpan Dauer { get; init; }
    /// <summary>Das Programm wollte mehr Eingaben lesen, als vorhanden waren.</summary>
    public bool EingabeAufgebraucht { get; init; }

    public bool Erfolgreich => Ausgang == Ausgang.Erfolg;
}

public sealed class Laufoptionen
{
    /// <summary>Vorab eingegebene Zeilen für Console.ReadLine.</summary>
    public IReadOnlyList<string> Eingabe { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Wird aufgerufen (im Hintergrund-Thread), wenn das Programm eine Eingabe braucht und keine vorab
    /// eingegebene Zeile mehr übrig ist. Soll blockieren, bis der Nutzer etwas eingibt; null = keine Eingabe.
    /// Wenn nicht gesetzt, liefert Console.ReadLine dann null (wie Dateiende).
    /// </summary>
    public Func<CancellationToken, string?>? InteraktiveEingabe { get; init; }

    /// <summary>Neue Ausgabe (Text-Stück). Wird im Hintergrund-Thread aufgerufen.</summary>
    public Action<string>? BeiAusgabe { get; init; }

    /// <summary>Das Übersetzen hat geklappt, das Programm startet jetzt.</summary>
    public Action? BeiStart { get; init; }

    /// <summary>Console.Clear() wurde aufgerufen.</summary>
    public Action? BeiLoeschen { get; init; }

    /// <summary>Rechenzeit, nach der abgebrochen wird. Wartezeit auf Eingaben zählt nicht mit.</summary>
    public TimeSpan Zeitlimit { get; init; } = TimeSpan.FromSeconds(10);

    public int MaxAusgabeZeichen { get; init; } = 200_000;

    public CancellationToken Abbruch { get; init; }
}
