using Kanban.Models;

namespace Kanban.Services
{
    public class CalculadoraService
    {
        public (decimal margem, decimal imposto) ObterParametrosPorCategoria(string categoria)
        {
            return categoria switch
            {
                "Tecnologia" => (20, 0),
                "Moda" => (50, 0),
                "Casa" => (40, 0),
                "Esportes" => (35, 0),
                "Importado" => (80, 60),
                "Acessorios" => (100, 0),
                _ => (30, 0)
            };
        }

        public CalculadoraResultado Calcular(
            decimal precoVenda,
            decimal custoProduto,
            decimal frete,
            decimal taxaPercentual,
            decimal margemDesejada,
            decimal custoDolar,
            decimal cotacaoDolar,
            decimal impostoPercentual,
            decimal taxaExtraPercentual,
            string categoria)
        {
            // 🔥 Parâmetros automáticos
            var parametros = ObterParametrosPorCategoria(categoria);

            if (margemDesejada == 0)
                margemDesejada = parametros.margem;

            if (impostoPercentual == 0)
                impostoPercentual = parametros.imposto;

            // 🔥 Conversão dólar
            var custoConvertido = custoDolar * cotacaoDolar;

            var custoBase = custoConvertido > 0 ? custoConvertido : custoProduto;

            // 🔥 Imposto
            var imposto = custoBase * (impostoPercentual / 100);

            // 🔥 Taxas
            var taxaML = precoVenda * (taxaPercentual / 100);
            var taxaExtra = precoVenda * (taxaExtraPercentual / 100);

            // 🔥 Lucro
            var lucro = precoVenda - custoBase - frete - imposto - taxaML - taxaExtra;

            var margem = precoVenda > 0 ? (lucro / precoVenda) * 100 : 0;

            // 🔥 Preço sugerido
            decimal precoSugerido = (custoBase + frete + imposto)
                / (1 - (taxaPercentual / 100) - (taxaExtraPercentual / 100) - (margemDesejada / 100));

            // 🔥 Status
            string status;
            if (lucro < 0)
                status = "Prejuízo ❌";
            else if (margem < 10)
                status = "Margem Muito Baixa ⚠️";
            else if (margem < 20)
                status = "Margem Média ⚡";
            else
                status = "Lucro Saudável ✅";

            return new CalculadoraResultado
            {
                PrecoVenda = precoVenda,
                CustoProduto = custoProduto,
                Frete = frete,
                TaxaPercentual = taxaPercentual,

                CustoDolar = custoDolar,
                CotacaoDolar = cotacaoDolar,
                ImpostoPercentual = impostoPercentual,
                TaxaExtraPercentual = taxaExtraPercentual,

                CustoConvertido = custoConvertido,
                ImpostoValor = imposto,
                TaxaValor = taxaML,
                TaxaExtraValor = taxaExtra,

                Lucro = lucro,
                Margem = margem,
                Status = status,
                PrecoSugerido = precoSugerido,
                MargemDesejada = margemDesejada,
                Categoria = categoria
            };
        }
    }
}