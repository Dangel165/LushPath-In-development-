using fNbt;
using Serilog;

namespace MinecraftLauncher.Core.Managers;

/// <summary>
/// Manages Minecraft server list (servers.dat)
/// </summary>
public class ServerManager
{
    private readonly ILogger _logger;
    
    /// <summary>
    /// Initializes a new instance of the ServerManager
    /// </summary>
    public ServerManager(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <summary>
    /// Adds or updates a server in the Minecraft server list
    /// </summary>
    /// <param name="serverName">Display name for the server</param>
    /// <param name="serverIp">Server IP address (can include port)</param>
    /// <returns>True if successful, false otherwise</returns>
    public async Task<bool> AddOrUpdateServerAsync(string serverName, string serverIp)
    {
        if (string.IsNullOrWhiteSpace(serverName))
            throw new ArgumentException("Server name cannot be null or empty", nameof(serverName));
        
        if (string.IsNullOrWhiteSpace(serverIp))
            throw new ArgumentException("Server IP cannot be null or empty", nameof(serverIp));
        
        _logger.Information("Adding/updating server {ServerName} ({ServerIp}) to server list", serverName, serverIp);
        
        try
        {
            var minecraftDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ".minecraft");
            
            var serversDatPath = Path.Combine(minecraftDir, "servers.dat");
            
            NbtCompound root;
            NbtList serversList;
            
            // Read existing file or create new one
            if (File.Exists(serversDatPath))
            {
                _logger.Debug("Reading existing servers.dat file");
                
                try
                {
                    var file = new NbtFile(serversDatPath);
                    file.LoadFromFile(serversDatPath);
                    root = file.RootTag;
                    
                    // Get or create servers list
                    serversList = root.Get<NbtList>("servers");
                    if (serversList == null)
                    {
                        _logger.Warning("servers.dat exists but has no 'servers' list, creating new list");
                        serversList = new NbtList("servers", NbtTagType.Compound);
                        root.Add(serversList);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Failed to read existing servers.dat, creating new file");
                    root = new NbtCompound("");
                    serversList = new NbtList("servers", NbtTagType.Compound);
                    root.Add(serversList);
                }
            }
            else
            {
                _logger.Debug("Creating new servers.dat file");
                root = new NbtCompound("");
                serversList = new NbtList("servers", NbtTagType.Compound);
                root.Add(serversList);
            }
            
            // Check if server already exists
            bool serverExists = false;
            foreach (NbtCompound server in serversList)
            {
                var ip = server.Get<NbtString>("ip")?.Value;
                if (ip == serverIp)
                {
                    // Update server name
                    var nameTag = server.Get<NbtString>("name");
                    if (nameTag != null)
                    {
                        nameTag.Value = serverName;
                    }
                    else
                    {
                        server.Add(new NbtString("name", serverName));
                    }
                    
                    serverExists = true;
                    _logger.Information("Updated existing server entry for {ServerIp}", serverIp);
                    break;
                }
            }
            
            // Add new server if it doesn't exist
            if (!serverExists)
            {
                var newServer = new NbtCompound
                {
                    new NbtString("name", serverName),
                    new NbtString("ip", serverIp),
                    new NbtByte("hidden", 0)
                };
                
                serversList.Add(newServer);
                _logger.Information("Added new server entry: {ServerName} ({ServerIp})", serverName, serverIp);
            }
            
            // Save file
            var nbtFile = new NbtFile(root);
            nbtFile.SaveToFile(serversDatPath, NbtCompression.None);
            
            _logger.Information("Server list saved successfully to {Path}", serversDatPath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error managing server list");
            return false;
        }
    }
    
    /// <summary>
    /// Removes a server from the Minecraft server list
    /// </summary>
    /// <param name="serverIp">Server IP address to remove</param>
    /// <returns>True if successful, false otherwise</returns>
    public async Task<bool> RemoveServerAsync(string serverIp)
    {
        if (string.IsNullOrWhiteSpace(serverIp))
            throw new ArgumentException("Server IP cannot be null or empty", nameof(serverIp));
        
        _logger.Information("Removing server {ServerIp} from server list", serverIp);
        
        try
        {
            var minecraftDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ".minecraft");
            
            var serversDatPath = Path.Combine(minecraftDir, "servers.dat");
            
            if (!File.Exists(serversDatPath))
            {
                _logger.Warning("servers.dat does not exist, nothing to remove");
                return true;
            }
            
            var file = new NbtFile(serversDatPath);
            file.LoadFromFile(serversDatPath);
            var root = file.RootTag;
            
            var serversList = root.Get<NbtList>("servers");
            if (serversList == null)
            {
                _logger.Warning("No servers list found in servers.dat");
                return true;
            }
            
            // Find and remove server
            NbtCompound? serverToRemove = null;
            foreach (NbtCompound server in serversList)
            {
                var ip = server.Get<NbtString>("ip")?.Value;
                if (ip == serverIp)
                {
                    serverToRemove = server;
                    break;
                }
            }
            
            if (serverToRemove != null)
            {
                serversList.Remove(serverToRemove);
                _logger.Information("Removed server {ServerIp} from list", serverIp);
                
                // Save file
                var nbtFile = new NbtFile(root);
                nbtFile.SaveToFile(serversDatPath, NbtCompression.None);
                
                _logger.Information("Server list saved successfully");
                return true;
            }
            else
            {
                _logger.Warning("Server {ServerIp} not found in server list", serverIp);
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error removing server from list");
            return false;
        }
    }
    
    /// <summary>
    /// Gets all servers from the Minecraft server list
    /// </summary>
    /// <returns>List of server entries (name, ip)</returns>
    public async Task<List<(string Name, string Ip)>> GetServersAsync()
    {
        var servers = new List<(string Name, string Ip)>();
        
        try
        {
            var minecraftDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ".minecraft");
            
            var serversDatPath = Path.Combine(minecraftDir, "servers.dat");
            
            if (!File.Exists(serversDatPath))
            {
                _logger.Debug("servers.dat does not exist");
                return servers;
            }
            
            var file = new NbtFile(serversDatPath);
            file.LoadFromFile(serversDatPath);
            var root = file.RootTag;
            
            var serversList = root.Get<NbtList>("servers");
            if (serversList == null)
            {
                _logger.Debug("No servers list found in servers.dat");
                return servers;
            }
            
            foreach (NbtCompound server in serversList)
            {
                var name = server.Get<NbtString>("name")?.Value ?? "Unknown";
                var ip = server.Get<NbtString>("ip")?.Value ?? "";
                
                if (!string.IsNullOrEmpty(ip))
                {
                    servers.Add((name, ip));
                }
            }
            
            _logger.Debug("Found {Count} servers in server list", servers.Count);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error reading server list");
        }
        
        return servers;
    }
}
