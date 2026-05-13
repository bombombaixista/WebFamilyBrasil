using Microsoft.AspNetCore.Mvc;
using WebFamilyBrasil.Models;

namespace WebFamilyBrasil.Controllers
{
    public class RelatorioSafraController : Controller
    {
        private static List<Safra> _safras = new();
        private static List<Insumo> _insumos = new();

        public IActionResult Index()
        {
            var relatorios = _safras.Select(s => new RelatorioSafra
            {
                Talhao = s.Talhao,
                Cultura = s.Cultura,
                CustoEstimado = s.CustoEstimado,
                CustoInsumos = _insumos.Sum(i => i.CustoUnitario * i.Quantidade),
                ProducaoEsperada = s.ProducaoEsperada,
                PrecoMercado = ObterPrecoMercado(s.Cultura) // função simulada
            }).ToList();

            return View(relatorios);
        }

        private decimal ObterPrecoMercado(string cultura)
        {
            // Simulação de preços de mercado
            return cultura switch
            {
                "Milho" => 50m,
                "Soja" => 120m,
                "Tomate" => 3m,
                _ => 10m
            };
        }
    }
}
