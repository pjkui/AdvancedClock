using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Media;
using WinForms = System.Windows.Forms;
using AdvancedClock.Actions;
using AdvancedClock.Services;

namespace AdvancedClock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<AlarmModel> _alarms;
        private readonly AlarmService _alarmService;
        private readonly DispatcherTimer _clockTimer;
        private readonly AlarmDataService _dataService;
        private readonly StartupService _startupService;
        private readonly AppSettings _appSettings;
        private readonly LogService _logService;
        private WinForms.NotifyIcon? _notifyIcon;
        private bool _isReallyClosing = false;

        public MainWindow()
        {
            InitializeComponent();

            // 设置窗口图标
            SetWindowIcon();

            // 初始化任务栏通知图标
            InitializeNotifyIcon();

            // 初始化数据服务
            _dataService = new AlarmDataService();
            _startupService = new StartupService();
            _appSettings = new AppSettings();
            _logService = LogService.Instance;

            // 记录应用启动

            // 初始化闹钟集合
            _alarms = new ObservableCollection<AlarmModel>();
            AlarmListView.DataContext = _alarms;

            // 监听集合变化以自动保存
            _alarms.CollectionChanged += Alarms_CollectionChanged;

            // 加载保存的闹钟数据
            LoadAlarms();

            // 初始化闹钟服务
            _alarmService = new AlarmService(_alarms);
            _alarmService.AlarmTriggered += AlarmService_AlarmTriggered;
            _alarmService.AlarmReminderTriggered += AlarmService_AlarmReminderTriggered;
            _alarmService.Start();

            // 初始化时钟显示定时器
            _clockTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _clockTimer.Tick += ClockTimer_Tick;
            _clockTimer.Start();

            // 初始化当前时间显示
            UpdateCurrentTime();

            // 显示时钟精度信息
            UpdateClockPrecisionInfo();

            // 更新开机启动状态显示
            UpdateStartupStatus();

            // 更新托盘设置状态显示
            UpdateTraySettings();
        }

        /// <summary>
        /// 加载闹钟数据
        /// </summary>
        private void LoadAlarms()
        {
            var savedAlarms = _dataService.LoadAlarms();

            if (savedAlarms.Count > 0)
            {
                bool needsSave = false;

                // 加载保存的闹钟
                foreach (var alarm in savedAlarms)
                {
                    // 自动修复过期的循环闹钟
                    if (alarm.IsEnabled && alarm.RepeatMode != AlarmRepeatMode.None && alarm.AlarmTime <= DateTime.Now)
                    {
                        var nextTime = alarm.GetNextAlarmTime();
                        if (nextTime != alarm.AlarmTime)
                        {
                            alarm.AlarmTime = nextTime;
                            needsSave = true;

                            string repeatModeText = alarm.RepeatMode switch
                            {
                                AlarmRepeatMode.Daily => "每天",
                                AlarmRepeatMode.Monthly => "每月",
                                AlarmRepeatMode.Yearly => "每年",
                                _ => "循环"
                            };

                            System.Diagnostics.Debug.WriteLine($"自动修复闹钟 '{alarm.Name}' 到下次{repeatModeText}时间: {nextTime:yyyy-MM-dd HH:mm:ss}");
                        }
                    }

                    // 为每个闹钟添加属性变化监听
                    alarm.PropertyChanged += Alarm_PropertyChanged;
                    _alarms.Add(alarm);
                }

                // 如果有修复，保存数据
                if (needsSave)
                {
                    SaveAlarms();
                }
            }
            else
            {
                // 首次运行，添加示例闹钟
                AddSampleAlarms();
            }
        }

        /// <summary>
        /// 添加示例闹钟
        /// </summary>
        private void AddSampleAlarms()
        {
            // 示例1：5分钟后的一次性闹钟（带提前提醒）
            var testAlarm = new AlarmModel
            {
                Name = "测试闹钟",
                AlarmTime = DateTime.Now.AddMinutes(5),
                RepeatMode = AlarmRepeatMode.None,
                Message = "这是一个测试闹钟！",
                IsEnabled = true,
                EnableAdvanceReminder = true,
                AdvanceMinutes = 2,
                RepeatIntervalMinutes = 1
            };
            testAlarm.PropertyChanged += Alarm_PropertyChanged;
            _alarms.Add(testAlarm);

            // 示例2：每天早上8点的闹钟（带提前提醒）
            var dailyAlarm = DateTime.Today.AddHours(8);
            if (dailyAlarm <= DateTime.Now)
            {
                dailyAlarm = dailyAlarm.AddDays(1);
            }
            var dailyAlarmModel = new AlarmModel
            {
                Name = "每日提醒",
                AlarmTime = dailyAlarm,
                RepeatMode = AlarmRepeatMode.Daily,
                Message = "早上好！新的一天开始了！",
                IsEnabled = true,
                EnableAdvanceReminder = true,
                AdvanceMinutes = 10,
                RepeatIntervalMinutes = 5
            };
            dailyAlarmModel.PropertyChanged += Alarm_PropertyChanged;
            _alarms.Add(dailyAlarmModel);

            // 示例3：每月1号的闹钟
            var monthlyAlarm = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 9, 0, 0);
            if (monthlyAlarm <= DateTime.Now)
            {
                monthlyAlarm = monthlyAlarm.AddMonths(1);
            }
            var monthlyAlarmModel = new AlarmModel
            {
                Name = "月度提醒",
                AlarmTime = monthlyAlarm,
                RepeatMode = AlarmRepeatMode.Monthly,
                Message = "新的一个月开始了！",
                IsEnabled = true
            };
            monthlyAlarmModel.PropertyChanged += Alarm_PropertyChanged;
            _alarms.Add(monthlyAlarmModel);

            // 示例4：每年生日提醒
            var yearlyAlarm = new DateTime(DateTime.Now.Year, 6, 15, 10, 0, 0);
            if (yearlyAlarm <= DateTime.Now)
            {
                yearlyAlarm = yearlyAlarm.AddYears(1);
            }
            var yearlyAlarmModel = new AlarmModel
            {
                Name = "生日提醒",
                AlarmTime = yearlyAlarm,
                RepeatMode = AlarmRepeatMode.Yearly,
                Message = "生日快乐！",
                IsEnabled = true,
                IsStrongAlert = true  // 设置为强提醒示例
            };
            yearlyAlarmModel.PropertyChanged += Alarm_PropertyChanged;
            _alarms.Add(yearlyAlarmModel);
        }

        /// <summary>
        /// 更新当前时间显示
        /// </summary>
        private void UpdateCurrentTime()
        {
            var now = HighPrecisionClock.Now;
            CurrentTimeText.Text = now.ToString("yyyy-MM-dd HH:mm:ss.fff dddd");
        }

        /// <summary>
        /// 更新时钟精度信息显示
        /// </summary>
        private void UpdateClockPrecisionInfo()
        {
            var precisionInfo = HighPrecisionClock.GetPrecisionInfo();
            ClockPrecisionText.Text = precisionInfo.ToString();
        }

        /// <summary>
        /// 时钟定时器触发
        /// </summary>
        private void ClockTimer_Tick(object? sender, EventArgs e)
        {
            UpdateCurrentTime();
            UpdateAllCountdowns();
        }

        /// <summary>
        /// 更新所有闹钟的倒计时显示
        /// </summary>
        private void UpdateAllCountdowns()
        {
            foreach (var alarm in _alarms)
            {
                alarm.UpdateCountdown();
            }
        }

        /// <summary>
        /// 设置窗口图标
        /// </summary>
        private void SetWindowIcon()
        {
            try
            {
                var customIcon = IconHelper.GetApplicationIcon();
                if (customIcon != null)
                {
                    // 将 System.Drawing.Icon 转换为 WPF ImageSource
                    using (var bitmap = customIcon.ToBitmap())
                    {
                        var hBitmap = bitmap.GetHbitmap();
                        try
                        {
                            this.Icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                hBitmap,
                                IntPtr.Zero,
                                System.Windows.Int32Rect.Empty,
                                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                        }
                        finally
                        {
                            // 释放 HBitmap
                            DeleteObject(hBitmap);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"设置窗口图标失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 释放 GDI 对象
        /// </summary>
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// 初始化任务栏通知图标
        /// </summary>
        private void InitializeNotifyIcon()
        {
            // 获取自定义图标，如果失败则使用系统默认图标
            var customIcon = IconHelper.GetApplicationIcon();

            _notifyIcon = new WinForms.NotifyIcon
            {
                Icon = customIcon ?? System.Drawing.SystemIcons.Information,
                Visible = true,
                Text = "高级闹钟"
            };

            // 气泡提示点击事件
            _notifyIcon.BalloonTipClicked += (s, e) =>
            {
                ShowMainWindow();
            };

            // 双击托盘图标显示窗口
            _notifyIcon.DoubleClick += (s, e) =>
            {
                ShowMainWindow();
            };

            // 创建右键菜单
            var contextMenu = new WinForms.ContextMenuStrip();

            var showMenuItem = new WinForms.ToolStripMenuItem("显示主窗口");
            showMenuItem.Click += (s, e) => ShowMainWindow();
            contextMenu.Items.Add(showMenuItem);

            contextMenu.Items.Add(new WinForms.ToolStripSeparator());

            var exitMenuItem = new WinForms.ToolStripMenuItem("退出程序");
            exitMenuItem.Click += (s, e) => ExitApplication();
            contextMenu.Items.Add(exitMenuItem);

            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        /// <summary>
        /// 显示主窗口
        /// </summary>
        private void ShowMainWindow()
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
        }

        /// <summary>
        /// 退出应用程序
        /// </summary>
        private void ExitApplication()
        {
            _isReallyClosing = true;
            this.Close();
        }

        /// <summary>
        /// 闹钟触发事件处理
        /// </summary>
        private void AlarmService_AlarmTriggered(object? sender, AlarmModel alarm)
        {
            // 记录闹钟触发
            _logService.LogAlarmTriggered(alarm.Name, DateTime.Now);

            // 执行配置的动作
            if (alarm.ActionType != AlarmActionType.None)
            {
                ExecuteAlarmAction(alarm);
            }

            // 根据强提醒设置选择不同的提醒方式
            if (alarm.IsStrongAlert)
            {
                // 强提醒：全屏遮罩
                ShowStrongAlert(alarm);
            }
            else
            {
                // 弱提醒：任务栏通知
                ShowWeakAlert(alarm);
            }
        }

        /// <summary>
        /// 闹钟提醒事件处理（包括提前提醒和正式闹钟）
        /// </summary>
        private void AlarmService_AlarmReminderTriggered(object? sender, (AlarmModel Alarm, bool IsAdvanceReminder) args)
        {
            var (alarm, isAdvanceReminder) = args;

            if (isAdvanceReminder)
            {
                // 提前提醒：总是使用弱提醒方式
                ShowAdvanceReminder(alarm);
            }
            else
            {
                // 正式闹钟：根据设置选择提醒方式（这里不需要处理，因为AlarmTriggered事件已经处理了）
            }
        }

        /// <summary>
        /// 显示强提醒（全屏遮罩）
        /// </summary>
        private void ShowStrongAlert(AlarmModel alarm)
        {
            // 在UI线程上显示强提醒窗口
            Dispatcher.Invoke(() =>
            {
                var alertWindow = new StrongAlertWindow(alarm);
                alertWindow.ShowDialog();
            });
        }

        /// <summary>
        /// 显示弱提醒（任务栏通知）
        /// </summary>
        private void ShowWeakAlert(AlarmModel alarm)
        {
            // 播放闹钟声音（自定义或系统默认）
            AudioService.Instance.PlayAlarmSound(alarm.CustomSoundPath, alarm.IsStrongAlert);

            // 在UI线程上显示任务栏通知
            Dispatcher.Invoke(() =>
            {
                if (_notifyIcon != null)
                {
                    _notifyIcon.BalloonTipTitle = $"闹钟提醒 - {alarm.Name}";
                    _notifyIcon.BalloonTipText = $"{alarm.Message}\n\n触发时间：{DateTime.Now:HH:mm:ss}";
                    _notifyIcon.BalloonTipIcon = WinForms.ToolTipIcon.Info;
                    _notifyIcon.ShowBalloonTip(5000); // 显示5秒
                }
            });
        }

        /// <summary>
        /// 显示提前提醒（任务栏通知）
        /// </summary>
        private void ShowAdvanceReminder(AlarmModel alarm)
        {
            // 播放提前提醒声音（自定义或系统默认）
            AudioService.Instance.PlayAdvanceReminderSound(alarm.CustomSoundPath);

            // 在UI线程上显示任务栏通知
            Dispatcher.Invoke(() =>
            {
                if (_notifyIcon != null)
                {
                    var remainingTime = alarm.AlarmTime - DateTime.Now;
                    var remainingMinutes = Math.Max(0, (int)remainingTime.TotalMinutes);

                    _notifyIcon.BalloonTipTitle = $"提前提醒 - {alarm.Name}";
                    _notifyIcon.BalloonTipText = $"距离闹钟时间还有约 {remainingMinutes} 分钟\n\n" +
                                               $"目标时间：{alarm.AlarmTime:HH:mm:ss}\n" +
                                               $"提醒消息：{alarm.Message}";
                    _notifyIcon.BalloonTipIcon = WinForms.ToolTipIcon.Warning;
                    _notifyIcon.ShowBalloonTip(3000); // 显示3秒
                }
            });
        }

        /// <summary>
        /// 执行闹钟配置的动作
        /// </summary>
        private async void ExecuteAlarmAction(AlarmModel alarm)
        {
            try
            {
                ActionExecutor? executor = alarm.ActionType switch
                {
                    AlarmActionType.OpenUrl => new UrlActionExecutor(),
                    AlarmActionType.ExecuteCommand => new CommandActionExecutor(),
                    AlarmActionType.RunPythonScript => new PythonScriptActionExecutor(),
                    _ => null
                };

                if (executor != null)
                {
                    _logService.LogInfo($"开始执行动作",
                        $"闹钟: {alarm.Name}, 动作类型: {alarm.ActionTypeText}, 参数: {alarm.ActionParameter}");

                    var result = await executor.ExecuteAsync(alarm.ActionParameter, alarm.ActionTimeoutSeconds);

                    // 记录执行结果
                    _logService.LogActionExecution(
                        alarm.Name,
                        alarm.ActionTypeText,
                        alarm.ActionParameter,
                        result.Success,
                        result.Message,
                        result.ErrorDetails,
                        result.Duration
                    );

                    // 如果执行失败，在UI线程显示错误提示
                    if (!result.Success)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(
                                $"闹钟动作执行失败！\n\n" +
                                $"闹钟：{alarm.Name}\n" +
                                $"动作：{alarm.ActionTypeText}\n" +
                                $"参数：{alarm.ActionParameter}\n\n" +
                                $"错误信息：\n{result.ErrorDetails}",
                                "动作执行失败",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.LogError("动作执行异常",
                    $"闹钟: {alarm.Name}, 动作类型: {alarm.ActionTypeText}", ex);

                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(
                        $"执行闹钟动作时发生异常！\n\n{ex.Message}",
                        "错误",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                });
            }
        }

        /// <summary>
        /// 添加闹钟按钮点击
        /// </summary>
        private void AddAlarmButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AlarmEditDialog();
            if (dialog.ShowDialog() == true)
            {
                _alarmService.AddAlarm(dialog.AlarmModel);
            }
        }

        /// <summary>
        /// 编辑闹钟按钮点击
        /// </summary>
        private void EditAlarmButton_Click(object sender, RoutedEventArgs e)
        {
            if (AlarmListView.SelectedItem is AlarmModel selectedAlarm)
            {
                var dialog = new AlarmEditDialog(selectedAlarm);
                dialog.ShowDialog();
            }
        }

        /// <summary>
        /// 删除闹钟按钮点击
        /// </summary>
        private void DeleteAlarmButton_Click(object sender, RoutedEventArgs e)
        {
            if (AlarmListView.SelectedItem is AlarmModel selectedAlarm)
            {
                var result = MessageBox.Show(
                    $"确定要删除闹钟 \"{selectedAlarm.Name}\" 吗？",
                    "确认删除",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _alarmService.RemoveAlarm(selectedAlarm);
                }
            }
        }

        /// <summary>
        /// 测试闹钟按钮点击
        /// </summary>
        private void TestAlarmButton_Click(object sender, RoutedEventArgs e)
        {
            if (AlarmListView.SelectedItem is AlarmModel selectedAlarm)
            {
                AlarmService_AlarmTriggered(this, selectedAlarm);
            }
        }

        /// <summary>
        /// 闹钟列表双击事件
        /// </summary>
        private void AlarmListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (AlarmListView.SelectedItem is AlarmModel selectedAlarm)
            {
                var dialog = new AlarmEditDialog(selectedAlarm);
                dialog.ShowDialog();
            }
        }

        /// <summary>
        /// 闹钟列表选择改变
        /// </summary>
        private void AlarmListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool hasSelection = AlarmListView.SelectedItem != null;
            EditAlarmButton.IsEnabled = hasSelection;
            DeleteAlarmButton.IsEnabled = hasSelection;
            TestAlarmButton.IsEnabled = hasSelection;
        }

        /// <summary>
        /// 闹钟启用状态改变
        /// </summary>
        private void AlarmEnabled_Changed(object sender, RoutedEventArgs e)
        {
            // 状态已通过数据绑定自动更新
            SaveAlarms();
        }

        /// <summary>
        /// 闹钟集合变化事件
        /// </summary>
        private void Alarms_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // 添加新闹钟时，为其添加属性变化监听
            if (e.NewItems != null)
            {
                foreach (AlarmModel alarm in e.NewItems)
                {
                    alarm.PropertyChanged += Alarm_PropertyChanged;
                }
            }

            // 移除闹钟时，取消属性变化监听
            if (e.OldItems != null)
            {
                foreach (AlarmModel alarm in e.OldItems)
                {
                    alarm.PropertyChanged -= Alarm_PropertyChanged;
                }
            }

            // 保存数据
            SaveAlarms();
        }

        /// <summary>
        /// 闹钟属性变化事件
        /// </summary>
        private void Alarm_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // 闹钟属性变化时自动保存
            SaveAlarms();
        }

        /// <summary>
        /// 保存闹钟数据
        /// </summary>
        private void SaveAlarms()
        {
            _dataService.SaveAlarms(_alarms);
        }

        /// <summary>
        /// 更新开机启动状态显示
        /// </summary>
        private void UpdateStartupStatus()
        {
            bool isEnabled = _startupService.IsStartupEnabled();
            StartupCheckBox.IsChecked = isEnabled;
        }

        /// <summary>
        /// 更新托盘设置状态显示
        /// </summary>
        private void UpdateTraySettings()
        {
            MinimizeToTrayCheckBox.IsChecked = _appSettings.MinimizeToTray;
        }

        /// <summary>
        /// 开机启动复选框改变
        /// </summary>
        private void StartupCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (StartupCheckBox.IsChecked.HasValue)
            {
                bool success = _startupService.SetStartup(StartupCheckBox.IsChecked.Value);

                if (!success)
                {
                    MessageBox.Show(
                        "设置开机启动失败，请检查是否有足够的权限。",
                        "错误",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    // 恢复原状态
                    StartupCheckBox.IsChecked = _startupService.IsStartupEnabled();
                }
            }
        }

        /// <summary>
        /// 最小化到托盘复选框改变
        /// </summary>
        private void MinimizeToTrayCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (MinimizeToTrayCheckBox.IsChecked.HasValue)
            {
                _appSettings.MinimizeToTray = MinimizeToTrayCheckBox.IsChecked.Value;
            }
        }

        /// <summary>
        /// 查看数据文件按钮点击
        /// </summary>
        private void ViewDataFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string dataPath = _dataService.DataFilePath;
                string? directory = System.IO.Path.GetDirectoryName(dataPath);

                if (!string.IsNullOrEmpty(directory))
                {
                    System.Diagnostics.Process.Start("explorer.exe", directory);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"打开数据目录失败：{ex.Message}",
                    "错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 查看日志按钮点击
        /// </summary>
        private void ViewLogsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 打开日志目录
                string logDirectory = _logService.LogDirectory;
                System.Diagnostics.Process.Start("explorer.exe", logDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"打开日志目录失败：{ex.Message}",
                    "错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 窗口关闭事件（处理最小化到托盘）
        /// </summary>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            // 如果不是真正关闭，且启用了托盘功能，则最小化到托盘
            if (!_isReallyClosing && _appSettings.MinimizeToTray)
            {
                e.Cancel = true;
                this.Hide();

                // 显示气泡提示
                if (_notifyIcon != null)
                {
                    _notifyIcon.BalloonTipTitle = "高级闹钟";
                    _notifyIcon.BalloonTipText = "程序已最小化到系统托盘，双击图标可打开窗口。";
                    _notifyIcon.BalloonTipIcon = WinForms.ToolTipIcon.Info;
                    _notifyIcon.ShowBalloonTip(3000);
                }
            }
        }

        /// <summary>
        /// 窗口关闭时停止服务
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // 保存数据
            SaveAlarms();

            // 停止服务
            _alarmService.Stop();
            _clockTimer.Stop();

            // 关闭日志服务
            _logService.Close();

            // 清理任务栏图标
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
            }
        }
    }
}
