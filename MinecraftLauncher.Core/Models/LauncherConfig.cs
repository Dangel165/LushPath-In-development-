using System;
using System.Collections.Generic;

namespace MinecraftLauncher.Core.Models
{
    /// <summary>
    /// Main launcher configuration containing all profiles and settings
    /// </summary>
    public class LauncherConfig : IEquatable<LauncherConfig>
    {
        /// <summary>
        /// List of all profiles
        /// </summary>
        public List<Profile> Profiles { get; set; } = new List<Profile>();

        /// <summary>
        /// ID of the last used profile
        /// </summary>
        public string LastUsedProfileId { get; set; } = string.Empty;

        /// <summary>
        /// Launcher version
        /// </summary>
        public string Version { get; set; } = "1.0.0";

        public bool Equals(LauncherConfig? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (LastUsedProfileId != other.LastUsedProfileId || Version != other.Version)
                return false;

            if (Profiles.Count != other.Profiles.Count)
                return false;

            for (int i = 0; i < Profiles.Count; i++)
            {
                if (!Profiles[i].Equals(other.Profiles[i]))
                    return false;
            }

            return true;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as LauncherConfig);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Profiles, LastUsedProfileId, Version);
        }
    }
}
