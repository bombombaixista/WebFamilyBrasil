namespace Kanban.Models
{
    public class Movimentacao
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string? Tipo { get; set; } // Entrada ou Saída
        public int Quantidade { get; set; }
        public DateTime Data { get; set; }
    }
}
