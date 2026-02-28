namespace MinecraftLauncher.Core.Interfaces;

/// <summary>
/// Service for making HTTP requests with retry logic and error handling
/// </summary>
public interface IHttpClientService
{
    /// <summary>
    /// Sends a GET request to the specified URL
    /// </summary>
    /// <param name="url">The URL to send the request to</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The response content as a string</returns>
    Task<string> GetStringAsync(string url, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Sends a GET request to the specified URL and returns the response as a byte array
    /// </summary>
    /// <param name="url">The URL to send the request to</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The response content as a byte array</returns>
    Task<byte[]> GetByteArrayAsync(string url, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Sends a GET request to the specified URL and returns the response stream
    /// </summary>
    /// <param name="url">The URL to send the request to</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The response content as a stream</returns>
    Task<Stream> GetStreamAsync(string url, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Sends a POST request to the specified URL with JSON content
    /// </summary>
    /// <param name="url">The URL to send the request to</param>
    /// <param name="jsonContent">The JSON content to send</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The response content as a string</returns>
    Task<string> PostJsonAsync(string url, string jsonContent, CancellationToken cancellationToken = default);
}
