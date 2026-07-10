using Locadora.Application.Queries.Publicadoras.BuscarPublicadorasPorNome;
using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;

namespace Locadora.Application.UnitTest.Queries.Publicadoras;

public class BuscarPublicadorasPorNomeQueryHandlerTests
{
    private readonly Mock<IPublicadoraRepository> _publicadoras = new();

    private BuscarPublicadorasPorNomeQueryHandler CriarHandler() => new(_publicadoras.Object);

    [Fact]
    public async Task Handle_DeveRetornarPublicadorasMapeadasParaDto()
    {
        _publicadoras.Setup(r => r.BuscarPorNomeAsync("Son"))
            .ReturnsAsync([new Publicadora { Id = 1, Nome = "Sony" }]);

        var handler = CriarHandler();
        var resultado = await handler.Handle(new BuscarPublicadorasPorNomeQuery("Son"), CancellationToken.None);

        resultado.Should().ContainSingle().Which.Nome.Should().Be("Sony");
    }
}
