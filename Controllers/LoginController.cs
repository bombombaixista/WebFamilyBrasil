using Microsoft.AspNetCore.Mvc;
using Kanban.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Linq;

public class LoginController : Controller
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<Cliente2> _passwordHasher;

    public LoginController(AppDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<Cliente2>();
    }

    // GET: Login
    [HttpGet]
    public IActionResult Index()
    {
        return View(); // Views/Login/Index.cshtml
    }

    // POST: Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string senha)
    {
        var cliente = _context.Clientes2.FirstOrDefault(c => c.Email == email);

        if (cliente == null)
        {
            ModelState.AddModelError("", "Usuário não encontrado.");
            return View("Index");
        }

        var result = _passwordHasher.VerifyHashedPassword(cliente, cliente.SenhaHash, senha);

        if (result == PasswordVerificationResult.Success)
        {
            // Cria claims do usuário
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, cliente.Nome),
                new Claim(ClaimTypes.Email, cliente.Email),
                new Claim("Plano", cliente.PlanoId.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Grava cookie de login
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }
        else
        {
            ModelState.AddModelError("", "Senha inválida.");
            return View("Index");
        }
    }

    // GET: Logout
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        // Agora redireciona para a tela de login
        return RedirectToAction("Index", "Login");
    }
}
