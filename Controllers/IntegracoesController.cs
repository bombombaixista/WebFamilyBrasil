using Kanban.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kanban.Controllers
{
    [Authorize]
    [Route("Integracoes")]
    public class IntegracoesController : Controller
    {
        private readonly MercadoLivreService _mlService;
        private readonly AppDbContext _context;

        public IntegracoesController(
            MercadoLivreService mlService,
            AppDbContext context)
        {
            _mlService = mlService;
            _context = context;
        }

        // ===============================
        // 🔄 SINCRONIZAR PEDIDOS ML
        // ===============================
        [HttpPost("SincronizarMercadoLivre")]
        public async Task<IActionResult> SincronizarMercadoLivre()
        {
            try
            {
                // 🔐 Pegar email do usuário logado
                var email = User.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(email))
                    return Unauthorized("Usuário não identificado.");

                // 🔎 Buscar Cliente2 no banco
                var cliente = _context.Clientes2
                    .FirstOrDefault(c => c.Email == email);

                if (cliente == null)
                    return NotFound("Cliente não encontrado no banco.");

                if (string.IsNullOrEmpty(cliente.MercadoLivreAccessToken))
                    return BadRequest("Token de acesso não encontrado.");

                // 🔄 Sincronizar
                await _mlService.SincronizarPedidosAsync(cliente);

                TempData["Sucesso"] = "Pedidos sincronizados com sucesso!";
                return RedirectToAction("Index", "Pedidos");
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao sincronizar: " + ex.Message;
                return RedirectToAction("Index", "Pedidos");
            }
        }
    }
}