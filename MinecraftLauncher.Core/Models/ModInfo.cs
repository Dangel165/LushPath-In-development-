using System;
using System.ComponentModel.DataAnnotations;

namespace MinecraftLauncher.Core.Models
{
    public class ModInfo : IEquatable<ModInfo>
    {
        [Required(ErrorMessage = "File name is required")]
        public string FileName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Download URL is required")]
        [Url(ErrorMessage = "Download URL must be a valid URL")]
        public string DownloadUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Checksum is required")]
        public string Checksum { get; set; } = string.Empty;

        [Range(1, long.MaxValue, ErrorMessage = "File size must be greater than 0")]
        public long FileSize { get; set; }

        [Required(ErrorMessage = "Version is required")]
        public string Version { get; set; } = string.Empty;

        public bool Required { get; set; }

        public bool Equals(ModInfo? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return FileName == other.FileName &&
                   DownloadUrl == other.DownloadUrl &&
                   Checksum == other.Checksum &&
                   FileSize == other.FileSize &&
                   Version == other.Version &&
                   Required == other.Required;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ModInfo);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FileName, DownloadUrl, Checksum, FileSize, Version, Required);
        }
    }
}
