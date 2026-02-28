using System;
using System.ComponentModel.DataAnnotations;

namespace MinecraftLauncher.Core.Models
{
    public class Profile : IEquatable<Profile>
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Profile name is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Profile name must be between 1 and 50 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Server IP is required")]
        public string ServerIp { get; set; } = string.Empty;

        [Required(ErrorMessage = "Minecraft version is required")]
        public string MinecraftVersion { get; set; } = string.Empty;

        [Required]
        public ModLoaderType ModLoader { get; set; }

        [Required]
        public ServerType ServerType { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUsed { get; set; }

        /// <summary>
        /// Maximum memory allocation in MB (default: 2048)
        /// </summary>
        public int? MaxMemory { get; set; } = 2048;

        /// <summary>
        /// Minimum memory allocation in MB (default: 512)
        /// </summary>
        public int? MinMemory { get; set; } = 512;

        public bool Equals(Profile? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Id == other.Id &&
                   Name == other.Name &&
                   ServerIp == other.ServerIp &&
                   MinecraftVersion == other.MinecraftVersion &&
                   ModLoader == other.ModLoader &&
                   ServerType == other.ServerType &&
                   CreatedAt == other.CreatedAt &&
                   LastUsed == other.LastUsed &&
                   MaxMemory == other.MaxMemory &&
                   MinMemory == other.MinMemory;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Profile);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Id);
            hash.Add(Name);
            hash.Add(ServerIp);
            hash.Add(MinecraftVersion);
            hash.Add(ModLoader);
            hash.Add(ServerType);
            hash.Add(CreatedAt);
            hash.Add(LastUsed);
            hash.Add(MaxMemory);
            hash.Add(MinMemory);
            return hash.ToHashCode();
        }
    }

    public enum ModLoaderType
    {
        Vanilla,
        Forge,
        Fabric,
        Paper
    }

    public enum ServerType
    {
        ModServer,
        PluginServer
    }
}
