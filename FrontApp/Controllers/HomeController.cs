using FrontApp.Data;
using FrontApp.Models;
using FrontApp.Util;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Diagnostics;

namespace FrontApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRedisService _redis;
        ICacheTelemetry _cacheTelemetry;

        // home controller
        public HomeController(ILogger<HomeController> logger, IRedisService redis,
            ICacheTelemetry cacheTelemetry)
        {
            _logger = logger;
            _redis = redis;
            _cacheTelemetry = cacheTelemetry;
        }

        // Getting Weather info from API service.
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

        // Access Redis Cache in k8s
        public async Task<IActionResult> Redis()
        {
            string cachedTimeUtc = string.Empty;
            if (_redis != null)
            {
                _cacheTelemetry.Start();
                cachedTimeUtc = await _redis.GetAsync("CachedTimeInUTC");

                if (string.IsNullOrEmpty(cachedTimeUtc))
                {
                    var Now = DateTime.UtcNow.ToString();
                    TimeSpan cacheExpire = new TimeSpan(0, 0, 10);
                    await _redis.SetAsync("CachedTimeInUTC", Now, cacheExpire);
                    cachedTimeUtc = Now;
                    _cacheTelemetry.End(CacheTelemetryNames.GetRedisMiss, "CachedTimeInUTC");
                }
                else
                {
                    _cacheTelemetry.End(CacheTelemetryNames.GetRedisHit, "CachedTimeInUTC");
                }
                ViewBag.CachedTimeUTC = cachedTimeUtc;
            }
            return View();
        }

        // Edit 
        [HttpPost]
        public async Task<string> Edit(int money, string name)
        {
            if (money <= 0)
            {
                return $"{name} got nothing";
            }

            int delay =
                System.Security.Cryptography.RandomNumberGenerator.GetInt32(100,500 );

            await Task.Delay(delay);

            return $"{name} got {money} USD!!";
        }

        // generate 500 internal server error
        public IActionResult ServerError()
        {
            throw new Exception("Server Error!!!");
            return View();
        }

        // put random delay 
        public async Task<IActionResult> RandomDelay()
        {
            var delay = Random.Shared.Next(1, 10000);
            await Task.Delay(delay);
            ViewData["Delay"] = delay;
            return View();
        }

        // Privacy Page
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