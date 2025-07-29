using System.ComponentModel.DataAnnotations;

namespace CrossfitLeaderboard.Entities
{
    public class Team
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public int TotalPoints { get; set; }
        
        // Foreign key para Category
        public int? CategoryId { get; set; }
        
        // Navigation properties
        public virtual Category? Category { get; set; }
        public virtual ICollection<WorkoutResult> WorkoutResults { get; set; } = new List<WorkoutResult>();
    }
} 