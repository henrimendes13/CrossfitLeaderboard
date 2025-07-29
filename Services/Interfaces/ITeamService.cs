using CrossfitLeaderboard.Entities;

namespace CrossfitLeaderboard.Services.Interfaces
{
    public interface ITeamService
    {
        Task<IEnumerable<Team>> GetAllTeamsAsync();
        Task<Team?> GetTeamByIdAsync(int id);
        Task<Team> CreateTeamAsync(Team team);
        Task<Team> UpdateTeamAsync(Team team);
        Task<bool> DeleteTeamAsync(int id);
        Task<int> GetTeamTotalPointsAsync(int teamId);
        Task<IEnumerable<Team>> GetTeamsOrderedByPointsAsync();
    }
} 