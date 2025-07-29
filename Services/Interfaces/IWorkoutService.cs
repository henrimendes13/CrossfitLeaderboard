using CrossfitLeaderboard.Entities;

namespace CrossfitLeaderboard.Services.Interfaces
{
    public interface IWorkoutService
    {
        Task<IEnumerable<Workout>> GetAllWorkoutsAsync();
        Task<Workout?> GetWorkoutByIdAsync(int id);
        Task<Workout> CreateWorkoutAsync(Workout workout);
        Task<Workout> UpdateWorkoutAsync(Workout workout);
        Task<bool> DeleteWorkoutAsync(int id);
        Task<IEnumerable<Workout>> GetWorkoutsByTypeAsync(WorkoutType type);
    }
} 