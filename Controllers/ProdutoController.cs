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
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = true };

        public ProdutoController(IWebHostEnvironment env)
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

        // =========================
        // Listagem de produtos
        // =========================
        [HttpGet]
        public IActionResult Index()
        {
            var produtos = CarregarProdutos();
            return View(produtos); // View tipada com IEnumerable<Produto>
        }

        // =========================
        // Criar produto
        // =========================
        [HttpGet("Create")]
        public IActionResult Create() => View();

        [HttpPost("Create")]
        public IActionResult Create(Produto produto)
        {
            var produtos = CarregarProdutos();
            produto.Id = produtos.Any() ? produtos.Max(p => p.Id) + 1 : 1;
            produtos.Add(produto);
            SalvarProdutos(produtos);

            TempData["Sucesso"] = "Produto criado com sucesso!";
            return RedirectToAction("Index");
        }

        // =========================
        // Editar produto
        // =========================
        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var produtos = CarregarProdutos();
            var produto = produtos.FirstOrDefault(p => p.Id == id);
            if (produto == null) return NotFound();
            return View(produto);
        }

        [HttpPost("Edit/{id}")]
        public IActionResult Edit(int id, Produto produtoAtualizado)
        {
            var produtos = CarregarProdutos();
            var produto = produtos.FirstOrDefault(p => p.Id == id);
            if (produto == null) return NotFound();

            produto.Nome = produtoAtualizado.Nome;
            produto.Categoria = produtoAtualizado.Categoria;
            produto.Preco = produtoAtualizado.Preco;
            produto.Estoque = produtoAtualizado.Estoque;
            produto.Descricao = produtoAtualizado.Descricao;
            produto.ImagemUrl = produtoAtualizado.ImagemUrl;

            SalvarProdutos(produtos);

            TempData["Sucesso"] = "Produto atualizado com sucesso!";
            return RedirectToAction("Index");
        }

        // =========================
        // Apagar produto
        // =========================
        [HttpPost("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var produtos = CarregarProdutos();
            var produto = produtos.FirstOrDefault(p => p.Id == id);
            if (produto == null) return NotFound();

            produtos.Remove(produto);
            SalvarProdutos(produtos);

            TempData["Sucesso"] = "Produto removido com sucesso!";
            return RedirectToAction("Index");
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
    }
}
