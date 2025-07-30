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

        public async Task<object> GetLeaderboardDtoAsync(int? categoryId = null)
        {
            var leaderboard = await GetLeaderboardAsync(categoryId);
            
            return new
            {
                teams = leaderboard.Teams.Select(t => new
                {
                    id = t.Id,
                    name = t.Name,
                    totalPoints = t.TotalPoints,
                    categoryId = t.CategoryId,
                    categoryName = t.Category?.Name
                }),
                workouts = leaderboard.Workouts.Select(w => new
                {
                    id = w.Id,
                    name = w.Name,
                    description = w.Description,
                    unit = w.Unit,
                    type = w.Type
                }),
                results = leaderboard.Results.Select(r => new
                {
                    teamId = r.TeamId,
                    workoutId = r.WorkoutId,
                    result = r.Result,
                    position = r.Position,
                    points = r.Points
                })
            };
        }

        public async Task UpdateResultAsync(int teamId, int workoutId, decimal? result)
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

            // Recalcular posições e pontos para este workout por categoria
            await CalculatePositionsAndPointsByCategoryAsync(workoutId);
            
            // Recalcular pontos totais por categoria
            await RecalculateTotalPointsByCategoryAsync();
        }

        private async Task RecalculateTotalPointsByCategoryAsync()
        {
            // Buscar todos os times com suas categorias
            var teams = await _context.Teams
                .Include(t => t.Category)
                .Where(t => t.CategoryId.HasValue)
                .ToListAsync();

            // Agrupar times por categoria
            var teamsByCategory = teams.GroupBy(t => t.CategoryId.Value).ToList();

            foreach (var categoryGroup in teamsByCategory)
            {
                var categoryId = categoryGroup.Key;
                var teamsInCategory = categoryGroup.ToList();

                // Buscar workouts aplicáveis a esta categoria
                var workoutsForCategory = await _context.Workouts
                    .Include(w => w.WorkoutCategories)
                    .Where(w => w.WorkoutCategories.Any(wc => wc.CategoryId == categoryId))
                    .ToListAsync();

                var workoutIds = workoutsForCategory.Select(w => w.Id).ToList();

                // Para cada time na categoria, calcular pontos totais apenas dos workouts aplicáveis
                foreach (var team in teamsInCategory)
                {
                    var totalPoints = await _context.WorkoutResults
                        .Where(r => r.TeamId == team.Id && workoutIds.Contains(r.WorkoutId))
                        .SumAsync(r => r.Points);

                    team.TotalPoints = (int)totalPoints;
                    _context.Teams.Update(team);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task CalculatePositionsAndPointsByCategoryAsync(int workoutId)
        {
            var workout = await _workoutService.GetWorkoutByIdAsync(workoutId);
            if (workout == null) return;

            // Buscar todos os resultados deste workout
            var allWorkoutResults = await _context.WorkoutResults
                .Include(r => r.Team)
                .Where(r => r.WorkoutId == workoutId)
                .ToListAsync();

            // Agrupar resultados por categoria
            var resultsByCategory = allWorkoutResults
                .Where(r => r.Team.CategoryId.HasValue)
                .GroupBy(r => r.Team.CategoryId.Value)
                .ToList();

            foreach (var categoryGroup in resultsByCategory)
            {
                var categoryId = categoryGroup.Key;
                var categoryResults = categoryGroup.ToList();

                if (categoryResults.Any())
                {
                    // Separar resultados válidos (não null) dos inválidos
                    var validResults = categoryResults.Where(r => r.Result.HasValue).ToList();
                    var invalidResults = categoryResults.Where(r => !r.Result.HasValue).ToList();
                    var disqualifiedResults = categoryResults.Where(r => r.Result.HasValue && r.Result.Value == 0).ToList();

                    // Ordenar resultados válidos baseado no tipo de workout
                    List<WorkoutResult> sortedValidResults = new List<WorkoutResult>();
                    if (validResults.Any())
                    {
                        switch (workout.Type)
                        {
                            case WorkoutType.Repetitions:
                            case WorkoutType.Weight:
                                // Mais é melhor (repetições e peso)
                                sortedValidResults = validResults.OrderByDescending(r => r.Result.Value).ToList();
                                break;
                            case WorkoutType.Time:
                                // Menos é melhor (tempo)
                                sortedValidResults = validResults.OrderBy(r => r.Result.Value).ToList();
                                break;
                            default:
                                sortedValidResults = validResults.ToList();
                                break;
                        }
                    }

                    // Calcular posições
                    int currentPosition = 1;

                    // 1. Atribuir posições para resultados válidos (não 0)
                    var nonZeroResults = sortedValidResults.Where(r => r.Result.Value > 0).ToList();
                    foreach (var result in nonZeroResults)
                    {
                        result.Position = currentPosition;
                        result.Points = currentPosition;
                        currentPosition++;
                    }

                    // 2. Atribuir pior posição para resultados desclassificados (0)
                    var totalTeamsInCategory = categoryResults.Count;
                    foreach (var result in disqualifiedResults)
                    {
                        result.Position = totalTeamsInCategory;
                        result.Points = totalTeamsInCategory;
                    }

                    // 3. Manter posição 0 para resultados não feitos (null)
                    foreach (var result in invalidResults)
                    {
                        result.Position = 0;
                        result.Points = 0;
                    }

                    // Atualizar no banco
                    foreach (var result in categoryResults)
                    {
                        _context.WorkoutResults.Update(result);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task ResetLeaderboardAsync()
        {
            // Remove all workout results
            var allResults = await _context.WorkoutResults.ToListAsync();
            _context.WorkoutResults.RemoveRange(allResults);
            
            // Reset total points for all teams
            var allTeams = await _context.Teams.ToListAsync();
            foreach (var team in allTeams)
            {
                team.TotalPoints = 0;
                _context.Teams.Update(team);
            }
            
            await _context.SaveChangesAsync();
        }
    }
} 