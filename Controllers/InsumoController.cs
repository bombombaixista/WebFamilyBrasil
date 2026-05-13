using Microsoft.AspNetCore.Mvc;
using WebFamilyBrasil.Models;

namespace WebFamilyBrasil.Controllers
{
    public class InsumoController : Controller
    {
        private static List<Insumo> _insumos = new();

        public IActionResult Index()
        {
            return View(_insumos);
        }

        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Criar(Insumo insumo)
        {
            insumo.Id = _insumos.Count + 1;
            _insumos.Add(insumo);
            return RedirectToAction("Index");
        }

        public IActionResult Editar(int id)
        {
            var insumo = _insumos.FirstOrDefault(i => i.Id == id);
            return View(insumo);
        }

        [HttpPost]
        public IActionResult Editar(Insumo insumo)
        {
            var existente = _insumos.FirstOrDefault(i => i.Id == insumo.Id);
            if (existente != null)
            {
                existente.Nome = insumo.Nome;
                existente.Tipo = insumo.Tipo;
                existente.Quantidade = insumo.Quantidade;
                existente.CustoUnitario = insumo.CustoUnitario;
            }
            return RedirectToAction("Index");
        }

        public IActionResult Excluir(int id)
        {
            var insumo = _insumos.FirstOrDefault(i => i.Id == id);
            if (insumo != null) _insumos.Remove(insumo);
            return RedirectToAction("Index");
        }
    }
}
