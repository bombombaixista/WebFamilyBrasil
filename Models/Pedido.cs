using Kanban.Models;
namespace Kanban.Models;
public class Pedido
{
    public long Id { get; set; }

    // 🔥 Relacionamento com Cliente2
    public Guid Cliente2Id { get; set; }

    public string? MarketplaceOrderId { get; set; }

    public string Cliente { get; set; } = string.Empty;

    public decimal ValorTotal { get; set; }

    public DateTime Data { get; set; }

    public string Status { get; set; } = "Pendente";

    public string Origem { get; set; } = "Manual";

    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? CEP { get; set; }

    public string? Transportadora { get; set; }
    public string? CodigoRastreamento { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? UltimaAtualizacao { get; set; }

    public string? JsonOriginal { get; set; }

    public Cliente2 Cliente2 { get; set; } = null!;

    public List<ItemPedido> Itens { get; set; } = new();
}