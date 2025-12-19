# 默认闹钟声音下载助手
# 本脚本提供快速访问免费音效网站的链接

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   AdvancedClock 默认声音下载助手" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 检查 defaults 目录是否存在
$defaultsPath = Join-Path $PSScriptRoot "defaults"
if (-not (Test-Path $defaultsPath)) {
    Write-Host "创建 defaults 目录..." -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $defaultsPath | Out-Null
    Write-Host "✓ defaults 目录已创建" -ForegroundColor Green
} else {
    Write-Host "✓ defaults 目录已存在" -ForegroundColor Green
}

# 检查 custom 目录是否存在
$customPath = Join-Path $PSScriptRoot "custom"
if (-not (Test-Path $customPath)) {
    Write-Host "创建 custom 目录..." -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $customPath | Out-Null
    Write-Host "✓ custom 目录已创建" -ForegroundColor Green
} else {
    Write-Host "✓ custom 目录已存在" -ForegroundColor Green
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   推荐的免费音效网站" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 定义音效网站
$websites = @(
    @{
        Name = "Pixabay (推荐)"
        URL = "https://pixabay.com/sound-effects/search/alarm/"
        Description = "完全免费，无需署名，可商用"
        SearchKeywords = "alarm, bell, notification"
    },
    @{
        Name = "Freesound"
        URL = "https://freesound.org/search/?q=alarm+clock"
        Description = "大量音效，需要注册"
        SearchKeywords = "alarm clock, morning alarm, beep"
    },
    @{
        Name = "Mixkit"
        URL = "https://mixkit.co/free-sound-effects/alarm/"
        Description = "高质量音效，可商用"
        SearchKeywords = "alarm, notification"
    },
    @{
        Name = "Zapsplat"
        URL = "https://www.zapsplat.com/sound-effect-category/alarms-and-sirens/"
        Description = "免费音效库，需要注册"
        SearchKeywords = "alarm clock, wake up"
    },
    @{
        Name = "SoundBible"
        URL = "https://soundbible.com/suggest.php?q=alarm&x=0&y=0"
        Description = "简单易用，分类清晰"
        SearchKeywords = "alarm, clock"
    }
)

# 显示网站列表
for ($i = 0; $i -lt $websites.Count; $i++) {
    $site = $websites[$i]
    Write-Host "[$($i + 1)] $($site.Name)" -ForegroundColor Yellow
    Write-Host "    URL: $($site.URL)" -ForegroundColor Gray
    Write-Host "    说明: $($site.Description)" -ForegroundColor Gray
    Write-Host "    搜索关键词: $($site.SearchKeywords)" -ForegroundColor Gray
    Write-Host ""
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   推荐的默认声音" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 推荐的声音列表
$recommendedSounds = @(
    @{Name = "alarm_01_classic_bell.wav"; Description = "经典机械闹钟铃声"; Style = "传统、熟悉"},
    @{Name = "alarm_02_electronic_beep.wav"; Description = "电子哔哔声"; Style = "现代、简洁"},
    @{Name = "alarm_03_gentle_piano.wav"; Description = "柔和钢琴旋律"; Style = "温和、舒缓"},
    @{Name = "alarm_04_bird_chirping.wav"; Description = "清晨鸟鸣声"; Style = "自然、清新"},
    @{Name = "alarm_05_emergency_alert.wav"; Description = "紧急警报声"; Style = "强烈、紧迫"}
)

Write-Host "基础包（推荐下载以下5个声音）：" -ForegroundColor Green
Write-Host ""
foreach ($sound in $recommendedSounds) {
    Write-Host "  • $($sound.Name)" -ForegroundColor Yellow
    Write-Host "    $($sound.Description) - $($sound.Style)" -ForegroundColor Gray
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   使用步骤" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "1. 选择一个网站（推荐从 Pixabay 开始）" -ForegroundColor White
Write-Host "2. 搜索并下载喜欢的闹钟声音" -ForegroundColor White
Write-Host "3. 将下载的文件保存到 'defaults' 目录" -ForegroundColor White
Write-Host "4. 按照推荐格式重命名文件" -ForegroundColor White
Write-Host "5. 在 AdvancedClock 中选择并使用" -ForegroundColor White

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   快速操作" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 提供快速操作选项
Write-Host "请选择操作：" -ForegroundColor Yellow
Write-Host "[1] 打开 Pixabay 音效页面（推荐）" -ForegroundColor White
Write-Host "[2] 打开 Freesound 音效页面" -ForegroundColor White
Write-Host "[3] 打开 Mixkit 音效页面" -ForegroundColor White
Write-Host "[4] 打开 defaults 目录" -ForegroundColor White
Write-Host "[5] 打开 custom 目录" -ForegroundColor White
Write-Host "[6] 查看详细指南（打开 DEFAULT_SOUNDS_GUIDE.md）" -ForegroundColor White
Write-Host "[0] 退出" -ForegroundColor White
Write-Host ""

$choice = Read-Host "请输入选项 (0-6)"

switch ($choice) {
    "1" {
        Write-Host "正在打开 Pixabay..." -ForegroundColor Green
        Start-Process "https://pixabay.com/sound-effects/search/alarm/"
    }
    "2" {
        Write-Host "正在打开 Freesound..." -ForegroundColor Green
        Start-Process "https://freesound.org/search/?q=alarm+clock"
    }
    "3" {
        Write-Host "正在打开 Mixkit..." -ForegroundColor Green
        Start-Process "https://mixkit.co/free-sound-effects/alarm/"
    }
    "4" {
        Write-Host "正在打开 defaults 目录..." -ForegroundColor Green
        Start-Process $defaultsPath
    }
    "5" {
        Write-Host "正在打开 custom 目录..." -ForegroundColor Green
        Start-Process $customPath
    }
    "6" {
        $guidePath = Join-Path $PSScriptRoot "DEFAULT_SOUNDS_GUIDE.md"
        if (Test-Path $guidePath) {
            Write-Host "正在打开详细指南..." -ForegroundColor Green
            Start-Process $guidePath
        } else {
            Write-Host "找不到指南文件！" -ForegroundColor Red
        }
    }
    "0" {
        Write-Host "再见！" -ForegroundColor Green
    }
    default {
        Write-Host "无效的选项！" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "提示：下载音效后，请将文件放入 'defaults' 目录" -ForegroundColor Yellow
Write-Host "目录路径：$defaultsPath" -ForegroundColor Gray
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 检查是否已有音效文件
$existingFiles = Get-ChildItem -Path $defaultsPath -Include *.wav,*.mp3,*.wma,*.m4a -Recurse -ErrorAction SilentlyContinue
if ($existingFiles.Count -gt 0) {
    Write-Host "✓ 已找到 $($existingFiles.Count) 个音效文件：" -ForegroundColor Green
    foreach ($file in $existingFiles) {
        Write-Host "  • $($file.Name)" -ForegroundColor Gray
    }
} else {
    Write-Host "⚠ defaults 目录中还没有音效文件" -ForegroundColor Yellow
    Write-Host "  请从上述网站下载音效文件并放入此目录" -ForegroundColor Gray
}

Write-Host ""
Write-Host "按任意键退出..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
