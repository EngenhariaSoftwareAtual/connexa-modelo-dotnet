using Npgsql;

namespace Connexa.Testes.Apoio;

/// <summary>
/// Banco usado pelos testes de aceitação: <c>connexa_teste</c>, separado do banco de desenvolvimento.
/// </summary>
/// <remarks>
/// <para>
/// Antes do primeiro teste de aceitação, apaga todos os schemas do banco de teste; a aplicação os
/// recria e aplica todas as migrações ao subir. Assim, sobras de execuções anteriores (ou de outro
/// branch) nunca interferem no resultado.
/// </para>
/// <para>
/// Antes de cada teste, esvazia todas as tabelas: um teste nunca vê os dados de outro.
/// </para>
/// <para>Só existe no código de teste: o banco de desenvolvimento nunca é apagado.</para>
/// </remarks>
internal static class BancoDeTesteLimpo
{
    public static string Nome => Variavel("CONNEXA_DB_TESTE", "connexa_teste");

    public static string TextoDeConexao => new NpgsqlConnectionStringBuilder
    {
        Host = Variavel("CONNEXA_DB_HOST", "localhost"),
        Port = int.Parse(Variavel("CONNEXA_DB_PORTA", "5432")),
        Database = Nome,
        Username = Variavel("CONNEXA_DB_USUARIO", "connexa"),
        Password = Variavel("CONNEXA_DB_SENHA", "connexa"),
        Timeout = 5,
    }.ConnectionString;

    public static async Task ApagarSchemasAsync()
    {
        await ExecutarAsync("""
            do $$
            declare s record;
            begin
              for s in select schema_name from information_schema.schemata
                       where schema_name not in ('public', 'information_schema')
                         and schema_name not like 'pg\_%'
              loop
                execute format('drop schema %I cascade', s.schema_name);
              end loop;
            end $$;
            """);
    }

    public static async Task EsvaziarTabelasAsync()
    {
        await ExecutarAsync("""
            do $$
            declare t record;
            begin
              for t in select table_schema, table_name from information_schema.tables
                       where table_type = 'BASE TABLE'
                         and table_schema not in ('information_schema')
                         and table_schema not like 'pg\_%'
                         and table_name <> '__historico_de_migracoes'
              loop
                execute format('truncate table %I.%I cascade', t.table_schema, t.table_name);
              end loop;
            end $$;
            """);
    }

    private static async Task ExecutarAsync(string sql)
    {
        await using var conexao = new NpgsqlConnection(TextoDeConexao);
        await conexao.OpenAsync();
        await using var comando = new NpgsqlCommand(sql, conexao);
        await comando.ExecuteNonQueryAsync();
    }

    private static string Variavel(string nome, string padrao) =>
        Environment.GetEnvironmentVariable(nome) is { Length: > 0 } valor ? valor : padrao;
}
