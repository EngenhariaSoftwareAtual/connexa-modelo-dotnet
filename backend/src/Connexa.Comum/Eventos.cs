using Connexa.Comum.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Connexa.Comum;

/// <summary>
/// Publica um evento para os módulos interessados, no mesmo processo.
/// </summary>
/// <remarks>
/// <para>
/// O módulo que publica define o evento como um <c>record</c> público no seu namespace raiz
/// (por exemplo, <c>Connexa.Grupos.EstudanteEntrouNoGrupo</c>) e o publica depois de salvar
/// os próprios dados. Os módulos interessados reagem com um <see cref="ITratadorDeEvento{TEvento}"/>.
/// </para>
/// <para>
/// Assim, um módulo reage ao que aconteceu em outro sem acessar os dados dele.
/// </para>
/// </remarks>
public interface IDespachanteDeEventos
{
    /// <summary>Entrega o evento a todos os tratadores registrados para o seu tipo, um de cada vez.</summary>
    Task PublicarAsync<TEvento>(TEvento evento, CancellationToken cancelamento = default)
        where TEvento : notnull;
}

/// <summary>Reação de um módulo a um evento publicado por outro.</summary>
public interface ITratadorDeEvento<in TEvento>
{
    Task TratarAsync(TEvento evento, CancellationToken cancelamento);
}

/// <summary>Registro dos tratadores de eventos.</summary>
public static class Eventos
{
    /// <example>
    /// <code>servicos.AdicionarTratadorDeEvento&lt;EstudanteEntrouNoGrupo, AtualizaAtividadeDoGrupo&gt;();</code>
    /// </example>
    public static IServiceCollection AdicionarTratadorDeEvento<TEvento, TTratador>(this IServiceCollection servicos)
        where TTratador : class, ITratadorDeEvento<TEvento>
    {
        servicos.AddScoped<ITratadorDeEvento<TEvento>, TTratador>();
        return servicos;
    }

    internal static IServiceCollection AdicionarDespachanteDeEventos(this IServiceCollection servicos)
    {
        servicos.AddScoped<IDespachanteDeEventos, DespachanteDeEventos>();
        return servicos;
    }
}
