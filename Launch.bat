@echo off
if exist publish\AtlasQuickPinner.exe (
    start "" "publish\AtlasQuickPinner.exe"
) else (
    echo Build first: run build.bat
    start "" "QuickPinner.exe"
)