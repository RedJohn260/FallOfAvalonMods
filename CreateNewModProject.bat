@echo off

:: %1 is the full path of the folder you dropped onto the script
set "TARGET_PATH=%~1"
:: %~n1 extracts just the folder name from that path
set "FOLDER_NAME=%~n1"

:: Check if a folder was actually dropped
if "%TARGET_PATH%"=="" (
    echo Error: Please drag and drop a folder onto this script.
    pause
    exit /b
)

echo Processing Mod: %FOLDER_NAME%
echo Path: %TARGET_PATH%
echo ------------------------------------------

:: 1. Install/Update the template
dotnet new install ./templates/mod --force

:: 2. Change directory to the dropped folder
cd /d "%TARGET_PATH%"

:: 3. Generate the project using the template
dotnet new tgfoamod --modAuthor "RedJohn260"

:: 4. Add the project to the solution file located in the parent directory
:: (Assuming the .sln is one level above where the folder was created)
dotnet sln ../FallOfAvalonMods.sln add .

echo ------------------------------------------
echo Success! %FOLDER_NAME% is now part of the solution.
pause