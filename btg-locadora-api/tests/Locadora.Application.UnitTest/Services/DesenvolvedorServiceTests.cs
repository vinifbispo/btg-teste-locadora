using Locadora.Application.Services;
using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;

namespace Locadora.Application.UnitTest.Services;

public class DesenvolvedorServiceTests
{
    private readonly Mock<IDesenvolvedorRepository> _desenvolvedores = new();

    private DesenvolvedorService CriarService() => new(_desenvolvedores.Object);

    [Fact]
    public async Task BuscarPorNomeAsync_DeveRetornarDesenvolvedoresMapeadosParaDto()
    {
        _desenvolvedores.Setup(r => r.BuscarPorNomeAsync("BTG"))
            .ReturnsAsync([new Desenvolvedor { Id = 1, Nome = "BTG Pactual" }]);

        var service = CriarService();
        var resultado = await service.BuscarPorNomeAsync("BTG");

        resultado.Should().ContainSingle().Which.Nome.Should().Be("BTG Pactual");
    }
}
