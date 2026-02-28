using MinecraftLauncher.Core.Models;

namespace MinecraftLauncher.Core.Interfaces
{
    public interface IFriendManager
    {
        void AddFriend(string username);
        void RemoveFriend(string username);
        List<Friend> GetAllFriends();
        Task<Friend> CheckFriendStatusAsync(string username, string serverUrl);
        Task RefreshAllFriendStatusesAsync(string serverUrl);
    }
}
