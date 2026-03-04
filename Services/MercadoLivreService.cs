using System.Net.Http;
using System.Text.Json;
using Kanban.Models;

namespace Kanban.Services
{
    public class MercadoLivreService
    {
        private readonly HttpClient _http;
        private readonly IMercadoLivreAuthService _authService;

        public MercadoLivreService(HttpClient http, IMercadoLivreAuthService authService)
        {
            _http = http;
            _authService = authService;
        }

        // 🔹 Consulta pública (sem token)
        public async Task<List<AfiliadoProduto>> BuscarProdutosAsync(string query = "tenis", int limit = 20)
        {
            return await BuscarProdutosInternoAsync(query, limit);
        }

        // 🔹 Consulta autenticada (com clienteId e token)
        public async Task<List<AfiliadoProduto>> BuscarProdutosPorClienteAsync(Guid clienteId, string query = "tenis", int limit = 20)
        {
            var token = await _authService.GetValidTokenAsync(clienteId);

            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);

            return await BuscarProdutosInternoAsync(query, limit);
        }

        // 🔹 Método interno compartilhado
        private async Task<List<AfiliadoProduto>> BuscarProdutosInternoAsync(string query, int limit)
        {
            var url = $"https://api.mercadolibre.com/sites/MLB/search?q={query}&limit={limit}";
            var response = await _http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao buscar produtos no Mercado Livre. Status: {response.StatusCode}, Detalhes: {msg}");
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("results", out var results))
                throw new Exception("Resposta da API não contém 'results'.");

            var produtos = new List<AfiliadoProduto>();
            foreach (var item in results.EnumerateArray())
            {
                produtos.Add(new AfiliadoProduto
                {
                    Id = item.GetProperty("id").GetString() ?? "",
                    Titulo = item.GetProperty("title").GetString() ?? "",
                    Preco = item.GetProperty("price").GetDecimal(),
                    LinkAfiliado = item.GetProperty("permalink").GetString() ?? "",
                    ImagemUrl = item.TryGetProperty("thumbnail", out var thumb)
                        ? (thumb.GetString() ?? "")
                        : ""
                });
            }

            return produtos;
        }
    }
}
