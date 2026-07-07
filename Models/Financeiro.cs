namespace Kanban.Models
{
    public enum TipoLancamento
    {
        Receita,
        Despesa
    }

    public enum CategoriaLancamento
    {
        Vendas,
        Deducoes,
        CMV,
        DespesasVendas,
        DespesasAdm,
        DespesasFinanceiras,
        ReceitasFinanceiras,
        OutrasReceitas,
        OutrasDespesas
    }

    public class Financeiro
    {
        public int Id { get; set; }
        public string? Descricao { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public TipoLancamento Tipo { get; set; }
        public CategoriaLancamento Categoria { get; set; }
    }
}
