using System.Text.Json;
using Locadora.Application.Dtos;
using Locadora.Application.Commands.Jogos.CriarJogo;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Exceptions;
using Locadora.Domain.Idempotencia;
using Locadora.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.UnitTest.Commands.Jogos;

public class CriarJogoCommandHandlerTests
{
    private readonly Mock<IJogoRepository> _jogos = new();
    private readonly Mock<IGeneroRepository> _generos = new();
    private readonly Mock<IDesenvolvedorRepository> _desenvolvedores = new();
    private readonly Mock<IPublicadoraRepository> _publicadoras = new();
    private readonly Mock<IJogoCache> _cache = new();
    private readonly Mock<IArmazenamentoIdempotencia> _idempotencia = new();

    private CriarJogoCommandHandler CriarHandler() => new(
        _jogos.Object,
        _generos.Object,
        _desenvolvedores.Object,
        _publicadoras.Object,
        _cache.Object,
        _idempotencia.Object,
        new Mock<ILogger<CriarJogoCommandHandler>>().Object);

    [Fact]
    public async Task Handle_DeveCriarJogoQuandoIdsValidos()
    {
        var genero = new Genero { Id = 1, Nome = "RPG" };
        var desenvolvedor = new Desenvolvedor { Id = 2, Nome = "BTG Pactual" };
        var publicadora = new Publicadora { Id = 3, Nome = "Sony" };

        _generos.Setup(r => r.ListarPorIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync([genero]);
        _desenvolvedores.Setup(r => r.ListarPorIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync([desenvolvedor]);
        _publicadoras.Setup(r => r.ListarPorIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync([publicadora]);

        var input = new JogoInputDto
        {
            Nome = "EA FC 26",
            GeneroIds = [1],
            DesenvolvedorIds = [2],
            PublicadoraIds = [3]
        };

        var handler = CriarHandler();
        var dto = await handler.Handle(new CriarJogoCommand(input, ChaveIdempotencia: null), CancellationToken.None);

        dto.Nome.Should().Be("EA FC 26");
        dto.Generos.Should().ContainSingle(g => g.Nome == "RPG");
        dto.Desenvolvedores.Should().ContainSingle(d => d.Nome == "BTG Pactual");
        dto.Publicadoras.Should().ContainSingle(p => p.Nome == "Sony");
        _jogos.Verify(r => r.AdicionarAsync(It.IsAny<Jogo>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveLancarNotFoundExceptionQuandoGeneroIdNaoExistir()
    {
        _generos.Setup(r => r.ListarPorIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync([]);
        _desenvolvedores.Setup(r => r.ListarPorIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync([]);
        _publicadoras.Setup(r => r.ListarPorIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync([]);

        var input = new JogoInputDto
        {
            Nome = "Jogo Inválido",
            GeneroIds = [99],
            DesenvolvedorIds = [],
            PublicadoraIds = []
        };

        var handler = CriarHandler();

        await FluentActions.Awaiting(() => handler.Handle(new CriarJogoCommand(input, ChaveIdempotencia: null), CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();

        _jogos.Verify(r => r.AdicionarAsync(It.IsAny<Jogo>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeveRetornarResultadoCacheadoQuandoChaveIdempotenciaJaProcessada()
    {
        var dtoCacheado = new JogoDto { Id = 1, Nome = "EA FC 26" };
        _idempotencia.Setup(i => i.ObterAsync("jogo:criar:chave-1")).ReturnsAsync(JsonSerializer.Serialize(dtoCacheado));

        var handler = CriarHandler();
        var dto = await handler.Handle(new CriarJogoCommand(new JogoInputDto(), ChaveIdempotencia: "chave-1"), CancellationToken.None);

        dto.Id.Should().Be(1);
        dto.Nome.Should().Be("EA FC 26");
        _jogos.Verify(r => r.AdicionarAsync(It.IsAny<Jogo>()), Times.Never);
        _generos.Verify(r => r.ListarPorIdsAsync(It.IsAny<IEnumerable<int>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeveArmazenarResultadoQuandoChaveIdempotenciaInformadaENaoExisteCache()
    {
        var genero = new Genero { Id = 1, Nome = "RPG" };
        var desenvolvedor = new Desenvolvedor { Id = 2, Nome = "BTG Pactual" };
        var publicadora = new Publicadora { Id = 3, Nome = "Sony" };

        _idempotencia.Setup(i => i.ObterAsync(It.IsAny<string>())).ReturnsAsync((string?)null);
        _generos.Setup(r => r.ListarPorIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync([genero]);
        _desenvolvedores.Setup(r => r.ListarPorIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync([desenvolvedor]);
        _publicadoras.Setup(r => r.ListarPorIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync([publicadora]);

        var input = new JogoInputDto { Nome = "EA FC 26", GeneroIds = [1], DesenvolvedorIds = [2], PublicadoraIds = [3] };

        var handler = CriarHandler();
        await handler.Handle(new CriarJogoCommand(input, ChaveIdempotencia: "chave-2"), CancellationToken.None);

        _idempotencia.Verify(i => i.ArmazenarAsync("jogo:criar:chave-2", It.IsAny<string>()), Times.Once);
    }
}
