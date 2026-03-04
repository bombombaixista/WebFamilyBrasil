using System;

namespace Kanban.Models
{
    public class RelatorioAfiliado
    {
        public DateTime Data { get; set; }
        public string? Produto { get; set; }
        public int Cliques { get; set; }
        public int Vendas { get; set; }
        public decimal Comissao { get; set; }
    }
}
