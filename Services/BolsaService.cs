using System.Text.Json;
using Kanban.Models;

namespace Kanban.Services
{
    public class BolsaService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        private static DateTime _ultimaAtualizacao = DateTime.MinValue;
        private static List<AtivoFinanceiro> _cache = new();

        public BolsaService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _apiKey = config["ApiKeys:HgBrasil"] ?? "";
        }

        public async Task<List<AtivoFinanceiro>> ObterAtivosAsync()
        {
            try
            {
                if ((DateTime.Now - _ultimaAtualizacao).TotalMinutes < 5 && _cache.Any())
                    return _cache;

                var url = $"https://api.hgbrasil.com/finance?key={_apiKey}";
                var response = await _http.GetStringAsync(url);
                var json = JsonDocument.Parse(response);

                var results = json.RootElement.GetProperty("results");

                var lista = new List<AtivoFinanceiro>();

                // 🔹 Ações
                if (results.TryGetProperty("stocks", out var stocks))
                {
                    if (stocks.TryGetProperty("IBOVESPA", out var ibov))
                    {
                        lista.Add(new AtivoFinanceiro
                        {
                            Nome = "IBOV",
                            Simbolo = "IBOV",
                            Preco = ibov.GetProperty("points").GetDecimal(),
                            Variacao = ibov.GetProperty("variation").GetDecimal(),
                            AtualizadoEm = DateTime.Now
                        });
                    }

                    if (stocks.TryGetProperty("IFIX", out var ifix))
                    {
                        lista.Add(new AtivoFinanceiro
                        {
                            Nome = "IFIX",
                            Simbolo = "IFIX",
                            Preco = ifix.GetProperty("points").GetDecimal(),
                            Variacao = ifix.GetProperty("variation").GetDecimal(),
                            AtualizadoEm = DateTime.Now
                        });
                    }
                }

                // 🔹 Moedas
                if (results.TryGetProperty("currencies", out var currencies))
                {
                    lista.Add(new AtivoFinanceiro
                    {
                        Nome = "USD/BRL",
                        Simbolo = "USD",
                        Preco = currencies.GetProperty("USD").GetProperty("buy").GetDecimal(),
                        Variacao = currencies.GetProperty("USD").GetProperty("variation").GetDecimal(),
                        AtualizadoEm = DateTime.Now
                    });

                    lista.Add(new AtivoFinanceiro
                    {
                        Nome = "EUR/BRL",
                        Simbolo = "EUR",
                        Preco = currencies.GetProperty("EUR").GetProperty("buy").GetDecimal(),
                        Variacao = currencies.GetProperty("EUR").GetProperty("variation").GetDecimal(),
                        AtualizadoEm = DateTime.Now
                    });

                    lista.Add(new AtivoFinanceiro
                    {
                        Nome = "BTC/BRL",
                        Simbolo = "BTC",
                        Preco = currencies.GetProperty("BTC").GetProperty("buy").GetDecimal(),
                        Variacao = currencies.GetProperty("BTC").GetProperty("variation").GetDecimal(),
                        AtualizadoEm = DateTime.Now
                    });
                }

                _cache = lista;
                _ultimaAtualizacao = DateTime.Now;

                return lista;
            }
            catch
            {
                return new List<AtivoFinanceiro>();
            }
        }
    }
}