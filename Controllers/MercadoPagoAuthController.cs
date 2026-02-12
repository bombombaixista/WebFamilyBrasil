using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebFamily.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MercadoPagoAuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public MercadoPagoAuthController(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClient = httpClientFactory.CreateClient();
        }

        // Inicia o login no Mercado Pago
        [HttpGet("login")]
        public IActionResult Login()
        {
            var clientId = _config["MercadoPago:ClientId"];
            var redirectUri = _config["MercadoPago:RedirectUri"];

            var url = $"https://auth.mercadopago.com/authorization?client_id={clientId}&response_type=code&redirect_uri={redirectUri}";
            return Redirect(url);
        }

        // Callback chamado pelo Mercado Pago após autorização
        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            var clientId = _config["MercadoPago:ClientId"];
            var clientSecret = _config["MercadoPago:ClientSecret"];
            var redirectUri = _config["MercadoPago:RedirectUri"];

            var tokenUrl = "https://api.mercadopago.com/oauth/token";
            var data = new Dictionary<string, string>
            {
                {"client_id", clientId},
                {"client_secret", clientSecret},
                {"grant_type", "authorization_code"},
                {"code", code},
                {"redirect_uri", redirectUri}
            };

            var response = await _httpClient.PostAsync(tokenUrl, new FormUrlEncodedContent(data));
            var content = await response.Content.ReadAsStringAsync();

            // Aqui você pode desserializar o JSON e salvar tokens em banco ou cache
            return Ok(content);
        }
    }
}
