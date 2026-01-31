using Kanban.Models;
using Kanban.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Kanban.Controllers
{
    public class CadastroController : Controller
    {
        private readonly UserService _userService;
        private readonly IWebHostEnvironment _env;

        public CadastroController(UserService userService, IWebHostEnvironment env)
        {
            _userService = userService;
            _env = env;
        }

        // =========================
        // FORM DE CADASTRO
        // =========================
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // =========================
        // CADASTRAR NOVO USUÁRIO
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string nome, string email, string senha, string plano)
        {
            var user = new User
            {
                Nome = nome,
                Email = email,
                Plano = plano,
                DataCadastro = DateTime.Now,
                DataExpiracao = DateTime.Now.AddMonths(1)
            };

            // cria usuário com senha hash automaticamente
            _userService.CriarUsuario(user, senha);

            // 🔹 Cria pasta inicial do usuário e arquivos JSON vazios
            CriarEstruturaInicial(email);

            // autentica o usuário imediatamente após cadastro
            await SignInUser(user);

            return RedirectToAction("Index", "Home");
        }

        // =========================
        // LOGIN AUTOMÁTICO APÓS CADASTRO
        // =========================
        private async Task SignInUser(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Nome),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("Plano", user.Plano)
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                "CookieAuth",
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(2)
                }
            );
        }

        // =========================
        // CRIA PASTA E ARQUIVOS INICIAIS DO USUÁRIO
        // =========================
        private void CriarEstruturaInicial(string email)
        {
            // 🔹 Normaliza o email igual aos outros controllers
            var folderName = email.Replace("@", "_").Replace(".", "_");

            // 🔹 Usa ContentRootPath para manter consistência
            var basePath = Path.Combine(_env.ContentRootPath, "Data", "User2", folderName);
            Directory.CreateDirectory(basePath);

            var arquivosIniciais = new[] { "tarefas.json", "pipeline.json", "agenda.json", "documento.json" };
            foreach (var arquivo in arquivosIniciais)
            {
                var caminho = Path.Combine(basePath, arquivo);
                if (!System.IO.File.Exists(caminho))
                {
                    System.IO.File.WriteAllText(caminho, "[]");
                }
            }
        }
    }
}
