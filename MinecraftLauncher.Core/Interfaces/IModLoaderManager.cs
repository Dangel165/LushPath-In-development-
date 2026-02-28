using MinecraftLauncher.Core.Models;

namespace MinecraftLauncher.Core.Interfaces;

/// <summary>
/// Manages mod loader installation and detection (Forge and Fabric)
/// </summary>
public interface IModLoaderManager
{
    /// <summary>
    /// Checks if a specific mod loader is installed for a Minecraft version
    /// </summary>
    /// <param name="version">The Minecraft version (e.g., "1.21.1")</param>
    /// <param name="loaderType">The type of mod loader (Forge or Fabric)</param>
    /// <returns>True if the mod loader is installed, false otherwise</returns>
    Task<bool> IsModLoaderInstalledAsync(string version, ModLoaderType loaderType);
    
    /// <summary>
    /// Installs a mod loader for a specific Minecraft version
    /// </summary>
    /// <param name="version">The Minecraft version</param>
    /// <param name="loaderType">The type of mod loader to install</param>
    /// <param name="progress">Optional progress reporter for installation</param>
    /// <returns>True if installation succeeded, false otherwise</returns>
    Task<bool> InstallModLoaderAsync(string version, ModLoaderType loaderType, IProgress<int>? progress = null);
    
    /// <summary>
    /// Gets available mod loader versions for a specific Minecraft version
    /// </summary>
    /// <param name="minecraftVersion">The Minecraft version</param>
    /// <param name="loaderType">The type of mod loader</param>
    /// <returns>List of available loader versions</returns>
    Task<List<string>> GetAvailableLoaderVersionsAsync(string minecraftVersion, ModLoaderType loaderType);
}
