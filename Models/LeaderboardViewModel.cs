using CrossfitLeaderboard.Entities;

namespace CrossfitLeaderboard.Models
{
    public class LeaderboardViewModel
    {
        public List<Team> Teams { get; set; } = new List<Team>();
        public List<Workout> Workouts { get; set; } = new List<Workout>();
        public List<WorkoutResult> Results { get; set; } = new List<WorkoutResult>();
        public Dictionary<int, Dictionary<int, WorkoutResult>> ResultsMatrix { get; set; } = new Dictionary<int, Dictionary<int, WorkoutResult>>();
    }

    public class UpdateResultModel
    {
        public int TeamId { get; set; }
        public int WorkoutId { get; set; }
        public decimal Result { get; set; }
    }
} 