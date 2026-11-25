using System.Text.Json;
using Xunit;

namespace CSharpLernen.Tests;

[CollectionDefinition("Speicherort", DisableParallelization = true)]
public class SpeicherortSammlung { }

/// <summary>Speicherort des Lernstands: Pfadauflösung, Zeiger-Datei, Fallback, Kopier-/Übernahmelogik, Trennung Einstellungen/Lernstand.
/// Läuft komplett in einem Temp-Ordner (StandardOrdnerUeberschrieben), ohne Oberfläche.</summary>
[Collection("Speicherort")]
public sealed class SpeicherortTests : IDisposable
{
    readonly string wurzel;
    readonly string standard;

    public SpeicherortTests()
    {
        wurzel = Path.Combine(Path.GetTempPath(), "sharplab-test-" + Guid.NewGuid().ToString("N")[..8]);
        standard = Path.Combine(wurzel, "AppData", "CSharpLernen");
        Directory.CreateDirectory(standard);
        Speicherort.StandardOrdnerUeberschrieben = standard;
        Fortschritt.LernstandOrdner = null;
    }

    [Fact]
    public void MatrixregenWirdProDesignGetrenntGespeichert()
    {
        var fortschritt = Fortschritt.AusJson("""{"Design":"gold","MatrixRegen":true}""");
        Assert.False(fortschritt.MatrixRegenAktiv());

        fortschritt.Design = "silber";
        Assert.False(fortschritt.MatrixRegenAktiv());
        fortschritt.MatrixRegenSetzen(true);
        fortschritt.Speichern();

        var geladen = Fortschritt.Laden();
        Assert.Equal("silber", geladen.Design);
        Assert.True(geladen.MatrixRegenAktiv());
        geladen.Design = "gold";
        Assert.False(geladen.MatrixRegenAktiv());
        geladen.Design = "matrix";
        Assert.True(geladen.MatrixRegenAktiv());
    }

    [Fact]
    public void EigeneProjekteUndCodeBleibenBeimNeuladenGetrennt()
    {
        var fortschritt = Fortschritt.AusJson("""{"Code":{"spielwiese":"alter Code"}}""");
        Assert.Empty(fortschritt.EigeneProjekte);

        var erstes = Guid.NewGuid();
        var zweites = Guid.NewGuid();
        fortschritt.EigeneProjekte.Add(erstes, "Test");
        fortschritt.EigeneProjekte.Add(zweites, "Test");
        fortschritt.Code.Add("projekt:" + erstes.ToString("N"), "Console.WriteLine(1);");
        fortschritt.Speichern();

        var geladen = Fortschritt.Laden();
        Assert.Equal(2, geladen.EigeneProjekte.Count);
        Assert.Equal("Console.WriteLine(1);", geladen.Code["projekt:" + erstes.ToString("N")]);
        Assert.DoesNotContain("projekt:" + zweites.ToString("N"), geladen.Code.Keys);
        Assert.Equal("alter Code", geladen.Code["spielwiese"]);
    }

    [Fact]
    public void ProjektversionenBleibenAuchNachAenderungenAmArbeitsstandErhalten()
    {
        var fortschritt = new Fortschritt();
        var id = Guid.NewGuid();
        var schluessel = Fortschritt.ProjektSchluessel(id);
        fortschritt.EigeneProjekte.Add(id, "Projekt");
        fortschritt.Code[schluessel] = "alter Stand";

        Assert.Equal(2, fortschritt.ProjektVersionAnlegen(id, "erster Import"));
        Assert.Equal("erster Import", fortschritt.Code[schluessel]);
        fortschritt.Code[schluessel] = "Arbeitsstand";
        Assert.Equal(3, fortschritt.ProjektVersionAnlegen(id, "zweiter Import"));
        fortschritt.Code[schluessel] = "neuer Arbeitsstand";
        fortschritt.Speichern();

        var geladen = Fortschritt.Laden();
        Assert.Equal(new[] { "alter Stand", "erster Import", "zweiter Import" }, geladen.ProjektVersionen[id]);
        Assert.Equal("neuer Arbeitsstand", geladen.Code[schluessel]);
    }

    public void Dispose()
    {
        Speicherort.StandardOrdnerUeberschrieben = null;
        Fortschritt.LernstandOrdner = null;
        try { Directory.Delete(wurzel, true); } catch { }
    }

    string Ordner(string name) { var p = Path.Combine(wurzel, name); Directory.CreateDirectory(p); return p; }

