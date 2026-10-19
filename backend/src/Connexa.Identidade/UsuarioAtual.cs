namespace Connexa.Identidade;

/// <summary>
/// API pública do módulo Identidade: informa qual estudante fez a requisição atual.
/// </summary>
/// <remarks>
/// Use sempre esta interface para saber "quem está usando o sistema".
/// Nunca leia cabeçalhos, tokens ou sessões diretamente em outros módulos.
/// <code>
/// internal static async Task&lt;IResult&gt; MeusGruposAsync(IUsuarioAtual usuarioAtual, ...)
/// {
///     Guid estudante = usuarioAtual.Obter().Id;
///     ...
/// }
/// </code>
/// </remarks>
public interface IUsuarioAtual
{
    /// <returns>O estudante da requisição atual.</returns>
    UsuarioAutenticado Obter();
}

/// <summary>
/// Estudante identificado na requisição atual.
/// </summary>
/// <remarks>
/// Outros módulos devem guardar apenas o <see cref="Id"/> — nunca copiar nome ou e-mail
/// para as próprias tabelas.
/// </remarks>
/// <param name="Id">Identificador estável do estudante.</param>
/// <param name="Nome">Nome para exibição.</param>
/// <param name="Email">E-mail institucional.</param>
public sealed record UsuarioAutenticado(Guid Id, string Nome, string Email);
