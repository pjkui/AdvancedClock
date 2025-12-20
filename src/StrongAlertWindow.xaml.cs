using System;
using System.Windows;
using System.Windows.Threading;
using System.Media;

namespace AdvancedClock
{
    /// <summary>
    /// 强提醒窗口 - 全屏遮罩
    /// </summary>
    public partial class StrongAlertWindow : Window
    {
        private readonly DispatcherTimer _timer;
        private readonly AlarmModel _alarm;

        public StrongAlertWindow(AlarmModel alarm)
        {
            InitializeComponent();

            _alarm = alarm;

            // 设置闹钟信息
            AlarmNameText.Text = alarm.Name;
            AlarmMessageText.Text = alarm.Message;
            CurrentTimeText.Text = DateTime.Now.ToString("HH:mm:ss");

            // 创建定时器更新时间显示
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();

            // 播放闹钟声音（自定义或系统默认，循环播放指定时长）
            AudioService.Instance.PlayAlarmSound(alarm.CustomSoundPath, alarm.IsStrongAlert, alarm.MaxPlayDurationSeconds);

            // 确保窗口在最前面
            this.Topmost = true;
            this.Focus();
        }

        /// <summary>
        /// 定时器更新时间显示
        /// </summary>
        private void Timer_Tick(object? sender, EventArgs e)
        {
            CurrentTimeText.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 关闭按钮点击
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// 窗口关闭时清理资源
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _timer?.Stop();
        }
    }
}
