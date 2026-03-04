using Kanban.DTOs;
using Kanban.Models;

namespace Kanban.Services
{
    public class SincronizacaoService
    {
        private readonly AppDbContext _context;

        public SincronizacaoService(AppDbContext context)
        {
            _context = context;
        }

        public List<SincronizacaoDto> ObterHistorico()
        {
            return _context.Set<LogSincronizacao>()
                .OrderByDescending(l => l.Data)
                .Select(l => new SincronizacaoDto
                {
                    Id = l.Id,
                    Data = l.Data,
                    QuantidadeProdutos = l.QuantidadeProdutos,
                    Status = l.Status
                })
                .ToList();
        }
    }
}
