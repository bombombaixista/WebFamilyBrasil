# Checklist de Implementação: Relatório DRE

Este checklist quebra o plano de implementação em pequenas atividades acionáveis.

## 🟢 Fase 1: Preparação de Dados e Lógica de Negócio
- [ ] **1.1 Validar Estrutura de Dados:**
    - [ ] Abrir `Data/financeiro.json`.
    - [ ] Verificar se existem lançamentos com as categorias: `Vendas`, `Deducoes`, `CMV`, `DespesasVendas`, `DespesasAdm`, `DespesasFinanceiras`, `ReceitasFinanceiras`.
    - [ ] Se não existirem, criar 10-15 lançamentos de exemplo para teste.
- [ ] **1.2 Refinar DreService:**
    - [ ] Alterar o método `GerarDre` para aceitar `mes` e `ano`.
    - [ ] Implementar o filtro por data dentro do `GerarDre` antes dos cálculos.
    - [ ] Garantir que o cálculo de `Impostos` (atualmente 20% fixo) seja facilmente ajustável.

## 🔵 Fase 2: Infraestrutura do Controlador
- [ ] **2.1 Criar Action no RelatorioController:**
    - [ ] Adicionar `[HttpGet("Dre")]` que recebe `int mes` e `int ano`.
    - [ ] Implementar a leitura do arquivo `financeiro.json` e conversão para `List<Financeiro>`.
    - [ ] Chamar `_dreService.GerarDre(lancamentos, mes, ano)`.
- [ ] **2.2 Injeção de Dependência:**
    - [ ] Adicionar `DreService` ao construtor do `RelatorioController`.
    - [ ] Registrar o `DreService` no `Program.cs`.

## 🟡 Fase 3: Layout do Relatório (QuestPDF)
- [ ] **3.1 Estrutura Básica do PDF:**
    - [ ] Criar método privado `GerarPdfDre(DreRelatorio model, int mes, int ano)`.
    - [ ] Definir cabeçalho com nome da empresa e período selecionado.
- [ ] **3.2 Implementar Linhas do DRE:**
    - [ ] Criar estilo para linhas de "Título" (ex: Receita Bruta) e "Subtotal" (ex: Lucro Bruto).
    - [ ] Implementar a seção de **Receitas e Deduções**.
    - [ ] Implementar a seção de **Custos (CMV)**.
    - [ ] Implementar a seção de **Despesas Operacionais**.
    - [ ] Implementar a seção de **Impostos e Resultado Líquido**.
- [ ] **3.3 Estilização Visual:**
    - [ ] Adicionar cores (verde para positivo, vermelho para negativo).
    - [ ] Adicionar bordas e espaçamentos para melhorar a legibilidade.

## 🟠 Fase 4: Interface e Integração
- [ ] **4.1 Refinar View de Seleção Existente:**
    - [ ] Localizar `Views/Financeiro/RelatorioDRE.cshtml`.
    - [ ] Adicionar formulário de filtro por Mês e Ano.
    - [ ] Incluir botão "Exportar para PDF" que aponta para `RelatorioController.Dre`.
- [ ] **4.2 Link de Acesso:**
    - [ ] Garantir que o link para o relatório DRE no menu lateral aponte para `FinanceiroController.RelatorioDRE`.

## 🔴 Fase 5: Validação Final
- [ ] **5.1 Teste de Cálculos:**
    - [ ] Bater os valores gerados no PDF com uma planilha Excel de referência.
- [ ] **5.2 Teste de Layout:**
    - [ ] Verificar se o PDF abre corretamente em diferentes leitores (Navegador, Adobe, Mobile).
- [ ] **5.3 Teste de Filtros:**
    - [ ] Gerar relatórios para meses diferentes e validar se os dados mudam conforme esperado.
