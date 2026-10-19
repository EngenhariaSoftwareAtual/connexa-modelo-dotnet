using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace Connexa.Comum.Internal;

/// <summary>
/// Converte erros em respostas no formato <i>Problem Detail</i> (RFC 9457), sempre com a
/// propriedade <c>codigo</c> quando o erro é conhecido.
/// </summary>
/// <remarks>
/// <list type="bullet">
///   <item><see cref="ErroDeNegocio"/>: status e código definidos por quem lançou o erro.</item>
///   <item>Corpo da requisição ilegível: 400, código <c>DADOS_INVALIDOS</c>.</item>
///   <item>Falha de validação: 400, código <c>DADOS_INVALIDOS</c> e <c>campos</c> (ver <see cref="ValidacaoDeEntrada"/>).</item>
/// </list>
/// </remarks>
internal sealed class TratamentoDeErros(IProblemDetailsService detalhes) : IExceptionHandler
{
    public const string DadosInvalidos = "DADOS_INVALIDOS";

    public async ValueTask<bool> TryHandleAsync(HttpContext contexto, Exception erro, CancellationToken cancelamento)
    {
        var problema = erro switch
        {
            ErroDeNegocio negocio => Problema(negocio.Status, negocio.Message, negocio.Codigo),
            BadHttpRequestException => Problema(
                StatusCodes.Status400BadRequest, "O corpo da requisição não pôde ser lido.", DadosInvalidos),
            _ => null,
        };
        if (problema is null)
        {
            return false;
        }

        contexto.Response.StatusCode = problema.Status!.Value;
        return await detalhes.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = contexto,
            ProblemDetails = problema,
            Exception = erro,
        });
    }

    public static ProblemDetails Problema(int status, string detalhe, string codigo)
    {
        var problema = new ProblemDetails
        {
            Status = status,
            Title = ReasonPhrases.GetReasonPhrase(status),
            Detail = detalhe,
        };
        problema.Extensions["codigo"] = codigo;
        return problema;
    }
}

internal static class RegistroDoTratamentoDeErros
{
    public static IServiceCollection AdicionarTratamentoDeErros(this IServiceCollection servicos)
    {
        servicos.AddProblemDetails();
        servicos.AddExceptionHandler<TratamentoDeErros>();
        return servicos;
    }
}
