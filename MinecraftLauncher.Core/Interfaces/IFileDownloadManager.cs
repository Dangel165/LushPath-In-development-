namespace MinecraftLauncher.Core.Interfaces;

/// <summary>
/// Service for downloading files with progress reporting and integrity verification
/// </summary>
public interface IFileDownloadManager
{
    /// <summary>
    /// Downloads a file from the specified URL to the target path with progress reporting
    /// </summary>
    /// <param name="url">The URL to download from</param>
    /// <param name="targetPath">The target file path</param>
    /// <param name="progress">Progress reporter for download progress (0-100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if download succeeded, false otherwise</returns>
    Task<bool> DownloadFileAsync(
        string url, 
        string targetPath, 
        IProgress<int>? progress = null, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Verifies the checksum of a file using SHA-256
    /// </summary>
    /// <param name="filePath">The path to the file to verify</param>
    /// <param name="expectedChecksum">The expected SHA-256 checksum (hex string)</param>
    /// <returns>True if checksum matches, false otherwise</returns>
    Task<bool> VerifyChecksumAsync(string filePath, string expectedChecksum);
}
