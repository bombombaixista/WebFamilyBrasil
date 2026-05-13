using Microsoft.AspNetCore.Mvc;
using WebFamilyBrasil.Models;
using System.Collections.Generic;

namespace WebFamilyBrasil.Controllers
{
    public class SafraController : Controller
    {
        // Lista simulada de safras
        private static List<Safra> _safras = new();

        // GET: /Safra/Index
        public IActionResult Index()
        {
            return View(_safras);
        }

        // GET: /Safra/Criar
        public IActionResult Criar()
        {
            return View();
        }

        // POST: /Safra/Criar
        [HttpPost]
        public IActionResult Criar(Safra safra)
        {
            if (ModelState.IsValid)
            {
                _safras.Add(safra);
                return RedirectToAction("Index");
            }
            return View(safra);
        }
    }
}
