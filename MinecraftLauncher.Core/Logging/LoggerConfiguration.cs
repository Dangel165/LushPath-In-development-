using Serilog;
using Serilog.Events;

namespace MinecraftLauncher.Core.Logging;

/// <summary>
/// Configures Serilog for the launcher with file rotation
/// </summary>
public static class LauncherLogger
{
    /// <summary>
    /// Configures and returns a Serilog logger with file sink and rotation
    /// </summary>
    /// <returns>Configured ILogger instance</returns>
    public static ILogger CreateLogger()
    {
        // Ensure logs directory exists
        LauncherPaths.EnsureDirectoriesExist();
        
        var logFilePath = Path.Combine(LauncherPaths.LogsDirectory, "launcher-.log");
        
        return new Serilog.LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                path: logFilePath,
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: 10_485_760, // 10 MB
                rollOnFileSizeLimit: true,
                retainedFileCountLimit: 31, // Keep logs for 31 days
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
            )
            .CreateLogger();
    }
    
    /// <summary>
    /// Initializes the global Serilog logger
    /// </summary>
    public static void InitializeGlobalLogger()
    {
        Log.Logger = CreateLogger();
        Log.Information("Minecraft Launcher started");
    }
    
    /// <summary>
    /// Closes and flushes the global logger
    /// </summary>
    public static void CloseLogger()
    {
        Log.Information("Minecraft Launcher shutting down");
        Log.CloseAndFlush();
    }
}
