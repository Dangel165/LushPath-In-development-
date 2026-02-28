using MinecraftLauncher.Core.Interfaces;
using Polly;
using Polly.Retry;
using Serilog;
using System.Net;
using System.Text;

namespace MinecraftLauncher.Core.Services;

/// <summary>
/// HTTP client service with Polly retry policies for resilient network requests
/// </summary>
public class HttpClientService : IHttpClientService
{
    private readonly HttpClient _httpClient;
    private readonly ResiliencePipeline<HttpResponseMessage> _retryPipeline;
    private readonly ILogger _logger;
    
    /// <summary>
    /// Initializes a new instance of the HttpClientService with default configuration
    /// </summary>
    public HttpClientService() : this(Log.Logger)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the HttpClientService with a custom logger
    /// </summary>
    /// <param name="logger">The logger to use for logging requests and errors</param>
    public HttpClientService(ILogger logger)
    {
        _logger = logger;
        
        // Configure HttpClient with 30-second timeout
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
        
        // Configure Polly retry pipeline with exponential backoff
        _retryPipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .Handle<TaskCanceledException>()
                    .HandleResult(response => 
                        response.StatusCode == HttpStatusCode.RequestTimeout ||
                        response.StatusCode == HttpStatusCode.TooManyRequests ||
                        response.StatusCode == HttpStatusCode.ServiceUnavailable ||
                        response.StatusCode == HttpStatusCode.GatewayTimeout),
                OnRetry = args =>
                {
                    var exception = args.Outcome.Exception;
                    var result = args.Outcome.Result;
                    
                    if (exception != null)
                    {
                        _logger.Warning(
                            "HTTP request failed (attempt {AttemptNumber}/{MaxAttempts}): {ExceptionMessage}. Retrying in {RetryDelay}ms",
                            args.AttemptNumber,
                            3,
                            exception.Message,
                            args.RetryDelay.TotalMilliseconds);
                    }
                    else if (result != null)
                    {
                        _logger.Warning(
                            "HTTP request returned {StatusCode} (attempt {AttemptNumber}/{MaxAttempts}). Retrying in {RetryDelay}ms",
                            result.StatusCode,
                            args.AttemptNumber,
                            3,
                            args.RetryDelay.TotalMilliseconds);
                    }
                    
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }
    
    /// <inheritdoc/>
    public async Task<string> GetStringAsync(string url, CancellationToken cancellationToken = default)
    {
        _logger.Debug("GET request to {Url}", url);
        
        try
        {
            var response = await _retryPipeline.ExecuteAsync(async ct =>
            {
                var httpResponse = await _httpClient.GetAsync(url, ct);
                httpResponse.EnsureSuccessStatusCode();
                return httpResponse;
            }, cancellationToken);
            
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.Debug("GET request to {Url} succeeded ({ContentLength} bytes)", url, content.Length);
            return content;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "GET request to {Url} failed after all retries", url);
            throw;
        }
    }
    
    /// <inheritdoc/>
    public async Task<byte[]> GetByteArrayAsync(string url, CancellationToken cancellationToken = default)
    {
        _logger.Debug("GET request (byte array) to {Url}", url);
        
        try
        {
            var response = await _retryPipeline.ExecuteAsync(async ct =>
            {
                var httpResponse = await _httpClient.GetAsync(url, ct);
                httpResponse.EnsureSuccessStatusCode();
                return httpResponse;
            }, cancellationToken);
            
            var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            _logger.Debug("GET request to {Url} succeeded ({ContentLength} bytes)", url, content.Length);
            return content;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "GET request (byte array) to {Url} failed after all retries", url);
            throw;
        }
    }
    
    /// <inheritdoc/>
    public async Task<Stream> GetStreamAsync(string url, CancellationToken cancellationToken = default)
    {
        _logger.Debug("GET request (stream) to {Url}", url);
        
        try
        {
            var response = await _retryPipeline.ExecuteAsync(async ct =>
            {
                var httpResponse = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);
                httpResponse.EnsureSuccessStatusCode();
                return httpResponse;
            }, cancellationToken);
            
            var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            _logger.Debug("GET request (stream) to {Url} succeeded", url);
            return stream;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "GET request (stream) to {Url} failed after all retries", url);
            throw;
        }
    }
    
    /// <inheritdoc/>
    public async Task<string> PostJsonAsync(string url, string jsonContent, CancellationToken cancellationToken = default)
    {
        _logger.Debug("POST request to {Url} with {ContentLength} bytes", url, jsonContent.Length);
        
        try
        {
            var response = await _retryPipeline.ExecuteAsync(async ct =>
            {
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var httpResponse = await _httpClient.PostAsync(url, content, ct);
                httpResponse.EnsureSuccessStatusCode();
                return httpResponse;
            }, cancellationToken);
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.Debug("POST request to {Url} succeeded ({ResponseLength} bytes)", url, responseContent.Length);
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "POST request to {Url} failed after all retries", url);
            throw;
        }
    }
}
