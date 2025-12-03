
# AdvancedClock 图标转换脚本
# 此脚本帮助你快速将 SVG 图标转换为 ICO 格式

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  AdvancedClock 图标转换助手" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$svgFile = "assets/icon.svg"
$icoFile = "icon.ico"

# 检查 SVG 文件是否存在
if (-not (Test-Path $svgFile)) {
    Write-Host "❌ 错误: 找不到 $svgFile 文件！" -ForegroundColor Red
    Write-Host "请确保在项目根目录运行此脚本。" -ForegroundColor Yellow
    exit 1
}

    Write-Host "✅ 找到 SVG 文件: $svgFile" -ForegroundColor Green
Write-Host ""
Write-Host "⚠️  请确保在项目根目录运行此脚本！" -ForegroundColor Yellow
Write-Host ""

# 检查是否已安装 ImageMagick
$imageMagickInstalled = $false
try {
    $magickVersion = magick --version 2>$null
    if ($LASTEXITCODE -eq 0) {
        $imageMagickInstalled = $true
        Write-Host "✅ 检测到 ImageMagick 已安装" -ForegroundColor Green
    }
} catch {
    $imageMagickInstalled = $false
}

if ($imageMagickInstalled) {
    Write-Host ""
    Write-Host "正在使用 ImageMagick 转换图标..." -ForegroundColor Cyan
    Write-Host "命令: magick convert $svgFile -define icon:auto-resize=256,128,64,48,32,16 $icoFile" -ForegroundColor Gray
    Write-Host ""
    
    try {
        magick convert $svgFile -define icon:auto-resize=256,128,64,48,32,16 $icoFile
        
        if (Test-Path $icoFile) {
            Write-Host "✅ 成功生成 ICO 文件: $icoFile" -ForegroundColor Green
            Write-Host ""
            Write-Host "下一步操作:" -ForegroundColor Yellow
            Write-Host "1. 编辑 AdvancedClock.csproj" -ForegroundColor White
            Write-Host "2. 在 <PropertyGroup> 中添加: <ApplicationIcon>icon.ico</ApplicationIcon>" -ForegroundColor White
            Write-Host "3. 运行: dotnet build" -ForegroundColor White
            Write-Host ""
            Write-Host "🎉 完成！" -ForegroundColor Green
        } else {
            throw "转换失败"
        }
    } catch {
        Write-Host "❌ 转换失败: $_" -ForegroundColor Red
        $imageMagickInstalled = $false
    }
}

if (-not $imageMagickInstalled) {
    Write-Host ""
    Write-Host "📝 未检测到 ImageMagick，请使用以下方法之一转换图标：" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "方法 1: 在线转换（推荐，最简单）" -ForegroundColor Cyan
    Write-Host "---------------------------------------" -ForegroundColor Gray
    Write-Host "1. 访问: https://convertio.co/zh/svg-ico/" -ForegroundColor White
    Write-Host "2. 上传 icon.svg 文件" -ForegroundColor White
    Write-Host "3. 下载生成的 icon.ico 文件" -ForegroundColor White
    Write-Host "4. 将 icon.ico 保存到项目根目录" -ForegroundColor White
    Write-Host ""
    
    Write-Host "方法 2: 安装 ImageMagick" -ForegroundColor Cyan
    Write-Host "---------------------------------------" -ForegroundColor Gray
    Write-Host "1. 访问: https://imagemagick.org/script/download.php" -ForegroundColor White
    Write-Host "2. 下载并安装 Windows 版本" -ForegroundColor White
    Write-Host "3. 重新运行此脚本" -ForegroundColor White
    Write-Host ""
    
    Write-Host "方法 3: 使用 Inkscape（免费）" -ForegroundColor Cyan
    Write-Host "---------------------------------------" -ForegroundColor Gray
    Write-Host "1. 访问: https://inkscape.org/" -ForegroundColor White
    Write-Host "2. 下载并安装 Inkscape" -ForegroundColor White
    Write-Host "3. 打开 icon.svg" -ForegroundColor White
    Write-Host "4. 导出为多个尺寸的 PNG" -ForegroundColor White
    Write-Host "5. 使用在线工具合并为 ICO" -ForegroundColor White
    Write-Host ""
    
    # 询问是否打开在线转换网站
    Write-Host "是否现在打开在线转换网站？(Y/N): " -ForegroundColor Yellow -NoNewline
    $response = Read-Host
    
    if ($response -eq "Y" -or $response -eq "y") {
        Start-Process "https://convertio.co/zh/svg-ico/"
        Write-Host "✅ 已在浏览器中打开转换网站" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "📚 更多信息请查看: docs/ICON_GUIDE.md" -ForegroundColor Cyan
Write-Host ""
