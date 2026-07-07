using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.Controllers
{
    public class PlanoController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View(); // mostra os cards de planos
        }

        // Recebe escolha do plano via POST
        [HttpPost]
        public IActionResult Escolher(string plano)
        {
            // Opção 1: salvar na sessão
            HttpContext.Session.SetString("PlanoEscolhido", plano);
            return RedirectToAction("Register", "Login");

            // --- OU ---

            // Opção 2: passar direto como parâmetro na rota
            // return RedirectToAction("Register", "Login", new { nomePlano = plano });
        }
    }
}
