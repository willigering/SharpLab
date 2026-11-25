; SharpLab – Installer (NSIS 3, Modern UI 2, Deutsch)
; Bauen:  makensis -DVERSION=1.0.18 -DSRC=<Ordner mit SharpLab.exe usw.> -DOUT=<Ziel.exe> installer/SharpLab.nsi
; Installation pro Benutzer ohne Adminrechte nach %LOCALAPPDATA%\Programs\SharpLab,
; damit das Auto-Update der App die exe im Installationsordner austauschen darf.

Unicode true
ManifestDPIAware true
SetCompressor /SOLID lzma
RequestExecutionLevel user

!ifndef VERSION
  !define VERSION "1.0.18"
!endif
!ifndef SRC
  !define SRC "..\publish"
!endif
!ifndef OUT
  !define OUT "SharpLab-Setup.exe"
!endif

!define APPNAME   "SharpLab"
!define FIRMA     "Gering Digital"
!define AUTOR     "Willi Gering"
!define EXE       "SharpLab.exe"
!define UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\SharpLab"
!define APP_KEY    "Software\Gering Digital\SharpLab"
!define WEB       "https://github.com/willigering/SharpLab"
!define DOTNET_DIREKT "https://aka.ms/dotnet/8.0/windowsdesktop-runtime-win-x64.exe"
!define DOTNET_SEITE  "https://dotnet.microsoft.com/download/dotnet/8.0"

!include "MUI2.nsh"
!include "LogicLib.nsh"
!include "FileFunc.nsh"
!include "WordFunc.nsh"
!include "x64.nsh"

Name "${APPNAME} ${VERSION}"
Caption "${APPNAME} ${VERSION} – Setup"
OutFile "${OUT}"
InstallDir "$LOCALAPPDATA\Programs\SharpLab"
InstallDirRegKey HKCU "${APP_KEY}" "InstallDir"
BrandingText "${APPNAME} ${VERSION} · ${FIRMA}"
ShowInstDetails nevershow
ShowUninstDetails nevershow

; ---------------------------------------------------------------- Versionsinfo der Setup.exe
VIProductVersion "${VERSION}.0"
VIFileVersion "${VERSION}.0"
VIAddVersionKey /LANG=1031 "ProductName" "${APPNAME}"
VIAddVersionKey /LANG=1031 "ProductVersion" "${VERSION}"
VIAddVersionKey /LANG=1031 "FileVersion" "${VERSION}"
VIAddVersionKey /LANG=1031 "FileDescription" "${APPNAME} ${VERSION} Setup – Der interaktive C#-Trainer"
VIAddVersionKey /LANG=1031 "CompanyName" "${FIRMA}"
VIAddVersionKey /LANG=1031 "LegalCopyright" "Copyright (c) 2026 ${AUTOR}, ${FIRMA}"
VIAddVersionKey /LANG=1031 "Comments" "Installiert SharpLab für den aktuellen Benutzer (ohne Adminrechte)."

; ---------------------------------------------------------------- Oberfläche
!define MUI_ICON   "${SRC}/app.ico"
!define MUI_UNICON "${SRC}/app.ico"
!define MUI_ABORTWARNING
!define MUI_ABORTWARNING_TEXT "Möchtest du die Installation von SharpLab wirklich abbrechen?"
!define MUI_COMPONENTSPAGE_NODESC

