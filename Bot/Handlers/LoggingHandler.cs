using Discord;
using Microsoft.Extensions.Logging;

namespace Bot.Handlers;

public sealed class LoggingHandler
{
	private readonly ILogger<LoggingHandler> _logger;

	public LoggingHandler(ILogger<LoggingHandler> logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public Task Log(LogMessage logMessage)
	{
		var logLevel = logMessage.Severity switch
		{
			LogSeverity.Critical => LogLevel.Critical,
			LogSeverity.Error => LogLevel.Error,
			LogSeverity.Warning => LogLevel.Warning,
			LogSeverity.Info => LogLevel.Information,
			LogSeverity.Verbose => LogLevel.Debug,
			LogSeverity.Debug => LogLevel.Trace,
			_ => throw new ArgumentException(nameof(logMessage.Severity))
		};
		if (logMessage.Exception is null)
		{
			_logger.Log(logLevel, "{Source}: {Message}", logMessage.Source, logMessage.Message);
		}
		else
		{
			_logger.Log(logLevel, logMessage.Exception, "{Source} has thrown an error!", logMessage.Source);
		}


		return Task.CompletedTask;
	}
}