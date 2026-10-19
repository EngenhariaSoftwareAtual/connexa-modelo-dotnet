using Connexa.Comum;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Connexa.Identidade.Internal;

/// <summary>
/// Implementação temporária de <see cref="IUsuarioAtual"/> para a Sprint 1.
/// </summary>
/// <remarks>
/// <para>
/// O estudante é escolhido pelo cabeçalho <see cref="Cabecalho"/>, cujo valor é a chave de um
/// estudante configurado. Sem o cabeçalho, vale o primeiro estudante da lista. Isso permite
/// testar cenários com mais de um estudante (por exemplo, um cria o grupo e outro entra nele).
/// </para>
/// <para>Será substituída pela autenticação real em uma sprint futura.</para>
/// </remarks>
internal sealed class UsuarioSimulado(IHttpContextAccessor requisicao, IOptions<ConfiguracaoDaSimulacao> configuracao)
    : IUsuarioAtual
{
    public const string Cabecalho = "X-Connexa-Usuario-Simulado";
    public const string UsuarioSimuladoInexistente = "USUARIO_SIMULADO_INEXISTENTE";

    private readonly IReadOnlyList<EstudanteSimulado> estudantes = configuracao.Value.Usuarios.Count > 0
        ? configuracao.Value.Usuarios
        : throw new InvalidOperationException(
            $"Configure ao menos um estudante em {ConfiguracaoDaSimulacao.Secao}:Usuarios.");

    public UsuarioAutenticado Obter()
    {
        var estudante = EstudanteDaRequisicao();
        return new UsuarioAutenticado(estudante.Id, estudante.Nome, estudante.Email);
    }

    /// <returns>O estudante simulado da requisição atual, com a sua chave.</returns>
    public EstudanteSimulado EstudanteDaRequisicao()
    {
        var chave = ChaveInformada();
        return chave is null ? estudantes[0] : BuscarPelaChave(chave);
    }

    public IReadOnlyList<EstudanteSimulado> Todos() => estudantes;

    private EstudanteSimulado BuscarPelaChave(string chave) =>
        estudantes.FirstOrDefault(estudante => estudante.Chave == chave)
        ?? throw new ErroDeNegocio(
            StatusCodes.Status400BadRequest,
            UsuarioSimuladoInexistente,
            $"Não existe estudante simulado com a chave '{chave}'.");

    private string? ChaveInformada()
    {
        var valor = requisicao.HttpContext?.Request.Headers[Cabecalho].ToString();
        return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }
}

internal static class RegistroDaSimulacao
{
    public static IServiceCollection AdicionarSimulacaoDeUsuario(this IServiceCollection servicos)
    {
        servicos.AddHttpContextAccessor();
        servicos.AddOptions<ConfiguracaoDaSimulacao>().BindConfiguration(ConfiguracaoDaSimulacao.Secao);
        servicos.AddScoped<UsuarioSimulado>();
        servicos.AddScoped<IUsuarioAtual>(provedor => provedor.GetRequiredService<UsuarioSimulado>());
        return servicos;
    }
}
