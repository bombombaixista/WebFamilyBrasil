using Kanban.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Kanban.Controllers
{
    public class DocumentoController : Controller
    {
        // Simulação de dados em memória (substitua por DbContext depois)
        private static List<Documento> _documentos = new List<Documento>
        {
            new Documento { Id = 1, Nome = "Cliente A", Categoria = "Contrato", Caminho = "/docs/contratoA.pdf", DataUpload = DateTime.Now.AddDays(-5) },
            new Documento { Id = 2, Nome = "Cliente B", Categoria = "Nota Fiscal", Caminho = "/docs/nfB.xml", DataUpload = DateTime.Now.AddDays(-2) },
            new Documento { Id = 3, Nome = "Cliente A", Categoria = "Relatório", Caminho = "/docs/relatorioA.docx", DataUpload = DateTime.Now.AddDays(-1) }
        };

        // READ: Lista documentos agrupados
        public IActionResult Index()
        {
            ViewBag.TotalClientes = _documentos.Select(d => d.Nome).Distinct().Count();
            ViewBag.TotalDocumentos = _documentos.Count;

            // Clientes e documentos por cliente para os gráficos
            var grouped = _documentos
                .GroupBy(d => d.Nome)
                .Select(g => new { Cliente = g.Key, Quantidade = g.Count() })
                .ToList();

            ViewBag.Clientes = grouped.Select(g => g.Cliente).ToList(); // List<string>
            ViewBag.DocumentosPorCliente = grouped.Select(g => g.Quantidade).ToList(); // List<int>

            return View(_documentos);
        }

        // CREATE: Upload de novo documento
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(Documento doc)
        {
            if (ModelState.IsValid)
            {
                doc.Id = _documentos.Max(d => d.Id) + 1;
                doc.DataUpload = DateTime.Now;
                _documentos.Add(doc);
                return RedirectToAction("Index");
            }
            return View(doc);
        }

        // UPDATE: Editar documento
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var doc = _documentos.FirstOrDefault(d => d.Id == id);
            if (doc == null) return NotFound();
            return View(doc);
        }

        [HttpPost]
        public IActionResult Edit(Documento doc)
        {
            var existing = _documentos.FirstOrDefault(d => d.Id == doc.Id);
            if (existing == null) return NotFound();

            existing.Nome = doc.Nome;
            existing.Categoria = doc.Categoria;
            existing.Caminho = doc.Caminho;
            existing.DataUpload = DateTime.Now;

            return RedirectToAction("Index");
        }

        // DELETE: Excluir documento
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var doc = _documentos.FirstOrDefault(d => d.Id == id);
            if (doc == null) return NotFound();
            return View(doc);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var doc = _documentos.FirstOrDefault(d => d.Id == id);
            if (doc == null) return NotFound();
            _documentos.Remove(doc);
            return RedirectToAction("Index");
        }
    }
}
