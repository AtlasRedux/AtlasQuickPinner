@echo off
echo Building QuickPinner (single-file self-contained)...
cd /d "%~dp0"
dotnet publish QuickPinner.csproj -c Release -o publish -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
if %errorlevel% neq 0 (
    echo Build failed!
    exit /b %errorlevel%
)
echo Build successful!
echo Output: %~dp0publish\