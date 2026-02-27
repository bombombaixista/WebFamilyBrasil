using Kanban.Models;
using MeuSistema.Models; // seu namespace real
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Security.Claims;

namespace MeuSistema.Controllers
{
    [Authorize]
    public class EcommerceController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = true };

        public EcommerceController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // =========================
        // Helpers para pasta do usuário
        // =========================
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

        private string GetProdutosFilePath() => Path.Combine(GetUserDataPath(), "produtos.json");
        private string GetPedidosFilePath() => Path.Combine(GetUserDataPath(), "pedidos.json");

        // =========================
        // Mostra catálogo de produtos
        // =========================
        public IActionResult Index()
        {
            var produtos = CarregarProdutos();
            return View(produtos); // View tipada com IEnumerable<Produto>
        }

        // =========================
        // Comprar produto
        // =========================
        [HttpPost]
        public IActionResult Comprar(int produtoId, int quantidade, string cliente)
        {
            var produtos = CarregarProdutos();
            var produto = produtos.FirstOrDefault(p => p.Id == produtoId);

            if (produto == null || produto.Estoque < quantidade)
            {
                TempData["Erro"] = "Produto indisponível ou estoque insuficiente.";
                return RedirectToAction("Index");
            }

            // Atualiza estoque
            produto.Estoque -= quantidade;
            SalvarProdutos(produtos);

            // Cria pedido
            var pedidos = CarregarPedidos();
            var novoId = pedidos.Any() ? pedidos.Max(p => p.Id) + 1 : 1;

#pragma warning disable CS8601 // Possível atribuição de referência nula.
            var pedido = new Pedido
            {
                Id = novoId,
                Cliente = cliente,
                Itens = new List<ItemPedido>
                {
                    new ItemPedido
                    {
                        ProdutoId = produto.Id,
                        NomeProduto = produto.Nome,
                        Quantidade = quantidade,
                        PrecoUnitario = produto.Preco
                    }
                },
                ValorTotal = produto.Preco * quantidade,
                Data = DateTime.Now,
                Status = "Confirmado"
            };
#pragma warning restore CS8601 // Possível atribuição de referência nula.

            pedidos.Add(pedido);
            SalvarPedidos(pedidos);

            TempData["Sucesso"] = "Pedido realizado com sucesso!";
            return RedirectToAction("Pedidos");
        }

        // =========================
        // Lista pedidos confirmados
        // =========================
        public IActionResult Pedidos()
        {
            var pedidos = CarregarPedidos();
            return View(pedidos); // usa Views/Ecommerce/Pedidos.cshtml
        }

        // =========================
        // Helpers de persistência
        // =========================
        private List<Produto> CarregarProdutos()
        {
            var path = GetProdutosFilePath();
            if (!System.IO.File.Exists(path)) return new List<Produto>();
            var json = System.IO.File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<Produto>>(json, _jsonOptions) ?? new List<Produto>();
        }

        private void SalvarProdutos(List<Produto> produtos)
        {
            var path = GetProdutosFilePath();
            var json = JsonSerializer.Serialize(produtos, _jsonOptions);
            System.IO.File.WriteAllText(path, json);
        }

        private List<Pedido> CarregarPedidos()
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
    }
}
