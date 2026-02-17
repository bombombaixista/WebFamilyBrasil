using System.Net.Http;
using Kanban.Models;

namespace WebFamily.Services
{
    public class AliExpressService
    {
        private readonly HttpClient _httpClient;

        public AliExpressService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Pedido>> ObterPedidosAsync(string accessToken)
        {
            // Exemplo fictício de chamada à API da AliExpress
            var response = await _httpClient.GetAsync("https://api.aliexpress.com/orders?token=" + accessToken);
            var json = await response.Content.ReadAsStringAsync();

            return new List<Pedido>
            {
                new Pedido { Id = 3, Cliente = "Cliente AliExpress", Data = DateTime.UtcNow, ValorTotal = 300, Origem = "AliExpress" }
            };
        }
    }
}
