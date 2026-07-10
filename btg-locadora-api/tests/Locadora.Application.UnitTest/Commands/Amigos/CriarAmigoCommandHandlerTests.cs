using System.Text.Json;
using Locadora.Application.Dtos;
using Locadora.Application.Commands.Amigos.CriarAmigo;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Idempotencia;
using Locadora.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.UnitTest.Commands.Amigos;

public class CriarAmigoCommandHandlerTests
{
    private readonly Mock<IAmigoRepository> _amigos = new();
    private readonly Mock<IAmigoCache> _cache = new();
    private readonly Mock<IArmazenamentoIdempotencia> _idempotencia = new();

    private CriarAmigoCommandHandler CriarHandler() => new(
        _amigos.Object,
        _cache.Object,
        _idempotencia.Object,
        new Mock<ILogger<CriarAmigoCommandHandler>>().Object);

    [Fact]
    public async Task Handle_DeveCriarAmigo()
    {
        var input = new AmigoInputDto { Nome = "Walanem", Sobrenome = "Figueiredo", Idade = 30 };

        var handler = CriarHandler();
        var dto = await handler.Handle(new CriarAmigoCommand(input, ChaveIdempotencia: null), CancellationToken.None);

        dto.Nome.Should().Be("Walanem");
        dto.Sobrenome.Should().Be("Figueiredo");
        _amigos.Verify(r => r.AdicionarAsync(It.IsAny<Amigo>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveRetornarResultadoCacheadoQuandoChaveIdempotenciaJaProcessada()
    {
        var dtoCacheado = new AmigoDto { Id = 1, Nome = "Walanem", Sobrenome = "Figueiredo" };
        _idempotencia.Setup(i => i.ObterAsync("amigo:criar:chave-1")).ReturnsAsync(JsonSerializer.Serialize(dtoCacheado));

        var handler = CriarHandler();
        var dto = await handler.Handle(new CriarAmigoCommand(new AmigoInputDto(), ChaveIdempotencia: "chave-1"), CancellationToken.None);

        dto.Id.Should().Be(1);
        dto.Nome.Should().Be("Walanem");
        _amigos.Verify(r => r.AdicionarAsync(It.IsAny<Amigo>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeveArmazenarResultadoQuandoChaveIdempotenciaInformadaENaoExisteCache()
    {
        _idempotencia.Setup(i => i.ObterAsync(It.IsAny<string>())).ReturnsAsync((string?)null);

        var input = new AmigoInputDto { Nome = "Vinicius", Sobrenome = "Bispo", Idade = 25 };

        var handler = CriarHandler();
        await handler.Handle(new CriarAmigoCommand(input, ChaveIdempotencia: "chave-2"), CancellationToken.None);

        _idempotencia.Verify(i => i.ArmazenarAsync("amigo:criar:chave-2", It.IsAny<string>()), Times.Once);
    }
}
