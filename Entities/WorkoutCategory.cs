namespace CrossfitLeaderboard.Entities
{
    public class WorkoutCategory
    {
        public int WorkoutId { get; set; }
        public int CategoryId { get; set; }
        
        // Navigation properties
        public virtual Workout Workout { get; set; } = null!;
        public virtual Category Category { get; set; } = null!;
    }
} 