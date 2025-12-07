using System;
using System.Windows;

namespace AdvancedClock
{
    /// <summary>
    /// 闹钟编辑对话框
    /// </summary>
    public partial class AlarmEditDialog : Window
    {
        public AlarmModel AlarmModel { get; private set; }
        private readonly bool _isEditMode;

        /// <summary>
        /// 构造函数（新建模式）
        /// </summary>
        public AlarmEditDialog()
        {
            InitializeComponent();
            _isEditMode = false;
            AlarmModel = new AlarmModel();
            DataContext = AlarmModel;
            Title = "添加闹钟";
            
            // 初始化时间输入框
            InitializeTimeInputs();
        }

        /// <summary>
        /// 构造函数（编辑模式）
        /// </summary>
        public AlarmEditDialog(AlarmModel alarm)
        {
            InitializeComponent();
            _isEditMode = true;
            AlarmModel = alarm;
            DataContext = AlarmModel;
            Title = "编辑闹钟";
            
            // 初始化时间输入框
            InitializeTimeInputs();
        }

        /// <summary>
        /// 初始化时间输入框
        /// </summary>
        private void InitializeTimeInputs()
        {
            HourTextBox.Text = AlarmModel.AlarmTime.Hour.ToString("D2");
            MinuteTextBox.Text = AlarmModel.AlarmTime.Minute.ToString("D2");
            SecondTextBox.Text = AlarmModel.AlarmTime.Second.ToString("D2");
        }

        /// <summary>
        /// 确定按钮点击
        /// </summary>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // 验证输入
            if (string.IsNullOrWhiteSpace(AlarmModel.Name))
            {
                MessageBox.Show("请输入闹钟名称！", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(AlarmModel.Message))
            {
                MessageBox.Show("请输入闹钟消息！", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 验证并更新时间
            if (!ValidateAndUpdateTime())
            {
                return;
            }

            // 检查时间是否在过去（仅对不循环的闹钟）
            if (AlarmModel.RepeatMode == AlarmRepeatMode.None && AlarmModel.AlarmTime <= DateTime.Now)
            {
                var result = MessageBox.Show(
                    "设置的时间已经过去，闹钟将不会触发。是否继续？",
                    "时间警告",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// 取消按钮点击
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// 验证并更新时间
        /// </summary>
        private bool ValidateAndUpdateTime()
        {
            // 验证小时
            if (!int.TryParse(HourTextBox.Text, out int hour) || hour < 0 || hour > 23)
            {
                MessageBox.Show("小时必须是0-23之间的数字！", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                HourTextBox.Focus();
                return false;
            }

            // 验证分钟
            if (!int.TryParse(MinuteTextBox.Text, out int minute) || minute < 0 || minute > 59)
            {
                MessageBox.Show("分钟必须是0-59之间的数字！", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                MinuteTextBox.Focus();
                return false;
            }

            // 验证秒
            if (!int.TryParse(SecondTextBox.Text, out int second) || second < 0 || second > 59)
            {
                MessageBox.Show("秒必须是0-59之间的数字！", "验证错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                SecondTextBox.Focus();
                return false;
            }

            // 更新闹钟时间
            try
            {
                DateTime date = AlarmModel.AlarmTime.Date;
                AlarmModel.AlarmTime = new DateTime(date.Year, date.Month, date.Day, hour, minute, second);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"时间设置错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
