using Locadora.Application.UnitTest.TestHelpers;
using Locadora.Domain.Entities;
using Locadora.Infrastructure.Persistence.Repositories;

namespace Locadora.Application.UnitTest.Repositories;

public class GeneroRepositoryTests
{
    [Fact]
    public async Task ListarPorIdsAsync_DeveRetornarApenasGenerosComIdsInformados()
    {
        var dados = new List<Genero>
        {
            new() { Id = 1, Nome = "RPG" },
            new() { Id = 2, Nome = "Ação" },
            new() { Id = 3, Nome = "Estratégia" }
        };

        var mockContext = TestDbContextFactory.Create();
        mockContext.Object.Generos = MockDbSetFactory.Create(dados).Object;
        var repositorio = new GeneroRepository(mockContext.Object);

        var resultado = await repositorio.ListarPorIdsAsync([1, 3]);

        resultado.Should().BeEquivalentTo(new[] { dados[0], dados[2] });
    }

    [Fact]
    public async Task ObterOuCriarPorNomesAsync_DeveReutilizarGeneroExistenteIgnorandoCase()
    {
        var dados = new List<Genero> { new() { Id = 1, Nome = "RPG" } };

        var mockContext = TestDbContextFactory.Create();
        mockContext.Object.Generos = MockDbSetFactory.Create(dados).Object;
        var repositorio = new GeneroRepository(mockContext.Object);

        var resultado = await repositorio.ObterOuCriarPorNomesAsync(["rpg"]);

        resultado.Should().ContainSingle().Which.Id.Should().Be(1);
        dados.Should().HaveCount(1);
    }

    [Fact]
    public async Task ObterOuCriarPorNomesAsync_DeveCriarNovoGeneroQuandoNaoExistir()
    {
        var dados = new List<Genero> { new() { Id = 1, Nome = "RPG" } };

        var mockContext = TestDbContextFactory.Create();
        mockContext.Object.Generos = MockDbSetFactory.Create(dados).Object;
        var repositorio = new GeneroRepository(mockContext.Object);

        var resultado = await repositorio.ObterOuCriarPorNomesAsync(["RPG", "Aventura"]);

        resultado.Should().HaveCount(2);
        dados.Should().Contain(g => g.Nome == "Aventura");
    }
}
