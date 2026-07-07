using Kanban.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Security.Claims;

namespace Kanban.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class FinanceiroController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

        public FinanceiroController(IWebHostEnvironment env)
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

        private string GetFinanceiroFilePath()
        {
            return Path.Combine(GetUserDataPath(), "financeiro.json");
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        public IActionResult Create(Financeiro registro)
        {
            var path = GetFinanceiroFilePath();
            var registros = System.IO.File.Exists(path)
                ? JsonSerializer.Deserialize<List<Financeiro>>(System.IO.File.ReadAllText(path), _jsonOptions) ?? new List<Financeiro>()
                : new List<Financeiro>();

            registro.Id = registros.Count > 0 ? registros.Max(r => r.Id) + 1 : 1;
            registros.Add(registro);

            System.IO.File.WriteAllText(path, JsonSerializer.Serialize(registros, _jsonOptions));
            return RedirectToAction("RelatorioDRE");
        }


        [HttpGet("RelatorioDRE")]
        public IActionResult RelatorioDRE()
        {
            var path = GetFinanceiroFilePath();
            if (!System.IO.File.Exists(path)) return View(new DreRelatorio());

            var registros = JsonSerializer.Deserialize<List<Financeiro>>(System.IO.File.ReadAllText(path), _jsonOptions) ?? new List<Financeiro>();
            var dreService = new Kanban.Services.DreService();
            var relatorio = dreService.GerarDre(registros);

            return View(relatorio);
        }



        // =========================
        // INDEX
        // =========================
        [HttpGet]
        public IActionResult Index() => View();

        // =========================
        // LISTAR
        // =========================
        [HttpGet("Listar")]
        public IActionResult Listar()
        {
            var path = GetFinanceiroFilePath();
            if (!System.IO.File.Exists(path)) return Ok(new List<Financeiro>());
            var json = System.IO.File.ReadAllText(path);
            var registros = JsonSerializer.Deserialize<List<Financeiro>>(json, _jsonOptions) ?? new List<Financeiro>();
            return Ok(registros);
        }

        // =========================
        // ADICIONAR
        // =========================
        [HttpPost("Adicionar")]
        public IActionResult Adicionar([FromBody] Financeiro registro)
        {
            var path = GetFinanceiroFilePath();
            var registros = System.IO.File.Exists(path)
                ? JsonSerializer.Deserialize<List<Financeiro>>(System.IO.File.ReadAllText(path), _jsonOptions) ?? new List<Financeiro>()
                : new List<Financeiro>();

            registro.Id = registros.Count > 0 ? registros.Max(r => r.Id) + 1 : 1;
            registros.Add(registro);

            System.IO.File.WriteAllText(path, JsonSerializer.Serialize(registros, _jsonOptions));
            return Ok(registro);
        }

        // =========================
        // EDITAR
        // =========================
        [HttpPut("Editar/{id}")]
        public IActionResult Editar(int id, [FromBody] Financeiro registroAtualizado)
        {
            var path = GetFinanceiroFilePath();
            if (!System.IO.File.Exists(path)) return NotFound();

            var registros = JsonSerializer.Deserialize<List<Financeiro>>(System.IO.File.ReadAllText(path), _jsonOptions) ?? new List<Financeiro>();
            var registro = registros.FirstOrDefault(r => r.Id == id);
            if (registro == null) return NotFound();

            registro.Descricao = registroAtualizado.Descricao;
            registro.Valor = registroAtualizado.Valor;
            registro.Data = registroAtualizado.Data;
            registro.Tipo = registroAtualizado.Tipo;
            registro.Categoria = registroAtualizado.Categoria;

            System.IO.File.WriteAllText(path, JsonSerializer.Serialize(registros, _jsonOptions));
            return Ok(registro);
        }

        // =========================
        // APAGAR
        // =========================
        [HttpDelete("Apagar/{id}")]
        public IActionResult Apagar(int id)
        {
            var path = GetFinanceiroFilePath();
            if (!System.IO.File.Exists(path)) return NotFound();

            var registros = JsonSerializer.Deserialize<List<Financeiro>>(System.IO.File.ReadAllText(path), _jsonOptions) ?? new List<Financeiro>();
            var registro = registros.FirstOrDefault(r => r.Id == id);
            if (registro == null) return NotFound();

            registros.Remove(registro);
            System.IO.File.WriteAllText(path, JsonSerializer.Serialize(registros, _jsonOptions));
            return Ok(new { message = "Registro financeiro removido com sucesso!" });
        }
    }
}
