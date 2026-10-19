using Connexa.Testes.Apoio;
using Npgsql;

namespace Connexa.Testes.Comum;

/// <summary>
/// Garante que o banco tem um schema para cada módulo (regra: cada módulo é dono dos seus dados).
/// </summary>
[TestClass]
public sealed class SchemasDosModulosTest : TesteDeAceitacao
{
    private static readonly string[] Modulos =
        ["identidade", "perfil", "grupos", "encontros", "acompanhamento", "painel", "materiais"];

    [TestMethod(DisplayName = "cada módulo tem o seu schema no banco")]
    public async Task CadaModuloTemOSeuSchema()
    {
        await using var conexao = new NpgsqlConnection(BancoDeTesteLimpo.TextoDeConexao);
        await conexao.OpenAsync();
        await using var comando = new NpgsqlCommand("select schema_name from information_schema.schemata", conexao);
        await using var leitor = await comando.ExecuteReaderAsync();

        var schemas = new List<string>();
        while (await leitor.ReadAsync())
        {
            schemas.Add(leitor.GetString(0));
        }

        foreach (var modulo in Modulos)
        {
            CollectionAssert.Contains(schemas, modulo, $"Falta o schema do módulo {modulo}.");
        }
    }
}
