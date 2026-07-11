using Locadora.Application.UnitTest.TestHelpers;
using Locadora.Domain.Entities;
using Locadora.Infrastructure.Persistence.Repositories.Query;

namespace Locadora.Application.UnitTest.Repositories.Query;

public class AmigoQueryRepositoryTests
{
    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarAmigoQuandoExistir()
    {
        var dados = new List<Amigo>
        {
            new() { Id = 1, Nome = "Vinicius", Sobrenome = "Bispo" },
            new() { Id = 2, Nome = "Walanem", Sobrenome = "Figueiredo" }
        };

        var mockContext = TestDbContextFactory.CreateRead();
        mockContext.Object.Amigos = MockDbSetFactory.Create(dados).Object;
        var repositorio = new AmigoQueryRepository(mockContext.Object);

        var resultado = await repositorio.ObterPorIdAsync(2);

        resultado.Should().NotBeNull();
        resultado!.Nome.Should().Be("Walanem");
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarNuloQuandoNaoExistir()
    {
        var dados = new List<Amigo> { new() { Id = 1, Nome = "Vinicius", Sobrenome = "Bispo" } };

        var mockContext = TestDbContextFactory.CreateRead();
        mockContext.Object.Amigos = MockDbSetFactory.Create(dados).Object;
        var repositorio = new AmigoQueryRepository(mockContext.Object);

        var resultado = await repositorio.ObterPorIdAsync(999);

        resultado.Should().BeNull();
    }
}
