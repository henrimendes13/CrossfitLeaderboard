using CrossfitLeaderboard.Data;
using CrossfitLeaderboard.Entities;
using CrossfitLeaderboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrossfitLeaderboard.Services
{
    public class TeamService : ITeamService
    {
        private readonly ApplicationDbContext _context;

        public TeamService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Team>> GetAllTeamsAsync()
        {
            return await _context.Teams
                .Include(t => t.WorkoutResults)
                .ToListAsync();
        }

        public async Task<Team?> GetTeamByIdAsync(int id)
        {
            return await _context.Teams
                .Include(t => t.WorkoutResults)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Team> CreateTeamAsync(Team team)
        {
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
            return team;
        }

        public async Task<Team> UpdateTeamAsync(Team team)
        {
            _context.Teams.Update(team);
            await _context.SaveChangesAsync();
            return team;
        }

        public async Task<bool> DeleteTeamAsync(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team != null)
            {
                _context.Teams.Remove(team);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<int> GetTeamTotalPointsAsync(int teamId)
        {
            return await _context.WorkoutResults
                .Where(r => r.TeamId == teamId)
                .SumAsync(r => r.Points);
        }

        public async Task<IEnumerable<Team>> GetTeamsOrderedByPointsAsync()
        {
            var teams = await _context.Teams
                .Include(t => t.WorkoutResults)
                .ToListAsync();

            // Calcular pontos totais para cada equipe
            foreach (var team in teams)
            {
                team.TotalPoints = await GetTeamTotalPointsAsync(team.Id);
            }

            // Ordenar por pontos (menor para maior - vencedor tem menos pontos)
            return teams.OrderBy(t => t.TotalPoints);
        }
    }
} 