using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AdvancedClock
{
    /// <summary>
    /// 闹钟诊断工具
    /// </summary>
    public static class AlarmDiagnostic
    {
        /// <summary>
        /// 诊断闹钟不触发的问题
        /// </summary>
        /// <param name="alarms">闹钟集合</param>
        public static void DiagnoseAlarms(ObservableCollection<AlarmModel> alarms)
        {
            Console.WriteLine("=== 闹钟诊断报告 ===");
            Console.WriteLine($"诊断时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
            Console.WriteLine($"高精度时间: {HighPrecisionClock.Now:yyyy-MM-dd HH:mm:ss.fff}");
            
            var precisionInfo = HighPrecisionClock.GetPrecisionInfo();
            Console.WriteLine($"时钟精度: {precisionInfo}");
            Console.WriteLine();

            Console.WriteLine($"总闹钟数量: {alarms.Count}");
            Console.WriteLine($"启用的闹钟: {alarms.Count(a => a.IsEnabled)}");
            Console.WriteLine();

            foreach (var alarm in alarms)
            {
                DiagnoseSingleAlarm(alarm);
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 诊断单个闹钟
        /// </summary>
        /// <param name="alarm">闹钟对象</param>
        public static void DiagnoseSingleAlarm(AlarmModel alarm)
        {
            Console.WriteLine($"闹钟: {alarm.Name} (ID: {alarm.Id})");
            Console.WriteLine($"  状态: {(alarm.IsEnabled ? "启用" : "禁用")}");
            Console.WriteLine($"  设定时间: {alarm.AlarmTime:yyyy-MM-dd HH:mm:ss.fff}");
            Console.WriteLine($"  循环模式: {alarm.RepeatModeText}");
            Console.WriteLine($"  消息: {alarm.Message}");
            Console.WriteLine($"  强提醒: {alarm.IsStrongAlert}");

            var now = HighPrecisionClock.Now;
            var timeDiff = alarm.AlarmTime - now;
            
            Console.WriteLine($"  当前时间: {now:yyyy-MM-dd HH:mm:ss.fff}");
            Console.WriteLine($"  时间差: {timeDiff.TotalMilliseconds:F1} 毫秒");

            // 检查时间状态
            if (timeDiff.TotalMilliseconds < -1000)
            {
                Console.WriteLine("  ⚠️  警告: 闹钟时间已过期超过1秒");
                
                if (alarm.RepeatMode == AlarmRepeatMode.None)
                {
                    Console.WriteLine("  ❌ 问题: 一次性闹钟时间已过，应该被禁用");
                }
                else
                {
                    var nextTime = alarm.GetNextAlarmTime();
                    Console.WriteLine($"  📅 下次闹钟时间应为: {nextTime:yyyy-MM-dd HH:mm:ss.fff}");
                    Console.WriteLine($"  ❌ 问题: 循环闹钟时间未更新到下次时间");
                }
            }
            else if (Math.Abs(timeDiff.TotalMilliseconds) < 100)
            {
                Console.WriteLine("  🔥 状态: 闹钟应该正在触发！");
            }
            else if (timeDiff.TotalMilliseconds > 0)
            {
                Console.WriteLine($"  ⏰ 状态: 闹钟将在 {timeDiff.TotalSeconds:F1} 秒后触发");
            }

            // 检查提前提醒
            if (alarm.EnableAdvanceReminder)
            {
                Console.WriteLine($"  提前提醒: 启用 (提前{alarm.AdvanceMinutes}分钟，每{alarm.RepeatIntervalMinutes}分钟重复)");
                
                var advanceStartTime = alarm.GetAdvanceReminderStartTime();
                if (advanceStartTime.HasValue)
                {
                    var advanceDiff = advanceStartTime.Value - now;
                    Console.WriteLine($"  提醒开始时间: {advanceStartTime.Value:yyyy-MM-dd HH:mm:ss.fff}");
                    Console.WriteLine($"  提醒时间差: {advanceDiff.TotalMilliseconds:F1} 毫秒");
                    
                    if (advanceDiff.TotalMilliseconds < 0 && timeDiff.TotalMilliseconds > 0)
                    {
                        Console.WriteLine("  🔔 状态: 应该正在提前提醒！");
                        
                        // 检查提醒触发逻辑
                        bool shouldTrigger = alarm.ShouldTriggerAdvanceReminder(now);
                        Console.WriteLine($"  提醒触发检查: {shouldTrigger}");
                    }
                }
            }
            else
            {
                Console.WriteLine("  提前提醒: 未启用");
            }
        }

        /// <summary>
        /// 测试闹钟触发逻辑
        /// </summary>
        /// <param name="alarm">闹钟对象</param>
        public static void TestAlarmTriggerLogic(AlarmModel alarm)
        {
            Console.WriteLine($"=== 测试闹钟触发逻辑: {alarm.Name} ===");
            
            var now = HighPrecisionClock.Now;
            var timeDiff = alarm.AlarmTime - now;
            
            Console.WriteLine($"当前时间: {now:yyyy-MM-dd HH:mm:ss.fff}");
            Console.WriteLine($"闹钟时间: {alarm.AlarmTime:yyyy-MM-dd HH:mm:ss.fff}");
            Console.WriteLine($"时间差: {timeDiff.TotalMilliseconds:F3} 毫秒");
            
            // 模拟 AlarmService 的触发条件
            bool shouldTriggerMain = Math.Abs(timeDiff.TotalMilliseconds) < 100;
            Console.WriteLine($"主闹钟触发条件 (误差<100ms): {shouldTriggerMain}");
            
            if (alarm.EnableAdvanceReminder)
            {
                bool shouldTriggerAdvance = alarm.ShouldTriggerAdvanceReminder(now);
                Console.WriteLine($"提前提醒触发条件: {shouldTriggerAdvance}");
                
                var advanceStartTime = alarm.GetAdvanceReminderStartTime();
                if (advanceStartTime.HasValue)
                {
                    var advanceDiff = advanceStartTime.Value - now;
                    Console.WriteLine($"提醒开始时间: {advanceStartTime.Value:yyyy-MM-dd HH:mm:ss.fff}");
                    Console.WriteLine($"提醒时间差: {advanceDiff.TotalMilliseconds:F3} 毫秒");
                    
                    if (now >= advanceStartTime.Value && now < alarm.AlarmTime)
                    {
                        var elapsedMs = (now - advanceStartTime.Value).TotalMilliseconds;
                        var intervalMs = alarm.RepeatIntervalMinutes * 60.0 * 1000.0;
                        var remainder = elapsedMs % intervalMs;
                        
                        Console.WriteLine($"提醒区间内 - 经过: {elapsedMs:F1}ms, 间隔: {intervalMs:F1}ms, 余数: {remainder:F1}ms");
                        Console.WriteLine($"余数检查 (<100ms): {remainder < 100.0}");
                    }
                }
            }
        }

        /// <summary>
        /// 监控闹钟服务状态
        /// </summary>
        /// <param name="alarmService">闹钟服务</param>
        /// <param name="durationSeconds">监控持续时间（秒）</param>
        public static void MonitorAlarmService(AlarmService alarmService, int durationSeconds = 10)
        {
            Console.WriteLine($"=== 监控闹钟服务 ({durationSeconds}秒) ===");
            
            var startTime = DateTime.Now;
            var endTime = startTime.AddSeconds(durationSeconds);
            
            int checkCount = 0;
            
            while (DateTime.Now < endTime)
            {
                checkCount++;
                var now = HighPrecisionClock.Now;
                
                Console.WriteLine($"[{checkCount:D3}] {now:HH:mm:ss.fff} - 服务运行中...");
                
                System.Threading.Thread.Sleep(1000); // 每秒检查一次
            }
            
            Console.WriteLine($"监控完成，共检查 {checkCount} 次");
        }
    }
}