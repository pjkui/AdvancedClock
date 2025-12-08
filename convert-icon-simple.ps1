# SVG to ICO Converter
Write-Host "AdvancedClock Icon Converter" -ForegroundColor Cyan
Write-Host "============================" -ForegroundColor Cyan
Write-Host ""

$svgFile = "assets\icon.svg"

if (-not (Test-Path $svgFile)) {
    Write-Host "Error: SVG file not found!" -ForegroundColor Red
    exit 1
}

Write-Host "Found SVG file: $svgFile" -ForegroundColor Green
Write-Host ""

Write-Host "Opening online converter..." -ForegroundColor Yellow
Start-Process "https://convertio.co/zh/svg-ico/"

Write-Host ""
Write-Host "Instructions:" -ForegroundColor Cyan
Write-Host "1. Upload the assets\icon.svg file" -ForegroundColor White
Write-Host "2. Click Convert button" -ForegroundColor White
Write-Host "3. Download the generated icon.ico file" -ForegroundColor White
Write-Host "4. Save it to project root directory (replace existing icon.ico)" -ForegroundColor White
Write-Host ""

Write-Host "Press any key when conversion is complete..." -ForegroundColor Green
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

Write-Host ""
Write-Host "Rebuilding project..." -ForegroundColor Cyan
dotnet build

Write-Host ""
Write-Host "Conversion process completed!" -ForegroundColor Green