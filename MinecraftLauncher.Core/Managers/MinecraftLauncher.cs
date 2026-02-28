using MinecraftLauncher.Core.Interfaces;
using MinecraftLauncher.Core.Models;
using Serilog;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.IO.Compression;

namespace MinecraftLauncher.Core.Managers;

/// <summary>
/// Manages Minecraft process launching and validation
/// </summary>
public class MinecraftLauncher : IMinecraftLauncher
{
    private readonly IVersionManager _versionManager;
    private readonly IModLoaderManager _modLoaderManager;
    private readonly IModManager _modManager;
    private readonly IHttpClientService _httpClient;
    private readonly ILogger _logger;
    private readonly ServerManager _serverManager;
    private Process? _minecraftProcess;
    
    /// <summary>
    /// Initializes a new instance of the MinecraftLauncher
    /// </summary>
    public MinecraftLauncher(
        IVersionManager versionManager,
        IModLoaderManager modLoaderManager,
        IModManager modManager,
        IHttpClientService httpClient)
        : this(versionManager, modLoaderManager, modManager, httpClient, Log.Logger)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the MinecraftLauncher with a custom logger
    /// </summary>
    public MinecraftLauncher(
        IVersionManager versionManager,
        IModLoaderManager modLoaderManager,
        IModManager modManager,
        IHttpClientService httpClient,
        ILogger logger)
    {
        _versionManager = versionManager ?? throw new ArgumentNullException(nameof(versionManager));
        _modLoaderManager = modLoaderManager ?? throw new ArgumentNullException(nameof(modLoaderManager));
        _modManager = modManager ?? throw new ArgumentNullException(nameof(modManager));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serverManager = new ServerManager(_logger);
    }
    
    /// <inheritdoc/>
    public async Task<LaunchResult> LaunchMinecraftAsync(Profile profile, string username)
    {
        if (profile == null)
            throw new ArgumentNullException(nameof(profile));
        
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty", nameof(username));
        
        _logger.Information("Launching Minecraft for profile {ProfileName} with username {Username}", 
            profile.Name, username);
        
        try
        {
            // Validate Minecraft version installation
            if (!await ValidateInstallationAsync(profile.MinecraftVersion))
            {
                var error = $"Minecraft version {profile.MinecraftVersion} is not properly installed";
                _logger.Error(error);
                return new LaunchResult
                {
                    Success = false,
                    ErrorMessage = error
                };
            }
            
            // Validate mod loader installation (if required)
            if (profile.ModLoader != ModLoaderType.Vanilla)
            {
                var modLoaderInstalled = await _modLoaderManager.IsModLoaderInstalledAsync(
                    profile.MinecraftVersion, profile.ModLoader);
                
                if (!modLoaderInstalled)
                {
                    var error = $"Mod loader {profile.ModLoader} is not installed for version {profile.MinecraftVersion}";
                    _logger.Error(error);
                    return new LaunchResult
                    {
                        Success = false,
                        ErrorMessage = error
                    };
                }
            }
            
            // Sync mods automatically (for mod servers with valid server URL)
            if (profile.ServerType == ServerType.ModServer && !string.IsNullOrEmpty(profile.ServerIp))
            {
                // Check if ServerIp looks like a valid HTTP URL for mod sync
                var isModSyncUrl = profile.ServerIp.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                                   profile.ServerIp.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
                
                if (isModSyncUrl)
                {
                    _logger.Information("Starting automatic mod synchronization for {ProfileName}", profile.Name);
                    
                    var syncProgress = new Progress<SyncProgress>(p =>
                    {
                        _logger.Information("Mod sync: {Stage} - {CurrentMod} ({ProcessedMods}/{TotalMods})", 
                            p.Stage, p.CurrentMod, p.ProcessedMods, p.TotalMods);
                    });
                    
                    try
                    {
                        var syncSuccess = await _modManager.SyncModsAsync(profile, syncProgress);
                        
                        if (!syncSuccess)
                        {
                            _logger.Warning("Failed to synchronize mods with server. Continuing with existing mods.");
                            // Don't block launch - just warn the user
                        }
                        else
                        {
                            _logger.Information("Mod synchronization completed successfully");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Warning(ex, "Error during mod synchronization. Continuing with existing mods.");
                        // Don't block launch - just log the error
                    }
                }
                else
                {
                    _logger.Debug("ServerIp is not a mod sync URL (doesn't start with http/https), skipping mod sync");
                }
                
                // Check if mods are present
                var installedMods = _modManager.GetInstalledMods(profile.Id);
                
                if (installedMods.Count == 0)
                {
                    _logger.Warning("No mods installed for mod server profile {ProfileName}. Game may not work properly on mod server.", profile.Name);
                }
                else
                {
                    _logger.Information("Found {ModCount} installed mods for profile {ProfileName}", installedMods.Count, profile.Name);
                }
            }
            
            // Add server to Minecraft server list automatically
            if (!string.IsNullOrEmpty(profile.ServerIp))
            {
                _logger.Information("Adding server {ServerIp} to Minecraft server list", profile.ServerIp);
                
                try
                {
                    // Extract server address without port for display name
                    var serverAddress = profile.ServerIp.Split(':')[0];
                    var serverName = $"{profile.Name} Server";
                    
                    var serverAdded = await _serverManager.AddOrUpdateServerAsync(serverName, profile.ServerIp);
                    
                    if (serverAdded)
                    {
                        _logger.Information("Server successfully added to server list");
                    }
                    else
                    {
                        _logger.Warning("Failed to add server to server list, user will need to add manually");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Error adding server to server list, continuing with launch");
                }
            }
            
            // Construct launch command
            var commandLine = ConstructLaunchCommand(profile, username);
            
            _logger.Debug("Launch command: {CommandLine}", commandLine);
            
            // Write command to debug file for troubleshooting
            var debugFile = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ".minecraft", "launcher_debug.txt");
            
            try
            {
                File.WriteAllText(debugFile, $"Launch Command:\njava {commandLine}\n\n" +
                    $"Working Directory: {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft")}\n" +
                    $"Profile: {profile.Name}\n" +
                    $"Version: {profile.MinecraftVersion}\n" +
                    $"Username: {username}\n" +
                    $"Time: {DateTime.Now}\n");
                _logger.Information("Debug command written to {DebugFile}", debugFile);
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Failed to write debug file");
            }
            
            // Create a batch file to run the command (easier debugging)
            var batchFile = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ".minecraft", "launch_minecraft.bat");
            
