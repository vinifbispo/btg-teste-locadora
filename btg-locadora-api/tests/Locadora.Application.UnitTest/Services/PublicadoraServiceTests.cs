using Locadora.Application.Services;
using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;

namespace Locadora.Application.UnitTest.Services;

public class PublicadoraServiceTests
{
    private readonly Mock<IPublicadoraRepository> _publicadoras = new();

    private PublicadoraService CriarService() => new(_publicadoras.Object);

    [Fact]
    public async Task BuscarPorNomeAsync_DeveRetornarPublicadorasMapeadasParaDto()
    {
        _publicadoras.Setup(r => r.BuscarPorNomeAsync("Son"))
            .ReturnsAsync([new Publicadora { Id = 1, Nome = "Sony" }]);

        var service = CriarService();
        var resultado = await service.BuscarPorNomeAsync("Son");

        resultado.Should().ContainSingle().Which.Nome.Should().Be("Sony");
    }
}
