namespace Kanban.Models
{
    public class Alerta
    {
        public int Id { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public bool Resolvido { get; set; }
    }
}
