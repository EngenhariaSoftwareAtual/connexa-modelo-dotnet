using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Connexa.Comum.Internal;

/// <summary>
/// Cria o schema de cada módulo e aplica as migrações de todos os contextos do EF Core.
/// </summary>
internal static class PreparacaoDoBanco
{
    public static async Task ExecutarAsync(IServiceProvider provedor)
    {
        var registro = provedor.GetRequiredService<RegistroDosModulos>();
        var fonte = provedor.GetRequiredService<NpgsqlDataSource>();

        foreach (var schema in registro.Schemas)
        {
            await using var comando = fonte.CreateCommand($"create schema if not exists \"{schema}\"");
            await comando.ExecuteNonQueryAsync();
        }

        await using var escopo = provedor.CreateAsyncScope();
        foreach (var tipo in registro.Contextos)
        {
            var contexto = (DbContext)escopo.ServiceProvider.GetRequiredService(tipo);
            await contexto.Database.MigrateAsync();
        }
    }
}
