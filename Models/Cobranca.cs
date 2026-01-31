using System;

namespace Kanban.Models
{
    public class Cobranca
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public Cliente2? Cliente { get; set; }   // ✅ corrigido para Cliente2
        public decimal Valor { get; set; }
        public DateTime DataCobranca { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pendente";
    }
}
