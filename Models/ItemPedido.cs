namespace Kanban.Models;

public class ItemPedido
{
    public long Id { get; set; }

    public long PedidoId { get; set; }

    public long ProdutoId { get; set; }

    public string NomeProduto { get; set; } = string.Empty;

    public string? SKU { get; set; }

    public int Quantidade { get; set; }

    public decimal PrecoUnitario { get; set; }

    public Pedido Pedido { get; set; } = null!;
}

