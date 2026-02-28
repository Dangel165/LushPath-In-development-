namespace MinecraftLauncher.Core.Models;

/// <summary>
/// Represents the Mojang version manifest
/// </summary>
public class VersionManifest
{
    /// <summary>
    /// Latest release version
    /// </summary>
    public LatestVersions? Latest { get; set; }
    
    /// <summary>
    /// List of all available versions
    /// </summary>
    public List<VersionInfo> Versions { get; set; } = new();
}

/// <summary>
/// Latest version information
/// </summary>
public class LatestVersions
{
    /// <summary>
    /// Latest release version
    /// </summary>
    public string Release { get; set; } = string.Empty;
    
    /// <summary>
    /// Latest snapshot version
    /// </summary>
    public string Snapshot { get; set; } = string.Empty;
}

/// <summary>
/// Information about a specific Minecraft version
/// </summary>
public class VersionInfo
{
    /// <summary>
    /// Version ID (e.g., "1.21.1")
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Version type (release, snapshot, etc.)
    /// </summary>
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// URL to version JSON
    /// </summary>
    public string Url { get; set; } = string.Empty;
    
    /// <summary>
    /// Release time
    /// </summary>
    public DateTime ReleaseTime { get; set; }
}
