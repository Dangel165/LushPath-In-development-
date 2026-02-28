using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace MinecraftLauncher.Core.Models
{
    public class UICustomization : IEquatable<UICustomization>
    {
        [Required]
        public string ProfileId { get; set; } = string.Empty;

        public string LogoPath { get; set; } = string.Empty;

        public string BackgroundPath { get; set; } = string.Empty;

        public string PrimaryColor { get; set; } = "#1e1e1e";

        public string SecondaryColor { get; set; } = "#ffffff";

        public bool DarkMode { get; set; } = true;

        public bool Equals(UICustomization? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return ProfileId == other.ProfileId &&
                   LogoPath == other.LogoPath &&
                   BackgroundPath == other.BackgroundPath &&
                   PrimaryColor == other.PrimaryColor &&
                   SecondaryColor == other.SecondaryColor &&
                   DarkMode == other.DarkMode;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as UICustomization);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ProfileId, LogoPath, BackgroundPath, PrimaryColor, SecondaryColor, DarkMode);
        }
    }
}
