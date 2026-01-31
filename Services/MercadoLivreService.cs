using System.Net.Http;
using System.Text.Json;
using WebFamily.Models;

namespace WebFamily.Services
{
    public class MercadoLivreService
    {
        private readonly HttpClient _httpClient;

        public MercadoLivreService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PedidoDto>> ObterPedidosAsync(string accessToken, string sellerId)
        {
            var response = await _httpClient.GetAsync(
                $"https://api.mercadolibre.com/orders/search?seller={sellerId}&access_token={accessToken}");

            var json = await response.Content.ReadAsStringAsync();
            // Aqui você faria o parse real da resposta da API
            return new List<PedidoDto>
            {
                new PedidoDto { Id = 1, Cliente = "Cliente ML", Data = DateTime.UtcNow, Valor = 100, Origem = "Mercado Livre" }
            };
        }
    }
}
