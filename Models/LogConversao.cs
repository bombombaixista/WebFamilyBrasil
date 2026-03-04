namespace Kanban.Models
{
    public class LogConversao
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public int? CampanhaId { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
    }
}
