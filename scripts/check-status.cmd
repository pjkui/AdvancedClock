@echo off
echo.
echo === AdvancedClock Project Status Check ===
echo.

echo Checking .NET version...
dotnet --version
echo.

echo Checking project structure...
if exist "AdvancedClock.csproj" (
    echo ✓ Project file exists
) else (
    echo ✗ Project file missing
    exit /b 1
)

if exist "src" (
    echo ✓ Source directory exists
) else (
    echo ✗ Source directory missing
)
echo.

echo Checking NuGet packages...
dotnet restore AdvancedClock.sln --verbosity minimal
echo.

echo Checking build status...
dotnet build AdvancedClock.csproj --configuration Debug --verbosity minimal --no-restore
echo.

echo Checking output files...
if exist "bin\Debug\net8.0-windows\AdvancedClock.exe" (
    echo ✓ Debug executable exists
) else (
    echo ✗ Debug executable not found
)
echo.

echo === Status check completed ===
echo.
echo VS Code Tips:
echo   - Press Ctrl+Shift+B to build quickly
echo   - Press F5 to start debugging
echo   - Use 'run-app' task to run the program
echo.