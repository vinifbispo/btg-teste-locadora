using Locadora.Application.Services;
using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;

namespace Locadora.Application.UnitTest.Services;

public class GeneroServiceTests
{
    private readonly Mock<IGeneroRepository> _generos = new();

    private GeneroService CriarService() => new(_generos.Object);

    [Fact]
    public async Task BuscarPorNomeAsync_DeveRetornarGenerosMapeadosParaDto()
    {
        _generos.Setup(r => r.BuscarPorNomeAsync("RPG"))
            .ReturnsAsync([new Genero { Id = 1, Nome = "RPG" }]);

        var service = CriarService();
        var resultado = await service.BuscarPorNomeAsync("RPG");

        resultado.Should().ContainSingle().Which.Nome.Should().Be("RPG");
    }
}
