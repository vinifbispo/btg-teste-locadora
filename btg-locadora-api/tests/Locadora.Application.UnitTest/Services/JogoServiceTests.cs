using System.Text.Json;
using Locadora.Application.Dtos;
using Locadora.Application.Services;
using Locadora.Application.UnitTest.TestHelpers;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Exceptions;
using Locadora.Domain.Idempotencia;
using Locadora.Domain.Integracoes;
using Locadora.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.UnitTest.Services;

public class JogoServiceTests
{
    private readonly Mock<IJogoRepository> _jogos = new();
    private readonly Mock<IGeneroRepository> _generos = new();
    private readonly Mock<IDesenvolvedorRepository> _desenvolvedores = new();
    private readonly Mock<IPublicadoraRepository> _publicadoras = new();
    private readonly Mock<IJogoCache> _cache = new();
    private readonly Mock<IArmazenamentoIdempotencia> _idempotencia = new();
    private readonly Mock<IJogoExternoApiClient> _jogoExterno = new();

    private JogoService CriarService() => new(
        _jogos.Object,
        _generos.Object,
        _desenvolvedores.Object,
        _publicadoras.Object,
        _cache.Object,
        _idempotencia.Object,
        _jogoExterno.Object,
        new Mock<ILogger<JogoService>>().Object);

    [Fact]
    public async Task CriarAsync_DeveCriarJogoQuandoIdsValidos()
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

        var service = CriarService();
        var dto = await service.CriarAsync(input, chaveIdempotencia: null);

        dto.Nome.Should().Be("EA FC 26");
        dto.Generos.Should().ContainSingle(g => g.Nome == "RPG");
        dto.Desenvolvedores.Should().ContainSingle(d => d.Nome == "BTG Pactual");
        dto.Publicadoras.Should().ContainSingle(p => p.Nome == "Sony");
        _jogos.Verify(r => r.AdicionarAsync(It.IsAny<Jogo>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_DeveLancarNotFoundExceptionQuandoGeneroIdNaoExistir()
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

        var service = CriarService();

        await FluentActions.Awaiting(() => service.CriarAsync(input, chaveIdempotencia: null))
            .Should().ThrowAsync<NotFoundException>();

        _jogos.Verify(r => r.AdicionarAsync(It.IsAny<Jogo>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarResultadoCacheadoQuandoChaveIdempotenciaJaProcessada()
    {
        var dtoCacheado = new JogoDto { Id = 1, Nome = "EA FC 26" };
        _idempotencia.Setup(i => i.ObterAsync("jogo:criar:chave-1")).ReturnsAsync(JsonSerializer.Serialize(dtoCacheado));

        var service = CriarService();
        var dto = await service.CriarAsync(new JogoInputDto(), chaveIdempotencia: "chave-1");

        dto.Id.Should().Be(1);
        dto.Nome.Should().Be("EA FC 26");
        _jogos.Verify(r => r.AdicionarAsync(It.IsAny<Jogo>()), Times.Never);
        _generos.Verify(r => r.ListarPorIdsAsync(It.IsAny<IEnumerable<int>>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_DeveArmazenarResultadoQuandoChaveIdempotenciaInformadaENaoExisteCache()
    {
        var genero = new Genero { Id = 1, Nome = "RPG" };
        var desenvolvedor = new Desenvolvedor { Id = 2, Nome = "BTG Pactual" };
        var publicadora = new Publicadora { Id = 3, Nome = "Sony" };

        _idempotencia.Setup(i => i.ObterAsync(It.IsAny<string>())).ReturnsAsync((string?)null);
        _generos.Setup(r => r.ListarPorIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync([genero]);
        _desenvolvedores.Setup(r => r.ListarPorIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync([desenvolvedor]);
        _publicadoras.Setup(r => r.ListarPorIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync([publicadora]);

        var input = new JogoInputDto { Nome = "EA FC 26", GeneroIds = [1], DesenvolvedorIds = [2], PublicadoraIds = [3] };

        var service = CriarService();
        await service.CriarAsync(input, chaveIdempotencia: "chave-2");

        _idempotencia.Verify(i => i.ArmazenarAsync("jogo:criar:chave-2", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ListarAsync_DeveRetornarPaginaCorretaOrdenadaPorNome()
    {
        var jogos = new List<Jogo>
        {
            new() { Id = 1, Nome = "Zelda" },
            new() { Id = 2, Nome = "Mario" },
            new() { Id = 3, Nome = "Alan Wake" }
        };
        _jogos.Setup(r => r.ListarAsync()).ReturnsAsync(new TestAsyncEnumerable<Jogo>(jogos));

        var service = CriarService();
        var resultado = await service.ListarAsync(busca: null, page: 2, pageSize: 2);

        resultado.TotalCount.Should().Be(3);
        resultado.TotalPages.Should().Be(2);
        resultado.Items.Should().ContainSingle(j => j.Nome == "Zelda");
    }

    [Fact]
    public async Task ListarAsync_DeveFiltrarPorBusca()
    {
        var jogos = new List<Jogo>
        {
            new() { Id = 1, Nome = "Zelda" },
            new() { Id = 2, Nome = "Mario" }
        };
        _jogos.Setup(r => r.ListarAsync()).ReturnsAsync(new TestAsyncEnumerable<Jogo>(jogos));

        var service = CriarService();
        var resultado = await service.ListarAsync(busca: "elda", page: 1, pageSize: 10);

        resultado.TotalCount.Should().Be(1);
        resultado.Items.Should().ContainSingle(j => j.Nome == "Zelda");
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarFalseQuandoJogoNaoEncontrado()
    {
        _jogos.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Jogo?)null);

        var service = CriarService();
        var resultado = await service.AtualizarAsync(1, new JogoInputDto());

        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task RemoverAsync_DeveRemoverQuandoExistir()
    {
        var jogo = new Jogo { Id = 1, Nome = "GTA VI" };
        _jogos.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(jogo);

        var service = CriarService();
        var resultado = await service.RemoverAsync(1);

        resultado.Should().BeTrue();
        _jogos.Verify(r => r.RemoverAsync(jogo), Times.Once);
        _cache.Verify(c => c.RemoverAsync(1), Times.Once);
    }

    [Fact]
    public async Task ImportarDoJogoExternoAsync_DeveIgnorarQuandoJaExistemJogos()
    {
        var existentes = new TestAsyncEnumerable<Jogo>([new Jogo { Id = 1, Nome = "EA FC 26" }]);
        _jogos.Setup(r => r.ListarAsync()).ReturnsAsync(existentes);

        var service = CriarService();
        await service.ImportarDoJogoExternoAsync();

        _jogoExterno.Verify(c => c.ListarAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
