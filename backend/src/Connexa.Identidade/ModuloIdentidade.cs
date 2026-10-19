using Connexa.Comum;
using Connexa.Identidade.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Connexa.Identidade;

/// <summary>
/// Módulo <b>Identidade</b>: quem é o estudante que está usando o Connexa.
/// </summary>
/// <remarks>
/// <para>
/// <b>Sprint 1 — autenticação simulada.</b> A autenticação real (cadastro, comprovação de e-mail,
/// senha) está fora da Sprint 1. Até lá, a API pública deste módulo (<see cref="IUsuarioAtual"/>)
/// devolve um estudante simulado, configurado em <c>appsettings.json</c>.
/// </para>
/// <para>
/// Os outros módulos dependem apenas de <see cref="IUsuarioAtual"/>. Quando a autenticação real
/// chegar, só a implementação interna deste módulo muda — nenhum outro módulo precisa ser alterado.
/// </para>
/// </remarks>
public static class ModuloIdentidade
{
    public const string Schema = "identidade";

    public static IServiceCollection AdicionarModuloIdentidade(this IServiceCollection servicos)
    {
        servicos.RegistrarSchemaDoModulo(Schema);
        servicos.AdicionarSimulacaoDeUsuario();
        return servicos;
    }

    public static IEndpointRouteBuilder MapearModuloIdentidade(this IEndpointRouteBuilder rotas)
    {
        SessaoSimuladaEndpoints.Mapear(rotas.MapearApiDoModulo(Schema));
        return rotas;
    }
}
