using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using Kanban.Services;

namespace Kanban.Controllers
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

        // 🔹 PASSO 1 — Redireciona para autorização
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

        // 🔹 PASSO 2 — Callback
        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Callback chamado sem code");

            // 🔥 Pegando cliente logado (Claims)
            var clienteIdClaim = User.FindFirst("ClienteId")?.Value;
            if (clienteIdClaim == null)
                return Unauthorized("Cliente não autenticado");

            var clienteId = Guid.Parse(clienteIdClaim);

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

            var accessToken = doc.RootElement.GetProperty("access_token").GetString()!;
            var refreshToken = doc.RootElement.GetProperty("refresh_token").GetString()!;
            var expiration = DateTime.UtcNow.AddSeconds(
                doc.RootElement.GetProperty("expires_in").GetInt32()
            );
            var userId = doc.RootElement.GetProperty("user_id").GetInt64();

            await _tokenService.SalvarTokenAsync(
                clienteId,
                accessToken,
                refreshToken,
                expiration,
                userId
            );

            return Ok(new
            {
                message = "Token salvo com sucesso",
                expiresAt = expiration
            });
        }

        // 🔹 PASSO 3 — Refresh automático
        [HttpGet("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var clienteIdClaim = User.FindFirst("ClienteId")?.Value;
            if (clienteIdClaim == null)
                return Unauthorized("Cliente não autenticado");

            var clienteId = Guid.Parse(clienteIdClaim);

            var cliente = await _tokenService.GetClienteComTokenAsync(clienteId);

            if (cliente == null || cliente.MercadoLivreRefreshToken == null)
                return BadRequest("Refresh token não encontrado");

            // 🔥 Se ainda é válido
            if (cliente.MercadoLivreTokenExpiraEm.HasValue &&
                cliente.MercadoLivreTokenExpiraEm > DateTime.UtcNow)
            {
                return Ok("Token ainda válido");
            }

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
                { "refresh_token", cliente.MercadoLivreRefreshToken }
            });

            var response = await client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return BadRequest($"Erro ao renovar token: {json}");

            var doc = JsonDocument.Parse(json);

            var accessToken = doc.RootElement.GetProperty("access_token").GetString()!;
            var refreshToken = doc.RootElement.GetProperty("refresh_token").GetString()!;
            var expiration = DateTime.UtcNow.AddSeconds(
                doc.RootElement.GetProperty("expires_in").GetInt32()
            );
            var userId = doc.RootElement.GetProperty("user_id").GetInt64();

            await _tokenService.SalvarTokenAsync(
                clienteId,
                accessToken,
                refreshToken,
                expiration,
                userId
            );

            return Ok("Token renovado com sucesso");
        }
    }
}
