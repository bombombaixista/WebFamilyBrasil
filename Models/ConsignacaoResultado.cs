namespace Kanban.Models
{
    public class ConsignacaoResultado
    {
        public string? Categoria { get; set; }

        public decimal PrecoVenda { get; set; }
        public decimal CustoProduto { get; set; }
        public decimal Frete { get; set; }

        public decimal TaxaPercentual { get; set; }
        public decimal TaxaValor { get; set; }

        public decimal ImpostoPercentual { get; set; }
        public decimal ImpostoValor { get; set; }

        // 🔥 Margens específicas
        public decimal MargemLoja { get; set; }
        public decimal ComissaoLoja { get; set; }
        public decimal LucroLoja { get; set; }

        public decimal MargemVendedor { get; set; }
        public decimal ValorVendedor { get; set; }

        // 🔥 Preço sugerido inteligente (garantindo margem mínima para ambos)
        public decimal PrecoSugerido { get; set; }

        public string? Alerta { get; set; }

        // 🔥 Campos extras opcionais
        public decimal CustoDolar { get; set; }
        public decimal CotacaoDolar { get; set; }
        public decimal CustoConvertido { get; set; }

        public decimal TaxaExtraPercentual { get; set; }
        public decimal TaxaExtraValor { get; set; }

        public string? Status { get; set; }
    }
}
