using System;
using System.Numerics;

namespace Kanban.Models
{
    public class Cliente2
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public Guid PlanoId { get; set; }
        public Plano? Plano { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
        public bool Ativo { get; set; } = true;
    }
}
