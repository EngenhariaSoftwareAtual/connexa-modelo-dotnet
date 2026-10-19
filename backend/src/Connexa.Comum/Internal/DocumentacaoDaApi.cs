using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Connexa.Comum.Internal;

/// <summary>
/// Documentação OpenAPI, gerada pelo próprio ASP.NET Core a partir dos endpoints.
/// </summary>
/// <remarks>
/// Contrato em JSON: <c>/openapi/v1.json</c> · Interface navegável: <c>/swagger</c>.
/// </remarks>
internal static class DocumentacaoDaApi
{
    public const string Caminho = "/openapi/v1.json";

    public static IServiceCollection AdicionarDocumentacaoDaApi(this IServiceCollection servicos) =>
        servicos.AddOpenApi(opcoes => opcoes.AddDocumentTransformer((documento, _, _) =>
        {
            documento.Info.Title = "Connexa — API";
            documento.Info.Version = SituacaoEndpoints.Versao;
            documento.Info.Description =
                "API do Connexa. Cada operação pertence a um módulo; o prefixo da rota indica qual.";
            return Task.CompletedTask;
        }));

    public static void MapearDocumentacaoDaApi(this IEndpointRouteBuilder rotas) => rotas.MapOpenApi();

    public static void UsarDocumentacaoDaApi(this WebApplication aplicacao) =>
        aplicacao.UseSwaggerUI(opcoes =>
        {
            opcoes.SwaggerEndpoint(Caminho, "Connexa — API");
            opcoes.RoutePrefix = "swagger";
            opcoes.DocumentTitle = "Connexa — API";
        });
}
