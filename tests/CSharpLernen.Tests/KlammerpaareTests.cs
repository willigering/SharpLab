using CSharpLernen.Engine;
using Xunit;

namespace CSharpLernen.Tests;

public sealed class KlammerpaareTests
{
    [Fact]
    public void FaerbtVerschachteltePaareUndIgnoriertLiteraleUndKommentare()
    {
        const string code = """if (x[0] == '(') { Console.WriteLine(")"); /* [ } */ }""";
        var farben = Klammerpaare.Ebenen(code);

        Assert.Equal(new[] { 0, 1, 1, 0, 0, 1, 1, 0 }, farben.OrderBy(p => p.Key).Select(p => p.Value));
        Assert.DoesNotContain(code.IndexOf("'('", StringComparison.Ordinal) + 1, farben.Keys);
        Assert.DoesNotContain(code.IndexOf("\")\"", StringComparison.Ordinal) + 1, farben.Keys);
        Assert.DoesNotContain(code.LastIndexOf('['), farben.Keys);
    }
}
