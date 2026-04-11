using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Kanban.Models;

namespace Kanban.Controllers
{
    [Authorize]
    public class ParcelamentoController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string categoria, decimal precoVenda, int numeroParcelas, decimal taxaCartaoPercentual)
        {
            // 🔥 Cálculo da taxa do cartão
            var taxaCartaoValor = precoVenda * (taxaCartaoPercentual / 100);

            // 🔥 Valor líquido para a loja (já descontando taxa do cartão)
            var valorLiquidoLoja = precoVenda - taxaCartaoValor;

            // 🔥 Valor total parcelado (considerando juros embutidos da taxa do cartão)
            var valorTotalParcelado = precoVenda + taxaCartaoValor;

            // 🔥 Valor de cada parcela
            var valorParcela = numeroParcelas > 0 ? valorTotalParcelado / numeroParcelas : valorTotalParcelado;

            // 🔥 Alerta inteligente
            string alerta = "";
            if (taxaCartaoPercentual > 10)
                alerta = "🔴 Taxa do cartão muito alta!";
            else if (numeroParcelas > 12)
                alerta = "🟡 Muitas parcelas, atenção ao risco!";
            else
                alerta = "🟢 Parcelamento saudável";

            var model = new ParcelamentoResultado
            {
                Categoria = categoria,
                PrecoVenda = precoVenda,
                NumeroParcelas = numeroParcelas,
                TaxaCartaoPercentual = taxaCartaoPercentual,
                TaxaCartaoValor = taxaCartaoValor,
                ValorLiquidoLoja = valorLiquidoLoja,
                ValorParcela = valorParcela,
                ValorTotalParcelado = valorTotalParcelado,
                Alerta = alerta
            };

            return View(model);
        }
    }
}
