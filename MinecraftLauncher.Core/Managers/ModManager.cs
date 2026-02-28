using MinecraftLauncher.Core.Interfaces;
using MinecraftLauncher.Core.Models;
using Serilog;
using System.Text.Json;

namespace MinecraftLauncher.Core.Managers;

/// <summary>
/// Manages Minecraft mods for profiles
/// </summary>
public class ModManager : IModManager
{
    private readonly IHttpClientService _httpClient;
    private readonly IFileDownloadManager _downloadManager;
    private readonly ILogger _logger;
    
    /// <summary>
    /// Initializes a new instance of the ModManager
    /// </summary>
    public ModManager(IHttpClientService httpClient, IFileDownloadManager downloadManager)
        : this(httpClient, downloadManager, Log.Logger)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the ModManager with a custom logger
    /// </summary>
    public ModManager(IHttpClientService httpClient, IFileDownloadManager downloadManager, ILogger logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _downloadManager = downloadManager ?? throw new ArgumentNullException(nameof(downloadManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <inheritdoc/>
    public async Task<ModManifest> FetchModManifestAsync(string serverUrl)
    {
        if (string.IsNullOrWhiteSpace(serverUrl))
            throw new ArgumentException("Server URL cannot be null or empty", nameof(serverUrl));
        
        _logger.Information("Fetching mod manifest from {ServerUrl}", serverUrl);
        
        try
        {
            var manifestUrl = $"{serverUrl.TrimEnd('/')}/api/mods/manifest";
            var json = await _httpClient.GetStringAsync(manifestUrl);
            
            var manifest = JsonSerializer.Deserialize<ModManifest>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            if (manifest == null)
            {
                _logger.Error("Failed to deserialize mod manifest from {ServerUrl}", serverUrl);
                throw new InvalidOperationException("Failed to deserialize mod manifest");
            }
            
            _logger.Information("Successfully fetched mod manifest with {ModCount} mods", manifest.Mods.Count);
            return manifest;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to fetch mod manifest from {ServerUrl}", serverUrl);
            throw;
        }
    }
    
    /// <inheritdoc/>
    public async Task<bool> DownloadModAsync(ModInfo mod, string profileId, IProgress<int>? progress = null)
    {
        if (mod == null)
            throw new ArgumentNullException(nameof(mod));
        
        if (string.IsNullOrWhiteSpace(profileId))
            throw new ArgumentException("Profile ID cannot be null or empty", nameof(profileId));
        
        _logger.Information("Downloading mod {FileName} for profile {ProfileId}", mod.FileName, profileId);
        
        try
        {
            var modsDirectory = LauncherPaths.GetProfileModsDirectory(profileId);
            var targetPath = Path.Combine(modsDirectory, mod.FileName);
            
            // Ensure directory exists
            Directory.CreateDirectory(modsDirectory);
            
            // Download the mod
            var success = await _downloadManager.DownloadFileAsync(mod.DownloadUrl, targetPath, progress);
            
            if (!success)
            {
                _logger.Error("Failed to download mod {FileName}", mod.FileName);
                return false;
            }
            
            // Verify checksum
            var checksumValid = await VerifyModIntegrityAsync(mod, targetPath);
            
            if (!checksumValid)
            {
                _logger.Error("Checksum verification failed for mod {FileName}", mod.FileName);
                
                // Delete the corrupted file
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                
                return false;
            }
            
            _logger.Information("Successfully downloaded and verified mod {FileName}", mod.FileName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error downloading mod {FileName}", mod.FileName);
            return false;
        }
    }

    
    /// <inheritdoc/>
    public async Task<bool> VerifyModIntegrityAsync(ModInfo mod, string filePath)
    {
        if (mod == null)
            throw new ArgumentNullException(nameof(mod));
        
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
        
        _logger.Debug("Verifying integrity of mod {FileName}", mod.FileName);
        
        return await _downloadManager.VerifyChecksumAsync(filePath, mod.Checksum);
    }
    
    /// <inheritdoc/>
    public async Task<bool> SyncModsAsync(Profile profile, IProgress<SyncProgress>? progress = null)
    {
        if (profile == null)
            throw new ArgumentNullException(nameof(profile));
        
        _logger.Information("Starting mod sync for profile {ProfileName}", profile.Name);
        
        try
        {
            // Stage 1: Fetch manifest
            progress?.Report(new SyncProgress
            {
                Stage = SyncStage.FetchingManifest,
                CurrentMod = "Fetching mod list from server..."
            });
            
            var manifest = await FetchModManifestAsync(profile.ServerIp);
            
            // Stage 2: Compare mods
            progress?.Report(new SyncProgress
            {
                Stage = SyncStage.ComparingMods,
                CurrentMod = "Comparing local mods with server..."
            });
            
            var installedMods = GetInstalledMods(profile.Id);
            var requiredModNames = manifest.Mods.Select(m => m.FileName).ToHashSet();
            var installedModNames = installedMods.ToHashSet();
            
            // Identify obsolete mods (installed but not in manifest)
            var obsoleteMods = installedModNames.Except(requiredModNames).ToList();
            
            // Identify new mods (in manifest but not installed)
            var newMods = manifest.Mods.Where(m => !installedModNames.Contains(m.FileName)).ToList();
            
            // Identify mods that need updating (installed but checksum doesn't match)
            var modsToUpdate = new List<ModInfo>();
            foreach (var mod in manifest.Mods.Where(m => installedModNames.Contains(m.FileName)))
            {
                var modPath = Path.Combine(LauncherPaths.GetProfileModsDirectory(profile.Id), mod.FileName);
                var isValid = await VerifyModIntegrityAsync(mod, modPath);
                
                if (!isValid)
                {
                    modsToUpdate.Add(mod);
                }
            }
            
            var totalOperations = obsoleteMods.Count + newMods.Count + modsToUpdate.Count;
            var completedOperations = 0;
            
            // Stage 3: Delete obsolete mods
            if (obsoleteMods.Any())
            {
                progress?.Report(new SyncProgress
                {
                    Stage = SyncStage.DeletingObsolete,
                    TotalMods = totalOperations,
                    ProcessedMods = completedOperations,
                    CurrentMod = $"Deleting {obsoleteMods.Count} obsolete mod(s)..."
                });
                
                foreach (var modName in obsoleteMods)
                {
                    await DeleteModAsync(profile.Id, modName);
                    completedOperations++;
                    
                    progress?.Report(new SyncProgress
                    {
                        Stage = SyncStage.DeletingObsolete,
                        TotalMods = totalOperations,
                        ProcessedMods = completedOperations,
                        CurrentMod = $"Deleted {modName}"
                    });
                }
            }
            
            // Stage 4: Download new mods
            if (newMods.Any())
            {
                progress?.Report(new SyncProgress
                {
                    Stage = SyncStage.DownloadingNew,
                    TotalMods = totalOperations,
                    ProcessedMods = completedOperations,
                    CurrentMod = $"Downloading {newMods.Count} new mod(s)..."
                });
                
                foreach (var mod in newMods)
                {
                    var downloadProgress = new Progress<int>(p =>
                    {
                        progress?.Report(new SyncProgress
                        {
                            Stage = SyncStage.DownloadingNew,
                            TotalMods = totalOperations,
                            ProcessedMods = completedOperations,
                            CurrentMod = $"Downloading {mod.FileName} ({p}%)"
                        });
                    });
                    
                    var success = await DownloadModAsync(mod, profile.Id, downloadProgress);
                    
                    if (!success)
                    {
                        _logger.Error("Failed to download new mod {FileName}", mod.FileName);
                        return false;
                    }
                    
                    completedOperations++;
                }
            }
            
            // Stage 5: Download updated mods
            if (modsToUpdate.Any())
            {
                progress?.Report(new SyncProgress
                {
                    Stage = SyncStage.DownloadingUpdates,
                    TotalMods = totalOperations,
                    ProcessedMods = completedOperations,
                    CurrentMod = $"Updating {modsToUpdate.Count} mod(s)..."
                });
                
                foreach (var mod in modsToUpdate)
                {
                    var downloadProgress = new Progress<int>(p =>
                    {
                        progress?.Report(new SyncProgress
                        {
                            Stage = SyncStage.DownloadingUpdates,
                            TotalMods = totalOperations,
                            ProcessedMods = completedOperations,
                            CurrentMod = $"Updating {mod.FileName} ({p}%)"
                        });
                    });
                    
                    var success = await DownloadModAsync(mod, profile.Id, downloadProgress);
                    
                    if (!success)
                    {
                        _logger.Error("Failed to update mod {FileName}", mod.FileName);
                        return false;
                    }
                    
                    completedOperations++;
                }
            }
            
            // Stage 6: Final verification
            progress?.Report(new SyncProgress
            {
                Stage = SyncStage.VerifyingIntegrity,
                TotalMods = totalOperations,
                ProcessedMods = completedOperations,
                CurrentMod = "Verifying all mods..."
            });
            
            foreach (var mod in manifest.Mods)
            {
                var modPath = Path.Combine(LauncherPaths.GetProfileModsDirectory(profile.Id), mod.FileName);
                var isValid = await VerifyModIntegrityAsync(mod, modPath);
                
                if (!isValid)
                {
                    _logger.Error("Final verification failed for mod {FileName}", mod.FileName);
                    return false;
                }
            }
            
            // Stage 7: Complete
            progress?.Report(new SyncProgress
            {
                Stage = SyncStage.Complete,
                TotalMods = totalOperations,
                ProcessedMods = completedOperations,
                CurrentMod = "Sync complete!"
            });
            
            _logger.Information("Mod sync completed successfully for profile {ProfileName}", profile.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during mod sync for profile {ProfileName}", profile.Name);
            return false;
        }
    }
    
    /// <inheritdoc/>
    public List<string> GetInstalledMods(string profileId)
    {
        if (string.IsNullOrWhiteSpace(profileId))
            throw new ArgumentException("Profile ID cannot be null or empty", nameof(profileId));
        
        var modsDirectory = LauncherPaths.GetProfileModsDirectory(profileId);
        
        if (!Directory.Exists(modsDirectory))
        {
            _logger.Debug("Mods directory does not exist for profile {ProfileId}", profileId);
            return new List<string>();
        }
        
        try
        {
            var modFiles = Directory.GetFiles(modsDirectory, "*.jar")
                .Select(Path.GetFileName)
                .Where(name => !string.IsNullOrEmpty(name))
                .Cast<string>()
                .ToList();
            
            _logger.Debug("Found {ModCount} installed mods for profile {ProfileId}", modFiles.Count, profileId);
            return modFiles;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting installed mods for profile {ProfileId}", profileId);
            return new List<string>();
        }
    }
    
    /// <inheritdoc/>
    public async Task<bool> DeleteModAsync(string profileId, string modFileName)
    {
        if (string.IsNullOrWhiteSpace(profileId))
            throw new ArgumentException("Profile ID cannot be null or empty", nameof(profileId));
        
        if (string.IsNullOrWhiteSpace(modFileName))
            throw new ArgumentException("Mod file name cannot be null or empty", nameof(modFileName));
        
        _logger.Information("Deleting mod {FileName} from profile {ProfileId}", modFileName, profileId);
        
        try
        {
            var modPath = Path.Combine(LauncherPaths.GetProfileModsDirectory(profileId), modFileName);
            
            if (!File.Exists(modPath))
            {
                _logger.Warning("Mod file {FileName} does not exist for profile {ProfileId}", modFileName, profileId);
                return true; // Consider it successful if file doesn't exist
            }
            
            await Task.Run(() => File.Delete(modPath));
            
            _logger.Information("Successfully deleted mod {FileName}", modFileName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error deleting mod {FileName} from profile {ProfileId}", modFileName, profileId);
            return false;
        }
    }
}
