using CrossfitLeaderboard.Entities;
using CrossfitLeaderboard.Services.Interfaces;
using CrossfitLeaderboard.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrossfitLeaderboard.Controllers
{
    public class WorkoutsController : Controller
    {
        private readonly IWorkoutService _workoutService;
        private readonly ApplicationDbContext _context;

        public WorkoutsController(IWorkoutService workoutService, ApplicationDbContext context)
        {
            _workoutService = workoutService;
            _context = context;
        }

        // GET: Workouts
        public async Task<IActionResult> Index()
        {
            var workouts = await _workoutService.GetAllWorkoutsAsync();
            return View(workouts);
        }

        // GET: Workouts/Create
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Categories = categories;
            return View();
        }

        // POST: Workouts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Type,Unit")] Workout workout, List<int> categoryIds)
        {
            if (ModelState.IsValid)
            {
                await _workoutService.CreateWorkoutAsync(workout);

                // Adicionar relacionamentos com categorias
                if (categoryIds != null && categoryIds.Any())
                {
                    foreach (var categoryId in categoryIds)
                    {
                        var workoutCategory = new WorkoutCategory
                        {
                            WorkoutId = workout.Id,
                            CategoryId = categoryId
                        };
                        _context.WorkoutCategories.Add(workoutCategory);
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Workout criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            var categoriesForView = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Categories = categoriesForView;
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

            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            var selectedCategories = await _context.WorkoutCategories
                .Where(wc => wc.WorkoutId == id)
                .Select(wc => wc.CategoryId)
                .ToListAsync();

            ViewBag.Categories = categories;
            ViewBag.SelectedCategories = selectedCategories;
            return View(workout);
        }

        // POST: Workouts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Type,Unit")] Workout workout, List<int> categoryIds)
        {
            if (id != workout.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _workoutService.UpdateWorkoutAsync(workout);

                // Atualizar relacionamentos com categorias
                var existingCategories = await _context.WorkoutCategories
                    .Where(wc => wc.WorkoutId == id)
                    .ToListAsync();

                _context.WorkoutCategories.RemoveRange(existingCategories);

                if (categoryIds != null && categoryIds.Any())
                {
                    foreach (var categoryId in categoryIds)
                    {
                        var workoutCategory = new WorkoutCategory
                        {
                            WorkoutId = workout.Id,
                            CategoryId = categoryId
                        };
                        _context.WorkoutCategories.Add(workoutCategory);
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Workout atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            var categoriesForView = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Categories = categoriesForView;
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