using System.Diagnostics;
using CrossfitLeaderboard.Models;
using CrossfitLeaderboard.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrossfitLeaderboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LeaderboardService _leaderboardService;

        public HomeController(ILogger<HomeController> logger, LeaderboardService leaderboardService)
        {
            _logger = logger;
            _leaderboardService = leaderboardService;
        }

        public async Task<IActionResult> Index()
        {
            var leaderboard = await _leaderboardService.GetLeaderboardAsync();
            return View(leaderboard);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateResult([FromBody] UpdateResultModel model)
        {
            if (ModelState.IsValid)
            {
                await _leaderboardService.UpdateResultAsync(model.TeamId, model.WorkoutId, model.Result);
                var leaderboard = await _leaderboardService.GetLeaderboardAsync();
                return Json(new { success = true, leaderboard = leaderboard });
            }
            return Json(new { success = false, message = "Dados inválidos" });
        }

        [HttpPost]
        public async Task<IActionResult> ResetLeaderboard()
        {
            await _leaderboardService.ResetLeaderboardAsync();
            var leaderboard = await _leaderboardService.GetLeaderboardAsync();
            return Json(new { success = true, leaderboard = leaderboard });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
