using Kanban.Models;
using Microsoft.EntityFrameworkCore;

namespace Kanban.Services
{
    public class MercadoLivreTokenService
    {
        private readonly AppDbContext _context;

        public MercadoLivreTokenService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cliente2?> GetClienteComTokenAsync(Guid clienteId)
        {
            return await _context.Clientes2
                .FirstOrDefaultAsync(c => c.Id == clienteId);
        }

        public async Task SalvarTokenAsync(
            Guid clienteId,
            string accessToken,
            string refreshToken,
            DateTime expiration,
            long userId)
        {
            var cliente = await _context.Clientes2
                .FirstOrDefaultAsync(c => c.Id == clienteId);

            if (cliente == null)
                return;

            cliente.MercadoLivreAccessToken = accessToken;
            cliente.MercadoLivreRefreshToken = refreshToken;
            cliente.MercadoLivreTokenExpiraEm = expiration;
            cliente.MercadoLivreUserId = userId;
            cliente.MercadoLivreConectado = true;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> TokenValidoAsync(Guid clienteId)
        {
            var cliente = await _context.Clientes2
                .FirstOrDefaultAsync(c => c.Id == clienteId);

            if (cliente == null)
                return false;

            return cliente.MercadoLivreTokenExpiraEm.HasValue &&
                   cliente.MercadoLivreTokenExpiraEm > DateTime.UtcNow;
        }
    }
}
