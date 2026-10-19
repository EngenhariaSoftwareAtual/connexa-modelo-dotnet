# language: pt
# Exemplo do modelo — não é uma história do Connexa.
# Testes correspondentes:
#   backend/tests/Connexa.Testes/Identidade/SessaoSimuladaAceitacaoTest.cs
#   frontend/src/features/sessao/SeletorDeEstudante.test.tsx

Funcionalidade: sessão do estudante simulado
  Como integrante da equipe
  Quero escolher qual estudante fictício está usando o sistema
  Para testar cenários com mais de um estudante antes de existir a autenticação real

  Cenário: sem indicação, a sessão é do primeiro estudante simulado
    Dado que nenhum estudante simulado foi escolhido
    Quando a sessão atual é consultada
    Então o estudante da sessão é "Alice Prado"

  Cenário: estudante simulado escolhido pelo cabeçalho
    Dado que o estudante simulado "bento" foi escolhido
    Quando a sessão atual é consultada
    Então o estudante da sessão é "Bento Ramos"

  Cenário: estudante simulado inexistente
    Dado que o estudante simulado "zeca" foi escolhido
    Quando a sessão atual é consultada
    Então a consulta é recusada com o código "USUARIO_SIMULADO_INEXISTENTE"

  Cenário: lista dos estudantes simulados disponíveis
    Quando a lista de estudantes simulados é consultada
    Então três estudantes são listados
    E o primeiro é "Alice Prado"

  Cenário: estudante simulado escolhido pelo usuário
    Dado que a tela mostra o seletor de estudante simulado
    Quando o usuário escolhe "Bento Ramos"
    Então as próximas consultas são feitas como "bento"
