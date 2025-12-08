using System;
using AdvancedClock;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("高精度时钟测试程序");
        Console.WriteLine("==================");
        
        // 显示精度信息
        var info = HighPrecisionClock.GetPrecisionInfo();
        Console.WriteLine($"\n时钟精度信息:");
        Console.WriteLine($"  {info}");
        Console.WriteLine($"  高精度支持: {info.IsHighPrecisionSupported}");
        Console.WriteLine($"  高精度API: {info.IsHighPrecisionApiAvailable}");
        Console.WriteLine($"  分辨率: {info.ResolutionMicroseconds:F3} 微秒");
        
        // 时间对比测试
        Console.WriteLine($"\n时间对比测试:");
        for (int i = 0; i < 5; i++)
        {
            var standardTime = DateTime.Now;
            var highPrecisionTime = HighPrecisionClock.Now;
            var diff = (highPrecisionTime - standardTime).TotalMicroseconds;
            
            Console.WriteLine($"  标准时间: {standardTime:HH:mm:ss.fff}");
            Console.WriteLine($"  高精度时间: {highPrecisionTime:HH:mm:ss.fff}");
            Console.WriteLine($"  差异: {diff:F1} 微秒");
            Console.WriteLine();
            
            System.Threading.Thread.Sleep(100);
        }
        
        // 精度测试
        Console.WriteLine("连续时间获取测试 (观察最小时间差):");
        var lastTime = HighPrecisionClock.Now;
        var minDiff = TimeSpan.MaxValue;
        
        for (int i = 0; i < 100; i++)
        {
            var currentTime = HighPrecisionClock.Now;
            var diff = currentTime - lastTime;
            if (diff.Ticks > 0 && diff < minDiff)
            {
                minDiff = diff;
            }
            lastTime = currentTime;
        }
        
        Console.WriteLine($"  最小检测到的时间差: {minDiff.TotalMicroseconds:F3} 微秒");
        
        Console.WriteLine("\n测试完成，按任意键退出...");
        Console.ReadKey();
    }
}