using Connexa.Comum;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Connexa.Materiais;

/// <summary>
/// Módulo <b>Materiais</b>.
/// </summary>
/// <remarks>
/// <para>Materiais compartilhados nos grupos: links, arquivos e anotações.</para>
/// <para>Regras deste módulo (valem para todos):</para>
/// <list type="bullet">
///   <item>Somente os tipos públicos do namespace <c>Connexa.Materiais</c> formam a API do módulo;
///   tudo em <c>Connexa.Materiais.Internal</c> é <c>internal</c> e não pode ser usado por outros módulos.</item>
///   <item>Os dados do módulo ficam no schema <c>materiais</c> do PostgreSQL e só este módulo os acessa.</item>
///   <item>Reações a fatos de outros módulos acontecem por eventos, nunca por acesso direto aos dados.</item>
/// </list>
/// <para>O módulo começa vazio. As fatias verticais são criadas a partir das Tasks do Azure Boards.</para>
/// </remarks>
public static class ModuloMateriais
{
    public const string Schema = "materiais";

    public static IServiceCollection AdicionarModuloMateriais(this IServiceCollection servicos)
    {
        servicos.RegistrarSchemaDoModulo(Schema);

        // Registre aqui os serviços internos do módulo, por exemplo:
        //   servicos.AdicionarContextoDoModulo<MateriaisContexto>(Schema);
        //   servicos.AdicionarTratadorDeEvento<EventoDeOutroModulo, MeuTratador>();
        return servicos;
    }

    public static IEndpointRouteBuilder MapearModuloMateriais(this IEndpointRouteBuilder rotas)
    {
        // Mapeie aqui os endpoints do módulo, por exemplo:
        //   var api = rotas.MapearApiDoModulo(Schema);
        //   MinhaFatiaEndpoints.Mapear(api);
        return rotas;
    }
}