!define MUI_WELCOMEPAGE_TITLE "Willkommen bei SharpLab ${VERSION}"
!define MUI_WELCOMEPAGE_TEXT "SharpLab ist der interaktive C#-Trainer von ${FIRMA}.$\r$\n$\r$\nDie Installation läuft nur für dein Benutzerkonto und braucht keine Administratorrechte. Ist SharpLab schon installiert, wird es einfach aktualisiert – dein Lernstand bleibt erhalten.$\r$\n$\r$\nKlicke auf „Weiter“, um fortzufahren."
!insertmacro MUI_PAGE_WELCOME
!define MUI_LICENSEPAGE_TEXT_TOP "SharpLab steht unter der MIT-Lizenz:"
!define MUI_LICENSEPAGE_TEXT_BOTTOM "Die Lizenzen der mitgelieferten Komponenten (Roslyn, AvalonEdit, JetBrains Mono u. a.) stehen in THIRD-PARTY-NOTICES.txt. Klicke auf „Annehmen“, um fortzufahren."
!insertmacro MUI_PAGE_LICENSE "${SRC}/LICENSE-crlf.txt"
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!define MUI_FINISHPAGE_TITLE "SharpLab ist bereit"
!define MUI_FINISHPAGE_TEXT "SharpLab ${VERSION} wurde installiert.$\r$\n$\r$\nDein Lernstand wird unter %AppData%\CSharpLernen gespeichert. Neue Versionen meldet SharpLab beim Start selbst."
!define MUI_FINISHPAGE_RUN "$INSTDIR\${EXE}"
!define MUI_FINISHPAGE_RUN_TEXT "SharpLab starten"
!define MUI_FINISHPAGE_SHOWREADME "$INSTDIR\LIESMICH.txt"
!define MUI_FINISHPAGE_SHOWREADME_TEXT "LIESMICH anzeigen"
!define MUI_FINISHPAGE_SHOWREADME_NOTCHECKED
!insertmacro MUI_PAGE_FINISH

!define MUI_UNCONFIRMPAGE_TEXT_TOP "SharpLab wird von deinem Computer entfernt.$\r$\n$\r$\nDein Lernstand und deine Einstellungen (%AppData%\CSharpLernen) bleiben erhalten – bei einer erneuten Installation geht es dort weiter, wo du aufgehört hast."
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_LANGUAGE "German"

; ---------------------------------------------------------------- Hilfsfunktionen

; Läuft SharpLab noch? Eine laufende exe lässt sich nicht zum Schreiben öffnen.
!macro WarteBisGeschlossen UN
Function ${UN}WarteBisGeschlossen
  ${If} ${FileExists} "$INSTDIR\${EXE}"
    nochmal:
    ClearErrors
    FileOpen $0 "$INSTDIR\${EXE}" a
    ${If} ${Errors}
      MessageBox MB_RETRYCANCEL|MB_ICONEXCLAMATION "SharpLab läuft noch.$\r$\n$\r$\nBitte schließe SharpLab (dein Fortschritt wird dabei gespeichert) und klicke dann auf „Wiederholen“." /SD IDCANCEL IDRETRY nochmal
      Abort
    ${EndIf}
    FileClose $0
  ${EndIf}
FunctionEnd
!macroend
!insertmacro WarteBisGeschlossen ""
!insertmacro WarteBisGeschlossen "un."

!include "${__FILEDIR__}\dotnet-pruefen.nsh"

Function .onInit
  ; Upgrade: den bisherigen Installationsordner verwenden (InstallDirRegKey); sonst Standard.
  Call DotnetPruefen
  ${If} $R9 == 0
    MessageBox MB_YESNO|MB_ICONINFORMATION "SharpLab braucht die kostenlose „.NET 8 Desktop Runtime“ von Microsoft, und die ist auf diesem Computer noch nicht installiert.$\r$\n$\r$\nSoll der Download von Microsoft jetzt im Browser starten? Installiere die Runtime einfach nebenbei – SharpLab wird trotzdem schon eingerichtet.$\r$\n$\r$\n(Download-Seite: ${DOTNET_SEITE})" /SD IDNO IDNO weiter
    ExecShell "open" "${DOTNET_DIREKT}"
    weiter:
  ${EndIf}
FunctionEnd

; ---------------------------------------------------------------- Abschnitte

