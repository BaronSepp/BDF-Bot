using Discord;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

#nullable enable
namespace Bot.Handlers
{
    public class LoggingHandler
    {
        private readonly ILogger<LoggingHandler> _logger;
        private delegate void Transform(string logMessage, params object?[] vs);

        public LoggingHandler(ILogger<LoggingHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Log(LogMessage logMessage)
        {
            Transform transform = logMessage.Severity switch
            {
                LogSeverity.Critical => _logger.LogCritical,
                LogSeverity.Error => _logger.LogError,
                LogSeverity.Warning => _logger.LogWarning,
                LogSeverity.Info => _logger.LogInformation,
                LogSeverity.Verbose => _logger.LogDebug,
                LogSeverity.Debug => _logger.LogTrace,
                _ => throw new ArgumentException(nameof(logMessage.Severity))
            };

            transform.Invoke(logMessage.Message, logMessage.Source, logMessage.Exception);

            return Task.CompletedTask;
        }
    }
}
