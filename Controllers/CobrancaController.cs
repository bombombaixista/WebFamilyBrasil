using Kanban.Models;
using Kanban.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kanban.Controllers
{
    public class CobrancaController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly AvisoPagamentoService _avisoPagamentoService;
        private readonly IConfiguration _configuration;

        public CobrancaController(AppDbContext dbContext, AvisoPagamentoService avisoPagamentoService, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _avisoPagamentoService = avisoPagamentoService;
            _configuration = configuration;
        }

        // 🔹 Gera cobrança e envia aviso
        [HttpPost]
        public async Task<IActionResult> Gerar(Guid clienteId)
        {
            var cliente = await _dbContext.Clientes2
                .Include(c => c.Plano)
                .FirstOrDefaultAsync(c => c.Id == clienteId);

            if (cliente == null)
                return NotFound("Cliente não encontrado.");

            var plano = cliente.Plano;
            if (plano == null)
                return BadRequest("Cliente não possui plano associado.");

            // Cria cobrança
            var cobranca = new Cobranca
            {
                Id = Guid.NewGuid(),
                ClienteId = cliente.Id,
                Valor = plano.PrecoMensal,
                DataCobranca = DateTime.UtcNow,
                Status = "Pendente"
            };

            _dbContext.Cobrancas.Add(cobranca);
            await _dbContext.SaveChangesAsync();

            // Gera pagamento PIX e envia aviso
            var accessToken = _configuration["MercadoPago:AccessToken"]!;
            await _avisoPagamentoService.AvisarClientePixAsync(
                accessToken,
                cliente.Email,
                cliente.Nome,
                plano.PrecoMensal,
                $"Cobrança do plano {plano.Nome}"
            );

            return Ok($"Cobrança criada e aviso enviado para {cliente.Email}");
        }

        // 🔹 Lista cobranças do cliente
        [HttpGet]
        public async Task<IActionResult> Listar(Guid clienteId)
        {
            var cobrancas = await _dbContext.Cobrancas
                .Where(c => c.ClienteId == clienteId)
                .OrderByDescending(c => c.DataCobranca)
                .ToListAsync();

            return Ok(cobrancas);
        }
    }
}
