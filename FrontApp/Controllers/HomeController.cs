using FrontApp.Data;
using FrontApp.Models;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Diagnostics;

namespace FrontApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRedisService _redis;

        public HomeController(ILogger<HomeController> logger, IRedisService redis)
        {
            _logger = logger;
            _redis = redis;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Message"] = "Hello from webfrontend";

            using (var client = new System.Net.Http.HttpClient())
            {
                // Call *mywebapi*, and display its response in the page
                var request = new System.Net.Http.HttpRequestMessage();
                request.RequestUri = new Uri("http://ApiApp7/WeatherForecast"); 
                var response = await client.SendAsync(request);
                ViewData["Message"] += " and " + await response.Content.ReadAsStringAsync();
            }
            return View();
        }

        public async Task<IActionResult> Redis()
        {
            string cachedTimeUtc = string.Empty;
            if (_redis != null)
            {
                cachedTimeUtc = await _redis.GetAsync("CachedTimeInUTC");

                if (string.IsNullOrEmpty(cachedTimeUtc))
                {
                    var Now = DateTime.UtcNow.ToString();
                    TimeSpan cacheExpire = new TimeSpan(0, 0, 10);
                    await _redis.SetAsync("CachedTimeInUTC", Now, cacheExpire);
                    cachedTimeUtc = Now;
                }                
                ViewBag.CachedTimeUTC = cachedTimeUtc;
            }
            return View();
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