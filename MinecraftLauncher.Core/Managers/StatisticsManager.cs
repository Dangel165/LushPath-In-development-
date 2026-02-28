using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MinecraftLauncher.Core.Interfaces;
using MinecraftLauncher.Core.Models;

namespace MinecraftLauncher.Core.Managers
{
    /// <summary>
    /// Manages player statistics and achievements.
    /// </summary>
    public class StatisticsManager : IStatisticsManager
    {
        private readonly IHttpClientService _httpClientService;
        private readonly string _cacheDirectory;
        private readonly string _statsDirectory;

        public StatisticsManager(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService ?? throw new ArgumentNullException(nameof(httpClientService));
            
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _cacheDirectory = Path.Combine(appDataPath, "MinecraftLauncher", "cache", "stats");
            _statsDirectory = Path.Combine(appDataPath, "MinecraftLauncher", "stats");
            
            if (!Directory.Exists(_cacheDirectory))
            {
                Directory.CreateDirectory(_cacheDirectory);
            }
            
            if (!Directory.Exists(_statsDirectory))
            {
                Directory.CreateDirectory(_statsDirectory);
            }
        }

        /// <summary>
        /// Fetches player statistics from the Minecraft server.
        /// </summary>
        public async Task<PlayerStats> FetchPlayerStatsAsync(string username, string serverAddress, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            }

            if (string.IsNullOrWhiteSpace(serverAddress))
            {
                throw new ArgumentException("Server address cannot be null or empty.", nameof(serverAddress));
            }

            // Check cache first
            var cachedStats = await GetCachedStatsAsync(username);
            if (cachedStats != null)
            {
                return cachedStats;
            }

            try
            {
                // Try to fetch from server API
                var statsUrl = $"https://{serverAddress}/api/stats/{username}";
                var statsJson = await _httpClientService.GetStringAsync(statsUrl, cancellationToken);
                
                var stats = JsonSerializer.Deserialize<PlayerStats>(statsJson);
                if (stats != null)
                {
                    await CacheStatsAsync(username, stats);
                    return stats;
                }
            }
            catch
            {
                // If server fetch fails, return cached stats or create default
                if (cachedStats != null)
                {
                    return cachedStats;
                }
            }

            // Return default stats if all else fails
            return CreateDefaultStats(username);
        }

        /// <summary>
        /// Gets cached player statistics.
        /// </summary>
        public async Task<PlayerStats?> GetCachedStatsAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            var cachePath = Path.Combine(_cacheDirectory, $"{username}.json");
            
            if (!File.Exists(cachePath))
            {
                return null;
            }

            try
            {
                var json = await File.ReadAllTextAsync(cachePath);
                return JsonSerializer.Deserialize<PlayerStats>(json);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Caches player statistics locally.
        /// </summary>
        public async Task CacheStatsAsync(string username, PlayerStats stats)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            }

            if (stats == null)
            {
                throw new ArgumentNullException(nameof(stats));
            }

            var cachePath = Path.Combine(_cacheDirectory, $"{username}.json");
            
            try
            {
                var json = JsonSerializer.Serialize(stats, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(cachePath, json);
            }
            catch (Exception ex)
            {
                // Log error but don't throw - caching is not critical
                Console.WriteLine($"Failed to cache stats for {username}: {ex.Message}");
            }
        }

        /// <summary>
        /// Tracks a game session.
        /// </summary>
        public async Task TrackSessionAsync(string username, int sessionDuration)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            }

            if (sessionDuration < 0)
            {
                throw new ArgumentException("Session duration cannot be negative.", nameof(sessionDuration));
            }

            var statsPath = Path.Combine(_statsDirectory, $"{username}_sessions.json");
            
            Dictionary<string, int> sessions;
            
            if (File.Exists(statsPath))
            {
                var json = await File.ReadAllTextAsync(statsPath);
                sessions = JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? new Dictionary<string, int>();
            }
            else
            {
                sessions = new Dictionary<string, int>();
            }

            // Add session
            var sessionKey = DateTime.UtcNow.ToString("yyyy-MM-dd");
            if (sessions.ContainsKey(sessionKey))
            {
                sessions[sessionKey] += sessionDuration;
            }
            else
            {
                sessions[sessionKey] = sessionDuration;
            }

            // Save updated sessions
            var updatedJson = JsonSerializer.Serialize(sessions, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(statsPath, updatedJson);
        }

        /// <summary>
        /// Gets total playtime for a user.
        /// </summary>
        public async Task<int> GetTotalPlaytimeAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            }

            var statsPath = Path.Combine(_statsDirectory, $"{username}_sessions.json");
            
            if (!File.Exists(statsPath))
            {
                return 0;
            }

            try
            {
                var json = await File.ReadAllTextAsync(statsPath);
                var sessions = JsonSerializer.Deserialize<Dictionary<string, int>>(json);
                
                if (sessions == null)
                {
                    return 0;
                }

                int totalMinutes = 0;
                foreach (var session in sessions.Values)
                {
                    totalMinutes += session;
                }

                return totalMinutes;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets all tracked statistics for a user.
        /// </summary>
        public async Task<Dictionary<string, int>> GetAllStatsAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            }

            var stats = new Dictionary<string, int>();

            // Get total playtime
            var totalPlaytime = await GetTotalPlaytimeAsync(username);
            stats["TotalPlaytime"] = totalPlaytime;

            // Get cached player stats
            var playerStats = await GetCachedStatsAsync(username);
            if (playerStats != null)
            {
                stats["Kills"] = playerStats.Kills;
                stats["Deaths"] = playerStats.Deaths;
                stats["AchievementCompletionPercentage"] = playerStats.AchievementCompletionPercentage;
            }

            return stats;
        }

        /// <summary>
        /// Creates default player statistics.
        /// </summary>
        private PlayerStats CreateDefaultStats(string username)
        {
            return new PlayerStats
            {
                Username = username,
                TotalPlaytime = TimeSpan.Zero,
                Kills = 0,
                Deaths = 0,
                KDRatio = 0.0,
                Achievements = new List<Achievement>(),
                AchievementCompletionPercentage = 0
            };
        }
    }
}
