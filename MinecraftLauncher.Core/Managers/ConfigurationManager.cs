using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using MinecraftLauncher.Core.Interfaces;
using MinecraftLauncher.Core.Models;
using Serilog;

namespace MinecraftLauncher.Core.Managers
{
    /// <summary>
    /// Manages persistence and loading of launcher configuration using System.Text.Json
    /// </summary>
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly JsonSerializerOptions _jsonOptions;

        public ConfigurationManager()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
        }

        /// <summary>
        /// Saves the main launcher configuration to %AppData%/MinecraftLauncher/config.json
        /// </summary>
        public void SaveConfiguration(LauncherConfig config)
        {
            try
            {
                LauncherPaths.EnsureDirectoriesExist();
                
                string json = JsonSerializer.Serialize(config, _jsonOptions);
                File.WriteAllText(LauncherPaths.ConfigFilePath, json);
                
                Log.Information("Configuration saved successfully to {Path}", LauncherPaths.ConfigFilePath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to save configuration to {Path}", LauncherPaths.ConfigFilePath);
                throw;
            }
        }

        /// <summary>
        /// Loads the main launcher configuration from disk
        /// Returns a default configuration if the file doesn't exist or is corrupted
        /// </summary>
        public LauncherConfig LoadConfiguration()
        {
            try
            {
                if (!File.Exists(LauncherPaths.ConfigFilePath))
                {
                    Log.Information("Configuration file not found, creating default configuration");
                    return CreateDefaultConfiguration();
                }

                string json = File.ReadAllText(LauncherPaths.ConfigFilePath);
                var config = JsonSerializer.Deserialize<LauncherConfig>(json, _jsonOptions);

                if (config == null)
                {
                    Log.Warning("Configuration deserialized to null, creating default configuration");
                    return CreateDefaultConfiguration();
                }

                Log.Information("Configuration loaded successfully from {Path}", LauncherPaths.ConfigFilePath);
                return config;
            }
            catch (JsonException ex)
            {
                Log.Error(ex, "Configuration file is corrupted, creating backup and default configuration");
                BackupCorruptedConfiguration();
                return CreateDefaultConfiguration();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load configuration from {Path}", LauncherPaths.ConfigFilePath);
                return CreateDefaultConfiguration();
            }
        }

        /// <summary>
        /// Saves an individual profile to its own JSON file
        /// </summary>
        public void SaveProfile(Profile profile)
        {
            try
            {
                string profileDir = Path.Combine(LauncherPaths.ProfilesDirectory, profile.Id);
                Directory.CreateDirectory(profileDir);

                string profilePath = Path.Combine(profileDir, "profile.json");
                string json = JsonSerializer.Serialize(profile, _jsonOptions);
                File.WriteAllText(profilePath, json);

                Log.Information("Profile {ProfileId} saved successfully", profile.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to save profile {ProfileId}", profile.Id);
                throw;
            }
        }

        /// <summary>
        /// Loads an individual profile from its JSON file
        /// </summary>
        public Profile? LoadProfile(string profileId)
        {
            try
            {
                string profilePath = Path.Combine(LauncherPaths.ProfilesDirectory, profileId, "profile.json");

                if (!File.Exists(profilePath))
                {
                    Log.Warning("Profile file not found for {ProfileId}", profileId);
                    return null;
                }

                string json = File.ReadAllText(profilePath);
                var profile = JsonSerializer.Deserialize<Profile>(json, _jsonOptions);

                if (profile == null)
                {
                    Log.Warning("Profile {ProfileId} deserialized to null", profileId);
                    return null;
                }

                Log.Information("Profile {ProfileId} loaded successfully", profileId);
                return profile;
            }
            catch (JsonException ex)
            {
                Log.Error(ex, "Profile file for {ProfileId} is corrupted", profileId);
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load profile {ProfileId}", profileId);
                return null;
            }
        }

        /// <summary>
        /// Saves UI customization settings for a profile
        /// </summary>
        public void SaveCustomization(UICustomization customization)
        {
            try
            {
                string customizationDir = LauncherPaths.GetProfileCustomizationDirectory(customization.ProfileId);
                Directory.CreateDirectory(customizationDir);

                string customizationPath = Path.Combine(customizationDir, "customization.json");
                string json = JsonSerializer.Serialize(customization, _jsonOptions);
                File.WriteAllText(customizationPath, json);

                Log.Information("Customization for profile {ProfileId} saved successfully", customization.ProfileId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to save customization for profile {ProfileId}", customization.ProfileId);
                throw;
            }
        }

        /// <summary>
        /// Loads UI customization settings for a profile
        /// Returns default settings if not found
        /// </summary>
        public UICustomization LoadCustomization(string profileId)
        {
            try
            {
                string customizationPath = Path.Combine(
                    LauncherPaths.GetProfileCustomizationDirectory(profileId),
                    "customization.json"
                );

                if (!File.Exists(customizationPath))
                {
                    Log.Information("Customization file not found for profile {ProfileId}, using defaults", profileId);
                    return CreateDefaultCustomization(profileId);
                }

                string json = File.ReadAllText(customizationPath);
                var customization = JsonSerializer.Deserialize<UICustomization>(json, _jsonOptions);

                if (customization == null)
                {
                    Log.Warning("Customization for profile {ProfileId} deserialized to null, using defaults", profileId);
                    return CreateDefaultCustomization(profileId);
                }

                Log.Information("Customization for profile {ProfileId} loaded successfully", profileId);
                return customization;
            }
            catch (JsonException ex)
            {
                Log.Error(ex, "Customization file for profile {ProfileId} is corrupted, using defaults", profileId);
                return CreateDefaultCustomization(profileId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load customization for profile {ProfileId}, using defaults", profileId);
                return CreateDefaultCustomization(profileId);
            }
        }

        /// <summary>
        /// Creates a default launcher configuration
        /// </summary>
        private LauncherConfig CreateDefaultConfiguration()
        {
            return new LauncherConfig
            {
                Profiles = new System.Collections.Generic.List<Profile>(),
                LastUsedProfileId = string.Empty,
                Version = "1.0.0"
            };
        }

        /// <summary>
        /// Creates default UI customization settings for a profile
        /// </summary>
        private UICustomization CreateDefaultCustomization(string profileId)
        {
            return new UICustomization
            {
                ProfileId = profileId,
                LogoPath = string.Empty,
                BackgroundPath = string.Empty,
                PrimaryColor = "#1e1e1e",
                SecondaryColor = "#ffffff",
                DarkMode = true
            };
        }

        /// <summary>
        /// Backs up a corrupted configuration file
        /// </summary>
        private void BackupCorruptedConfiguration()
        {
            try
            {
                if (File.Exists(LauncherPaths.ConfigFilePath))
                {
                    string backupPath = LauncherPaths.ConfigFilePath + ".corrupted." + DateTime.Now.ToString("yyyyMMddHHmmss");
                    File.Copy(LauncherPaths.ConfigFilePath, backupPath);
                    Log.Information("Corrupted configuration backed up to {BackupPath}", backupPath);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to backup corrupted configuration");
            }
        }
    }
}
