using Kanban.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Security.Claims;

namespace Kanban.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class AgendaController : Controller
    {
        private readonly string _dataPath;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public AgendaController(IWebHostEnvironment env)
        {
            _dataPath = Path.Combine(env.ContentRootPath, "Data");
            Directory.CreateDirectory(_dataPath);
        }

        private string GetUserDataPath()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                throw new Exception("Usuário não logado.");

            // 🔹 Normaliza o email para nome de pasta
            var folderName = email.Replace("@", "_").Replace(".", "_");

            // 🔹 Estrutura correta: Kanban/Data/User2/<email>
            var userPath = Path.Combine(_dataPath, "User2", folderName);

            if (!Directory.Exists(userPath))
                Directory.CreateDirectory(userPath);

            return userPath;
        }

        private string GetAgendaFilePath()
        {
            // 🔹 Apenas junta o arquivo dentro da pasta do usuário
            return Path.Combine(GetUserDataPath(), "agenda.json");
        }

        // ====================== INDEX ======================
        [HttpGet]
        public IActionResult Index()
        {
            return View(); // mantém a view existente
        }

        // ====================== LISTAR ======================
        [HttpGet("Listar")]
        public IActionResult Listar()
        {
            var path = GetAgendaFilePath();
            if (!System.IO.File.Exists(path))
                return Ok(new List<AgendaEvento>());

            var json = System.IO.File.ReadAllText(path);
            var eventos = JsonSerializer.Deserialize<List<AgendaEvento>>(json, _jsonOptions) ?? new List<AgendaEvento>();
            return Ok(eventos);
        }

        // ====================== SALVAR ======================
        [HttpPost("Salvar")]
        public IActionResult Salvar([FromBody] AgendaEvento evento)
        {
            if (evento == null) return BadRequest("Evento inválido");

            var path = GetAgendaFilePath();
            var eventosExistentes = System.IO.File.Exists(path)
                ? JsonSerializer.Deserialize<List<AgendaEvento>>(System.IO.File.ReadAllText(path), _jsonOptions)
                ?? new List<AgendaEvento>()
                : new List<AgendaEvento>();

            if (evento.Id > 0)
            {
                var e = eventosExistentes.FirstOrDefault(ev => ev.Id == evento.Id);
                if (e != null)
                {
                    e.Titulo = evento.Titulo;
                    e.Categoria = evento.Categoria;
                    e.Descricao = evento.Descricao;
                    e.Inicio = evento.Inicio;
                    e.Fim = evento.Fim;
                }
                else
                {
                    evento.Id = eventosExistentes.Count == 0 ? 1 : eventosExistentes.Max(ev => ev.Id) + 1;
                    eventosExistentes.Add(evento);
                }
            }
            else
            {
                evento.Id = eventosExistentes.Count == 0 ? 1 : eventosExistentes.Max(ev => ev.Id) + 1;
                eventosExistentes.Add(evento);
            }

            System.IO.File.WriteAllText(path, JsonSerializer.Serialize(eventosExistentes, _jsonOptions));
            return Ok(evento);
        }

        // ====================== EXCLUIR ======================
        [HttpDelete("Excluir/{id}")]
        public IActionResult Excluir(int id)
        {
            var path = GetAgendaFilePath();
            if (!System.IO.File.Exists(path)) return NotFound();

            var eventosExistentes = JsonSerializer.Deserialize<List<AgendaEvento>>(System.IO.File.ReadAllText(path), _jsonOptions) ?? new List<AgendaEvento>();

            var evento = eventosExistentes.FirstOrDefault(e => e.Id == id);
            if (evento == null) return NotFound();

            eventosExistentes.Remove(evento);
            System.IO.File.WriteAllText(path, JsonSerializer.Serialize(eventosExistentes, _jsonOptions));

            return Ok(new { message = "Evento removido com sucesso" });
        }

        // ====================== IMPORTAR ======================
        [HttpPost("Importar")]
        public IActionResult Importar([FromBody] List<AgendaEvento> importados)
        {
            if (importados == null || importados.Count == 0)
                return BadRequest("Arquivo inválido ou vazio");

            var path = GetAgendaFilePath();
            var eventosExistentes = System.IO.File.Exists(path)
                ? JsonSerializer.Deserialize<List<AgendaEvento>>(System.IO.File.ReadAllText(path), _jsonOptions)
                ?? new List<AgendaEvento>()
                : new List<AgendaEvento>();

            int proximoId = eventosExistentes.Count == 0 ? 1 : eventosExistentes.Max(e => e.Id) + 1;

            foreach (var evt in importados)
            {
                bool existe = eventosExistentes.Any(e => e.Titulo == evt.Titulo && e.Inicio == evt.Inicio);
                if (!existe)
                {
                    evt.Id = proximoId++;
                    eventosExistentes.Add(evt);
                }
            }

            System.IO.File.WriteAllText(path, JsonSerializer.Serialize(eventosExistentes, _jsonOptions));

            return Ok(new { message = "Eventos importados com sucesso", total = eventosExistentes.Count });
        }
    }
}
