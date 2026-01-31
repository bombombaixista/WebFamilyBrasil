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

        // Configure no appsettings.json
        private readonly string _clientId = "SEU_CLIENT_ID";
        private readonly string _clientSecret = "SEU_CLIENT_SECRET";
        private readonly string _redirectUri = "https://seusite.com/api/MercadoLivreAuth/callback";

        public MercadoLivreAuthController(IHttpClientFactory httpClientFactory, MercadoLivreTokenService tokenService)
        {
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Passo 1: Redireciona o usuário para autorizar no Mercado Livre
        /// </summary>
        [HttpGet("login")]
        public IActionResult Login()
        {
            var url = $"https://auth.mercadolibre.com.br/authorization?response_type=code&client_id={_clientId}&redirect_uri={_redirectUri}";
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
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "code", code },
                { "redirect_uri", _redirectUri }
            });
            request.Content = content;

            var response = await client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return BadRequest($"Erro ao obter token: {json}");

            var doc = JsonDocument.Parse(json);
            var accessToken = doc.RootElement.GetProperty("access_token").GetString();
            var refreshToken = doc.RootElement.GetProperty("refresh_token").GetString();
            var expiresIn = doc.RootElement.GetProperty("expires_in").GetInt32();

            var token = new MercadoLivreToken
            {
                AccessToken = accessToken ?? "",
                RefreshToken = refreshToken ?? "",
                ExpirationDate = DateTime.UtcNow.AddSeconds(expiresIn)
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
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "refresh_token", token?.RefreshToken ?? "" }
            });
            request.Content = content;

            var response = await client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return BadRequest($"Erro ao renovar token: {json}");

            var doc = JsonDocument.Parse(json);
            var accessToken = doc.RootElement.GetProperty("access_token").GetString();
            var refreshToken = doc.RootElement.GetProperty("refresh_token").GetString();
            var expiresIn = doc.RootElement.GetProperty("expires_in").GetInt32();

            var newToken = new MercadoLivreToken
            {
                AccessToken = accessToken ?? "",
                RefreshToken = refreshToken ?? "",
                ExpirationDate = DateTime.UtcNow.AddSeconds(expiresIn)
            };

            await _tokenService.SaveTokenAsync(newToken);

            return Ok(new { message = "Token renovado com sucesso!", newToken });
        }
    }
}
