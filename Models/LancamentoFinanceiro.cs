namespace MeuSistema.Models
{
    public class LancamentoFinanceiro
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = "Receita"; // Receita ou Despesa
        public string Cliente { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Aberto"; // Aberto, Pago, Cancelado
        public int PedidoId { get; set; } // vínculo com o pedido
    }
}
