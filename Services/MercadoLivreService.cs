using System.Net.Http;
using System.Text.Json;
using Kanban.Models;

namespace Kanban.Services
{
    public class MercadoLivreService
    {
        private readonly HttpClient _httpClient;

        public MercadoLivreService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Pedido>> ObterPedidosAsync(string accessToken, string sellerId)
        {
            var response = await _httpClient.GetAsync(
                $"https://api.mercadolibre.com/orders/search?seller={sellerId}&access_token={accessToken}");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);

            var pedidos = new List<Pedido>();

            if (doc.RootElement.TryGetProperty("results", out var results))
            {
                foreach (var item in results.EnumerateArray())
                {
                    var pedido = new Pedido
                    {
                        Id = item.GetProperty("id").GetInt64(),
                        Cliente = item.GetProperty("buyer").GetProperty("nickname").GetString() ?? "",
                        ValorTotal = item.GetProperty("total_amount").GetDecimal(),
                        Data = item.GetProperty("date_created").GetDateTime(),
                        Origem = "Mercado Livre"
                    };

                    pedidos.Add(pedido);
                }
            }

            return pedidos;
        }
    }
}
