using MinecraftLauncher.Core.Models;

namespace MinecraftLauncher.Core.Interfaces
{
    /// <summary>
    /// Manages server announcements including fetching, caching, and read status tracking
    /// </summary>
    public interface IAnnouncementManager
    {
        /// <summary>
        /// Fetches announcements from the server API and caches them locally
        /// </summary>
        /// <param name="serverUrl">The base URL of the server API</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of announcements from the server</returns>
        Task<List<Announcement>> FetchAnnouncementsAsync(string serverUrl, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves cached announcements from local storage
        /// </summary>
        /// <returns>List of cached announcements, or empty list if no cache exists</returns>
        List<Announcement> GetCachedAnnouncements();

        /// <summary>
        /// Marks an announcement as read and updates the cache
        /// </summary>
        /// <param name="announcementId">The ID of the announcement to mark as read</param>
        void MarkAnnouncementAsRead(string announcementId);

        /// <summary>
        /// Retrieves all announcements that have not been marked as read
        /// </summary>
        /// <returns>List of unread announcements</returns>
        List<Announcement> GetUnreadAnnouncements();
    }
}
