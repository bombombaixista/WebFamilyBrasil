namespace Kanban.Models
{
    public class Financeiro
    {
        public int Id { get; set; }
        public string? Descricao { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public string? Tipo { get; set; } // Receita ou Despesa
        public string? Categoria { get; set; } // Venda, Taxa, Frete, etc.
    }
}
