using Microsoft.AspNetCore.Mvc;

namespace Kanban.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
