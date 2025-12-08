using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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
        private static Mutex? _mutex;
        private const string MutexName = "AdvancedClock_SingleInstance_Mutex";

        protected override void OnStartup(StartupEventArgs e)
        {
            // 检查单实例运行
            if (!EnsureSingleInstance())
            {
                // 如果已有实例在运行，退出当前实例
                Current.Shutdown();
                return;
            }

            // 设置全局异常处理
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            base.OnStartup(e);
        }

        /// <summary>
        /// 确保单实例运行
        /// </summary>
        /// <returns>如果是第一个实例返回true，否则返回false</returns>
        private bool EnsureSingleInstance()
        {
            try
            {
                // 尝试创建互斥锁
                _mutex = new Mutex(true, MutexName, out bool createdNew);
                
                if (!createdNew)
                {
                    // 如果互斥锁已存在，说明已有实例在运行
                    // 杀掉其他同名进程
                    KillOtherInstances();
                    
                    // 重新尝试创建互斥锁
                    _mutex?.Dispose();
                    _mutex = new Mutex(true, MutexName, out createdNew);
                    
                    if (!createdNew)
                    {
                        // 如果仍然无法创建，可能是权限问题或其他原因
                        return false;
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"单实例检查失败: {ex.Message}");
                return true; // 出错时允许启动，避免无法启动程序
            }
        }

        /// <summary>
        /// 杀掉其他同名进程实例
        /// </summary>
        private void KillOtherInstances()
        {
            try
            {
                var currentProcess = Process.GetCurrentProcess();
                var currentProcessName = currentProcess.ProcessName;
                var currentProcessId = currentProcess.Id;
                
                // 获取所有同名进程
                var processes = Process.GetProcessesByName(currentProcessName);
                
                foreach (var process in processes)
                {
                    try
                    {
                        // 跳过当前进程
                        if (process.Id == currentProcessId)
                            continue;
                        
                        // 检查是否是同一个可执行文件
                        if (IsSameExecutable(process, currentProcess))
                        {
                            System.Diagnostics.Debug.WriteLine($"正在终止进程: {process.ProcessName} (PID: {process.Id})");
                            
                            // 尝试优雅关闭
                            if (!process.CloseMainWindow())
                            {
                                // 如果无法优雅关闭，强制终止
                                process.Kill();
                            }
                            
                            // 等待进程退出
                            if (!process.WaitForExit(3000)) // 等待3秒
                            {
                                // 如果3秒后仍未退出，强制终止
                                if (!process.HasExited)
                                {
                                    process.Kill();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"终止进程失败: {ex.Message}");
                    }
                    finally
                    {
                        process.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"杀掉其他实例失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 检查两个进程是否是同一个可执行文件
        /// </summary>
        /// <param name="process1">进程1</param>
        /// <param name="process2">进程2</param>
        /// <returns>如果是同一个可执行文件返回true</returns>
        private bool IsSameExecutable(Process process1, Process process2)
        {
            try
            {
                var path1 = process1.MainModule?.FileName;
                var path2 = process2.MainModule?.FileName;
                
                if (string.IsNullOrEmpty(path1) || string.IsNullOrEmpty(path2))
                    return false;
                
                return string.Equals(path1, path2, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                // 如果无法获取路径（权限问题等），则认为是同一个程序
                return true;
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // 释放互斥锁
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
            
            base.OnExit(e);
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