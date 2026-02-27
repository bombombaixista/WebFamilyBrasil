using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Kanban.Models;

namespace Kanban.Controllers
{
    public class IntegracaoMercadoLivreController : Controller
    {
        private readonly AppDbContext _context;

        public IntegracaoMercadoLivreController(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 Página principal da integração
        public async Task<IActionResult> Index()
        {
            var totalPedidos = await _context.Pedidos
                .Where(p => p.Origem == "MercadoLivre")
                .CountAsync();

            var pedidosPagos = await _context.Pedidos
                .Where(p => p.Origem == "MercadoLivre" && p.Status == "paid")
                .CountAsync();

            var totalErros = await _context.LogIntegracoes
                .Where(l => l.Marketplace == "MercadoLivre" && !l.Sucesso)
                .CountAsync();

            ViewBag.TotalPedidos = totalPedidos;
            ViewBag.PedidosPagos = pedidosPagos;
            ViewBag.TotalErros = totalErros;

            return View();
        }

        // 🔹 Tela de Logs
        public async Task<IActionResult> Logs()
        {
            var logs = await _context.LogIntegracoes
                .Where(l => l.Marketplace == "MercadoLivre")
                .OrderByDescending(l => l.Data)
                .Take(200)
                .ToListAsync();

            return View(logs);
        }

        // 🔹 Monitoramento gráfico
        public async Task<IActionResult> Monitoramento()
        {
            var hoje = DateTime.UtcNow.Date;

            var sucessoHoje = await _context.LogIntegracoes
                .Where(l => l.Marketplace == "MercadoLivre"
                         && l.Sucesso
                         && l.Data >= hoje)
                .CountAsync();

            var erroHoje = await _context.LogIntegracoes
                .Where(l => l.Marketplace == "MercadoLivre"
                         && !l.Sucesso
                         && l.Data >= hoje)
                .CountAsync();

            ViewBag.SucessoHoje = sucessoHoje;
            ViewBag.ErroHoje = erroHoje;

            return View();
        }
    }
}