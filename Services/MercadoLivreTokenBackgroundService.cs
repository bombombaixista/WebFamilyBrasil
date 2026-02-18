using Kanban.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

public class MercadoLivreTokenBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public MercadoLivreTokenBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<MercadoLivreTokenService>();

            var clientes = await dbContext.Clientes2
                .Where(c => c.MercadoLivreConectado && c.MercadoLivreTokenExpiraEm <= DateTime.UtcNow.AddMinutes(10))
                .ToListAsync();

            foreach (var cliente in clientes)
            {
                try
                {
                    await tokenService.GetValidTokenAsync(cliente.Id);
                    Console.WriteLine($"Token do cliente {cliente.Nome} renovado com sucesso.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao renovar token do cliente {cliente.Nome}: {ex.Message}");
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }
}
