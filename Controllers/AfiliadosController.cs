using Kanban.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;


namespace Kanban.Controllers
{
    public class AfiliadosController : Controller
    {
        // Lista de produtos afiliados
        public ActionResult Produtos()
        {
            var produtos = new List<AfiliadoProduto>
            {
                new AfiliadoProduto
                {
                    Id = 1,
                    Nome = "Fone de Ouvido Sem Fio",
                    Preco = 199.90m,
                    ImagemUrl = "/imagens/fone.jpg",
                    LinkAfiliado = "https://mercadolivre.com.br/fone?affiliate_id=SEU_ID"
                },
                new AfiliadoProduto
                {
                    Id = 2,
                    Nome = "Smartphone XYZ",
                    Preco = 1499.00m,
                    ImagemUrl = "/imagens/smartphone.jpg",
                    LinkAfiliado = "https://mercadolivre.com.br/smartphone?affiliate_id=SEU_ID"
                }
            };

            return View(produtos);
        }

        // Gerador de links
        public ActionResult Links()
        {
            ViewBag.LinkGerado = "https://mercadolivre.com.br/produto?affiliate_id=SEU_ID";
            return View();
        }

        // Campanhas promocionais
        public ActionResult Campanhas()
        {
            ViewBag.Campanhas = new List<string>
            {
                "Campanha de Carnaval - até 20% de comissão",
                "Campanha de Eletrônicos - bônus especial"
            };

            return View();
        }

        // Relatórios de desempenho
        public ActionResult Relatorios()
        {
            var relatorios = new List<RelatorioAfiliado>
            {
                new RelatorioAfiliado
                {
                    Data = DateTime.Now.AddDays(-1),
                    Produto = "Fone de Ouvido Sem Fio",
                    Cliques = 120,
                    Vendas = 8,
                    Comissao = 150.75m
                },
                new RelatorioAfiliado
                {
                    Data = DateTime.Now.AddDays(-2),
                    Produto = "Smartphone XYZ",
                    Cliques = 200,
                    Vendas = 15,
                    Comissao = 1200.00m
                }
            };

            return View(relatorios);
        }

        // Configurações
        public ActionResult Configuracoes()
        {
            ViewBag.AfiliateId = "SEU_ID";
            return View();
        }
    }
}
