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
                
                // 尝试多种可能的资源名称
                string[] possibleNames = {
                    "AdvancedClock.icon.ico",
                    "icon.ico",
                    "AdvancedClock.Resources.icon.ico"
                };
                
                foreach (var resourceName in possibleNames)
                {
                    var iconStream = assembly.GetManifestResourceStream(resourceName);
                    if (iconStream != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"成功从资源加载图标: {resourceName}");
                        return new Icon(iconStream);
                    }
                }
                
                // 调试：列出所有嵌入资源
                var resourceNames = assembly.GetManifestResourceNames();
                System.Diagnostics.Debug.WriteLine($"可用的嵌入资源: {string.Join(", ", resourceNames)}");
                
                // 尝试从文件系统获取图标
                var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.ico");
                if (File.Exists(iconPath))
                {
                    System.Diagnostics.Debug.WriteLine($"从文件系统加载图标: {iconPath}");
                    return new Icon(iconPath);
                }
                
                System.Diagnostics.Debug.WriteLine("未找到任何图标文件");
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