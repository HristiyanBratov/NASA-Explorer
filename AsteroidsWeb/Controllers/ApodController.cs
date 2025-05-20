using Asteroids.Services.Contracts;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;

namespace AsteroidsWeb.Controllers
{
    public class ApodController : Controller
    {
        private readonly INasaApiService _nasaApiService;
        private readonly ILogger<ApodController> _logger;

        public ApodController(INasaApiService nasaApiService, ILogger<ApodController> logger)
        {
            _nasaApiService = nasaApiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(DateTime? date)
        {
            try
            {
                var selectedDate = date ?? DateTime.Today;

                var apod = await _nasaApiService.GetApodAsync(selectedDate);

                ViewBag.SelectedDate = selectedDate.ToString("yyyy-MM-dd");

                return View(apod);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "NASA API request failed. ");
                ViewBag.Error = "NASA API request failed: " + ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Index action.");
                ViewBag.Error = "An unexpected error occurred: " + ex.Message;
            }

            return View(null);
        }
    }
}