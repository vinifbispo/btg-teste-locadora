using Locadora.Application.Queries.Amigos.ObterAmigoPorId;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.UnitTest.Queries.Amigos;

public class ObterAmigoPorIdQueryHandlerTests
{
    private readonly Mock<IAmigoRepository> _amigos = new();
    private readonly Mock<IAmigoCache> _cache = new();

    private ObterAmigoPorIdQueryHandler CriarHandler() => new(_amigos.Object, _cache.Object, new Mock<ILogger<ObterAmigoPorIdQueryHandler>>().Object);

    [Fact]
    public async Task Handle_DeveRetornarDoCacheQuandoDisponivel()
    {
        var amigoCacheado = new Amigo { Id = 1, Nome = "Vinicius", Sobrenome = "Bispo" };
        _cache.Setup(c => c.ObterAsync(1)).ReturnsAsync(amigoCacheado);

        var handler = CriarHandler();
        var dto = await handler.Handle(new ObterAmigoPorIdQuery(1), CancellationToken.None);

        dto.Should().NotBeNull();
        dto!.Nome.Should().Be("Vinicius");
        _amigos.Verify(r => r.ObterPorIdAsync(It.IsAny<int>()), Times.Never);
    }
}
