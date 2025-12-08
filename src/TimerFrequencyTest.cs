using System;
using System.Diagnostics;
using System.Windows.Threading;

namespace AdvancedClock
{
    /// <summary>
    /// 定时器频率测试工具
    /// </summary>
    public static class TimerFrequencyTest
    {
        /// <summary>
        /// 测试定时器实际频率
        /// </summary>
        public static void TestTimerFrequency()
        {
            Console.WriteLine("=== 定时器频率测试 ===\n");

            const int testDurationSeconds = 10;
            const double expectedFrequency = 30.0; // 每秒30次
            
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000.0 / 30.0) // 33.33ms间隔
            };

            var tickCount = 0;
            var startTime = HighPrecisionClock.Now;
            var lastTickTime = startTime;
            var intervals = new System.Collections.Generic.List<double>();

            timer.Tick += (sender, e) =>
            {
                var currentTime = HighPrecisionClock.Now;
                
                if (tickCount > 0) // 跳过第一次，因为没有前一次的时间
                {
                    var interval = (currentTime - lastTickTime).TotalMilliseconds;
                    intervals.Add(interval);
                }
                
                tickCount++;
                lastTickTime = currentTime;

                // 测试持续时间
                if ((currentTime - startTime).TotalSeconds >= testDurationSeconds)
                {
                    timer.Stop();
                    AnalyzeResults(startTime, currentTime, tickCount, intervals, expectedFrequency);
                }
            };

            Console.WriteLine($"开始测试，持续时间: {testDurationSeconds} 秒");
            Console.WriteLine($"预期频率: {expectedFrequency} 次/秒");
            Console.WriteLine($"预期间隔: {1000.0 / expectedFrequency:F2} 毫秒");
            Console.WriteLine("测试中...\n");

            timer.Start();

            // 等待测试完成
            var dispatcher = Dispatcher.CurrentDispatcher;
            var frame = new DispatcherFrame();
            
            timer.Tick += (s, e) =>
            {
                if (!timer.IsEnabled)
                    frame.Continue = false;
            };

            Dispatcher.PushFrame(frame);
        }

        private static void AnalyzeResults(DateTime startTime, DateTime endTime, int tickCount, 
            System.Collections.Generic.List<double> intervals, double expectedFrequency)
        {
            var totalDuration = (endTime - startTime).TotalSeconds;
            var actualFrequency = tickCount / totalDuration;
            var expectedInterval = 1000.0 / expectedFrequency;

            Console.WriteLine("=== 测试结果 ===");
            Console.WriteLine($"总持续时间: {totalDuration:F3} 秒");
            Console.WriteLine($"总触发次数: {tickCount}");
            Console.WriteLine($"实际频率: {actualFrequency:F2} 次/秒");
            Console.WriteLine($"预期频率: {expectedFrequency:F2} 次/秒");
            Console.WriteLine($"频率误差: {Math.Abs(actualFrequency - expectedFrequency):F2} 次/秒 ({Math.Abs(actualFrequency - expectedFrequency) / expectedFrequency * 100:F1}%)");

            if (intervals.Count > 0)
            {
                // 分析间隔统计
                var sum = 0.0;
                var min = double.MaxValue;
                var max = double.MinValue;

                foreach (var interval in intervals)
                {
                    sum += interval;
                    if (interval < min) min = interval;
                    if (interval > max) max = interval;
                }

                var avgInterval = sum / intervals.Count;
                var intervalError = Math.Abs(avgInterval - expectedInterval);

                Console.WriteLine($"\n=== 间隔分析 ===");
                Console.WriteLine($"平均间隔: {avgInterval:F2} 毫秒");
                Console.WriteLine($"预期间隔: {expectedInterval:F2} 毫秒");
                Console.WriteLine($"间隔误差: {intervalError:F2} 毫秒 ({intervalError / expectedInterval * 100:F1}%)");
                Console.WriteLine($"最小间隔: {min:F2} 毫秒");
                Console.WriteLine($"最大间隔: {max:F2} 毫秒");
                Console.WriteLine($"间隔范围: {max - min:F2} 毫秒");

                // 计算标准差
                var sumSquares = 0.0;
                foreach (var interval in intervals)
                {
                    var diff = interval - avgInterval;
                    sumSquares += diff * diff;
                }
                var stdDev = Math.Sqrt(sumSquares / intervals.Count);
                Console.WriteLine($"标准差: {stdDev:F2} 毫秒");

                // 精度评估
                Console.WriteLine($"\n=== 精度评估 ===");
                if (intervalError < 1.0)
                    Console.WriteLine("✅ 优秀: 间隔误差 < 1ms");
                else if (intervalError < 5.0)
                    Console.WriteLine("✅ 良好: 间隔误差 < 5ms");
                else if (intervalError < 10.0)
                    Console.WriteLine("⚠️  一般: 间隔误差 < 10ms");
                else
                    Console.WriteLine("❌ 较差: 间隔误差 >= 10ms");

                if (stdDev < 2.0)
                    Console.WriteLine("✅ 稳定性优秀: 标准差 < 2ms");
                else if (stdDev < 5.0)
                    Console.WriteLine("✅ 稳定性良好: 标准差 < 5ms");
                else
                    Console.WriteLine("⚠️  稳定性一般: 标准差 >= 5ms");
            }

            Console.WriteLine("\n测试完成！");
        }

        /// <summary>
        /// 比较不同频率的性能
        /// </summary>
        public static void CompareFrequencies()
        {
            Console.WriteLine("=== 不同频率性能比较 ===\n");

            var frequencies = new double[] { 1, 10, 30, 60 }; // 每秒次数

            foreach (var freq in frequencies)
            {
                Console.WriteLine($"测试频率: {freq} 次/秒");
                
                var interval = TimeSpan.FromMilliseconds(1000.0 / freq);
                var testDuration = TimeSpan.FromSeconds(5);
                
                // 测量CPU使用率和精度
                var sw = Stopwatch.StartNew();
                var tickCount = 0;
                var timer = new DispatcherTimer { Interval = interval };
                
                timer.Tick += (s, e) =>
                {
                    tickCount++;
                    if (sw.Elapsed >= testDuration)
                    {
                        timer.Stop();
                    }
                };

                var startCpuTime = Process.GetCurrentProcess().TotalProcessorTime;
                timer.Start();

                // 等待测试完成
                while (timer.IsEnabled)
                {
                    System.Threading.Thread.Sleep(10);
                }

                var endCpuTime = Process.GetCurrentProcess().TotalProcessorTime;
                var cpuUsage = (endCpuTime - startCpuTime).TotalMilliseconds / sw.Elapsed.TotalMilliseconds * 100;

                Console.WriteLine($"  实际触发: {tickCount} 次");
                Console.WriteLine($"  预期触发: {freq * testDuration.TotalSeconds:F0} 次");
                Console.WriteLine($"  CPU使用: {cpuUsage:F3}%");
                Console.WriteLine($"  精度: ±{interval.TotalMilliseconds / 2:F1} ms\n");
            }
        }
    }
}