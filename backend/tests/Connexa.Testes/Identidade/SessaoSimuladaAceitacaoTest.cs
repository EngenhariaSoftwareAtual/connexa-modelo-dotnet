using System.Net;
using Connexa.Identidade.Internal;

namespace Connexa.Testes.Identidade;

/// <summary>
/// Exemplo do modelo — cenários em <c>docs/exemplo/sessao-simulada.feature</c>.
/// </summary>
/// <remarks>
/// Repare: um método por cenário, com o nome do cenário no <c>[TestMethod]</c>; cada teste verifica
/// o resultado observável descrito no cenário, incluindo o <c>codigo</c> do erro — e não apenas o status HTTP.
/// </remarks>
[TestClass]
public sealed class SessaoSimuladaAceitacaoTest : TesteDeAceitacao
{
    [TestMethod(DisplayName = "Cenário: sem indicação, a sessão é do primeiro estudante simulado")]
    public async Task SemIndicacaoASessaoEDoPrimeiroEstudante()
    {
        var resposta = await Cliente.GetAsync("/api/identidade/sessao");

        Assert.AreEqual(HttpStatusCode.OK, resposta.StatusCode);
        var corpo = await LerJsonAsync(resposta);
        Assert.AreEqual("alice", corpo.GetProperty("chave").GetString());
        Assert.AreEqual("Alice Prado", corpo.GetProperty("nome").GetString());
        Assert.IsTrue(corpo.GetProperty("simulado").GetBoolean());
    }

    [TestMethod(DisplayName = "Cenário: estudante simulado escolhido pelo cabeçalho")]
    public async Task EstudanteEscolhidoPeloCabecalho()
    {
        using var bento = ClienteComo("bento");

        var resposta = await bento.GetAsync("/api/identidade/sessao");

        Assert.AreEqual(HttpStatusCode.OK, resposta.StatusCode);
        var corpo = await LerJsonAsync(resposta);
        Assert.AreEqual("bento", corpo.GetProperty("chave").GetString());
        Assert.AreEqual("Bento Ramos", corpo.GetProperty("nome").GetString());
    }

    [TestMethod(DisplayName = "Cenário: estudante simulado inexistente")]
    public async Task EstudanteSimuladoInexistente()
    {
        using var zeca = ClienteComo("zeca");

        var resposta = await zeca.GetAsync("/api/identidade/sessao");

        Assert.AreEqual(HttpStatusCode.BadRequest, resposta.StatusCode);
        var corpo = await LerJsonAsync(resposta);
        Assert.AreEqual(UsuarioSimulado.UsuarioSimuladoInexistente, corpo.GetProperty("codigo").GetString());
    }

    [TestMethod(DisplayName = "Cenário: lista dos estudantes simulados disponíveis")]
    public async Task ListaDosEstudantesSimulados()
    {
        var resposta = await Cliente.GetAsync("/api/identidade/usuarios-simulados");

        Assert.AreEqual(HttpStatusCode.OK, resposta.StatusCode);
        var corpo = await LerJsonAsync(resposta);
        Assert.AreEqual(3, corpo.GetArrayLength());
        Assert.AreEqual("alice", corpo[0].GetProperty("chave").GetString());
    }
}
