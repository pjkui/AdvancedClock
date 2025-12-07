@echo off
echo.
echo === Checking AdvancedClock Resources ===
echo.

echo Building application...
dotnet build AdvancedClock.csproj --configuration Debug --verbosity minimal
if %ERRORLEVEL% neq 0 (
    echo Build failed!
    exit /b 1
)

echo.
echo Checking XAML files in source...
if exist "src\App.xaml" (
    echo ✓ App.xaml found
) else (
    echo ✗ App.xaml missing
)

if exist "src\MainWindow.xaml" (
    echo ✓ MainWindow.xaml found
) else (
    echo ✗ MainWindow.xaml missing
)

if exist "src\AlarmEditDialog.xaml" (
    echo ✓ AlarmEditDialog.xaml found
) else (
    echo ✗ AlarmEditDialog.xaml missing
)

if exist "src\StrongAlertWindow.xaml" (
    echo ✓ StrongAlertWindow.xaml found
) else (
    echo ✗ StrongAlertWindow.xaml missing
)

echo.
echo Checking embedded resources in assembly...
powershell -Command "try { [System.Reflection.Assembly]::LoadFrom('bin\Debug\net8.0-windows\AdvancedClock.dll').GetManifestResourceNames() | Where-Object { $_ -like '*.xaml' -or $_ -like '*.baml' } | ForEach-Object { Write-Host \"✓ Resource: $_\" } } catch { Write-Host \"✗ Failed to load assembly or check resources\" }"

echo.
echo Testing application startup...
start "" "bin\Debug\net8.0-windows\AdvancedClock.exe"
timeout /t 3 /nobreak > nul

tasklist /fi "imagename eq AdvancedClock.exe" 2>nul | find /i "AdvancedClock.exe" >nul
if %ERRORLEVEL% equ 0 (
    echo ✓ Application started successfully - resources are working!
    taskkill /f /im AdvancedClock.exe >nul 2>&1
) else (
    echo ✗ Application failed to start - possible resource issue!
)

echo.
echo === Resource check completed ===