using System;
using System.Collections.Generic;

namespace WebFamily.Models
{
    public class PedidoDto
    {
        public long Id { get; set; }

        // Origem do pedido (Mercado Livre, Shopee, AliExpress)
        public string Origem { get; set; } = "";

        // Dados do cliente
        public string Cliente { get; set; } = "";
        public DateTime Data { get; set; }

        // Valor do pedido
        public decimal Valor { get; set; }   // simples
        public decimal ValorTotal { get; set; } // mais descritivo, se preferir

        // Status do pedido (Pago, Enviado, Aguardando envio, etc.)
        public string Status { get; set; } = "";

        // Endereço de entrega
        public string Endereco { get; set; } = "";
        public string Cidade { get; set; } = "";
        public string Estado { get; set; } = "";
        public string CEP { get; set; } = "";

        // Informações de envio
        public string Transportadora { get; set; } = "";
        public string CodigoRastreamento { get; set; } = "";

        // Itens do pedido
        public List<ItemPedidoDto> Itens { get; set; } = new();
    }

    public class ItemPedidoDto
    {
        public string SKU { get; set; } = "";
        public string NomeProduto { get; set; } = "";
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
