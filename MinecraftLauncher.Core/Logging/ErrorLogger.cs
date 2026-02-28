using Serilog;

namespace MinecraftLauncher.Core.Logging;

/// <summary>
/// Provides error logging functionality using Serilog
/// </summary>
public class ErrorLogger
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the ErrorLogger class
    /// </summary>
    public ErrorLogger()
    {
        _logger = Log.Logger;
    }

    /// <summary>
    /// Initializes a new instance of the ErrorLogger class with a specific logger
    /// </summary>
    /// <param name="logger">The Serilog logger to use</param>
    public ErrorLogger(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Logs an error with timestamp, message, and stack trace
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="exception">The exception to log</param>
    public void LogError(string message, Exception? exception = null)
    {
        if (exception != null)
        {
            _logger.Error(exception, message);
        }
        else
        {
            _logger.Error(message);
        }
    }

    /// <summary>
    /// Logs a network error with URL and error details
    /// </summary>
    /// <param name="url">The URL that failed</param>
    /// <param name="errorMessage">The error message</param>
    /// <param name="exception">The exception that occurred</param>
    public void LogNetworkError(string url, string errorMessage, Exception? exception = null)
    {
        var logMessage = $"Network error for URL: {url} - {errorMessage}";
        
        if (exception != null)
        {
            _logger.Error(exception, logMessage);
        }
        else
        {
            _logger.Error(logMessage);
        }
    }

    /// <summary>
    /// Gets the logs directory path
    /// </summary>
    /// <returns>The full path to the logs directory</returns>
    public string GetLogsDirectory()
    {
        return LauncherPaths.LogsDirectory;
    }

    /// <summary>
    /// Logs an informational message
    /// </summary>
    /// <param name="message">The message to log</param>
    public void LogInformation(string message)
    {
        _logger.Information(message);
    }

    /// <summary>
    /// Logs a warning message
    /// </summary>
    /// <param name="message">The warning message</param>
    public void LogWarning(string message)
    {
        _logger.Warning(message);
    }

    /// <summary>
    /// Logs a debug message
    /// </summary>
    /// <param name="message">The debug message</param>
    public void LogDebug(string message)
    {
        _logger.Debug(message);
    }
}
