using System.Net.Http;
using Kanban.Models;

namespace Kanban.Services
{
    public class ShopeeService
    {
        private readonly HttpClient _httpClient;

        public ShopeeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Pedido>> ObterPedidosAsync(string accessToken)
        {
            // Exemplo fictício de chamada à API da Shopee
            var response = await _httpClient.GetAsync("https://api.shopee.com/orders?token=" + accessToken);
            var json = await response.Content.ReadAsStringAsync();

            return new List<Pedido>
            {
                new Pedido { Id = 2, Cliente = "Cliente Shopee", Data = DateTime.UtcNow, ValorTotal = 200, Origem = "Shopee" }
            };
        }
    }
}
