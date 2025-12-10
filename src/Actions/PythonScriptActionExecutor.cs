using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdvancedClock.Actions
{
    /// <summary>
    /// Python脚本动作执行器 - 运行Python脚本
    /// </summary>
    public class PythonScriptActionExecutor : ActionExecutor
    {
        public override string ActionTypeName => "运行Python脚本";

        public override async Task<ActionExecutionResult> ExecuteAsync(string parameter, int timeoutSeconds)
        {
            var startTime = DateTime.Now;
            
            try
            {
                // 验证参数
                var validationError = ValidateParameter(parameter);
                if (validationError != null)
                {
                    return CreateResult(false, "参数验证失败", validationError);
                }

                // 查找Python解释器
                var pythonExe = FindPythonExecutable();
                if (pythonExe == null)
                {
                    return CreateResult(false, "未找到Python解释器", 
                        "请确保已安装Python并将其添加到系统PATH环境变量中");
                }

                var output = new StringBuilder();
                var error = new StringBuilder();

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = pythonExe,
                    Arguments = $"\"{parameter}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };

                using var process = new Process { StartInfo = processStartInfo };
                
                // 异步读取输出
                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        output.AppendLine(e.Data);
                    }
                };
                
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        error.AppendLine(e.Data);
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // 使用超时等待
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
                
                await Task.Run(() =>
                {
                    if (!process.WaitForExit(timeoutSeconds * 1000))
                    {
                        // 超时，强制终止进程
                        try
                        {
                            process.Kill(true);
                        }
                        catch { }
                    }
                }, cts.Token);

                var duration = DateTime.Now - startTime;
                var exitCode = process.ExitCode;

                if (exitCode == 0)
                {
                    var result = output.Length > 0 ? output.ToString() : "Python脚本执行成功（无输出）";
                    return CreateResult(true, result, null, duration);
                }
                else
                {
                    var errorMessage = error.Length > 0 ? error.ToString() : $"脚本执行失败，退出码：{exitCode}";
                    return CreateResult(false, "Python脚本执行失败", errorMessage, duration);
                }
            }
            catch (OperationCanceledException)
            {
                var duration = DateTime.Now - startTime;
                return CreateResult(false, "Python脚本执行超时", $"执行时间超过 {timeoutSeconds} 秒", duration);
            }
            catch (Exception ex)
            {
                var duration = DateTime.Now - startTime;
                return CreateResult(false, "Python脚本执行异常", ex.Message, duration);
            }
        }

        public override string? ValidateParameter(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                return "脚本路径不能为空";
            }

            if (!File.Exists(parameter))
            {
                return "脚本文件不存在";
            }

            var extension = Path.GetExtension(parameter).ToLower();
            if (extension != ".py")
            {
                return "脚本文件必须是 .py 格式";
            }

            return null;
        }

        /// <summary>
        /// 查找Python解释器
        /// </summary>
        private string? FindPythonExecutable()
        {
            // 尝试常见的Python命令
            var pythonCommands = new[] { "python", "python3", "py" };

            foreach (var cmd in pythonCommands)
            {
                try
                {
                    var processStartInfo = new ProcessStartInfo
                    {
                        FileName = cmd,
                        Arguments = "--version",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    };

                    using var process = Process.Start(processStartInfo);
                    if (process != null)
                    {
                        process.WaitForExit(1000);
                        if (process.ExitCode == 0)
                        {
                            return cmd;
                        }
                    }
                }
                catch
                {
                    // 继续尝试下一个命令
                }
            }

            return null;
        }
    }
}
