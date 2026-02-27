using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

using Kanban.Models;

[ApiController]
[Route("webhook")]
public class WebhookController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _httpFactory;

    public WebhookController(
        AppDbContext context,
        IHttpClientFactory httpFactory)
    {
        _context = context;
        _httpFactory = httpFactory;
    }

    [HttpPost("mercadolivre")]
    public async Task<IActionResult> MercadoLivre([FromBody] JsonElement body)
    {
        string jsonRecebido = body.ToString();

        try
        {
            if (!body.TryGetProperty("resource", out var resourceProp))
                return Ok();

            if (!body.TryGetProperty("user_id", out var userIdProp))
                return Ok();

            var resourceUrl = resourceProp.GetString();
            var userId = userIdProp.GetInt64();

            var cliente = await _context.Clientes2
                .FirstOrDefaultAsync(c => c.MercadoLivreUserId == userId);

            if (cliente == null)
                return Ok();

            var http = _httpFactory.CreateClient();
            http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", cliente.MercadoLivreAccessToken);

            var response = await http.GetAsync(resourceUrl);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Erro ao buscar pedido na API.");

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var order = doc.RootElement;

            var orderId = order.GetProperty("id").ToString();

            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(p => p.MarketplaceOrderId == orderId);

            if (pedido == null)
            {
                pedido = new Pedido
                {
                    Cliente2Id = cliente.Id,
                    MarketplaceOrderId = orderId,
                    Cliente = order.GetProperty("buyer")
                                   .GetProperty("nickname").ToString(),
                    ValorTotal = order.GetProperty("total_amount").GetDecimal(),
                    Data = order.GetProperty("date_created").GetDateTime(),
                    Origem = "MercadoLivre"
                };

                _context.Pedidos.Add(pedido);
            }

            pedido.Status = order.GetProperty("status").ToString();
            pedido.JsonOriginal = json;
            pedido.UltimaAtualizacao = DateTime.UtcNow;

            // 🔎 LOG SUCESSO
            _context.LogIntegracoes.Add(new LogIntegracao
            {
                Tipo = "Webhook",
                Marketplace = "MercadoLivre",
                Evento = "AtualizacaoPedido",
                Conteudo = jsonRecebido,
                Sucesso = true
            });

            await _context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            // 🔎 LOG ERRO
            _context.LogIntegracoes.Add(new LogIntegracao
            {
                Tipo = "Webhook",
                Marketplace = "MercadoLivre",
                Evento = "ErroWebhook",
                Conteudo = jsonRecebido,
                Sucesso = false,
                Erro = ex.Message
            });

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}