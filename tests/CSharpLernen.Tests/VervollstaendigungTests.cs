using CSharpLernen.Engine;
using Xunit;

namespace CSharpLernen.Tests;

public class VervollstaendigungTests
{
    static Task<VervollstaendigungsListe?> An(string code, char? zeichen = null)
    {
        int pos = code.IndexOf('|');
        Assert.True(pos >= 0, "Testcode braucht | als Cursor");
        return Vervollstaendigung.Standard.VorschlaegeAsync(code.Remove(pos, 1), pos, zeichen);
    }

    static Vorschlag? Finde(VervollstaendigungsListe? l, string anzeige) => l?.Vorschlaege.FirstOrDefault(v => v.Anzeige == anzeige);

    [Fact]
    public async Task Nach_Console_Punkt_kommt_WriteLine()
    {
        var l = await An("Console.|", '.');
        Assert.NotNull(l);
        var wl = Finde(l, "WriteLine");
        Assert.NotNull(wl);
        Assert.Equal(VorschlagArt.Methode, wl!.Art);
        Assert.Equal("Methode", wl.ArtText);
        Assert.Equal("WriteLine", wl.Einfuegen);
        Assert.NotNull(Finde(l, "ReadLine"));
        Assert.Null(Finde(l, "int"));                       // keine Schlüsselwörter hinter dem Punkt
        Assert.Equal(8, l!.Start);
        var beschreibung = await wl.BeschreibungAsync();
        Assert.Contains("WriteLine", beschreibung);
    }

    [Fact]
    public async Task Eigene_Variable_und_Schluesselwoerter()
    {
        // erster Buchstabe öffnet die Liste (wie in Visual Studio), danach filtert die Liste selbst
        var l = await An("int zahl = 5;\nz|", 'z');
        Assert.NotNull(l);
        var v = Finde(l, "zahl");
        Assert.NotNull(v);
        Assert.Equal(VorschlagArt.Variable, v!.Art);
        Assert.NotNull(Finde(await An("int zahl = 5;\nza|", null), "zahl"));   // Strg+Leertaste mitten im Wort

        var k = await An("i|", 'i');
        Assert.NotNull(k);
        Assert.Equal(VorschlagArt.Schluesselwort, Finde(k, "int")!.Art);
        Assert.Equal(VorschlagArt.Schluesselwort, Finde(k, "if")!.Art);
    }

    [Fact]
    public async Task Eigene_Klasse_Methode_und_Member()
    {
        string code = "Auto a = new Auto();\na.|\n\nclass Auto\n{\n    public string Marke;\n    public int PS { get; set; }\n    public void Hupen() { }\n}\n";
        var l = await An(code, '.');
        Assert.Equal(VorschlagArt.Feld, Finde(l, "Marke")!.Art);
        Assert.Equal(VorschlagArt.Eigenschaft, Finde(l, "PS")!.Art);
        Assert.Equal(VorschlagArt.Methode, Finde(l, "Hupen")!.Art);

        var t = await An("A|\nclass Auto { }\nstatic void Ausgeben() { }\n", 'A');
        Assert.Equal(VorschlagArt.Klasse, Finde(t, "Auto")!.Art);
        Assert.Equal(VorschlagArt.Methode, Finde(t, "Ausgeben")!.Art);
    }

    [Fact]
    public async Task Typen_aus_globalen_usings_und_Generika()
    {
        var l = await An("var liste = new |();", null);
        Assert.NotNull(l);
        var list = l!.Vorschlaege.FirstOrDefault(v => v.FilterText == "List");
        Assert.NotNull(list);
        Assert.Equal("List", list!.Einfuegen);
        Assert.Equal(VorschlagArt.Klasse, list.Art);

        var m = await An("List<int> z = new List<int>();\nz.|", '.');
        Assert.NotNull(Finde(m, "Add"));
        Assert.NotNull(Finde(m, "Count"));
    }

    [Fact]
    public async Task Keine_Vorschlaege_in_Strings_und_Kommentaren()
    {
        Assert.Null(await An("Console.WriteLine(\"Hallo a|\");", 'a'));
        Assert.Null(await An("// ein Kommentar a|", 'a'));
    }

    [Fact]
    public async Task Parallel_und_abbrechbar()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            Vervollstaendigung.Standard.VorschlaegeAsync("Console.", 8, '.', cts.Token));
        var aufgaben = Enumerable.Range(0, 8).Select(_ => An("Math.|", '.')).ToArray();
        foreach (var r in await Task.WhenAll(aufgaben)) Assert.NotNull(Finde(r, "Sqrt"));
    }
}
