using Locadora.Application.Queries.Generos.BuscarGenerosPorNome;
using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;

namespace Locadora.Application.UnitTest.Queries.Generos;

public class BuscarGenerosPorNomeQueryHandlerTests
{
    private readonly Mock<IGeneroQueryRepository> _generos = new();

    private BuscarGenerosPorNomeQueryHandler CriarHandler() => new(_generos.Object);

    [Fact]
    public async Task Handle_DeveRetornarGenerosMapeadosParaDto()
    {
        _generos.Setup(r => r.BuscarPorNomeAsync("RPG"))
            .ReturnsAsync([new Genero { Id = 1, Nome = "RPG" }]);

        var handler = CriarHandler();
        var resultado = await handler.Handle(new BuscarGenerosPorNomeQuery("RPG"), CancellationToken.None);

        resultado.Should().ContainSingle().Which.Nome.Should().Be("RPG");
    }
}
