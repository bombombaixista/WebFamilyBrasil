namespace Kanban.Models
{
    public class AtivoFinanceiro
    {
        public string Nome { get; set; } = "";
        public string Simbolo { get; set; } = "";
        public decimal Preco { get; set; }
        public decimal Variacao { get; set; }
        public DateTime AtualizadoEm { get; set; }
    }
}