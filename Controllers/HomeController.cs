using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kanban.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var nome = User.Identity?.Name;
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var plano = User.Claims.FirstOrDefault(c => c.Type == "Plano")?.Value;

            ViewBag.Nome = nome;
            ViewBag.Email = email;
            ViewBag.Plano = plano;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
