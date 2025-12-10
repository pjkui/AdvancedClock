using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using System.Windows;

namespace AdvancedClock
{
    /// <summary>
    /// 增强的闹钟管理服务，解决触发问题
    /// </summary>
    public class EnhancedAlarmService
    {
        private readonly DispatcherTimer _timer;
        private readonly ObservableCollection<AlarmModel> _alarms;
        private readonly Dictionary<Guid, DateTime> _lastAdvanceReminderTimes;
        private readonly Dictionary<Guid, DateTime> _lastMainAlarmTriggerTimes;
        private readonly HashSet<Guid> _triggeredMainAlarms; // 防止重复触发主闹钟

        public event EventHandler<AlarmModel>? AlarmTriggered;
        public event EventHandler<(AlarmModel Alarm, bool IsAdvanceReminder)>? AlarmReminderTriggered;

        public EnhancedAlarmService(ObservableCollection<AlarmModel> alarms)
        {
            _alarms = alarms;
            _lastAdvanceReminderTimes = new Dictionary<Guid, DateTime>();
            _lastMainAlarmTriggerTimes = new Dictionary<Guid, DateTime>();
            _triggeredMainAlarms = new HashSet<Guid>();
            
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100) // 每100毫秒检查一次，更频繁但不会过度消耗CPU
            };
            _timer.Tick += Timer_Tick;
        }

        /// <summary>
        /// 启动闹钟服务
        /// </summary>
        public void Start()
        {
            _timer.Start();
            System.Diagnostics.Debug.WriteLine("增强闹钟服务已启动");
        }

        /// <summary>
        /// 停止闹钟服务
        /// </summary>
        public void Stop()
        {
            _timer.Stop();
            System.Diagnostics.Debug.WriteLine("增强闹钟服务已停止");
        }

        /// <summary>
        /// 添加闹钟
        /// </summary>
        public void AddAlarm(AlarmModel alarm)
        {
            _alarms.Add(alarm);
        }

        /// <summary>
        /// 删除闹钟
        /// </summary>
        public void RemoveAlarm(AlarmModel alarm)
        {
            _alarms.Remove(alarm);
            // 清理相关记录
            _lastAdvanceReminderTimes.Remove(alarm.Id);
            _lastMainAlarmTriggerTimes.Remove(alarm.Id);
            _triggeredMainAlarms.Remove(alarm.Id);
        }

        /// <summary>
        /// 定时器触发事件
        /// </summary>
        private void Timer_Tick(object? sender, EventArgs e)
        {
            try
            {
                // 使用高精度时钟
                DateTime now = HighPrecisionClock.Now;
                DateTime systemNow = DateTime.Now; // 也获取系统时间作为备用

                // 检查所有启用的闹钟
                foreach (var alarm in _alarms.Where(a => a.IsEnabled).ToList())
                {
                    CheckAlarm(alarm, now, systemNow);
                }

                // 清理过期的触发记录（避免内存泄漏）
                CleanupOldRecords(now);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"闹钟检查出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 检查单个闹钟
        /// </summary>
        private void CheckAlarm(AlarmModel alarm, DateTime now, DateTime systemNow)
        {
            // 使用两种时间进行检查，提高准确性
            var timeDiffHigh = (alarm.AlarmTime - now).TotalMilliseconds;
            var timeDiffSystem = (alarm.AlarmTime - systemNow).TotalMilliseconds;
            
            // 取较小的时间差（更接近触发时间）
            var timeDiff = Math.Min(Math.Abs(timeDiffHigh), Math.Abs(timeDiffSystem));
            var isTimeToTrigger = timeDiff < 200; // 放宽到200毫秒误差

            // 检查提前提醒
            if (alarm.EnableAdvanceReminder)
            {
                CheckAdvanceReminder(alarm, now);
            }

            // 检查主闹钟触发
            if (isTimeToTrigger && !_triggeredMainAlarms.Contains(alarm.Id))
            {
                // 防止在同一分钟内重复触发
                if (ShouldTriggerMainAlarm(alarm, now))
                {
                    TriggerMainAlarm(alarm, now);
                }
            }
            
            // 如果时间已经过去超过1分钟，清除触发标记
            if (timeDiffHigh < -60000) // 超过1分钟
            {
                _triggeredMainAlarms.Remove(alarm.Id);
            }
        }

        /// <summary>
        /// 检查提前提醒
        /// </summary>
        private void CheckAdvanceReminder(AlarmModel alarm, DateTime now)
        {
            if (alarm.ShouldTriggerAdvanceReminder(now))
            {
                if (ShouldTriggerAdvanceReminderNow(alarm, now))
                {
                    System.Diagnostics.Debug.WriteLine($"[{now:HH:mm:ss.fff}] 触发提前提醒: {alarm.Name}");
                    
                    OnAlarmReminderTriggered(alarm, true);
                    _lastAdvanceReminderTimes[alarm.Id] = now;
                }
            }
        }

        /// <summary>
        /// 触发主闹钟
        /// </summary>
        private void TriggerMainAlarm(AlarmModel alarm, DateTime now)
        {
            System.Diagnostics.Debug.WriteLine($"[{now:HH:mm:ss.fff}] 触发主闹钟: {alarm.Name}");
            
            // 标记为已触发，防止重复
            _triggeredMainAlarms.Add(alarm.Id);
            _lastMainAlarmTriggerTimes[alarm.Id] = now;
            
            // 触发事件
            OnAlarmTriggered(alarm);
            OnAlarmReminderTriggered(alarm, false);

            // 清除提前提醒记录
            _lastAdvanceReminderTimes.Remove(alarm.Id);

            // 处理循环模式
            HandleAlarmRepeat(alarm);
        }

        /// <summary>
        /// 处理闹钟循环
        /// </summary>
        private void HandleAlarmRepeat(AlarmModel alarm)
        {
            if (alarm.RepeatMode == AlarmRepeatMode.None)
            {
                // 不循环，禁用闹钟
                alarm.IsEnabled = false;
                System.Diagnostics.Debug.WriteLine($"一次性闹钟已禁用: {alarm.Name}");
            }
            else
            {
                // 循环模式，计算下一次闹钟时间
                var nextTime = alarm.GetNextAlarmTime();
                alarm.AlarmTime = nextTime;
                
                // 清除触发标记，允许下次触发
                _triggeredMainAlarms.Remove(alarm.Id);
                
                System.Diagnostics.Debug.WriteLine($"循环闹钟已更新到下次时间: {alarm.Name} -> {nextTime:yyyy-MM-dd HH:mm:ss}");
            }
        }

        /// <summary>
        /// 检查是否应该现在触发主闹钟
        /// </summary>
        private bool ShouldTriggerMainAlarm(AlarmModel alarm, DateTime now)
        {
            // 如果从未触发过，可以触发
            if (!_lastMainAlarmTriggerTimes.TryGetValue(alarm.Id, out DateTime lastTriggerTime))
            {
                return true;
            }

            // 检查距离上次触发是否超过30秒（防止短时间内重复触发）
            var elapsedSinceLastTrigger = (now - lastTriggerTime).TotalSeconds;
            return elapsedSinceLastTrigger >= 30;
        }

        /// <summary>
        /// 检查是否应该现在触发提前提醒
        /// </summary>
        private bool ShouldTriggerAdvanceReminderNow(AlarmModel alarm, DateTime now)
        {
            if (!_lastAdvanceReminderTimes.TryGetValue(alarm.Id, out DateTime lastTriggerTime))
            {
                return true;
            }

            var elapsedSinceLastTrigger = (now - lastTriggerTime).TotalMilliseconds;
            var intervalMilliseconds = alarm.RepeatIntervalMinutes * 60.0 * 1000.0;
            
            return elapsedSinceLastTrigger >= (intervalMilliseconds - 100);
        }

        /// <summary>
        /// 清理过期记录
        /// </summary>
        private void CleanupOldRecords(DateTime now)
        {
            var cutoffTime = now.AddHours(-1); // 清理1小时前的记录
            
            var expiredAdvanceKeys = _lastAdvanceReminderTimes
                .Where(kvp => kvp.Value < cutoffTime)
                .Select(kvp => kvp.Key)
                .ToList();
                
            var expiredMainKeys = _lastMainAlarmTriggerTimes
                .Where(kvp => kvp.Value < cutoffTime)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredAdvanceKeys)
            {
                _lastAdvanceReminderTimes.Remove(key);
            }
            
            foreach (var key in expiredMainKeys)
            {
                _lastMainAlarmTriggerTimes.Remove(key);
            }
        }

        /// <summary>
        /// 触发闹钟事件
        /// </summary>
        private void OnAlarmTriggered(AlarmModel alarm)
        {
            try
            {
                AlarmTriggered?.Invoke(this, alarm);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"触发闹钟事件出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 触发闹钟提醒事件
        /// </summary>
        private void OnAlarmReminderTriggered(AlarmModel alarm, bool isAdvanceReminder)
        {
            try
            {
                AlarmReminderTriggered?.Invoke(this, (alarm, isAdvanceReminder));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"触发闹钟提醒事件出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取服务状态信息
        /// </summary>
        public string GetServiceStatus()
        {
            var status = new System.Text.StringBuilder();
            status.AppendLine($"服务状态: {(_timer.IsEnabled ? "运行中" : "已停止")}");
            status.AppendLine($"检查间隔: {_timer.Interval.TotalMilliseconds} 毫秒");
            status.AppendLine($"监控闹钟数: {_alarms.Count(a => a.IsEnabled)}");
            status.AppendLine($"提前提醒记录: {_lastAdvanceReminderTimes.Count}");
            status.AppendLine($"主闹钟记录: {_lastMainAlarmTriggerTimes.Count}");
            status.AppendLine($"已触发标记: {_triggeredMainAlarms.Count}");
            
            return status.ToString();
        }
    }
}