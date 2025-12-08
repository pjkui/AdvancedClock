using System;
using System.ComponentModel;

namespace AdvancedClock
{
    /// <summary>
    /// 闹钟循环模式枚举
    /// </summary>
    public enum AlarmRepeatMode
    {
        /// <summary>
        /// 不循环（一次性）
        /// </summary>
        [Description("不循环")]
        None,
        
        /// <summary>
        /// 按天循环
        /// </summary>
        [Description("每天")]
        Daily,
        
        /// <summary>
        /// 按月循环
        /// </summary>
        [Description("每月")]
        Monthly,
        
        /// <summary>
        /// 按年循环
        /// </summary>
        [Description("每年")]
        Yearly
    }

    /// <summary>
    /// 闹钟数据模型
    /// </summary>
    public class AlarmModel : INotifyPropertyChanged
    {
        private Guid _id;
        private string _name;
        private DateTime _alarmTime;
        private AlarmRepeatMode _repeatMode;
        private bool _isEnabled;
        private string _message;
        private bool _isStrongAlert;
        private bool _enableAdvanceReminder;
        private int _advanceMinutes;
        private int _repeatIntervalMinutes;

        public event PropertyChangedEventHandler? PropertyChanged;

        public AlarmModel()
        {
            _id = Guid.NewGuid();
            _name = "新闹钟";
            _alarmTime = DateTime.Now.AddMinutes(5);
            _repeatMode = AlarmRepeatMode.None;
            _isEnabled = true;
            _message = "闹钟时间到！";
            _isStrongAlert = false;
            _enableAdvanceReminder = false;
            _advanceMinutes = 5;
            _repeatIntervalMinutes = 1;
        }

        /// <summary>
        /// 闹钟唯一标识
        /// </summary>
        public Guid Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        /// <summary>
        /// 闹钟名称
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// 闹钟时间
        /// </summary>
        public DateTime AlarmTime
        {
            get => _alarmTime;
            set
            {
                _alarmTime = value;
                OnPropertyChanged(nameof(AlarmTime));
                OnPropertyChanged(nameof(DisplayTime));
                OnPropertyChanged(nameof(AdvanceStartTimeText));
            }
        }

        /// <summary>
        /// 循环模式
        /// </summary>
        public AlarmRepeatMode RepeatMode
        {
            get => _repeatMode;
            set
            {
                _repeatMode = value;
                OnPropertyChanged(nameof(RepeatMode));
                OnPropertyChanged(nameof(RepeatModeText));
            }
        }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        /// <summary>
        /// 闹钟消息
        /// </summary>
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        /// <summary>
        /// 是否为强提醒（全屏遮罩）
        /// </summary>
        public bool IsStrongAlert
        {
            get => _isStrongAlert;
            set
            {
                _isStrongAlert = value;
                OnPropertyChanged(nameof(IsStrongAlert));
            }
        }

        /// <summary>
        /// 是否启用提前提醒
        /// </summary>
        public bool EnableAdvanceReminder
        {
            get => _enableAdvanceReminder;
            set
            {
                _enableAdvanceReminder = value;
                OnPropertyChanged(nameof(EnableAdvanceReminder));
                OnPropertyChanged(nameof(AdvanceReminderText));
                OnPropertyChanged(nameof(AdvanceStartTimeText));
            }
        }

        /// <summary>
        /// 提前提醒分钟数（在目标时间前X分钟开始提醒）
        /// </summary>
        public int AdvanceMinutes
        {
            get => _advanceMinutes;
            set
            {
                _advanceMinutes = Math.Max(1, Math.Min(60, value)); // 限制在1-60分钟之间
                OnPropertyChanged(nameof(AdvanceMinutes));
                OnPropertyChanged(nameof(AdvanceReminderText));
                OnPropertyChanged(nameof(AdvanceStartTimeText));
            }
        }

        /// <summary>
        /// 重复提醒间隔分钟数（每Y分钟重复提醒一次）
        /// </summary>
        public int RepeatIntervalMinutes
        {
            get => _repeatIntervalMinutes;
            set
            {
                _repeatIntervalMinutes = Math.Max(1, Math.Min(10, value)); // 限制在1-10分钟之间
                OnPropertyChanged(nameof(RepeatIntervalMinutes));
                OnPropertyChanged(nameof(AdvanceReminderText));
            }
        }

