using Connexa.Comum.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Connexa.Comum;

/// <summary>
/// Módulo <b>Comum</b>: infraestrutura compartilhada, <b>sem regra de negócio</b>.
/// </summary>
/// <remarks>
/// <para>
/// Contém o padrão de erros da API (<see cref="ErroDeNegocio"/>), o acesso ao banco
/// (<see cref="BancoDeDados"/>), a validação de entrada (<see cref="RotasDosModulos"/>),
/// o despachante de eventos (<see cref="IDespachanteDeEventos"/>), a consulta de situação
/// do sistema e a documentação OpenAPI.
/// </para>
/// <para>
/// Não coloque aqui entidades, serviços ou regras de nenhum território do Connexa.
/// "Pode ser útil para outros módulos no futuro" não é motivo para algo morar em <c>Comum</c>.
/// </para>
/// </remarks>
public static class ModuloComum
{
    public static IServiceCollection AdicionarModuloComum(this IServiceCollection servicos)
    {
        servicos.AdicionarFonteDeDados();
        servicos.AdicionarTratamentoDeErros();
        servicos.AdicionarDespachanteDeEventos();
        servicos.AdicionarDocumentacaoDaApi();
        return servicos;
    }

    /// <summary>Middlewares comuns: tratamento de erros e documentação da API.</summary>
    public static WebApplication UsarModuloComum(this WebApplication aplicacao)
    {
        aplicacao.UseExceptionHandler();
        aplicacao.UseStatusCodePages();
        aplicacao.UsarDocumentacaoDaApi();
        return aplicacao;
    }

    public static IEndpointRouteBuilder MapearModuloComum(this IEndpointRouteBuilder rotas)
    {
        SituacaoEndpoints.Mapear(rotas);
        rotas.MapearDocumentacaoDaApi();
        return rotas;
    }

    /// <summary>
    /// Cria os schemas de todos os módulos e aplica as migrações de todos os contextos do EF Core.
    /// Executado quando a aplicação sobe, antes de atender requisições.
    /// </summary>
    public static Task PrepararBancoDeDadosAsync(this WebApplication aplicacao) =>
        PreparacaoDoBanco.ExecutarAsync(aplicacao.Services);
}
