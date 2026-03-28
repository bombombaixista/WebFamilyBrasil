using Microsoft.AspNetCore.Mvc;
using Kanban.Models;
using Microsoft.AspNetCore.Authorization;

namespace Kanban.Controllers
{
    [Authorize]
    public class CalculadoraController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string categoria, decimal precoVenda, decimal custoProduto, decimal frete,
                                  decimal taxaPercentual, decimal? margemDesejada, decimal? impostoPercentual)
        {
            // 🔥 Margem automática por categoria
            if (!margemDesejada.HasValue || margemDesejada == 0)
            {
                margemDesejada = categoria switch
                {
                    "Tecnologia" => 20,
                    "Moda" => 35,
                    "Casa" => 25,
                    "Esportes" => 22,
                    "Importado" => 45,
                    _ => 20
                };
            }

            var taxaValor = precoVenda * (taxaPercentual / 100);
            var impostoValor = precoVenda * ((impostoPercentual ?? 0) / 100);

            var custoTotal = custoProduto + frete + taxaValor + impostoValor;

            var lucro = precoVenda - custoTotal;

            var margem = precoVenda > 0 ? (lucro / precoVenda) * 100 : 0;

            // 🔥 Inteligência de lucro
            string alerta = "";
            if (lucro < 0)
                alerta = "🔴 Prejuízo!";
            else if (margem < 10)
                alerta = "🟡 Margem muito baixa";
            else
                alerta = "🟢 Lucro saudável";

            // 🔥 Preço sugerido inteligente
            var precoSugerido = custoTotal / (1 - (margemDesejada.Value / 100));

            var model = new CalculadoraResultado
            {
                Categoria = categoria,
                PrecoVenda = precoVenda,
                CustoProduto = custoProduto,
                Frete = frete,
                TaxaPercentual = taxaPercentual,
                TaxaValor = taxaValor,
                ImpostoPercentual = impostoPercentual ?? 0,
                ImpostoValor = impostoValor,
                MargemDesejada = margemDesejada.Value,
                Margem = margem,
                Lucro = lucro,
                PrecoSugerido = precoSugerido,
                Alerta = alerta
            };

            return View(model);
        }
    }
}