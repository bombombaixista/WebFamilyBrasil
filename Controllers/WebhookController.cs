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

    public WebhookController(AppDbContext context, IHttpClientFactory httpFactory)
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

            if (!body.TryGetProperty("topic", out var topicProp))
                return Ok();

            var resourceUrl = resourceProp.GetString();
            var userId = userIdProp.GetInt64();
            var topic = topicProp.GetString();

            // 🔹 Garantir que o resource seja URL absoluta
            if (!string.IsNullOrEmpty(resourceUrl) && !resourceUrl.StartsWith("http"))
                resourceUrl = "https://api.mercadolibre.com" + resourceUrl;

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
            {
                var erro = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao buscar dados na API: {response.StatusCode} - {erro}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            switch (topic)
            {
                case "orders":
                    await TratarPedido(root, cliente, json, jsonRecebido);
                    break;

                case "items":
                    await TratarItem(root, cliente, json, jsonRecebido);
                    break;

                case "questions":
                    await TratarPergunta(root, cliente, json, jsonRecebido);
                    break;

                default:
                    _context.LogIntegracoes.Add(new LogIntegracao
                    {
                        Tipo = "Webhook",
                        Marketplace = "MercadoLivre",
                        Evento = $"EventoNaoTratado:{topic}",
                        Conteudo = jsonRecebido,
                        Sucesso = true
                    });
                    break;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex)
        {
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

    private async Task TratarPedido(JsonElement order, Cliente2 cliente, string json, string jsonRecebido)
    {
        var orderId = order.GetProperty("id").ToString();

        var pedido = await _context.Pedidos
            .FirstOrDefaultAsync(p => p.MarketplaceOrderId == orderId);

        if (pedido == null)
        {
            pedido = new Pedido
            {
                Cliente2Id = cliente.Id,
                MarketplaceOrderId = orderId,
                Cliente = order.GetProperty("buyer").GetProperty("nickname").ToString(),
                ValorTotal = order.GetProperty("total_amount").GetDecimal(),
                Data = order.GetProperty("date_created").GetDateTime(),
                Origem = "MercadoLivre"
            };

            _context.Pedidos.Add(pedido);
        }

        pedido.Status = order.GetProperty("status").ToString();
        pedido.JsonOriginal = json;
        pedido.UltimaAtualizacao = DateTime.UtcNow;

        _context.LogIntegracoes.Add(new LogIntegracao
        {
            Tipo = "Webhook",
            Marketplace = "MercadoLivre",
            Evento = "AtualizacaoPedido",
            Conteudo = jsonRecebido,
            Sucesso = true
        });
    }

    private async Task TratarItem(JsonElement item, Cliente2 cliente, string json, string jsonRecebido)
    {
        var itemId = item.GetProperty("id").GetString();
        var titulo = item.GetProperty("title").GetString();
        var preco = item.GetProperty("price").GetDecimal();

        _context.LogIntegracoes.Add(new LogIntegracao
        {
            Tipo = "Webhook",
            Marketplace = "MercadoLivre",
            Evento = "AtualizacaoItem",
            Conteudo = $"Item {itemId} - {titulo} - R${preco}",
            Sucesso = true
        });
    }

    private async Task TratarPergunta(JsonElement question, Cliente2 cliente, string json, string jsonRecebido)
    {
        var perguntaId = question.GetProperty("id").GetString();
        var texto = question.GetProperty("text").GetString();

        _context.LogIntegracoes.Add(new LogIntegracao
        {
            Tipo = "Webhook",
            Marketplace = "MercadoLivre",
            Evento = "NovaPergunta",
            Conteudo = $"Pergunta {perguntaId}: {texto}",
            Sucesso = true
        });
    }
}
