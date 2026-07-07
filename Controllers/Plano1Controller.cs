using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.Controllers
{
    public class Plano1Controller : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
