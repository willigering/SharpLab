; Test der .NET-Prüfung: schreibt 1/0 nach $EXEDIR\dotnet-ergebnis.txt
Unicode true
RequestExecutionLevel user
OutFile "${OUT}"
!include "LogicLib.nsh"
!include "WordFunc.nsh"
!include "x64.nsh"
!include "${__FILEDIR__}\dotnet-pruefen.nsh"
Function .onInit
  Call DotnetPruefen
  FileOpen $0 "$EXEDIR\dotnet-ergebnis.txt" w
  FileWrite $0 $R9
  FileClose $0
  Quit
FunctionEnd
Section
SectionEnd
