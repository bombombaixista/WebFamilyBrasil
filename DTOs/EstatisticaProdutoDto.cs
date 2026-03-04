namespace Kanban.DTOs
{
    public class EstatisticaProdutoDto
    {
        public int ProdutoId { get; set; }
        public string? Nome { get; set; }
        public decimal Preco { get; set; }
        public int Cliques { get; set; }
        public int Conversoes { get; set; }
        public decimal ReceitaGerada { get; set; }
    }
}
