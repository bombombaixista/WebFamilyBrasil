using Kanban.Models;
using System.Linq;

namespace Kanban.Services
{
    public class DreService
    {
        public DreRelatorio GerarDre(List<Financeiro> registros)
        {
            var receitaBruta = registros
                .Where(r => r.Tipo == TipoLancamento.Receita && r.Categoria == CategoriaLancamento.Vendas)
                .Sum(r => r.Valor);

            var deducoes = registros
                .Where(r => r.Tipo == TipoLancamento.Despesa && r.Categoria == CategoriaLancamento.Deducoes)
                .Sum(r => r.Valor);

            var receitaLiquida = receitaBruta - deducoes;

            var cmv = registros
                .Where(r => r.Tipo == TipoLancamento.Despesa && r.Categoria == CategoriaLancamento.CMV)
                .Sum(r => r.Valor);

            var lucroBruto = receitaLiquida - cmv;

            var despesasVendas = registros
                .Where(r => r.Tipo == TipoLancamento.Despesa && r.Categoria == CategoriaLancamento.DespesasVendas)
                .Sum(r => r.Valor);

            var despesasAdm = registros
                .Where(r => r.Tipo == TipoLancamento.Despesa && r.Categoria == CategoriaLancamento.DespesasAdm)
                .Sum(r => r.Valor);

            var despesasFinanceiras = registros
                .Where(r => r.Tipo == TipoLancamento.Despesa && r.Categoria == CategoriaLancamento.DespesasFinanceiras)
                .Sum(r => r.Valor);

            var receitasFinanceiras = registros
                .Where(r => r.Tipo == TipoLancamento.Receita && r.Categoria == CategoriaLancamento.ReceitasFinanceiras)
                .Sum(r => r.Valor);

            var resultadoOperacional = lucroBruto - (despesasVendas + despesasAdm + despesasFinanceiras) + receitasFinanceiras;

            var outrasReceitas = registros
                .Where(r => r.Tipo == TipoLancamento.Receita && r.Categoria == CategoriaLancamento.OutrasReceitas)
                .Sum(r => r.Valor);

            var outrasDespesas = registros
                .Where(r => r.Tipo == TipoLancamento.Despesa && r.Categoria == CategoriaLancamento.OutrasDespesas)
                .Sum(r => r.Valor);

            var lair = resultadoOperacional + outrasReceitas - outrasDespesas;

            var impostos = lair * 0.20m;
            var lucroLiquido = lair - impostos;

            return new DreRelatorio
            {
                ReceitaBruta = receitaBruta,
                Deducoes = deducoes,
                ReceitaLiquida = receitaLiquida,
                CMV = cmv,
                LucroBruto = lucroBruto,
                DespesasVendas = despesasVendas,
                DespesasAdm = despesasAdm,
                DespesasFinanceiras = despesasFinanceiras,
                ReceitasFinanceiras = receitasFinanceiras,
                ResultadoOperacional = resultadoOperacional,
                OutrasReceitas = outrasReceitas,
                OutrasDespesas = outrasDespesas,
                LAIR = lair,
                Impostos = impostos,
                LucroLiquido = lucroLiquido
            };
        }
    }
}
