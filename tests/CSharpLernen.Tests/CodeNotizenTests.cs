using CSharpLernen.Engine;
using Xunit;

namespace CSharpLernen.Tests;

public sealed class CodeNotizenTests : IDisposable
{
    readonly string ordner = Path.Combine(Path.GetTempPath(), "SharpLab-Notizen-" + Guid.NewGuid().ToString("N"));

    [Fact]
    public void GespeicherteNotizenSindNachNeustartInAllenFeldernDurchsuchbar()
    {
        var ablage = new CodeNotizen(ordner);
        var notiz = new CodeNotiz(Guid.NewGuid(), "Arrays", "Index bei Null", "// Reihenfolge\r\nConsole.WriteLine(liste[0]);", DateTimeOffset.Now);
        var andere = notiz with { Id = Guid.NewGuid(), Kommentar = "Andere Variante" };
        ablage.Speichern(notiz);
        ablage.Speichern(andere);

        var geladen = new CodeNotizen(ordner).Laden((_, ex) => throw ex);
        Assert.Equal(2, geladen.Count);
        Assert.Equal(notiz, Assert.Single(geladen, n => n.PasstZu("ARRAY Null liste[0] Reihenfolge")));
        Assert.DoesNotContain(geladen, n => n.PasstZu("fehlend"));

        var bearbeitet = notiz with { Titel = "Felder", Kommentar = "Index bei 0" };
        ablage.Speichern(bearbeitet);
        ablage.Loeschen(andere.Id);
        Assert.Equal(bearbeitet, Assert.Single(ablage.Laden((_, ex) => throw ex)));
    }

    public void Dispose()
    {
        if (Directory.Exists(ordner)) Directory.Delete(ordner, recursive: true);
    }
}
