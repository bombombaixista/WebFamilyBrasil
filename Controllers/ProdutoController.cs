using Kanban.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Security.Claims;

namespace Kanban.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ProdutoController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public ProdutoController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // ===================== HELPERS =====================
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
        private string GetMovFilePath() => Path.Combine(GetUserDataPath(), "movimentacoes.json");

        private List<Produto> LerProdutos()
        {
            var path = GetProdutosFilePath();
            if (!System.IO.File.Exists(path)) System.IO.File.WriteAllText(path, "[]");
            var json = System.IO.File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<Produto>>(json) ?? new();
        }

        private void SalvarProdutos(List<Produto> produtos)
        {
            var path = GetProdutosFilePath();
            var json = JsonSerializer.Serialize(produtos, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(path, json);
        }

        private List<Movimentacao> LerMovimentacoes()
        {
            var path = GetMovFilePath();
            if (!System.IO.File.Exists(path)) System.IO.File.WriteAllText(path, "[]");
            var json = System.IO.File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<Movimentacao>>(json) ?? new();
        }

        private void SalvarMovimentacoes(List<Movimentacao> movs)
        {
            var path = GetMovFilePath();
            var json = JsonSerializer.Serialize(movs, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(path, json);
        }

        // ===================== LISTAGEM =====================
        [HttpGet]
        public IActionResult Index()
        {
            return View(LerProdutos());
        }

        [HttpGet("Movimentacoes")]
        public IActionResult Movimentacoes()
        {
            return View(LerMovimentacoes());
        }

        // ===================== CRUD =====================
        [HttpGet("Create")]
        public IActionResult Create() => View();

        [HttpPost("Create")]
        public IActionResult Create(Produto produto)
        {
            var produtos = LerProdutos();
            produto.Id = produtos.Count > 0 ? produtos.Max(p => p.Id) + 1 : 1;
            produtos.Add(produto);
            SalvarProdutos(produtos);
            return RedirectToAction("Index");
        }

        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var produto = LerProdutos().FirstOrDefault(p => p.Id == id);
            if (produto == null) return NotFound();
            return View(produto);
        }

        [HttpPost("Edit/{id}")]
        public IActionResult Edit(int id, Produto atualizado)
        {
            var produtos = LerProdutos();
            var produto = produtos.FirstOrDefault(p => p.Id == id);
            if (produto == null) return NotFound();

            produto.Nome = atualizado.Nome;
            produto.Categoria = atualizado.Categoria;
            produto.Fornecedor = atualizado.Fornecedor;
            produto.Marca = atualizado.Marca;
            produto.Tamanho = atualizado.Tamanho;
            produto.Cor = atualizado.Cor;
            produto.Material = atualizado.Material;
            produto.Estoque = atualizado.Estoque;
            produto.Preco = atualizado.Preco;

            SalvarProdutos(produtos);
            return RedirectToAction("Index");
        }

        // ===================== ESTOQUE =====================
        [HttpPost("Entrada")]
        public IActionResult Entrada(int id, int quantidade)
        {
            if (quantidade <= 0)
                return RedirectToAction("Index");

            var produtos = LerProdutos();
            var produto = produtos.FirstOrDefault(p => p.Id == id);
            if (produto == null)
                return RedirectToAction("Index");

            produto.Estoque += quantidade;
            SalvarProdutos(produtos);

            var movs = LerMovimentacoes();
            movs.Add(new Movimentacao
            {
                Id = movs.Count > 0 ? movs.Max(m => m.Id) + 1 : 1,
                ProdutoId = id,
                Tipo = "Entrada",
                Quantidade = quantidade,
                Data = DateTime.Now
            });
            SalvarMovimentacoes(movs);

            return RedirectToAction("Index");
        }

        [HttpPost("Saida")]
        public IActionResult Saida(int id, int quantidade)
        {
            if (quantidade <= 0)
                return RedirectToAction("Index");

            var produtos = LerProdutos();
            var produto = produtos.FirstOrDefault(p => p.Id == id);
            if (produto == null)
                return RedirectToAction("Index");

            if (produto.Estoque < quantidade)
                return RedirectToAction("Index");

            produto.Estoque -= quantidade;
            SalvarProdutos(produtos);

            var movs = LerMovimentacoes();
            movs.Add(new Movimentacao
            {
                Id = movs.Count > 0 ? movs.Max(m => m.Id) + 1 : 1,
                ProdutoId = id,
                Tipo = "Saída",
                Quantidade = quantidade,
                Data = DateTime.Now
            });
            SalvarMovimentacoes(movs);

            return RedirectToAction("Index");
        }
    }
}
