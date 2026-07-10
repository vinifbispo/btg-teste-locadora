using Locadora.Application.Dtos;
using Locadora.Application.Commands.Jogos.AtualizarJogo;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.UnitTest.Commands.Jogos;

public class AtualizarJogoCommandHandlerTests
{
    private readonly Mock<IJogoRepository> _jogos = new();
    private readonly Mock<IGeneroRepository> _generos = new();
    private readonly Mock<IDesenvolvedorRepository> _desenvolvedores = new();
    private readonly Mock<IPublicadoraRepository> _publicadoras = new();
    private readonly Mock<IJogoCache> _cache = new();

    private AtualizarJogoCommandHandler CriarHandler() => new(
        _jogos.Object,
        _generos.Object,
        _desenvolvedores.Object,
        _publicadoras.Object,
        _cache.Object,
        new Mock<ILogger<AtualizarJogoCommandHandler>>().Object);

    [Fact]
    public async Task Handle_DeveRetornarFalseQuandoJogoNaoEncontrado()
    {
        _jogos.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Jogo?)null);

        var handler = CriarHandler();
        var resultado = await handler.Handle(new AtualizarJogoCommand(1, new JogoInputDto()), CancellationToken.None);

        resultado.Should().BeFalse();
    }
}
