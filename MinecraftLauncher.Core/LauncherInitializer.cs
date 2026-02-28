using MinecraftLauncher.Core.Logging;
using Serilog;

namespace MinecraftLauncher.Core;

/// <summary>
/// Handles launcher initialization and setup
/// </summary>
public static class LauncherInitializer
{
    private static bool _isInitialized = false;
    
    /// <summary>
    /// Initializes the launcher: creates directories, sets up logging
    /// </summary>
    public static void Initialize()
    {
        if (_isInitialized)
            return;
            
        // Create all required directories
        LauncherPaths.EnsureDirectoriesExist();
        
        // Initialize logging
        LauncherLogger.InitializeGlobalLogger();
        
        Log.Information("Launcher initialized successfully");
        Log.Information("Root directory: {RootDirectory}", LauncherPaths.RootDirectory);
        
        _isInitialized = true;
    }
    
    /// <summary>
    /// Shuts down the launcher gracefully
    /// </summary>
    public static void Shutdown()
    {
        if (!_isInitialized)
            return;
            
        Log.Information("Launcher shutting down");
        LauncherLogger.CloseLogger();
        
        _isInitialized = false;
    }
    
    /// <summary>
    /// Gets whether the launcher has been initialized
    /// </summary>
    public static bool IsInitialized => _isInitialized;
}
