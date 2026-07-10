using Locadora.Application.Commands.Amigos.RemoverAmigo;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Exceptions;
using Locadora.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.UnitTest.Commands.Amigos;

public class RemoverAmigoCommandHandlerTests
{
    private readonly Mock<IAmigoRepository> _amigos = new();
    private readonly Mock<IEmprestimoRepository> _emprestimos = new();
    private readonly Mock<IAmigoCache> _cache = new();

    private RemoverAmigoCommandHandler CriarHandler() => new(
        _amigos.Object,
        _emprestimos.Object,
        _cache.Object,
        new Mock<ILogger<RemoverAmigoCommandHandler>>().Object);

    [Fact]
    public async Task Handle_DeveRemoverQuandoExistir()
    {
        var amigo = new Amigo { Id = 1, Nome = "Walanem", Sobrenome = "Figueiredo" };
        _amigos.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(amigo);
        _emprestimos.Setup(r => r.ExisteParaAmigoAsync(1)).ReturnsAsync(false);

        var handler = CriarHandler();
        var resultado = await handler.Handle(new RemoverAmigoCommand(1), CancellationToken.None);

        resultado.Should().BeTrue();
        _amigos.Verify(r => r.RemoverAsync(amigo), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveLancarConflitoExceptionQuandoAmigoPossuiEmprestimos()
    {
        var amigo = new Amigo { Id = 1, Nome = "Walanem", Sobrenome = "Figueiredo" };
        _amigos.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(amigo);
        _emprestimos.Setup(r => r.ExisteParaAmigoAsync(1)).ReturnsAsync(true);

        var handler = CriarHandler();

        await FluentActions.Awaiting(() => handler.Handle(new RemoverAmigoCommand(1), CancellationToken.None))
            .Should().ThrowAsync<ConflitoException>();

        _amigos.Verify(r => r.RemoverAsync(It.IsAny<Amigo>()), Times.Never);
    }
}