    /// <summary>Ein Pfad, der nie ein Ordner werden kann (liegt „unter“ einer Datei).</summary>
    string Unerreichbar()
    {
        string datei = Path.Combine(wurzel, "ich-bin-eine-datei");
        File.WriteAllText(datei, "x");
        return Path.Combine(datei, "OneDrive");
    }

    // ---------------------------------------------------------------- Pfadauflösung

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Leere_Basis_ist_Standard(string? basis)
    {
        Assert.Equal(standard, Speicherort.LernstandOrdner(basis));
        Assert.True(Speicherort.IstStandard(Speicherort.LernstandOrdner(basis)));
    }

    [Fact]
    public void Eigener_Ordner_bekommt_Unterordner_SharpLab()
    {
        string od = Ordner("OneDrive");
        Assert.Equal(Path.Combine(od, "SharpLab"), Speicherort.LernstandOrdner(od));
    }

    [Fact]
    public void Ordner_der_schon_SharpLab_heisst_bekommt_keinen_zweiten()
    {
        string sl = Ordner(Path.Combine("OneDrive", "SharpLab"));
        Assert.Equal(sl, Speicherort.LernstandOrdner(sl));
        Assert.Equal(sl, Speicherort.LernstandOrdner(sl + Path.DirectorySeparatorChar));
        Assert.Equal(Path.Combine(wurzel, "OneDrive", "sharplab"), Speicherort.LernstandOrdner(Path.Combine(wurzel, "OneDrive", "sharplab")));
    }

    [Fact]
    public void Standardordner_als_Basis_bleibt_Standard()
    {
        Assert.Equal(standard, Speicherort.LernstandOrdner(standard));
    }

    [Fact]
    public void Zeigerdatei_liegt_immer_im_lokalen_Standardordner()
    {
        Assert.Equal(Path.Combine(standard, "speicherort.json"), Speicherort.ZeigerDatei);
        Speicherort.SchreibeBasis(Ordner("OneDrive"));
        Assert.True(File.Exists(Path.Combine(standard, "speicherort.json")));
        Assert.False(File.Exists(Path.Combine(wurzel, "OneDrive", "SharpLab", "speicherort.json")));
    }

    [Fact]
    public void Zeiger_hin_und_zurueck()
    {
        string od = Ordner("OneDrive");
        Assert.Null(Speicherort.LeseBasis());
        Speicherort.SchreibeBasis(od);
        Assert.Equal(od, Speicherort.LeseBasis());
        Speicherort.SchreibeBasis(null);
        Assert.Null(Speicherort.LeseBasis());
        Assert.False(File.Exists(Speicherort.ZeigerDatei));
    }

    [Theory]
    [InlineData("kaputt{")]
    [InlineData("")]
    [InlineData("{ \"Ordner\": \"   \" }")]
    [InlineData("null")]
    public void Kaputter_Zeiger_bedeutet_Standard(string inhalt)
    {
        File.WriteAllText(Speicherort.ZeigerDatei, inhalt);
        Assert.Null(Speicherort.LeseBasis());
        Fortschritt.Laden();
        Assert.Equal(standard, Fortschritt.LernstandOrdner);
    }

    // ---------------------------------------------------------------- Schreibprüfung, OneDrive, atomar

    [Fact]
    public void Beschreibbar_legt_Ordner_an_und_hinterlaesst_nichts()
    {
        string ziel = Path.Combine(wurzel, "neu", "SharpLab");
        Assert.True(Speicherort.IstBeschreibbar(ziel, out var fehler), fehler);
        Assert.True(Directory.Exists(ziel));
        Assert.Empty(Directory.GetFiles(ziel));
    }

    [Fact]
    public void Nicht_erreichbarer_Ordner_ist_nicht_beschreibbar()
    {
        Assert.False(Speicherort.IstBeschreibbar(Unerreichbar(), out var fehler));
        Assert.False(string.IsNullOrWhiteSpace(fehler));
        Assert.False(Speicherort.IstBeschreibbar("", out _));
    }

    [Fact]
    public void OneDrive_wird_aus_Umgebungsvariablen_erkannt()
    {
        string privat = Ordner("OneDrive - Privat");
        string firma = Ordner("OneDrive - Firma");
        var env = new Dictionary<string, string?> { ["OneDrive"] = Path.Combine(wurzel, "gibts-nicht"), ["OneDriveConsumer"] = privat, ["OneDriveCommercial"] = firma };
        Assert.Equal(privat, Speicherort.OneDriveOrdner(n => env.GetValueOrDefault(n)));
        env["OneDrive"] = firma;
        Assert.Equal(firma, Speicherort.OneDriveOrdner(n => env.GetValueOrDefault(n)));
        Assert.Null(Speicherort.OneDriveOrdner(_ => null));
        Assert.Null(Speicherort.OneDriveOrdner(_ => "   "));
    }

