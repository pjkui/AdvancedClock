using System;
using System.IO;
using System.Text.Json;

namespace AdvancedClock
{
    /// <summary>
    /// 应用程序设置类，用于管理用户配置
    /// </summary>
    public class AppSettings
    {
        private const string SettingsFileName = "settings.json";
        private readonly string _settingsFilePath;
        private bool _minimizeToTray = false;
        
        /// <summary>
        /// 是否最小化到系统托盘
        /// </summary>
        public bool MinimizeToTray 
        { 
            get => _minimizeToTray;
            set
            {
                if (_minimizeToTray != value)
                {
                    _minimizeToTray = value;
                    SaveSettings();
                }
            }
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public AppSettings()
        {
            // 获取应用程序数据目录
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appDirectory = Path.Combine(appDataPath, "AdvancedClock");
            
            // 确保目录存在
            if (!Directory.Exists(appDirectory))
            {
                Directory.CreateDirectory(appDirectory);
            }
            
            _settingsFilePath = Path.Combine(appDirectory, SettingsFileName);
            
            // 加载设置
            LoadSettings();
        }
        
        /// <summary>
        /// 加载设置
        /// </summary>
        private void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    var json = File.ReadAllText(_settingsFilePath);
                    var settings = JsonSerializer.Deserialize<AppSettingsData>(json);
                    
                    if (settings != null)
                    {
                        _minimizeToTray = settings.MinimizeToTray;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载设置失败: {ex.Message}");
                // 使用默认设置
                _minimizeToTray = false;
            }
        }
        
        /// <summary>
        /// 保存设置
        /// </summary>
        public void SaveSettings()
        {
            try
            {
                var settings = new AppSettingsData
                {
                    MinimizeToTray = this.MinimizeToTray
                };
                
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                
                File.WriteAllText(_settingsFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存设置失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 设置数据类（用于序列化）
        /// </summary>
        private class AppSettingsData
        {
            public bool MinimizeToTray { get; set; }
        }
    }
}