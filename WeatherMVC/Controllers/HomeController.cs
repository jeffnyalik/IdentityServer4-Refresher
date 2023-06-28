using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using WeatherMVC.Models;
using WeatherMVC.Services;

namespace WeatherMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITokenService _tokenService;

        public HomeController(ITokenService tokenService, ILogger<HomeController> logger)
        {
            _logger = logger;
            _tokenService = tokenService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Weather()
        {
            var data = new List<WeatherData>();
            using(var client = new HttpClient())
            {
                var tokenResponse = await _tokenService.GetToken("weatherapi.read");
                client.SetBearerToken(tokenResponse.AccessToken);
                var res = client
                    .GetAsync("https://localhost:5445/api/food").Result;
                if (res.IsSuccessStatusCode)
                {
                    var model = res.Content.ReadAsStringAsync().Result;
                    data = JsonConvert.DeserializeObject<List<WeatherData>>(model);
                    return View(data);
                }
                else
                {
                    return NotFound("Unable to handle request");
                }
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