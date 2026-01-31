using Microsoft.AspNetCore.Mvc;
using Kanban.Models;
using System.Text.Json;

namespace Kanban.Controllers
{
    public class UserController : Controller
    {
        private readonly string _basePath =
            Path.Combine(Directory.GetCurrentDirectory(), "Data", "usuarios");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Dados inválidos";
                return View();
            }

            // cria pasta do usuário
            string userFolder = Path.Combine(_basePath, Sanitize(user.Email));
            Directory.CreateDirectory(userFolder);

            // salva dados do usuário em arquivo
            string userFile = Path.Combine(userFolder, "user.json");
            string json = JsonSerializer.Serialize(user, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            System.IO.File.WriteAllText(userFile, json);

            TempData["Mensagem"] = "Usuário cadastrado com sucesso!";
            return RedirectToAction("Index", "Login");
        }

        private string Sanitize(string email)
        {
            return email.Replace("@", "_").Replace(".", "_");
        }
    }
}
