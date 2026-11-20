using System.Text;
using CSharpLernen.Engine;
using Xunit;

namespace CSharpLernen.Tests;

/// <summary>Willi (1.0.5): 8 Aufgaben pro Lektion, nach 5 gelösten darf man weiter; „Nächste Lektion“ erst ab 5, grün erst bei 8.</summary>
public class LektionsstatusTests
{
    static readonly Kurs Kurs = Lade();

    static Kurs Lade()
    {
        using var s = typeof(KursLader).Assembly.GetManifestResourceStream(KursLader.RessourcenName)!;
        using var r = new StreamReader(s, Encoding.UTF8);
        return KursLader.AusJson(r.ReadToEnd());
    }

    static Lektion L(int nr) => Kurs.Lektionen[nr - 1];
    static HashSet<string> Erste(Lektion l, int n) => l.Aufgaben.Take(n).Select(a => a.Id).ToHashSet();

    [Theory]
    [InlineData(0, false, false)]
    [InlineData(4, false, false)]
    [InlineData(5, true, false)]
    [InlineData(7, true, false)]
    [InlineData(8, true, true)]
    public void Fuenf_von_acht_schliesst_ab_acht_macht_komplett(int geloest, bool abgeschlossen, bool komplett)
    {
        var l = L(8);
        var g = l.Aufgaben.Skip(8 - geloest).Select(a => a.Id).ToHashSet(); // die letzten, nicht die alten ersten
        Assert.Equal(8, l.Aufgaben.Count);
        Assert.Equal(abgeschlossen, Lektionsstatus.IstAbgeschlossen(l, g));
        Assert.Equal(komplett, Lektionsstatus.IstKomplett(l, g));
        Assert.Equal(abgeschlossen, Lektionsstatus.NaechsteSichtbar(l, g));
        Assert.Equal(komplett, Lektionsstatus.NaechsteHervorgehoben(l, g));
    }

    [Fact]
    public void Naechste_Lektion_wird_mit_fuenf_freigeschaltet()
    {
        var alle = Kurs.Lektionen;
        Assert.True(Lektionsstatus.IstFreigeschaltet(alle, 0, new HashSet<string>(), false));
        var vier = L(1).Aufgaben.Skip(4).Select(a => a.Id).ToHashSet();
        Assert.Equal(4, vier.Count);
        Assert.False(Lektionsstatus.IstFreigeschaltet(alle, 1, vier, false));
        Assert.True(Lektionsstatus.IstFreigeschaltet(alle, 1, Erste(L(1), 5), false));
        Assert.True(Lektionsstatus.IstFreigeschaltet(alle, 5, new HashSet<string>(), true));
    }

    /// <summary>Alte Fortschrittsdateien: wer alle alten 3–4 Aufgaben gelöst hatte, behält „abgeschlossen“ und die Freischaltung.</summary>
    [Fact]
    public void Alter_Fortschritt_bleibt_gueltig()
    {
        foreach (var l in Kurs.Lektionen)
        {
            int alt = Lektionsstatus.AlteAnzahl[l.Id];
            Assert.InRange(alt, 3, 4);
            Assert.True(Lektionsstatus.IstAbgeschlossen(l, Erste(l, alt)), l.Id);
            Assert.False(Lektionsstatus.IstKomplett(l, Erste(l, alt)), l.Id);
            Assert.False(Lektionsstatus.IstAbgeschlossen(l, Erste(l, alt - 1)), l.Id);
        }
        Assert.True(Lektionsstatus.IstFreigeschaltet(Kurs.Lektionen, 4, Erste(L(4), 4), false));
    }

    [Fact]
    public void Alte_Ids_sind_unveraendert_und_fortlaufend()
    {
        foreach (var l in Kurs.Lektionen)
            for (int i = 0; i < 8; i++)
                Assert.Equal($"{l.Id}a{i + 1}", l.Aufgaben[i].Id);
    }
}
