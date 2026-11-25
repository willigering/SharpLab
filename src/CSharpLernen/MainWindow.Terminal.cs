using System.Diagnostics;
using System.IO;
using System.Windows;
using CSharpLernen.Engine;
using Microsoft.Win32;

namespace CSharpLernen;

public partial class MainWindow
{
    void TerminalAuswahl_Click(object sender, RoutedEventArgs e)
    {
        fortschritt.ExternesTerminal = TerminalAuswahl.IsChecked == true;
        fortschritt.Speichern();
    }

    async Task ImTerminalAusfuehren(string code)
    {
        int nr = LaufStart();
        TabAusgabe.IsChecked = true;
        Status("Kompiliere für das externe Terminal …", "TextMittel");
        var token = laufCts.Token;
        string ordner = null;
        try
        {
            ordner = Directory.CreateTempSubdirectory("SharpLab-Terminal-").FullName;
            string datei = Path.Combine(ordner, "Programm.dll");
            var kompiliert = await Task.Run(() => CodeAusfuehrer.KompilierenFuerTerminal(code, datei), token);
            token.ThrowIfCancellationRequested();
            if (nr != laufNr) return;
            ZeigeMeldungen(kompiliert.Meldungen);
            if (!kompiliert.Ok)
            {
                TabFehler.IsChecked = true;
                Status("Kompilierung fehlgeschlagen.", "Rot");
                return;
            }

            var start = new ProcessStartInfo(Environment.ProcessPath)
            {
                UseShellExecute = false,
                ArgumentList = { ExternesTerminal.Argument, datei },
            };
            using var prozess = Process.Start(start) ?? throw new InvalidOperationException("Das Terminal konnte nicht gestartet werden.");
            Status("Externes Terminal geöffnet. Ausgabe und Eingabe erfolgen dort.", "Akzent");
            using var abbruch = token.Register(() =>
            {
                try { prozess.Kill(entireProcessTree: true); }
                catch (InvalidOperationException) { }
                catch (System.ComponentModel.Win32Exception ex) { Fortschritt.Protokoll(ex, "Externes Terminal stoppen"); }
            });
            await prozess.WaitForExitAsync();
            token.ThrowIfCancellationRequested();
            if (nr != laufNr) return;

            fortschritt.Ausfuehrungen++;
            if (editorSchluessel == "spielwiese") fortschritt.SpielwieseLaeufe++;
            if (prozess.ExitCode == 0) fortschritt.ErfolgreicheLaeufe++;
            fortschritt.Speichern();
            PruefeErfolge();
            Status($"Externes Terminal geschlossen · Exitcode {prozess.ExitCode}", prozess.ExitCode == 0 ? "Gruen" : "Gelb");
        }
        catch (OperationCanceledException)
        {
            if (nr == laufNr) Status("Externes Terminal gestoppt.", "TextGedimmt");
        }
        catch (Exception ex)
        {
            Fortschritt.Protokoll(ex, "Externes Terminal");
            if (nr == laufNr) Status("Externes Terminal: " + ex.Message, "Rot");
        }
        finally
        {
            TemporaerenOrdnerLoeschen(ordner);
            LaufEnde();
        }
    }

    async void ExeExport_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new SaveFileDialog
        {
            Filter = "Windows-Programm (*.exe)|*.exe",
            FileName = "Programm.exe",
            DefaultExt = ".exe",
            Title = "Programm als Windows-EXE exportieren",
        };
        if (dlg.ShowDialog(this) != true) return;

        string code = Editor.Text;
        string ordner = null;
        ExeExportKnopf.IsEnabled = false;
        try
        {
            ordner = Directory.CreateTempSubdirectory("SharpLab-Exe-").FullName;
            Status("Erstelle Windows-EXE …", "TextMittel");
            string dotnet = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "dotnet", "dotnet.exe");
            if (!File.Exists(dotnet)) dotnet = "dotnet.exe";

            string projekt = Path.Combine(ordner, "Export.csproj");
            await File.WriteAllTextAsync(projekt, """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <OutputType>Exe</OutputType>
                    <TargetFramework>net8.0</TargetFramework>
                    <LangVersion>12.0</LangVersion>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <AssemblyName>SharpLabProgramm</AssemblyName>
                    <PublishSingleFile>true</PublishSingleFile>
                    <IncludeNativeLibrariesForSelfExtract>true</IncludeNativeLibrariesForSelfExtract>
                    <EnableCompressionInSingleFile>true</EnableCompressionInSingleFile>
                    <DebugType>none</DebugType>
                  </PropertyGroup>
                </Project>
                """);
            await File.WriteAllTextAsync(Path.Combine(ordner, "Program.cs"), code);

            var start = new ProcessStartInfo(dotnet)
            {
                WorkingDirectory = ordner,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            string zielOrdner = Path.Combine(ordner, "publish");
            foreach (string arg in new[] { "publish", projekt, "-c", "Release", "-r", "win-x64", "--self-contained", "true",
                       "-o", zielOrdner, "-p:ImportDirectoryBuildProps=false", "-p:ImportDirectoryBuildTargets=false" })
                start.ArgumentList.Add(arg);

            using var prozess = Process.Start(start) ?? throw new InvalidOperationException("Das .NET SDK konnte nicht gestartet werden.");
            var ausgabe = prozess.StandardOutput.ReadToEndAsync();
            var fehler = prozess.StandardError.ReadToEndAsync();
            await prozess.WaitForExitAsync();
            LogZeile(await ausgabe);
            string diagnose = await fehler;
            if (!string.IsNullOrWhiteSpace(diagnose)) LogZeile(diagnose);
            string ergebnis = Path.Combine(zielOrdner, "SharpLabProgramm.exe");
            if (prozess.ExitCode != 0 || !File.Exists(ergebnis))
            {
                TabLog.IsChecked = true;
                throw new InvalidOperationException("Der Build ist fehlgeschlagen. Prüfe das Compiler-Log und ob das .NET 8 SDK installiert ist.");
            }

            string temporaer = dlg.FileName + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                File.Copy(ergebnis, temporaer);
                File.Move(temporaer, dlg.FileName, overwrite: true);
            }
            finally
            {
                try { File.Delete(temporaer); } catch (IOException) { } catch (UnauthorizedAccessException) { }
            }
            Status("EXE gespeichert: " + Path.GetFileName(dlg.FileName), "Gruen");
        }
        catch (Exception ex)
        {
            Fortschritt.Protokoll(ex, "EXE exportieren");
            await DialogService.ShowAsync("EXE-Export fehlgeschlagen", ex.Message, DialogKind.Warnung, "OK",
                hint: "Der Export benötigt das .NET 8 SDK. Details stehen im Compiler-Log oder Fehlerprotokoll.");
        }
        finally
        {
            TemporaerenOrdnerLoeschen(ordner);
            ExeExportKnopf.IsEnabled = true;
        }
    }

    static void TemporaerenOrdnerLoeschen(string ordner)
    {
        if (ordner == null) return;
        try { Directory.Delete(ordner, recursive: true); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}
