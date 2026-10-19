# language: pt
# Exemplo do modelo — não é uma história do Connexa.
# Testes correspondentes:
#   backend/tests/Connexa.Testes/Comum/SituacaoAceitacaoTest.cs
#   frontend/src/features/situacao/PainelSituacao.test.tsx

Funcionalidade: situação do sistema
  Como integrante da equipe
  Quero saber se a aplicação e o banco estão respondendo
  Para identificar rapidamente problemas no ambiente

  Cenário: aplicação e banco disponíveis
    Dado que o back-end e o banco de dados estão em execução
    Quando a situação do sistema é consultada
    Então a aplicação aparece como "disponível"
    E o banco de dados aparece como "disponível"
    E a versão da aplicação é informada

  Cenário: banco de dados indisponível
    Dado que o banco de dados não está respondendo
    Quando a situação do sistema é consultada
    Então a mensagem "O banco de dados está indisponível." é exibida
