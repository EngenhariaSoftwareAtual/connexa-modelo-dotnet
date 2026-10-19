using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Connexa.Comum.Internal;

/// <summary>
/// Conexão com o PostgreSQL. As chaves <c>CONNEXA_DB_*</c> vêm do <c>appsettings.json</c>
/// e podem ser trocadas por variáveis de ambiente com o mesmo nome.
/// </summary>
internal static class FonteDeDados
{
    public static IServiceCollection AdicionarFonteDeDados(this IServiceCollection servicos)
    {
        // A configuração é lida só quando a fonte é criada, para que os testes
        // consigam apontar para o banco de teste.
        servicos.AddSingleton(provedor =>
            new NpgsqlDataSourceBuilder(TextoDeConexao(provedor.GetRequiredService<IConfiguration>())).Build());
        return servicos;
    }

    public static string TextoDeConexao(IConfiguration configuracao) =>
        new NpgsqlConnectionStringBuilder
        {
            Host = configuracao["CONNEXA_DB_HOST"] ?? "localhost",
            Port = int.Parse(configuracao["CONNEXA_DB_PORTA"] ?? "5432"),
            Database = configuracao["CONNEXA_DB_NOME"] ?? "connexa",
            Username = configuracao["CONNEXA_DB_USUARIO"] ?? "connexa",
            Password = configuracao["CONNEXA_DB_SENHA"] ?? "connexa",
            Timeout = 5,
        }.ConnectionString;
}
