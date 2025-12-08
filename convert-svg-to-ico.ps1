# SVG 转 ICO 图标转换脚本
# 自动打开在线转换工具并提供操作指导

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  AdvancedClock SVG 转 ICO 图标转换" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$svgFile = "assets\icon.svg"
$icoFile = "icon.ico"

# 检查 SVG 文件是否存在
if (-not (Test-Path $svgFile)) {
    Write-Host "❌ 错误: 找不到 $svgFile 文件！" -ForegroundColor Red
    exit 1
}

Write-Host "✅ 找到 SVG 文件: $svgFile" -ForegroundColor Green
Write-Host ""

# 显示文件信息
$svgInfo = Get-Item $svgFile
Write-Host "📄 文件信息:" -ForegroundColor Yellow
Write-Host "   文件大小: $([math]::Round($svgInfo.Length / 1KB, 2)) KB" -ForegroundColor White
Write-Host "   修改时间: $($svgInfo.LastWriteTime)" -ForegroundColor White
Write-Host ""

Write-Host "🔄 开始转换流程..." -ForegroundColor Cyan
Write-Host ""

Write-Host "方法 1: 使用 Convertio (推荐)" -ForegroundColor Green
Write-Host "---------------------------------------" -ForegroundColor Gray
Write-Host "1. 即将打开 https://convertio.co/zh/svg-ico/" -ForegroundColor White
Write-Host "2. 上传 assets\icon.svg 文件" -ForegroundColor White
Write-Host "3. 点击 '转换' 按钮" -ForegroundColor White
Write-Host "4. 下载生成的 icon.ico 文件" -ForegroundColor White
Write-Host "5. 将下载的文件保存到项目根目录，替换现有的 icon.ico" -ForegroundColor White
Write-Host ""

Write-Host "方法 2: 使用 AConvert" -ForegroundColor Green
Write-Host "---------------------------------------" -ForegroundColor Gray
Write-Host "1. 访问 https://www.aconvert.com/cn/icon/svg-to-ico/" -ForegroundColor White
Write-Host "2. 选择多个尺寸: 16x16, 32x32, 48x48, 64x64, 128x128, 256x256" -ForegroundColor White
Write-Host "3. 上传并转换" -ForegroundColor White
Write-Host ""

Write-Host "方法 3: 使用元素空间" -ForegroundColor Green
Write-Host "---------------------------------------" -ForegroundColor Gray
Write-Host "1. 访问 https://ico.elespaces.com/" -ForegroundColor White
Write-Host "2. 拖拽上传 SVG 文件" -ForegroundColor White
Write-Host "3. 选择多种尺寸生成" -ForegroundColor White
Write-Host ""

# 询问用户选择
Write-Host "请选择转换方法:" -ForegroundColor Yellow
Write-Host "[1] 打开 Convertio (推荐)" -ForegroundColor White
Write-Host "[2] 打开 AConvert" -ForegroundColor White
Write-Host "[3] 打开元素空间" -ForegroundColor White
Write-Host "[4] 打开所有网站" -ForegroundColor White
Write-Host "[0] 取消" -ForegroundColor White
Write-Host ""
Write-Host "请输入选择 (1-4): " -ForegroundColor Yellow -NoNewline

$choice = Read-Host

switch ($choice) {
    "1" {
        Write-Host "正在打开 Convertio..." -ForegroundColor Green
        Start-Process "https://convertio.co/zh/svg-ico/"
    }
    "2" {
        Write-Host "正在打开 AConvert..." -ForegroundColor Green
        Start-Process "https://www.aconvert.com/cn/icon/svg-to-ico/"
    }
    "3" {
        Write-Host "正在打开元素空间..." -ForegroundColor Green
        Start-Process "https://ico.elespaces.com/"
    }
    "4" {
        Write-Host "正在打开所有转换网站..." -ForegroundColor Green
        Start-Process "https://convertio.co/zh/svg-ico/"
        Start-Sleep -Seconds 1
        Start-Process "https://www.aconvert.com/cn/icon/svg-to-ico/"
        Start-Sleep -Seconds 1
        Start-Process "https://ico.elespaces.com/"
    }
    "0" {
        Write-Host "已取消转换。" -ForegroundColor Yellow
        exit 0
    }
    default {
        Write-Host "无效选择，正在打开默认网站..." -ForegroundColor Yellow
        Start-Process "https://convertio.co/zh/svg-ico/"
    }
}

Write-Host ""
Write-Host "📋 转换完成后的操作步骤:" -ForegroundColor Cyan
Write-Host "---------------------------------------" -ForegroundColor Gray
Write-Host "1. 将下载的 icon.ico 文件保存到项目根目录" -ForegroundColor White
Write-Host "2. 替换现有的 icon.ico 文件" -ForegroundColor White
Write-Host "3. 运行: dotnet build" -ForegroundColor White
Write-Host "4. 运行: dotnet run" -ForegroundColor White
Write-Host ""

Write-Host "⚠️  重要提示:" -ForegroundColor Yellow
Write-Host "• 建议选择多种尺寸 (16x16, 32x32, 48x48, 64x64, 128x128, 256x256)" -ForegroundColor White
Write-Host "• 确保下载的文件名为 icon.ico" -ForegroundColor White
Write-Host "• 如果图标有透明背景，请选择支持透明度的转换选项" -ForegroundColor White
Write-Host ""

Write-Host "🎯 转换完成后，请按任意键继续..." -ForegroundColor Green
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

# 检查是否已经有新的 icon.ico 文件
if (Test-Path $icoFile) {
    $icoInfo = Get-Item $icoFile
    Write-Host ""
    Write-Host "✅ 检测到 icon.ico 文件:" -ForegroundColor Green
    Write-Host "   文件大小: $([math]::Round($icoInfo.Length / 1KB, 2)) KB" -ForegroundColor White
    Write-Host "   修改时间: $($icoInfo.LastWriteTime)" -ForegroundColor White
    
    Write-Host ""
    Write-Host "🔨 正在重新构建项目..." -ForegroundColor Cyan
    try {
        $buildResult = dotnet build 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ 项目构建成功！" -ForegroundColor Green
            Write-Host ""
            Write-Host "🚀 是否现在运行程序测试图标？(Y/N): " -ForegroundColor Yellow -NoNewline
            $runChoice = Read-Host
            
            if ($runChoice -eq "Y" -or $runChoice -eq "y") {
                Write-Host "正在启动程序..." -ForegroundColor Green
                dotnet run
            }
        } else {
            Write-Host "❌ 项目构建失败:" -ForegroundColor Red
            Write-Host $buildResult -ForegroundColor Red
        }
    } catch {
        Write-Host "❌ 构建过程中出现错误: $_" -ForegroundColor Red
    }
} else {
    Write-Host ""
    Write-Host "⚠️  未检测到新的 icon.ico 文件" -ForegroundColor Yellow
    Write-Host "请确保已下载并保存 icon.ico 文件到项目根目录" -ForegroundColor White
}

Write-Host ""
Write-Host "🎉 脚本执行完成！" -ForegroundColor Green