using Locadora.Infrastructure.UnitTest.TestHelpers;
using Locadora.Domain.Entities;
using Locadora.Infrastructure.Persistence.Repositories.Command;

namespace Locadora.Infrastructure.UnitTest.Repositories.Command;

public class AmigoRepositoryTests
{
    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarAmigoQuandoExistir()
    {
        var dados = new List<Amigo> { new() { Id = 1, Nome = "Walanem", Sobrenome = "Figueiredo" } };

        var mockContext = TestDbContextFactory.Create();
        mockContext.Object.Amigos = MockDbSetFactory.Create(dados).Object;
        var repositorio = new AmigoRepository(mockContext.Object);

        var resultado = await repositorio.ObterPorIdAsync(1);

        resultado.Should().NotBeNull();
        resultado!.Nome.Should().Be("Walanem");
    }

    [Fact]
    public async Task ExisteAsync_DeveRetornarFalseQuandoAmigoNaoExiste()
    {
        var dados = new List<Amigo> { new() { Id = 1, Nome = "Walanem", Sobrenome = "Figueiredo" } };

        var mockContext = TestDbContextFactory.Create();
        mockContext.Object.Amigos = MockDbSetFactory.Create(dados).Object;
        var repositorio = new AmigoRepository(mockContext.Object);

        var existe = await repositorio.ExisteAsync(999);

        existe.Should().BeFalse();
    }

    [Fact]
    public async Task AdicionarAsync_DeveAdicionarAmigoNaLista()
    {
        var dados = new List<Amigo>();

        var mockContext = TestDbContextFactory.Create();
        mockContext.Object.Amigos = MockDbSetFactory.Create(dados).Object;
        var repositorio = new AmigoRepository(mockContext.Object);

        await repositorio.AdicionarAsync(new Amigo { Nome = "Vinicius", Sobrenome = "Bispo" });

        dados.Should().ContainSingle().Which.Nome.Should().Be("Vinicius");
    }
}
