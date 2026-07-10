using Locadora.Application.Queries.Desenvolvedores.BuscarDesenvolvedoresPorNome;
using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;

namespace Locadora.Application.UnitTest.Queries.Desenvolvedores;

public class BuscarDesenvolvedoresPorNomeQueryHandlerTests
{
    private readonly Mock<IDesenvolvedorQueryRepository> _desenvolvedores = new();

    private BuscarDesenvolvedoresPorNomeQueryHandler CriarHandler() => new(_desenvolvedores.Object);

    [Fact]
    public async Task Handle_DeveRetornarDesenvolvedoresMapeadosParaDto()
    {
        _desenvolvedores.Setup(r => r.BuscarPorNomeAsync("BTG"))
            .ReturnsAsync([new Desenvolvedor { Id = 1, Nome = "BTG Pactual" }]);

        var handler = CriarHandler();
        var resultado = await handler.Handle(new BuscarDesenvolvedoresPorNomeQuery("BTG"), CancellationToken.None);

        resultado.Should().ContainSingle().Which.Nome.Should().Be("BTG Pactual");
    }
}
