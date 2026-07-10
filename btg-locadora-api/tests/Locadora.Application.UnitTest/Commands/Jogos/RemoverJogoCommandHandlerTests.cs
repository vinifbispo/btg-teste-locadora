using Locadora.Application.Commands.Jogos.RemoverJogo;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Exceptions;
using Locadora.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.UnitTest.Commands.Jogos;

public class RemoverJogoCommandHandlerTests
{
    private readonly Mock<IJogoRepository> _jogos = new();
    private readonly Mock<IEmprestimoRepository> _emprestimos = new();
    private readonly Mock<IJogoCache> _cache = new();

    private RemoverJogoCommandHandler CriarHandler() => new(
        _jogos.Object,
        _emprestimos.Object,
        _cache.Object,
        new Mock<ILogger<RemoverJogoCommandHandler>>().Object);

    [Fact]
    public async Task Handle_DeveRemoverQuandoExistir()
    {
        var jogo = new Jogo { Id = 1, Nome = "GTA VI" };
        _jogos.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(jogo);
        _emprestimos.Setup(r => r.ExisteParaJogoAsync(1)).ReturnsAsync(false);

        var handler = CriarHandler();
        var resultado = await handler.Handle(new RemoverJogoCommand(1), CancellationToken.None);

        resultado.Should().BeTrue();
        _jogos.Verify(r => r.RemoverAsync(jogo), Times.Once);
        _cache.Verify(c => c.RemoverAsync(1), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveLancarConflitoExceptionQuandoJogoPossuiEmprestimos()
    {
        var jogo = new Jogo { Id = 1, Nome = "GTA VI" };
        _jogos.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(jogo);
        _emprestimos.Setup(r => r.ExisteParaJogoAsync(1)).ReturnsAsync(true);

        var handler = CriarHandler();

        await FluentActions.Awaiting(() => handler.Handle(new RemoverJogoCommand(1), CancellationToken.None))
            .Should().ThrowAsync<ConflitoException>();

        _jogos.Verify(r => r.RemoverAsync(It.IsAny<Jogo>()), Times.Never);
    }
}
