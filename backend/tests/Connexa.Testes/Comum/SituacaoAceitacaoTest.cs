using System.Net;
using Connexa.Comum.Internal;

namespace Connexa.Testes.Comum;

/// <summary>Exemplo do modelo — cenários em <c>docs/exemplo/situacao-do-sistema.feature</c>.</summary>
[TestClass]
public sealed class SituacaoAceitacaoTest : TesteDeAceitacao
{
    [TestMethod(DisplayName = "Cenário: aplicação e banco disponíveis")]
    public async Task AplicacaoEBancoDisponiveis()
    {
        var resposta = await Cliente.GetAsync("/api/situacao");

        Assert.AreEqual(HttpStatusCode.OK, resposta.StatusCode);
        var corpo = await LerJsonAsync(resposta);
        Assert.AreEqual(SituacaoEndpoints.Disponivel, corpo.GetProperty("aplicacao").GetString());
        Assert.AreEqual(SituacaoEndpoints.Disponivel, corpo.GetProperty("banco").GetString());
        Assert.IsFalse(string.IsNullOrWhiteSpace(corpo.GetProperty("versao").GetString()));
    }
}
