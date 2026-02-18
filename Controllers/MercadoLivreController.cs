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

        public MercadoLivreController(
            MercadoLivreTokenService mercadoLivreTokenService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _mercadoLivreTokenService = mercadoLivreTokenService;
            _httpClientFactory = httpClientFactory;
            _clientId = configuration["MercadoLivre:ClientId"]!;
            _clientSecret = configuration["MercadoLivre:ClientSecret"]!;
        }

        // 🔹 Passo 1: Conectar
        public IActionResult Connect(Guid clienteId)
        {
            var redirectUri = Url.Action("Callback", "MercadoLivre", new { clienteId }, Request.Scheme);
            var url = $"https://auth.mercadolibre.com/authorization?response_type=code&client_id={_clientId}&redirect_uri={redirectUri}";
            return Redirect(url);
        }

        // 🔹 Passo 2: Callback
        public async Task<IActionResult> Callback(string code, Guid clienteId)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();

#pragma warning disable CS8604 // Possível argumento de referência nula.
                var requestBody = new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                    { "client_id", _clientId },
                    { "client_secret", _clientSecret },
                    { "code", code },
                    { "redirect_uri", Url.Action("Callback", "MercadoLivre", new { clienteId }, Request.Scheme) }
                };
#pragma warning restore CS8604 // Possível argumento de referência nula.

                var response = await httpClient.PostAsync("https://api.mercadolibre.com/oauth/token", new FormUrlEncodedContent(requestBody));
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return Content($"Erro ao obter token:\n{content}", "text/plain");

                var json = JsonDocument.Parse(content).RootElement;

                var token = new MercadoLivreToken
                {
                    AccessToken = json.GetProperty("access_token").GetString()!,
                    RefreshToken = json.GetProperty("refresh_token").GetString()!,
                    ExpiresIn = json.GetProperty("expires_in").GetInt32(),
                    ExpirationDate = DateTime.UtcNow.AddSeconds(json.GetProperty("expires_in").GetInt32()),
                    UserId = json.GetProperty("user_id").GetInt64()
                };

                await _mercadoLivreTokenService.SaveInitialTokenAsync(clienteId, token);

                return Content("Conexão com Mercado Livre realizada com sucesso!");
            }
            catch (Exception ex)
            {
                return Content($"Erro interno no Callback: {ex.Message}\n{ex.StackTrace}", "text/plain");
            }
        }

        // 🔹 Passo 3: Testar API
        public async Task<IActionResult> TestApi(Guid clienteId)
        {
            try
            {
                var token = await _mercadoLivreTokenService.GetValidTokenAsync(clienteId);
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);

                var response = await httpClient.GetAsync("https://api.mercadolibre.com/users/me");
                var content = await response.Content.ReadAsStringAsync();

                return Content(content, "application/json");
            }
            catch (Exception ex)
            {
                return Content($"Erro ao chamar API: {ex.Message}\n{ex.StackTrace}", "text/plain");
            }
        }
    }
}
