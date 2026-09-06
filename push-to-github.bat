@echo off
chcp 65001 >nul 2>&1
setlocal enabledelayedexpansion
cd /d "%~dp0"
echo.
echo ================================================================
echo   TOOL FOR EXAM · PUSH TO GITHUB
echo ================================================================
echo.
echo   Repo: https://github.com/TheLER0N/tool-for-exam
echo   Dir : %~dp0
echo.
echo   [1] Full rewrite (force push) - old history removed
echo   [2] Update - normal commit on top of history
echo   [3] Cancel
echo.
set /p "CHOICE=  Choose [1/2/3] > "
if "%CHOICE%"=="3" goto :end
if "%CHOICE%"=="1" goto :full_rewrite
if "%CHOICE%"=="2" goto :normal_push
goto :end

:full_rewrite
echo.
echo   [!] Removing old history (.git)...
if exist ".git" rmdir /s /q ".git"
git init -b main
goto :add_remote

:normal_push
echo.
echo   [i] Updating existing history...
if not exist ".git" (
    git init -b main
)
goto :add_remote

:add_remote
git remote get-url origin >nul 2>&1
if errorlevel 1 (
    echo   [i] Adding remote origin...
    git remote add origin https://github.com/TheLER0N/tool-for-exam.git
)
goto :commit

:commit
if not exist ".gitignore" (
    (
        echo bin/
        echo obj/
        echo .vs/
        echo *.user
        echo *.exe
        echo *.pdb
        echo *.log
        echo favorites.txt
        echo *_report.txt
        echo push-to-github.bat
        echo Thumbs.db
    ) > ".gitignore"
)
echo   [i] Untracking build artifacts (bin/obj) if any...
git rm -r --cached bin obj >nul 2>&1
git add -A
git status --short
echo.
set /p "MSG=  Commit message [Tool for Exam update] > "
if "!MSG!"=="" set "MSG=Tool for Exam update"
git commit -m "!MSG!"
if "%CHOICE%"=="1" (
    echo.
    echo   [!] Force push to origin main...
    git push -f -u origin main
) else (
    echo.
    echo   [i] Push to origin main...
    git push -u origin main
)

:end
echo.
echo   Done.
pause