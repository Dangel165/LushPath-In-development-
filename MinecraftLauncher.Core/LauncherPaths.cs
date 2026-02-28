using System;
using System.IO;

namespace MinecraftLauncher.Core;

/// <summary>
/// Manages all file system paths used by the launcher
/// </summary>
public static class LauncherPaths
{
    private static string? _testRootDirectory = null;
    
    /// <summary>
    /// Sets a custom root directory for testing purposes.
    /// This should only be called from test code.
    /// </summary>
    public static void SetTestRootDirectory(string? testDirectory)
    {
        _testRootDirectory = testDirectory;
    }
    
    private static string AppDataPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    
    /// <summary>
    /// Root directory for all launcher data: %AppData%/MinecraftLauncher
    /// In test mode, uses the test directory instead.
    /// </summary>
    public static string RootDirectory => _testRootDirectory ?? Path.Combine(AppDataPath, "MinecraftLauncher");
    
    /// <summary>
    /// Directory for profile configurations
    /// </summary>
    public static string ProfilesDirectory => Path.Combine(RootDirectory, "profiles");
    
    /// <summary>
    /// Directory for downloaded mods
    /// </summary>
    public static string ModsDirectory => Path.Combine(RootDirectory, "mods");
    
    /// <summary>
    /// Directory for Minecraft versions
    /// </summary>
    public static string VersionsDirectory => Path.Combine(RootDirectory, "versions");
    
    /// <summary>
    /// Directory for log files
    /// </summary>
    public static string LogsDirectory => Path.Combine(RootDirectory, "logs");
    
    /// <summary>
    /// Directory for cached data (skins, announcements, statistics)
    /// </summary>
    public static string CacheDirectory => Path.Combine(RootDirectory, "cache");
    
    /// <summary>
    /// Main configuration file path
    /// </summary>
    public static string ConfigFilePath => Path.Combine(RootDirectory, "config.json");
    
    /// <summary>
    /// Friend list file path
    /// </summary>
    public static string FriendsFilePath => Path.Combine(RootDirectory, "friends.json");
    
    /// <summary>
    /// Gets the mods directory for a specific profile
    /// </summary>
    public static string GetProfileModsDirectory(string profileId)
    {
        return Path.Combine(ProfilesDirectory, profileId, "mods");
    }
    
    /// <summary>
    /// Gets the customization directory for a specific profile
    /// </summary>
    public static string GetProfileCustomizationDirectory(string profileId)
    {
        return Path.Combine(ProfilesDirectory, profileId, "customization");
    }
    
    /// <summary>
    /// Gets the versions directory
    /// </summary>
    public static string GetVersionsDirectory()
    {
        return VersionsDirectory;
    }
    
    /// <summary>
    /// Ensures all required directories exist
    /// </summary>
    public static void EnsureDirectoriesExist()
    {
        Directory.CreateDirectory(RootDirectory);
        Directory.CreateDirectory(ProfilesDirectory);
        Directory.CreateDirectory(ModsDirectory);
        Directory.CreateDirectory(VersionsDirectory);
        Directory.CreateDirectory(LogsDirectory);
        Directory.CreateDirectory(CacheDirectory);
    }
}
