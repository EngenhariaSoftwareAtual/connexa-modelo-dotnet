using Connexa.Comum.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Connexa.Comum;

/// <summary>
/// Convenções das rotas da API: cada módulo publica as suas operações em <c>/api/&lt;modulo&gt;</c>,
/// com a entrada validada automaticamente.
/// </summary>
public static class RotasDosModulos
{
    /// <summary>
    /// Cria o grupo de rotas <c>/api/&lt;modulo&gt;</c>, já com a validação de entrada.
    /// </summary>
    /// <example>
    /// <code>
    /// var api = rotas.MapearApiDoModulo("grupos");
    /// api.MapPost("/", CriarGrupo);
    /// </code>
    /// </example>
    public static RouteGroupBuilder MapearApiDoModulo(this IEndpointRouteBuilder rotas, string modulo) =>
        rotas.MapGroup($"/api/{modulo}")
            .WithTags(modulo)
            .ValidarEntrada();

    /// <summary>
    /// Valida, com os atributos de <c>System.ComponentModel.DataAnnotations</c>, os objetos recebidos
    /// pela operação. Dados inválidos viram 400 com <c>codigo: DADOS_INVALIDOS</c> e o mapa <c>campos</c>.
    /// </summary>
    public static TBuilder ValidarEntrada<TBuilder>(this TBuilder construtor)
        where TBuilder : IEndpointConventionBuilder =>
        construtor.AddEndpointFilter(new ValidacaoDeEntrada());
}
