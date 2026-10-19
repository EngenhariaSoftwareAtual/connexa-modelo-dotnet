using Connexa.Comum;
using Microsoft.Extensions.DependencyInjection;

namespace Connexa.Testes.Comum;

/// <summary>Testes de unidade do despachante de eventos em processo.</summary>
[TestClass]
public sealed class DespachanteDeEventosTest
{
    private sealed record GrupoCriado(Guid GrupoId);

    private sealed class Anotador : ITratadorDeEvento<GrupoCriado>
    {
        public List<Guid> Recebidos { get; } = [];

        public Task TratarAsync(GrupoCriado evento, CancellationToken cancelamento)
        {
            Recebidos.Add(evento.GrupoId);
            return Task.CompletedTask;
        }
    }

    [TestMethod(DisplayName = "entrega o evento a todos os tratadores registrados")]
    public async Task EntregaOEventoATodosOsTratadores()
    {
        var primeiro = new Anotador();
        var segundo = new Anotador();
        var servicos = new ServiceCollection().AdicionarModuloComum();
        servicos.AddScoped<ITratadorDeEvento<GrupoCriado>>(_ => primeiro);
        servicos.AddScoped<ITratadorDeEvento<GrupoCriado>>(_ => segundo);
        await using var provedor = servicos.BuildServiceProvider();
        await using var escopo = provedor.CreateAsyncScope();
        var grupo = Guid.NewGuid();

        await escopo.ServiceProvider.GetRequiredService<IDespachanteDeEventos>().PublicarAsync(new GrupoCriado(grupo));

        CollectionAssert.AreEqual(new[] { grupo }, primeiro.Recebidos);
        CollectionAssert.AreEqual(new[] { grupo }, segundo.Recebidos);
    }

    [TestMethod(DisplayName = "publicar sem tratadores não é erro")]
    public async Task PublicarSemTratadoresNaoEErro()
    {
        await using var provedor = new ServiceCollection().AdicionarModuloComum().BuildServiceProvider();
        await using var escopo = provedor.CreateAsyncScope();

        await escopo.ServiceProvider.GetRequiredService<IDespachanteDeEventos>().PublicarAsync(new GrupoCriado(Guid.NewGuid()));
    }
}
