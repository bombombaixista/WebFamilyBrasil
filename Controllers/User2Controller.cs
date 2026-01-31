using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Kanban.Model;
using System;
using System.Threading.Tasks;
using Kanban.Models;

namespace WebFamily.Controllers
{
    public class User2Controller : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public User2Controller(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /User2/Register
        public IActionResult Register()
        {
            ViewBag.PlanoId = TempData["PlanoId"];
            ViewBag.Expiracao = TempData["Expiracao"];
            return View();
        }

        // POST: /User2/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string Nome, string Email, string Senha, string ConfirmarSenha, Guid PlanoId, string Expiracao)
        {
            if (Senha != ConfirmarSenha)
            {
                ModelState.AddModelError("ConfirmarSenha", "As senhas não conferem.");
                return View();
            }

            try
            {
                var cliente = new Cliente2
                {
                    Id = Guid.NewGuid(),
                    Nome = Nome,
                    Email = Email,
                    PlanoId = PlanoId,
                    DataCadastro = DateTime.UtcNow,
                    Ativo = true
                };

                var hasher = new PasswordHasher<Cliente2>();
                cliente.SenhaHash = hasher.HashPassword(cliente, Senha);

                _context.Clientes2.Add(cliente);
                _context.SaveChanges();

                // 🔹 Cria pasta e arquivos iniciais do usuário
                CriarEstruturaInicial(Email);

                // 🔑 autentica automaticamente
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, cliente.Id.ToString()),
                    new Claim(ClaimTypes.Name, cliente.Nome),
                    new Claim(ClaimTypes.Email, cliente.Email),
                    new Claim("Plano", PlanoId.ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Erro ao cadastrar: " + ex.Message);
                return View();
            }
        }

        private void CriarEstruturaInicial(string email)
        {
            var folderName = email.Replace("@", "_").Replace(".", "_");
            var basePath = Path.Combine(_env.ContentRootPath, "Data", "User2", folderName);
            Directory.CreateDirectory(basePath);

            var arquivosIniciais = new[] { "tarefas.json", "pipeline.json", "agenda.json", "documento.json", "financeiro.json", "funcionarios.json", "fornecedores.json", "produtos.json", "movimentacoes.json", "pedidos.json", "transacoes.json" };
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
