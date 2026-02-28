using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MinecraftLauncher.Core.Models
{
    public class PlayerStats : IEquatable<PlayerStats>
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;

        public TimeSpan TotalPlaytime { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Kills must be non-negative")]
        public int Kills { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Deaths must be non-negative")]
        public int Deaths { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "KD Ratio must be non-negative")]
        public double KDRatio { get; set; }

        public List<Achievement> Achievements { get; set; } = new List<Achievement>();

        [Range(0, 100, ErrorMessage = "Achievement completion percentage must be between 0 and 100")]
        public int AchievementCompletionPercentage { get; set; }

        public bool Equals(PlayerStats? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            // Compare achievements
            if (Achievements.Count != other.Achievements.Count)
                return false;

            for (int i = 0; i < Achievements.Count; i++)
            {
                if (!Achievements[i].Equals(other.Achievements[i]))
                    return false;
            }

            return Username == other.Username &&
                   TotalPlaytime == other.TotalPlaytime &&
                   Kills == other.Kills &&
                   Deaths == other.Deaths &&
                   Math.Abs(KDRatio - other.KDRatio) < 0.001 &&
                   AchievementCompletionPercentage == other.AchievementCompletionPercentage;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as PlayerStats);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Username);
            hash.Add(TotalPlaytime);
            hash.Add(Kills);
            hash.Add(Deaths);
            hash.Add(KDRatio);
            hash.Add(AchievementCompletionPercentage);
            foreach (var achievement in Achievements)
            {
                hash.Add(achievement);
            }
            return hash.ToHashCode();
        }
    }

    public class Achievement : IEquatable<Achievement>
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Achievement name is required")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        public DateTime? CompletedAt { get; set; }

        public bool Equals(Achievement? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Id == other.Id &&
                   Name == other.Name &&
                   Description == other.Description &&
                   IsCompleted == other.IsCompleted &&
                   CompletedAt == other.CompletedAt;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Achievement);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Description, IsCompleted, CompletedAt);
        }
    }
}