    [Fact]
    public void Atomares_Schreiben_ersetzt_und_hinterlaesst_keine_Tempdateien()
    {
        string datei = Path.Combine(wurzel, "x", "fortschritt.json");
        Speicherort.AtomarSchreiben(datei, "eins");
        Speicherort.AtomarSchreiben(datei, "zwei");
        Assert.Equal("zwei", File.ReadAllText(datei));
        Assert.Single(Directory.GetFiles(Path.GetDirectoryName(datei)!));
    }

    // ---------------------------------------------------------------- Wechseln (Kopierlogik)

    [Fact]
    public void Wechsel_in_leeren_Ordner_kopiert_aktuellen_Stand()
    {
        string od = Ordner("OneDrive");
        string ziel = Speicherort.Wechseln(od, "{\"GeloesteAufgaben\":[\"a1\"]}", Speicherort.Wechselart.MeinenKopieren);
        Assert.Equal(Path.Combine(od, "SharpLab"), ziel);
        Assert.Contains("a1", File.ReadAllText(Path.Combine(ziel, "fortschritt.json")));
        Assert.Equal(od, Speicherort.LeseBasis());
    }

    [Fact]
    public void Kopieren_ueber_vorhandenen_Stand_legt_Sicherung_an()
    {
        string od = Ordner("OneDrive");
        string ziel = Ordner(Path.Combine("OneDrive", "SharpLab"));
        File.WriteAllText(Path.Combine(ziel, "fortschritt.json"), "{\"GeloesteAufgaben\":[\"alt\"]}");
        Speicherort.Wechseln(od, "{\"GeloesteAufgaben\":[\"neu\"]}", Speicherort.Wechselart.MeinenKopieren);
        Assert.Contains("neu", File.ReadAllText(Path.Combine(ziel, "fortschritt.json")));
        var sicherung = Assert.Single(Directory.GetFiles(ziel, "fortschritt.vorher-*.json"));
        Assert.Contains("alt", File.ReadAllText(sicherung));
    }

    [Fact]
    public void Uebernehmen_laesst_vorhandenen_Stand_unveraendert()
    {
        string od = Ordner("OneDrive");
        string ziel = Ordner(Path.Combine("OneDrive", "SharpLab"));
        string datei = Path.Combine(ziel, "fortschritt.json");
        File.WriteAllText(datei, "{\"GeloesteAufgaben\":[\"dort\"]}");
        Speicherort.Wechseln(od, "{\"GeloesteAufgaben\":[\"hier\"]}", Speicherort.Wechselart.VorhandenenUebernehmen);
        Assert.Equal("{\"GeloesteAufgaben\":[\"dort\"]}", File.ReadAllText(datei));
        Assert.Empty(Directory.GetFiles(ziel, "fortschritt.vorher-*.json"));
        Assert.Equal(od, Speicherort.LeseBasis());
    }

    [Fact]
    public void Wechsel_in_unerreichbaren_Ordner_wirft_und_laesst_Zeiger_stehen()
    {
        string od = Ordner("OneDrive");
        Speicherort.SchreibeBasis(od);
        Assert.Throws<IOException>(() => Speicherort.Wechseln(Unerreichbar(), "{}", Speicherort.Wechselart.MeinenKopieren));
        Assert.Equal(od, Speicherort.LeseBasis());
    }

    [Fact]
    public void Wechsel_zurueck_auf_Standard_entfernt_Zeiger()
    {
        Speicherort.SchreibeBasis(Ordner("OneDrive"));
        string ziel = Speicherort.Wechseln(null, "{}", Speicherort.Wechselart.MeinenKopieren);
        Assert.Equal(standard, ziel);
        Assert.False(File.Exists(Speicherort.ZeigerDatei));
    }

    // ---------------------------------------------------------------- Laden / Speichern / Fallback / Migration

