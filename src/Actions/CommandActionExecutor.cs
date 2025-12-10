using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdvancedClock.Actions
{
    /// <summary>
    /// 命令动作执行器 - 执行系统命令
    /// </summary>
    public class CommandActionExecutor : ActionExecutor
    {
        public override string ActionTypeName => "执行系统命令";

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

                var output = new StringBuilder();
                var error = new StringBuilder();

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {parameter}",
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
                    var result = output.Length > 0 ? output.ToString() : "命令执行成功（无输出）";
                    return CreateResult(true, result, null, duration);
                }
                else
                {
                    var errorMessage = error.Length > 0 ? error.ToString() : $"命令执行失败，退出码：{exitCode}";
                    return CreateResult(false, "命令执行失败", errorMessage, duration);
                }
            }
            catch (OperationCanceledException)
            {
                var duration = DateTime.Now - startTime;
                return CreateResult(false, "命令执行超时", $"执行时间超过 {timeoutSeconds} 秒", duration);
            }
            catch (Exception ex)
            {
                var duration = DateTime.Now - startTime;
                return CreateResult(false, "命令执行异常", ex.Message, duration);
            }
        }

        public override string? ValidateParameter(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                return "命令不能为空";
            }

            // 检查危险命令（基本安全检查）
            var lowerParam = parameter.ToLower();
            var dangerousCommands = new[] { "format", "del /s", "rd /s", "rmdir /s" };
            
            foreach (var dangerous in dangerousCommands)
            {
                if (lowerParam.Contains(dangerous))
                {
                    return $"出于安全考虑，不允许执行包含 '{dangerous}' 的命令";
                }
            }

            return null;
        }
    }
}
