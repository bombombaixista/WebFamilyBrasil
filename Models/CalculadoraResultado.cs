namespace Kanban.Models
{
    public class CalculadoraResultado
    {
        public string? Categoria { get; set; }

        public decimal PrecoVenda { get; set; }
        public decimal CustoProduto { get; set; }
        public decimal Frete { get; set; }

        public decimal TaxaPercentual { get; set; }
        public decimal TaxaValor { get; set; }

        public decimal ImpostoPercentual { get; set; }
        public decimal ImpostoValor { get; set; }

        public decimal MargemDesejada { get; set; }
        public decimal Margem { get; set; }

        public decimal Lucro { get; set; }
        public decimal PrecoSugerido { get; set; }

        public string? Alerta { get; set; }

        // 🔥 NOVOS CAMPOS (ERRO DO BUILD)
        public decimal CustoDolar { get; set; }
        public decimal CotacaoDolar { get; set; }
        public decimal CustoConvertido { get; set; }

        public decimal TaxaExtraPercentual { get; set; }
        public decimal TaxaExtraValor { get; set; }

        public string? Status { get; set; }
    }
}