using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.Controllers
{
    [Authorize]
    public class AfiliadosController : Controller
    {
        public IActionResult Links()
        {
            var links = new List<dynamic>
            {
                new { Nome="Mercado Livre", Url="https://www.mercadolivre.com.br", Icone="bi-shop"},
                new { Nome="Ofertas do Dia", Url="https://www.mercadolivre.com.br/ofertas", Icone="bi-lightning"},
                new { Nome="Mais Vendidos", Url="https://www.mercadolivre.com.br/mais-vendidos", Icone="bi-graph-up"},

                new { Nome="Tecnologia", Url="https://lista.mercadolivre.com.br/eletronicos-audio-video", Icone="bi-cpu"},
                new { Nome="Moda", Url="https://lista.mercadolivre.com.br/roupas", Icone="bi-bag"},
                new { Nome="Casa e Móveis", Url="https://lista.mercadolivre.com.br/casa-moveis-decoracao", Icone="bi-house"},
                new { Nome="Esportes", Url="https://lista.mercadolivre.com.br/esportes-fitness", Icone="bi-trophy"},

                // Monitoramento de cliques no Mercado Livre
                new { Nome="Monitoramento de Cliques", Url="https://www.mercadolivre.com.br/afiliados", Icone="bi-bar-chart"}
            };

            return View(links);
        }
    }
}