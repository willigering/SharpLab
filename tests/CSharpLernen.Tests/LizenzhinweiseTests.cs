using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace CSharpLernen.Tests;

/// <summary>Stellt sicher, dass THIRD-PARTY-NOTICES.txt alle mitgelieferten Komponenten samt Lizenztexten nennt.</summary>
public class LizenzhinweiseTests
{
    static string Wurzel()
    {
        var d = new DirectoryInfo(System.AppContext.BaseDirectory);
        while (d != null && !File.Exists(Path.Combine(d.FullName, "CSharpLernen.sln"))) d = d.Parent;
        Assert.NotNull(d);
        return d!.FullName;
    }

    static string Hinweise() => File.ReadAllText(Path.Combine(Wurzel(), "THIRD-PARTY-NOTICES.txt"));

    [Fact]
    public void Alle_direkten_Pakete_sind_mit_Version_genannt()
    {
        string text = Hinweise();
        foreach (var csproj in new[] { "src/CSharpLernen/CSharpLernen.csproj", "src/CSharpLernen.Engine/CSharpLernen.Engine.csproj" })
        {
            var refs = XDocument.Load(Path.Combine(Wurzel(), csproj)).Descendants("PackageReference");
            foreach (var r in refs)
            {
                string name = (string)r.Attribute("Include")!, version = (string)r.Attribute("Version")!;
                string kurz = name.Replace("Microsoft.CodeAnalysis.", ".");
                Assert.True(text.Contains(name) || text.Contains(kurz), $"{name} fehlt");
                Assert.Contains(version, text);
            }
        }
    }

    [Theory]
    [InlineData("Humanizer.Core")]
    [InlineData("Microsoft.CodeAnalysis.AnalyzerUtilities")]
    [InlineData("Microsoft.CodeAnalysis.Elfie")]
    [InlineData("Microsoft.DiaSymReader")]
    [InlineData("System.Composition")]
    [InlineData("System.IO.Pipelines")]
    [InlineData("System.Diagnostics.EventLog")]
    [InlineData("JetBrains Mono")]
    public void Transitive_Komponenten_und_Schrift_sind_genannt(string name) => Assert.Contains(name, Hinweise());

    [Fact]
    public void Volltexte_der_Lizenzen_sind_enthalten()
    {
        string text = Hinweise();
        Assert.Contains("Permission is hereby granted, free of charge", text);           // MIT
        Assert.Contains("TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION", text); // Apache-2.0
        Assert.Contains("SIL OPEN FONT LICENSE Version 1.1", text);                       // OFL
        Assert.True(text.Split("Permission is hereby granted").Length > 5);
    }

    [Fact]
    public void Eigene_MIT_Lizenz_liegt_bei()
    {
        string lic = File.ReadAllText(Path.Combine(Wurzel(), "LICENSE"));
        Assert.StartsWith("MIT License", lic);
        Assert.Contains("Copyright (c) 2026 Willi Gering, Gering Digital", lic);
        Assert.Contains("Permission is hereby granted, free of charge", lic);
        string csproj = File.ReadAllText(Path.Combine(Wurzel(), "src/CSharpLernen/CSharpLernen.csproj"));
        Assert.Contains("<PackageLicenseExpression>MIT</PackageLicenseExpression>", csproj);
        Assert.Contains("CSharpLernen.LICENSE", csproj);
        Assert.Contains("MIT-Lizenz", File.ReadAllText(Path.Combine(Wurzel(), "LIESMICH.txt")));
    }
}
