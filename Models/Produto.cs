namespace Kanban.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Categoria { get; set; }
        public string? Fornecedor { get; set; }
        public string? Marca { get; set; }
        public string? Tamanho { get; set; }
        public string? Cor { get; set; }
        public string? Material { get; set; }
        public int Estoque { get; set; }
        public decimal Preco { get; set; }
        public string? Descricao { get; set; }
        public string? Condicao { get; set; } // "new" ou "used"
        public string? ImagemUrl { get; set; }
    }
}
