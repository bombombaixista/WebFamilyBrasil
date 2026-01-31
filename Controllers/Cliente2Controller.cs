using Microsoft.AspNetCore.Mvc;
using Kanban.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO; // ✅ para manipular arquivos e pastas

public class Cliente2Controller : Controller
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<Cliente2> _passwordHasher;

    public Cliente2Controller(AppDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<Cliente2>();
    }

    // GET: Cliente2/Index → lista de clientes
    public IActionResult Index()
    {
        var clientes = _context.Clientes2.ToList();
        return View(clientes);
    }

    // GET: Cliente2/Login → tela de login
    public IActionResult Login()
    {
        return View();
    }

    // POST: Cliente2/Login → valida login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string senha)
    {
        var cliente = _context.Clientes2.FirstOrDefault(c => c.Email == email);

        if (cliente == null)
        {
            ModelState.AddModelError("", "Usuário não encontrado.");
            return View();
        }

        var result = _passwordHasher.VerifyHashedPassword(cliente, cliente.SenhaHash, senha);

        if (result == PasswordVerificationResult.Success)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, cliente.Nome),
                new Claim(ClaimTypes.Email, cliente.Email),
                new Claim("Plano", cliente.PlanoId.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home"); // ✅ entra no programa Kanban
        }

        ModelState.AddModelError("", "Senha inválida.");
        return View();
    }

    // GET: Cliente2/Cadastrar → tela de cadastro
    public IActionResult Cadastrar()
    {
        ViewBag.Planos = _context.Planos
            .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Nome} - R$ {p.PrecoMensal}"
            }).ToList();

        return View();
    }

    // POST: Cliente2/Cadastrar → cadastro
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cadastrar(Cliente2 cliente, string senha)
    {
        if (ModelState.IsValid)
        {
            cliente.Id = Guid.NewGuid();
            cliente.DataCadastro = DateTime.UtcNow;
            cliente.Ativo = true;
            cliente.SenhaHash = _passwordHasher.HashPassword(cliente, senha);

            _context.Clientes2.Add(cliente);
            _context.SaveChanges();

            // ✅ Cria pasta e arquivos JSON para o cliente
            CriarEstruturaArquivos(cliente.Email);

            return RedirectToAction("Login");
        }

        ViewBag.Planos = _context.Planos
            .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Nome} - R$ {p.PrecoMensal}"
            }).ToList();

        return View(cliente);
    }

    // Método auxiliar para criar pasta e arquivos JSON
    private void CriarEstruturaArquivos(string email)
    {
        // Normaliza email para nome de pasta (evita caracteres inválidos)
        var pastaCliente = Path.Combine("Data", "User2", email.Replace("@", "_at_").Replace(".", "_"));

        if (!Directory.Exists(pastaCliente))
        {
            Directory.CreateDirectory(pastaCliente);
        }

        // Lista de arquivos JSON
        string[] arquivos = {
            "agenda.json",
            "clientes.json",
            "documentos.json",
            "financeiro.json",
            "fornecedores.json",
            "funcionarios.json",
            "pedidos.json",
            "pipeline.json",
            "produtos.json",
            "tarefas.json",
            "transacoes.json"
        };

        foreach (var arquivo in arquivos)
        {
            var caminho = Path.Combine(pastaCliente, arquivo);
            if (!System.IO.File.Exists(caminho))
            {
                System.IO.File.WriteAllText(caminho, "{}"); // cria JSON vazio
            }
        }
    }

    // GET: Cliente2/Logout
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
