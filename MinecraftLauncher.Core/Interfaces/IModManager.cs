using MinecraftLauncher.Core.Models;

namespace MinecraftLauncher.Core.Interfaces;

/// <summary>
/// Interface for managing Minecraft mods
/// </summary>
public interface IModManager
{
    /// <summary>
    /// Fetches the mod manifest from the server
    /// </summary>
    /// <param name="serverUrl">The server URL to fetch the manifest from</param>
    /// <returns>The mod manifest containing all required mods</returns>
    Task<ModManifest> FetchModManifestAsync(string serverUrl);
    
    /// <summary>
    /// Downloads a mod with progress reporting
    /// </summary>
    /// <param name="mod">The mod information</param>
    /// <param name="profileId">The profile ID to download the mod for</param>
    /// <param name="progress">Progress reporter for download progress</param>
    /// <returns>True if download was successful, false otherwise</returns>
    Task<bool> DownloadModAsync(ModInfo mod, string profileId, IProgress<int>? progress = null);
    
    /// <summary>
    /// Verifies the integrity of a mod file using checksum
    /// </summary>
    /// <param name="mod">The mod information containing the expected checksum</param>
    /// <param name="filePath">The path to the mod file</param>
    /// <returns>True if the checksum matches, false otherwise</returns>
    Task<bool> VerifyModIntegrityAsync(ModInfo mod, string filePath);
    
    /// <summary>
    /// Synchronizes mods for a profile with the server manifest
    /// </summary>
    /// <param name="profile">The profile to sync mods for</param>
    /// <param name="progress">Progress reporter for sync progress</param>
    /// <returns>True if sync was successful, false otherwise</returns>
    Task<bool> SyncModsAsync(Profile profile, IProgress<SyncProgress>? progress = null);
    
    /// <summary>
    /// Gets all installed mods for a profile
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <returns>List of installed mod file names</returns>
    List<string> GetInstalledMods(string profileId);
    
    /// <summary>
    /// Deletes a mod from a profile
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="modFileName">The mod file name to delete</param>
    /// <returns>True if deletion was successful, false otherwise</returns>
    Task<bool> DeleteModAsync(string profileId, string modFileName);
}

/// <summary>
/// Represents a mod manifest from the server
/// </summary>
public class ModManifest
{
    public List<ModInfo> Mods { get; set; } = new();
    public string Version { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// Progress information for mod synchronization
/// </summary>
public class SyncProgress
{
    public int TotalMods { get; set; }
    public int ProcessedMods { get; set; }
    public string CurrentMod { get; set; } = string.Empty;
    public SyncStage Stage { get; set; }
    public int PercentComplete => TotalMods > 0 ? (ProcessedMods * 100) / TotalMods : 0;
}

/// <summary>
/// Stages of mod synchronization
/// </summary>
public enum SyncStage
{
    FetchingManifest,
    ComparingMods,
    DeletingObsolete,
    DownloadingNew,
    DownloadingUpdates,
    VerifyingIntegrity,
    Complete
}
