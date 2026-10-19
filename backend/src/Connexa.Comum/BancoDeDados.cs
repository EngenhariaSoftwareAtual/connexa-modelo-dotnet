using Connexa.Comum.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Connexa.Comum;

/// <summary>
/// Registro dos dados de cada módulo: um schema por módulo e, quando o módulo tiver
/// tabelas, um contexto do EF Core com as migrações no próprio schema.
/// </summary>
/// <remarks>
/// <para>
/// Os schemas registrados são criados e as migrações de todos os contextos são aplicadas
/// quando a aplicação sobe (<see cref="ModuloComum.PrepararBancoDeDadosAsync"/>).
/// </para>
/// <para>
/// Cada módulo é dono dos seus dados: nenhum módulo lê ou escreve tabelas de outro.
/// </para>
/// </remarks>
public static class BancoDeDados
{
    /// <summary>Nome da tabela de histórico de migrações, criada no schema de cada módulo.</summary>
    public const string TabelaDeHistoricoDeMigracoes = "__historico_de_migracoes";

    /// <summary>Garante que o schema do módulo exista no banco.</summary>
    public static IServiceCollection RegistrarSchemaDoModulo(this IServiceCollection servicos, string schema)
    {
        RegistroDosModulos.Obter(servicos).AdicionarSchema(schema);
        return servicos;
    }

    /// <summary>
    /// Registra o contexto do EF Core de um módulo. As tabelas e o histórico de migrações
    /// ficam no schema do módulo.
    /// </summary>
    /// <example>
    /// <code>servicos.AdicionarContextoDoModulo&lt;GruposContexto&gt;(ModuloGrupos.Schema);</code>
    /// </example>
    public static IServiceCollection AdicionarContextoDoModulo<TContexto>(this IServiceCollection servicos, string schema)
        where TContexto : DbContext
    {
        servicos.RegistrarSchemaDoModulo(schema);
        RegistroDosModulos.Obter(servicos).AdicionarContexto(typeof(TContexto));
        servicos.AddDbContext<TContexto>((provedor, opcoes) => opcoes.UseNpgsql(
            provedor.GetRequiredService<NpgsqlDataSource>(),
            npgsql => npgsql.MigrationsHistoryTable(TabelaDeHistoricoDeMigracoes, schema)));
        return servicos;
    }
}
