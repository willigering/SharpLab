using System.Reflection;
using System.Text.Json;

namespace CSharpLernen.Engine;

public static class KursLader
{
    public const string RessourcenName = "CSharpLernen.Lektionen.json";

    static readonly JsonSerializerOptions Optionen = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    /// <summary>Lädt den eingebetteten Kurs. Wenn <paramref name="eigeneDatei"/> existiert, wird stattdessen diese Datei genommen.</summary>
    public static Kurs Laden(string? eigeneDatei = null)
    {
        if (!string.IsNullOrEmpty(eigeneDatei) && File.Exists(eigeneDatei))
            return AusJson(File.ReadAllText(eigeneDatei));

        using var stream = typeof(KursLader).Assembly.GetManifestResourceStream(RessourcenName)
            ?? throw new InvalidOperationException("Eingebettete Lektionen nicht gefunden.");
        using var reader = new StreamReader(stream);
        return AusJson(reader.ReadToEnd());
    }

    public static Kurs AusJson(string json) =>
        JsonSerializer.Deserialize<Kurs>(json, Optionen) ?? new Kurs();
}