    [Fact]
    public void Nicht_erreichbarer_Ordner_beim_Start_faellt_auf_Standard_zurueck_und_protokolliert()
    {
        File.WriteAllText(Speicherort.ZeigerDatei, JsonSerializer.Serialize(new { Ordner = Unerreichbar() }));
        var f = Fortschritt.Laden();
        Assert.NotNull(f);
        Assert.Equal(standard, Fortschritt.LernstandOrdner);
        Assert.False(string.IsNullOrWhiteSpace(Fortschritt.StartHinweis));
        Assert.Null(Speicherort.LeseBasis());
        string log = File.ReadAllText(Path.Combine(standard, "fehler.log"));
        Assert.Contains("nicht erreichbar", log);
    }

    [Fact]
    public void Einstellungen_bleiben_lokal_Lernstand_wandert()
    {
        string od = Ordner("OneDrive");
        Speicherort.SchreibeBasis(od);
        var f = Fortschritt.Laden();
        Assert.Null(Fortschritt.StartHinweis);
        f.GeloesteAufgaben.Add("l01-a1");
        f.Schriftgroesse = 21;
        f.MatrixTempo = 1.7;
        f.Autovervollstaendigung = false;
        f.Speichern();

        string lern = File.ReadAllText(Path.Combine(od, "SharpLab", "fortschritt.json"));
        Assert.Contains("l01-a1", lern);
        foreach (var feld in Fortschritt.EinstellungsFelder) Assert.DoesNotContain("\"" + feld + "\"", lern);
        string einst = File.ReadAllText(Path.Combine(standard, "einstellungen.json"));
        Assert.Contains("\"Schriftgroesse\": 21", einst);
        Assert.DoesNotContain("l01-a1", einst);

        var g = Fortschritt.Laden();
        Assert.Contains("l01-a1", g.GeloesteAufgaben);
        Assert.Equal(21, g.Schriftgroesse);
        Assert.Equal(1.7, g.MatrixTempo);
        Assert.False(g.Autovervollstaendigung);
    }

    [Fact]
    public void Alte_Gesamtdatei_wird_uebernommen()
    {
        // So sah 1.0.5 und älter aus: alles in %AppData%\CSharpLernen\fortschritt.json
        File.WriteAllText(Path.Combine(standard, "fortschritt.json"),
            """{ "GeloesteAufgaben": ["l02-a3"], "Schriftgroesse": 19, "MatrixStufe": 0.3, "Autovervollstaendigung": false, "LetzteLektion": "l02" }""");
        var f = Fortschritt.Laden();
        Assert.Contains("l02-a3", f.GeloesteAufgaben);
        Assert.Equal(19, f.Schriftgroesse);
        Assert.Equal(0.3, f.MatrixStufe);
        Assert.False(f.Autovervollstaendigung);
        Assert.Equal("l02", f.LetzteLektion);

        // Auch nach einem Wechsel in einen leeren eigenen Ordner bleiben die Einstellungen erhalten.
        string od = Ordner("OneDrive");
        Speicherort.Wechseln(od, f.LernstandJson(), Speicherort.Wechselart.MeinenKopieren);
        var g = Fortschritt.Laden();
        Assert.Equal(Path.Combine(od, "SharpLab"), Fortschritt.LernstandOrdner);
        Assert.Contains("l02-a3", g.GeloesteAufgaben);
        Assert.Equal(19, g.Schriftgroesse);
    }

    [Fact]
    public void Uebernehmen_behaelt_lokale_Einstellungen()
    {
        var hier = new Fortschritt { Schriftgroesse = 22, MatrixRegen = false };
        hier.GeloesteAufgaben.Add("hier");
        var dort = Fortschritt.AusJson("""{ "GeloesteAufgaben": ["dort"], "Schriftgroesse": 12, "MatrixRegen": true, "QuizPunkte": { "l01": 3 } }""");
        hier.UebernimmLernstand(dort);
        Assert.Equal(new[] { "dort" }, hier.GeloesteAufgaben);
        Assert.Equal(3, hier.QuizPunkte["l01"]);
        Assert.Equal(22, hier.Schriftgroesse);
        Assert.False(hier.MatrixRegen);
    }

    [Fact]
    public void Kaputte_Lernstanddatei_im_Zielordner_wird_gesichert_und_ignoriert()
    {
        string od = Ordner("OneDrive");
        string ziel = Ordner(Path.Combine("OneDrive", "SharpLab"));
        File.WriteAllText(Path.Combine(ziel, "fortschritt.json"), "{ kaputt");
        Speicherort.SchreibeBasis(od);
        var f = Fortschritt.Laden();
        Assert.Empty(f.GeloesteAufgaben);
        Assert.True(File.Exists(Path.Combine(ziel, "fortschritt.json.kaputt")));
    }
}
