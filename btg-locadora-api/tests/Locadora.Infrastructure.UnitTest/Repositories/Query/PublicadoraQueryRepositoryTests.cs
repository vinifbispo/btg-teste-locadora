using Locadora.Infrastructure.UnitTest.TestHelpers;
using Locadora.Domain.Entities;
using Locadora.Infrastructure.Persistence.Repositories.Query;

namespace Locadora.Infrastructure.UnitTest.Repositories.Query;

public class PublicadoraQueryRepositoryTests
{
    [Fact]
    public async Task BuscarPorNomeAsync_DeveRetornarApenasPublicadorasQueContenhamOTermoOrdenadasPorNome()
    {
        var dados = new List<Publicadora>
        {
            new() { Id = 1, Nome = "Nintendo" },
            new() { Id = 2, Nome = "Microsoft" },
            new() { Id = 3, Nome = "Sony" }
        };

        var mockContext = TestDbContextFactory.CreateRead();
        mockContext.Object.Publicadoras = MockDbSetFactory.Create(dados).Object;
        var repositorio = new PublicadoraQueryRepository(mockContext.Object);

        var resultado = await repositorio.BuscarPorNomeAsync("cro");

        resultado.Should().ContainSingle().Which.Nome.Should().Be("Microsoft");
    }
}
