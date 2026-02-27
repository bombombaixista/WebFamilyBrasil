public class LogIntegracao
{
    public long Id { get; set; }
    public string Tipo { get; set; } = string.Empty; // Webhook, SyncManual
    public string Marketplace { get; set; } = string.Empty;
    public string Evento { get; set; } = string.Empty;
    public string? Conteudo { get; set; }
    public bool Sucesso { get; set; }
    public string? Erro { get; set; }
    public DateTime Data { get; set; } = DateTime.UtcNow;
}