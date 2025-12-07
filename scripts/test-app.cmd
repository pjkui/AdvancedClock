@echo off
echo.
echo === Testing AdvancedClock Application ===
echo.

echo Building application...
dotnet build AdvancedClock.csproj --configuration Debug --verbosity minimal
if %ERRORLEVEL% neq 0 (
    echo Build failed!
    exit /b 1
)

echo.
echo Checking executable...
if not exist "bin\Debug\net8.0-windows\AdvancedClock.exe" (
    echo Executable not found!
    exit /b 1
)

echo.
echo Testing application startup...
echo Starting AdvancedClock.exe for 5 seconds...

start "" "bin\Debug\net8.0-windows\AdvancedClock.exe"

echo Waiting 5 seconds...
timeout /t 5 /nobreak > nul

echo.
echo Checking if application is running...
tasklist /fi "imagename eq AdvancedClock.exe" 2>nul | find /i "AdvancedClock.exe" >nul
if %ERRORLEVEL% equ 0 (
    echo ✓ Application is running successfully!
    echo Terminating application...
    taskkill /f /im AdvancedClock.exe >nul 2>&1
) else (
    echo ✗ Application failed to start or crashed!
    echo.
    echo Checking Windows Event Log for errors...
    powershell -Command "Get-WinEvent -FilterHashtable @{LogName='Application'; Level=2; StartTime=(Get-Date).AddMinutes(-1)} | Where-Object {$_.ProcessId -ne 0} | Select-Object -First 3 | Format-Table TimeCreated, Id, LevelDisplayName, Message -Wrap"
)

echo.
echo === Test completed ===