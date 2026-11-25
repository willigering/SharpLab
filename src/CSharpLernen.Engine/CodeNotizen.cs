using System.Text.Json;

namespace CSharpLernen.Engine;

public sealed record CodeNotiz(Guid Id, string Titel, string Kommentar, string Code, DateTimeOffset Geaendert)
{
    public bool PasstZu(string suche)
    {
        return suche.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).All(wort =>
            Titel.Contains(wort, StringComparison.OrdinalIgnoreCase) ||
            Kommentar.Contains(wort, StringComparison.OrdinalIgnoreCase) ||
            Code.Contains(wort, StringComparison.OrdinalIgnoreCase));
    }
}

public sealed class CodeNotizen(string ordner)
{
    string Datei(Guid id) => Path.Combine(ordner, id.ToString("N") + ".json");

    public List<CodeNotiz> Laden(Action<string, Exception> lesefehler)
    {
        Directory.CreateDirectory(ordner);
        var notizen = new List<CodeNotiz>();
        foreach (string datei in Directory.EnumerateFiles(ordner, "*.json"))
        {
            if (!Guid.TryParseExact(Path.GetFileNameWithoutExtension(datei), "N", out var id)) continue;
            try
            {
                var notiz = JsonSerializer.Deserialize<CodeNotiz>(File.ReadAllText(datei));
                if (notiz == null || notiz.Id != id || string.IsNullOrWhiteSpace(notiz.Titel) ||
                    notiz.Kommentar == null || notiz.Code == null)
                    throw new JsonException("Code-Notiz ist unvollständig.");
                notizen.Add(notiz);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
            {
                lesefehler(datei, ex);
            }
        }
        return notizen;
    }

    public void Speichern(CodeNotiz notiz)
    {
        Directory.CreateDirectory(ordner);
        string temporaer = Path.Combine(ordner, Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            File.WriteAllText(temporaer, JsonSerializer.Serialize(notiz));
            File.Move(temporaer, Datei(notiz.Id), overwrite: true);
        }
        finally
        {
            File.Delete(temporaer);
        }
    }

    public void Loeschen(Guid id) => File.Delete(Datei(id));
}
