using FreelanceMarketPlace.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FreelanceMarketPlace.Controllers
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
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            ViewBag.CurrentUser = authToken;
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
