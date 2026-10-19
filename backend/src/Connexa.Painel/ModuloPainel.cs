using Connexa.Comum;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Connexa.Painel;

/// <summary>
/// Módulo <b>Painel</b>.
/// </summary>
/// <remarks>
/// <para>Visão consolidada para o estudante e para a coordenação, montada a partir dos dados publicados pelos outros módulos.</para>
/// <para>Regras deste módulo (valem para todos):</para>
/// <list type="bullet">
///   <item>Somente os tipos públicos do namespace <c>Connexa.Painel</c> formam a API do módulo;
///   tudo em <c>Connexa.Painel.Internal</c> é <c>internal</c> e não pode ser usado por outros módulos.</item>
///   <item>Os dados do módulo ficam no schema <c>painel</c> do PostgreSQL e só este módulo os acessa.</item>
///   <item>Reações a fatos de outros módulos acontecem por eventos, nunca por acesso direto aos dados.</item>
/// </list>
/// <para>O módulo começa vazio. As fatias verticais são criadas a partir das Tasks do Azure Boards.</para>
/// </remarks>
public static class ModuloPainel
{
    public const string Schema = "painel";

    public static IServiceCollection AdicionarModuloPainel(this IServiceCollection servicos)
    {
        servicos.RegistrarSchemaDoModulo(Schema);

        // Registre aqui os serviços internos do módulo, por exemplo:
        //   servicos.AdicionarContextoDoModulo<PainelContexto>(Schema);
        //   servicos.AdicionarTratadorDeEvento<EventoDeOutroModulo, MeuTratador>();
        return servicos;
    }

    public static IEndpointRouteBuilder MapearModuloPainel(this IEndpointRouteBuilder rotas)
    {
        // Mapeie aqui os endpoints do módulo, por exemplo:
        //   var api = rotas.MapearApiDoModulo(Schema);
        //   MinhaFatiaEndpoints.Mapear(api);
        return rotas;
    }
}