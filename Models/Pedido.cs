namespace MeuSistema.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public List<ItemPedido> Itens { get; set; } = new();
        public decimal ValorTotal { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pendente";
    }

    public class ItemPedido
    {
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
