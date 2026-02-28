using MinecraftLauncher.Core.Models;

namespace MinecraftLauncher.Core.Interfaces
{
    /// <summary>
    /// Manages persistence and loading of launcher configuration, profiles, and customization settings
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// Saves the main launcher configuration to disk
        /// </summary>
        /// <param name="config">The launcher configuration to save</param>
        void SaveConfiguration(LauncherConfig config);

        /// <summary>
        /// Loads the main launcher configuration from disk
        /// </summary>
        /// <returns>The loaded launcher configuration, or a default configuration if loading fails</returns>
        LauncherConfig LoadConfiguration();

        /// <summary>
        /// Saves an individual profile to disk
        /// </summary>
        /// <param name="profile">The profile to save</param>
        void SaveProfile(Profile profile);

        /// <summary>
        /// Loads an individual profile from disk
        /// </summary>
        /// <param name="profileId">The ID of the profile to load</param>
        /// <returns>The loaded profile, or null if not found</returns>
        Profile? LoadProfile(string profileId);

        /// <summary>
        /// Saves UI customization settings for a profile
        /// </summary>
        /// <param name="customization">The customization settings to save</param>
        void SaveCustomization(UICustomization customization);

        /// <summary>
        /// Loads UI customization settings for a profile
        /// </summary>
        /// <param name="profileId">The ID of the profile whose customization to load</param>
        /// <returns>The loaded customization settings, or default settings if not found</returns>
        UICustomization LoadCustomization(string profileId);
    }
}
