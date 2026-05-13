namespace WebFamilyBrasil.Models
{
    public class RelatorioSafra
    {
        public string Talhao { get; set; }
        public string Cultura { get; set; }
        public decimal CustoEstimado { get; set; }
        public decimal CustoInsumos { get; set; }
        public decimal CustoTotal => CustoEstimado + CustoInsumos;

        public decimal? ProducaoEsperada { get; set; }
        public decimal PrecoMercado { get; set; }
        public decimal? LucroEstimado => ProducaoEsperada.HasValue
            ? (ProducaoEsperada.Value * PrecoMercado) - CustoTotal
            : null;
    }
}
