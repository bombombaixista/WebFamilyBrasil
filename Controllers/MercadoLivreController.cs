using Kanban.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MercadoLivreController : ControllerBase
    {
        private readonly MercadoLivreTokenService _mercadoLivreTokenService;
        private readonly IHttpClientFactory _httpClientFactory;

        public MercadoLivreController(
            MercadoLivreTokenService mercadoLivreTokenService,
            IHttpClientFactory httpClientFactory)
        {
            _mercadoLivreTokenService = mercadoLivreTokenService;
            _httpClientFactory = httpClientFactory;
        }

        // 🔹 Testa chamada à API do Mercado Livre
        [HttpGet("TestApi")]
        public async Task<IActionResult> TestApi(Guid clienteId)
        {
            try
            {
                var token = await _mercadoLivreTokenService.GetValidTokenAsync(clienteId);

                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync($"https://api.mercadolibre.com/users/me?access_token={token.AccessToken}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return Ok(content);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao chamar API: {ex.Message}");
            }
        }
    }
}
