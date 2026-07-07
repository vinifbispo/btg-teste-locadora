using System.Text.Json;
using Locadora.Application.Dtos;
using Locadora.Application.Services;
using Locadora.Application.UnitTest.TestHelpers;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Exceptions;
using Locadora.Domain.Idempotencia;
using Locadora.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.UnitTest.Services;

public class AmigoServiceTests
{
    private readonly Mock<IAmigoRepository> _amigos = new();
    private readonly Mock<IEmprestimoRepository> _emprestimos = new();
    private readonly Mock<IAmigoCache> _cache = new();
    private readonly Mock<IArmazenamentoIdempotencia> _idempotencia = new();

    private AmigoService CriarService() => new(
        _amigos.Object,
        _emprestimos.Object,
        _cache.Object,
        _idempotencia.Object,
        new Mock<ILogger<AmigoService>>().Object);

    [Fact]
    public async Task CriarAsync_DeveCriarAmigo()
    {
        var input = new AmigoInputDto { Nome = "Walanem", Sobrenome = "Figueiredo", Idade = 30 };

        var service = CriarService();
        var dto = await service.CriarAsync(input, chaveIdempotencia: null);

        dto.Nome.Should().Be("Walanem");
        dto.Sobrenome.Should().Be("Figueiredo");
        _amigos.Verify(r => r.AdicionarAsync(It.IsAny<Amigo>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarResultadoCacheadoQuandoChaveIdempotenciaJaProcessada()
    {
        var dtoCacheado = new AmigoDto { Id = 1, Nome = "Walanem", Sobrenome = "Figueiredo" };
        _idempotencia.Setup(i => i.ObterAsync("amigo:criar:chave-1")).ReturnsAsync(JsonSerializer.Serialize(dtoCacheado));

        var service = CriarService();
        var dto = await service.CriarAsync(new AmigoInputDto(), chaveIdempotencia: "chave-1");

        dto.Id.Should().Be(1);
        dto.Nome.Should().Be("Walanem");
        _amigos.Verify(r => r.AdicionarAsync(It.IsAny<Amigo>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_DeveArmazenarResultadoQuandoChaveIdempotenciaInformadaENaoExisteCache()
    {
        _idempotencia.Setup(i => i.ObterAsync(It.IsAny<string>())).ReturnsAsync((string?)null);

        var input = new AmigoInputDto { Nome = "Vinicius", Sobrenome = "Bispo", Idade = 25 };

        var service = CriarService();
        await service.CriarAsync(input, chaveIdempotencia: "chave-2");

        _idempotencia.Verify(i => i.ArmazenarAsync("amigo:criar:chave-2", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarDoCacheQuandoDisponivel()
    {
        var amigoCacheado = new Amigo { Id = 1, Nome = "Vinicius", Sobrenome = "Bispo" };
        _cache.Setup(c => c.ObterAsync(1)).ReturnsAsync(amigoCacheado);

        var service = CriarService();
        var dto = await service.ObterPorIdAsync(1);

        dto.Should().NotBeNull();
        dto!.Nome.Should().Be("Vinicius");
        _amigos.Verify(r => r.ObterPorIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ListarAsync_DeveRetornarPaginaCorretaOrdenadaPorNome()
    {
        var amigos = new List<Amigo>
        {
            new() { Id = 1, Nome = "Carlos", Sobrenome = "Silva" },
            new() { Id = 2, Nome = "Ana", Sobrenome = "Souza" },
            new() { Id = 3, Nome = "Bruno", Sobrenome = "Costa" }
        };
        _amigos.Setup(r => r.ListarAsync()).ReturnsAsync(new TestAsyncEnumerable<Amigo>(amigos));

        var service = CriarService();
        var resultado = await service.ListarAsync(busca: null, page: 1, pageSize: 2);

        resultado.TotalCount.Should().Be(3);
        resultado.TotalPages.Should().Be(2);
        resultado.Items.Select(a => a.Nome).Should().Equal("Ana", "Bruno");
    }

    [Fact]
    public async Task ListarAsync_DeveFiltrarPorBusca()
    {
        var amigos = new List<Amigo>
        {
            new() { Id = 1, Nome = "Carlos", Sobrenome = "Silva" },
            new() { Id = 2, Nome = "Ana", Sobrenome = "Souza" }
        };
        _amigos.Setup(r => r.ListarAsync()).ReturnsAsync(new TestAsyncEnumerable<Amigo>(amigos));

        var service = CriarService();
        var resultado = await service.ListarAsync(busca: "arlos", page: 1, pageSize: 10);

        resultado.TotalCount.Should().Be(1);
        resultado.Items.Should().ContainSingle(a => a.Nome == "Carlos");
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarFalseQuandoNaoEncontrado()
    {
        _amigos.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Amigo?)null);

        var service = CriarService();
        var resultado = await service.AtualizarAsync(1, new AmigoInputDto());

        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task RemoverAsync_DeveRemoverQuandoExistir()
    {
        var amigo = new Amigo { Id = 1, Nome = "Walanem", Sobrenome = "Figueiredo" };
        _amigos.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(amigo);
        _emprestimos.Setup(r => r.ExisteParaAmigoAsync(1)).ReturnsAsync(false);

        var service = CriarService();
        var resultado = await service.RemoverAsync(1);

        resultado.Should().BeTrue();
        _amigos.Verify(r => r.RemoverAsync(amigo), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_DeveLancarConflitoExceptionQuandoAmigoPossuiEmprestimos()
    {
        var amigo = new Amigo { Id = 1, Nome = "Walanem", Sobrenome = "Figueiredo" };
        _amigos.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(amigo);
        _emprestimos.Setup(r => r.ExisteParaAmigoAsync(1)).ReturnsAsync(true);

        var service = CriarService();

        await FluentActions.Awaiting(() => service.RemoverAsync(1))
            .Should().ThrowAsync<ConflitoException>();

        _amigos.Verify(r => r.RemoverAsync(It.IsAny<Amigo>()), Times.Never);
    }
}
