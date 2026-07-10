using Locadora.Application.Dtos;
using Locadora.Application.Commands.Amigos.AtualizarAmigo;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.UnitTest.Commands.Amigos;

public class AtualizarAmigoCommandHandlerTests
{
    private readonly Mock<IAmigoRepository> _amigos = new();
    private readonly Mock<IAmigoCache> _cache = new();

    private AtualizarAmigoCommandHandler CriarHandler() => new(_amigos.Object, _cache.Object, new Mock<ILogger<AtualizarAmigoCommandHandler>>().Object);

    [Fact]
    public async Task Handle_DeveRetornarFalseQuandoNaoEncontrado()
    {
        _amigos.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Amigo?)null);

        var handler = CriarHandler();
        var resultado = await handler.Handle(new AtualizarAmigoCommand(1, new AmigoInputDto()), CancellationToken.None);

        resultado.Should().BeFalse();
    }
}
