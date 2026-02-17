namespace Kanban.Models
{
    public class MercadoLivreToken
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public DateTime ExpirationDate { get; set; }
        public long UserId { get; set; }
    }
}
