using System;
using System.Diagnostics;
using System.Threading;

namespace AdvancedClock
{
    /// <summary>
    /// 时钟精度测试工具
    /// </summary>
    public static class ClockPrecisionTest
    {
        /// <summary>
        /// 运行时钟精度测试
        /// </summary>
        public static void RunPrecisionTest()
        {
            Console.WriteLine("=== 高精度时钟测试 ===\n");

            // 显示时钟精度信息
            var precisionInfo = HighPrecisionClock.GetPrecisionInfo();
            Console.WriteLine($"时钟精度信息:");
            Console.WriteLine($"  {precisionInfo}");
            Console.WriteLine($"  高精度支持: {precisionInfo.IsHighPrecisionSupported}");
            Console.WriteLine($"  高精度API可用: {precisionInfo.IsHighPrecisionApiAvailable}");
            Console.WriteLine();

            // 测试时间获取性能
            TestTimeRetrievalPerformance();

            // 测试时间精度
            TestTimePrecision();

            // 测试高精度延迟
            TestHighPrecisionDelay();

            Console.WriteLine("测试完成！");
        }

        /// <summary>
        /// 测试时间获取性能
        /// </summary>
        private static void TestTimeRetrievalPerformance()
        {
            Console.WriteLine("=== 时间获取性能测试 ===");
            const int iterations = 1000000;

            // 测试 DateTime.Now
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                var _ = DateTime.Now;
            }
            sw.Stop();
            var dateTimeNowTime = sw.Elapsed.TotalMicroseconds / iterations;

            // 测试 HighPrecisionClock.Now
            sw.Restart();
            for (int i = 0; i < iterations; i++)
            {
                var _ = HighPrecisionClock.Now;
            }
            sw.Stop();
            var highPrecisionTime = sw.Elapsed.TotalMicroseconds / iterations;

            Console.WriteLine($"DateTime.Now 平均耗时: {dateTimeNowTime:F3} 微秒");
            Console.WriteLine($"HighPrecisionClock.Now 平均耗时: {highPrecisionTime:F3} 微秒");
            Console.WriteLine($"性能比较: {(highPrecisionTime / dateTimeNowTime):F2}x");
            Console.WriteLine();
        }

        /// <summary>
        /// 测试时间精度
        /// </summary>
        private static void TestTimePrecision()
        {
            Console.WriteLine("=== 时间精度测试 ===");
            
            // 连续获取时间，观察最小时间差
            var samples = new TimeSpan[1000];
            var lastTime = HighPrecisionClock.Now;

            for (int i = 0; i < samples.Length; i++)
            {
                var currentTime = HighPrecisionClock.Now;
                samples[i] = currentTime - lastTime;
                lastTime = currentTime;
            }

            // 分析时间差
            var minDiff = TimeSpan.MaxValue;
            var maxDiff = TimeSpan.MinValue;
            var totalTicks = 0L;
            var nonZeroCount = 0;

            foreach (var sample in samples)
            {
                if (sample.Ticks > 0)
                {
                    if (sample < minDiff) minDiff = sample;
                    if (sample > maxDiff) maxDiff = sample;
                    totalTicks += sample.Ticks;
                    nonZeroCount++;
                }
            }

            if (nonZeroCount > 0)
            {
                var avgTicks = totalTicks / nonZeroCount;
                var avgMicroseconds = avgTicks / 10.0; // 1 tick = 100 nanoseconds

                Console.WriteLine($"最小时间差: {minDiff.TotalMicroseconds:F3} 微秒");
                Console.WriteLine($"最大时间差: {maxDiff.TotalMicroseconds:F3} 微秒");
                Console.WriteLine($"平均时间差: {avgMicroseconds:F3} 微秒");
                Console.WriteLine($"有效样本数: {nonZeroCount}/{samples.Length}");
            }
            else
            {
                Console.WriteLine("警告: 未检测到时间差异，可能精度不足");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// 测试高精度延迟
        /// </summary>
        private static void TestHighPrecisionDelay()
        {
            Console.WriteLine("=== 高精度延迟测试 ===");

            var testDelays = new[] { 100, 500, 1000, 5000 }; // 微秒

            foreach (var targetDelay in testDelays)
            {
                Console.WriteLine($"测试 {targetDelay} 微秒延迟:");

                var measurements = new double[10];
                for (int i = 0; i < measurements.Length; i++)
                {
                    var actualDelay = HighPrecisionClock.MeasureExecutionTime(() =>
                    {
                        HighPrecisionClock.DelayMicroseconds(targetDelay);
                    });
                    measurements[i] = actualDelay;
                }

                // 计算统计信息
                var sum = 0.0;
                var min = double.MaxValue;
                var max = double.MinValue;

                foreach (var measurement in measurements)
                {
                    sum += measurement;
                    if (measurement < min) min = measurement;
                    if (measurement > max) max = measurement;
                }

                var avg = sum / measurements.Length;
                var error = Math.Abs(avg - targetDelay);
                var errorPercent = (error / targetDelay) * 100;

                Console.WriteLine($"  目标: {targetDelay} μs, 实际: {avg:F1} μs (±{(max - min) / 2:F1})");
                Console.WriteLine($"  误差: {error:F1} μs ({errorPercent:F1}%)");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// 比较不同时钟源的稳定性
        /// </summary>
        public static void CompareClockStability()
        {
            Console.WriteLine("=== 时钟稳定性比较 ===");
            const int samples = 100;
            const int intervalMs = 10;

            // 测试 DateTime.Now
            Console.WriteLine("测试 DateTime.Now 稳定性...");
            TestClockStability("DateTime.Now", samples, intervalMs, () => DateTime.Now);

            // 测试 HighPrecisionClock.Now
            Console.WriteLine("测试 HighPrecisionClock.Now 稳定性...");
            TestClockStability("HighPrecisionClock.Now", samples, intervalMs, () => HighPrecisionClock.Now);
        }

        private static void TestClockStability(string clockName, int samples, int intervalMs, Func<DateTime> getClock)
        {
            var intervals = new double[samples - 1];
            var lastTime = getClock();

            for (int i = 0; i < samples - 1; i++)
            {
                Thread.Sleep(intervalMs);
                var currentTime = getClock();
                intervals[i] = (currentTime - lastTime).TotalMilliseconds;
                lastTime = currentTime;
            }

            // 计算统计信息
            var sum = 0.0;
            var sumSquares = 0.0;
            foreach (var interval in intervals)
            {
                sum += interval;
                sumSquares += interval * interval;
            }

            var mean = sum / intervals.Length;
            var variance = (sumSquares / intervals.Length) - (mean * mean);
            var stdDev = Math.Sqrt(variance);

            Console.WriteLine($"  {clockName}:");
            Console.WriteLine($"    平均间隔: {mean:F3} ms (目标: {intervalMs} ms)");
            Console.WriteLine($"    标准差: {stdDev:F3} ms");
            Console.WriteLine($"    变异系数: {(stdDev / mean * 100):F2}%");
            Console.WriteLine();
        }
    }
}