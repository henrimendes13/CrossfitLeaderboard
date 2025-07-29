using CrossfitLeaderboard.Entities;
using CrossfitLeaderboard.Services.Interfaces;
using CrossfitLeaderboard.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrossfitLeaderboard.Controllers
{
    public class TeamsController : Controller
    {
        private readonly ITeamService _teamService;
        private readonly ApplicationDbContext _context;

        public TeamsController(ITeamService teamService, ApplicationDbContext context)
        {
            _teamService = teamService;
            _context = context;
        }

        // GET: Teams
        public async Task<IActionResult> Index()
        {
            var teams = await _teamService.GetAllTeamsAsync();
            return View(teams);
        }

        // GET: Teams/Create
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Categories = categories;
            return View();
        }

        // POST: Teams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,CategoryId")] Team team)
        {
            if (ModelState.IsValid)
            {
                // Verificar se já existe um time com o mesmo nome na mesma categoria
                var existingTeam = await _context.Teams
                    .FirstOrDefaultAsync(t => t.Name.ToLower() == team.Name.ToLower() && t.CategoryId == team.CategoryId);

                if (existingTeam != null)
                {
                    ModelState.AddModelError("Name", "Já existe um time com este nome nesta categoria.");
                    var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
                    ViewBag.Categories = categories;
                    return View(team);
                }

                await _teamService.CreateTeamAsync(team);
                TempData["SuccessMessage"] = "Time criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            var categoriesForView = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Categories = categoriesForView;
            return View(team);
        }

        // GET: Teams/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);
            if (team == null)
            {
                return NotFound();
            }

            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Categories = categories;
            return View(team);
        }

        // POST: Teams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,TotalPoints,CategoryId")] Team team)
        {
            if (id != team.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Verificar se já existe um time com o mesmo nome na mesma categoria (excluindo o atual)
                var existingTeam = await _context.Teams
                    .FirstOrDefaultAsync(t => t.Name.ToLower() == team.Name.ToLower() && 
                                            t.CategoryId == team.CategoryId && 
                                            t.Id != id);

                if (existingTeam != null)
                {
                    ModelState.AddModelError("Name", "Já existe um time com este nome nesta categoria.");
                    var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
                    ViewBag.Categories = categories;
                    return View(team);
                }

                await _teamService.UpdateTeamAsync(team);
                TempData["SuccessMessage"] = "Time atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            var categoriesForView = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Categories = categoriesForView;
            return View(team);
        }

        // GET: Teams/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _teamService.DeleteTeamAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
} 