using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using Serilog;

namespace MinecraftLauncher.Core.Validators;

/// <summary>
/// Validates IP addresses and domain names for server connections
/// </summary>
public class IPValidator
{
    // IPv4 regex that doesn't allow leading zeros (except for 0 itself)
    private static readonly Regex IPv4Regex = new Regex(
        @"^(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])\.(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])\.(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])\.(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])$",
        RegexOptions.Compiled
    );

    private static readonly Regex DomainNameRegex = new Regex(
        @"^(?:[a-zA-Z0-9](?:[a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?\.)*[a-zA-Z0-9](?:[a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?$",
        RegexOptions.Compiled
    );

    private readonly ILogger _logger;

    public IPValidator(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Validates if the input string is a valid IPv4 address
    /// </summary>
    /// <param name="ipAddress">The IP address string to validate</param>
    /// <returns>True if valid IPv4 address, false otherwise</returns>
    public bool ValidateIPv4(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            _logger.Debug("IP validation failed: input is null or whitespace");
            return false;
        }

        bool isValid = IPv4Regex.IsMatch(ipAddress);
        
        if (!isValid)
        {
            _logger.Debug("IP validation failed for: {IpAddress}", ipAddress);
        }
        
        return isValid;
    }

    /// <summary>
    /// Validates if the input string is a valid domain name
    /// </summary>
    /// <param name="domainName">The domain name string to validate</param>
    /// <returns>True if valid domain name, false otherwise</returns>
    public bool ValidateDomainName(string domainName)
    {
        if (string.IsNullOrWhiteSpace(domainName))
        {
            _logger.Debug("Domain validation failed: input is null or whitespace");
            return false;
        }

        // Check length constraints
        if (domainName.Length > 253)
        {
            _logger.Debug("Domain validation failed: exceeds maximum length of 253 characters");
            return false;
        }

        // Reject if it looks like an incomplete IP address (e.g., "192.168.1" or "256.1.1.1")
        // This prevents invalid IPs from being accepted as domain names
        if (Regex.IsMatch(domainName, @"^\d+(\.\d+)*$"))
        {
            _logger.Debug("Domain validation failed: looks like an IP address pattern");
            return false;
        }

        // Check if it matches domain name pattern
        bool isValid = DomainNameRegex.IsMatch(domainName);
        
        if (!isValid)
        {
            _logger.Debug("Domain validation failed for: {DomainName}", domainName);
        }
        
        return isValid;
    }

    /// <summary>
    /// Checks if a server is reachable using ICMP ping
    /// </summary>
    /// <param name="serverAddress">The server IP address or domain name</param>
    /// <param name="timeoutMs">Timeout in milliseconds (default: 3000ms)</param>
    /// <returns>True if server is reachable, false otherwise</returns>
    public async Task<bool> IsReachable(string serverAddress, int timeoutMs = 3000)
    {
        if (string.IsNullOrWhiteSpace(serverAddress))
        {
            _logger.Warning("Reachability check failed: server address is null or whitespace");
            return false;
        }

        try
        {
            using var ping = new Ping();
            _logger.Debug("Pinging server: {ServerAddress} with timeout {TimeoutMs}ms", serverAddress, timeoutMs);
            
            var reply = await ping.SendPingAsync(serverAddress, timeoutMs);
            
            if (reply.Status == IPStatus.Success)
            {
                _logger.Information("Server {ServerAddress} is reachable (RTT: {RoundtripTime}ms)", 
                    serverAddress, reply.RoundtripTime);
                return true;
            }
            else
            {
                _logger.Warning("Server {ServerAddress} is not reachable: {Status}", 
                    serverAddress, reply.Status);
                return false;
            }
        }
        catch (PingException ex)
        {
            _logger.Error(ex, "Ping failed for server {ServerAddress}", serverAddress);
            return false;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error during reachability check for {ServerAddress}", serverAddress);
            return false;
        }
    }

    /// <summary>
    /// Validates if the input is either a valid IPv4 address or domain name
    /// </summary>
    /// <param name="serverAddress">The server address to validate</param>
    /// <returns>True if valid IPv4 or domain name, false otherwise</returns>
    public bool ValidateServerAddress(string serverAddress)
    {
        if (string.IsNullOrWhiteSpace(serverAddress))
        {
            _logger.Debug("Server address validation failed: input is null or whitespace");
            return false;
        }

        // Check if it's a valid IPv4 address
        if (ValidateIPv4(serverAddress))
        {
            _logger.Debug("Server address {ServerAddress} validated as IPv4", serverAddress);
            return true;
        }

        // Check if it's a valid domain name
        if (ValidateDomainName(serverAddress))
        {
            _logger.Debug("Server address {ServerAddress} validated as domain name", serverAddress);
            return true;
        }

        _logger.Warning("Server address validation failed: {ServerAddress} is neither valid IPv4 nor domain name", 
            serverAddress);
        return false;
    }
}