Section "SharpLab (erforderlich)" SecApp
  SectionIn RO
  SetOutPath "$INSTDIR"
  Call WarteBisGeschlossen
  ; Reste eines früheren Auto-Updates aufräumen
  Delete "$INSTDIR\${EXE}.old"
  Delete "$INSTDIR\LIESMICH.txt.old"
  Delete "$INSTDIR\LICENSE.old"
  Delete "$INSTDIR\THIRD-PARTY-NOTICES.txt.old"
  File "${SRC}/${EXE}"
  File "${SRC}/LIESMICH.txt"
  File "${SRC}/LICENSE"
  File "${SRC}/THIRD-PARTY-NOTICES.txt"
  WriteUninstaller "$INSTDIR\Uninstall.exe"

  ; Startmenü
  CreateShortcut "$SMPROGRAMS\SharpLab.lnk" "$INSTDIR\${EXE}" "" "$INSTDIR\${EXE}" 0 SW_SHOWNORMAL "" "SharpLab – Der interaktive C#-Trainer"

  ; Eintrag unter „Apps & Features“ (pro Benutzer)
  WriteRegStr HKCU "${APP_KEY}" "InstallDir" "$INSTDIR"
  WriteRegStr HKCU "${UNINST_KEY}" "DisplayName" "${APPNAME}"
  WriteRegStr HKCU "${UNINST_KEY}" "DisplayVersion" "${VERSION}"
  WriteRegStr HKCU "${UNINST_KEY}" "Publisher" "${FIRMA}"
  WriteRegStr HKCU "${UNINST_KEY}" "DisplayIcon" "$INSTDIR\${EXE},0"
  WriteRegStr HKCU "${UNINST_KEY}" "InstallLocation" "$INSTDIR"
  WriteRegStr HKCU "${UNINST_KEY}" "UninstallString" '"$INSTDIR\Uninstall.exe"'
  WriteRegStr HKCU "${UNINST_KEY}" "QuietUninstallString" '"$INSTDIR\Uninstall.exe" /S'
  WriteRegStr HKCU "${UNINST_KEY}" "URLInfoAbout" "${WEB}"
  WriteRegStr HKCU "${UNINST_KEY}" "HelpLink" "${WEB}"
  WriteRegStr HKCU "${UNINST_KEY}" "URLUpdateInfo" "${WEB}/releases/latest"
  WriteRegDWORD HKCU "${UNINST_KEY}" "NoModify" 1
  WriteRegDWORD HKCU "${UNINST_KEY}" "NoRepair" 1
  ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
  IntFmt $0 "0x%08X" $0
  WriteRegDWORD HKCU "${UNINST_KEY}" "EstimatedSize" "$0"
SectionEnd

Section "Desktop-Verknüpfung" SecDesktop
  CreateShortcut "$DESKTOP\SharpLab.lnk" "$INSTDIR\${EXE}" "" "$INSTDIR\${EXE}" 0 SW_SHOWNORMAL "" "SharpLab – Der interaktive C#-Trainer"
SectionEnd

; ---------------------------------------------------------------- Deinstallation

Section "Uninstall"
  Call un.WarteBisGeschlossen
  Delete "$INSTDIR\${EXE}"
  Delete "$INSTDIR\${EXE}.old"
  Delete "$INSTDIR\LIESMICH.txt"
  Delete "$INSTDIR\LIESMICH.txt.old"
  Delete "$INSTDIR\LICENSE"
  Delete "$INSTDIR\LICENSE.old"
  Delete "$INSTDIR\THIRD-PARTY-NOTICES.txt"
  Delete "$INSTDIR\THIRD-PARTY-NOTICES.txt.old"
  Delete "$INSTDIR\Uninstall.exe"
  RMDir "$INSTDIR"
  Delete "$SMPROGRAMS\SharpLab.lnk"
  Delete "$DESKTOP\SharpLab.lnk"
  DeleteRegKey HKCU "${UNINST_KEY}"
  DeleteRegKey HKCU "${APP_KEY}"
  DeleteRegKey /ifempty HKCU "Software\Gering Digital"
  ; Lernstand (%AppData%\CSharpLernen) bleibt absichtlich erhalten.
SectionEnd
