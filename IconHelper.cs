using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace AdvancedClock
{
    /// <summary>
    /// 图标帮助类，用于获取应用程序图标
    /// </summary>
    public static class IconHelper
    {
        /// <summary>
        /// 获取应用程序图标
        /// </summary>
        /// <returns>应用程序图标，如果获取失败则返回null</returns>
        public static Icon? GetApplicationIcon()
        {
            try
            {
                // 尝试从程序集中获取图标
                var assembly = Assembly.GetExecutingAssembly();
                var iconStream = assembly.GetManifestResourceStream("AdvancedClock.icon.ico");
                
                if (iconStream != null)
                {
                    return new Icon(iconStream);
                }
                
                // 尝试从文件系统获取图标
                var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.ico");
                if (File.Exists(iconPath))
                {
                    return new Icon(iconPath);
                }
                
                // 如果都没有找到，返回null
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"获取应用程序图标失败: {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// 获取默认的系统图标
        /// </summary>
        /// <returns>系统信息图标</returns>
        public static Icon GetDefaultIcon()
        {
            return SystemIcons.Information;
        }
    }
}