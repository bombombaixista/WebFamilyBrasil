namespace Kanban.Models
{
    public class LogClique
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public int? CampanhaId { get; set; }
        public DateTime Data { get; set; }
    }
}
