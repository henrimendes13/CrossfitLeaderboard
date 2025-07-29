using CrossfitLeaderboard.Entities;
using CrossfitLeaderboard.Models;

namespace CrossfitLeaderboard.Services.Interfaces
{
    public interface ILeaderboardService
    {
        Task<LeaderboardViewModel> GetLeaderboardAsync();
        Task UpdateResultAsync(int teamId, int workoutId, decimal result);
        Task ResetLeaderboardAsync();
        Task<Dictionary<int, Dictionary<int, WorkoutResult>>> GetResultsMatrixAsync();
    }
} 