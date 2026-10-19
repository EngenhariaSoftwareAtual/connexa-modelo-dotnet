using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Connexa.Identidade.Internal;

/// <summary>
/// Endpoints de apoio à simulação: qual estudante está "logado" e quais podem ser escolhidos.
/// O front-end usa estes endpoints no seletor de estudante simulado.
/// </summary>
internal static class SessaoSimuladaEndpoints
{
    public static void Mapear(IEndpointRouteBuilder api)
    {
        api.MapGet("/sessao", (UsuarioSimulado usuario) => Sessao.De(usuario.EstudanteDaRequisicao()))
            .Produces<Sessao>();

        api.MapGet("/usuarios-simulados", (UsuarioSimulado usuario) => usuario.Todos().Select(Sessao.De).ToList())
            .Produces<List<Sessao>>();
    }

    internal sealed record Sessao(string Chave, Guid Id, string Nome, string Email, bool Simulado)
    {
        public static Sessao De(EstudanteSimulado estudante) =>
            new(estudante.Chave, estudante.Id, estudante.Nome, estudante.Email, Simulado: true);
    }
}
