using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AdvancedClock.Actions
{
    /// <summary>
    /// URL动作执行器 - 在默认浏览器中打开网址
    /// </summary>
    public class UrlActionExecutor : ActionExecutor
    {
        public override string ActionTypeName => "打开网址";

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

                // 在默认浏览器中打开URL
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = parameter,
                    UseShellExecute = true
                };

                await Task.Run(() =>
                {
                    using var process = Process.Start(processStartInfo);
                    // 浏览器启动后立即返回，不需要等待进程结束
                });

                var duration = DateTime.Now - startTime;
                return CreateResult(true, $"成功打开网址：{parameter}", null, duration);
            }
            catch (Exception ex)
            {
                var duration = DateTime.Now - startTime;
                return CreateResult(false, "打开网址失败", ex.Message, duration);
            }
        }

        public override string? ValidateParameter(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                return "URL不能为空";
            }

            // 简单验证URL格式
            if (!Uri.TryCreate(parameter, UriKind.Absolute, out var uriResult))
            {
                return "URL格式无效";
            }

            if (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps)
            {
                return "URL必须以 http:// 或 https:// 开头";
            }

            return null;
        }
    }
}
