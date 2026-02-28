using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MinecraftLauncher.Core.Models;

namespace MinecraftLauncher.Core.Interfaces
{
    /// <summary>
    /// Interface for managing player statistics and achievements.
    /// </summary>
    public interface IStatisticsManager
    {
        /// <summary>
        /// Fetches player statistics from the Minecraft server.
        /// </summary>
        /// <param name="username">The Minecraft username.</param>
        /// <param name="serverAddress">The server address.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Player statistics.</returns>
        Task<PlayerStats> FetchPlayerStatsAsync(string username, string serverAddress, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets cached player statistics.
        /// </summary>
        /// <param name="username">The Minecraft username.</param>
        /// <returns>Cached player statistics, or null if not cached.</returns>
        Task<PlayerStats?> GetCachedStatsAsync(string username);

        /// <summary>
        /// Caches player statistics locally.
        /// </summary>
        /// <param name="username">The Minecraft username.</param>
        /// <param name="stats">The player statistics to cache.</param>
        Task CacheStatsAsync(string username, PlayerStats stats);

        /// <summary>
        /// Tracks a game session.
        /// </summary>
        /// <param name="username">The Minecraft username.</param>
        /// <param name="sessionDuration">Duration of the session in minutes.</param>
        Task TrackSessionAsync(string username, int sessionDuration);

        /// <summary>
        /// Gets total playtime for a user.
        /// </summary>
        /// <param name="username">The Minecraft username.</param>
        /// <returns>Total playtime in minutes.</returns>
        Task<int> GetTotalPlaytimeAsync(string username);

        /// <summary>
        /// Gets all tracked statistics for a user.
        /// </summary>
        /// <param name="username">The Minecraft username.</param>
        /// <returns>Dictionary of statistic names and values.</returns>
        Task<Dictionary<string, int>> GetAllStatsAsync(string username);
    }
}
