using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using System.Windows;

namespace AdvancedClock
{
    /// <summary>
    /// 闹钟管理服务
    /// </summary>
    public class AlarmService
    {
        private readonly DispatcherTimer _timer;
        private readonly ObservableCollection<AlarmModel> _alarms;
        private readonly Dictionary<Guid, DateTime> _lastAdvanceReminderTimes;

        public event EventHandler<AlarmModel>? AlarmTriggered;
        public event EventHandler<(AlarmModel Alarm, bool IsAdvanceReminder)>? AlarmReminderTriggered;

        public AlarmService(ObservableCollection<AlarmModel> alarms)
        {
            _alarms = alarms;
            _lastAdvanceReminderTimes = new Dictionary<Guid, DateTime>();
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000.0 / 30.0) // 每秒检查30次 (~33.33ms间隔)
            };
            _timer.Tick += Timer_Tick;
        }

        /// <summary>
        /// 启动闹钟服务
        /// </summary>
        public void Start()
        {
            _timer.Start();
        }

        /// <summary>
        /// 停止闹钟服务
        /// </summary>
        public void Stop()
        {
            _timer.Stop();
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
        }

        /// <summary>
        /// 定时器触发事件
        /// </summary>
        private void Timer_Tick(object? sender, EventArgs e)
        {
            // 使用高精度时钟
            DateTime now = HighPrecisionClock.Now;

            // 检查所有启用的闹钟
            foreach (var alarm in _alarms.Where(a => a.IsEnabled).ToList())
            {
                // 检查提前提醒
                if (alarm.EnableAdvanceReminder && alarm.ShouldTriggerAdvanceReminder(now))
                {
                    // 检查是否需要防重复触发
                    if (ShouldTriggerAdvanceReminderNow(alarm, now))
                    {
                        // 调试输出
                        System.Diagnostics.Debug.WriteLine($"触发提前提醒: {alarm.Name} at {now:HH:mm:ss.fff}");
                        
                        // 触发提前提醒
                        OnAlarmReminderTriggered(alarm, true);
                        
                        // 记录触发时间
                        _lastAdvanceReminderTimes[alarm.Id] = now;
                    }
                }

                // 检查是否到达闹钟时间（高精度匹配，允许100毫秒误差，因为检查频率更高）
                if (Math.Abs((alarm.AlarmTime - now).TotalMilliseconds) < 100)
                {
                    // 调试输出
                    System.Diagnostics.Debug.WriteLine($"触发正式闹钟: {alarm.Name} at {now:HH:mm:ss.fff}");
                    
                    // 触发正式闹钟
                    OnAlarmTriggered(alarm);
                    OnAlarmReminderTriggered(alarm, false);

                    // 清除提前提醒记录
                    _lastAdvanceReminderTimes.Remove(alarm.Id);

                    // 根据循环模式处理
                    if (alarm.RepeatMode == AlarmRepeatMode.None)
                    {
                        // 不循环，禁用闹钟
                        alarm.IsEnabled = false;
                    }
                    else
                    {
                        // 循环模式，计算下一次闹钟时间
                        alarm.AlarmTime = alarm.GetNextAlarmTime();
                    }
                }
            }
        }

        /// <summary>
        /// 检查是否应该现在触发提前提醒（防重复触发）
        /// </summary>
        private bool ShouldTriggerAdvanceReminderNow(AlarmModel alarm, DateTime now)
        {
            if (!_lastAdvanceReminderTimes.TryGetValue(alarm.Id, out DateTime lastTriggerTime))
            {
                // 第一次触发
                return true;
            }

            // 检查距离上次触发是否已经超过间隔时间（使用高精度比较）
            var elapsedSinceLastTrigger = (now - lastTriggerTime).TotalMilliseconds;
            var intervalMilliseconds = alarm.RepeatIntervalMinutes * 60.0 * 1000.0;
            
            // 允许50毫秒的误差（因为检查频率更高，可以更精确）
            return elapsedSinceLastTrigger >= (intervalMilliseconds - 50);
        }

        /// <summary>
        /// 触发闹钟事件
        /// </summary>
        private void OnAlarmTriggered(AlarmModel alarm)
        {
            AlarmTriggered?.Invoke(this, alarm);
        }

        /// <summary>
        /// 触发闹钟提醒事件
        /// </summary>
        /// <param name="alarm">闹钟对象</param>
        /// <param name="isAdvanceReminder">是否为提前提醒</param>
        private void OnAlarmReminderTriggered(AlarmModel alarm, bool isAdvanceReminder)
        {
            AlarmReminderTriggered?.Invoke(this, (alarm, isAdvanceReminder));
        }
    }
}
