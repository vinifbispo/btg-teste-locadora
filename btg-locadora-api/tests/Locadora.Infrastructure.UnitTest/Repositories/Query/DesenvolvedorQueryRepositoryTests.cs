using Locadora.Infrastructure.UnitTest.TestHelpers;
using Locadora.Domain.Entities;
using Locadora.Infrastructure.Persistence.Repositories.Query;

namespace Locadora.Infrastructure.UnitTest.Repositories.Query;

public class DesenvolvedorQueryRepositoryTests
{
    [Fact]
    public async Task BuscarPorNomeAsync_DeveRetornarApenasDesenvolvedoresQueContenhamOTermoOrdenadosPorNome()
    {
        var dados = new List<Desenvolvedor>
        {
            new() { Id = 1, Nome = "CD Projekt Red" },
            new() { Id = 2, Nome = "BTG Pactual" },
            new() { Id = 3, Nome = "Naughty Dog" }
        };

        var mockContext = TestDbContextFactory.CreateRead();
        mockContext.Object.Desenvolvedores = MockDbSetFactory.Create(dados).Object;
        var repositorio = new DesenvolvedorQueryRepository(mockContext.Object);

        var resultado = await repositorio.BuscarPorNomeAsync("Pactual");

        resultado.Should().ContainSingle().Which.Nome.Should().Be("BTG Pactual");
    }
}
