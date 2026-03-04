namespace Kanban.DTOs
{
    public class AlertaDto
    {
        public int Id { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public bool Resolvido { get; set; }
    }
}
