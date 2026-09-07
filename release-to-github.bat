@echo off
chcp 65001 >nul 2>&1
setlocal enabledelayedexpansion
cd /d "%~dp0"

set "PROJ=%~dp0Ekzamen.csproj"
set "BUILD=%~dp0release\build"
set "RELDIR=%~dp0release"

echo.
echo ================================================================
echo   TOOL FOR EXAM - RELEASE TO GITHUB
echo ================================================================
echo.
echo   Repo   : https://github.com/TheLER0N/tool-for-exam
echo   Project: %PROJ%
echo   Dir    : %~dp0
echo.

where dotnet >nul 2>&1
if errorlevel 1 (
    echo   [XX] dotnet not found. Install .NET SDK.
    pause
    exit /b 1
)
if not exist "%PROJ%" (
    echo   [XX] Ekzamen.csproj not found: %PROJ%
    pause
    exit /b 1
)

set /p "VER=   Version (e.g. 1.0.0): "
if "!VER!"=="" (
    echo   [XX] Version required.
    pause
    exit /b 1
)
set /p "TITLE=   Title (Enter = tool-for-exam v!VER!): "
if "!TITLE!"=="" set "TITLE=tool-for-exam v!VER!"
set /p "DESC=    Description (Enter = default): "
if "!DESC!"=="" set "DESC=tool-for-exam v!VER! - dark-themed WinForms panel with form duplication."

echo.
echo ================================================================
echo   Version : !VER!
echo   Title   : !TITLE!
echo   Build   : %BUILD%
echo ================================================================
echo.
set /p "CONFIRM=   Continue? [Y/n]: "
if /i "!CONFIRM!"=="n" (
    echo   Cancelled.
    pause
    exit /b 0
)

echo.
echo   [1/3] dotnet publish (Release, net48)...
if exist "%BUILD%" (
    for %%F in ("%BUILD%\*.exe" "%BUILD%\*.dll" "%BUILD%\*.config" "%BUILD%\*.pdb") do (
        del "%%F" 2>nul
    )
)
if not exist "%RELDIR%" mkdir "%RELDIR%"
dotnet publish "%PROJ%" -c Release -o "%BUILD%" --nologo -v q
if errorlevel 1 (
    echo.
    echo   [XX] Build failed.
    pause
    exit /b 1
)
echo   [OK] Built into %BUILD%

echo.
echo   [2/3] Creating ZIP archive...
set "ZIP_NAME=tool-for-exam-v!VER!-win-x64.zip"
set "ZIP_PATH=%RELDIR%\!ZIP_NAME!"
if exist "!ZIP_PATH!" del "!ZIP_PATH!"
powershell -NoProfile -Command "Compress-Archive -Path '%BUILD%\*' -DestinationPath '!ZIP_PATH!' -Force"
if errorlevel 1 (
    echo   [XX] ZIP creation failed.
    pause
    exit /b 1
)
for %%A in ("!ZIP_PATH!") do set /a "ZIP_KB=%%~zA / 1024"
echo   [OK] Archive: !ZIP_PATH! (~!ZIP_KB! KB)

echo|set /p="!ZIP_PATH!" | clip

echo.
echo   [3/3] Opening GitHub Releases page...
echo.
echo ================================================================
echo   READY
echo ================================================================
echo.
echo   1. Tag version   : !VER!
echo   2. Release title : !TITLE!
echo   3. Description   : !DESC!
echo   4. Attach binary : press Ctrl+V in the file picker
echo                       (path is already in clipboard)
echo   5. Publish release
echo.
echo   ZIP path copied to clipboard:
echo     !ZIP_PATH!
echo.

start "" "https://github.com/TheLER0N/tool-for-exam/releases/new?tag=v!VER!&title=!TITLE!"
pause