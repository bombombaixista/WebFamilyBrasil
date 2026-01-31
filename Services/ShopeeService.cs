using System.Net.Http;
using WebFamily.Models;

namespace WebFamily.Services
{
    public class ShopeeService
    {
        private readonly HttpClient _httpClient;

        public ShopeeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PedidoDto>> ObterPedidosAsync(string accessToken)
        {
            // Exemplo fictício de chamada à API da Shopee
            var response = await _httpClient.GetAsync("https://api.shopee.com/orders?token=" + accessToken);
            var json = await response.Content.ReadAsStringAsync();

            return new List<PedidoDto>
            {
                new PedidoDto { Id = 2, Cliente = "Cliente Shopee", Data = DateTime.UtcNow, Valor = 200, Origem = "Shopee" }
            };
        }
    }
}
