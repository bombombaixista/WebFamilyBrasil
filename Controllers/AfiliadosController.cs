using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;

namespace Kanban.Controllers
{
    public class AfiliadosController : Controller
    {
        private readonly HttpClient _http;

        public AfiliadosController(HttpClient http)
        {
            _http = http;
        }

        // GET: /Afiliados/Produtos
        public async Task<IActionResult> Produtos()
        {
            var listaProdutos = new List<object>();
            var url = "https://api.mercadolibre.com/users/me/items/search"; // substitua pela sua URL real ou LinkTree

            try
            {
                var res = await _http.GetStringAsync(url);
                var json = JsonDocument.Parse(res);

                if (json.RootElement.TryGetProperty("results", out var results))
                {
                    foreach (var item in results.EnumerateArray())
                    {
                        listaProdutos.Add(new
                        {
                            Id = item.GetProperty("id").GetString(),
                            Title = item.GetProperty("title").GetString(),
                            Link = $"https://www.mercadolivre.com.br/{item.GetProperty("id").GetString()}"
                        });
                    }
                }
            }
            catch
            {
                listaProdutos.Add(new { Id = "0", Title = "Nenhum produto encontrado", Link = "#" });
            }

            return View(listaProdutos);
        }

        // GET: /Afiliados/Links
        public IActionResult Links()
        {
            var links = new List<object>
            {
                new { Nome="LinkTree", Url="https://linktr.ee/seu_usuario" },
                new { Nome="Promoções", Url="https://www.mercadolivre.com.br/ofertas" }
            };
            return View(links);
        }

        // GET: /Afiliados/Campanhas
        public IActionResult Campanhas()
        {
            var campanhas = new List<object>
            {
                new { Nome="Campanha 1", Status="Ativa", Vendas=120 },
                new { Nome="Campanha 2", Status="Finalizada", Vendas=250 }
            };
            return View(campanhas);
        }

        // GET: /Afiliados/Relatorios
        public IActionResult Relatorios()
        {
            var relatorios = new List<object>
            {
                new { Nome="Relatório 1", Cliques=150, Conversao=12 },
                new { Nome="Relatório 2", Cliques=300, Conversao=18 }
            };
            return View(relatorios);
        }

        // GET: /Afiliados/Configuracoes
        public IActionResult Configuracoes()
        {
            var config = new { Notificacoes = true, Pagamento = "Pix", Tema = "Claro" };
            return View(config);
        }
    }
}