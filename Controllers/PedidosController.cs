using Microsoft.AspNetCore.Mvc;
using Kanban.Models;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Security.Claims;

namespace Kanban.Controllers
{
    [Authorize]
    [Route("Pedidos")]
    public class PedidosController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

        public PedidosController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // Helpers
        private string GetUserDataPath()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                throw new Exception("Usuário não logado.");

            var folderName = email.Replace("@", "_").Replace(".", "_");
            var userPath = Path.Combine(_env.ContentRootPath, "Data", "User2", folderName);

            if (!Directory.Exists(userPath))
                Directory.CreateDirectory(userPath);

            return userPath;
        }

        private string GetPedidosFilePath() => Path.Combine(GetUserDataPath(), "pedidos.json");

        private List<Pedido> GetPedidos()
        {
            var path = GetPedidosFilePath();
            if (!System.IO.File.Exists(path)) return new List<Pedido>();
            var json = System.IO.File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<Pedido>>(json, _jsonOptions) ?? new List<Pedido>();
        }

        private void SalvarPedidos(List<Pedido> pedidos)
        {
            var path = GetPedidosFilePath();
            var json = JsonSerializer.Serialize(pedidos, _jsonOptions);
            System.IO.File.WriteAllText(path, json);
        }

        // INDEX
        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index(string cliente, string origem, string status)
        {
            var pedidos = GetPedidos().AsQueryable();

            if (!string.IsNullOrWhiteSpace(cliente))
                pedidos = pedidos.Where(p => p.Cliente.Contains(cliente, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(origem))
                pedidos = pedidos.Where(p => p.Origem == origem);

            if (!string.IsNullOrWhiteSpace(status))
                pedidos = pedidos.Where(p => p.Status == status);

            return View(pedidos.ToList());
        }

        // COMPRAR (novo método)
        [HttpPost("Comprar")]
        public IActionResult Comprar(int produtoId, int quantidade, string cliente)
        {
            var pedidos = GetPedidos();

            // Aqui você pode buscar o produto real do seu repositório/DB
            // Exemplo simples: produto fictício
            var produto = new Produto { Id = produtoId, Nome = "Produto X", Preco = 100, Estoque = 10 };

            var novoPedido = new Pedido
            {
                Id = pedidos.Count > 0 ? pedidos.Max(p => p.Id) + 1 : 1,
                Cliente = cliente,
                Origem = "E-commerce",
                Status = "Pago",
                Data = DateTime.Now,
                ValorTotal = produto.Preco * quantidade
            };

            pedidos.Add(novoPedido);
            SalvarPedidos(pedidos);

            return RedirectToAction("Index");
        }

        // DETALHES
        [HttpGet("Detalhes/{id}")]
        public IActionResult Detalhes(long id) => View(GetPedidos().FirstOrDefault(p => p.Id == id));

        // ATUALIZAR STATUS
        [HttpGet("AtualizarStatus/{id}")]
        public IActionResult AtualizarStatus(long id) => View(GetPedidos().FirstOrDefault(p => p.Id == id));

        [HttpPost("AtualizarStatus/{id}")]
        public IActionResult AtualizarStatus(long id, string status)
        {
            var pedidos = GetPedidos();
            var pedido = pedidos.FirstOrDefault(p => p.Id == id);
            if (pedido != null)
            {
                pedido.Status = status;
                SalvarPedidos(pedidos);
            }
            return RedirectToAction("Index");
        }

        // IMPRIMIR ETIQUETA
        [HttpGet("ImprimirEtiqueta/{id}")]
        public IActionResult ImprimirEtiqueta(long id) => View(GetPedidos().FirstOrDefault(p => p.Id == id));

        // ADICIONAR (API)
        [HttpPost("Adicionar")]
        public IActionResult Adicionar([FromBody] Pedido pedido)
        {
            var pedidos = GetPedidos();
            pedido.Id = pedidos.Count > 0 ? pedidos.Max(p => p.Id) + 1 : 1;
            pedido.Data = DateTime.Now;
            pedidos.Add(pedido);
            SalvarPedidos(pedidos);
            return Ok(pedido);
        }
    }
}
