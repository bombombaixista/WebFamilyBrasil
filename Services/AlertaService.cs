using Kanban.DTOs;
using Kanban.Models;

namespace Kanban.Services
{
    public class AlertaService
    {
        private readonly AppDbContext _context;

        public AlertaService(AppDbContext context)
        {
            _context = context;
        }

        public List<AlertaDto> ObterAlertas()
        {
            return _context.Set<Alerta>()
                .Where(a => !a.Resolvido)
                .Select(a => new AlertaDto
                {
                    Id = a.Id,
                    Mensagem = a.Mensagem,
                    Data = a.Data,
                    Resolvido = a.Resolvido
                })
                .ToList();
        }

        public void MarcarResolvido(int id)
        {
            var alerta = _context.Set<Alerta>().Find(id);
            if (alerta != null)
            {
                alerta.Resolvido = true;
                _context.SaveChanges();
            }
        }
    }
}
