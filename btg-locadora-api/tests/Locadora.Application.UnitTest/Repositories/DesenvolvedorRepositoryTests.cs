using Locadora.Application.UnitTest.TestHelpers;
using Locadora.Domain.Entities;
using Locadora.Infrastructure.Persistence.Repositories;

namespace Locadora.Application.UnitTest.Repositories;

public class DesenvolvedorRepositoryTests
{
    [Fact]
    public async Task ListarPorIdsAsync_DeveRetornarApenasDesenvolvedoresComIdsInformados()
    {
        var dados = new List<Desenvolvedor>
        {
            new() { Id = 1, Nome = "BTG Pactual" },
            new() { Id = 2, Nome = "BTG Empresas" },
            new() { Id = 3, Nome = "BTG Investimentos" }
        };

        var mockContext = TestDbContextFactory.Create();
        mockContext.Object.Desenvolvedores = MockDbSetFactory.Create(dados).Object;
        var repositorio = new DesenvolvedorRepository(mockContext.Object);

        var resultado = await repositorio.ListarPorIdsAsync([1, 3]);

        resultado.Should().BeEquivalentTo(new[] { dados[0], dados[2] });
    }

    [Fact]
    public async Task ObterOuCriarPorNomesAsync_DeveReutilizarDesenvolvedorExistenteIgnorandoCase()
    {
        var dados = new List<Desenvolvedor> { new() { Id = 1, Nome = "BTG Pactual" } };

        var mockContext = TestDbContextFactory.Create();
        mockContext.Object.Desenvolvedores = MockDbSetFactory.Create(dados).Object;
        var repositorio = new DesenvolvedorRepository(mockContext.Object);

        var resultado = await repositorio.ObterOuCriarPorNomesAsync(["btg pactual"]);

        resultado.Should().ContainSingle().Which.Id.Should().Be(1);
        dados.Should().HaveCount(1);
    }

    [Fact]
    public async Task ObterOuCriarPorNomesAsync_DeveCriarNovoDesenvolvedorQuandoNaoExistir()
    {
        var dados = new List<Desenvolvedor> { new() { Id = 1, Nome = "BTG Pactual" } };

        var mockContext = TestDbContextFactory.Create();
        mockContext.Object.Desenvolvedores = MockDbSetFactory.Create(dados).Object;
        var repositorio = new DesenvolvedorRepository(mockContext.Object);

        var resultado = await repositorio.ObterOuCriarPorNomesAsync(["BTG Pactual", "BTG Empresas"]);

        resultado.Should().HaveCount(2);
        dados.Should().Contain(d => d.Nome == "BTG Empresas");
    }

    [Fact]
    public async Task BuscarPorNomeAsync_DeveRetornarApenasDesenvolvedoresQueContenhamOTermoOrdenadosPorNome()
    {
        var dados = new List<Desenvolvedor>
        {
            new() { Id = 1, Nome = "CD Projekt Red" },
            new() { Id = 2, Nome = "BTG Pactual" },
            new() { Id = 3, Nome = "Naughty Dog" }
        };

        var mockContext = TestDbContextFactory.Create();
        mockContext.Object.Desenvolvedores = MockDbSetFactory.Create(dados).Object;
        var repositorio = new DesenvolvedorRepository(mockContext.Object);

        var resultado = await repositorio.BuscarPorNomeAsync("Pactual");

        resultado.Should().ContainSingle().Which.Nome.Should().Be("BTG Pactual");
    }
}
