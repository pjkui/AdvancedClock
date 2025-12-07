# 创建新版本发布的 PowerShell 脚本
# 使用方法: .\scripts\create-release.ps1 -Version "v1.0.0" -Message "版本 1.0.0 发布"

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [Parameter(Mandatory=$false)]
    [string]$Message = "版本 $Version 发布"
)

# 验证版本格式
if ($Version -notmatch '^v\d+\.\d+\.\d+(-\w+)?$') {
    Write-Error "版本格式错误！请使用格式：v1.0.0 或 v1.0.0-beta"
    exit 1
}

Write-Host "🚀 准备创建版本发布: $Version" -ForegroundColor Green

# 检查是否在 Git 仓库中
if (-not (Test-Path ".git")) {
    Write-Error "当前目录不是 Git 仓库！"
    exit 1
}

# 检查工作区是否干净
$status = git status --porcelain
if ($status) {
    Write-Warning "工作区有未提交的更改："
    git status --short
    $continue = Read-Host "是否继续创建发布？(y/N)"
    if ($continue -ne "y" -and $continue -ne "Y") {
        Write-Host "已取消发布创建。" -ForegroundColor Yellow
        exit 0
    }
}

# 检查标签是否已存在
$existingTag = git tag -l $Version
if ($existingTag) {
    Write-Error "标签 $Version 已存在！"
    exit 1
}

try {
    # 确保在主分支上
    $currentBranch = git branch --show-current
    if ($currentBranch -ne "main" -and $currentBranch -ne "master") {
        Write-Warning "当前不在主分支上，当前分支：$currentBranch"
        $continue = Read-Host "是否继续？(y/N)"
        if ($continue -ne "y" -and $continue -ne "Y") {
            Write-Host "已取消发布创建。" -ForegroundColor Yellow
            exit 0
        }
    }

    # 拉取最新代码
    Write-Host "📥 拉取最新代码..." -ForegroundColor Blue
    git pull origin $currentBranch

    # 创建标签
    Write-Host "🏷️  创建标签: $Version" -ForegroundColor Blue
    git tag -a $Version -m $Message

    # 推送标签到远程仓库
    Write-Host "📤 推送标签到远程仓库..." -ForegroundColor Blue
    git push origin $Version

    Write-Host "✅ 版本发布创建成功！" -ForegroundColor Green
    Write-Host ""
    Write-Host "📋 接下来的步骤：" -ForegroundColor Cyan
    Write-Host "1. GitHub Actions 将自动开始构建"
    Write-Host "2. 构建完成后会自动创建 GitHub Release"
    Write-Host "3. 发布包会自动上传到 Release 页面"
    Write-Host ""
    Write-Host "🔗 查看构建状态: https://github.com/$((git remote get-url origin) -replace '\.git$', '' -replace '^.*github\.com[:/]', 'https://github.com/')/actions" -ForegroundColor Blue
    Write-Host "🔗 查看发布页面: https://github.com/$((git remote get-url origin) -replace '\.git$', '' -replace '^.*github\.com[:/]', 'https://github.com/')/releases" -ForegroundColor Blue

} catch {
    Write-Error "创建发布时出错: $($_.Exception.Message)"
    exit 1
}