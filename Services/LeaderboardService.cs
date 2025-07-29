using CrossfitLeaderboard.Data;
using CrossfitLeaderboard.Entities;
using CrossfitLeaderboard.Models;
using CrossfitLeaderboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrossfitLeaderboard.Services
{
    public class LeaderboardService
    {
        private readonly ITeamService _teamService;
        private readonly IWorkoutService _workoutService;
        private readonly ApplicationDbContext _context;

        public LeaderboardService(
            ITeamService teamService,
            IWorkoutService workoutService,
            ApplicationDbContext context)
        {
            _teamService = teamService;
            _workoutService = workoutService;
            _context = context;
        }

        public async Task<LeaderboardViewModel> GetLeaderboardAsync(int? categoryId = null)
        {
            IQueryable<Team> teamsQuery;
            IQueryable<Workout> workoutsQuery;
            
            if (categoryId.HasValue)
            {
                // Filtrar por categoria específica
                teamsQuery = _context.Teams
                    .Include(t => t.Category)
                    .Where(t => t.CategoryId == categoryId);
                
                workoutsQuery = _context.Workouts
                    .Include(w => w.WorkoutCategories)
                    .Where(w => w.WorkoutCategories.Any(wc => wc.CategoryId == categoryId));
            }
            else
            {
                // Buscar todos os times e workouts
                teamsQuery = _context.Teams.Include(t => t.Category);
                workoutsQuery = _context.Workouts.Include(w => w.WorkoutCategories);
            }
            
            var teams = await teamsQuery.ToListAsync();
            var workouts = await workoutsQuery.ToListAsync();
            
            // Buscar resultados apenas para os times e workouts filtrados
            var teamIds = teams.Select(t => t.Id).ToList();
            var workoutIds = workouts.Select(w => w.Id).ToList();
            
            var results = await _context.WorkoutResults
                .Include(r => r.Team)
                .Include(r => r.Workout)
                .Where(r => teamIds.Contains(r.TeamId) && workoutIds.Contains(r.WorkoutId))
                .ToListAsync();

            var viewModel = new LeaderboardViewModel
            {
                Teams = teams.ToList(),
                Workouts = workouts.ToList(),
                Results = results.ToList()
            };

            // Criar matriz de resultados para facilitar o acesso na view
            viewModel.ResultsMatrix = new Dictionary<int, Dictionary<int, WorkoutResult>>();
            
            foreach (var team in teams)
            {
                viewModel.ResultsMatrix[team.Id] = new Dictionary<int, WorkoutResult>();
                
                foreach (var workout in workouts)
                {
                    var result = results.FirstOrDefault(r => r.TeamId == team.Id && r.WorkoutId == workout.Id);
                    
                    if (result != null)
                    {
                        viewModel.ResultsMatrix[team.Id][workout.Id] = result;
                    }
                    else
                    {
                        // Criar um resultado vazio se não existir
                        var emptyResult = new WorkoutResult
                        {
                            Id = 0,
                            TeamId = team.Id,
                            WorkoutId = workout.Id,
                            Result = 0,
                            Position = 0,
                            Points = 0
                        };
                        viewModel.ResultsMatrix[team.Id][workout.Id] = emptyResult;
                    }
                }
            }

            return viewModel;
        }

        public async Task UpdateResultAsync(int teamId, int workoutId, decimal result)
        {
            var workoutResult = await _context.WorkoutResults
                .FirstOrDefaultAsync(r => r.TeamId == teamId && r.WorkoutId == workoutId);
            
            if (workoutResult == null)
            {
                // Criar novo resultado se não existir
                workoutResult = new WorkoutResult
                {
                    TeamId = teamId,
                    WorkoutId = workoutId,
                    Result = result
                };
                _context.WorkoutResults.Add(workoutResult);
            }
            else
            {
                // Atualizar resultado existente
                workoutResult.Result = result;
                _context.WorkoutResults.Update(workoutResult);
            }

            await _context.SaveChangesAsync();

            // Recalcular posições e pontos para este workout
            await CalculatePositionsAndPointsAsync(workoutId);
        }

        private async Task CalculatePositionsAndPointsAsync(int workoutId)
        {
            var workout = await _workoutService.GetWorkoutByIdAsync(workoutId);
            if (workout == null) return;

            var workoutResults = await _context.WorkoutResults
                .Where(r => r.WorkoutId == workoutId && r.Result > 0)
                .ToListAsync();

            if (workoutResults.Any())
            {
                // Ordenar baseado no tipo de workout
                List<WorkoutResult> sortedResults;
                switch (workout.Type)
                {
                    case WorkoutType.Repetitions:
                    case WorkoutType.Weight:
                        // Mais é melhor (repetições e peso)
                        sortedResults = workoutResults.OrderByDescending(r => r.Result).ToList();
                        break;
                    case WorkoutType.Time:
                        // Menos é melhor (tempo)
                        sortedResults = workoutResults.OrderBy(r => r.Result).ToList();
                        break;
                    default:
                        sortedResults = workoutResults.ToList();
                        break;
                }

                // Atribuir posições e pontos
                for (int i = 0; i < sortedResults.Count; i++)
                {
                    sortedResults[i].Position = i + 1;
                    sortedResults[i].Points = i + 1;
                }

                // Atualizar no banco
                foreach (var result in sortedResults)
                {
                    _context.WorkoutResults.Update(result);
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task ResetLeaderboardAsync()
        {
            var allResults = await _context.WorkoutResults.ToListAsync();
            _context.WorkoutResults.RemoveRange(allResults);
            await _context.SaveChangesAsync();
        }
    }
} 