using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class DashboardMlController : Controller
{
    private readonly AppDbContext _context;

    public DashboardMlController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Integracoes()
    {
        var totalPedidos = await _context.Pedidos.CountAsync();
        var pedidosPagos = await _context.Pedidos
            .CountAsync(p => p.Status == "paid");

        var erros = await _context.LogIntegracoes
            .CountAsync(l => !l.Sucesso);

        ViewBag.TotalPedidos = totalPedidos;
        ViewBag.PedidosPagos = pedidosPagos;
        ViewBag.Erros = erros;


        return View();

    }
}