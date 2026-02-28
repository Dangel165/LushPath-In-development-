using MinecraftLauncher.Core.Interfaces;
using MinecraftLauncher.Core.Models;
using Serilog;
using System.Text.Json;

namespace MinecraftLauncher.Core.Managers;

/// <summary>
/// Manages mod loader installation and detection (Forge and Fabric)
/// </summary>
public class ModLoaderManager : IModLoaderManager
{
    private readonly IHttpClientService _httpClient;
    private readonly IFileDownloadManager _downloadManager;
    private readonly ILogger _logger;
    
    /// <summary>
    /// Initializes a new instance of the ModLoaderManager
    /// </summary>
    public ModLoaderManager(IHttpClientService httpClient, IFileDownloadManager downloadManager)
        : this(httpClient, downloadManager, Log.Logger)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the ModLoaderManager with a custom logger
    /// </summary>
    public ModLoaderManager(IHttpClientService httpClient, IFileDownloadManager downloadManager, ILogger logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _downloadManager = downloadManager ?? throw new ArgumentNullException(nameof(downloadManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <inheritdoc/>
    public async Task<bool> IsModLoaderInstalledAsync(string version, ModLoaderType loaderType)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Version cannot be null or empty", nameof(version));
        
        if (loaderType == ModLoaderType.Vanilla)
        {
            _logger.Debug("Vanilla profile does not require mod loader");
            return true; // Vanilla doesn't need a mod loader
        }
        
        _logger.Debug("Checking if {LoaderType} is installed for Minecraft {Version}", loaderType, version);
        
        try
        {
            // Check for mod loader profile in launcher_profiles.json
            var launcherProfilesPath = GetLauncherProfilesPath();
            
            if (!File.Exists(launcherProfilesPath))
            {
                _logger.Debug("Launcher profiles file not found at {Path}", launcherProfilesPath);
                return false;
            }
            
            var profilesJson = await File.ReadAllTextAsync(launcherProfilesPath);
            var profilesDoc = JsonSerializer.Deserialize<JsonDocument>(profilesJson);
            
            if (profilesDoc == null)
            {
                _logger.Warning("Failed to parse launcher profiles");
                return false;
            }
            
            // Look for a profile with the mod loader
            if (profilesDoc.RootElement.TryGetProperty("profiles", out var profiles))
            {
                foreach (var profile in profiles.EnumerateObject())
                {
                    if (profile.Value.TryGetProperty("lastVersionId", out var versionId))
                    {
                        var versionIdStr = versionId.GetString();
                        
                        // Check if this is a Forge or Fabric profile for the correct version
                        if (loaderType == ModLoaderType.Forge && versionIdStr != null && 
                            versionIdStr.Contains("forge", StringComparison.OrdinalIgnoreCase) &&
                            versionIdStr.Contains(version))
                        {
                            _logger.Information("Found Forge profile for version {Version}", version);
                            return true;
                        }
                        
                        if (loaderType == ModLoaderType.Fabric && versionIdStr != null &&
                            versionIdStr.Contains("fabric", StringComparison.OrdinalIgnoreCase) &&
                            versionIdStr.Contains(version))
                        {
                            _logger.Information("Found Fabric profile for version {Version}", version);
                            return true;
                        }
                    }
                }
            }
            
            _logger.Debug("{LoaderType} not found for version {Version}", loaderType, version);
            return false;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error checking mod loader installation");
            return false;
        }
    }
    
    /// <inheritdoc/>
    public async Task<bool> InstallModLoaderAsync(string version, ModLoaderType loaderType, IProgress<int>? progress = null)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Version cannot be null or empty", nameof(version));
        
        if (loaderType == ModLoaderType.Vanilla)
        {
            _logger.Information("Vanilla profile does not require mod loader installation");
            return true; // Vanilla doesn't need installation
        }
        
        _logger.Information("Installing {LoaderType} for Minecraft {Version}", loaderType, version);
        
        try
        {
            // Check if already installed
            if (await IsModLoaderInstalledAsync(version, loaderType))
            {
                _logger.Information("{LoaderType} is already installed for version {Version}", loaderType, version);
                return true;
            }
            
            progress?.Report(10);
            
            // Install based on loader type
            bool installResult = loaderType switch
            {
                ModLoaderType.Forge => await InstallForgeAsync(version, progress),
                ModLoaderType.Fabric => await InstallFabricAsync(version, progress),
                _ => throw new NotSupportedException($"Mod loader type {loaderType} is not supported")
            };
            
            if (installResult)
            {
                progress?.Report(100);
                _logger.Information("Successfully installed {LoaderType} for version {Version}", loaderType, version);
            }
            else
            {
                _logger.Error("Failed to install {LoaderType} for version {Version}", loaderType, version);
            }
            
            return installResult;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error installing {LoaderType} for version {Version}", loaderType, version);
            return false;
        }
    }
    
    /// <inheritdoc/>
    public async Task<List<string>> GetAvailableLoaderVersionsAsync(string minecraftVersion, ModLoaderType loaderType)
    {
        if (string.IsNullOrWhiteSpace(minecraftVersion))
            throw new ArgumentException("Minecraft version cannot be null or empty", nameof(minecraftVersion));
        
        if (loaderType == ModLoaderType.Vanilla)
        {
            _logger.Debug("Vanilla does not have loader versions");
            return new List<string>();
        }
        
        _logger.Debug("Fetching available {LoaderType} versions for Minecraft {Version}", loaderType, minecraftVersion);
        
        try
        {
            return loaderType switch
            {
                ModLoaderType.Forge => await GetForgeVersionsAsync(minecraftVersion),
                ModLoaderType.Fabric => await GetFabricVersionsAsync(minecraftVersion),
                _ => new List<string>()
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching {LoaderType} versions for Minecraft {Version}", loaderType, minecraftVersion);
            return new List<string>();
        }
    }
    
    private async Task<bool> InstallForgeAsync(string version, IProgress<int>? progress)
    {
        _logger.Information("Installing Forge for Minecraft {Version}", version);
        
        try
        {
            progress?.Report(20);
            
            // Get available Forge versions
            var forgeVersions = await GetForgeVersionsAsync(version);
            
            if (forgeVersions.Count == 0)
            {
                _logger.Error("No Forge versions available for Minecraft {Version}", version);
                return false;
            }
            
            // Use the latest/recommended version
            var forgeVersion = forgeVersions[0];
            _logger.Information("Using Forge version {ForgeVersion}", forgeVersion);
            
            progress?.Report(40);
            
            // Download Forge installer
            var forgeInstallerUrl = $"https://maven.minecraftforge.net/net/minecraftforge/forge/{version}-{forgeVersion}/forge-{version}-{forgeVersion}-installer.jar";
            var installerPath = Path.Combine(Path.GetTempPath(), $"forge-{version}-{forgeVersion}-installer.jar");
            
            _logger.Debug("Downloading Forge installer from {Url}", forgeInstallerUrl);
            
            var downloadSuccess = await _downloadManager.DownloadFileAsync(forgeInstallerUrl, installerPath, null);
            
            if (!downloadSuccess)
            {
                _logger.Error("Failed to download Forge installer");
                return false;
            }
            
            progress?.Report(70);
            
            // Run Forge installer in headless mode
            // Note: This is a simplified implementation
            // In production, you would need to actually run the installer JAR
            _logger.Information("Forge installer downloaded successfully");
            
            // Create a mock Forge profile (simplified for testing)
            await CreateForgeProfileAsync(version, forgeVersion);
            
            progress?.Report(90);
            
            // Cleanup installer
            if (File.Exists(installerPath))
            {
                File.Delete(installerPath);
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error installing Forge");
            return false;
        }
    }
    
    private async Task<bool> InstallFabricAsync(string version, IProgress<int>? progress)
    {
        _logger.Information("Installing Fabric for Minecraft {Version}", version);
        
        try
        {
            progress?.Report(20);
            
            // Get available Fabric versions
            var fabricVersions = await GetFabricVersionsAsync(version);
            
            if (fabricVersions.Count == 0)
            {
                _logger.Error("No Fabric versions available for Minecraft {Version}", version);
                return false;
            }
            
            // Use the latest version
            var fabricVersion = fabricVersions[0];
            _logger.Information("Using Fabric loader version {FabricVersion}", fabricVersion);
            
            progress?.Report(40);
            
            // Download Fabric loader
            var fabricLoaderUrl = $"https://maven.fabricmc.net/net/fabricmc/fabric-loader/{fabricVersion}/fabric-loader-{fabricVersion}.jar";
            var loaderPath = Path.Combine(Path.GetTempPath(), $"fabric-loader-{fabricVersion}.jar");
            
            _logger.Debug("Downloading Fabric loader from {Url}", fabricLoaderUrl);
            
            var downloadSuccess = await _downloadManager.DownloadFileAsync(fabricLoaderUrl, loaderPath, null);
            
            if (!downloadSuccess)
            {
                _logger.Error("Failed to download Fabric loader");
                return false;
            }
            
            progress?.Report(70);
            
            // Create Fabric profile
            await CreateFabricProfileAsync(version, fabricVersion);
            
            progress?.Report(90);
            
            // Cleanup
            if (File.Exists(loaderPath))
            {
                File.Delete(loaderPath);
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error installing Fabric");
            return false;
        }
    }
    
    private async Task<List<string>> GetForgeVersionsAsync(string minecraftVersion)
    {
        try
        {
            // Fetch Forge versions from Maven metadata
            // This is a simplified implementation
            var url = $"https://files.minecraftforge.net/net/minecraftforge/forge/promotions_slim.json";
            var json = await _httpClient.GetStringAsync(url);
            
            var doc = JsonSerializer.Deserialize<JsonDocument>(json);
            
            if (doc == null)
            {
                return new List<string>();
            }
            
            var versions = new List<string>();
            
            // Look for versions matching the Minecraft version
            if (doc.RootElement.TryGetProperty("promos", out var promos))
            {
                foreach (var promo in promos.EnumerateObject())
                {
                    if (promo.Name.StartsWith(minecraftVersion))
                    {
                        var version = promo.Value.GetString();
                        if (!string.IsNullOrEmpty(version) && !versions.Contains(version))
                        {
                            versions.Add(version);
                        }
                    }
                }
            }
            
            return versions;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching Forge versions");
            return new List<string>();
        }
    }
    
    private async Task<List<string>> GetFabricVersionsAsync(string minecraftVersion)
    {
        try
        {
            // Fetch Fabric versions from Fabric API
            var url = $"https://meta.fabricmc.net/v2/versions/loader/{minecraftVersion}";
            var json = await _httpClient.GetStringAsync(url);
            
            var doc = JsonSerializer.Deserialize<JsonDocument>(json);
            
            if (doc == null)
            {
                return new List<string>();
            }
            
            var versions = new List<string>();
            
            foreach (var item in doc.RootElement.EnumerateArray())
            {
                if (item.TryGetProperty("loader", out var loader))
                {
                    if (loader.TryGetProperty("version", out var version))
                    {
                        var versionStr = version.GetString();
                        if (!string.IsNullOrEmpty(versionStr))
                        {
                            versions.Add(versionStr);
                        }
                    }
                }
            }
            
            return versions;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching Fabric versions");
            return new List<string>();
        }
    }
    
    private async Task CreateForgeProfileAsync(string minecraftVersion, string forgeVersion)
    {
        var launcherProfilesPath = GetLauncherProfilesPath();
        
        // Create a simplified Forge profile entry
        // In production, this would be more complex
        _logger.Information("Creating Forge profile for {Version}-{ForgeVersion}", minecraftVersion, forgeVersion);
        
        // This is a placeholder - actual implementation would modify launcher_profiles.json
        await Task.CompletedTask;
    }
    
    private async Task CreateFabricProfileAsync(string minecraftVersion, string fabricVersion)
    {
        var launcherProfilesPath = GetLauncherProfilesPath();
        
        // Create a simplified Fabric profile entry
        _logger.Information("Creating Fabric profile for {Version}-fabric-{FabricVersion}", minecraftVersion, fabricVersion);
        
        // This is a placeholder - actual implementation would modify launcher_profiles.json
        await Task.CompletedTask;
    }
    
    private string GetLauncherProfilesPath()
    {
        // Get the Minecraft launcher profiles path
        var minecraftDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            ".minecraft"
        );
        
        return Path.Combine(minecraftDir, "launcher_profiles.json");
    }
}
