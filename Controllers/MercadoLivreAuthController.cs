using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using WebFamily.Models;
using WebFamily.Services;

namespace WebFamily.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MercadoLivreAuthController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly MercadoLivreTokenService _tokenService;

        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;

        public MercadoLivreAuthController(IHttpClientFactory httpClientFactory, MercadoLivreTokenService tokenService, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;

            // Lendo do appsettings.json ou variáveis de ambiente
            _clientId = config["ML_CLIENT_ID"] ?? "";
            _clientSecret = config["ML_CLIENT_SECRET"] ?? "";
            _redirectUri = config["ML_REDIRECT_URI"] ?? "https://webfamilybrasil-production.up.railway.app/api/MercadoLivreAuth/callback";
        }

        /// <summary>
        /// Passo 1: Redireciona o usuário para autorizar no Mercado Livre
        /// </summary>
        [HttpGet("login")]
        public IActionResult Login()
        {
            var url = $"https://auth.mercadolibre.com.ar/authorization?response_type=code&client_id={_clientId}&redirect_uri={_redirectUri}";
            return Redirect(url);
        }

        /// <summary>
        /// Passo 2: Callback do Mercado Livre com o "code"
        /// </summary>
        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            var client = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.mercadolibre.com/oauth/token");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "code", code },
                { "redirect_uri", _redirectUri }
            });

            var response = await client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return BadRequest($"Erro ao obter token: {json}");

            var doc = JsonDocument.Parse(json);
            var token = new MercadoLivreToken
            {
                AccessToken = doc.RootElement.GetProperty("access_token").GetString() ?? "",
                RefreshToken = doc.RootElement.GetProperty("refresh_token").GetString() ?? "",
                ExpirationDate = DateTime.UtcNow.AddSeconds(doc.RootElement.GetProperty("expires_in").GetInt32())
            };

            await _tokenService.SaveTokenAsync(token);

            return Ok(new { message = "Token salvo com sucesso!", token });
        }

        /// <summary>
        /// Passo 3: Refresh do token quando expirar
        /// </summary>
        [HttpGet("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var token = await _tokenService.GetValidTokenAsync();
            if (token != null && token.IsValid)
                return Ok(new { message = "Token ainda válido", token });

            var client = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.mercadolibre.com/oauth/token");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "refresh_token", token?.RefreshToken ?? "" }
            });

            var response = await client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return BadRequest($"Erro ao renovar token: {json}");

            var doc = JsonDocument.Parse(json);
            var newToken = new MercadoLivreToken
            {
                AccessToken = doc.RootElement.GetProperty("access_token").GetString() ?? "",
                RefreshToken = doc.RootElement.GetProperty("refresh_token").GetString() ?? "",
                ExpirationDate = DateTime.UtcNow.AddSeconds(doc.RootElement.GetProperty("expires_in").GetInt32())
            };

            await _tokenService.SaveTokenAsync(newToken);

            return Ok(new { message = "Token renovado com sucesso!", newToken });
        }
    }
}
