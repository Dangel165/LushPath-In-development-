using System;
using System.ComponentModel.DataAnnotations;

namespace MinecraftLauncher.Core.Models
{
    public class Friend : IEquatable<Friend>
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(16, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 16 characters")]
        public string Username { get; set; } = string.Empty;

        public bool IsOnline { get; set; }

        public string CurrentServer { get; set; } = string.Empty;

        public DateTime LastSeen { get; set; }

        public bool Equals(Friend? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Username == other.Username &&
                   IsOnline == other.IsOnline &&
                   CurrentServer == other.CurrentServer &&
                   LastSeen == other.LastSeen;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Friend);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Username, IsOnline, CurrentServer, LastSeen);
        }
    }
}
