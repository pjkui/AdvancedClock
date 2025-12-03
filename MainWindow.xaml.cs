using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Media;
using WinForms = System.Windows.Forms;

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
        private WinForms.NotifyIcon? _notifyIcon;

        public MainWindow()
        {
            InitializeComponent();
            
            // 初始化任务栏通知图标
            InitializeNotifyIcon();

            // 初始化数据服务
            _dataService = new AlarmDataService();
            _startupService = new StartupService();

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

            // 更新开机启动状态显示
            UpdateStartupStatus();
        }

        /// <summary>
        /// 加载闹钟数据
        /// </summary>
        private void LoadAlarms()
        {
            var savedAlarms = _dataService.LoadAlarms();
            
            if (savedAlarms.Count > 0)
            {
                // 加载保存的闹钟
                foreach (var alarm in savedAlarms)
                {                    
                    // 为每个闹钟添加属性变化监听
                    alarm.PropertyChanged += Alarm_PropertyChanged;
                    _alarms.Add(alarm);
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
            // 示例1：5分钟后的一次性闹钟
            var testAlarm = new AlarmModel
            {
                Name = "测试闹钟",
                AlarmTime = DateTime.Now.AddMinutes(5),
                RepeatMode = AlarmRepeatMode.None,
                Message = "这是一个测试闹钟！",
                IsEnabled = true
            };
            testAlarm.PropertyChanged += Alarm_PropertyChanged;
            _alarms.Add(testAlarm);

            // 示例2：每天早上8点的闹钟
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
                IsEnabled = true
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
                IsEnabled = true
            };
            yearlyAlarmModel.PropertyChanged += Alarm_PropertyChanged;
            _alarms.Add(yearlyAlarmModel);
        }

        /// <summary>
        /// 更新当前时间显示
        /// </summary>
        private void UpdateCurrentTime()
        {
            CurrentTimeText.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss dddd");
        }

        /// <summary>
        /// 时钟定时器触发
        /// </summary>
        private void ClockTimer_Tick(object? sender, EventArgs e)
        {
            UpdateCurrentTime();
        }

        /// <summary>
        /// 初始化任务栏通知图标
        /// </summary>
        private void InitializeNotifyIcon()
        {
            _notifyIcon = new WinForms.NotifyIcon
            {
                Icon = System.Drawing.SystemIcons.Information,
                Visible = true,
                Text = "高级闹钟"
            };
            
            _notifyIcon.BalloonTipClicked += (s, e) =>
            {
                this.Show();
                this.WindowState = WindowState.Normal;
                this.Activate();
            };
        }

        /// <summary>
        /// 闹钟触发事件处理
        /// </summary>
        private void AlarmService_AlarmTriggered(object? sender, AlarmModel alarm)
        {
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
            // 播放系统提示音
            SystemSounds.Beep.Play();

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
            
            // 清理任务栏图标
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
            }
        }
    }
}