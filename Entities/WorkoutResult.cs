using System.ComponentModel.DataAnnotations;

namespace CrossfitLeaderboard.Entities
{
    public class WorkoutResult
    {
        public int Id { get; set; }
        
        public int TeamId { get; set; }
        
        public int WorkoutId { get; set; }
        
        public decimal? Result { get; set; } // Nullable para diferenciar entre não feito (null) e desclassificado (0)
        
        public int Position { get; set; }
        
        public int Points { get; set; }
        
        // Navigation properties
        public virtual Team? Team { get; set; }
        public virtual Workout? Workout { get; set; }
    }
} 