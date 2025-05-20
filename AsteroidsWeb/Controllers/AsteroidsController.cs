using Asteroids.Models;
using Asteroids.Services.Contracts;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;

namespace AsteroidsWeb.Controllers
{
    public class AsteroidsController : Controller
    {
        private readonly INasaApiService _nasaApiService;
        private readonly IExportToExcelService _excelExportService;
        private readonly ILogger<AsteroidsController> _logger;

        public AsteroidsController(INasaApiService nasaApiService, IExportToExcelService excelExportService, ILogger<AsteroidsController> logger)
        {
            _nasaApiService = nasaApiService;
            _excelExportService = excelExportService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var today = DateTime.Now;
                var start = startDate ?? today;
                var end = endDate ?? today;

                var asteroids = await _nasaApiService.GetAsteroidsAsync(start, end);

                HttpContext.Session.SetString("LastAsteroidsData", JsonSerializer.Serialize(asteroids));
                HttpContext.Session.SetString("LastStartDate", start.ToString("yyyy-MM-dd"));
                HttpContext.Session.SetString("LastEndDate", end.ToString("yyyy-MM-dd"));

                ViewBag.Today = today.ToString("yyyy-MM-dd");
                ViewBag.StartDate = start.ToString("yyyy-MM-dd");
                ViewBag.EndDate = end.ToString("yyyy-MM-dd");

                return View(asteroids);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "NASA API request failed for dates: {Start} to {End}", startDate, endDate);
                ViewBag.Error = "NASA API request failed: " + ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Index action.");
                ViewBag.Error = "An unexpected error occurred: " + ex.Message;
            }

            return View(new List<Asteroid>());
        }

        public IActionResult Sort(string sortBy, string sortOrder)
        {
            var json = HttpContext.Session.GetString("LastAsteroidsData");

            if (string.IsNullOrEmpty(json))
                return RedirectToAction("Index"); 

            var asteroids = JsonSerializer.Deserialize<List<Asteroid>>(json);

            sortBy = sortBy?.ToLower();
            sortOrder = sortOrder?.ToLower();
            bool isDesc = sortOrder == "desc";

            asteroids = sortBy switch
            {
                "name" => isDesc ? asteroids.OrderByDescending(a => a.Name).ToList() : asteroids.OrderBy(a => a.Name).ToList(),
                "date" => isDesc ? asteroids.OrderByDescending(a => a.CloseApproachDate).ToList() : asteroids.OrderBy(a => a.CloseApproachDate).ToList(),
                "diameter" => isDesc ? asteroids.OrderByDescending(a => a.EstimatedDiameterKm).ToList() : asteroids.OrderBy(a => a.EstimatedDiameterKm).ToList(),
                "velocity" => isDesc ? asteroids.OrderByDescending(a => a.RelativeVelocityKph).ToList() : asteroids.OrderBy(a => a.RelativeVelocityKph).ToList(),
                "distance" => isDesc ? asteroids.OrderByDescending(a => a.MissDistanceKm).ToList() : asteroids.OrderBy(a => a.MissDistanceKm).ToList(),
                "hazardous" => isDesc ? asteroids.OrderByDescending(a => a.IsPotentiallyHazardous).ToList() : asteroids.OrderBy(a => a.IsPotentiallyHazardous).ToList(),
                _ => asteroids
            };

            var startDate = HttpContext.Session.GetString("LastStartDate") ?? DateTime.Today.ToString("yyyy-MM-dd");
            var endDate = HttpContext.Session.GetString("LastEndDate") ?? DateTime.Today.ToString("yyyy-MM-dd");

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder;

            return View("Index", asteroids);
        }


        [HttpGet]
        public async Task<IActionResult> ExportToExcel()
        {
            var json = HttpContext.Session.GetString("LastAsteroidsData");
            var startDateStr = HttpContext.Session.GetString("LastStartDate");
            var endDateStr = HttpContext.Session.GetString("LastEndDate");

            if (string.IsNullOrEmpty(json))
                return RedirectToAction(nameof(Index));

            var asteroids = JsonSerializer.Deserialize<List<Asteroid>>(json);

            var fileContent = _excelExportService.ExportToExcel(asteroids);

            if (!DateTime.TryParse(startDateStr, out var startDate)) 
                startDate = DateTime.Today;

            if (!DateTime.TryParse(endDateStr, out var endDate)) 
                endDate = DateTime.Today;

            string fileName = $"Asteroids_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx";

            return File(fileContent,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
    }
}