            try
            {
                var batchContent = $"@echo off\n" +
                    $"echo Starting Minecraft {profile.MinecraftVersion}...\n" +
                    $"echo.\n" +
                    $"cd /d \"{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft")}\"\n" +
                    $"start \"Minecraft {profile.MinecraftVersion}\" java {commandLine}\n";
                
                File.WriteAllText(batchFile, batchContent);
                _logger.Information("Launch batch file created at {BatchFile}", batchFile);
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Failed to create batch file");
            }
            
            // Start Minecraft process directly using java
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "java",
                Arguments = commandLine,
                UseShellExecute = false,
                CreateNoWindow = false,
                WorkingDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    ".minecraft")
            };
            
            _minecraftProcess = Process.Start(processStartInfo);
            
            if (_minecraftProcess == null)
            {
                var error = "Failed to start Minecraft process";
                _logger.Error(error);
                return new LaunchResult
                {
                    Success = false,
                    ErrorMessage = error,
                    CommandLine = commandLine
                };
            }
            
            _logger.Information("Minecraft process started with PID {ProcessId}", _minecraftProcess.Id);
            
            return new LaunchResult
            {
                Success = true,
                Process = _minecraftProcess,
                CommandLine = commandLine
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error launching Minecraft");
            return new LaunchResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
    
    /// <inheritdoc/>
    public async Task<bool> ValidateInstallationAsync(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Version cannot be null or empty", nameof(version));
        
        _logger.Debug("Validating installation for Minecraft {Version}", version);
        
        try
        {
            // Check if version is installed
            if (!_versionManager.IsVersionInstalled(version))
            {
                _logger.Warning("Minecraft version {Version} is not installed", version);
                return false;
            }
            
            // Check for required files in custom launcher directory first
            var versionDir = _versionManager.GetVersionDirectory(version);
            var versionJsonPath = Path.Combine(versionDir, $"{version}.json");
            var versionJarPath = Path.Combine(versionDir, $"{version}.jar");
            
            // If not found in custom directory, check official Minecraft directory
            if (!File.Exists(versionJsonPath) || !File.Exists(versionJarPath))
            {
                var officialVersionDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    ".minecraft", "versions", version);
                
                versionJsonPath = Path.Combine(officialVersionDir, $"{version}.json");
                versionJarPath = Path.Combine(officialVersionDir, $"{version}.jar");
                
                _logger.Debug("Checking official Minecraft directory: {Path}", officialVersionDir);
            }
            
            if (!File.Exists(versionJsonPath))
            {
                _logger.Error("Version JSON not found at {Path}", versionJsonPath);
                return false;
            }
            
            if (!File.Exists(versionJarPath))
            {
                _logger.Error("Version JAR not found at {Path}", versionJarPath);
                return false;
            }
            
            // Verify JAR file is not corrupted (basic check)
            var jarInfo = new FileInfo(versionJarPath);
            if (jarInfo.Length == 0)
            {
                _logger.Error("Version JAR is empty at {Path}", versionJarPath);
                return false;
            }
            
            _logger.Debug("Installation validation successful for Minecraft {Version}", version);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error validating installation for version {Version}", version);
            return false;
        }
    }
    
    /// <inheritdoc/>
    public async Task<VersionManifest?> GetVersionManifestAsync()
    {
        _logger.Debug("Fetching version manifest from Mojang");
        
        try
        {
            var manifestUrl = "https://launchermeta.mojang.com/mc/game/version_manifest.json";
            var manifestJson = await _httpClient.GetStringAsync(manifestUrl);
            
            var manifest = JsonSerializer.Deserialize<VersionManifest>(manifestJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            if (manifest == null)
            {
                _logger.Error("Failed to deserialize version manifest");
                return null;
            }
            
            _logger.Information("Fetched version manifest with {Count} versions", manifest.Versions.Count);
            return manifest;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching version manifest");
            return null;
        }
    }
    
    /// <inheritdoc/>
    public void KillMinecraftProcess()
    {
        if (_minecraftProcess == null || _minecraftProcess.HasExited)
        {
            _logger.Debug("No Minecraft process to kill");
            return;
        }
        
        try
        {
            var processId = _minecraftProcess.Id;
            _logger.Information("Killing Minecraft process tree starting from PID {ProcessId}", processId);
            
            // Kill the entire process tree using taskkill on Windows
            if (OperatingSystem.IsWindows())
            {
                try
                {
                    var killProcess = new ProcessStartInfo
                    {
                        FileName = "taskkill",
                        Arguments = $"/F /T /PID {processId}",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    
                    var process = Process.Start(killProcess);
                    if (process != null)
                    {
                        process.WaitForExit(5000);
                        var output = process.StandardOutput.ReadToEnd();
                        var error = process.StandardError.ReadToEnd();
                        
                        if (process.ExitCode == 0)
                        {
                            _logger.Information("Process tree killed successfully using taskkill");
                        }
                        else
                        {
                            _logger.Warning("taskkill returned exit code {ExitCode}. Output: {Output}, Error: {Error}", 
                                process.ExitCode, output, error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Failed to use taskkill, falling back to Process.Kill()");
                    
                    // Fallback: try to kill child processes manually
                    KillProcessAndChildren(processId);
                }
            }
            else
            {
                // On Linux/Mac, kill the process group
                KillProcessAndChildren(processId);
            }
            
            // Wait for process to exit
            if (!_minecraftProcess.HasExited)
            {
                _minecraftProcess.WaitForExit(5000);
            }
            
            _minecraftProcess.Dispose();
            _minecraftProcess = null;
            _logger.Information("Minecraft process killed successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error killing Minecraft process");
            
            // Try to dispose anyway
            try
            {
                _minecraftProcess?.Dispose();
                _minecraftProcess = null;
            }
            catch { }
        }
    }
    
    /// <summary>
    /// Kills a process and all its child processes
    /// </summary>
    private void KillProcessAndChildren(int pid)
    {
        try
        {
            // Find all child processes
            var childProcesses = GetChildProcesses(pid);
            
            // Kill children first
            foreach (var childPid in childProcesses)
            {
                try
                {
                    var childProcess = Process.GetProcessById(childPid);
                    _logger.Debug("Killing child process {ChildPid}", childPid);
                    childProcess.Kill();
                    childProcess.WaitForExit(2000);
                    childProcess.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Failed to kill child process {ChildPid}", childPid);
                }
            }
            
            // Kill parent process
            try
            {
                var parentProcess = Process.GetProcessById(pid);
                _logger.Debug("Killing parent process {ParentPid}", pid);
                parentProcess.Kill();
                parentProcess.WaitForExit(2000);
                parentProcess.Dispose();
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Failed to kill parent process {ParentPid}", pid);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error in KillProcessAndChildren");
        }
    }
    
    /// <summary>
    /// Gets all child process IDs for a given parent process ID
    /// </summary>
    private List<int> GetChildProcesses(int parentPid)
    {
        var childPids = new List<int>();
        
        try
        {
            if (OperatingSystem.IsWindows())
            {
                // Use WMI to find child processes on Windows
                var searcher = new System.Management.ManagementObjectSearcher(
                    $"SELECT ProcessId FROM Win32_Process WHERE ParentProcessId = {parentPid}");
                
                foreach (System.Management.ManagementObject obj in searcher.Get())
                {
                    var childPid = Convert.ToInt32(obj["ProcessId"]);
                    childPids.Add(childPid);
                    
                    // Recursively get children of children
                    childPids.AddRange(GetChildProcesses(childPid));
                }
            }
            else
            {
                // On Linux/Mac, use ps command
                var psi = new ProcessStartInfo
                {
                    FileName = "ps",
                    Arguments = $"-o pid --ppid {parentPid} --noheaders",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                
                var process = Process.Start(psi);
                if (process != null)
                {
                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    
                    foreach (var line in output.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (int.TryParse(line.Trim(), out var childPid))
                        {
                            childPids.Add(childPid);
                            
                            // Recursively get children of children
                            childPids.AddRange(GetChildProcesses(childPid));
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Failed to get child processes for PID {ParentPid}", parentPid);
        }
        
        return childPids;
    }
    
    /// <summary>
    /// Constructs the JVM launch command for Minecraft
    /// </summary>
    private string ConstructLaunchCommand(Profile profile, string username)
    {
        // Use official .minecraft directory
        var minecraftDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            ".minecraft");
        
        var versionDir = Path.Combine(minecraftDir, "versions", profile.MinecraftVersion);
        var versionJar = Path.Combine(versionDir, $"{profile.MinecraftVersion}.jar");
        var versionJsonPath = Path.Combine(versionDir, $"{profile.MinecraftVersion}.json");
        
        _logger.Debug("Using Minecraft directory: {MinecraftDir}", minecraftDir);
        _logger.Debug("Version directory: {VersionDir}", versionDir);
        
        var args = new StringBuilder();
        
        // JVM arguments
        args.Append($"-Xmx{profile.MaxMemory ?? 2048}M "); // Max memory from profile
        args.Append($"-Xms{profile.MinMemory ?? 512}M "); // Min memory from profile
        
        // Native library path - extract natives if needed
        var nativesDir = PrepareNativesDirectory(versionDir, versionJsonPath, minecraftDir, profile.MinecraftVersion);
        args.Append($"-Djava.library.path=\"{nativesDir}\" ");
        
        // Build classpath with all libraries
        var classpath = BuildClasspath(versionJsonPath, versionJar, minecraftDir);
        args.Append($"-cp \"{classpath}\" ");
        
        // Main class
        args.Append("net.minecraft.client.main.Main ");
        
        // Minecraft arguments
        args.Append($"--username \"{username}\" ");
        args.Append($"--version \"{profile.MinecraftVersion}\" ");
        args.Append($"--gameDir \"{minecraftDir}\" ");
        args.Append($"--assetsDir \"{Path.Combine(minecraftDir, "assets")}\" ");
        
        // Asset index
        var assetIndex = GetAssetIndex(versionJsonPath);
        if (!string.IsNullOrEmpty(assetIndex))
        {
            args.Append($"--assetIndex \"{assetIndex}\" ");
        }
        
        // UUID (generate from username for offline mode)
        var uuid = GenerateOfflineUUID(username);
        args.Append($"--uuid \"{uuid}\" ");
        
        // Access token (offline mode)
        args.Append("--accessToken \"0\" ");
        args.Append("--userType \"legacy\" ");
        
        // Server auto-connect arguments
        if (!string.IsNullOrWhiteSpace(profile.ServerIp))
        {
            var serverParts = profile.ServerIp.Split(':');
            var serverAddress = serverParts[0];
            var serverPort = serverParts.Length > 1 ? serverParts[1] : "25565";
            
            args.Append($"--server \"{serverAddress}\" ");
            args.Append($"--port {serverPort} ");
        }
        
        return args.ToString().Trim();
    }
    
    /// <summary>
    /// Builds the classpath including all required libraries
    /// </summary>
    private string BuildClasspath(string versionJsonPath, string versionJar, string minecraftDir)
    {
        var classpath = new List<string> { versionJar };
        
        try
        {
            if (File.Exists(versionJsonPath))
            {
                var versionJson = File.ReadAllText(versionJsonPath);
                var versionData = JsonSerializer.Deserialize<JsonElement>(versionJson);
                
                if (versionData.TryGetProperty("libraries", out var libraries))
                {
                    var libraryCount = 0;
                    var missingLibraries = new List<string>();
                    
                    foreach (var library in libraries.EnumerateArray())
                    {
                        // Check if library should be included (rules check)
                        if (library.TryGetProperty("rules", out var rules))
                        {
                            var shouldInclude = EvaluateLibraryRules(rules);
                            if (!shouldInclude)
                            {
                                continue;
                            }
                        }
                        
                        if (library.TryGetProperty("name", out var name))
                        {
                            var libPath = GetLibraryPath(name.GetString() ?? "", minecraftDir);
                            if (File.Exists(libPath))
                            {
                                classpath.Add(libPath);
                                libraryCount++;
                            }
                            else
                            {
                                missingLibraries.Add(name.GetString() ?? "");
                                _logger.Debug("Library not found: {LibraryName} at {Path}", name.GetString(), libPath);
                            }
                        }
                    }
                    
                    _logger.Information("Added {Count} libraries to classpath", libraryCount);
                    
                    if (missingLibraries.Count > 0)
                    {
                        _logger.Warning("Missing {Count} libraries. Minecraft may not launch properly.", missingLibraries.Count);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Failed to parse version JSON for classpath, using minimal classpath");
        }
        
        return string.Join(Path.PathSeparator, classpath);
    }
    
    /// <summary>
    /// Evaluates library rules to determine if library should be included
    /// </summary>
    private bool EvaluateLibraryRules(JsonElement rules)
    {
        // Default action is to allow
        var allow = true;
        
        foreach (var rule in rules.EnumerateArray())
        {
            if (rule.TryGetProperty("action", out var action))
            {
                var actionValue = action.GetString();
                var ruleMatches = true;
                
                // Check OS rule
                if (rule.TryGetProperty("os", out var os))
                {
                    if (os.TryGetProperty("name", out var osName))
                    {
                        var osNameValue = osName.GetString();
                        ruleMatches = osNameValue switch
                        {
                            "windows" => OperatingSystem.IsWindows(),
                            "linux" => OperatingSystem.IsLinux(),
                            "osx" => OperatingSystem.IsMacOS(),
                            _ => false
                        };
                    }
                }
                
                if (ruleMatches)
                {
                    allow = actionValue == "allow";
                }
            }
        }
        
        return allow;
    }
    
    /// <summary>
    /// Prepares natives directory by extracting native libraries from JARs
    /// </summary>
    private string PrepareNativesDirectory(string versionDir, string versionJsonPath, string minecraftDir, string version)
    {
        var nativesDir = Path.Combine(versionDir, $"{version}-natives");
        
        // Check if natives already exist and are not empty
        if (Directory.Exists(nativesDir))
        {
            var files = Directory.GetFiles(nativesDir, "*.dll");
            if (files.Length > 0)
            {
                _logger.Debug("Natives directory already exists with {Count} DLL files", files.Length);
                return nativesDir;
            }
            else
            {
                _logger.Warning("Natives directory exists but is empty, will re-extract");
            }
        }
        
        // Create natives directory
        try
        {
            Directory.CreateDirectory(nativesDir);
            _logger.Information("Created natives directory at {NativesDir}", nativesDir);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to create natives directory");
            return nativesDir;
        }
        
        // Extract natives from library JARs
        var extractedCount = 0;
        var librariesChecked = 0;
        var librariesWithNatives = 0;
        
        try
        {
            if (File.Exists(versionJsonPath))
            {
                var versionJson = File.ReadAllText(versionJsonPath);
                var versionData = JsonSerializer.Deserialize<JsonElement>(versionJson);
                
                if (versionData.TryGetProperty("libraries", out var libraries))
                {
                    _logger.Information("Processing {Count} libraries from version JSON", libraries.GetArrayLength());
                    
                    foreach (var library in libraries.EnumerateArray())
                    {
                        librariesChecked++;
                        
                        if (!library.TryGetProperty("name", out var nameElement))
                        {
                            continue;
                        }
                        
                        var libName = nameElement.GetString() ?? "";
                        
                        // Check if this is a natives library by looking for classifier in the name
                        // Format: group:artifact:version:classifier (e.g., org.lwjgl:lwjgl:3.3.1:natives-windows)
                        var parts = libName.Split(':');
                        if (parts.Length >= 4)
                        {
                            var classifier = parts[3];
                            
                            // Check if this is a natives library for the current OS
                            var isNativeForCurrentOS = false;
                            if (OperatingSystem.IsWindows() && classifier.Contains("natives-windows"))
                            {
                                isNativeForCurrentOS = true;
                            }
                            else if (OperatingSystem.IsLinux() && classifier.Contains("natives-linux"))
                            {
                                isNativeForCurrentOS = true;
                            }
                            else if (OperatingSystem.IsMacOS() && (classifier.Contains("natives-macos") || classifier.Contains("natives-osx")))
                            {
                                isNativeForCurrentOS = true;
                            }
                            
                            if (isNativeForCurrentOS)
                            {
                                librariesWithNatives++;
                                _logger.Debug("Library #{Index} is a native library: {LibName}", librariesChecked, libName);
                                
                                // Check rules
                                if (library.TryGetProperty("rules", out var rules))
                                {
                                    if (!EvaluateLibraryRules(rules))
                                    {
                                        _logger.Debug("Library #{Index} excluded by rules", librariesChecked);
                                        continue;
                                    }
                                }
                                
                                // Get the path from downloads.artifact.path if available
                                string? nativeJarPath = null;
                                if (library.TryGetProperty("downloads", out var downloads) &&
                                    downloads.TryGetProperty("artifact", out var artifact) &&
                                    artifact.TryGetProperty("path", out var pathElement))
                                {
                                    var relativePath = pathElement.GetString();
                                    if (!string.IsNullOrEmpty(relativePath))
                                    {
                                        nativeJarPath = Path.Combine(minecraftDir, "libraries", relativePath);
                                    }
                                }
                                
                                // Fallback: construct path from library name
                                if (string.IsNullOrEmpty(nativeJarPath))
                                {
                                    nativeJarPath = GetNativeLibraryPath(libName, classifier, minecraftDir);
                                }
                                
                                _logger.Debug("Looking for native JAR at: {Path}", nativeJarPath);
                                
                                if (File.Exists(nativeJarPath))
                                {
                                    _logger.Information("Found native JAR: {Path}", Path.GetFileName(nativeJarPath));
                                    var extracted = ExtractNativesFromJar(nativeJarPath, nativesDir);
                                    extractedCount += extracted;
                                }
                                else
                                {
                                    _logger.Warning("Native library JAR not found: {Path}", nativeJarPath);
                                }
                            }
                        }
                    }
                    
                    _logger.Information("Checked {Total} libraries, {WithNatives} had natives property, extracted {Count} native files", 
                        librariesChecked, librariesWithNatives, extractedCount);
                }
            }
            
            var extractedFiles = Directory.GetFiles(nativesDir);
            _logger.Information("Total files in natives directory: {Count}", extractedFiles.Length);
            
            if (extractedFiles.Length == 0)
            {
                _logger.Error("WARNING: No native files were extracted! Minecraft will likely fail to start.");
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to extract natives");
        }
        
        return nativesDir;
    }
    
    /// <summary>
    /// Gets the path to a native library JAR with classifier
    /// </summary>
    private string GetNativeLibraryPath(string libraryName, string classifier, string minecraftDir)
    {
        // Format: group:artifact:version
        var parts = libraryName.Split(':');
        if (parts.Length < 3) return "";
        
        var group = parts[0].Replace('.', Path.DirectorySeparatorChar);
        var artifact = parts[1];
        var version = parts[2];
        
        // Replace ${arch} placeholder with actual architecture
        classifier = classifier.Replace("${arch}", Environment.Is64BitOperatingSystem ? "64" : "32");
        
        return Path.Combine(minecraftDir, "libraries", group, artifact, version,
            $"{artifact}-{version}-{classifier}.jar");
    }
    
    /// <summary>
    /// Extracts native DLL/SO files from a JAR
    /// </summary>
    private int ExtractNativesFromJar(string jarPath, string targetDir)
    {
        var extractedCount = 0;
        
        try
        {
            using var archive = System.IO.Compression.ZipFile.OpenRead(jarPath);
            
            _logger.Debug("Opened JAR archive: {JarPath} with {Count} entries", jarPath, archive.Entries.Count);
            
            foreach (var entry in archive.Entries)
            {
                // Extract only native library files
                var isNative = entry.FullName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ||
                              entry.FullName.EndsWith(".so", StringComparison.OrdinalIgnoreCase) ||
                              entry.FullName.EndsWith(".dylib", StringComparison.OrdinalIgnoreCase);
                
                if (isNative)
                {
                    // Check if file is in root or subdirectory
                    var fileName = Path.GetFileName(entry.FullName);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var destPath = Path.Combine(targetDir, fileName);
                        
                        // Skip if already exists
                        if (File.Exists(destPath))
                        {
                            _logger.Debug("Native already exists, skipping: {FileName}", fileName);
                            continue;
                        }
                        
                        entry.ExtractToFile(destPath, overwrite: true);
                        _logger.Information("Extracted native: {FileName} from {JarPath}", fileName, Path.GetFileName(jarPath));
                        extractedCount++;
                    }
                }
            }
            
            if (extractedCount == 0)
            {
                _logger.Warning("No native files found in {JarPath}", jarPath);
            }
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Failed to extract natives from {JarPath}", jarPath);
        }
        
        return extractedCount;
    }
    
    /// <summary>
    /// Converts library name to file path
    /// </summary>
    private string GetLibraryPath(string libraryName, string minecraftDir)
    {
        // Format: group:artifact:version
        var parts = libraryName.Split(':');
        if (parts.Length < 3) return "";
        
        var group = parts[0].Replace('.', Path.DirectorySeparatorChar);
        var artifact = parts[1];
        var version = parts[2];
        
        return Path.Combine(minecraftDir, "libraries", group, artifact, version, 
            $"{artifact}-{version}.jar");
    }
    
    /// <summary>
    /// Gets the asset index from version JSON
    /// </summary>
    private string GetAssetIndex(string versionJsonPath)
    {
        try
        {
            if (File.Exists(versionJsonPath))
            {
                var versionJson = File.ReadAllText(versionJsonPath);
                var versionData = JsonSerializer.Deserialize<JsonElement>(versionJson);
                
                if (versionData.TryGetProperty("assetIndex", out var assetIndex))
                {
                    if (assetIndex.TryGetProperty("id", out var id))
                    {
                        return id.GetString() ?? "";
                    }
                }
                
                // Fallback: use version as asset index for older versions
                if (versionData.TryGetProperty("assets", out var assets))
                {
                    return assets.GetString() ?? "";
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Failed to get asset index from version JSON");
        }
        
        return "";
    }
    
    /// <summary>
    /// Generates offline UUID from username
    /// </summary>
    private string GenerateOfflineUUID(string username)
    {
        // Generate UUID v3 (MD5 hash) from username
        using var md5 = System.Security.Cryptography.MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes($"OfflinePlayer:{username}"));
        
        // Set version to 3
        hash[6] = (byte)((hash[6] & 0x0F) | 0x30);
        // Set variant to RFC 4122
        hash[8] = (byte)((hash[8] & 0x3F) | 0x80);
        
        var uuid = new Guid(hash);
        return uuid.ToString();
    }
    
    /// <summary>
    /// Handles Minecraft process exit event
    /// </summary>
    private void OnMinecraftProcessExited(object? sender, EventArgs e)
    {
        if (_minecraftProcess == null)
            return;
        
        var exitCode = _minecraftProcess.ExitCode;
        
        if (exitCode != 0)
        {
            _logger.Error("Minecraft crashed with exit code {ExitCode}", exitCode);
            
            // Check for crash reports
            var crashReportsDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ".minecraft", "crash-reports");
            
            if (Directory.Exists(crashReportsDir))
            {
                var crashReports = Directory.GetFiles(crashReportsDir, "*.txt")
                    .OrderByDescending(f => File.GetLastWriteTime(f))
                    .FirstOrDefault();
                
                if (crashReports != null)
                {
                    _logger.Error("Crash report found at {Path}", crashReports);
                }
            }
        }
        else
        {
            _logger.Information("Minecraft exited normally");
        }
        
        _minecraftProcess.Dispose();
        _minecraftProcess = null;
    }
}