        /// <summary>
        /// 显示时间（用于UI）
        /// </summary>
        public string DisplayTime => _alarmTime.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// 循环模式文本（用于UI）
        /// </summary>
        public string RepeatModeText
        {
            get
            {
                return _repeatMode switch
                {
                    AlarmRepeatMode.None => "不循环",
                    AlarmRepeatMode.Daily => "每天",
                    AlarmRepeatMode.Monthly => "每月",
                    AlarmRepeatMode.Yearly => "每年",
                    _ => "未知"
                };
            }
        }

        /// <summary>
        /// 提前提醒显示文本（用于UI）
        /// </summary>
        public string AdvanceReminderText
        {
            get
            {
                if (!_enableAdvanceReminder)
                    return "未启用";

                return $"提前{_advanceMinutes}分钟\n每{_repeatIntervalMinutes}分钟重复";
            }
        }

        /// <summary>
        /// 提前提醒开始时间显示文本（用于UI）
        /// </summary>
        public string AdvanceStartTimeText
        {
            get
            {
                if (!_enableAdvanceReminder)
                    return "-";

                var startTime = GetAdvanceReminderStartTime();
                return startTime?.ToString("HH:mm:ss") ?? "-";
            }
        }

        /// <summary>
        /// 计算下一次闹钟时间
        /// </summary>
        /// <returns>下一次闹钟时间</returns>
        public DateTime GetNextAlarmTime()
        {
            DateTime now = DateTime.Now;
            DateTime nextTime = _alarmTime;

            switch (_repeatMode)
            {
                case AlarmRepeatMode.None:
                    // 不循环，如果时间已过，返回原时间（将被禁用）
                    return nextTime;

                case AlarmRepeatMode.Daily:
                    // 按天循环
                    while (nextTime <= now)
                    {
                        nextTime = nextTime.AddDays(1);
                    }
                    break;

                case AlarmRepeatMode.Monthly:
                    // 按月循环
                    while (nextTime <= now)
                    {
                        nextTime = nextTime.AddMonths(1);
                    }
                    break;

                case AlarmRepeatMode.Yearly:
                    // 按年循环
                    while (nextTime <= now)
                    {
                        nextTime = nextTime.AddYears(1);
                    }
                    break;
            }

            return nextTime;
        }

        /// <summary>
        /// 获取提前提醒开始时间
        /// </summary>
        /// <returns>提前提醒开始时间，如果未启用提前提醒则返回null</returns>
        public DateTime? GetAdvanceReminderStartTime()
        {
            if (!_enableAdvanceReminder)
                return null;

            return _alarmTime.AddMinutes(-_advanceMinutes);
        }

        /// <summary>
        /// 检查指定时间是否应该触发提前提醒
        /// </summary>
        /// <param name="currentTime">当前时间</param>
        /// <returns>是否应该触发提前提醒</returns>
        public bool ShouldTriggerAdvanceReminder(DateTime currentTime)
        {
            if (!_enableAdvanceReminder)
                return false;

            var startTime = GetAdvanceReminderStartTime();
            if (!startTime.HasValue)
                return false;

            // 检查是否在提前提醒时间范围内
            if (currentTime < startTime.Value || currentTime >= _alarmTime)
                return false;

            // 计算从开始时间到现在经过的总毫秒数（高精度）
            var elapsedMilliseconds = (currentTime - startTime.Value).TotalMilliseconds;
            var intervalMilliseconds = _repeatIntervalMinutes * 60.0 * 1000.0;
            
            // 检查是否到了下一个提醒间隔点（允许100毫秒的误差，因为检查频率更高）
            var remainder = elapsedMilliseconds % intervalMilliseconds;
            bool shouldTrigger = remainder < 100.0;
            
            // 调试输出
            if (shouldTrigger)
            {
                System.Diagnostics.Debug.WriteLine($"提前提醒检查 - {_name}: 开始时间={startTime:HH:mm:ss.fff}, 当前时间={currentTime:HH:mm:ss.fff}, 经过毫秒={elapsedMilliseconds:F1}, 间隔毫秒={intervalMilliseconds}, 余数={remainder:F1}");
            }
            
            return shouldTrigger;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
