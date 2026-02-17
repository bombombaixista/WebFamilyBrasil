using Microsoft.AspNetCore.Mvc;

namespace Kanban.Controllers
{
    public class MercadoLivreController : Controller
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;
        private readonly IHttpClientFactory _httpClientFactory;

        public MercadoLivreController(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

            _clientId = configuration["MercadoLivre:ClientId"]
                ?? throw new Exception("MercadoLivre:ClientId não configurado");

            _clientSecret = configuration["MercadoLivre:ClientSecret"]
                ?? throw new Exception("MercadoLivre:ClientSecret não configurado");

            _redirectUri = configuration["MercadoLivre:RedirectUri"]
                ?? throw new Exception("MercadoLivre:RedirectUri não configurado");
        }

        // 🔹 PASSO 1 - Redireciona para autorização
        public IActionResult Connect()
        {
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

            return Content(content, "application/json");
        }
    }
}
