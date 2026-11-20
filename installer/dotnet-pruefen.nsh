; .NET-8-Desktop-Runtime-Prüfung für SharpLab.nsi (eigene Datei, damit sie testbar ist: dotnet-pruefen-test.nsi)
; Prüft, ob die .NET Desktop Runtime 8 oder neuer (x64) vorhanden ist. Ergebnis in $R9: 1 = ja, 0 = nein.
Function DotnetPruefen
  StrCpy $R9 0
  ; 1) Registry (64- und 32-Bit-Ansicht)
  ${If} ${RunningX64}
    SetRegView 64
  ${EndIf}
  Call DotnetRegistry
  SetRegView 32
  ${If} $R9 == 0
    Call DotnetRegistry
  ${EndIf}
  SetRegView default
  ; 2) Ordner %ProgramFiles%\dotnet\shared\Microsoft.WindowsDesktop.App\<Version>
  ${If} $R9 == 0
    ${If} ${RunningX64}
      StrCpy $R8 "$PROGRAMFILES64\dotnet\shared\Microsoft.WindowsDesktop.App"
    ${Else}
      StrCpy $R8 "$PROGRAMFILES\dotnet\shared\Microsoft.WindowsDesktop.App"
    ${EndIf}
    FindFirst $R6 $R7 "$R8\*"
    ${DoWhile} $R7 != ""
      ${If} ${FileExists} "$R8\$R7\*.dll"
        Push $R7
        Call IstAbVersion8
        Pop $R5
        ${If} $R5 == 1
          StrCpy $R9 1
          ${Break}
        ${EndIf}
      ${EndIf}
      FindNext $R6 $R7
    ${Loop}
    FindClose $R6
  ${EndIf}
FunctionEnd

Function DotnetRegistry
  StrCpy $R4 0
  ${Do}
    EnumRegValue $R7 HKLM "SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App" $R4
    ${If} $R7 == ""
      ${Break}
    ${EndIf}
    Push $R7
    Call IstAbVersion8
    Pop $R5
    ${If} $R5 == 1
      StrCpy $R9 1
      ${Break}
    ${EndIf}
    IntOp $R4 $R4 + 1
  ${Loop}
FunctionEnd

; Stack: "8.0.11" → 1 (Hauptversion >= 8), sonst 0
Function IstAbVersion8
  Exch $0
  Push $1
  ${WordFind} $0 "." "+1" $1
  IntCmp $1 8 ja nein ja
  ja:
    StrCpy $0 1
    Goto fertig
  nein:
    StrCpy $0 0
  fertig:
  Pop $1
  Exch $0
FunctionEnd

