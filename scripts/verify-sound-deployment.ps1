# 验证声音文件部署脚本
# 用于检查 sounds 目录是否正确复制到输出目录

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "声音文件部署验证脚本" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 1. 检查源目录
$sourceDir = "E:\code\AdvancedClock\sounds\defaults"
Write-Host "1. 检查源目录..." -ForegroundColor Yellow
Write-Host "   路径: $sourceDir" -ForegroundColor Gray

if (Test-Path $sourceDir) {
    $sourceFiles = Get-ChildItem -Path $sourceDir -Filter "*.mp3"
    Write-Host "   ✓ 源目录存在" -ForegroundColor Green
    Write-Host "   ✓ 找到 $($sourceFiles.Count) 个 MP3 文件:" -ForegroundColor Green
    foreach ($file in $sourceFiles) {
        Write-Host "     - $($file.Name)" -ForegroundColor Gray
    }
} else {
    Write-Host "   ✗ 源目录不存在！" -ForegroundColor Red
    exit 1
}

Write-Host ""

# 2. 检查输出目录
$outputDir = "E:\code\AdvancedClock\bin\Debug\net8.0-windows\sounds\defaults"
Write-Host "2. 检查输出目录..." -ForegroundColor Yellow
Write-Host "   路径: $outputDir" -ForegroundColor Gray

if (Test-Path $outputDir) {
    $outputFiles = Get-ChildItem -Path $outputDir -Filter "*.mp3"
    Write-Host "   ✓ 输出目录存在" -ForegroundColor Green
    Write-Host "   ✓ 找到 $($outputFiles.Count) 个 MP3 文件:" -ForegroundColor Green
    foreach ($file in $outputFiles) {
        Write-Host "     - $($file.Name)" -ForegroundColor Gray
    }
    
    # 比较文件数量
    if ($sourceFiles.Count -eq $outputFiles.Count) {
        Write-Host "   ✓ 文件数量匹配！" -ForegroundColor Green
    } else {
        Write-Host "   ✗ 文件数量不匹配！源: $($sourceFiles.Count), 输出: $($outputFiles.Count)" -ForegroundColor Red
    }
} else {
    Write-Host "   ✗ 输出目录不存在！" -ForegroundColor Red
    Write-Host "   提示: 请先编译项目 (dotnet build)" -ForegroundColor Yellow
}

Write-Host ""

# 3. 检查项目文件配置
Write-Host "3. 检查项目文件配置..." -ForegroundColor Yellow
$projectFile = "E:\code\AdvancedClock\AdvancedClock.csproj"
$projectContent = Get-Content $projectFile -Raw

if ($projectContent -match '<None Include="sounds\\\*\*\\\*\.\*">') {
    Write-Host "   ✓ 项目文件已配置 sounds 目录复制" -ForegroundColor Green
} else {
    Write-Host "   ✗ 项目文件未配置 sounds 目录复制！" -ForegroundColor Red
    Write-Host "   提示: 请在 .csproj 文件中添加以下配置:" -ForegroundColor Yellow
    Write-Host @"
   <ItemGroup>
       <None Include="sounds\**\*.*">
           <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
           <Link>sounds\%(RecursiveDir)%(Filename)%(Extension)</Link>
       </None>
   </ItemGroup>
"@ -ForegroundColor Gray
}

Write-Host ""

# 4. 提供操作建议
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "操作建议" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

if (-not (Test-Path $outputDir)) {
    Write-Host "1. 清理项目: dotnet clean" -ForegroundColor Yellow
    Write-Host "2. 重新编译: dotnet build" -ForegroundColor Yellow
    Write-Host "3. 再次运行此脚本验证" -ForegroundColor Yellow
} else {
    Write-Host "✓ 声音文件部署正常！" -ForegroundColor Green
    Write-Host "  可以运行应用程序测试声音选择功能" -ForegroundColor Gray
}

Write-Host ""
Write-Host "按任意键退出..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
