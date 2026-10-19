using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Connexa.Comum.Internal;

/// <summary>
/// Valida os objetos recebidos por uma operação com os atributos de
/// <c>System.ComponentModel.DataAnnotations</c> (<c>[Required]</c>, <c>[StringLength]</c>, <c>[Range]</c>...).
/// </summary>
/// <remarks>
/// Os atributos podem estar nas propriedades ou nos parâmetros de um <c>record</c> posicional.
/// Se algum campo for inválido, a operação não é executada e a resposta é 400 com
/// <c>codigo: DADOS_INVALIDOS</c> e o mapa <c>campos</c>, com a primeira mensagem de cada campo.
/// </remarks>
internal sealed class ValidacaoDeEntrada : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext contexto, EndpointFilterDelegate proximo)
    {
        var campos = new Dictionary<string, string>();
        foreach (var argumento in contexto.Arguments)
        {
            if (argumento is not null && DeveSerValidado(argumento.GetType()))
            {
                Validar(argumento, campos);
            }
        }

        if (campos.Count == 0)
        {
            return await proximo(contexto);
        }

        var problema = TratamentoDeErros.Problema(
            StatusCodes.Status400BadRequest, "Um ou mais campos estão inválidos.", TratamentoDeErros.DadosInvalidos);
        problema.Extensions["campos"] = campos;
        return TypedResults.Problem(problema);
    }

    internal static void Validar(object objeto, IDictionary<string, string> campos)
    {
        var tipo = objeto.GetType();
        var parametros = tipo.GetConstructors()
            .SelectMany(construtor => construtor.GetParameters())
            .Where(parametro => parametro.Name is not null)
            .GroupBy(parametro => parametro.Name!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(grupo => grupo.Key, grupo => grupo.First(), StringComparer.OrdinalIgnoreCase);

        foreach (var propriedade in tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var atributos = propriedade.GetCustomAttributes<ValidationAttribute>(inherit: true)
                .Concat(parametros.TryGetValue(propriedade.Name, out var parametro)
                    ? parametro.GetCustomAttributes<ValidationAttribute>()
                    : []);

            var valor = propriedade.GetValue(objeto);
            var nome = JsonNamingPolicy.CamelCase.ConvertName(propriedade.Name);
            var contextoDaValidacao = new ValidationContext(objeto) { MemberName = propriedade.Name, DisplayName = nome };

            foreach (var atributo in atributos)
            {
                var resultado = atributo.GetValidationResult(valor, contextoDaValidacao);
                if (resultado != ValidationResult.Success && !campos.ContainsKey(nome))
                {
                    campos[nome] = resultado?.ErrorMessage ?? "Valor inválido.";
                }
            }
        }
    }

    private static bool DeveSerValidado(Type tipo) =>
        tipo.Assembly.GetName().Name?.StartsWith("Connexa.", StringComparison.Ordinal) == true
        && !tipo.IsPrimitive
        && !tipo.IsEnum;
}
