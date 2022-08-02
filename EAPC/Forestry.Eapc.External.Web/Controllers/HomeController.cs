using System.Diagnostics;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Infrastructure;
using Forestry.Eapc.External.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Forestry.Eapc.External.Web.Controllers
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
            if (User.IsLoggedIn())
            {
                return RedirectToAction("Index","Applications");
            }

            return View();
        }

        public IActionResult Login()
        {
            if (User.IsLoggedIn())
            {
                return RedirectToAction("Index","Applications");
            }

            return Challenge();
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync();
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult CookiePolicy()
        {
            return View();
        }

        public IActionResult Accessibility()
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
