using Connexa.Comum;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Connexa.Perfil;

/// <summary>
/// Módulo <b>Perfil</b>.
/// </summary>
/// <remarks>
/// <para>Perfil acadêmico do estudante: curso, disciplinas de interesse, disponibilidade e preferências de estudo.</para>
/// <para>Regras deste módulo (valem para todos):</para>
/// <list type="bullet">
///   <item>Somente os tipos públicos do namespace <c>Connexa.Perfil</c> formam a API do módulo;
///   tudo em <c>Connexa.Perfil.Internal</c> é <c>internal</c> e não pode ser usado por outros módulos.</item>
///   <item>Os dados do módulo ficam no schema <c>perfil</c> do PostgreSQL e só este módulo os acessa.</item>
///   <item>Reações a fatos de outros módulos acontecem por eventos, nunca por acesso direto aos dados.</item>
/// </list>
/// <para>O módulo começa vazio. As fatias verticais são criadas a partir das Tasks do Azure Boards.</para>
/// </remarks>
public static class ModuloPerfil
{
    public const string Schema = "perfil";

    public static IServiceCollection AdicionarModuloPerfil(this IServiceCollection servicos)
    {
        servicos.RegistrarSchemaDoModulo(Schema);

        // Registre aqui os serviços internos do módulo, por exemplo:
        //   servicos.AdicionarContextoDoModulo<PerfilContexto>(Schema);
        //   servicos.AdicionarTratadorDeEvento<EventoDeOutroModulo, MeuTratador>();
        return servicos;
    }

    public static IEndpointRouteBuilder MapearModuloPerfil(this IEndpointRouteBuilder rotas)
    {
        // Mapeie aqui os endpoints do módulo, por exemplo:
        //   var api = rotas.MapearApiDoModulo(Schema);
        //   MinhaFatiaEndpoints.Mapear(api);
        return rotas;
    }
}