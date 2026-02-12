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

        public MercadoLivreAuthController(
            IHttpClientFactory httpClientFactory,
            MercadoLivreTokenService tokenService,
            IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;

            _clientId = config["MercadoLivre:ClientId"]!;
            _clientSecret = config["MercadoLivre:ClientSecret"]!;
            _redirectUri = config["MercadoLivre:RedirectUri"]!;
        }

        /// <summary>
        /// Passo 1: Redireciona o vendedor para autorizar no Mercado Livre (Brasil)
        /// </summary>
        [HttpGet("login")]
        public IActionResult Login()
        {
            var url =
                "https://auth.mercadolivre.com.br/authorization" +
                "?response_type=code" +
                $"&client_id={_clientId}" +
                $"&redirect_uri={Uri.EscapeDataString(_redirectUri)}";

            return Redirect(url);
        }

        /// <summary>
        /// Passo 2: Callback com o authorization code
        /// </summary>
        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Callback chamado sem code");

            var client = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.mercadolibre.com/oauth/token"
            );

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
                AccessToken = doc.RootElement.GetProperty("access_token").GetString()!,
                RefreshToken = doc.RootElement.GetProperty("refresh_token").GetString()!,
                ExpirationDate = DateTime.UtcNow.AddSeconds(
                    doc.RootElement.GetProperty("expires_in").GetInt32()
                )
            };

            await _tokenService.SaveTokenAsync(token);

            return Ok(new
            {
                message = "Token obtido e salvo com sucesso",
                expiresAt = token.ExpirationDate
            });
        }

        /// <summary>
        /// Passo 3: Refresh automático do token
        /// </summary>
        [HttpGet("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var token = await _tokenService.GetValidTokenAsync();

            if (token != null && token.IsValid)
                return Ok(new { message = "Token ainda válido" });

            if (token?.RefreshToken == null)
                return BadRequest("Não há refresh token salvo");

            var client = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.mercadolibre.com/oauth/token"
            );

            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "refresh_token", token.RefreshToken }
            });

            var response = await client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return BadRequest($"Erro ao renovar token: {json}");

            var doc = JsonDocument.Parse(json);

            var newToken = new MercadoLivreToken
            {
                AccessToken = doc.RootElement.GetProperty("access_token").GetString()!,
                RefreshToken = doc.RootElement.GetProperty("refresh_token").GetString()!,
                ExpirationDate = DateTime.UtcNow.AddSeconds(
                    doc.RootElement.GetProperty("expires_in").GetInt32()
                )
            };

            await _tokenService.SaveTokenAsync(newToken);

            return Ok(new { message = "Token renovado com sucesso" });
        }
    }
}
