using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Kanban.Services;


namespace WebFamily.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagamentosController : ControllerBase
    {
        private readonly MercadoPagoService _mercadoPagoService;
        private readonly string _accessToken;

        public PagamentosController(MercadoPagoService mercadoPagoService, IConfiguration configuration)
        {
            _mercadoPagoService = mercadoPagoService;
            _accessToken = configuration["MercadoPago:AccessToken"] ?? string.Empty;
        }

        [HttpPost("criar")]
        public async Task<IActionResult> CriarPagamento([FromBody] PagamentoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Metodo))
                return BadRequest("Informe o método de pagamento: pix, cartao ou boleto.");

            if (string.IsNullOrWhiteSpace(request.Descricao) || string.IsNullOrWhiteSpace(request.EmailCliente))
                return BadRequest("Descrição e EmailCliente são obrigatórios.");

            switch (request.Metodo.ToLower())
            {
                case "pix":
                    var pix = await _mercadoPagoService.CriarPagamentoPixAsync(
                        _accessToken,
                        request.Valor,
                        request.Descricao,
                        request.EmailCliente
                    );
                    return Ok(pix);

                case "cartao":
                    if (string.IsNullOrEmpty(request.CardToken))
                        return BadRequest("É necessário informar o CardToken para pagamentos com cartão.");

                    var cartao = await _mercadoPagoService.CriarPagamentoCartaoAsync(
                        _accessToken,
                        request.Valor,
                        request.Descricao,
                        request.EmailCliente,
                        request.CardToken
                    );
                    return Ok(cartao);

                case "boleto":
                    var boleto = await _mercadoPagoService.CriarPagamentoBoletoAsync(
                        _accessToken,
                        request.Valor,
                        request.Descricao,
                        request.EmailCliente
                    );
                    return Ok(boleto);

                default:
                    return BadRequest("Método de pagamento inválido. Use: pix, cartao ou boleto.");
            }
        }

        [HttpGet("status/{id}")]
        public async Task<IActionResult> ConsultarPagamento(long id)
        {
            var status = await _mercadoPagoService.ConsultarPagamentoAsync(_accessToken, id);
            return Ok(status);
        }
    }

    // DTO para requisição
    public class PagamentoRequest
    {
        public decimal Valor { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string EmailCliente { get; set; } = string.Empty;
        public string Metodo { get; set; } = string.Empty; // pix, cartao, boleto
        public string? CardToken { get; set; } // usado apenas para cartão
    }
}
