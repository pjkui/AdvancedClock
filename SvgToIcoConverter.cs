using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace AdvancedClock
{
    /// <summary>
    /// SVG 转 ICO 转换器
    /// 注意：这是一个简化的实现，实际项目中建议使用专业的转换工具
    /// </summary>
    public static class SvgToIcoConverter
    {
        /// <summary>
        /// 创建一个简单的 ICO 文件（使用默认图标作为临时解决方案）
        /// </summary>
        public static void CreateTempIcon(string outputPath)
        {
            try
            {
                // 创建一个简单的彩色图标
                using (var bitmap = new Bitmap(256, 256))
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    // 设置高质量渲染
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    
                    // 绘制一个时钟图标
                    var centerX = 128;
                    var centerY = 128;
                    var radius = 100;
                    
                    // 绘制外圆（表盘）
                    using (var brush = new SolidBrush(Color.FromArgb(70, 130, 180))) // 钢蓝色
                    {
                        graphics.FillEllipse(brush, centerX - radius, centerY - radius, radius * 2, radius * 2);
                    }
                    
                    // 绘制内圆（表盘边框）
                    using (var pen = new Pen(Color.White, 8))
                    {
                        graphics.DrawEllipse(pen, centerX - radius, centerY - radius, radius * 2, radius * 2);
                    }
                    
                    // 绘制时钟刻度
                    using (var pen = new Pen(Color.White, 4))
                    {
                        for (int i = 0; i < 12; i++)
                        {
                            double angle = i * Math.PI / 6;
                            int x1 = centerX + (int)((radius - 20) * Math.Sin(angle));
                            int y1 = centerY - (int)((radius - 20) * Math.Cos(angle));
                            int x2 = centerX + (int)((radius - 10) * Math.Sin(angle));
                            int y2 = centerY - (int)((radius - 10) * Math.Cos(angle));
                            graphics.DrawLine(pen, x1, y1, x2, y2);
                        }
                    }
                    
                    // 绘制时针（指向3点）
                    using (var pen = new Pen(Color.White, 6))
                    {
                        graphics.DrawLine(pen, centerX, centerY, centerX + 50, centerY);
                    }
                    
                    // 绘制分针（指向12点）
                    using (var pen = new Pen(Color.White, 4))
                    {
                        graphics.DrawLine(pen, centerX, centerY, centerX, centerY - 70);
                    }
                    
                    // 绘制中心点
                    using (var brush = new SolidBrush(Color.White))
                    {
                        graphics.FillEllipse(brush, centerX - 8, centerY - 8, 16, 16);
                    }
                    
                    // 保存为 ICO 文件
                    SaveAsIcon(bitmap, outputPath);
                }
                
                Console.WriteLine($"成功创建图标文件: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"创建图标失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 将 Bitmap 保存为 ICO 文件
        /// </summary>
        private static void SaveAsIcon(Bitmap bitmap, string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                // ICO 文件头
                fileStream.Write(new byte[] { 0, 0, 1, 0, 1, 0 }, 0, 6);
                
                // 图标目录条目
                fileStream.WriteByte(0); // 宽度 (0 = 256)
                fileStream.WriteByte(0); // 高度 (0 = 256)
                fileStream.WriteByte(0); // 颜色数
                fileStream.WriteByte(0); // 保留
                fileStream.Write(BitConverter.GetBytes((short)1), 0, 2); // 颜色平面
                fileStream.Write(BitConverter.GetBytes((short)32), 0, 2); // 位深度
                
                // 将 bitmap 转换为 PNG 数据
                using (var memoryStream = new MemoryStream())
                {
                    bitmap.Save(memoryStream, ImageFormat.Png);
                    var pngData = memoryStream.ToArray();
                    
                    // 写入数据大小和偏移
                    fileStream.Write(BitConverter.GetBytes(pngData.Length), 0, 4);
                    fileStream.Write(BitConverter.GetBytes(22), 0, 4); // 偏移 (6 + 16)
                    
                    // 写入 PNG 数据
                    fileStream.Write(pngData, 0, pngData.Length);
                }
            }
        }
    }
}