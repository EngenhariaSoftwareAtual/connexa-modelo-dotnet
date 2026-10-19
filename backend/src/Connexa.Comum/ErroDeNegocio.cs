using Microsoft.AspNetCore.Http;

namespace Connexa.Comum;

/// <summary>
/// Violação de uma regra de negócio que deve chegar ao cliente como resposta de erro.
/// </summary>
/// <remarks>
/// <para>
/// É convertida em um <i>Problem Detail</i> (RFC 9457) com a propriedade <c>codigo</c>,
/// que os testes de aceitação e o front-end usam para identificar o erro sem depender
/// do texto da mensagem.
/// </para>
/// <code>
/// throw new ErroDeNegocio(StatusCodes.Status422UnprocessableEntity,
///     "GRUPO_LOTADO", "O grupo já atingiu o número máximo de participantes.");
/// </code>
/// <para>
/// Convenções: <c>codigo</c> em MAIÚSCULAS_COM_SUBLINHADO; 404 para recurso inexistente,
/// 409 para conflito de estado, 422 para regra de negócio violada.
/// </para>
/// </remarks>
public class ErroDeNegocio(int status, string codigo, string mensagem) : Exception(mensagem)
{
    /// <summary>Status HTTP da resposta, por exemplo <see cref="StatusCodes.Status422UnprocessableEntity"/>.</summary>
    public int Status { get; } = status;

    /// <summary>Código do erro, em MAIÚSCULAS_COM_SUBLINHADO.</summary>
    public string Codigo { get; } = codigo;
}
