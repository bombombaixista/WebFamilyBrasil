public class User2
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string SenhaHash { get; set; } = string.Empty;

    public string Plano { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; } = DateTime.Now;
    public DateTime DataExpiracao { get; set; }
}
