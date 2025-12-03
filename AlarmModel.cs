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

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
