using System.Diagnostics;
using System.Text;

namespace CSharpLernen.Engine;

/// <summary>Stoppuhr, die angehalten wird, solange das Programm auf eine Eingabe wartet.</summary>
internal sealed class Rechenzeit
{
    readonly Stopwatch uhr = new();
    public void Start() => uhr.Start();
    public void Pause() => uhr.Stop();
    public void Weiter() => uhr.Start();
    public TimeSpan Vergangen => uhr.Elapsed;
}

/// <summary>Fängt alles ab, was das Programm mit Console.Write/WriteLine ausgibt.</summary>
internal sealed class AusgabeSchreiber : TextWriter
{
    readonly StringBuilder puffer = new();
    readonly object sperre = new();
    readonly Action<string>? beiAusgabe;
    readonly Action? beiLoeschen;
    readonly Action beiLimit;
    readonly int maxZeichen;
    int gesamt;
    bool geschlossen;

    public AusgabeSchreiber(Action<string>? beiAusgabe, Action? beiLoeschen, int maxZeichen, Action beiLimit)
    {
        this.beiAusgabe = beiAusgabe;
        this.beiLoeschen = beiLoeschen;
        this.maxZeichen = maxZeichen;
        this.beiLimit = beiLimit;
    }

    public bool LimitErreicht { get; private set; }
    public override Encoding Encoding => Encoding.UTF8;

    public override void Write(char value) => Write(value.ToString());
    public override void Write(char[] buffer, int index, int count) => Write(new string(buffer, index, count));
    public override void Write(ReadOnlySpan<char> buffer) => Write(new string(buffer));

    public override void Write(string? value)
    {
        if (string.IsNullOrEmpty(value)) return;
        bool limitJetzt = false;
        lock (sperre)
        {
            if (geschlossen || LimitErreicht) return;
            int start = 0;
            while (true)
            {
                int ff = value.IndexOf('\f', start);
                string teil = ff < 0 ? value[start..] : value[start..ff];
                if (teil.Length > 0)
                {
                    if (gesamt + teil.Length > maxZeichen)
                    {
                        teil = teil[..Math.Max(0, maxZeichen - gesamt)];
                        LimitErreicht = true;
                        limitJetzt = true;
                    }
                    gesamt += teil.Length;
                    puffer.Append(teil);
                    if (teil.Length > 0) beiAusgabe?.Invoke(teil);
                    if (LimitErreicht) break;
                }
                if (ff < 0) break;
                puffer.Clear();
                beiLoeschen?.Invoke();
                start = ff + 1;
            }
        }
        if (limitJetzt) beiLimit();
    }

    public void Schliessen()
    {
        lock (sperre) geschlossen = true;
    }

    public string Text
    {
        get { lock (sperre) return puffer.ToString(); }
    }
}

/// <summary>Liefert Console.ReadLine erst die vorab eingegebenen Zeilen, danach (optional) interaktive Eingaben.</summary>
internal sealed class EingabeLeser : TextReader
{
    readonly Queue<string> vorab;
    readonly Func<CancellationToken, string?>? interaktiv;
    readonly CancellationToken token;
    readonly Rechenzeit zeit;
    string? zeile;   // angefangene Zeile bei Console.Read()
    int pos;

    public EingabeLeser(IEnumerable<string> vorab, Func<CancellationToken, string?>? interaktiv, CancellationToken token, Rechenzeit zeit)
    {
        this.vorab = new Queue<string>(vorab);
        this.interaktiv = interaktiv;
        this.token = token;
        this.zeit = zeit;
    }

    public bool Aufgebraucht { get; private set; }

    string? NaechsteZeile()
    {
        token.ThrowIfCancellationRequested();
        if (vorab.Count > 0) return vorab.Dequeue();
        if (interaktiv != null)
        {
            zeit.Pause();
            try
            {
                string? z = interaktiv(token);
                token.ThrowIfCancellationRequested();
                if (z == null) Aufgebraucht = true;
                return z;
            }
            finally { zeit.Weiter(); }
        }
        Aufgebraucht = true;
        return null;
    }

    public override string? ReadLine()
    {
        if (zeile != null)
        {
            string rest = zeile[Math.Min(pos, zeile.Length)..];
            zeile = null;
            return rest;
        }
        return NaechsteZeile();
    }

    public override int Read()
    {
        if (zeile == null)
        {
            string? z = NaechsteZeile();
            if (z == null) return -1;
            zeile = z;
            pos = 0;
        }
        if (pos < zeile.Length) return zeile[pos++];
        zeile = null;
        return '\n';
    }

    /// <summary>Blockiert nie: -1, wenn keine Eingabe sofort verfügbar ist.</summary>
    public override int Peek()
    {
        if (zeile != null) return pos < zeile.Length ? zeile[pos] : '\n';
        if (vorab.Count > 0) { var z = vorab.Peek(); return z.Length > 0 ? z[0] : '\n'; }
        return -1;
    }

    public override string ReadToEnd()
    {
        var sb = new StringBuilder();
        string? z;
        while ((z = ReadLine()) != null) sb.AppendLine(z);
        return sb.ToString();
    }
}
