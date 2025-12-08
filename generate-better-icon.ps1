# 生成更好的图标文件
Add-Type -AssemblyName System.Drawing

Write-Host "Generating custom clock icon..." -ForegroundColor Cyan

# 创建 256x256 的位图
$bitmap = New-Object System.Drawing.Bitmap(256, 256)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)

# 设置高质量渲染
$graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$graphics.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality

# 绘制时钟图标
$centerX = 128
$centerY = 128
$radius = 100

# 绘制外圆（表盘）- 使用渐变色
$brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
    [System.Drawing.Point]::new(0, 0),
    [System.Drawing.Point]::new(256, 256),
    [System.Drawing.Color]::FromArgb(70, 130, 180),
    [System.Drawing.Color]::FromArgb(100, 149, 237)
)
$graphics.FillEllipse($brush, $centerX - $radius, $centerY - $radius, $radius * 2, $radius * 2)
$brush.Dispose()

# 绘制内圆边框
$pen = New-Object System.Drawing.Pen([System.Drawing.Color]::White, 8)
$graphics.DrawEllipse($pen, $centerX - $radius, $centerY - $radius, $radius * 2, $radius * 2)
$pen.Dispose()

# 绘制时钟刻度
$pen = New-Object System.Drawing.Pen([System.Drawing.Color]::White, 4)
for ($i = 0; $i -lt 12; $i++) {
    $angle = $i * [Math]::PI / 6
    $x1 = $centerX + [int](($radius - 20) * [Math]::Sin($angle))
    $y1 = $centerY - [int](($radius - 20) * [Math]::Cos($angle))
    $x2 = $centerX + [int](($radius - 10) * [Math]::Sin($angle))
    $y2 = $centerY - [int](($radius - 10) * [Math]::Cos($angle))
    $graphics.DrawLine($pen, $x1, $y1, $x2, $y2)
}
$pen.Dispose()

# 绘制时针（指向3点）
$pen = New-Object System.Drawing.Pen([System.Drawing.Color]::White, 6)
$graphics.DrawLine($pen, $centerX, $centerY, $centerX + 50, $centerY)
$pen.Dispose()

# 绘制分针（指向12点）
$pen = New-Object System.Drawing.Pen([System.Drawing.Color]::White, 4)
$graphics.DrawLine($pen, $centerX, $centerY, $centerX, $centerY - 70)
$pen.Dispose()

# 绘制中心点
$brush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
$graphics.FillEllipse($brush, $centerX - 8, $centerY - 8, 16, 16)
$brush.Dispose()

# 保存为临时 PNG 文件
$tempPng = "temp_icon.png"
$bitmap.Save($tempPng, [System.Drawing.Imaging.ImageFormat]::Png)

# 创建 ICO 文件
$icon = [System.Drawing.Icon]::FromHandle($bitmap.GetHicon())
$fileStream = [System.IO.FileStream]::new("icon.ico", [System.IO.FileMode]::Create)
$icon.Save($fileStream)
$fileStream.Close()

# 清理资源
$graphics.Dispose()
$bitmap.Dispose()
$icon.Dispose()

# 删除临时文件
if (Test-Path $tempPng) {
    Remove-Item $tempPng
}

Write-Host "Custom icon generated successfully: icon.ico" -ForegroundColor Green
Write-Host "File size: $([math]::Round((Get-Item 'icon.ico').Length / 1KB, 2)) KB" -ForegroundColor White