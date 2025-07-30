using System.Diagnostics;
using CrossfitLeaderboard.Models;
using CrossfitLeaderboard.Services;
using CrossfitLeaderboard.Services.Interfaces;
using CrossfitLeaderboard.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrossfitLeaderboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LeaderboardService _leaderboardService;
        private readonly IPdfService _pdfService;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, LeaderboardService leaderboardService, IPdfService pdfService, ApplicationDbContext context)
        {
            _logger = logger;
            _leaderboardService = leaderboardService;
            _pdfService = pdfService;
            _context = context;
        }

        public async Task<IActionResult> Index(int? categoryId = null)
        {
            var leaderboard = await _leaderboardService.GetLeaderboardAsync(categoryId);
            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            
            ViewBag.CategoryId = categoryId;
            ViewBag.Categories = categories;
            return View(leaderboard);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateResult([FromBody] UpdateResultModel model)
        {
            if (ModelState.IsValid)
            {
                await _leaderboardService.UpdateResultAsync(model.TeamId, model.WorkoutId, model.Result);
                var leaderboardDto = await _leaderboardService.GetLeaderboardDtoAsync();
                
                return Json(new { success = true, leaderboard = leaderboardDto });
            }
            return Json(new { success = false, message = "Dados inválidos" });
        }

        [HttpPost]
        public async Task<IActionResult> ResetLeaderboard()
        {
            await _leaderboardService.ResetLeaderboardAsync();
            var leaderboardDto = await _leaderboardService.GetLeaderboardDtoAsync();
            
            return Json(new { success = true, leaderboard = leaderboardDto });
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPdf()
        {
            try
            {
                var leaderboard = await _leaderboardService.GetLeaderboardAsync();
                var pdfBytes = await _pdfService.GenerateLeaderboardPdfAsync(leaderboard);
                
                string fileName = $"CrossFit_Leaderboard_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar PDF");
                return RedirectToAction("Index");
            }
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
