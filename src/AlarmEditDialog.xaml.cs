using System;
using System.ComponentModel;
using System.Windows;

namespace AdvancedClock
{
    /// <summary>
    /// 闹钟编辑对话框
    /// </summary>
    public partial class AlarmEditDialog : Window
    {
        public AlarmModel AlarmModel { get; private set; }

        /// <summary>
        /// 原始数据的副本，用于检测修改
        /// </summary>
        private AlarmModel _originalData;

        /// <summary>
        /// 构造函数（新建模式）
        /// </summary>
        public AlarmEditDialog()
        {
            InitializeComponent();
            AlarmModel = new AlarmModel();
            DataContext = AlarmModel;
            Title = "添加闹钟";

            // 保存原始数据副本
            SaveOriginalData();

            // 初始化时间输入框
            InitializeTimeInputs();
        }

        /// <summary>
        /// 构造函数（编辑模式）
        /// </summary>
        public AlarmEditDialog(AlarmModel alarm)
        {
            InitializeComponent();
            AlarmModel = alarm;
            DataContext = AlarmModel;
            Title = "编辑闹钟";

            // 保存原始数据副本
            SaveOriginalData();

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
        /// 保存原始数据副本
        /// </summary>
        private void SaveOriginalData()
        {
            _originalData = new AlarmModel
            {
                Id = AlarmModel.Id,
                Name = AlarmModel.Name,
                AlarmTime = AlarmModel.AlarmTime,
                RepeatMode = AlarmModel.RepeatMode,
                IsEnabled = AlarmModel.IsEnabled,
                Message = AlarmModel.Message,
                IsStrongAlert = AlarmModel.IsStrongAlert
            };
        }

        /// <summary>
        /// 检查是否有未保存的修改
        /// </summary>
        private bool HasUnsavedChanges()
        {
            // 检查时间输入框是否有变化（不修改实际数据）
            if (HasTimeInputChanged())
            {
                return true;
            }

            return _originalData.Name != AlarmModel.Name ||
                   _originalData.AlarmTime != AlarmModel.AlarmTime ||
                   _originalData.RepeatMode != AlarmModel.RepeatMode ||
                   _originalData.IsEnabled != AlarmModel.IsEnabled ||
                   _originalData.Message != AlarmModel.Message ||
                   _originalData.IsStrongAlert != AlarmModel.IsStrongAlert;
        }

        /// <summary>
        /// 检查时间输入框是否有变化（不修改数据）
        /// </summary>
        private bool HasTimeInputChanged()
        {
            // 获取当前时间输入框的值
            string currentHour = HourTextBox.Text;
            string currentMinute = MinuteTextBox.Text;
            string currentSecond = SecondTextBox.Text;

            // 获取原始时间的字符串表示
            string originalHour = _originalData.AlarmTime.Hour.ToString("D2");
            string originalMinute = _originalData.AlarmTime.Minute.ToString("D2");
            string originalSecond = _originalData.AlarmTime.Second.ToString("D2");

            return currentHour != originalHour ||
                   currentMinute != originalMinute ||
                   currentSecond != originalSecond;
        }

        /// <summary>
        /// 恢复到编辑前的状态
        /// </summary>
        private void RestoreOriginalData()
        {
            // 恢复所有属性到原始状态
            AlarmModel.Id = _originalData.Id;
            AlarmModel.Name = _originalData.Name;
            AlarmModel.AlarmTime = _originalData.AlarmTime;
            AlarmModel.RepeatMode = _originalData.RepeatMode;
            AlarmModel.IsEnabled = _originalData.IsEnabled;
            AlarmModel.Message = _originalData.Message;
            AlarmModel.IsStrongAlert = _originalData.IsStrongAlert;

            // 恢复时间输入框显示
            HourTextBox.Text = _originalData.AlarmTime.Hour.ToString("D2");
            MinuteTextBox.Text = _originalData.AlarmTime.Minute.ToString("D2");
            SecondTextBox.Text = _originalData.AlarmTime.Second.ToString("D2");
        }

        /// <summary>
        /// 窗口关闭事件处理
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            // 如果是通过确定或取消按钮关闭的，不需要检查
            if (DialogResult.HasValue)
            {
                base.OnClosing(e);
                return;
            }

            // 检查是否有未保存的修改
            if (HasUnsavedChanges())
            {
                var result = ShowUnsavedChangesDialog();
                switch (result)
                {
                    case MessageBoxResult.Yes: // 保存修改
                        if (SaveChanges())
                        {
                            DialogResult = true;
                        }
                        else
                        {
                            e.Cancel = true; // 保存失败，取消关闭
                        }
                        break;
                    case MessageBoxResult.No: // 不保存，恢复原始状态
                        RestoreOriginalData();
                        DialogResult = false;
                        break;
                    case MessageBoxResult.Cancel: // 取消关闭，继续编辑
                        e.Cancel = true;
                        break;
                }
            }

            base.OnClosing(e);
        }

        /// <summary>
        /// 显示未保存修改确认对话框
        /// </summary>
        private MessageBoxResult ShowUnsavedChangesDialog()
        {
            return MessageBox.Show(
                "检测到您有未保存的修改，请选择如何处理：\n\n" +
                "• 点击\"是\" - 保存所有修改并关闭\n" +
                "• 点击\"否\" - 不保存任何修改，恢复到编辑前状态\n" +
                "• 点击\"取消\" - 返回继续编辑\n\n" +
                "注意：选择\"否\"将丢失所有未保存的更改！",
                "保存确认",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question,
                MessageBoxResult.Yes);
        }

        /// <summary>
        /// 保存修改
        /// </summary>
        private bool SaveChanges()
        {
            // 验证输入
            if (string.IsNullOrWhiteSpace(AlarmModel.Name))
            {
                MessageBox.Show("请输入闹钟名称！", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(AlarmModel.Message))
            {
                MessageBox.Show("请输入闹钟消息！", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // 验证并更新时间
            if (!ValidateAndUpdateTime())
            {
                return false;
            }

            // 检查时间是否在过去（仅对不循环的闹钟）
            if (AlarmModel.RepeatMode == AlarmRepeatMode.None && AlarmModel.AlarmTime <= DateTime.Now)
            {
                var result = MessageBox.Show(
                    "设置的时间已经过去，闹钟将不会触发。是否继续保存？",
                    "时间提醒",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 确定按钮点击
        /// </summary>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveChanges())
            {
                DialogResult = true;
                Close();
            }
        }

        /// <summary>
        /// 取消按钮点击
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // 检查是否有未保存的修改
            if (HasUnsavedChanges())
            {
                var result = MessageBox.Show(
                    "您有未保存的修改，确定要取消编辑吗？\n\n所有修改将会丢失！",
                    "取消确认",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning,
                    MessageBoxResult.No);

                if (result == MessageBoxResult.No)
                {
                    return; // 用户选择不取消，继续编辑
                }
            }

            // 恢复到原始状态
            RestoreOriginalData();
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
                MessageBox.Show("小时必须是0-23之间的数字！", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                HourTextBox.Focus();
                return false;
            }

            // 验证分钟
            if (!int.TryParse(MinuteTextBox.Text, out int minute) || minute < 0 || minute > 59)
            {
                MessageBox.Show("分钟必须是0-59之间的数字！", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                MinuteTextBox.Focus();
                return false;
            }

            // 验证秒
            if (!int.TryParse(SecondTextBox.Text, out int second) || second < 0 || second > 59)
            {
                MessageBox.Show("秒必须是0-59之间的数字！", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
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
