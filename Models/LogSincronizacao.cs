namespace Kanban.Models
{
    public class LogSincronizacao
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public int QuantidadeProdutos { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
