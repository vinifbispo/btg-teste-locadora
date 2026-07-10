using Locadora.Application.UnitTest.TestHelpers;
using Locadora.Domain.Entities;
using Locadora.Infrastructure.Persistence.Repositories.Command;

namespace Locadora.Application.UnitTest.Repositories.Command;

public class JogoRepositoryTests
{
    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarJogoQuandoExistir()
    {
        var dados = new List<Jogo>
        {
            new() { Id = 1, Nome = "EA FC 26" },
            new() { Id = 2, Nome = "GTA VI" }
        };

        var mockContext = TestDbContextFactory.Create();
        mockContext.Object.Jogos = MockDbSetFactory.Create(dados).Object;
        var repositorio = new JogoRepository(mockContext.Object);

        var resultado = await repositorio.ObterPorIdAsync(2);

        resultado.Should().NotBeNull();
        resultado!.Nome.Should().Be("GTA VI");
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarNuloQuandoNaoExistir()
    {
        var dados = new List<Jogo> { new() { Id = 1, Nome = "EA FC 26" } };

        var mockContext = TestDbContextFactory.Create();
        mockContext.Object.Jogos = MockDbSetFactory.Create(dados).Object;
        var repositorio = new JogoRepository(mockContext.Object);

        var resultado = await repositorio.ObterPorIdAsync(999);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task AdicionarAsync_DeveAdicionarJogoNaLista()
    {
        var dados = new List<Jogo>();

        var mockContext = TestDbContextFactory.Create();
        mockContext.Object.Jogos = MockDbSetFactory.Create(dados).Object;
        var repositorio = new JogoRepository(mockContext.Object);

        var jogo = new Jogo { Nome = "EA FC 26" };
        await repositorio.AdicionarAsync(jogo);

        dados.Should().ContainSingle().Which.Nome.Should().Be("EA FC 26");
    }

    [Fact]
    public async Task RemoverAsync_DeveRemoverJogoDaLista()
    {
        var jogo = new Jogo { Id = 1, Nome = "EA FC 26" };
        var dados = new List<Jogo> { jogo };

        var mockContext = TestDbContextFactory.Create();
        mockContext.Object.Jogos = MockDbSetFactory.Create(dados).Object;
        var repositorio = new JogoRepository(mockContext.Object);

        await repositorio.RemoverAsync(jogo);

        dados.Should().BeEmpty();
    }
}
