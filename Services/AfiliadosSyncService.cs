using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Kanban.Services
{
    public class AfiliadosSyncService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer? _timer;

        public AfiliadosSyncService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Executa imediatamente e depois a cada 5 minutos
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            using var scope = _scopeFactory.CreateScope();
            var mlService = scope.ServiceProvider.GetRequiredService<MercadoLivreService>();

            // Apenas consulta produtos em tempo real
            var produtos = await mlService.BuscarProdutosAsync("tenis", 10);

            // Aqui você poderia logar, enviar notificação, etc.
            Console.WriteLine($"[Sync] {produtos.Count} produtos carregados do Mercado Livre.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
