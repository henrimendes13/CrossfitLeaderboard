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
        public string Description { get; set; } = string.Empty;
        
        public WorkoutType Type { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Unit { get; set; } = string.Empty;
    }

    public enum WorkoutType
    {
        Repetitions,
        Time,
        Weight
    }
} 