using System.Net;
using System.Net.Mail;

namespace Kanban.Services
{
    public class EmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;

        public EmailService(string smtpHost, int smtpPort, string smtpUser, string smtpPass)
        {
            _smtpHost = smtpHost;
            _smtpPort = smtpPort;
            _smtpUser = smtpUser;
            _smtpPass = smtpPass;
        }

        public void EnviarAvisoPagamento(string emailDestino, string nomeCliente, decimal valor, string linkPagamento)
        {
            var smtp = new SmtpClient(_smtpHost)
            {
                Port = _smtpPort,
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true
            };

            var mensagem = new MailMessage(_smtpUser, emailDestino)
            {
                Subject = "Aviso de Cobrança - KanbanERP",
                Body = $"Olá {nomeCliente},\n\n" +
                       $"Sua cobrança de R$ {valor} está disponível.\n" +
                       $"Clique no link abaixo para realizar o pagamento via Mercado Pago:\n\n" +
                       $"{linkPagamento}\n\n" +
                       "Obrigado por utilizar nosso sistema!"
            };

            smtp.Send(mensagem);
        }
    }
}
