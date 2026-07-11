using Locadora.Infrastructure.UnitTest.TestHelpers;
using Locadora.Domain.Entities;
using Locadora.Infrastructure.Persistence.Repositories.Query;

namespace Locadora.Infrastructure.UnitTest.Repositories.Query;

public class GeneroQueryRepositoryTests
{
    [Fact]
    public async Task BuscarPorNomeAsync_DeveRetornarApenasGenerosQueContenhamOTermoOrdenadosPorNome()
    {
        var dados = new List<Genero>
        {
            new() { Id = 1, Nome = "RPG" },
            new() { Id = 2, Nome = "Ação" },
            new() { Id = 3, Nome = "Estratégia" }
        };

        var mockContext = TestDbContextFactory.CreateRead();
        mockContext.Object.Generos = MockDbSetFactory.Create(dados).Object;
        var repositorio = new GeneroQueryRepository(mockContext.Object);

        var resultado = await repositorio.BuscarPorNomeAsync("tra");

        resultado.Should().ContainSingle().Which.Nome.Should().Be("Estratégia");
    }
}
