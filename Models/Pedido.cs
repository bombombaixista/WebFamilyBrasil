namespace Kanban.Models
{
    public class Pedido
    {
        public long Id { get; set; }

        public string Cliente { get; set; } = string.Empty;

        public decimal ValorTotal { get; set; }

        public DateTime Data { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Pendente";

        // 🔥 Origem do pedido (Shopee, ML, AliExpress, Manual etc)
        public string Origem { get; set; } = "Manual";

        // 🔥 Endereço
        public string? Endereco { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? CEP { get; set; }

        // 🔥 Envio
        public string? Transportadora { get; set; }
        public string? CodigoRastreamento { get; set; }

        public List<ItemPedido> Itens { get; set; } = new();
    }

    public class ItemPedido
    {
        public long ProdutoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string? SKU { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
