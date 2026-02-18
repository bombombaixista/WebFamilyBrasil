using Kanban.Models;
using Kanban.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Kanban.Controllers
{
    public class MercadoLivreController : Controller
    {
        private readonly MercadoLivreTokenService _mercadoLivreTokenService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;

        public MercadoLivreController(
            MercadoLivreTokenService mercadoLivreTokenService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _mercadoLivreTokenService = mercadoLivreTokenService;
            _httpClientFactory = httpClientFactory;

            _clientId = configuration["MercadoLivre:ClientId"]!;
            _clientSecret = configuration["MercadoLivre:ClientSecret"]!;
            _redirectUri = configuration["MercadoLivre:RedirectUri"]!;
        }

        // 🔹 PASSO 1 - Redireciona para autorização
        public IActionResult Connect(Guid clienteId)
        {
            // Guardar clienteId em TempData para usar no Callback
            TempData["ClienteId"] = clienteId;

            var url =
                "https://auth.mercadolivre.com.br/authorization" +
                "?response_type=code" +
                $"&client_id={_clientId}" +
                $"&redirect_uri={Uri.EscapeDataString(_redirectUri)}";

            return Redirect(url);
        }

        // 🔹 PASSO 2 - Callback
        public async Task<IActionResult> Callback(string code)
        {
            if (string.IsNullOrEmpty(code))
                return Content("Código não recebido.");

            var httpClient = _httpClientFactory.CreateClient();

            var requestBody = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "code", code },
                { "redirect_uri", _redirectUri }
            };

            var response = await httpClient.PostAsync(
                "https://api.mercadolibre.com/oauth/token",
                new FormUrlEncodedContent(requestBody)
            );

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return Content($"Erro ao obter token:\n{content}", "text/plain");

            var json = JsonDocument.Parse(content).RootElement;

            var token = new MercadoLivreToken
            {
                AccessToken = json.GetProperty("access_token").GetString()!,
                RefreshToken = json.GetProperty("refresh_token").GetString()!,
                TokenType = json.GetProperty("token_type").GetString()!,
                ExpiresIn = json.GetProperty("expires_in").GetInt32(),
                ExpirationDate = DateTime.UtcNow.AddSeconds(json.GetProperty("expires_in").GetInt32()),
                UserId = json.GetProperty("user_id").GetInt64()
            };

            // Recupera clienteId salvo no Connect
            var clienteId = Guid.Parse(TempData["ClienteId"]!.ToString()!);

            await _mercadoLivreTokenService.SaveInitialTokenAsync(clienteId, token);

            return Content("Conexão com Mercado Livre realizada com sucesso!");
        }

        // 🔹 PASSO 3 - Exemplo de uso do token válido
        public async Task<IActionResult> TestApi(Guid clienteId)
        {
            var token = await _mercadoLivreTokenService.GetValidTokenAsync(clienteId);

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);

            var response = await httpClient.GetAsync("https://api.mercadolibre.com/users/me");
            var content = await response.Content.ReadAsStringAsync();

            return Content(content, "application/json");
        }
    }
}
