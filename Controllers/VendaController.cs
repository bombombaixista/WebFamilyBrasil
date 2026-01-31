using Microsoft.AspNetCore.Mvc;
using Kanban.Model;
using System;
using System.Linq;

namespace WebFamily.Controllers
{
    public class VendaController : Controller
    {
        private readonly AppDbContext _context;

        public VendaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Venda/Index
        public IActionResult Index()
        {
            ViewData["Title"] = "Planos WebFamily";

            // Carrega os planos cadastrados no banco
            var planos = _context.Planos.ToList();
            return View(planos); // a view deve ser tipada como IEnumerable<Plano>
        }

        // POST: /Venda/EscolherPlano
        [HttpPost]
        public IActionResult EscolherPlano(Guid PlanoId)
        {
            // guarda o plano escolhido em TempData
            TempData["PlanoId"] = PlanoId;
            TempData["Expiracao"] = DateTime.UtcNow.AddMonths(1).ToString("yyyy-MM-dd");

            // redireciona para tela de cadastro
            return RedirectToAction("Register", "User2");
        }
    }
}
