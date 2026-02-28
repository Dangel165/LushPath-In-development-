using MinecraftLauncher.Core.Interfaces;
using MinecraftLauncher.Core.Models;
using Serilog;
using System.Text.Json;

namespace MinecraftLauncher.Core.Managers
{
    /// <summary>
    /// Manages server announcements with caching and read status tracking
    /// </summary>
    public class AnnouncementManager : IAnnouncementManager
    {
        private readonly IHttpClientService _httpClient;
        private readonly ILogger _logger;
        private readonly string _cacheDirectory;
        private readonly string _cacheFilePath;
        private List<Announcement> _cachedAnnouncements;

        /// <summary>
        /// Initializes a new instance of the AnnouncementManager
        /// </summary>
        /// <param name="httpClient">HTTP client service for network requests</param>
        /// <param name="logger">Logger for diagnostic information</param>
        public AnnouncementManager(IHttpClientService httpClient, ILogger logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Set up cache directory: %AppData%/MinecraftLauncher/cache/
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _cacheDirectory = Path.Combine(appDataPath, "MinecraftLauncher", "cache");
            _cacheFilePath = Path.Combine(_cacheDirectory, "announcements.json");

            // Ensure cache directory exists
            Directory.CreateDirectory(_cacheDirectory);

            // Load cached announcements on initialization
            _cachedAnnouncements = LoadAnnouncementsFromCache();
        }

        /// <inheritdoc/>
        public async Task<List<Announcement>> FetchAnnouncementsAsync(string serverUrl, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(serverUrl))
            {
                throw new ArgumentException("Server URL cannot be null or empty", nameof(serverUrl));
            }

            try
            {
                _logger.Information("Fetching announcements from server: {ServerUrl}", serverUrl);

                // Construct API endpoint
                var apiUrl = $"{serverUrl.TrimEnd('/')}/api/announcements";

                // Fetch announcements from server
                var jsonResponse = await _httpClient.GetStringAsync(apiUrl, cancellationToken);

                // Deserialize response
                var announcements = JsonSerializer.Deserialize<List<Announcement>>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (announcements == null || announcements.Count == 0)
                {
                    _logger.Warning("No announcements received from server");
                    return _cachedAnnouncements;
                }

                _logger.Information("Successfully fetched {Count} announcements from server", announcements.Count);

                // Merge with cached announcements to preserve read status
                var mergedAnnouncements = MergeWithCachedReadStatus(announcements);

                // Update cache
                _cachedAnnouncements = mergedAnnouncements;
                SaveAnnouncementsToCache(mergedAnnouncements);

                return mergedAnnouncements;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch announcements from server. Falling back to cache.");

                // Return cached announcements on failure
                return _cachedAnnouncements;
            }
        }

        /// <inheritdoc/>
        public List<Announcement> GetCachedAnnouncements()
        {
            _logger.Debug("Retrieving {Count} cached announcements", _cachedAnnouncements.Count);
            return new List<Announcement>(_cachedAnnouncements);
        }

        /// <inheritdoc/>
        public void MarkAnnouncementAsRead(string announcementId)
        {
            if (string.IsNullOrWhiteSpace(announcementId))
            {
                throw new ArgumentException("Announcement ID cannot be null or empty", nameof(announcementId));
            }

            var announcement = _cachedAnnouncements.FirstOrDefault(a => a.Id == announcementId);

            if (announcement == null)
            {
                _logger.Warning("Announcement with ID {AnnouncementId} not found in cache", announcementId);
                return;
            }

            if (!announcement.IsRead)
            {
                announcement.IsRead = true;
                SaveAnnouncementsToCache(_cachedAnnouncements);
                _logger.Debug("Marked announcement {AnnouncementId} as read", announcementId);
            }
        }

        /// <inheritdoc/>
        public List<Announcement> GetUnreadAnnouncements()
        {
            var unreadAnnouncements = _cachedAnnouncements.Where(a => !a.IsRead).ToList();
            _logger.Debug("Found {Count} unread announcements", unreadAnnouncements.Count);
            return unreadAnnouncements;
        }

        /// <summary>
        /// Loads announcements from the cache file
        /// </summary>
        /// <returns>List of cached announcements, or empty list if cache doesn't exist</returns>
        private List<Announcement> LoadAnnouncementsFromCache()
        {
            try
            {
                if (!File.Exists(_cacheFilePath))
                {
                    _logger.Debug("No announcement cache file found at {CacheFilePath}", _cacheFilePath);
                    return new List<Announcement>();
                }

                var jsonContent = File.ReadAllText(_cacheFilePath);
                var announcements = JsonSerializer.Deserialize<List<Announcement>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (announcements == null)
                {
                    _logger.Warning("Failed to deserialize announcement cache");
                    return new List<Announcement>();
                }

                _logger.Information("Loaded {Count} announcements from cache", announcements.Count);
                return announcements;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load announcements from cache");
                return new List<Announcement>();
            }
        }

        /// <summary>
        /// Saves announcements to the cache file
        /// </summary>
        /// <param name="announcements">List of announcements to cache</param>
        private void SaveAnnouncementsToCache(List<Announcement> announcements)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(announcements, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(_cacheFilePath, jsonContent);
                _logger.Debug("Saved {Count} announcements to cache", announcements.Count);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to save announcements to cache");
            }
        }

        /// <summary>
        /// Merges newly fetched announcements with cached announcements to preserve read status
        /// </summary>
        /// <param name="newAnnouncements">Newly fetched announcements from server</param>
        /// <returns>Merged list with preserved read status</returns>
        private List<Announcement> MergeWithCachedReadStatus(List<Announcement> newAnnouncements)
        {
            var mergedAnnouncements = new List<Announcement>();

            foreach (var newAnnouncement in newAnnouncements)
            {
                // Check if this announcement exists in cache
                var cachedAnnouncement = _cachedAnnouncements.FirstOrDefault(a => a.Id == newAnnouncement.Id);

                if (cachedAnnouncement != null)
                {
                    // Preserve read status from cache
                    newAnnouncement.IsRead = cachedAnnouncement.IsRead;
                }
                else
                {
                    // New announcement - mark as unread
                    newAnnouncement.IsRead = false;
                }

                mergedAnnouncements.Add(newAnnouncement);
            }

            return mergedAnnouncements;
        }
    }
}
