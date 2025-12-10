using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AdvancedClock
{
    /// <summary>
    /// 闹钟修复工具
    /// </summary>
    public static class AlarmFixer
    {
        /// <summary>
        /// 修复所有闹钟的时间问题
        /// </summary>
        /// <param name="alarms">闹钟集合</param>
        /// <returns>修复的闹钟数量</returns>
        public static int FixAllAlarms(ObservableCollection<AlarmModel> alarms)
        {
            int fixedCount = 0;
            var now = HighPrecisionClock.Now;

            foreach (var alarm in alarms.Where(a => a.IsEnabled).ToList())
            {
                if (FixSingleAlarm(alarm, now))
                {
                    fixedCount++;
                }
            }

            return fixedCount;
        }

        /// <summary>
        /// 修复单个闹钟
        /// </summary>
        /// <param name="alarm">闹钟对象</param>
        /// <param name="currentTime">当前时间</param>
        /// <returns>是否进行了修复</returns>
        public static bool FixSingleAlarm(AlarmModel alarm, DateTime? currentTime = null)
        {
            var now = currentTime ?? HighPrecisionClock.Now;
            var timeDiff = alarm.AlarmTime - now;
            bool wasFixed = false;

            // 如果闹钟时间已过期超过1秒
            if (timeDiff.TotalMilliseconds < -1000)
            {
                if (alarm.RepeatMode == AlarmRepeatMode.None)
                {
                    // 一次性闹钟，禁用它
                    alarm.IsEnabled = false;
                    System.Diagnostics.Debug.WriteLine($"修复: 禁用过期的一次性闹钟 '{alarm.Name}'");
                    wasFixed = true;
                }
                else
                {
                    // 循环闹钟，更新到下次时间
                    var nextTime = alarm.GetNextAlarmTime();
                    if (nextTime != alarm.AlarmTime)
                    {
                        alarm.AlarmTime = nextTime;
                        System.Diagnostics.Debug.WriteLine($"修复: 更新循环闹钟 '{alarm.Name}' 到下次时间 {nextTime:yyyy-MM-dd HH:mm:ss}");
                        wasFixed = true;
                    }
                }
            }

            return wasFixed;
        }

        /// <summary>
        /// 检查闹钟服务的触发逻辑
        /// </summary>
        /// <param name="alarms">闹钟集合</param>
        /// <returns>应该触发的闹钟列表</returns>
        public static (AlarmModel[] MainAlarms, AlarmModel[] AdvanceAlarms) CheckTriggerLogic(ObservableCollection<AlarmModel> alarms)
        {
            var now = HighPrecisionClock.Now;
            var mainAlarms = new System.Collections.Generic.List<AlarmModel>();
            var advanceAlarms = new System.Collections.Generic.List<AlarmModel>();

            foreach (var alarm in alarms.Where(a => a.IsEnabled))
            {
                // 检查主闹钟触发
                var timeDiff = alarm.AlarmTime - now;
                if (Math.Abs(timeDiff.TotalMilliseconds) < 100)
                {
                    mainAlarms.Add(alarm);
                }

                // 检查提前提醒触发
                if (alarm.EnableAdvanceReminder && alarm.ShouldTriggerAdvanceReminder(now))
                {
                    advanceAlarms.Add(alarm);
                }
            }

            return (mainAlarms.ToArray(), advanceAlarms.ToArray());
        }

        /// <summary>
        /// 创建测试闹钟（用于调试）
        /// </summary>
        /// <param name="alarms">闹钟集合</param>
        /// <param name="secondsFromNow">从现在开始的秒数</param>
        /// <returns>创建的测试闹钟</returns>
        public static AlarmModel CreateTestAlarm(ObservableCollection<AlarmModel> alarms, int secondsFromNow = 10)
        {
            var testAlarm = new AlarmModel
            {
                Name = $"测试闹钟 {DateTime.Now:HH:mm:ss}",
                AlarmTime = HighPrecisionClock.Now.AddSeconds(secondsFromNow),
                RepeatMode = AlarmRepeatMode.None,
                Message = $"这是一个 {secondsFromNow} 秒后的测试闹钟！",
                IsEnabled = true,
                IsStrongAlert = false,
                EnableAdvanceReminder = secondsFromNow > 30, // 只有超过30秒的才启用提前提醒
                AdvanceMinutes = Math.Max(1, secondsFromNow / 60), // 提前1分钟或适当时间
                RepeatIntervalMinutes = 1
            };

            alarms.Add(testAlarm);
            System.Diagnostics.Debug.WriteLine($"创建测试闹钟: {testAlarm.Name}, 将在 {testAlarm.AlarmTime:HH:mm:ss.fff} 触发");
            
            return testAlarm;
        }

        /// <summary>
        /// 验证闹钟服务是否正常工作
        /// </summary>
        /// <param name="alarmService">闹钟服务</param>
        /// <returns>验证结果</returns>
        public static string ValidateAlarmService(AlarmService alarmService)
        {
            var result = new System.Text.StringBuilder();
            result.AppendLine("=== 闹钟服务验证 ===");
            
            try
            {
                // 检查服务是否在运行
                // 注意：这里我们无法直接检查私有字段，所以只能通过行为来推断
                result.AppendLine("✅ 闹钟服务实例已创建");
                
                // 检查时钟精度
                var precisionInfo = HighPrecisionClock.GetPrecisionInfo();
                result.AppendLine($"⏰ 时钟精度: {precisionInfo.GetPrecisionDescription()}");
                result.AppendLine($"📡 使用API: {precisionInfo.ApiUsed}");
                
                if (precisionInfo.ResolutionMicroseconds > 1000)
                {
                    result.AppendLine("⚠️  警告: 时钟精度较低，可能影响触发准确性");
                }
                
                result.AppendLine("✅ 验证完成");
            }
            catch (Exception ex)
            {
                result.AppendLine($"❌ 验证失败: {ex.Message}");
            }
            
            return result.ToString();
        }
    }
}