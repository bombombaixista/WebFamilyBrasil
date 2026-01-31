using System.Net.Http;
using WebFamily.Models;

namespace WebFamily.Services
{
    public class AliExpressService
    {
        private readonly HttpClient _httpClient;

        public AliExpressService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PedidoDto>> ObterPedidosAsync(string accessToken)
        {
            // Exemplo fictício de chamada à API da AliExpress
            var response = await _httpClient.GetAsync("https://api.aliexpress.com/orders?token=" + accessToken);
            var json = await response.Content.ReadAsStringAsync();

            return new List<PedidoDto>
            {
                new PedidoDto { Id = 3, Cliente = "Cliente AliExpress", Data = DateTime.UtcNow, Valor = 300, Origem = "AliExpress" }
            };
        }
    }
}
