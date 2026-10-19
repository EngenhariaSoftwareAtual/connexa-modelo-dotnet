using System.ComponentModel.DataAnnotations;
using Connexa.Comum.Internal;

namespace Connexa.Testes.Comum;

/// <summary>Testes de unidade da validação de entrada usada por todos os módulos.</summary>
[TestClass]
public sealed class ValidacaoDeEntradaTest
{
    private sealed record PedidoPosicional(
        [Required(ErrorMessage = "Informe o nome.")] string? Nome,
        [Range(2, 6, ErrorMessage = "O grupo deve ter de 2 a 6 vagas.")] int Vagas);

    private sealed class PedidoComPropriedades
    {
        [Required(ErrorMessage = "Informe a disciplina.")]
        public string? Disciplina { get; init; }
    }

    [TestMethod(DisplayName = "aceita dados válidos")]
    public void AceitaDadosValidos()
    {
        var campos = new Dictionary<string, string>();

        ValidacaoDeEntrada.Validar(new PedidoPosicional("Cálculo I", 4), campos);

        Assert.IsEmpty(campos);
    }

    [TestMethod(DisplayName = "aponta cada campo inválido de um record posicional, com o nome em camelCase")]
    public void ApontaCamposInvalidosDeRecordPosicional()
    {
        var campos = new Dictionary<string, string>();

        ValidacaoDeEntrada.Validar(new PedidoPosicional(null, 9), campos);

        Assert.AreEqual("Informe o nome.", campos["nome"]);
        Assert.AreEqual("O grupo deve ter de 2 a 6 vagas.", campos["vagas"]);
    }

    [TestMethod(DisplayName = "aponta campo inválido declarado na propriedade")]
    public void ApontaCampoInvalidoDeclaradoNaPropriedade()
    {
        var campos = new Dictionary<string, string>();

        ValidacaoDeEntrada.Validar(new PedidoComPropriedades(), campos);

        Assert.AreEqual("Informe a disciplina.", campos["disciplina"]);
    }
}
