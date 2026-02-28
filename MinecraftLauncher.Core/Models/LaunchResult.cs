using System.Diagnostics;

namespace MinecraftLauncher.Core.Models;

/// <summary>
/// Result of a Minecraft launch operation
/// </summary>
public class LaunchResult
{
    /// <summary>
    /// Whether the launch was successful
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// The Minecraft process if launch succeeded
    /// </summary>
    public Process? Process { get; set; }
    
    /// <summary>
    /// Error message if launch failed
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// The command line used to launch Minecraft
    /// </summary>
    public string? CommandLine { get; set; }
}
