using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class MercadoLivreProdutosController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public MercadoLivreProdutosController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Buscar(string query = "promoções")
    {
        // Pegue o último access token válido do banco
        string accessToken = "APP_USR-6591774819819967-022200-ec819529d0f112f4f25801f57b6d8bf7-3134918415"; // substitua pelo token atual

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // Endpoint de busca de produtos
        var url = $"https://api.mercadolibre.com/sites/MLB/search?q={query}";
        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return Content($"Erro: {response.StatusCode}");
        }

        var json = await response.Content.ReadAsStringAsync();
        var produtos = JObject.Parse(json);

        return View(produtos);
    }
}
