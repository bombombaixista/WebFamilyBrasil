using Microsoft.AspNetCore.Mvc;
using Kanban.Services;
using Microsoft.AspNetCore.Authorization;

namespace Kanban.Controllers
{
    [Authorize]
    public class BolsaController : Controller
    {
        private readonly BolsaService _service;

        public BolsaController(BolsaService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Monitor()
        {
            var ativos = await _service.ObterAtivosAsync();
            return View(ativos);
        }
    }
}