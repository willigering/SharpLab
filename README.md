# SharpLab – Der interaktive C#-Trainer

Entwickelt von Willi Gering · Gering Digital

Eine Windows-App, mit der du C# von null an lernst: 27 Lektionen in 5 Stufen, 87 automatisch geprüfte Übungen, Quizfragen, ein Begriffstrainer und ein eingebauter C#-Compiler, mit dem du deinen Code direkt in der App ausführst.

## Download

Die fertige App findest du unter [Releases](../../releases/latest).

1. ZIP herunterladen und in einen Ordner entpacken.
2. `SharpLab.exe` starten.
3. Falls Windows „Der Computer wurde durch Windows geschützt“ meldet: „Weitere Informationen“ und dann „Trotzdem ausführen“ klicken (die App ist nicht signiert).
4. Benötigt die [.NET Desktop Runtime](https://dotnet.microsoft.com/de-de/download) 8 oder neuer.

## Funktionen

- Lernpfad: Grundlagen, Kontrollstrukturen, Methoden, Klassen & Objekte, Projekte
- Theorie, Übung und Quiz pro Lektion
- Code-Editor mit Syntaxhervorhebung, Eingaben über `Console.ReadLine()` und Stopp-Knopf für Endlosschleifen
- Freie Spielwiese zum Ausprobieren
- Autovervollständigung im Editor wie in Visual Studio (Vorschläge nach dem Punkt, beim Tippen und mit Strg+Leertaste; in den Einstellungen abschaltbar)
- Aufgaben nennen nur das Ziel, nicht den Lösungsweg; Tipps und Hinweise helfen bei Bedarf
- Knöpfe „Nächste Aufgabe“ und „Nächste Lektion“ (grün, sobald alle Aufgaben gelöst sind; Überspringen ist möglich)
- Fortschritt und Erfolge
- Matrixcode im Hintergrund wie im Film (Zeichen bauen sich auf und klingen ab), mit Reglern für Helligkeit und Geschwindigkeit

Dein Fortschritt und dein Code werden unter `%AppData%\CSharpLernen` gespeichert und bleiben bei Updates erhalten.

## Selbst bauen

`CSharpLernen.sln` in Visual Studio öffnen und starten (Projekt- und Namespace-Namen heißen aus Kompatibilitätsgründen weiter `CSharpLernen`; die erzeugte Datei ist `SharpLab.exe`). Die Lektionen liegen als JSON in der App und werden mit dem Generator unter `werkzeuge/lektionen-generator` erzeugt (`python3 build.py`).

Schriftart: JetBrains Mono (SIL Open Font License).
