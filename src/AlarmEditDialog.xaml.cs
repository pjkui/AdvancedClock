using System;
using System.ComponentModel;
using System.Windows;
using Microsoft.Win32;
using AdvancedClock.Actions;

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
        private AlarmModel _originalData = null!;

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
            
            // 初始化动作配置界面
            UpdateActionParameterVisibility();
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
            
            // 初始化动作配置界面
            UpdateActionParameterVisibility();
        }

        /// <summary>
        /// 初始化时间输入框
        /// </summary>
        private void InitializeTimeInputs()
        {
            HourTextBox.Text = AlarmModel.AlarmTime.Hour.ToString("D2");
            MinuteTextBox.Text = AlarmModel.AlarmTime.Minute.ToString("D2");
            SecondTextBox.Text = AlarmModel.AlarmTime.Second.ToString("D2");
            
            // 初始化提前提醒输入框
            AdvanceMinutesTextBox.Text = AlarmModel.AdvanceMinutes.ToString();
            RepeatIntervalTextBox.Text = AlarmModel.RepeatIntervalMinutes.ToString();
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
                IsStrongAlert = AlarmModel.IsStrongAlert,
                EnableAdvanceReminder = AlarmModel.EnableAdvanceReminder,
                AdvanceMinutes = AlarmModel.AdvanceMinutes,
                RepeatIntervalMinutes = AlarmModel.RepeatIntervalMinutes,
                ActionType = AlarmModel.ActionType,
                ActionParameter = AlarmModel.ActionParameter,
                ActionTimeoutSeconds = AlarmModel.ActionTimeoutSeconds
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

            // 检查提前提醒输入框是否有变化
            if (HasAdvanceReminderInputChanged())
            {
                return true;
            }

            return _originalData.Name != AlarmModel.Name ||
                   _originalData.AlarmTime != AlarmModel.AlarmTime ||
                   _originalData.RepeatMode != AlarmModel.RepeatMode ||
                   _originalData.IsEnabled != AlarmModel.IsEnabled ||
                   _originalData.Message != AlarmModel.Message ||
                   _originalData.IsStrongAlert != AlarmModel.IsStrongAlert ||
                   _originalData.EnableAdvanceReminder != AlarmModel.EnableAdvanceReminder ||
                   _originalData.AdvanceMinutes != AlarmModel.AdvanceMinutes ||
                   _originalData.RepeatIntervalMinutes != AlarmModel.RepeatIntervalMinutes ||
                   _originalData.ActionType != AlarmModel.ActionType ||
                   _originalData.ActionParameter != AlarmModel.ActionParameter ||
                   _originalData.ActionTimeoutSeconds != AlarmModel.ActionTimeoutSeconds;
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
        /// 检查提前提醒输入框是否有变化（不修改数据）
        /// </summary>
        private bool HasAdvanceReminderInputChanged()
        {
            string currentAdvanceMinutes = AdvanceMinutesTextBox.Text;
            string currentRepeatInterval = RepeatIntervalTextBox.Text;

            string originalAdvanceMinutes = _originalData.AdvanceMinutes.ToString();
            string originalRepeatInterval = _originalData.RepeatIntervalMinutes.ToString();

            return currentAdvanceMinutes != originalAdvanceMinutes ||
                   currentRepeatInterval != originalRepeatInterval;
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
            AlarmModel.EnableAdvanceReminder = _originalData.EnableAdvanceReminder;
            AlarmModel.AdvanceMinutes = _originalData.AdvanceMinutes;
            AlarmModel.RepeatIntervalMinutes = _originalData.RepeatIntervalMinutes;
            AlarmModel.ActionType = _originalData.ActionType;
            AlarmModel.ActionParameter = _originalData.ActionParameter;
            AlarmModel.ActionTimeoutSeconds = _originalData.ActionTimeoutSeconds;

            // 恢复时间输入框显示
            HourTextBox.Text = _originalData.AlarmTime.Hour.ToString("D2");
            MinuteTextBox.Text = _originalData.AlarmTime.Minute.ToString("D2");
            SecondTextBox.Text = _originalData.AlarmTime.Second.ToString("D2");
            
            // 恢复提前提醒输入框显示
            AdvanceMinutesTextBox.Text = _originalData.AdvanceMinutes.ToString();
            RepeatIntervalTextBox.Text = _originalData.RepeatIntervalMinutes.ToString();
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

            // 验证并更新提前提醒设置
            if (!ValidateAndUpdateAdvanceReminder())
            {
                return false;
            }

            // 验证并更新动作配置
            if (!ValidateActionConfiguration())
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

        /// <summary>
        /// 验证并更新提前提醒设置
        /// </summary>
        private bool ValidateAndUpdateAdvanceReminder()
        {
            // 如果未启用提前提醒，跳过验证
            if (!AlarmModel.EnableAdvanceReminder)
            {
                return true;
            }

            // 验证提前分钟数
            if (!int.TryParse(AdvanceMinutesTextBox.Text, out int advanceMinutes) || advanceMinutes < 1 || advanceMinutes > 60)
            {
                MessageBox.Show("提前分钟数必须是1-60之间的数字！", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                AdvanceMinutesTextBox.Focus();
                return false;
            }

            // 验证重复间隔分钟数
            if (!int.TryParse(RepeatIntervalTextBox.Text, out int repeatInterval) || repeatInterval < 1 || repeatInterval > 10)
            {
                MessageBox.Show("重复间隔分钟数必须是1-10之间的数字！", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                RepeatIntervalTextBox.Focus();
                return false;
            }

            // 更新提前提醒设置
            AlarmModel.AdvanceMinutes = advanceMinutes;
            AlarmModel.RepeatIntervalMinutes = repeatInterval;

            return true;
        }

        /// <summary>
        /// 验证动作配置
        /// </summary>
        private bool ValidateActionConfiguration()
        {
            // 如果动作类型为"仅提醒"，无需验证参数
            if (AlarmModel.ActionType == AlarmActionType.None)
            {
                return true;
            }

            // 验证超时设置
            if (!int.TryParse(TimeoutTextBox.Text, out int timeout) || timeout < 5 || timeout > 300)
            {
                MessageBox.Show("执行超时必须是5-300秒之间的数字！", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                TimeoutTextBox.Focus();
                return false;
            }
            AlarmModel.ActionTimeoutSeconds = timeout;

            // 根据动作类型验证参数
            ActionExecutor? executor = AlarmModel.ActionType switch
            {
                AlarmActionType.OpenUrl => new UrlActionExecutor(),
                AlarmActionType.ExecuteCommand => new CommandActionExecutor(),
                AlarmActionType.RunPythonScript => new PythonScriptActionExecutor(),
                _ => null
            };

            if (executor != null)
            {
                var validationError = executor.ValidateParameter(AlarmModel.ActionParameter);
                if (validationError != null)
                {
                    MessageBox.Show($"动作参数验证失败：\n{validationError}", "输入错误", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 动作类型改变事件
        /// </summary>
        private void ActionType_Changed(object sender, RoutedEventArgs e)
        {
            UpdateActionParameterVisibility();
        }

        /// <summary>
        /// 更新动作参数输入面板的可见性
        /// </summary>
        private void UpdateActionParameterVisibility()
        {
            // 隐藏所有输入面板
            UrlInputPanel.Visibility = Visibility.Collapsed;
            CommandInputPanel.Visibility = Visibility.Collapsed;
            PythonInputPanel.Visibility = Visibility.Collapsed;

            // 根据动作类型显示相应的输入面板
            if (AlarmModel.ActionType != AlarmActionType.None)
            {
                ActionParameterPanel.Visibility = Visibility.Visible;

                switch (AlarmModel.ActionType)
                {
                    case AlarmActionType.OpenUrl:
                        UrlInputPanel.Visibility = Visibility.Visible;
                        break;
                    case AlarmActionType.ExecuteCommand:
                        CommandInputPanel.Visibility = Visibility.Visible;
                        break;
                    case AlarmActionType.RunPythonScript:
                        PythonInputPanel.Visibility = Visibility.Visible;
                        break;
                }
            }
            else
            {
                ActionParameterPanel.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 测试动作按钮点击
        /// </summary>
        private async void TestAction_Click(object sender, RoutedEventArgs e)
        {
            if (AlarmModel.ActionType == AlarmActionType.None)
            {
                return;
            }

            // 验证参数
            if (!ValidateActionConfiguration())
            {
                return;
            }

            try
            {
                ActionExecutor? executor = AlarmModel.ActionType switch
                {
                    AlarmActionType.OpenUrl => new UrlActionExecutor(),
                    AlarmActionType.ExecuteCommand => new CommandActionExecutor(),
                    AlarmActionType.RunPythonScript => new PythonScriptActionExecutor(),
                    _ => null
                };

                if (executor != null)
                {
                    MessageBox.Show("正在执行动作，请稍候...", "测试中", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    var result = await executor.ExecuteAsync(AlarmModel.ActionParameter, AlarmModel.ActionTimeoutSeconds);
                    
                    if (result.Success)
                    {
                        MessageBox.Show($"动作执行成功！\n\n{result.Message}", "测试成功", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show($"动作执行失败！\n\n{result.Message}\n\n错误详情：\n{result.ErrorDetails}", 
                            "测试失败", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"测试过程中发生异常：\n{ex.Message}", "错误", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 浏览Python脚本按钮点击
        /// </summary>
        private void BrowsePython_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "选择Python脚本文件",
                Filter = "Python文件 (*.py)|*.py|所有文件 (*.*)|*.*",
                FilterIndex = 1,
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                AlarmModel.ActionParameter = openFileDialog.FileName;
                PythonScriptTextBox.Text = openFileDialog.FileName;
            }
        }
    }
}
