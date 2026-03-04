using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;

namespace Kanban.Controllers
{
    [Route("")]
    public class LandingController : Controller
    {
        private readonly HttpClient _http;
        private readonly string _apiKey = "J52DWZXMJ53CTHH4"; // Alpha Vantage

        public LandingController(HttpClient http)
        {
            _http = http;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View(); // Razor page ultimate
        }

        [HttpGet("api/dados")]
        public async Task<IActionResult> ObterDados()
        {
            var dados = new Dictionary<string, decimal>();

            // Ações brasileiras
            dados["PETR4"] = await BuscarPrecoAsync("PETR4.SA");
            dados["VALE3"] = await BuscarPrecoAsync("VALE3.SA");
            dados["ITUB4"] = await BuscarPrecoAsync("ITUB4.SA");

            // Câmbio
            dados["USD"] = await BuscarCambioAsync("USD", "BRL");
            dados["EUR"] = await BuscarCambioAsync("EUR", "BRL");

            // IBOV (simulado)
            dados["IBOV"] = 120000;

            // Margem / lucro simulados
            dados["MargemMedia"] = 78;
            dados["LucroEstimado"] = 55000;

            return Json(dados);
        }

        private async Task<decimal> BuscarPrecoAsync(string simbolo)
        {
            try
            {
                var url = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={simbolo}&apikey={_apiKey}";
                var response = await _http.GetStringAsync(url);
                var json = JsonDocument.Parse(response);
                if (json.RootElement.TryGetProperty("Global Quote", out var quote) &&
                    quote.TryGetProperty("05. price", out var price))
                {
                    return decimal.Parse(price.GetString()!);
                }
            }
            catch { }
            return 0;
        }

        private async Task<decimal> BuscarCambioAsync(string from, string to)
        {
            try
            {
                var url = $"https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&from_currency={from}&to_currency={to}&apikey={_apiKey}";
                var response = await _http.GetStringAsync(url);
                var json = JsonDocument.Parse(response);
                if (json.RootElement.TryGetProperty("Realtime Currency Exchange Rate", out var rate) &&
                    rate.TryGetProperty("5. Exchange Rate", out var exchange))
                {
                    return decimal.Parse(exchange.GetString()!);
                }
            }
            catch { }
            return 0;
        }
    }
}