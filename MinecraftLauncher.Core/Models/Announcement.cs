using System;
using System.ComponentModel.DataAnnotations;

namespace MinecraftLauncher.Core.Models
{
    public class Announcement : IEquatable<Announcement>
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;

        [Required]
        public DateTime Timestamp { get; set; }

        public bool IsRead { get; set; }

        [Required]
        public AnnouncementPriority Priority { get; set; }

        public bool Equals(Announcement? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Id == other.Id &&
                   Title == other.Title &&
                   Content == other.Content &&
                   Timestamp == other.Timestamp &&
                   IsRead == other.IsRead &&
                   Priority == other.Priority;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Announcement);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Title, Content, Timestamp, IsRead, Priority);
        }
    }

    public enum AnnouncementPriority
    {
        Low,
        Normal,
        High,
        Critical
    }
}
