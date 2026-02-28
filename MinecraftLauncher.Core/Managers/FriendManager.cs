using MinecraftLauncher.Core.Interfaces;
using MinecraftLauncher.Core.Models;
using System.Text.Json;

namespace MinecraftLauncher.Core.Managers
{
    public class FriendManager : IFriendManager
    {
        private readonly IHttpClientService _httpClient;
        private readonly string _friendsFilePath;
        private List<Friend> _friends;

        public FriendManager(IHttpClientService httpClient)
        {
            _httpClient = httpClient;
            
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var launcherDirectory = Path.Combine(appDataPath, "MinecraftLauncher");
            Directory.CreateDirectory(launcherDirectory);
            
            _friendsFilePath = Path.Combine(launcherDirectory, "friends.json");
            _friends = LoadFriendsFromFile();
        }

        public void AddFriend(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be empty", nameof(username));
            }

            // Check if friend already exists
            if (_friends.Any(f => f.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                return; // Friend already exists
            }

            var friend = new Friend
            {
                Username = username,
                IsOnline = false,
                CurrentServer = string.Empty,
                LastSeen = DateTime.UtcNow
            };

            _friends.Add(friend);
            SortFriendList();
            SaveFriendsToFile();
        }

        public void RemoveFriend(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be empty", nameof(username));
            }

            var friend = _friends.FirstOrDefault(f => f.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (friend != null)
            {
                _friends.Remove(friend);
                SaveFriendsToFile();
            }
        }

        public List<Friend> GetAllFriends()
        {
            // Return a copy to prevent external modification
            return new List<Friend>(_friends);
        }

        public async Task<Friend> CheckFriendStatusAsync(string username, string serverUrl)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be empty", nameof(username));
            }

            if (string.IsNullOrWhiteSpace(serverUrl))
            {
                throw new ArgumentException("Server URL cannot be empty", nameof(serverUrl));
            }

            try
            {
                var apiUrl = $"{serverUrl.TrimEnd('/')}/api/friends/{username}/status";
                var jsonResponse = await _httpClient.GetStringAsync(apiUrl);
                
                var friendStatus = JsonSerializer.Deserialize<Friend>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (friendStatus != null)
                {
                    // Update the friend in our list if they exist
                    var existingFriend = _friends.FirstOrDefault(f => f.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
                    if (existingFriend != null)
                    {
                        existingFriend.IsOnline = friendStatus.IsOnline;
                        existingFriend.CurrentServer = friendStatus.CurrentServer ?? string.Empty;
                        existingFriend.LastSeen = friendStatus.IsOnline ? DateTime.UtcNow : friendStatus.LastSeen;
                        SaveFriendsToFile();
                    }

                    return friendStatus;
                }
            }
            catch (Exception)
            {
                // If we can't reach the server, return offline status
            }

            // Return offline status if fetch fails
            return new Friend
            {
                Username = username,
                IsOnline = false,
                CurrentServer = string.Empty,
                LastSeen = DateTime.UtcNow
            };
        }

        public async Task RefreshAllFriendStatusesAsync(string serverUrl)
        {
            if (string.IsNullOrWhiteSpace(serverUrl))
            {
                throw new ArgumentException("Server URL cannot be empty", nameof(serverUrl));
            }

            var tasks = _friends.Select(friend => CheckFriendStatusAsync(friend.Username, serverUrl)).ToList();
            await Task.WhenAll(tasks);
        }

        private void SortFriendList()
        {
            _friends = _friends.OrderBy(f => f.Username, StringComparer.OrdinalIgnoreCase).ToList();
        }

        private List<Friend> LoadFriendsFromFile()
        {
            if (!File.Exists(_friendsFilePath))
            {
                return new List<Friend>();
            }

            try
            {
                var json = File.ReadAllText(_friendsFilePath);
                var friends = JsonSerializer.Deserialize<List<Friend>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (friends != null)
                {
                    // Ensure list is sorted
                    return friends.OrderBy(f => f.Username, StringComparer.OrdinalIgnoreCase).ToList();
                }
            }
            catch (Exception)
            {
                // If loading fails, return empty list
            }

            return new List<Friend>();
        }

        private void SaveFriendsToFile()
        {
            try
            {
                var json = JsonSerializer.Serialize(_friends, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(_friendsFilePath, json);
            }
            catch (Exception)
            {
                // Log error but don't throw - saving friends is not critical
            }
        }
    }
}
