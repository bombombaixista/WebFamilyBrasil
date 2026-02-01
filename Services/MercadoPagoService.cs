using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebFamily.Services
{
    public class MercadoPagoService
    {
        private readonly HttpClient _httpClient;

        public MercadoPagoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Criar pagamento PIX
        public async Task<PaymentResponse> CriarPagamentoPixAsync(string accessToken, decimal valor, string descricao, string emailCliente)
        {
            var pagamento = new
            {
                transaction_amount = valor,
                description = descricao,
                payment_method_id = "pix",
                payer = new { email = emailCliente }
            };

            return await EnviarPagamentoAsync(accessToken, pagamento);
        }

        // Criar pagamento Cartão
        public async Task<PaymentResponse> CriarPagamentoCartaoAsync(string accessToken, decimal valor, string descricao, string emailCliente, string cardToken)
        {
            var pagamento = new
            {
                transaction_amount = valor,
                description = descricao,
                payment_method_id = "visa", // pode ser visa, mastercard etc.
                token = cardToken, // gerado pelo frontend com SDK do Mercado Pago
                payer = new
                {
                    email = emailCliente,
                    identification = new { type = "CPF", number = "12345678909" },
                    first_name = "João",
                    last_name = "Silva"
                }
            };

            return await EnviarPagamentoAsync(accessToken, pagamento);
        }

        // Criar pagamento Boleto
        public async Task<PaymentResponse> CriarPagamentoBoletoAsync(string accessToken, decimal valor, string descricao, string emailCliente)
        {
            var pagamento = new
            {
                transaction_amount = valor,
                description = descricao,
                payment_method_id = "bolbradesco",
                payer = new
                {
                    email = emailCliente,
                    first_name = "Maria",
                    last_name = "Souza",
                    identification = new { type = "CPF", number = "12345678909" },
                    address = new
                    {
                        zip_code = "12345678",
                        street_name = "Rua Exemplo",
                        street_number = "123",
                        neighborhood = "Centro",
                        city = "São Paulo",
                        federal_unit = "SP"
                    }
                }
            };

            return await EnviarPagamentoAsync(accessToken, pagamento);
        }

        // Consultar status de pagamento
        public async Task<PaymentResponse> ConsultarPagamentoAsync(string accessToken, long paymentId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.mercadopago.com/v1/payments/{paymentId}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PaymentResponse>(json)!;

        }

        // Método privado para enviar pagamento
        private async Task<PaymentResponse> EnviarPagamentoAsync(string accessToken, object pagamento)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.mercadopago.com/v1/payments");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(JsonSerializer.Serialize(pagamento), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PaymentResponse>(json)!;

        }

        internal async Task CriarPagamentoAsync(string accessToken, decimal valor, string descricao, string emailCliente)
        {
            throw new NotImplementedException();
        }
    }

    // DTOs para mapear resposta da API
    public class PaymentResponse
    {
        public long Id { get; set; }
        public string? Status { get; set; } // approved, pending, rejected
        public string? StatusDetail { get; set; }
        public string? Description { get; set; }
        public TransactionDetails? TransactionDetails { get; set; }
        public Payer? Payer { get; set; }
    }

    public class TransactionDetails
    {
        public string? ExternalResourceUrl { get; set; } // QR Code PIX ou link do boleto
    }

    public class Payer
    {
        public string? Email { get; set; }
    }
}
