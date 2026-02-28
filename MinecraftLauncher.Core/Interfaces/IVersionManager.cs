namespace MinecraftLauncher.Core.Interfaces;

/// <summary>
/// Manages Minecraft version installation and detection
/// </summary>
public interface IVersionManager
{
    /// <summary>
    /// Checks if a specific Minecraft version is installed
    /// </summary>
    /// <param name="version">The version to check (e.g., "1.21.1")</param>
    /// <returns>True if the version is installed, false otherwise</returns>
    bool IsVersionInstalled(string version);
    
    /// <summary>
    /// Installs a specific Minecraft version from Mojang API
    /// </summary>
    /// <param name="version">The version to install</param>
    /// <param name="progress">Optional progress reporter</param>
    /// <returns>True if installation succeeded, false otherwise</returns>
    Task<bool> InstallVersionAsync(string version, IProgress<int>? progress = null);
    
    /// <summary>
    /// Gets a list of all installed Minecraft versions
    /// </summary>
    /// <returns>List of installed version strings</returns>
    List<string> GetInstalledVersions();
    
    /// <summary>
    /// Gets the directory path for a specific version
    /// </summary>
    /// <param name="version">The version</param>
    /// <returns>Full path to the version directory</returns>
    string GetVersionDirectory(string version);
}
