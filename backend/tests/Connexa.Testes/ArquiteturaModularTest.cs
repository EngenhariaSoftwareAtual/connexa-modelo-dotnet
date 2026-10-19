using System.Reflection;
using Connexa.Acompanhamento;
using Connexa.Comum;
using Connexa.Encontros;
using Connexa.Grupos;
using Connexa.Identidade;
using Connexa.Materiais;
using Connexa.Painel;
using Connexa.Perfil;
using Microsoft.EntityFrameworkCore.Migrations;
using NetArchTest.Rules;

namespace Connexa.Testes;

/// <summary>
/// Verificação automática das fronteiras entre módulos.
/// </summary>
/// <remarks>
/// <para>
/// O compilador já impede que um módulo use classes <c>internal</c> de outro. Este teste confere o resto:
/// tipos públicos esquecidos em <c>Internal</c>, uso do <c>Internal</c> de outro módulo e dependências
/// proibidas. Dependência circular entre módulos é impedida pelo próprio build: projetos não podem
/// referenciar uns aos outros em ciclo.
/// </para>
/// <para><b>Nunca desative, ignore ou altere este teste para o build passar.</b>
/// Se ele falhar, a mudança precisa ser refeita respeitando as fronteiras.</para>
/// </remarks>
[TestClass]
public sealed class ArquiteturaModularTest
{
    private static readonly Dictionary<string, Assembly> ModulosDeNegocio = new()
    {
        ["Identidade"] = typeof(ModuloIdentidade).Assembly,
        ["Perfil"] = typeof(ModuloPerfil).Assembly,
        ["Grupos"] = typeof(ModuloGrupos).Assembly,
        ["Encontros"] = typeof(ModuloEncontros).Assembly,
        ["Acompanhamento"] = typeof(ModuloAcompanhamento).Assembly,
        ["Painel"] = typeof(ModuloPainel).Assembly,
        ["Materiais"] = typeof(ModuloMateriais).Assembly,
    };

    private static readonly Assembly Comum = typeof(ModuloComum).Assembly;

    private static IEnumerable<KeyValuePair<string, Assembly>> TodosOsModulos =>
        ModulosDeNegocio.Append(new("Comum", Comum));

    [TestMethod(DisplayName = "tipos em Internal não são públicos (exceto migrações do EF Core)")]
    public void TiposInternosNaoSaoPublicos()
    {
        foreach (var (nome, modulo) in TodosOsModulos)
        {
            var resultado = Types.InAssembly(modulo)
                .That().ResideInNamespace($"Connexa.{nome}.Internal")
                .And().DoNotInherit(typeof(Migration))
                .Should().NotBePublic()
                .GetResult();

            Verificar(resultado, $"Em Connexa.{nome}.Internal, crie as classes como internal");
        }
    }

    [TestMethod(DisplayName = "nenhum módulo usa o Internal de outro módulo")]
    public void NenhumModuloUsaOInternalDeOutro()
    {
        foreach (var (nome, modulo) in TodosOsModulos)
        {
            var internosDosOutros = TodosOsModulos
                .Where(outro => outro.Key != nome)
                .Select(outro => $"Connexa.{outro.Key}.Internal")
                .ToArray();

            var resultado = Types.InAssembly(modulo)
                .ShouldNot().HaveDependencyOnAny(internosDosOutros)
                .GetResult();

            Verificar(resultado, $"O módulo {nome} usa tipos internos de outro módulo");
        }
    }

    [TestMethod(DisplayName = "o módulo Comum não depende de nenhum módulo de negócio")]
    public void ComumNaoDependeDosModulosDeNegocio()
    {
        var resultado = Types.InAssembly(Comum)
            .ShouldNot().HaveDependencyOnAny(ModulosDeNegocio.Keys.Select(nome => $"Connexa.{nome}").ToArray())
            .GetResult();

        Verificar(resultado, "O módulo Comum não pode conter regra de negócio nem usar outros módulos");
    }

    [TestMethod(DisplayName = "nenhum módulo depende do projeto de entrada (Connexa.Api)")]
    public void NenhumModuloDependeDaApi()
    {
        foreach (var (nome, modulo) in TodosOsModulos)
        {
            var referenciaAApi = modulo.GetReferencedAssemblies().Any(referencia => referencia.Name == "Connexa.Api");
            Assert.IsFalse(referenciaAApi, $"O módulo {nome} não pode referenciar Connexa.Api.");
        }
    }

    [TestMethod(DisplayName = "cada módulo tem um projeto próprio, com o nome Connexa.<Modulo>")]
    public void CadaModuloTemUmProjetoProprio()
    {
        foreach (var (nome, modulo) in TodosOsModulos)
        {
            Assert.AreEqual($"Connexa.{nome}", modulo.GetName().Name);
        }

        Assert.HasCount(ModulosDeNegocio.Count, ModulosDeNegocio.Values.Distinct());
    }

    private static void Verificar(NetArchTest.Rules.TestResult resultado, string mensagem)
    {
        if (!resultado.IsSuccessful)
        {
            Assert.Fail($"{mensagem}: {string.Join(", ", resultado.FailingTypeNames ?? [])}");
        }
    }
}
