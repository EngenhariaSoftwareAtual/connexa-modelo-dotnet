using Connexa.Comum;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Connexa.Grupos;

/// <summary>
/// Módulo <b>Grupos</b>.
/// </summary>
/// <remarks>
/// <para>Grupos de estudo: criação, busca, entrada e saída de participantes, papéis dentro do grupo.</para>
/// <para>Regras deste módulo (valem para todos):</para>
/// <list type="bullet">
///   <item>Somente os tipos públicos do namespace <c>Connexa.Grupos</c> formam a API do módulo;
///   tudo em <c>Connexa.Grupos.Internal</c> é <c>internal</c> e não pode ser usado por outros módulos.</item>
///   <item>Os dados do módulo ficam no schema <c>grupos</c> do PostgreSQL e só este módulo os acessa.</item>
///   <item>Reações a fatos de outros módulos acontecem por eventos, nunca por acesso direto aos dados.</item>
/// </list>
/// <para>O módulo começa vazio. As fatias verticais são criadas a partir das Tasks do Azure Boards.</para>
/// </remarks>
public static class ModuloGrupos
{
    public const string Schema = "grupos";

    public static IServiceCollection AdicionarModuloGrupos(this IServiceCollection servicos)
    {
        servicos.RegistrarSchemaDoModulo(Schema);

        // Registre aqui os serviços internos do módulo, por exemplo:
        //   servicos.AdicionarContextoDoModulo<GruposContexto>(Schema);
        //   servicos.AdicionarTratadorDeEvento<EventoDeOutroModulo, MeuTratador>();
        return servicos;
    }

    public static IEndpointRouteBuilder MapearModuloGrupos(this IEndpointRouteBuilder rotas)
    {
        // Mapeie aqui os endpoints do módulo, por exemplo:
        //   var api = rotas.MapearApiDoModulo(Schema);
        //   MinhaFatiaEndpoints.Mapear(api);
        return rotas;
    }
}