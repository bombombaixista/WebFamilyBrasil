# Plano de Implementação: Relatório DRE (Demonstrativo do Resultado do Exercício)

Este documento descreve as etapas necessárias para implementar o relatório DRE no sistema Kanban.

## 1. Visão Geral
O DRE é um relatório contábil que detalha a performance financeira da empresa em um determinado período, calculando o lucro ou prejuízo líquido através da subtração de despesas e impostos da receita bruta.

## 2. Componentes Atuais
- **Modelo:** `Models/DreRelatorio.cs` define a estrutura de dados do relatório.
- **Enums:** `Models/Financeiro.cs` contém `TipoLancamento` e `CategoriaLancamento`.
- **Serviço:** `Services/DreService.cs` contém a lógica de cálculo (já implementada).
- **Controlador:** `Controllers/RelatorioController.cs` gerencia a geração de PDFs.

## 3. Etapas de Implementação

### Passo 1: Ajuste de Dados e Fontes
- Verificar se os lançamentos financeiros no arquivo `Data/financeiro.json` (ou banco de dados) estão utilizando as categorias corretas definidas no enum `CategoriaLancamento`.
- Garantir que o `DreService` possa filtrar por período (mês/ano).

### Passo 2: Implementação no Controlador
- Adicionar uma nova Action `Dre()` no `RelatorioController.cs`.
- Esta Action deve:
    1. Receber parâmetros de filtro (mês e ano).
    2. Carregar os dados financeiros necessários.
    3. Chamar o `DreService.GerarDre(dados)`.
    4. Utilizar a biblioteca **QuestPDF** para formatar o relatório em PDF.

### Passo 3: Design do PDF (QuestPDF)
O layout do PDF deve seguir a estrutura padrão de um DRE:
- **(+) Receita Bruta**
- **(-) Deduções e Impostos sobre Vendas**
- **(=) Receita Líquida**
- **(-) CMV (Custo de Mercadoria Vendida)**
- **(=) Lucro Bruto**
- **(-) Despesas Operacionais (Vendas, Adm, Financeiras)**
- **(+) Receitas Financeiras**
- **(=) Resultado Operacional**
- **(+/-) Outras Receitas/Despesas**
- **(=) LAIR (Lucro Antes do Imposto de Renda)**
- **(-) Impostos (IRPJ/CSLL)**
- **(=) Lucro Líquido**

### Passo 4: Interface do Usuário (View)
- Adicionar um formulário de filtro na View de Relatórios (ou Dashboard).
- Botão "Gerar DRE (PDF)" que aponta para a nova Action do controlador.

### Passo 5: Validação e Testes
- Criar um conjunto de dados de teste que cubra todas as categorias do DRE.
- Validar manualmente os cálculos do `DreService`.
- Verificar a formatação e legibilidade do PDF gerado.

## 4. Tecnologias Utilizadas
- **ASP.NET Core MVC**
- **C# / LINQ**
- **QuestPDF** (para geração de documentos)
- **JSON / Entity Framework** (para persistência de dados)

## 5. Cronograma Estimado
- **Passo 1 e 2:** 2 horas
- **Passo 3:** 3 horas (ajuste fino de layout)
- **Passo 4 e 5:** 2 horas
- **Total:** ~7 horas de desenvolvimento.
