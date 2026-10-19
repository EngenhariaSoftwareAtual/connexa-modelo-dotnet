# Connexa — modelo de repositório (.NET + React)

Repositório-modelo para as equipes que escolheram **C# e .NET** no back-end do Connexa. A partir dele, cada equipe cria, na organização do GitHub da própria equipe, o seu `<repositorio-da-equipe>` — com nome escolhido pela equipe — e já recebe a infraestrutura pronta: arquitetura modular, banco de dados, testes, CI, ambiente no GitHub Codespaces e a conexão do agente de IA com o Azure Boards.

> **O que este modelo não traz:** nenhuma funcionalidade do Connexa. Os módulos estão vazios e são preenchidos sprint a sprint, a partir das Tasks do Azure Boards. As duas fatias incluídas (situação do sistema e estudante simulado) são exemplos de estrutura e apoio ao desenvolvimento.

| Parte | Tecnologia |
|---|---|
| Back-end | .NET 10 (LTS) · C# · ASP.NET Core Minimal APIs · um projeto por módulo · EF Core + Npgsql · OpenAPI nativo do ASP.NET Core + Swagger UI |
| Banco de dados | PostgreSQL 17, em contêiner |
| Front-end | React 19 · TypeScript · Vite · React Router · TanStack Query · React Hook Form + Zod |
| Testes | MSTest + `WebApplicationFactory` (aceitação na API) · NetArchTest (fronteiras) · Vitest + Testing Library |
| Ambiente | GitHub Codespaces (Dev Container) ou máquina local com Docker |
| Integração contínua | GitHub Actions |

---

## Sumário

