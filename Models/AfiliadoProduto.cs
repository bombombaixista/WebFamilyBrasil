namespace Kanban.Models
{
    public class AfiliadoProduto
    {
        public string Id { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty; // 🔹 agora pode ser atribuído
        public decimal Preco { get; set; }
        public string LinkAfiliado { get; set; } = string.Empty;
        public string ImagemUrl { get; set; } = string.Empty;

        // Extras para compatibilidade
        public bool Favorito { get; set; } = false;
        public DateTime? UltimaSincronizacao { get; set; }
        public int? CampanhaId { get; set; }
    }
}
