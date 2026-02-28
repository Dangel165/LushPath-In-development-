using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using MinecraftLauncher.Core.Interfaces;
using MinecraftLauncher.Core.Models;
using Serilog;

namespace MinecraftLauncher.Core.Managers
{
    /// <summary>
    /// Manages server profiles and their configurations
    /// </summary>
    public class ProfileManager : IProfileManager
    {
        private readonly IConfigurationManager _configurationManager;
        private LauncherConfig _config;

        public ProfileManager(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager ?? throw new ArgumentNullException(nameof(configurationManager));
            _config = _configurationManager.LoadConfiguration();
        }

        /// <summary>
        /// Creates a new profile with validation
        /// </summary>
        public Profile CreateProfile(string name, string serverIp, string version, ModLoaderType loaderType, ServerType serverType)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Profile name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(serverIp))
                throw new ArgumentException("Server IP cannot be empty", nameof(serverIp));

            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentException("Minecraft version cannot be empty", nameof(version));

            // Create profile with unique ID
            var profile = new Profile
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                ServerIp = serverIp,
                MinecraftVersion = version,
                ModLoader = loaderType,
                ServerType = serverType,
                CreatedAt = DateTime.UtcNow,
                LastUsed = DateTime.UtcNow
            };

            // Validate using data annotations
            ValidateProfile(profile);

            // Save profile
            _configurationManager.SaveProfile(profile);

            // Add to config and save
            _config.Profiles.Add(profile);
            _configurationManager.SaveConfiguration(_config);

            Log.Information("Created profile {ProfileId} with name {ProfileName}", profile.Id, profile.Name);

            return profile;
        }

        /// <summary>
        /// Gets a profile by its ID
        /// </summary>
        public Profile? GetProfile(string profileId)
        {
            if (string.IsNullOrWhiteSpace(profileId))
                return null;

            // Try to find in memory first
            var profile = _config.Profiles.FirstOrDefault(p => p.Id == profileId);
            if (profile != null)
                return profile;

            // Try to load from disk
            profile = _configurationManager.LoadProfile(profileId);
            if (profile != null)
            {
                // Add to in-memory config if found on disk but not in memory
                if (!_config.Profiles.Any(p => p.Id == profileId))
                {
                    _config.Profiles.Add(profile);
                }
            }

            return profile;
        }

        /// <summary>
        /// Gets all profiles
        /// </summary>
        public List<Profile> GetAllProfiles()
        {
            return new List<Profile>(_config.Profiles);
        }

        /// <summary>
        /// Updates an existing profile
        /// </summary>
        public void UpdateProfile(Profile profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            // Validate profile
            ValidateProfile(profile);

            // Find and update in memory
            var existingProfile = _config.Profiles.FirstOrDefault(p => p.Id == profile.Id);
            if (existingProfile != null)
            {
                var index = _config.Profiles.IndexOf(existingProfile);
                _config.Profiles[index] = profile;
            }
            else
            {
                // Profile not in memory, add it
                _config.Profiles.Add(profile);
            }

            // Save to disk
            _configurationManager.SaveProfile(profile);
            _configurationManager.SaveConfiguration(_config);

            Log.Information("Updated profile {ProfileId}", profile.Id);
        }

        /// <summary>
        /// Deletes a profile and its associated data
        /// </summary>
        public void DeleteProfile(string profileId)
        {
            if (string.IsNullOrWhiteSpace(profileId))
                throw new ArgumentException("Profile ID cannot be empty", nameof(profileId));

            // Remove from in-memory config
            var profile = _config.Profiles.FirstOrDefault(p => p.Id == profileId);
            if (profile != null)
            {
                _config.Profiles.Remove(profile);
            }

            // Delete profile directory and all associated data
            string profileDir = Path.Combine(LauncherPaths.ProfilesDirectory, profileId);
            if (Directory.Exists(profileDir))
            {
                try
                {
                    Directory.Delete(profileDir, recursive: true);
                    Log.Information("Deleted profile directory for {ProfileId}", profileId);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to delete profile directory for {ProfileId}", profileId);
                    throw;
                }
            }

            // If this was the last used profile, clear it
            if (_config.LastUsedProfileId == profileId)
            {
                _config.LastUsedProfileId = string.Empty;
            }

            // Save configuration
            _configurationManager.SaveConfiguration(_config);

            Log.Information("Deleted profile {ProfileId}", profileId);
        }

        /// <summary>
        /// Gets the last used profile
        /// </summary>
        public Profile? GetLastUsedProfile()
        {
            if (string.IsNullOrWhiteSpace(_config.LastUsedProfileId))
                return null;

            return GetProfile(_config.LastUsedProfileId);
        }

        /// <summary>
        /// Sets the last used profile
        /// </summary>
        public void SetLastUsedProfile(string profileId)
        {
            if (string.IsNullOrWhiteSpace(profileId))
                throw new ArgumentException("Profile ID cannot be empty", nameof(profileId));

            // Verify profile exists
            var profile = GetProfile(profileId);
            if (profile == null)
                throw new InvalidOperationException($"Profile {profileId} does not exist");

            _config.LastUsedProfileId = profileId;
            _configurationManager.SaveConfiguration(_config);

            Log.Information("Set last used profile to {ProfileId}", profileId);
        }

        /// <summary>
        /// Validates a profile using data annotations
        /// </summary>
        private void ValidateProfile(Profile profile)
        {
            var validationContext = new ValidationContext(profile);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(profile, validationContext, validationResults, validateAllProperties: true))
            {
                var errors = string.Join(", ", validationResults.Select(r => r.ErrorMessage));
                throw new ValidationException($"Profile validation failed: {errors}");
            }
        }
    }
}
