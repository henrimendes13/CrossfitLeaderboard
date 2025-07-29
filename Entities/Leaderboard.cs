namespace CrossfitLeaderboard.Entities
{
    public class Leaderboard
    {
        public Guid Id { get; set; }
        public required string Category { get; set; }
        public List<Team> Teams { get; set; } = new List<Team>();
        public List<Workout> Workouts { get; set; } = new List<Workout>();
        public List<WorkoutResult> WorkoutResults { get; set; } = new List<WorkoutResult>();
    }
}
