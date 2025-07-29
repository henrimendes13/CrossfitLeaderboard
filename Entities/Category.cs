using System.ComponentModel.DataAnnotations;

namespace CrossfitLeaderboard.Entities
{
    public class Category
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        // Navigation properties
        public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
        public virtual ICollection<WorkoutCategory> WorkoutCategories { get; set; } = new List<WorkoutCategory>();
    }
} 