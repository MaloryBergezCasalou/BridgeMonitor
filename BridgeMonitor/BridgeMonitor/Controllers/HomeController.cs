using BridgeMonitor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BridgeMonitor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<BoatModel> apiInfo = GetFromAPI();
            apiInfo.Sort((s1, s2) => DateTimeOffset.Compare(s1.ClosingDate, s2.ClosingDate));

            foreach (var close in apiInfo)
            {
                if (DateTimeOffset.Compare(DateTimeOffset.Now, close.ClosingDate) < 0)
                {
                    ViewData["Info0"] = close;
                    break;
                }
            }

            return View();
        }

        public IActionResult AllClose()
        {
            ViewData["Info1"] = GetFromAPI();
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public static List<BoatModel> GetFromAPI()
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync("https://api.alexandredubois.com/pont-chaban/api.php");
                var stringResult = response.Result.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<BoatModel>>(stringResult.Result);
                return result;
            }
        }
    }
}
