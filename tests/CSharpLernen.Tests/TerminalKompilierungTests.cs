using System.Reflection;
using System.Runtime.Loader;
using CSharpLernen.Engine;
using Xunit;

namespace CSharpLernen.Tests;

public sealed class TerminalKompilierungTests
{
    [Fact]
    public void TerminalProgrammLaeuftOhneInternenWaechter()
    {
        var ordner = Directory.CreateTempSubdirectory("SharpLab-Test-").FullName;
        var kontext = new AssemblyLoadContext("Terminal-Test", isCollectible: true);
        try
        {
            string pfad = Path.Combine(ordner, "Programm.dll");
            var kompiliert = CodeAusfuehrer.KompilierenFuerTerminal("var n = 0; while (n < 4) n++; return n;", pfad);
            Assert.True(kompiliert.Ok);
            using var stream = File.OpenRead(pfad);
            var programm = kontext.LoadFromStream(stream);
            var ergebnis = programm.EntryPoint!.Invoke(null, BindingFlags.Default, null, new object[] { Array.Empty<string>() }, null);
            Assert.Equal(4, ergebnis);
            Assert.Null(programm.GetType("__Waechter"));
        }
        finally
        {
            kontext.Unload();
            Directory.Delete(ordner, recursive: true);
        }
    }
}
