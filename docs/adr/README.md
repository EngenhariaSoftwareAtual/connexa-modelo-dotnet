# Registros de decisão de arquitetura (ADR)

Um ADR registra **uma** decisão de arquitetura: o contexto, a decisão, as alternativas descartadas e as consequências.

- Um arquivo por decisão, numerado em sequência: `ADR-001-<assunto>.md`, `ADR-002-<assunto>.md`, ...
- Use o modelo em [`modelo-adr.md`](modelo-adr.md).
- Uma decisão registrada não é editada depois de aceita. Se mudar, crie um novo ADR que **substitui** o anterior e atualize o campo *Situação* do antigo.

## Decisões que exigem ADR

- O **ADR-001** registra a arquitetura e a stack escolhidas pela equipe.
- Incluir, trocar ou remover uma dependência (`Directory.Packages.props`, `package.json`).
- Criar um módulo novo ou mudar as regras de fronteira.
- Qualquer exceção às regras de `AGENTS.md`.
