namespace Kanban.Models
{
    public class User
    {
        public Guid Id { get; set; }   // ✅ agora aceita GUID
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public string Plano { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; }
        public DateTime DataExpiracao { get; set; }
    }
}
