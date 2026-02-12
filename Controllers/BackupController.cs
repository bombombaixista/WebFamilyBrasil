using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;

namespace Kanban.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class BackupController : Controller
    {
        private readonly string _baseDataPath;

        public BackupController(IWebHostEnvironment env)
        {
            _baseDataPath = Path.Combine(env.ContentRootPath, "Data");
            Directory.CreateDirectory(_baseDataPath);
        }

        // =========================
        // Helper – caminho do usuário
        // =========================
        private string GetUserDataPath()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("Usuário não autenticado.");

            var folderName = email
                .Replace("@", "_")
                .Replace(".", "_");

            var userPath = Path.Combine(_baseDataPath, "User2", folderName);

            if (!Directory.Exists(userPath))
                Directory.CreateDirectory(userPath);

            return userPath;
        }

        // =========================
        // Tela principal
        // =========================
        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            var userPath = GetUserDataPath();

            var arquivosJson = Directory
                .GetFiles(userPath, "*.json")
                .Select(Path.GetFileName)
                .ToList();

            return View(arquivosJson);
        }

        // =========================
        // EXPORTAR – SOMENTE JSON
        // =========================
        [HttpGet("ExportarTudo")]
        public IActionResult ExportarTudo()
        {
            var userPath = GetUserDataPath();
            var arquivosJson = Directory.GetFiles(userPath, "*.json");

            if (!arquivosJson.Any())
                return BadRequest("Nenhum arquivo JSON encontrado para backup.");

            var nomeZip = $"backup_{DateTime.Now:yyyyMMdd_HHmm}.zip";
            var zipTemp = Path.Combine(Path.GetTempPath(), nomeZip);

            using (var zip = ZipFile.Open(zipTemp, ZipArchiveMode.Create))
            {
                foreach (var arquivo in arquivosJson)
                {
                    zip.CreateEntryFromFile(
                        arquivo,
                        Path.GetFileName(arquivo),
                        CompressionLevel.Optimal
                    );
                }
            }

            var bytes = System.IO.File.ReadAllBytes(zipTemp);
            System.IO.File.Delete(zipTemp);

            return File(bytes, "application/zip", nomeZip);
        }

        // =========================
        // IMPORTAR – SOMENTE JSON
        // =========================
        [HttpPost("ImportarTudo")]
        public IActionResult ImportarTudo(IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Nenhum arquivo enviado.");

            var userPath = GetUserDataPath();
            var tempZip = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.zip");

            using (var stream = new FileStream(tempZip, FileMode.Create))
            {
                arquivo.CopyTo(stream);
            }

            using (var zip = ZipFile.OpenRead(tempZip))
            {
                foreach (var entry in zip.Entries)
                {
                    // 🔒 BLOQUEIO TOTAL: só aceita JSON
                    if (!entry.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var destino = Path.Combine(userPath, entry.Name);
                    entry.ExtractToFile(destino, overwrite: true);
                }
            }

            System.IO.File.Delete(tempZip);

            return Ok("Backup restaurado com sucesso (somente arquivos JSON).");
        }

        // =========================
        // LISTAR – SOMENTE JSON
        // =========================
        [HttpGet("ListarArquivos")]
        public IActionResult ListarArquivos()
        {
            var userPath = GetUserDataPath();

            var arquivosJson = Directory
                .GetFiles(userPath, "*.json")
                .Select(Path.GetFileName)
                .ToList();

            return Ok(arquivosJson);
        }
    }
}
