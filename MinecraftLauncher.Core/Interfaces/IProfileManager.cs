using MinecraftLauncher.Core.Models;
using System.Collections.Generic;

namespace MinecraftLauncher.Core.Interfaces
{
    /// <summary>
    /// Manages server profiles and their configurations
    /// </summary>
    public interface IProfileManager
    {
        /// <summary>
        /// Creates a new profile with validation
        /// </summary>
        /// <param name="name">Profile name</param>
        /// <param name="serverIp">Server IP address or domain</param>
        /// <param name="version">Minecraft version</param>
        /// <param name="loaderType">Mod loader type</param>
        /// <param name="serverType">Server type (ModServer or PluginServer)</param>
        /// <returns>The created profile</returns>
        Profile CreateProfile(string name, string serverIp, string version, ModLoaderType loaderType, ServerType serverType);

        /// <summary>
        /// Gets a profile by its ID
        /// </summary>
        /// <param name="profileId">The profile ID</param>
        /// <returns>The profile, or null if not found</returns>
        Profile? GetProfile(string profileId);

        /// <summary>
        /// Gets all profiles
        /// </summary>
        /// <returns>List of all profiles</returns>
        List<Profile> GetAllProfiles();

        /// <summary>
        /// Updates an existing profile
        /// </summary>
        /// <param name="profile">The profile to update</param>
        void UpdateProfile(Profile profile);

        /// <summary>
        /// Deletes a profile and its associated data
        /// </summary>
        /// <param name="profileId">The ID of the profile to delete</param>
        void DeleteProfile(string profileId);

        /// <summary>
        /// Gets the last used profile
        /// </summary>
        /// <returns>The last used profile, or null if none exists</returns>
        Profile? GetLastUsedProfile();

        /// <summary>
        /// Sets the last used profile
        /// </summary>
        /// <param name="profileId">The ID of the profile to set as last used</param>
        void SetLastUsedProfile(string profileId);
    }
}
