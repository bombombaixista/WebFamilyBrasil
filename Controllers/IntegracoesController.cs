using Kanban.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.Controllers
{
    [Authorize]
    [Route("Integracoes")]
    public class IntegracoesController : Controller
    {
        private readonly MercadoLivreService _mlService;

        public IntegracoesController(MercadoLivreService mlService)
        {
            _mlService = mlService;
        }

        // ===============================
        // 🔄 BUSCAR PRODUTOS DO MERCADO LIVRE
        // ===============================
        [HttpPost("SincronizarMercadoLivre")]
        public async Task<IActionResult> SincronizarMercadoLivre(string query = "tenis", int limit = 20)
        {
            try
            {
                // Consulta produtos em tempo real
                var produtos = await _mlService.BuscarProdutosAsync(query, limit);

                TempData["Sucesso"] = $"Foram carregados {produtos.Count} produtos do Mercado Livre!";
                return RedirectToAction("ProdutosAfiliados", "Afiliados");
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao buscar produtos: " + ex.Message;
                return RedirectToAction("ProdutosAfiliados", "Afiliados");
            }
        }
    }
}
