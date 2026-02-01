public class PagamentoRequest
{
    public decimal Valor { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string EmailCliente { get; set; } = string.Empty;
    public string Metodo { get; set; } = string.Empty; // pix, cartao, boleto
    public string? CardToken { get; set; } // usado apenas para cartão
}
