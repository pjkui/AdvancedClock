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

            // 初始化声音选择
            InitializeSoundSelection();

            // 初始化农历选择
            InitializeLunarSelection();
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

            // 初始化声音选择
            InitializeSoundSelection();

            // 初始化农历选择
            InitializeLunarSelection();
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

            // 验证并更新声音播放时长
            if (!ValidateAndUpdateSoundDuration())
            {
                return false;
            }

            // 验证并更新动作配置
            if (!ValidateActionConfiguration())
            {
                return false;
            }

            // 检查时间是否在过去
            if (AlarmModel.AlarmTime <= DateTime.Now)
            {
                if (AlarmModel.RepeatMode == AlarmRepeatMode.None)
                {
                    // 不循环的闹钟，提示用户
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
                else
                {
                    // 循环闹钟，自动调整到下一个有效时间
                    var nextTime = AlarmModel.GetNextAlarmTime();
                    if (nextTime != AlarmModel.AlarmTime)
                    {
                        AlarmModel.AlarmTime = nextTime;

                        string repeatModeText = AlarmModel.RepeatMode switch
                        {
                            AlarmRepeatMode.Daily => "每天",
                            AlarmRepeatMode.Monthly => "每月",
                            AlarmRepeatMode.Yearly => "每年",
                            _ => "循环"
                        };

                        MessageBox.Show(
                            $"设置的时间已经过去，已自动调整到下次{repeatModeText}的时间：\n{nextTime:yyyy-MM-dd HH:mm:ss}",
                            "时间已调整",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
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
        /// 浏览声音文件按钮点击
        /// </summary>
        private void BrowseSound_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择闹钟声音文件",
                Filter = AudioService.GetSupportedFormats(),
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                // 验证文件是否有效
                if (AudioService.Instance.IsValidAudioFile(filePath))
                {
                    AlarmModel.CustomSoundPath = filePath;
                    MessageBox.Show($"已选择声音文件：\n{System.IO.Path.GetFileName(filePath)}",
                        "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("选择的文件不是有效的音频文件！\n支持的格式：WAV, MP3, WMA, M4A",
                        "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// 试听声音按钮点击
        /// </summary>
        private void TestSound_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 获取当前选中的声音路径
                string soundPath = string.Empty;

                if (UseDefaultSoundRadio.IsChecked == true)
                {
                    // 使用默认声音
                    if (DefaultSoundComboBox.SelectedItem is System.Windows.Controls.ComboBoxItem item)
                    {
                        soundPath = item.Tag?.ToString() ?? string.Empty;
                    }
                }
                else
                {
                    // 使用自定义声音
                    soundPath = AlarmModel.CustomSoundPath;
                }

                if (string.IsNullOrWhiteSpace(soundPath))
                {
                    // 播放系统默认声音（5秒）
                    AudioService.Instance.PlayAlarmSound(null, AlarmModel.IsStrongAlert, 5);
                    MessageBox.Show("正在播放系统默认声音（5秒）", "试听",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (System.IO.File.Exists(soundPath))
                {
                    // 播放自定义声音（5秒）
                    AudioService.Instance.PlayAlarmSound(soundPath, AlarmModel.IsStrongAlert, 5);
                    MessageBox.Show($"正在播放：\n{System.IO.Path.GetFileName(soundPath)}\n\n试听时长：5秒",
                        "试听", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("声音文件不存在！\n请重新选择或清除后使用系统默认声音。",
                        "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"播放声音失败：\n{ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 清除声音按钮点击
        /// </summary>
        private void ClearSound_Click(object sender, RoutedEventArgs e)
        {
            AlarmModel.CustomSoundPath = string.Empty;
            UseDefaultSoundRadio.IsChecked = true;
            DefaultSoundComboBox.SelectedIndex = 0; // 选中系统默认
            MessageBox.Show("已清除自定义声音，将使用系统默认声音", "提示",
                MessageBoxButton.OK, MessageBoxImage.Information);
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

        /// <summary>
        /// 初始化声音选择
        /// </summary>
        private void InitializeSoundSelection()
        {
            // 确保控件已初始化
            if (DefaultSoundComboBox == null || MaxPlayDurationTextBox == null)
            {
                return;
            }

            // 加载默认声音列表
            LoadDefaultSounds();

            // 初始化播放时长输入框
            MaxPlayDurationTextBox.Text = AlarmModel.MaxPlayDurationSeconds.ToString();

            // 根据当前声音路径设置选择状态
            if (string.IsNullOrWhiteSpace(AlarmModel.CustomSoundPath))
            {
                UseDefaultSoundRadio.IsChecked = true;
            }
            else
            {
                // 检查是否是默认声音
                var defaultSounds = AudioService.GetDefaultSounds();
                if (defaultSounds.Contains(AlarmModel.CustomSoundPath))
                {
                    UseDefaultSoundRadio.IsChecked = true;
                    // 选中对应的默认声音
                    for (int i = 0; i < DefaultSoundComboBox.Items.Count; i++)
                    {
                        var item = DefaultSoundComboBox.Items[i] as System.Windows.Controls.ComboBoxItem;
                        if (item?.Tag?.ToString() == AlarmModel.CustomSoundPath)
                        {
                            DefaultSoundComboBox.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    UseCustomSoundRadio.IsChecked = true;
                }
            }
        }

        /// <summary>
        /// 加载默认声音列表
        /// </summary>
        private void LoadDefaultSounds()
        {
            // 确保控件已初始化
            if (DefaultSoundComboBox == null)
            {
                return;
            }

            DefaultSoundComboBox.Items.Clear();

            // 添加系统默认选项
            var systemDefaultItem = new System.Windows.Controls.ComboBoxItem
            {
                Content = "系统默认声音",
                Tag = string.Empty
            };
            DefaultSoundComboBox.Items.Add(systemDefaultItem);

            // 获取 sounds/defaults 目录下的所有声音文件
            var defaultSounds = AudioService.GetDefaultSounds();

            if (defaultSounds.Count > 0)
            {
                foreach (var soundPath in defaultSounds)
                {
                    var fileName = System.IO.Path.GetFileName(soundPath);
                    var item = new System.Windows.Controls.ComboBoxItem
                    {
                        Content = fileName,
                        Tag = soundPath
                    };
                    DefaultSoundComboBox.Items.Add(item);
                }
            }
            else
            {
                // 如果没有默认声音，添加提示
                var noSoundItem = new System.Windows.Controls.ComboBoxItem
                {
                    Content = "（没有找到默认声音文件）",
                    Tag = string.Empty,
                    IsEnabled = false
                };
                DefaultSoundComboBox.Items.Add(noSoundItem);
            }

            // 默认选中第一项（系统默认）
            DefaultSoundComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// 声音源改变事件
        /// </summary>
        private void SoundSourceChanged(object sender, RoutedEventArgs e)
        {
            // 防止在初始化阶段访问未创建的控件
            if (DefaultSoundPanel == null || CustomSoundPanel == null)
            {
                return;
            }

            if (UseDefaultSoundRadio.IsChecked == true)
            {
                DefaultSoundPanel.Visibility = Visibility.Visible;
                CustomSoundPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                DefaultSoundPanel.Visibility = Visibility.Collapsed;
                CustomSoundPanel.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 默认声音下拉框选择改变事件
        /// </summary>
        private void DefaultSoundComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // 防止在初始化阶段访问
            if (DefaultSoundComboBox == null || AlarmModel == null)
            {
                return;
            }

            if (DefaultSoundComboBox.SelectedItem is System.Windows.Controls.ComboBoxItem item)
            {
                string soundPath = item.Tag?.ToString() ?? string.Empty;
                AlarmModel.CustomSoundPath = soundPath;
            }
        }

        /// <summary>
        /// 验证并更新声音播放时长
        /// </summary>
        private bool ValidateAndUpdateSoundDuration()
        {
            // 验证播放时长
            if (!int.TryParse(MaxPlayDurationTextBox.Text, out int duration) || duration < 5 || duration > 600)
            {
                MessageBox.Show("播放时长必须是5-600秒之间的数字！\n\n提示：60秒=1分钟，300秒=5分钟，600秒=10分钟",
                    "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                MaxPlayDurationTextBox.Focus();
                return false;
            }

            // 更新播放时长
            AlarmModel.MaxPlayDurationSeconds = duration;

            return true;
        }

        #region 农历相关方法

        /// <summary>
        /// 初始化农历选择控件
        /// </summary>
        private void InitializeLunarSelection()
        {
            // 填充农历月份下拉框
            var monthNames = LunarCalendarService.GetMonthNames();
            for (int i = 0; i < monthNames.Count; i++)
            {
                var item = new System.Windows.Controls.ComboBoxItem
                {
                    Content = monthNames[i],
                    Tag = i + 1
                };
                LunarMonthComboBox.Items.Add(item);
            }

            // 填充农历日期下拉框
            var dayNames = LunarCalendarService.GetDayNames();
            for (int i = 0; i < dayNames.Count; i++)
            {
                var item = new System.Windows.Controls.ComboBoxItem
                {
                    Content = dayNames[i],
                    Tag = i + 1
                };
                LunarDayComboBox.Items.Add(item);
            }

            // 设置当前值
            if (AlarmModel.IsLunarCalendar)
            {
                LunarMonthComboBox.SelectedIndex = AlarmModel.LunarMonth - 1;
                LunarDayComboBox.SelectedIndex = AlarmModel.LunarDay - 1;
                IsLeapMonthCheckBox.IsChecked = AlarmModel.IsLeapMonth;
            }
            else
            {
                // 默认选择正月初一
                LunarMonthComboBox.SelectedIndex = 0;
                LunarDayComboBox.SelectedIndex = 0;
                IsLeapMonthCheckBox.IsChecked = false;
            }

            // 更新农历面板可见性
            UpdateLunarPanelVisibility();

            // 更新当前农历日期显示
            UpdateCurrentLunarDateDisplay();
        }

        /// <summary>
        /// 循环模式改变事件
        /// </summary>
        private void RepeatModeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateLunarPanelVisibility();
        }

        /// <summary>
        /// 更新农历面板可见性
        /// </summary>
        private void UpdateLunarPanelVisibility()
        {
            if (LunarDatePanel == null || RepeatModeComboBox == null)
                return;

            // 只有选择"每年农历"时才显示农历面板
            if (RepeatModeComboBox.SelectedItem is System.Windows.Controls.ComboBoxItem item)
            {
                if (item.Tag is AlarmRepeatMode mode && mode == AlarmRepeatMode.LunarYearly)
                {
                    LunarDatePanel.Visibility = Visibility.Visible;
                    AlarmModel.IsLunarCalendar = true;
                    UpdateCorrespondingSolarDate();
                }
                else
                {
                    LunarDatePanel.Visibility = Visibility.Collapsed;
                    AlarmModel.IsLunarCalendar = false;
                }
            }
        }

        /// <summary>
        /// 农历日期改变事件
        /// </summary>
        private void LunarDateChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            UpdateCorrespondingSolarDate();
        }

        /// <summary>
        /// 更新当前农历日期显示
        /// </summary>
        private void UpdateCurrentLunarDateDisplay()
        {
            if (CurrentLunarDateText == null)
                return;

            try
            {
                var currentDate = DateTime.Now;
                var lunarDate = LunarCalendarService.SolarToLunar(currentDate);
                CurrentLunarDateText.Text = $"当前公历日期对应的农历：{lunarDate.DisplayText}";
            }
            catch (Exception ex)
            {
                CurrentLunarDateText.Text = $"无法获取当前农历日期：{ex.Message}";
            }
        }

        /// <summary>
        /// 更新对应的公历日期显示
        /// </summary>
        private void UpdateCorrespondingSolarDate()
        {
            if (CorrespondingSolarDateText == null ||
                LunarMonthComboBox == null ||
                LunarDayComboBox == null ||
                IsLeapMonthCheckBox == null)
                return;

            try
            {
                if (LunarMonthComboBox.SelectedItem is System.Windows.Controls.ComboBoxItem monthItem &&
                    LunarDayComboBox.SelectedItem is System.Windows.Controls.ComboBoxItem dayItem)
                {
                    int month = (int)monthItem.Tag;
                    int day = (int)dayItem.Tag;
                    bool isLeapMonth = IsLeapMonthCheckBox.IsChecked == true;

                    // 保存到模型
                    AlarmModel.LunarMonth = month;
                    AlarmModel.LunarDay = day;
                    AlarmModel.IsLeapMonth = isLeapMonth;

                    // 计算对应的公历日期（使用当前年份或下一年）
                    var currentDate = DateTime.Now;
                    var currentLunar = LunarCalendarService.SolarToLunar(currentDate);

                    // 尝试当前年份
                    try
                    {
                        var solarDate = LunarCalendarService.LunarToSolar(currentLunar.Year, month, day, isLeapMonth);

                        // 如果日期已过，尝试下一年
                        if (solarDate < currentDate.Date)
                        {
                            solarDate = LunarCalendarService.LunarToSolar(currentLunar.Year + 1, month, day, isLeapMonth);
                        }

                        // 更新闹钟时间的日期部分（保留时分秒）
                        AlarmModel.AlarmTime = new DateTime(
                            solarDate.Year, solarDate.Month, solarDate.Day,
                            AlarmModel.AlarmTime.Hour, AlarmModel.AlarmTime.Minute, AlarmModel.AlarmTime.Second);

                        string leapText = isLeapMonth ? "闰" : "";
                        CorrespondingSolarDateText.Text =
                            $"选择的农历日期对应的公历：{leapText}{GetLunarMonthName(month)}{GetLunarDayName(day)} → {solarDate:yyyy年MM月dd日}";
                    }
                    catch (Exception ex)
                    {
                        CorrespondingSolarDateText.Text = $"无法转换：{ex.Message}";
                    }
                }
            }
            catch (Exception ex)
            {
                CorrespondingSolarDateText.Text = $"转换错误：{ex.Message}";
            }
        }

        private static string GetLunarMonthName(int month)
        {
            string[] monthNames = { "", "正月", "二月", "三月", "四月", "五月", "六月",
                                   "七月", "八月", "九月", "十月", "冬月", "腊月" };
            return month >= 1 && month <= 12 ? monthNames[month] : month.ToString();
        }

        private static string GetLunarDayName(int day)
        {
            string[] dayNames = { "", "初一", "初二", "初三", "初四", "初五", "初六", "初七", "初八", "初九", "初十",
                                 "十一", "十二", "十三", "十四", "十五", "十六", "十七", "十八", "十九", "二十",
                                 "廿一", "廿二", "廿三", "廿四", "廿五", "廿六", "廿七", "廿八", "廿九", "三十" };
            return day >= 1 && day <= 30 ? dayNames[day] : day.ToString();
        }

        #endregion
    }
}
