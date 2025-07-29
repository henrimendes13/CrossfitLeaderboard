using CrossfitLeaderboard.Entities;
using CrossfitLeaderboard.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CrossfitLeaderboard.Controllers
{
    public class WorkoutsController : Controller
    {
        private readonly IWorkoutService _workoutService;

        public WorkoutsController(IWorkoutService workoutService)
        {
            _workoutService = workoutService;
        }

        // GET: Workouts
        public async Task<IActionResult> Index()
        {
            var workouts = await _workoutService.GetAllWorkoutsAsync();
            return View(workouts);
        }

        // GET: Workouts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Workouts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Type,Unit")] Workout workout)
        {
            if (ModelState.IsValid)
            {
                await _workoutService.CreateWorkoutAsync(workout);
                return RedirectToAction(nameof(Index));
            }
            return View(workout);
        }

        // GET: Workouts/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var workout = await _workoutService.GetWorkoutByIdAsync(id);
            if (workout == null)
            {
                return NotFound();
            }
            return View(workout);
        }

        // POST: Workouts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Type,Unit")] Workout workout)
        {
            if (id != workout.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _workoutService.UpdateWorkoutAsync(workout);
                return RedirectToAction(nameof(Index));
            }
            return View(workout);
        }

        // GET: Workouts/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var workout = await _workoutService.GetWorkoutByIdAsync(id);
            if (workout == null)
            {
                return NotFound();
            }

            return View(workout);
        }

        // POST: Workouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _workoutService.DeleteWorkoutAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
} 