using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Kanban.Models;

namespace Kanban.Controllers
{
    [Authorize]
    public class ConsignacaoController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string categoria, decimal precoVenda, decimal custoProduto, decimal frete,
                                  decimal taxaPercentual, decimal margemLoja, decimal margemVendedor,
                                  decimal? impostoPercentual)
        {
            // 🔥 Margem automática por categoria (se quiser manter lógica por tipo de produto)
            if (margemLoja == 0)
            {
                margemLoja = categoria switch
                {
                    "Guitarra" => 15,
                    "Baixo" => 15,
                    "Pedal" => 20,
                    "Acessório" => 25,
                    _ => 15
                };
            }

            var taxaValor = precoVenda * (taxaPercentual / 100);
            var impostoValor = precoVenda * ((impostoPercentual ?? 0) / 100);

            var custoTotal = custoProduto + frete + taxaValor + impostoValor;

            // 🔥 Comissão da loja
            var comissaoLoja = precoVenda * (margemLoja / 100);

            // 🔥 Valor líquido para o vendedor
            var valorVendedor = precoVenda - comissaoLoja - impostoValor - taxaValor - frete;

            // 🔥 Lucro da loja
            var lucroLoja = comissaoLoja - (custoProduto > 0 ? custoProduto : 0);

            // 🔥 Margem do vendedor em relação ao preço de venda
            var margemRealVendedor = precoVenda > 0 ? (valorVendedor / precoVenda) * 100 : 0;

            string alerta = "";
            if (valorVendedor < 0)
                alerta = "🔴 Vendedor sai no prejuízo!";
            else if (margemRealVendedor < 10)
                alerta = "🟡 Margem muito baixa para o vendedor";
            else
                alerta = "🟢 Consignação saudável";

            var model = new ConsignacaoResultado
            {
                Categoria = categoria,
                PrecoVenda = precoVenda,
                CustoProduto = custoProduto,
                Frete = frete,
                TaxaPercentual = taxaPercentual,
                TaxaValor = taxaValor,
                ImpostoPercentual = impostoPercentual ?? 0,
                ImpostoValor = impostoValor,
                MargemLoja = margemLoja,
                ComissaoLoja = comissaoLoja,
                LucroLoja = lucroLoja,
                MargemVendedor = margemRealVendedor,
                ValorVendedor = valorVendedor,
                Alerta = alerta
            };

            return View(model);
        }
    }
}
