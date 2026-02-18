using Kanban.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Kanban.Controllers
{
    [ApiController]
    [Route("api/webhook/mercadopago")]
    public class MercadoPagoWebhookController : ControllerBase
    {
        private readonly MercadoPagoService _mercadoPagoService;
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public MercadoPagoWebhookController(MercadoPagoService mercadoPagoService, AppDbContext dbContext, IConfiguration configuration)
        {
            _mercadoPagoService = mercadoPagoService;
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> ReceberEvento([FromBody] JsonElement evento)
        {
            try
            {
                // O Mercado Pago envia um campo "data.id" com o ID do pagamento
                if (evento.TryGetProperty("data", out var data) &&
                    data.TryGetProperty("id", out var idProp) &&
                    long.TryParse(idProp.GetString(), out var paymentId))
                {
                    var accessToken = _configuration["MercadoPago:AccessToken"]!;
                    var pagamento = await _mercadoPagoService.ConsultarPagamentoAsync(accessToken, paymentId);

                    // Atualiza a cobrança correspondente
                    var cobranca = _dbContext.Cobrancas.FirstOrDefault(c => c.Id == Guid.Parse(pagamento.Id.ToString()));
                    if (cobranca != null)
                    {
                        cobranca.Status = pagamento.Status ?? "Desconhecido";
                        await _dbContext.SaveChangesAsync();
                    }

                    return Ok();
                }

                return BadRequest("Evento inválido");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro no webhook: {ex.Message}");
            }
        }
    }
}
