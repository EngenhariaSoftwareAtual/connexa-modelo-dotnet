using Microsoft.Extensions.DependencyInjection;

namespace Connexa.Comum.Internal;

/// <summary>
/// Despachante de eventos em processo: entrega cada evento aos tratadores registrados,
/// um de cada vez, na mesma requisição.
/// </summary>
internal sealed class DespachanteDeEventos(IServiceProvider provedor) : IDespachanteDeEventos
{
    public async Task PublicarAsync<TEvento>(TEvento evento, CancellationToken cancelamento = default)
        where TEvento : notnull
    {
        foreach (var tratador in provedor.GetServices<ITratadorDeEvento<TEvento>>())
        {
            await tratador.TratarAsync(evento, cancelamento);
        }
    }
}
