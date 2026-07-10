using Locadora.Application.UnitTest.TestHelpers;
using Locadora.Domain.Entities;
using Locadora.Infrastructure.Persistence.Repositories.Query;

namespace Locadora.Application.UnitTest.Repositories.Query;

public class JogoQueryRepositoryTests
{
    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarJogoQuandoExistir()
    {
        var dados = new List<Jogo>
        {
            new() { Id = 1, Nome = "EA FC 26" },
            new() { Id = 2, Nome = "GTA VI" }
        };

        var mockContext = TestDbContextFactory.CreateRead();
        mockContext.Object.Jogos = MockDbSetFactory.Create(dados).Object;
        var repositorio = new JogoQueryRepository(mockContext.Object);

        var resultado = await repositorio.ObterPorIdAsync(2);

        resultado.Should().NotBeNull();
        resultado!.Nome.Should().Be("GTA VI");
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarNuloQuandoNaoExistir()
    {
        var dados = new List<Jogo> { new() { Id = 1, Nome = "EA FC 26" } };

        var mockContext = TestDbContextFactory.CreateRead();
        mockContext.Object.Jogos = MockDbSetFactory.Create(dados).Object;
        var repositorio = new JogoQueryRepository(mockContext.Object);

        var resultado = await repositorio.ObterPorIdAsync(999);

        resultado.Should().BeNull();
    }
}
