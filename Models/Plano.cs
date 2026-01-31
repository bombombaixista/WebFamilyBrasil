using System;

namespace Kanban.Models
{
    public class Plano
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public decimal PrecoMensal { get; set; }
    }
}
