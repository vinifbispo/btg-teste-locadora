using Locadora.Infrastructure.UnitTest.TestHelpers;
using Locadora.Domain.Entities;
using Locadora.Infrastructure.Persistence.Repositories.Command;

namespace Locadora.Infrastructure.UnitTest.Repositories.Command;

public class PublicadoraRepositoryTests
{
    [Fact]
    public async Task ListarPorIdsAsync_DeveRetornarApenasPublicadorasComIdsInformados()
    {
        var dados = new List<Publicadora>
        {
            new() { Id = 1, Nome = "Sony" },
            new() { Id = 2, Nome = "Microsoft" },
            new() { Id = 3, Nome = "Nintendo" }
        };

        var mockContext = TestDbContextFactory.Create();
        mockContext.Object.Publicadoras = MockDbSetFactory.Create(dados).Object;
        var repositorio = new PublicadoraRepository(mockContext.Object);

        var resultado = await repositorio.ListarPorIdsAsync([1, 3]);

        resultado.Should().BeEquivalentTo(new[] { dados[0], dados[2] });
    }

    [Fact]
    public async Task ObterOuCriarPorNomesAsync_DeveReutilizarPublicadoraExistenteIgnorandoCase()
    {
        var dados = new List<Publicadora> { new() { Id = 1, Nome = "Sony" } };

        var mockContext = TestDbContextFactory.Create();
        mockContext.Object.Publicadoras = MockDbSetFactory.Create(dados).Object;
        var repositorio = new PublicadoraRepository(mockContext.Object);

        var resultado = await repositorio.ObterOuCriarPorNomesAsync(["sony"]);

        resultado.Should().ContainSingle().Which.Id.Should().Be(1);
        dados.Should().HaveCount(1);
    }

    [Fact]
    public async Task ObterOuCriarPorNomesAsync_DeveCriarNovaPublicadoraQuandoNaoExistir()
    {
        var dados = new List<Publicadora> { new() { Id = 1, Nome = "Sony" } };

        var mockContext = TestDbContextFactory.Create();
        mockContext.Object.Publicadoras = MockDbSetFactory.Create(dados).Object;
        var repositorio = new PublicadoraRepository(mockContext.Object);

        var resultado = await repositorio.ObterOuCriarPorNomesAsync(["Sony", "Ubisoft"]);

        resultado.Should().HaveCount(2);
        dados.Should().Contain(p => p.Nome == "Ubisoft");
    }
}
