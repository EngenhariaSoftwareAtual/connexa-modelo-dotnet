// Ponto de entrada do back-end do Connexa.
//
// Cada projeto Connexa.<Modulo> é um módulo da aplicação. Aqui eles só são montados:
// registro dos serviços, mapeamento das rotas e preparação do banco.
// As fronteiras entre os módulos são verificadas pelo ArquiteturaModularTest em todo build.

using Connexa.Acompanhamento;
using Connexa.Comum;
using Connexa.Encontros;
using Connexa.Grupos;
using Connexa.Identidade;
using Connexa.Materiais;
using Connexa.Painel;
using Connexa.Perfil;

var construtor = WebApplication.CreateBuilder(args);

construtor.Services
    .AdicionarModuloComum()
    .AdicionarModuloIdentidade()
    .AdicionarModuloPerfil()
    .AdicionarModuloGrupos()
    .AdicionarModuloEncontros()
    .AdicionarModuloAcompanhamento()
    .AdicionarModuloPainel()
    .AdicionarModuloMateriais();

var aplicacao = construtor.Build();

aplicacao.UsarModuloComum();

aplicacao
    .MapearModuloComum()
    .MapearModuloIdentidade()
    .MapearModuloPerfil()
    .MapearModuloGrupos()
    .MapearModuloEncontros()
    .MapearModuloAcompanhamento()
    .MapearModuloPainel()
    .MapearModuloMateriais();

await aplicacao.PrepararBancoDeDadosAsync();

await aplicacao.RunAsync();