1. [Estrutura do repositório](#1-estrutura-do-repositório)
2. [Começar a trabalhar](#2-começar-a-trabalhar)
3. [Comandos do dia a dia](#3-comandos-do-dia-a-dia)
4. [Arquitetura](#4-arquitetura)
5. [Banco de dados](#5-banco-de-dados)
6. [Estudante simulado](#6-estudante-simulado)
7. [Testes e verificação](#7-testes-e-verificação)
8. [Agente de IA e Azure Boards](#8-agente-de-ia-e-azure-boards)
9. [Configuração depois de criar o repositório da equipe](#9-configuração-depois-de-criar-o-repositório-da-equipe)
10. [Decisões deste modelo](#10-decisões-deste-modelo)
11. [Solução de problemas](#11-solução-de-problemas)

---

## 1. Estrutura do repositório

```
<repositorio-da-equipe>/
├── AGENTS.md                      instruções para agentes de IA (Codex e outros)
├── .github/
│   ├── copilot-instructions.md    cópia idêntica do AGENTS.md (GitHub Copilot)
│   ├── pull_request_template.md   evidências exigidas em todo pull request
│   └── workflows/ci.yml           CI: back-end, front-end e instruções sincronizadas
├── .devcontainer/                 ambiente do GitHub Codespaces (.NET, Node e PostgreSQL)
├── .vscode/mcp.json               conexão do GitHub Copilot com o Azure Boards (MCP remoto)
├── .codex/config.toml             conexão do Codex com o Azure Boards (MCP local)
├── docker-compose.yml             PostgreSQL para desenvolvimento e testes
├── verificar · verificar.ps1      rodam localmente tudo o que o CI roda
├── backend/                       API — monolito modular em ASP.NET Core
│   ├── Connexa.slnx               solução com todos os projetos
│   ├── Directory.Packages.props   versões de todos os pacotes NuGet
│   ├── MIGRACOES.md               como criar tabelas com o EF Core
│   ├── src/Connexa.Api/           ponto de entrada: monta os módulos
│   ├── src/Connexa.<Modulo>/      um projeto por módulo
│   └── tests/Connexa.Testes/      aceitação, fronteiras e unidade
├── frontend/                      SPA em React, organizada por features
├── docs/
│   ├── adr/                       registros de decisão de arquitetura
│   └── exemplo/                   cenários Gherkin das fatias de exemplo
└── infra/postgres/init/           cria o banco de teste na primeira inicialização
```

---

## 2. Começar a trabalhar

### Opção A — GitHub Codespaces (recomendada)

Nada precisa ser instalado na máquina: .NET 10, Node.js 24 e PostgreSQL já vêm no ambiente.

1. No repositório da equipe, clique em **Code › Codespaces › Create codespace on main**.
2. Aguarde a criação. Na primeira vez, o script `.devcontainer/pos-criacao.sh` baixa as dependências, o que leva alguns minutos.
3. No terminal do Codespace, rode `./verificar`. Tudo deve terminar com **"Verificação concluída sem falhas."**
4. Suba a aplicação em dois terminais:
   ```bash
   cd backend && dotnet run --project src/Connexa.Api
   ```
   ```bash
   cd frontend && npm run dev
   ```
5. A aba **Ports** do VS Code mostra os endereços. A interface (porta 5173) abre no navegador.

> **Cota do Codespaces.** Contas com o GitHub Student Developer Pack têm uma cota mensal de horas de uso. Pare o Codespace quando não estiver usando (**Codespaces › Stop current codespace**); ele também para sozinho depois de um período sem atividade.

### Opção B — máquina local (Windows, Linux ou macOS)

**Pré-requisitos:** .NET SDK 10, Node.js 24, Docker Desktop (ou Docker Engine) e VS Code com as extensões recomendadas (o VS Code oferece instalá-las ao abrir a pasta).

```powershell
docker compose up -d db          # sobe o PostgreSQL
.\verificar.ps1                  # Windows (no Linux/macOS: ./verificar)
cd backend; dotnet run --project src/Connexa.Api
cd frontend; npm install; npm run dev
```

### Endereços

| Endereço | O que é |
|---|---|
| <http://localhost:5173> | Interface (o Vite repassa `/api` para o back-end) |
| <http://localhost:8080/swagger> | Contrato da API, navegável |
| <http://localhost:8080/openapi/v1.json> | Contrato da API em JSON (OpenAPI) |
| `localhost:5432` | PostgreSQL — usuário `connexa`, senha `connexa`, bancos `connexa` e `connexa_teste` |

---

## 3. Comandos do dia a dia

| Ação | Comando |
|---|---|
| Verificar tudo (o mesmo que o CI) | `./verificar` · Windows: `.\verificar.ps1` |
| Só o back-end / só o front-end | `./verificar backend` · `./verificar frontend` |
| Subir a API | `cd backend && dotnet run --project src/Connexa.Api` |
| Subir a interface | `cd frontend && npm run dev` |
| Testes do front-end em modo contínuo | `cd frontend && npm run test:observar` |
| Criar uma migração | ver `backend/MIGRACOES.md` |
| Apagar os dados e recomeçar o banco | `docker compose down -v && docker compose up -d db` |

> **`./verificar` é o `<comando-de-verificacao>`** citado no roteiro da Atividade de Sprint e nas Tasks prontas para agente.

---

## 4. Arquitetura

**Monolito modular com fatias verticais + SPA**, conforme a proposta de arquitetura da disciplina. O ADR-001 de cada equipe registra a adoção ou a adaptação.

### Módulos do back-end

| Projeto | Território | Situação no modelo |
|---|---|---|
| `Connexa.Identidade` | Quem é o estudante atual | Estudante **simulado** (ver seção 6) |
| `Connexa.Perfil` | Perfil acadêmico | Vazio |
| `Connexa.Grupos` | Grupos de estudo e participação | Vazio |
| `Connexa.Encontros` | Encontros dos grupos | Vazio |
| `Connexa.Acompanhamento` | Atividade e inatividade dos grupos | Vazio |
| `Connexa.Painel` | Visões consolidadas | Vazio |
| `Connexa.Materiais` | Materiais compartilhados | Vazio |
| `Connexa.Comum` | Infraestrutura sem regra de negócio: erros, banco, validação, eventos, situação, OpenAPI | Pronto |

### Regras de fronteira

1. Cada projeto `Connexa.<Modulo>` é um módulo. O **namespace raiz** do módulo é a sua API pública; tudo em `Internal/` é `internal` e o compilador impede o uso por outros projetos.
2. Nenhum módulo usa tipos internos de outro, `Connexa.Comum` não depende de módulos de negócio e nenhum módulo depende de `Connexa.Api`. O teste `ArquiteturaModularTest` (NetArchTest) verifica isso **em todo build**. Dependência circular é impossível: o build não aceita referências em ciclo entre projetos.
3. Cada módulo é dono dos seus dados: um schema por módulo e nenhum acesso às tabelas de outro.
4. Reações entre módulos acontecem por **eventos** (`IDespachanteDeEventos` e `ITratadorDeEvento<T>`), não por chamadas que atravessam os dados de outro módulo.

No front-end vale a mesma ideia: cada pasta de `src/features/` só é usada pelo seu `index.ts`, e o lint bloqueia importações de arquivos internos de outra feature.

### O caminho de uma fatia vertical

```
Cenário Gherkin (Azure Boards)
   ↓
Teste de aceitação     backend/tests/Connexa.Testes/<Modulo>/<Historia>AceitacaoTest.cs
   ↓
Contexto e migração    backend/src/Connexa.<Modulo>/Internal/ e Internal/Migracoes/ (dotnet ef)
   ↓
Entidade, serviço e endpoints em Connexa.<Modulo>.Internal, mapeados no Modulo<Modulo>.cs
   ↓
Contrato visível em    /swagger
   ↓
Feature no front-end   frontend/src/features/<feature>/ (consultas.ts, componentes, testes)
```

As duas fatias de exemplo seguem exatamente esse caminho. Estude-as antes da primeira Task:

| Fatia | Cenários | Back-end | Front-end |
|---|---|---|---|
| Situação do sistema | `docs/exemplo/situacao-do-sistema.feature` | `Connexa.Comum/Internal/SituacaoEndpoints.cs` | `features/situacao/` |
| Sessão do estudante simulado | `docs/exemplo/sessao-simulada.feature` | `Connexa.Identidade/Internal/` | `features/sessao/` |

### Erros da API

Toda resposta de erro segue o formato *Problem Detail* (RFC 9457) e traz a propriedade `codigo`:

```json
{ "status": 422, "title": "Unprocessable Entity", "detail": "O grupo já atingiu o limite de participantes.", "codigo": "GRUPO_LOTADO" }
```

- **Regra de negócio violada:** `throw new ErroDeNegocio(StatusCodes.Status422UnprocessableEntity, "GRUPO_LOTADO", "...")`
- **Dados inválidos** (atributos de `DataAnnotations`): 400, `codigo: "DADOS_INVALIDOS"` e o mapa `campos` com a mensagem de cada campo. A validação vale para toda rota criada com `MapearApiDoModulo`.
- No front-end, `requisitar` converte a resposta em `ErroDaApi`, com `status`, `codigo` e `campos`.

---

## 5. Banco de dados

| Banco | Uso | Quem cria |
|---|---|---|
| `connexa` | Desenvolvimento (`dotnet run`) | variável `POSTGRES_DB` do contêiner |
| `connexa_teste` | Testes automatizados — schemas apagados e recriados a cada execução da suíte | `infra/postgres/init/01-cria-banco-de-teste.sql` |

- Cada `Modulo<Modulo>.cs` registra o schema do módulo. Quando a aplicação sobe, os schemas são criados e as migrações de todos os contextos do EF Core são aplicadas.
- Tabelas são criadas **somente por migrações** do EF Core. As convenções estão em `backend/MIGRACOES.md`.
- Conexão: as chaves `CONNEXA_DB_HOST`, `CONNEXA_DB_PORTA`, `CONNEXA_DB_NOME`, `CONNEXA_DB_USUARIO` e `CONNEXA_DB_SENHA` do `appsettings.json` podem ser trocadas por variáveis de ambiente com o mesmo nome.

---

## 6. Estudante simulado

A autenticação real está **fora da Sprint 1**. Enquanto isso, o módulo `Connexa.Identidade` simula o estudante "logado".

- Outros módulos usam apenas a interface `Connexa.Identidade.IUsuarioAtual`: `usuarioAtual.Obter().Id`.
- Há três estudantes fictícios em `appsettings.json`: `alice` (padrão), `bento` e `cecilia`.
- O estudante é escolhido pelo cabeçalho `X-Connexa-Usuario-Simulado: <chave>`:
  - na interface, pelo seletor no topo da página;
  - nos testes, com `ClienteComo("bento")`;
  - em ferramentas como o Swagger ou o `curl`, informando o cabeçalho na requisição.

Quando a autenticação real chegar, só a implementação interna de `Connexa.Identidade` muda; os outros módulos continuam usando `IUsuarioAtual`.

---

## 7. Testes e verificação

| Verificação | Onde | Ferramenta |
|---|---|---|
| Compilação sem avisos | `Directory.Build.props` (`TreatWarningsAsErrors`) | compilador C# e analisadores |
| Testes de aceitação na API (um por cenário Gherkin) | `backend/tests/Connexa.Testes/<Modulo>/*AceitacaoTest.cs` | MSTest + `WebApplicationFactory`, classe base `TesteDeAceitacao` |
| Fronteiras entre módulos | `ArquiteturaModularTest` | NetArchTest + compilador |
| Um schema por módulo | `SchemasDosModulosTest` | MSTest + Npgsql |
| Lint, tipos e fronteiras entre features | `frontend/eslint.config.js`, `tsconfig.app.json` | ESLint + TypeScript |
| Testes de componente | `frontend/src/**/*.test.tsx` | Vitest + Testing Library |
| Instruções do agente sincronizadas | `AGENTS.md` = `.github/copilot-instructions.md` | `cmp` |

**`TesteDeAceitacao`** sobe a aplicação inteira contra o banco `connexa_teste` e esvazia todas as tabelas antes de cada teste, de modo que um teste nunca vê os dados de outro.

**Convenção de nomes:** uma classe por história e um método por cenário, com `[TestMethod(DisplayName = "Cenário: <nome exato do cenário>")]`. No front-end, `it('Cenário: <nome exato>')`.

> **Nunca desative uma verificação** para o build passar. Teste ignorado, asserção removida, aviso suprimido ou regra de lint desligada transformam a evidência em mentira e invalidam o trabalho da Task.

---

## 8. Agente de IA e Azure Boards

O agente lê as Tasks do Azure Boards por MCP. As instruções que ele segue estão em `AGENTS.md` (Codex) e `.github/copilot-instructions.md` (Copilot) — mantenha os dois idênticos; o CI confere.

| Caminho | Agente | Servidor MCP | Arquivo |
|---|---|---|---|
| 1 | GitHub Copilot no VS Code | Remoto, com login da conta institucional | `.vscode/mcp.json` — conexões `ado-planejamento` (escrita) e `ado-leitura` (somente leitura) |
| 2 | Codex no VS Code | Local, via `npx` (requer Node.js 20+) | `.codex/config.toml` |

Os detalhes de cada caminho, os prompts de referência e o protocolo de falha estão no roteiro da Atividade de Sprint.

---

## 9. Configuração depois de criar o repositório da equipe

A Parte 1 do roteiro da Atividade de Sprint (Etapas 3 e 4) detalha estes passos. Os nomes entre `< >` são escolhidos pela equipe; só o projeto do Azure DevOps tem nome fixo: `Connexa`.

1. **Nome da organização do Azure DevOps:** substitua `<organizacao-azure-devops>` em `.vscode/mcp.json` e `.codex/config.toml`. A organização deve ter sido criada com a conta institucional (Microsoft Entra ID).
2. **Proteção do `main`** (**Settings › Rules › Rulesets** ou **Settings › Branches**):
   - exigir pull request antes do merge, com **1 aprovação**;
   - exigir os checks **Back-end (testes e fronteiras)**, **Front-end (lint, testes e build)** e **Instruções do agente sincronizadas** — eles aparecem na lista depois da primeira execução do CI;
   - bloquear *force push* e exclusão do branch.
3. **Azure Boards ↔ GitHub:** conecte o projeto `Connexa` a `<repositorio-da-equipe>`, em `<organizacao-github>` (**Project Settings › Boards › GitHub connections**), para que `AB#<id>` em commits e pull requests crie o vínculo com o work item.
4. **ADR-001:** registre em `docs/adr/` a arquitetura e a stack adotadas, usando `docs/adr/modelo-adr.md`.

---

## 10. Decisões deste modelo

| Decisão | Motivo |
|---|---|
| Testes usam o PostgreSQL do Compose, em um banco separado (`connexa_teste`) recriado a cada execução | Funciona igual no Codespaces, no CI e na máquina local sem exigir Docker dentro do Codespace. A proposta de arquitetura sugere Testcontainers; a equipe pode adotá-lo em um ADR |
| Isolamento entre testes esvaziando as tabelas antes de cada teste | A aplicação roda em um servidor de teste separado do teste; uma transação desfeita ao final não alcançaria as requisições |
| `network_mode: service:db` no Codespaces | O banco responde em `localhost:5432` dentro e fora do Codespaces, e nenhuma configuração muda entre os ambientes |
| Um projeto por módulo, com `internal` em `Internal/` | O compilador protege as fronteiras; o NetArchTest confere o que o compilador não alcança |
| Schemas e migrações aplicados quando a aplicação sobe | Mesmo comportamento no desenvolvimento, nos testes e no CI, sem passos manuais |
| Sem ASP.NET Core Identity na Sprint 1 | A autenticação está fora da sprint |
| `IUsuarioAtual` como API do módulo `Connexa.Identidade` | Os outros módulos não dependem de como a autenticação será feita |
| Validação com `DataAnnotations` por um filtro próprio em `Connexa.Comum` | Funciona com endpoints mapeados em qualquer projeto de módulo e produz o mesmo formato de erro do front-end |
| Despachante de eventos em processo, em `Connexa.Comum` | Classe simples, sem biblioteca externa, suficiente para um monolito |
| Swagger UI apenas como visualizador do OpenAPI nativo | O contrato é gerado pelo próprio ASP.NET Core; o pacote só desenha a página |
| `nuget.config` com somente o nuget.org | O restore não depende das fontes configuradas em cada máquina |
| Dependências da sprint já incluídas (EF Core, Npgsql, OpenAPI, React Hook Form, Zod) | As Tasks proíbem o agente de incluir dependências; o que a equipe precisa já está disponível |
| Sem testes de ponta a ponta no navegador (Playwright) | Fora do escopo da Sprint 1; o roteiro da Atividade de Sprint exige verificação manual dos cenários na interface |
| Versões fixas no `Directory.Packages.props` e no `package.json` | O build é reprodutível; atualizar é uma decisão registrada em ADR |

---

## 11. Solução de problemas

| Sintoma | O que fazer |
|---|---|
| Testes do back-end falham com `Failed to connect` ou a API não sobe | O PostgreSQL não está rodando. Fora do Codespaces: `docker compose up -d db` |
| `database "connexa_teste" does not exist` | O volume foi criado antes do script de inicialização. Rode `docker compose down -v` e `docker compose up -d db` (apaga os dados de desenvolvimento) |
| `ArquiteturaModularTest` falha | Um tipo público ficou em `Internal`, um módulo usou o `Internal` de outro ou uma dependência proibida apareceu. A mensagem indica os tipos. Refaça a mudança respeitando a fronteira — nunca altere o teste |
| O build falha com um aviso transformado em erro | Corrija a causa do aviso; não o suprima |
| `dotnet ef` não é reconhecido | Na pasta `backend/`, rode `dotnet tool restore` |
| O lint acusa importação restrita | Importe a feature pelo `index.ts` (`@/features/<feature>`) e use o alias `@/` |
| A interface mostra "O back-end não está respondendo" | Suba a API em outro terminal: `cd backend && dotnet run --project src/Connexa.Api` |
| Porta 5432 ocupada na máquina local | Outro PostgreSQL está instalado. Pare-o ou troque a porta publicada no `docker-compose.yml` e use `CONNEXA_DB_PORTA` |

---

*Modelo de repositório — Disciplina de Engenharia de Software — Projeto Connexa.*
