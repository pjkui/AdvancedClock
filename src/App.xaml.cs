using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace AdvancedClock
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // 设置全局异常处理
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            base.OnStartup(e);
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMessage = $"应用程序遇到未处理的异常:\n\n{e.Exception.Message}\n\n详细信息:\n{e.Exception}";
                
                MessageBox.Show(errorMessage, "AdvancedClock - 错误", MessageBoxButton.OK, MessageBoxImage.Error);
                
                // 记录到调试输出
                System.Diagnostics.Debug.WriteLine($"DispatcherUnhandledException: {e.Exception}");
                
                // 标记异常已处理，防止应用程序崩溃
                e.Handled = true;
            }
            catch
            {
                // 如果异常处理本身失败，让应用程序正常崩溃
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                if (e.ExceptionObject is Exception exception)
                {
                    string errorMessage = $"应用程序遇到严重错误:\n\n{exception.Message}\n\n详细信息:\n{exception}";
                    
                    MessageBox.Show(errorMessage, "AdvancedClock - 严重错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    
                    // 记录到调试输出
                    System.Diagnostics.Debug.WriteLine($"UnhandledException: {exception}");
                }
            }
            catch
            {
                // 如果异常处理本身失败，让应用程序正常崩溃
            }
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                string errorMessage = $"后台任务遇到未处理的异常:\n\n{e.Exception.Message}\n\n详细信息:\n{e.Exception}";
                
                System.Diagnostics.Debug.WriteLine($"UnobservedTaskException: {e.Exception}");
                
                // 标记异常已处理
                e.SetObserved();
            }
            catch
            {
                // 如果异常处理本身失败，忽略
            }
        }
    }
}