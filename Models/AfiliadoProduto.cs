using System;

namespace Kanban.Models
{
    public class AfiliadoProduto
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public decimal Preco { get; set; }
        public string? ImagemUrl { get; set; }
        public string? LinkAfiliado { get; set; }
    }
}
