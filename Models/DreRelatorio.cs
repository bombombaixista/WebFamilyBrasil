namespace Kanban.Models
{
    public class DreRelatorio
    {
        public decimal ReceitaBruta { get; set; }
        public decimal Deducoes { get; set; }
        public decimal ReceitaLiquida { get; set; }
        public decimal CMV { get; set; }
        public decimal LucroBruto { get; set; }
        public decimal DespesasVendas { get; set; }
        public decimal DespesasAdm { get; set; }
        public decimal DespesasFinanceiras { get; set; }
        public decimal ReceitasFinanceiras { get; set; }
        public decimal ResultadoOperacional { get; set; }
        public decimal OutrasReceitas { get; set; }
        public decimal OutrasDespesas { get; set; }
        public decimal LAIR { get; set; }
        public decimal Impostos { get; set; }
        public decimal LucroLiquido { get; set; }
    }
}
