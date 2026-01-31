using Kanban.Services;
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
        private readonly UserService _userService;

        public BackupController(IWebHostEnvironment env, UserService userService)
        {
            _baseDataPath = Path.Combine(env.ContentRootPath, "Data");
            Directory.CreateDirectory(_baseDataPath);
            _userService = userService;
        }

        // =========================
        // Helpers
        // =========================
        private string GetUserDataPath()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                throw new Exception("Usuário não logado.");

            // 🔹 Normaliza o email para nome de pasta
            var folderName = email.Replace("@", "_").Replace(".", "_");

            // 🔹 Estrutura correta: Kanban/Data/User2/<email>
            var userPath = Path.Combine(_baseDataPath, "User2", folderName);

            if (!Directory.Exists(userPath))
                Directory.CreateDirectory(userPath);

            return userPath;
        }

        // =========================
        // Página principal
        // =========================
        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            var userPath = GetUserDataPath();
            var arquivos = Directory.GetFiles(userPath)
                .Select(Path.GetFileName)
                .Where(f => f != null)
                .ToList();

            return View(arquivos); // precisa criar Views/Backup/Index.cshtml
        }

        // =========================
        // Exportar tudo (ZIP)
        // =========================
        [HttpGet("ExportarTudo")]
        public IActionResult ExportarTudo()
        {
            var userPath = GetUserDataPath();
            var arquivos = Directory.GetFiles(userPath);

            if (arquivos.Length == 0)
                return BadRequest($"Nenhum arquivo encontrado em {userPath}");

            var nomeArquivo = $"backup_{DateTime.Now:yyyyMMdd_HHmm}.zip";
            var zipPath = Path.Combine(Path.GetTempPath(), nomeArquivo);

            ZipFile.CreateFromDirectory(userPath, zipPath);
            var bytes = System.IO.File.ReadAllBytes(zipPath);
            System.IO.File.Delete(zipPath);

            return File(bytes, "application/zip", nomeArquivo);
        }

        // =========================
        // Importar tudo (ZIP)
        // =========================
        [HttpPost("ImportarTudo")]
        public IActionResult ImportarTudo(IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Nenhum arquivo enviado.");

            var userPath = GetUserDataPath();
            var tempZip = Path.Combine(Path.GetTempPath(), $"import_{Guid.NewGuid()}.zip");

            using (var stream = new FileStream(tempZip, FileMode.Create))
            {
                arquivo.CopyTo(stream);
            }

            ZipFile.ExtractToDirectory(tempZip, userPath, overwriteFiles: true);
            System.IO.File.Delete(tempZip);

            return Ok(new { message = "Backup restaurado com sucesso!" });
        }

        // =========================
        // Listar arquivos (JSON)
        // =========================
        [HttpGet("ListarArquivos")]
        public IActionResult ListarArquivos()
        {
            var userPath = GetUserDataPath();
            var arquivos = Directory.GetFiles(userPath)
                .Select(Path.GetFileName)
                .Where(f => f != null)
                .ToList();

            return Ok(arquivos);
        }
    }
}
