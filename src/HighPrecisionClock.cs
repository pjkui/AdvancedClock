using System;
using System.Runtime.InteropServices;

namespace AdvancedClock
{
    /// <summary>
    /// 高精度时钟服务，使用Windows高精度时间API
    /// </summary>
    public static class HighPrecisionClock
    {
        #region Windows API 声明

        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        [DllImport("kernel32.dll")]
        private static extern void GetSystemTimeAsFileTime(out long lpSystemTimeAsFileTime);

        [DllImport("kernel32.dll")]
        private static extern void GetSystemTimePreciseAsFileTime(out long lpSystemTimeAsFileTime);

        #endregion

        private static readonly long _frequency;
        private static readonly long _startCounter;
        private static readonly DateTime _startTime;
        private static readonly bool _useHighPrecisionApi;

        /// <summary>
        /// 静态构造函数，初始化高精度时钟
        /// </summary>
        static HighPrecisionClock()
        {
            // 检查是否支持高精度性能计数器
            if (QueryPerformanceFrequency(out _frequency) && _frequency > 0)
            {
                QueryPerformanceCounter(out _startCounter);
                
                // 尝试使用高精度系统时间API（Windows 8及以上版本）
                try
                {
                    GetSystemTimePreciseAsFileTime(out long fileTime);
                    _startTime = DateTime.FromFileTime(fileTime);
                    _useHighPrecisionApi = true;
                }
                catch
                {
                    // 降级到标准API
                    GetSystemTimeAsFileTime(out long fileTime);
                    _startTime = DateTime.FromFileTime(fileTime);
                    _useHighPrecisionApi = false;
                }
            }
            else
            {
                // 性能计数器不可用，使用标准时间
                _startTime = DateTime.Now;
                _frequency = 0;
                _useHighPrecisionApi = false;
            }
        }

        /// <summary>
        /// 获取当前高精度时间
        /// </summary>
        public static DateTime Now
        {
            get
            {
                if (_frequency == 0)
                {
                    // 降级到标准DateTime.Now
                    return DateTime.Now;
                }

                // 使用性能计数器计算经过的时间
                QueryPerformanceCounter(out long currentCounter);
                long elapsedTicks = currentCounter - _startCounter;
                double elapsedSeconds = (double)elapsedTicks / _frequency;
                
                return _startTime.AddSeconds(elapsedSeconds);
            }
        }

        /// <summary>
        /// 获取当前高精度UTC时间
        /// </summary>
        public static DateTime UtcNow
        {
            get
            {
                if (_useHighPrecisionApi)
                {
                    try
                    {
                        GetSystemTimePreciseAsFileTime(out long fileTime);
                        return DateTime.FromFileTimeUtc(fileTime);
                    }
                    catch
                    {
                        // 降级处理
                    }
                }

                // 使用标准API或降级处理
                GetSystemTimeAsFileTime(out long standardFileTime);
                return DateTime.FromFileTimeUtc(standardFileTime);
            }
        }

        /// <summary>
        /// 获取时钟精度信息
        /// </summary>
        public static ClockPrecisionInfo GetPrecisionInfo()
        {
            return new ClockPrecisionInfo
            {
                IsHighPrecisionSupported = _frequency > 0,
                IsHighPrecisionApiAvailable = _useHighPrecisionApi,
                Frequency = _frequency,
                ResolutionMicroseconds = _frequency > 0 ? 1_000_000.0 / _frequency : 1000.0, // 标准精度约1ms
                ApiUsed = _useHighPrecisionApi ? "GetSystemTimePreciseAsFileTime" : 
                         _frequency > 0 ? "QueryPerformanceCounter" : "DateTime.Now"
            };
        }

        /// <summary>
        /// 高精度延迟（微秒级）
        /// </summary>
        /// <param name="microseconds">延迟微秒数</param>
        public static void DelayMicroseconds(int microseconds)
        {
            if (_frequency == 0 || microseconds <= 0)
                return;

            QueryPerformanceCounter(out long start);
            long targetTicks = start + (long)((double)microseconds * _frequency / 1_000_000);

            // 自旋等待以获得高精度
            while (true)
            {
                QueryPerformanceCounter(out long current);
                if (current >= targetTicks)
                    break;
                
                // 避免100%CPU占用，在最后100微秒才进入自旋
                if (targetTicks - current > _frequency / 10000) // 大于100微秒
                {
                    System.Threading.Thread.Sleep(0); // 让出CPU时间片
                }
            }
        }

        /// <summary>
        /// 测量代码执行时间（高精度）
        /// </summary>
        /// <param name="action">要测量的代码</param>
        /// <returns>执行时间（微秒）</returns>
        public static double MeasureExecutionTime(Action action)
        {
            if (_frequency == 0)
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                action();
                sw.Stop();
                return sw.Elapsed.TotalMicroseconds;
            }

            QueryPerformanceCounter(out long start);
            action();
            QueryPerformanceCounter(out long end);

            return (double)(end - start) * 1_000_000 / _frequency;
        }
    }

    /// <summary>
    /// 时钟精度信息
    /// </summary>
    public class ClockPrecisionInfo
    {
        /// <summary>
        /// 是否支持高精度时钟
        /// </summary>
        public bool IsHighPrecisionSupported { get; set; }

        /// <summary>
        /// 是否可用高精度API
        /// </summary>
        public bool IsHighPrecisionApiAvailable { get; set; }

        /// <summary>
        /// 性能计数器频率
        /// </summary>
        public long Frequency { get; set; }

        /// <summary>
        /// 时钟分辨率（微秒）
        /// </summary>
        public double ResolutionMicroseconds { get; set; }

        /// <summary>
        /// 使用的API
        /// </summary>
        public string ApiUsed { get; set; } = string.Empty;

        /// <summary>
        /// 获取精度描述
        /// </summary>
        public string GetPrecisionDescription()
        {
            if (ResolutionMicroseconds < 1)
                return $"超高精度 (~{ResolutionMicroseconds:F3} 微秒)";
            else if (ResolutionMicroseconds < 100)
                return $"高精度 (~{ResolutionMicroseconds:F1} 微秒)";
            else if (ResolutionMicroseconds < 1000)
                return $"中等精度 (~{ResolutionMicroseconds:F0} 微秒)";
            else
                return $"标准精度 (~{ResolutionMicroseconds / 1000:F1} 毫秒)";
        }

        public override string ToString()
        {
            return $"时钟精度: {GetPrecisionDescription()}, API: {ApiUsed}, 频率: {Frequency:N0} Hz";
        }
    }
}