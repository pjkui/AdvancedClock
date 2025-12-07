using System;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace AdvancedClock
{
    /// <summary>
    /// 开机启动管理服务
    /// </summary>
    public class StartupService
    {
        private const string APP_NAME = "AdvancedClock";
        private readonly string _executablePath;

        public StartupService()
        {
            // 获取当前可执行文件路径
            // 在单文件发布模式下，Assembly.Location 可能返回空字符串
            // 使用 Environment.ProcessPath 或 AppContext.BaseDirectory 作为备选
            string? processPath = Environment.ProcessPath;
            if (!string.IsNullOrEmpty(processPath))
            {
                _executablePath = processPath;
            }
            else
            {
                // 备选方案：使用 Assembly.Location 并替换扩展名
                string assemblyLocation = Assembly.GetExecutingAssembly().Location;
                if (!string.IsNullOrEmpty(assemblyLocation))
                {
                    _executablePath = assemblyLocation.Replace(".dll", ".exe");
                }
                else
                {
                    // 最后的备选方案：使用 AppContext.BaseDirectory
                    string baseDir = AppContext.BaseDirectory;
                    _executablePath = Path.Combine(baseDir, "AdvancedClock.exe");
                }
            }
        }

        /// <summary>
        /// 检查是否已设置开机启动
        /// </summary>
        /// <returns>是否已设置开机启动</returns>
        public bool IsStartupEnabled()
        {
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false);
                
                if (key != null)
                {
                    object? value = key.GetValue(APP_NAME);
                    return value != null && value.ToString() == _executablePath;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"检查开机启动状态失败: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// 设置开机启动
        /// </summary>
        /// <returns>是否设置成功</returns>
        public bool EnableStartup()
        {
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                
                if (key != null)
                {
                    key.SetValue(APP_NAME, _executablePath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"设置开机启动失败: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// 取消开机启动
        /// </summary>
        /// <returns>是否取消成功</returns>
        public bool DisableStartup()
        {
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                
                if (key != null)
                {
                    if (key.GetValue(APP_NAME) != null)
                    {
                        key.DeleteValue(APP_NAME);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"取消开机启动失败: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// 切换开机启动状态
        /// </summary>
        /// <param name="enable">是否启用</param>
        /// <returns>是否操作成功</returns>
        public bool SetStartup(bool enable)
        {
            return enable ? EnableStartup() : DisableStartup();
        }
    }
}
