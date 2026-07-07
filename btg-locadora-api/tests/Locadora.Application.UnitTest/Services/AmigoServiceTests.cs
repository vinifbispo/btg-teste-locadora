using System.Text.Json;
using Locadora.Application.Dtos;
using Locadora.Application.Services;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Idempotencia;
using Locadora.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.UnitTest.Services;

public class AmigoServiceTests
{
    private readonly Mock<IAmigoRepository> _amigos = new();
    private readonly Mock<IAmigoCache> _cache = new();
    private readonly Mock<IArmazenamentoIdempotencia> _idempotencia = new();

    private AmigoService CriarService() => new(
        _amigos.Object,
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

        var service = CriarService();
        var resultado = await service.RemoverAsync(1);

        resultado.Should().BeTrue();
        _amigos.Verify(r => r.RemoverAsync(amigo), Times.Once);
    }
}
