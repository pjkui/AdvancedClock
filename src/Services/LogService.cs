using System;
using System.IO;
using Serilog;
using Serilog.Events;

namespace AdvancedClock.Services
{
    /// <summary>
    /// 日志服务 - 使用Serilog记录应用程序日志
    /// </summary>
    public class LogService
    {
        private static LogService? _instance;
        private static readonly object _lock = new object();
        private readonly ILogger _logger;

        /// <summary>
        /// 获取单例实例
        /// </summary>
        public static LogService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new LogService();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 日志目录路径
        /// </summary>
        public string LogDirectory { get; }

        private LogService()
        {
            // 设置日志目录
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            LogDirectory = Path.Combine(appDataPath, "AdvancedClock", "logs");

            // 确保日志目录存在
            Directory.CreateDirectory(LogDirectory);

            // 配置Serilog
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                    Path.Combine(LogDirectory, "alarm-log-.txt"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    retainedFileCountLimit: 30,
                    encoding: System.Text.Encoding.UTF8
                )
                .CreateLogger();

            LogInfo("日志服务已初始化", $"日志目录: {LogDirectory}");
        }

        /// <summary>
        /// 记录信息日志
        /// </summary>
        public void LogInfo(string message, string? details = null)
        {
            if (string.IsNullOrEmpty(details))
            {
                _logger.Information(message);
            }
            else
            {
                _logger.Information("{Message} | {Details}", message, details);
            }
        }

        /// <summary>
        /// 记录警告日志
        /// </summary>
        public void LogWarning(string message, string? details = null)
        {
            if (string.IsNullOrEmpty(details))
            {
                _logger.Warning(message);
            }
            else
            {
                _logger.Warning("{Message} | {Details}", message, details);
            }
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        public void LogError(string message, string? errorDetails = null, Exception? exception = null)
        {
            if (exception != null)
            {
                _logger.Error(exception, "{Message} | {ErrorDetails}", message, errorDetails ?? string.Empty);
            }
            else if (!string.IsNullOrEmpty(errorDetails))
            {
                _logger.Error("{Message} | {ErrorDetails}", message, errorDetails);
            }
            else
            {
                _logger.Error(message);
            }
        }

        /// <summary>
        /// 记录闹钟触发日志
        /// </summary>
        public void LogAlarmTriggered(string alarmName, DateTime triggerTime)
        {
            LogInfo($"闹钟触发", $"名称: {alarmName}, 触发时间: {triggerTime:yyyy-MM-dd HH:mm:ss}");
        }

        /// <summary>
        /// 记录动作执行日志
        /// </summary>
        public void LogActionExecution(string alarmName, string actionType, string actionParameter, bool success, string message, string? errorDetails = null, TimeSpan? duration = null)
        {
            var durationText = duration.HasValue ? $", 耗时: {duration.Value.TotalSeconds:F2}秒" : "";
            var statusText = success ? "成功" : "失败";
            
            if (success)
            {
                LogInfo(
                    $"动作执行{statusText}",
                    $"闹钟: {alarmName}, 动作: {actionType}, 参数: {actionParameter}, 结果: {message}{durationText}"
                );
            }
            else
            {
                LogError(
                    $"动作执行{statusText}",
                    $"闹钟: {alarmName}, 动作: {actionType}, 参数: {actionParameter}, 结果: {message}{durationText}, 错误: {errorDetails}"
                );
            }
        }

        /// <summary>
        /// 关闭日志服务
        /// </summary>
        public void Close()
        {
            Log.CloseAndFlush();
        }
    }
}
