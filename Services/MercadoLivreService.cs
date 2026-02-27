using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Kanban.Models;

namespace Kanban.Services
{
    public class MercadoLivreService
    {
        private readonly HttpClient _http;
        private readonly AppDbContext _context;

        public MercadoLivreService(HttpClient http, AppDbContext context)
        {
            _http = http;
            _context = context;
        }

        public async Task SincronizarPedidosAsync(Cliente2 cliente)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", cliente.MercadoLivreAccessToken);

            int offset = 0;
            int limit = 50;
            bool continuar = true;

            while (continuar)
            {
                var url = $"https://api.mercadolibre.com/orders/search?sort=date_desc&limit={limit}&offset={offset}";
                var response = await _http.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Erro ao buscar pedidos no Mercado Livre.");

                var json = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var results = root.GetProperty("results");

                if (results.GetArrayLength() == 0)
                    break;

                foreach (var order in results.EnumerateArray())
                {
                    var orderId = order.GetProperty("id").ToString();

                    var pedidoExistente = await _context.Pedidos
                        .FirstOrDefaultAsync(p => p.MarketplaceOrderId == orderId);

                    var status = order.GetProperty("status").ToString();
                    var total = order.GetProperty("total_amount").GetDecimal();
                    var dataCriacao = order.GetProperty("date_created").GetDateTime();
                    var clienteNome = order.GetProperty("buyer")
                                           .GetProperty("nickname").ToString();

                    if (pedidoExistente == null)
                    {
                        var novoPedido = new Pedido
                        {
                            Cliente2Id = cliente.Id,
                            MarketplaceOrderId = orderId,
                            Cliente = clienteNome,
                            ValorTotal = total,
                            Data = dataCriacao,
                            Status = status,
                            Origem = "MercadoLivre",
                            JsonOriginal = order.ToString(),
                            DataCriacao = DateTime.UtcNow
                        };

                        _context.Pedidos.Add(novoPedido);
                    }
                    else
                    {
                        pedidoExistente.Status = status;
                        pedidoExistente.ValorTotal = total;
                        pedidoExistente.UltimaAtualizacao = DateTime.UtcNow;
                        pedidoExistente.JsonOriginal = order.ToString();

                        _context.Pedidos.Update(pedidoExistente);
                    }
                }

                await _context.SaveChangesAsync();

                offset += limit;

                int totalResultados = root.GetProperty("paging")
                                          .GetProperty("total")
                                          .GetInt32();

                if (offset >= totalResultados)
                    continuar = false;
            }
        }
    }
}