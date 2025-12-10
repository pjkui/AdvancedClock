using System;
using System.Threading.Tasks;

namespace AdvancedClock.Actions
{
    /// <summary>
    /// 动作执行结果
    /// </summary>
    public class ActionExecutionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ErrorDetails { get; set; }
        public DateTime ExecutionTime { get; set; }
        public TimeSpan Duration { get; set; }
    }

    /// <summary>
    /// 动作执行器抽象基类
    /// </summary>
    public abstract class ActionExecutor
    {
        /// <summary>
        /// 执行动作
        /// </summary>
        /// <param name="parameter">动作参数</param>
        /// <param name="timeoutSeconds">超时时间（秒）</param>
        /// <returns>执行结果</returns>
        public abstract Task<ActionExecutionResult> ExecuteAsync(string parameter, int timeoutSeconds);

        /// <summary>
        /// 验证参数是否有效
        /// </summary>
        /// <param name="parameter">要验证的参数</param>
        /// <returns>验证结果消息，null表示验证通过</returns>
        public abstract string? ValidateParameter(string parameter);

        /// <summary>
        /// 获取动作类型名称
        /// </summary>
        public abstract string ActionTypeName { get; }

        /// <summary>
        /// 创建执行结果
        /// </summary>
        protected ActionExecutionResult CreateResult(bool success, string message, string? errorDetails = null, TimeSpan? duration = null)
        {
            return new ActionExecutionResult
            {
                Success = success,
                Message = message,
                ErrorDetails = errorDetails,
                ExecutionTime = DateTime.Now,
                Duration = duration ?? TimeSpan.Zero
            };
        }
    }
}
