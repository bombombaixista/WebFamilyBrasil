using Kanban.Model;
using Kanban.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace WebFamilyERP.Controllers;

[Authorize]
public class KanbanController : Controller
{
    private readonly IWebHostEnvironment _env;

    public KanbanController(IWebHostEnvironment env)
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

    private JsonDatabase<Transacao> GetDb()
    {
        var path = Path.Combine(GetUserDataPath(), "transacoes.json");
        return new JsonDatabase<Transacao>(path);
    }

    // =========================
    // Ações
    // =========================
    public IActionResult Index()
    {
        var transacoes = GetDb().GetAll().OrderByDescending(t => t.Data).ToList();
        return View(transacoes);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Create(Transacao transacao)
    {
        var db = GetDb();
        transacao.Id = db.GetAll().Count > 0 ? db.GetAll().Max(t => t.Id) + 1 : 1;
        db.Add(transacao);
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        var db = GetDb();
        var transacao = db.GetAll().FirstOrDefault(t => t.Id == id);
        if (transacao == null) return NotFound();
        return View(transacao);
    }

    [HttpPost]
    public IActionResult Edit(Transacao transacao)
    {
        var db = GetDb();
        var transacoes = db.GetAll();
        var index = transacoes.FindIndex(t => t.Id == transacao.Id);
        if (index == -1) return NotFound();
        transacoes[index] = transacao;
        db.Update(transacoes);
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        var db = GetDb();
        var transacoes = db.GetAll();
        var transacao = transacoes.FirstOrDefault(t => t.Id == id);
        if (transacao != null)
        {
            transacoes.Remove(transacao);
            db.Update(transacoes);
        }
        return RedirectToAction("Index");
    }
}
