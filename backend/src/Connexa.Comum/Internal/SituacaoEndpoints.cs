using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Npgsql;

namespace Connexa.Comum.Internal;

/// <summary>
/// Fatia de exemplo do modelo: informa se a aplicação e o banco estão disponíveis.
/// </summary>
/// <remarks>
/// Não é uma funcionalidade do Connexa. Existe para mostrar, de ponta a ponta, o caminho
/// que toda fatia percorre: endpoint, teste de aceitação com o nome do cenário e tela no front-end.
/// </remarks>
internal static class SituacaoEndpoints
{
    public const string Disponivel = "disponivel";
    public const string Indisponivel = "indisponivel";

    public static readonly string Versao =
        typeof(SituacaoEndpoints).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
        ?? "desconhecida";

    public static void Mapear(IEndpointRouteBuilder rotas) =>
        rotas.MapGet("/api/situacao", ConsultarAsync)
            .WithTags("comum")
            .Produces<Situacao>(StatusCodes.Status200OK)
            .Produces<Situacao>(StatusCodes.Status503ServiceUnavailable);

    private static async Task<IResult> ConsultarAsync(NpgsqlDataSource banco, CancellationToken cancelamento)
    {
        var bancoDisponivel = await BancoRespondeAsync(banco, cancelamento);
        var situacao = new Situacao(Disponivel, bancoDisponivel ? Disponivel : Indisponivel, Versao);
        return TypedResults.Json(
            situacao,
            statusCode: bancoDisponivel ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable);
    }

    private static async Task<bool> BancoRespondeAsync(NpgsqlDataSource banco, CancellationToken cancelamento)
    {
        try
        {
            await using var comando = banco.CreateCommand("select 1");
            return Convert.ToInt32(await comando.ExecuteScalarAsync(cancelamento)) == 1;
        }
        catch (Exception falha) when (falha is NpgsqlException or TimeoutException or InvalidOperationException)
        {
            return false;
        }
    }

    internal sealed record Situacao(string Aplicacao, string Banco, string Versao);
}
