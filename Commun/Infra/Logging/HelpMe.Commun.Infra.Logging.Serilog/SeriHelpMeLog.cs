using HelpMe.Commun.Infra.Logging.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Diagnostics;

namespace HelpMe.Commun.Infra.Logging.Serilog
{
    public class SeriHelpMeLog<TCategoryName> : IHelpMeLog<TCategoryName>
    {
        protected readonly ILogger<TCategoryName> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private static string GetCallingMethod() => new StackTrace().GetFrame(3).GetMethod().Name;
        public SeriHelpMeLog(ILogger<TCategoryName> logger, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
        }
        public void Log(Action action)
        {
            string ip = $"({_contextAccessor?.HttpContext.Connection.RemoteIpAddress.ToString() ?? "unknown"})";
            using (LogContext.PushProperty("Method", GetCallingMethod()))
            {
                using (LogContext.PushProperty("ip", ip))
                {
                    using (LogContext.PushProperty("UserId", _contextAccessor?.HttpContext?.User?.Identity?.Name ?? "anonyme"))
                    {
                        action();
                    }
                }
            }
        }

        public void LogTrace(string message) => Log(() => _logger.LogTrace(message));
        public void LogDebug(string message) => Log(() => _logger.LogDebug(message));
        public void LogInformation(string message) => Log(() => _logger.LogInformation(message));
        public void LogWarning(string message) => Log(() => _logger.LogWarning(message));
        public void LogError(string message) => Log(() => _logger.LogError(message));
        public void LogError(Exception ex, string message) => Log(() => _logger.LogError(ex, message));
        public void LogCritical(string message) => Log(() => _logger.LogCritical(message));
        public void LogCritical(Exception ex, string message) => Log(() => _logger.LogCritical(ex, message));


    }
}