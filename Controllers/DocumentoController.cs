using Kanban.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Claims;

namespace Kanban.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class DocumentoController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly JsonSerializerOptions _jsonOptions;

        public DocumentoController(IWebHostEnvironment env)
        {
            _env = env;
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
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

        private string GetDocumentosFilePath()
        {
            return Path.Combine(GetUserDataPath(), "documento.json");
        }

        // =========================
        // INDEX
        // =========================
        [HttpGet]
        public IActionResult Index()
        {
            var docs = CarregarDocumentos();

            var totalClientes = docs
                .Select(d => d.Nome)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();

            ViewBag.TotalClientes = totalClientes;
            ViewBag.TotalDocumentos = docs.Count;
            ViewBag.TotalVendas = 0;

            var categorias = docs
                .GroupBy(d => d.Categoria)
                .Select(g => new { Categoria = g.Key, Quantidade = g.Count() })
                .ToList();

            ViewBag.Categorias = categorias.Select(c => c.Categoria).ToList();
            ViewBag.Quantidades = categorias.Select(c => c.Quantidade).ToList();

            var docsPorCliente = docs
                .GroupBy(d => d.Nome)
                .Select(g => new { Cliente = g.Key, Quantidade = g.Count() })
                .ToList();

            ViewBag.Clientes = docsPorCliente.Select(c => c.Cliente).ToList();
            ViewBag.DocsPorCliente = docsPorCliente.Select(c => c.Quantidade).ToList();

            return View(docs);
        }

        // =========================
        // UPLOAD (GET)
        // =========================
        [HttpGet("Upload")]
        public IActionResult Upload()
        {
            return View();
        }

        // =========================
        // UPLOAD (POST)
        // =========================
        [HttpPost("Upload")]
        [ValidateAntiForgeryToken]
        public IActionResult Upload(string cliente, List<IFormFile> arquivos, string categoria)
        {
            if (string.IsNullOrWhiteSpace(cliente) || arquivos == null || arquivos.Count == 0)
            {
                TempData["Erro"] = "Informe o cliente e selecione ao menos um arquivo.";
                return RedirectToAction("Upload");
            }

            var docs = CarregarDocumentos();

            var pastaCliente = Path.Combine(GetUserDataPath(), "docs", SanitizeFolder(cliente));
            Directory.CreateDirectory(pastaCliente);

            foreach (var arquivo in arquivos)
            {
                if (arquivo.Length > 0)
                {
                    var nomeArquivo = Path.GetFileName(arquivo.FileName);
                    var caminhoFisico = Path.Combine(pastaCliente, nomeArquivo);

                    using var stream = new FileStream(caminhoFisico, FileMode.Create);
                    arquivo.CopyTo(stream);

                    var novoId = docs.Any() ? docs.Max(d => d.Id) + 1 : 1;

                    docs.Add(new Documento
                    {
                        Id = novoId,
                        Nome = cliente,
                        Categoria = string.IsNullOrWhiteSpace(categoria) ? "Documento" : categoria,
                        Caminho = $"/docs/{SanitizeFolder(cliente)}/{nomeArquivo}",
                        DataUpload = DateTime.Now
                    });
                }
            }

            SalvarDocumentos(docs);
            TempData["Sucesso"] = "Upload realizado com sucesso.";
            return RedirectToAction("Index");
        }

        // =========================
        // HELPERS
        // =========================
        private List<Documento> CarregarDocumentos()
        {
            var path = GetDocumentosFilePath();
            if (!System.IO.File.Exists(path))
                return new List<Documento>();

            var json = System.IO.File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<Documento>>(json, _jsonOptions) ?? new List<Documento>();
        }

        private void SalvarDocumentos(List<Documento> docs)
        {
            var path = GetDocumentosFilePath();
            var dir = Path.GetDirectoryName(path)!;
            Directory.CreateDirectory(dir);
            var json = JsonSerializer.Serialize(docs, _jsonOptions);
            System.IO.File.WriteAllText(path, json);
        }

        private static string SanitizeFolder(string name)
        {
            var invalid = Path.GetInvalidFileNameChars();
            return new string(name.Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray()).Trim();
        }
    }
}
