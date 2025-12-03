using System;
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

        public event EventHandler<AlarmModel>? AlarmTriggered;

        public AlarmService(ObservableCollection<AlarmModel> alarms)
        {
            _alarms = alarms;
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // 每秒检查一次
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
            DateTime now = DateTime.Now;

            // 检查所有启用的闹钟
            foreach (var alarm in _alarms.Where(a => a.IsEnabled).ToList())
            {
                // 检查是否到达闹钟时间（精确到秒）
                if (Math.Abs((alarm.AlarmTime - now).TotalSeconds) < 1)
                {
                    // 触发闹钟
                    OnAlarmTriggered(alarm);

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
        /// 触发闹钟事件
        /// </summary>
        private void OnAlarmTriggered(AlarmModel alarm)
        {
            AlarmTriggered?.Invoke(this, alarm);
        }
    }
}
