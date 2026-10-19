using Microsoft.Extensions.DependencyInjection;

namespace Connexa.Comum.Internal;

/// <summary>
/// Schemas e contextos do EF Core registrados pelos módulos, usados na preparação do banco.
/// </summary>
internal sealed class RegistroDosModulos
{
    private readonly List<string> schemas = [];
    private readonly List<Type> contextos = [];

    public IReadOnlyList<string> Schemas => schemas;

    public IReadOnlyList<Type> Contextos => contextos;

    public void AdicionarSchema(string schema)
    {
        if (!schemas.Contains(schema))
        {
            schemas.Add(schema);
        }
    }

    public void AdicionarContexto(Type contexto)
    {
        if (!contextos.Contains(contexto))
        {
            contextos.Add(contexto);
        }
    }

    /// <summary>Devolve a instância única do registro, criando-a no primeiro uso.</summary>
    public static RegistroDosModulos Obter(IServiceCollection servicos)
    {
        var existente = servicos
            .Select(descritor => descritor.IsKeyedService ? null : descritor.ImplementationInstance)
            .OfType<RegistroDosModulos>()
            .FirstOrDefault();
        if (existente is not null)
        {
            return existente;
        }

        var registro = new RegistroDosModulos();
        servicos.AddSingleton(registro);
        return registro;
    }
}
