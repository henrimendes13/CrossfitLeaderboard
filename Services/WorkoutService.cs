using CrossfitLeaderboard.Data;
using CrossfitLeaderboard.Entities;
using CrossfitLeaderboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrossfitLeaderboard.Services
{
    public class WorkoutService : IWorkoutService
    {
        private readonly ApplicationDbContext _context;

        public WorkoutService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Workout>> GetAllWorkoutsAsync()
        {
            return await _context.Workouts.ToListAsync();
        }

        public async Task<Workout?> GetWorkoutByIdAsync(int id)
        {
            return await _context.Workouts.FindAsync(id);
        }

        public async Task<Workout> CreateWorkoutAsync(Workout workout)
        {
            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();
            return workout;
        }

        public async Task<Workout> UpdateWorkoutAsync(Workout workout)
        {
            _context.Workouts.Update(workout);
            await _context.SaveChangesAsync();
            return workout;
        }

        public async Task<bool> DeleteWorkoutAsync(int id)
        {
            var workout = await _context.Workouts.FindAsync(id);
            if (workout != null)
            {
                _context.Workouts.Remove(workout);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Workout>> GetWorkoutsByTypeAsync(WorkoutType type)
        {
            return await _context.Workouts
                .Where(w => w.Type == type)
                .ToListAsync();
        }
    }
} 