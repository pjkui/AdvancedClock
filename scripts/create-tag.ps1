# Git 标签创建脚本
# 用于快速创建和推送版本标签

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [Parameter(Mandatory=$false)]
    [string]$Message = "",
    
    [Parameter(Mandatory=$false)]
    [switch]$Push = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$Annotated = $false
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Git 标签创建脚本" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 验证版本号格式
if ($Version -notmatch '^v?\d+\.\d+\.\d+') {
    Write-Host "✗ 版本号格式错误！" -ForegroundColor Red
    Write-Host "  正确格式: v2.5.2 或 2.5.2" -ForegroundColor Yellow
    exit 1
}

# 确保版本号以 'v' 开头
if ($Version -notmatch '^v') {
    $Version = "v$Version"
}

Write-Host "版本号: $Version" -ForegroundColor Green

# 检查工作区状态
Write-Host ""
Write-Host "1. 检查工作区状态..." -ForegroundColor Yellow
$status = git status --porcelain
if ($status) {
    Write-Host "  ⚠ 工作区有未提交的更改：" -ForegroundColor Yellow
    git status --short
    Write-Host ""
    $continue = Read-Host "  是否继续创建标签？(y/n)"
    if ($continue -ne 'y') {
        Write-Host "  已取消" -ForegroundColor Gray
        exit 0
    }
}
else {
    Write-Host "  ✓ 工作区干净" -ForegroundColor Green
}

# 检查标签是否已存在
Write-Host ""
Write-Host "2. 检查标签是否存在..." -ForegroundColor Yellow
$existingTag = git tag -l $Version
if ($existingTag) {
    Write-Host "  ✗ 标签 $Version 已存在！" -ForegroundColor Red
    Write-Host ""
    $overwrite = Read-Host "  是否删除并重新创建？(y/n)"
    if ($overwrite -eq 'y') {
        Write-Host "  删除本地标签..." -ForegroundColor Yellow
        git tag -d $Version
        
        Write-Host "  删除远程标签..." -ForegroundColor Yellow
        git push origin --delete $Version 2>$null
        
        Write-Host "  ✓ 已删除旧标签" -ForegroundColor Green
    }
    else {
        Write-Host "  已取消" -ForegroundColor Gray
        exit 0
    }
}
else {
    Write-Host "  ✓ 标签不存在，可以创建" -ForegroundColor Green
}

# 显示最近的提交
Write-Host ""
Write-Host "3. 最近的提交记录：" -ForegroundColor Yellow
git log --oneline -5
Write-Host ""

# 创建标签
Write-Host "4. 创建标签..." -ForegroundColor Yellow

if ($Annotated) {
    # 创建附注标签
    if ([string]::IsNullOrWhiteSpace($Message)) {
        $Message = Read-Host "  请输入标签说明"
    }
    
    Write-Host "  创建附注标签: $Version" -ForegroundColor Gray
    Write-Host "  说明: $Message" -ForegroundColor Gray
    
    git tag -a $Version -m $Message
}
else {
    # 创建轻量级标签
    Write-Host "  创建轻量级标签: $Version" -ForegroundColor Gray
    git tag $Version
}

if ($LASTEXITCODE -eq 0) {
    Write-Host "  ✓ 标签创建成功" -ForegroundColor Green
}
else {
    Write-Host "  ✗ 标签创建失败" -ForegroundColor Red
    exit 1
}

# 显示标签信息
Write-Host ""
Write-Host "5. 标签信息：" -ForegroundColor Yellow
git show $Version --stat

# 推送标签
if ($Push) {
    Write-Host ""
    Write-Host "6. 推送标签到远程仓库..." -ForegroundColor Yellow
    git push origin $Version
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ✓ 标签推送成功" -ForegroundColor Green
    }
    else {
        Write-Host "  ✗ 标签推送失败" -ForegroundColor Red
        exit 1
    }
}
else {
    Write-Host ""
    Write-Host "6. 标签未推送到远程仓库" -ForegroundColor Yellow
    Write-Host "  提示: 使用 -Push 参数自动推送，或手动执行：" -ForegroundColor Gray
    Write-Host "  git push origin $Version" -ForegroundColor Gray
}

# 显示所有标签
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "当前所有标签：" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
git tag --sort=-version:refname

Write-Host ""
Write-Host "✓ 完成！" -ForegroundColor Green
Write-Host ""

# 提供后续操作建议
Write-Host "后续操作建议：" -ForegroundColor Yellow
Write-Host "1. 推送标签: git push origin $Version" -ForegroundColor Gray
Write-Host "2. 创建 GitHub Release: https://github.com/pjkui/AdvancedClock/releases/new" -ForegroundColor Gray
Write-Host "3. 更新 README.md 中的版本记录" -ForegroundColor Gray
Write-Host "4. 编译并发布二进制文件" -ForegroundColor Gray
Write-Host ""
