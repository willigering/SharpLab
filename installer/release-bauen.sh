#!/usr/bin/env bash
# Baut SharpLab-Windows.zip (portabel, für das Auto-Update) und SharpLab-Setup.exe samt .sha256.
# Aufruf aus dem Projektordner:  installer/release-bauen.sh <Ausgabeordner>
# Benötigt: .NET 8 SDK, NSIS (makensis), python3.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
OUT="$(mkdir -p "${1:-$ROOT/release}" && cd "${1:-$ROOT/release}" && pwd)"
VERSION="$(sed -n 's:.*<Version>\(.*\)</Version>.*:\1:p' "$ROOT/src/CSharpLernen/CSharpLernen.csproj")"
PUB="$OUT/publish"; rm -rf "$PUB"
dotnet publish "$ROOT/src/CSharpLernen/CSharpLernen.csproj" -c Release -r win-x64 --self-contained false \
  -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=false -p:EnableWindowsTargeting=true -p:RollForward=Major -o "$PUB"
rm -f "$PUB"/*.pdb
cp "$ROOT/installer/LIESMICH-App.txt" "$PUB/LIESMICH.txt"
cp "$ROOT/LICENSE" "$ROOT/THIRD-PARTY-NOTICES.txt" "$PUB/"
python3 - "$PUB" "$OUT" <<'PY'
import sys, zipfile, os
pub, out = sys.argv[1], sys.argv[2]
with zipfile.ZipFile(os.path.join(out, 'SharpLab-Windows.zip'), 'w', zipfile.ZIP_DEFLATED, compresslevel=9) as z:
    for f in ['SharpLab.exe', 'LIESMICH.txt', 'LICENSE', 'THIRD-PARTY-NOTICES.txt']:
        z.write(os.path.join(pub, f), 'SharpLab/' + f)
lic = open(os.path.join(pub, 'LICENSE'), encoding='utf-8').read().replace('\r\n', '\n').replace('\n', '\r\n')
open(os.path.join(pub, 'LICENSE-crlf.txt'), 'w', encoding='utf-8', newline='').write(lic)
PY
cp "$ROOT/src/CSharpLernen/Ressourcen/app.ico" "$PUB/"
makensis -WX -V2 -DVERSION="$VERSION" -DSRC="$PUB" -DOUT="$OUT/SharpLab-Setup.exe" "$ROOT/installer/SharpLab.nsi"
cd "$OUT"
for f in SharpLab-Windows.zip SharpLab-Setup.exe; do sha256sum "$f" > "$f.sha256"; done
cat ./*.sha256
