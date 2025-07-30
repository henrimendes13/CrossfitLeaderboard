using CrossfitLeaderboard.Models;

namespace CrossfitLeaderboard.Services.Interfaces
{
    public interface IPdfService
    {
        Task<byte[]> GenerateLeaderboardPdfAsync(LeaderboardViewModel leaderboard);
    }
} 