using System;
using System.Threading.Tasks;

namespace Kanban.Services
{
    public class AvisoPagamentoService
    {
        private readonly MercadoPagoService _mercadoPagoService;
        private readonly EmailService _emailService;

        public AvisoPagamentoService(MercadoPagoService mercadoPagoService, EmailService emailService)
        {
            _mercadoPagoService = mercadoPagoService;
            _emailService = emailService;
        }

        public async Task AvisarClientePixAsync(string accessToken, string emailCliente, string nomeCliente, decimal valor, string descricao)
        {
            // Cria pagamento PIX
            var pagamento = await _mercadoPagoService.CriarPagamentoPixAsync(accessToken, valor, descricao, emailCliente);

            // Pega o link/QR code
            var linkPagamento = pagamento.TransactionDetails?.ExternalResourceUrl ?? "Link não disponível";

            // Envia email
            _emailService.EnviarAvisoPagamento(emailCliente, nomeCliente, valor, linkPagamento);
        }

        public async Task AvisarClienteBoletoAsync(string accessToken, string emailCliente, string nomeCliente, decimal valor, string descricao)
        {
            var pagamento = await _mercadoPagoService.CriarPagamentoBoletoAsync(accessToken, valor, descricao, emailCliente);

            var linkPagamento = pagamento.TransactionDetails?.ExternalResourceUrl ?? "Link não disponível";

            _emailService.EnviarAvisoPagamento(emailCliente, nomeCliente, valor, linkPagamento);
        }

        // Se quiser também para cartão, pode adaptar aqui
    }
}
