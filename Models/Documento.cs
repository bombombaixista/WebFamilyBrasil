namespace Kanban.Models
{
    public class Documento
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Caminho { get; set; } = string.Empty;
        public DateTime DataUpload { get; set; }
    }
}
