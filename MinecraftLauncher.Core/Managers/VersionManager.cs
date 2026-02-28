using MinecraftLauncher.Core.Interfaces;
using Serilog;
using System.Text.Json;

namespace MinecraftLauncher.Core.Managers;

/// <summary>
/// Manages Minecraft version installation and detection
/// </summary>
public class VersionManager : IVersionManager
{
    private readonly IHttpClientService _httpClient;
    private readonly IFileDownloadManager _downloadManager;
    private readonly ILogger _logger;
    
    /// <summary>
    /// Initializes a new instance of the VersionManager
    /// </summary>
    public VersionManager(IHttpClientService httpClient, IFileDownloadManager downloadManager)
        : this(httpClient, downloadManager, Log.Logger)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the VersionManager with a custom logger
    /// </summary>
    public VersionManager(IHttpClientService httpClient, IFileDownloadManager downloadManager, ILogger logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _downloadManager = downloadManager ?? throw new ArgumentNullException(nameof(downloadManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <inheritdoc/>
    public bool IsVersionInstalled(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Version cannot be null or empty", nameof(version));
        
        // Check custom launcher path first
        var versionDir = GetVersionDirectory(version);
        var versionJsonPath = Path.Combine(versionDir, $"{version}.json");
        
        if (File.Exists(versionJsonPath))
        {
            _logger.Debug("Version {Version} found in custom launcher directory", version);
            return true;
        }
        
        // Check official Minecraft launcher path
        var officialMinecraftPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            ".minecraft", "versions", version, $"{version}.json");
        
        var isInstalledInOfficial = File.Exists(officialMinecraftPath);
        
        if (isInstalledInOfficial)
        {
            _logger.Debug("Version {Version} found in official Minecraft directory", version);
        }
        else
        {
            _logger.Debug("Version {Version} not found in any directory", version);
        }
        
        return isInstalledInOfficial;
    }
    
    /// <inheritdoc/>
    public async Task<bool> InstallVersionAsync(string version, IProgress<int>? progress = null)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Version cannot be null or empty", nameof(version));
        
        _logger.Information("Installing Minecraft version {Version}", version);
        
        try
        {
            // Check if already installed
            if (IsVersionInstalled(version))
            {
                _logger.Information("Version {Version} is already installed", version);
                return true;
            }
            
            // Fetch version manifest from Mojang
            progress?.Report(10);
            var versionManifestUrl = $"https://launchermeta.mojang.com/mc/game/version_manifest.json";
            var manifestJson = await _httpClient.GetStringAsync(versionManifestUrl);
            
            var manifest = JsonSerializer.Deserialize<JsonDocument>(manifestJson);
            if (manifest == null)
            {
                _logger.Error("Failed to deserialize version manifest");
                return false;
            }
            
            // Find the version in the manifest
            progress?.Report(20);
            var versions = manifest.RootElement.GetProperty("versions");
            JsonElement? versionInfo = null;
            
            foreach (var v in versions.EnumerateArray())
            {
                if (v.GetProperty("id").GetString() == version)
                {
                    versionInfo = v;
                    break;
                }
            }
            
            if (versionInfo == null)
            {
                _logger.Error("Version {Version} not found in Mojang manifest", version);
                return false;
            }
            
            // Download version JSON
            progress?.Report(30);
            var versionUrl = versionInfo.Value.GetProperty("url").GetString();
            if (string.IsNullOrEmpty(versionUrl))
            {
                _logger.Error("Version URL is empty for {Version}", version);
                return false;
            }
            
            var versionDir = GetVersionDirectory(version);
            Directory.CreateDirectory(versionDir);
            
            var versionJsonPath = Path.Combine(versionDir, $"{version}.json");
            var versionJsonContent = await _httpClient.GetStringAsync(versionUrl);
            await File.WriteAllTextAsync(versionJsonPath, versionJsonContent);
            
            _logger.Debug("Downloaded version JSON for {Version}", version);
            
            // Parse version JSON to get client JAR URL
            progress?.Report(50);
            var versionData = JsonSerializer.Deserialize<JsonDocument>(versionJsonContent);
            if (versionData == null)
            {
                _logger.Error("Failed to parse version JSON for {Version}", version);
                return false;
            }
            
            var downloads = versionData.RootElement.GetProperty("downloads");
            var clientInfo = downloads.GetProperty("client");
            var clientUrl = clientInfo.GetProperty("url").GetString();
            var clientSha1 = clientInfo.GetProperty("sha1").GetString();
            
            if (string.IsNullOrEmpty(clientUrl) || string.IsNullOrEmpty(clientSha1))
            {
                _logger.Error("Client download info is missing for {Version}", version);
                return false;
            }
            
            // Download client JAR
            progress?.Report(60);
            var clientJarPath = Path.Combine(versionDir, $"{version}.jar");
            
            var downloadProgress = new Progress<int>(p =>
            {
                // Map download progress (60-90%)
                var mappedProgress = 60 + (int)(p * 0.3);
                progress?.Report(mappedProgress);
            });
            
            var downloadSuccess = await _downloadManager.DownloadFileAsync(clientUrl, clientJarPath, downloadProgress);
            
            if (!downloadSuccess)
            {
                _logger.Error("Failed to download client JAR for {Version}", version);
                return false;
            }
            
            // Verify checksum
            progress?.Report(95);
            var checksumValid = await _downloadManager.VerifyChecksumAsync(clientJarPath, clientSha1);
            
            if (!checksumValid)
            {
                _logger.Error("Client JAR checksum verification failed for {Version}", version);
                
                // Delete corrupted file
                if (File.Exists(clientJarPath))
                {
                    File.Delete(clientJarPath);
                }
                
                return false;
            }
            
            progress?.Report(100);
            _logger.Information("Successfully installed Minecraft version {Version}", version);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error installing Minecraft version {Version}", version);
            return false;
        }
    }
    
    /// <inheritdoc/>
    public List<string> GetInstalledVersions()
    {
        _logger.Debug("Getting list of installed Minecraft versions");
        
        try
        {
            var versions = new HashSet<string>();
            
            // Check custom launcher directory
            var versionsDir = LauncherPaths.GetVersionsDirectory();
            
            if (Directory.Exists(versionsDir))
            {
                foreach (var versionDir in Directory.GetDirectories(versionsDir))
                {
                    var versionName = Path.GetFileName(versionDir);
                    var versionJsonPath = Path.Combine(versionDir, $"{versionName}.json");
                    
                    if (File.Exists(versionJsonPath))
                    {
                        versions.Add(versionName);
                    }
                }
            }
            
            // Check official Minecraft directory
            var officialMinecraftVersionsDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ".minecraft", "versions");
            
            if (Directory.Exists(officialMinecraftVersionsDir))
            {
                foreach (var versionDir in Directory.GetDirectories(officialMinecraftVersionsDir))
                {
                    var versionName = Path.GetFileName(versionDir);
                    var versionJsonPath = Path.Combine(versionDir, $"{versionName}.json");
                    
                    if (File.Exists(versionJsonPath))
                    {
                        versions.Add(versionName);
                    }
                }
            }
            
            var versionList = versions.ToList();
            versionList.Sort();
            
            _logger.Debug("Found {Count} installed versions (custom: {CustomCount}, official: {OfficialCount})", 
                versionList.Count, 
                Directory.Exists(versionsDir) ? Directory.GetDirectories(versionsDir).Length : 0,
                Directory.Exists(officialMinecraftVersionsDir) ? Directory.GetDirectories(officialMinecraftVersionsDir).Length : 0);
            
            return versionList;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting installed versions");
            return new List<string>();
        }
    }
    
    /// <inheritdoc/>
    public string GetVersionDirectory(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Version cannot be null or empty", nameof(version));
        
        return Path.Combine(LauncherPaths.GetVersionsDirectory(), version);
    }
}
