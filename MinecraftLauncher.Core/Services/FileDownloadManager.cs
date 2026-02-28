using MinecraftLauncher.Core.Interfaces;
using Serilog;
using System.Security.Cryptography;

namespace MinecraftLauncher.Core.Services;

/// <summary>
/// Service for downloading files with progress reporting and integrity verification
/// </summary>
public class FileDownloadManager : IFileDownloadManager
{
    private readonly IHttpClientService _httpClient;
    private readonly ILogger _logger;
    
    /// <summary>
    /// Initializes a new instance of the FileDownloadManager
    /// </summary>
    /// <param name="httpClient">The HTTP client service to use for downloads</param>
    public FileDownloadManager(IHttpClientService httpClient) : this(httpClient, Log.Logger)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the FileDownloadManager with a custom logger
    /// </summary>
    /// <param name="httpClient">The HTTP client service to use for downloads</param>
    /// <param name="logger">The logger to use for logging</param>
    public FileDownloadManager(IHttpClientService httpClient, ILogger logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <inheritdoc/>
    public async Task<bool> DownloadFileAsync(
        string url, 
        string targetPath, 
        IProgress<int>? progress = null, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be null or empty", nameof(url));
        
        if (string.IsNullOrWhiteSpace(targetPath))
            throw new ArgumentException("Target path cannot be null or empty", nameof(targetPath));
        
        _logger.Information("Starting download from {Url} to {TargetPath}", url, targetPath);
        
        try
        {
            // Create a temporary file path for atomic writes
            var tempPath = targetPath + ".tmp";
            
            // Ensure the target directory exists
            var directory = Path.GetDirectoryName(targetPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                _logger.Debug("Created directory {Directory}", directory);
            }
            
            // Download to temporary file with progress reporting
            using (var stream = await _httpClient.GetStreamAsync(url, cancellationToken))
            using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
            {
                var buffer = new byte[8192];
                long totalBytesRead = 0;
                long? totalBytes = null;
                
                // Try to get content length for progress calculation
                try
                {
                    if (stream.CanSeek)
                    {
                        totalBytes = stream.Length;
                    }
                }
                catch
                {
                    // Content length not available, progress will be indeterminate
                    _logger.Debug("Content length not available for {Url}", url);
                }
                
                int bytesRead;
                int lastReportedProgress = -1;
                
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                    totalBytesRead += bytesRead;
                    
                    // Report progress if total bytes is known
                    if (progress != null && totalBytes.HasValue && totalBytes.Value > 0)
                    {
                        var currentProgress = (int)((totalBytesRead * 100) / totalBytes.Value);
                        
                        // Only report if progress changed
                        if (currentProgress != lastReportedProgress)
                        {
                            progress.Report(currentProgress);
                            lastReportedProgress = currentProgress;
                        }
                    }
                }
                
                // Ensure 100% is reported at the end
                if (progress != null && lastReportedProgress != 100)
                {
                    progress.Report(100);
                }
            }
            
            // Atomic move: replace target file with temp file
            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }
            File.Move(tempPath, targetPath);
            
            _logger.Information("Successfully downloaded {Url} to {TargetPath}", url, targetPath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to download {Url} to {TargetPath}", url, targetPath);
            
            // Clean up temporary file if it exists
            var tempPath = targetPath + ".tmp";
            if (File.Exists(tempPath))
            {
                try
                {
                    File.Delete(tempPath);
                }
                catch (Exception cleanupEx)
                {
                    _logger.Warning(cleanupEx, "Failed to clean up temporary file {TempPath}", tempPath);
                }
            }
            
            return false;
        }
    }
    
    /// <inheritdoc/>
    public async Task<bool> VerifyChecksumAsync(string filePath, string expectedChecksum)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
        
        if (string.IsNullOrWhiteSpace(expectedChecksum))
            throw new ArgumentException("Expected checksum cannot be null or empty", nameof(expectedChecksum));
        
        if (!File.Exists(filePath))
        {
            _logger.Warning("Cannot verify checksum: file {FilePath} does not exist", filePath);
            return false;
        }
        
        _logger.Debug("Verifying checksum for {FilePath}", filePath);
        
        try
        {
            using (var sha256 = SHA256.Create())
            using (var stream = File.OpenRead(filePath))
            {
                var hashBytes = await sha256.ComputeHashAsync(stream);
                var actualChecksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                var normalizedExpected = expectedChecksum.Replace("-", "").ToLowerInvariant();
                
                var matches = actualChecksum == normalizedExpected;
                
                if (matches)
                {
                    _logger.Debug("Checksum verification succeeded for {FilePath}", filePath);
                }
                else
                {
                    _logger.Warning(
                        "Checksum verification failed for {FilePath}. Expected: {Expected}, Actual: {Actual}",
                        filePath, normalizedExpected, actualChecksum);
                }
                
                return matches;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error verifying checksum for {FilePath}", filePath);
            return false;
        }
    }
}
