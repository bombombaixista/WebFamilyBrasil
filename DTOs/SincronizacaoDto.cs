namespace Kanban.DTOs
{
    public class SincronizacaoDto
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public int QuantidadeProdutos { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
