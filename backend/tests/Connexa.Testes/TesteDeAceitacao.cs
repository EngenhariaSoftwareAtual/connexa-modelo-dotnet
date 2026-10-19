using System.Net.Http.Json;
using System.Text.Json;
using Connexa.Testes.Apoio;

namespace Connexa.Testes;

/// <summary>
/// Classe base dos testes de aceitação no nível da API.
/// </summary>
/// <remarks>
/// <list type="bullet">
///   <item>Sobe a aplicação inteira, com um <see cref="HttpClient"/> para fazer as requisições.</item>
///   <item>Usa o banco <c>connexa_teste</c>, recriado antes do primeiro teste (<see cref="BancoDeTesteLimpo"/>).</item>
///   <item>Antes de cada teste, todas as tabelas são esvaziadas: um teste nunca vê os dados de outro.</item>
/// </list>
/// <para>
/// Convenção: uma classe por história, um método por cenário Gherkin, com
/// <c>[TestMethod(DisplayName = "Cenário: &lt;nome exato do cenário&gt;")]</c>.
/// </para>
/// </remarks>
public abstract class TesteDeAceitacao
{
    /// <summary>Cabeçalho que escolhe o estudante simulado da requisição.</summary>
    protected const string CabecalhoDoUsuarioSimulado = "X-Connexa-Usuario-Simulado";

    private static readonly SemaphoreSlim Preparacao = new(1, 1);
    private static FabricaDaAplicacao? fabrica;

    /// <summary>Cliente HTTP da aplicação, como o estudante simulado padrão (<c>alice</c>).</summary>
    protected HttpClient Cliente { get; private set; } = null!;

    [TestInitialize]
    public async Task PrepararAsync()
    {
        await Preparacao.WaitAsync();
        try
        {
            if (fabrica is null)
            {
                await BancoDeTesteLimpo.ApagarSchemasAsync();
                fabrica = new FabricaDaAplicacao();
                _ = fabrica.Server;
            }
        }
        finally
        {
            Preparacao.Release();
        }

        await BancoDeTesteLimpo.EsvaziarTabelasAsync();
        Cliente = fabrica.CreateClient();
    }

    [TestCleanup]
    public void Encerrar() => Cliente.Dispose();

    /// <summary>Cria um cliente HTTP que faz as requisições como outro estudante simulado.</summary>
    protected static HttpClient ClienteComo(string estudanteSimulado)
    {
        var cliente = fabrica!.CreateClient();
        cliente.DefaultRequestHeaders.Add(CabecalhoDoUsuarioSimulado, estudanteSimulado);
        return cliente;
    }

    /// <summary>Lê o corpo da resposta como JSON.</summary>
    protected static async Task<JsonElement> LerJsonAsync(HttpResponseMessage resposta) =>
        await resposta.Content.ReadFromJsonAsync<JsonElement>();

    internal static async Task EncerrarAplicacaoAsync()
    {
        if (fabrica is not null)
        {
            await fabrica.DisposeAsync();
            fabrica = null;
        }
    }
}

[TestClass]
public static class CicloDaSuite
{
    [AssemblyCleanup]
    public static Task EncerrarAsync() => TesteDeAceitacao.EncerrarAplicacaoAsync();
}
