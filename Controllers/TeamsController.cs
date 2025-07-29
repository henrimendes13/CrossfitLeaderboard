using CrossfitLeaderboard.Entities;
using CrossfitLeaderboard.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CrossfitLeaderboard.Controllers
{
    public class TeamsController : Controller
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        // GET: Teams
        public async Task<IActionResult> Index()
        {
            var teams = await _teamService.GetAllTeamsAsync();
            return View(teams);
        }

        // GET: Teams/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Team team)
        {
            if (ModelState.IsValid)
            {
                await _teamService.CreateTeamAsync(team);
                return RedirectToAction(nameof(Index));
            }
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
            return View(team);
        }

        // POST: Teams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,TotalPoints")] Team team)
        {
            if (id != team.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _teamService.UpdateTeamAsync(team);
                return RedirectToAction(nameof(Index));
            }
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