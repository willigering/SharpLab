using System.Text.RegularExpressions;
using CSharpLernen.Engine;
using Xunit;

namespace CSharpLernen.Tests;

/// <summary>Regressionstests für den Absturz „'' ist kein gültiger Wert für die Eigenschaft 'FontFamily'“ (1.0.4, Quiz)
/// sowie für das Fehlerprotokoll und das Bereinigen alter Fortschrittsdateien.</summary>
public class RobustheitTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(" , ,")]
    [InlineData("#")]
    public void Leere_Schriftnamen_ergeben_Theme_Schrift(string? name)
    {
        Assert.Equal(SchriftNamen.ThemeSchrift, SchriftNamen.Bereinige(name));
    }

    [Fact]
    public void Unbekannte_Schrift_ergibt_Theme_Schrift()
    {
        Func<string, bool> installiert = f => f is "Consolas" or "Cascadia Mono";
        Assert.Equal(SchriftNamen.ThemeSchrift, SchriftNamen.Bereinige("Gibt Es Nicht", istInstalliert: installiert));
        Assert.Equal("Gibt Es Nicht, Consolas", SchriftNamen.Bereinige(" Gibt Es Nicht ,Consolas", istInstalliert: installiert));
        Assert.Equal("Consolas", SchriftNamen.Bereinige("Consolas", istInstalliert: installiert));
        // Eingebettete (pack-)Schriften gelten als vorhanden
        Assert.Equal(SchriftNamen.ThemeSchrift, SchriftNamen.Bereinige(SchriftNamen.ThemeSchrift, istInstalliert: _ => false));
    }

    static string Wurzel()
    {
        var d = new DirectoryInfo(AppContext.BaseDirectory);
        while (d != null && !File.Exists(Path.Combine(d.FullName, "CSharpLernen.sln"))) d = d.Parent;
        Assert.NotNull(d);
        return d!.FullName;
    }

    /// <summary>WPF erlaubt für FontFamily kein null (ValidateValueCallback) – genau das war der Quiz-Absturz.
    /// Kein Code darf FontFamily auf null oder einen leeren Namen setzen.</summary>
    [Fact]
    public void Kein_FontFamily_null_oder_leer_im_App_Code()
    {
        var fehler = new List<string>();
        foreach (var datei in Directory.EnumerateFiles(Path.Combine(Wurzel(), "src", "CSharpLernen"), "*.*", SearchOption.AllDirectories)
                     .Where(p => (p.EndsWith(".cs") || p.EndsWith(".xaml")) && !p.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}") && !p.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}")))
        {
            var zeilen = File.ReadAllLines(datei);
            for (int i = 0; i < zeilen.Length; i++)
            {
                string z = zeilen[i];
                if (z.TrimStart().StartsWith("//")) continue;
                if (Regex.IsMatch(z, @"FontFamily\s*=\s*[^;,}\n]*\bnull\b") || Regex.IsMatch(z, @"FontFamily\s*=\s*""\s*""")
                    || Regex.IsMatch(z, @"new\s+FontFamily\s*\(\s*""\s*""\s*\)") || Regex.IsMatch(z, @"FontFamily\s*=\s*\(FontFamily\)"))
                    fehler.Add($"{Path.GetFileName(datei)}:{i + 1}: {z.Trim()}");
            }
        }
        Assert.True(fehler.Count == 0, string.Join(Environment.NewLine, fehler));
    }

    [Fact]
    public void Fehlerprotokoll_schreibt_Stacktrace_und_kuerzt()
    {
        string ordner = Path.Combine(Path.GetTempPath(), "sharplab-test-" + Guid.NewGuid().ToString("N"));
        string datei = Path.Combine(ordner, "fehler.log");
        try
        {
            Exception ex;
            try { throw new ArgumentException("'' ist kein gültiger Wert für die Eigenschaft 'FontFamily'."); }
            catch (Exception e) { ex = e; }
            Assert.True(FehlerProtokoll.Schreiben(datei, ex, "Test", "1.0.5"));
            string text = File.ReadAllText(datei);
            Assert.Contains("FontFamily", text);
            Assert.Contains("SharpLab 1.0.5", text);
            Assert.Contains(nameof(Fehlerprotokoll_schreibt_Stacktrace_und_kuerzt), text); // Stacktrace

            File.WriteAllText(datei, new string('x', (int)FehlerProtokoll.MaxGroesse + 10));
            Assert.True(FehlerProtokoll.Schreiben(datei, ex, "Test"));
            Assert.True(new FileInfo(datei).Length < FehlerProtokoll.MaxGroesse);
            Assert.Contains("gekürzt", File.ReadAllText(datei));
        }
        finally { try { Directory.Delete(ordner, true); } catch { } }
    }

    [Fact]
    public void Alte_Fortschrittsdatei_mit_leeren_Werten_wird_bereinigt()
    {
        string json = """
        {
          "GeloesteAufgaben": ["l01a1", "", null],
          "Code": null,
          "QuizPunkte": { "l01": 3, "": 1, "l02": -4 },
          "GeleseneLektionen": null,
          "LetzteLektion": "",
          "Schriftgroesse": 0,
          "Zeitlimit": 0,
          "MatrixStufe": 7,
          "MatrixTempo": -1,
          "ErsterStart": "0001-01-01T00:00:00"
        }
        """;
        var f = CSharpLernen.Fortschritt.AusJson(json);
        Assert.Equal(new[] { "l01a1" }, f.GeloesteAufgaben.ToArray());
        Assert.NotNull(f.Code);
        Assert.NotNull(f.GeleseneLektionen);
        Assert.Equal(2, f.QuizPunkte.Count);
        Assert.Equal(0, f.QuizPunkte["l02"]);
        Assert.Null(f.LetzteLektion);
        Assert.Equal(15, f.Schriftgroesse);
        Assert.Equal(10, f.Zeitlimit);
        Assert.Equal(0.7, f.MatrixStufe);
        Assert.Equal(1.0, f.MatrixTempo);
        Assert.True(f.Autovervollstaendigung);
        Assert.NotEqual(default, f.ErsterStart);
    }

    [Fact]
    public void Gueltige_Fortschrittsdatei_bleibt_unveraendert()
    {
        var f = CSharpLernen.Fortschritt.AusJson("""{ "Schriftgroesse": 18, "MatrixStufe": 0.4, "MatrixTempo": 2.0, "Autovervollstaendigung": false, "LetzteLektion": "l05" }""");
        Assert.Equal(18, f.Schriftgroesse);
        Assert.Equal(0.4, f.MatrixStufe);
        Assert.Equal(2.0, f.MatrixTempo);
        Assert.False(f.Autovervollstaendigung);
        Assert.Equal("l05", f.LetzteLektion);
    }
}
