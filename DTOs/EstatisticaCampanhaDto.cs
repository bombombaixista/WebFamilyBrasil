namespace Kanban.DTOs
{
    public class EstatisticaCampanhaDto
    {
        public int CampanhaId { get; set; }
        public string? Nome { get; set; }
        public int Produtos { get; set; }
        public int Cliques { get; set; }
        public int Conversoes { get; set; }
        public decimal ReceitaGerada { get; set; }
    }
}
