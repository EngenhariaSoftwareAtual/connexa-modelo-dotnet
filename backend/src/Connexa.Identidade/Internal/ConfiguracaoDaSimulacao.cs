namespace Connexa.Identidade.Internal;

/// <summary>
/// Estudantes simulados disponíveis enquanto a autenticação real não existe.
/// Valores em <c>Connexa:Identidade:Simulacao</c> no <c>appsettings.json</c>; o primeiro é o padrão.
/// </summary>
internal sealed class ConfiguracaoDaSimulacao
{
    public const string Secao = "Connexa:Identidade:Simulacao";

    public List<EstudanteSimulado> Usuarios { get; init; } = [];
}

/// <summary>Estudante fictício.</summary>
/// <remarks><see cref="Chave"/> é o identificador curto usado no cabeçalho <c>X-Connexa-Usuario-Simulado</c>.</remarks>
internal sealed class EstudanteSimulado
{
    public string Chave { get; init; } = "";

    public Guid Id { get; init; }

    public string Nome { get; init; } = "";

    public string Email { get; init; } = "";
}
