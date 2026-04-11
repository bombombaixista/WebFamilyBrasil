namespace Kanban.Models
{
    public class ParcelamentoResultado
    {
        public string? Categoria { get; set; }

        public decimal PrecoVenda { get; set; }
        public int NumeroParcelas { get; set; }
        public decimal TaxaCartaoPercentual { get; set; }

        // Valores calculados
        public decimal TaxaCartaoValor { get; set; }
        public decimal ValorLiquidoLoja { get; set; }
        public decimal ValorParcela { get; set; }
        public decimal ValorTotalParcelado { get; set; }

        public string? Alerta { get; set; }
    }
}
