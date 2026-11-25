using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CSharpLernen;

static class ExternesTerminal
{
    public const string Argument = "--sharplab-terminal";

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool AllocConsole();

    public static int Starten(string datei)
    {
        if (!AllocConsole() && Console.OpenStandardOutput() == Stream.Null) return 1;
        Console.Title = "SharpLab – Programmausgabe";
        var ausgabe = new StreamWriter(Console.OpenStandardOutput(), Console.OutputEncoding) { AutoFlush = true };
        Console.SetOut(ausgabe);
        Console.SetError(new StreamWriter(Console.OpenStandardError(), Console.OutputEncoding) { AutoFlush = true });
        Console.SetIn(new StreamReader(Console.OpenStandardInput(), Console.InputEncoding));

        int exitCode = 0;
        try
        {
            var einstieg = Assembly.LoadFile(Path.GetFullPath(datei)).EntryPoint
                ?? throw new InvalidOperationException("Das Programm hat keinen Einstiegspunkt.");
            object[] argumente = einstieg.GetParameters().Length == 0 ? null : new object[] { Array.Empty<string>() };
            if (einstieg.Invoke(null, argumente) is int code) exitCode = code;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex is TargetInvocationException { InnerException: { } ursache } ? ursache : ex);
            exitCode = 1;
        }
        ausgabe.WriteLine($"\nProgramm beendet (Exitcode {exitCode}). Taste zum Schließen drücken …");
        try { Console.ReadKey(intercept: true); } catch (InvalidOperationException) { }
        return exitCode;
    }
}
