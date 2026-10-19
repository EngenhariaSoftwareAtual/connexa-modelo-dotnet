using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Connexa.Testes.Apoio;

/// <summary>
/// Sobe a aplicação inteira, em memória, apontando para o banco de teste.
/// </summary>
internal sealed class FabricaDaAplicacao : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder construtor)
    {
        construtor.UseEnvironment("Teste");
        construtor.UseSetting("CONNEXA_DB_NOME", BancoDeTesteLimpo.Nome);
    }
}
