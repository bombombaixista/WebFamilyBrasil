using Microsoft.AspNetCore.Mvc;

namespace WebFamily.Controllers
{
    public class LandingController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "WebFamilyBrasil - Bem-vindo";
            return View();
        }
    }
}
