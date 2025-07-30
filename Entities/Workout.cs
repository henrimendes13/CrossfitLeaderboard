using System.ComponentModel.DataAnnotations;

namespace CrossfitLeaderboard.Entities
{
    public class Workout
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public WorkoutType Type { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Unit { get; set; } = string.Empty;
        
        // Navigation property para many-to-many com Category
        public virtual ICollection<WorkoutCategory> WorkoutCategories { get; set; } = new List<WorkoutCategory>();
    }

    public enum WorkoutType
    {
        Repetitions,
        Time,
        Weight
    }
} 