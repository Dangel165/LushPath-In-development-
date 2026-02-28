using MinecraftLauncher.Core.Models;

namespace MinecraftLauncher.Core.Interfaces;

/// <summary>
/// Manages Minecraft process launching and validation
/// </summary>
public interface IMinecraftLauncher
{
    /// <summary>
    /// Launches Minecraft with the specified profile and username
    /// </summary>
    /// <param name="profile">The profile containing server and version information</param>
    /// <param name="username">The player's username</param>
    /// <returns>Launch result containing success status and process information</returns>
    Task<LaunchResult> LaunchMinecraftAsync(Profile profile, string username);
    
    /// <summary>
    /// Validates that all required files for a Minecraft version are present
    /// </summary>
    /// <param name="version">The Minecraft version to validate</param>
    /// <returns>True if installation is valid, false otherwise</returns>
    Task<bool> ValidateInstallationAsync(string version);
    
    /// <summary>
    /// Fetches the version manifest from Mojang API
    /// </summary>
    /// <returns>Version manifest containing available versions</returns>
    Task<VersionManifest?> GetVersionManifestAsync();
    
    /// <summary>
    /// Kills the currently running Minecraft process if any
    /// </summary>
    void KillMinecraftProcess();
}
