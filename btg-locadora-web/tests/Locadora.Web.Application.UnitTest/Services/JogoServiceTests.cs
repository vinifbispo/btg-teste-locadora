using FluentAssertions;
using Locadora.Web.Application.Interfaces;
using Locadora.Web.Application.Services;
using Locadora.Web.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace Locadora.Web.Application.UnitTest.Services;

public class JogoServiceTests
{
    private readonly Mock<IJogoApiClient> _apiMock = new();
    private readonly JogoService _service;

    public JogoServiceTests()
    {
        _service = new JogoService(_apiMock.Object, Mock.Of<ILogger<JogoService>>());
    }

    [Fact]
    public async Task ListarAsync_DeveRetornarResultadoDoApiClient()
    {
        var esperado = new PagedResult<Jogo>
        {
            Items = new List<Jogo> { new() { Id = 1, Nome = "The Witcher 3" } },
            Page = 1,
            PageSize = 10,
            TotalCount = 1,
            TotalPages = 1
        };
        _apiMock.Setup(a => a.ListarAsync("witcher", 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(esperado);

        var resultado = await _service.ListarAsync("witcher");

        resultado.Should().BeSameAs(esperado);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoJogoNaoExiste_DeveRetornarNull()
    {
        _apiMock.Setup(a => a.ObterPorIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Jogo?)null);

        var resultado = await _service.ObterPorIdAsync(99);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task CriarAsync_DeveRepassarInputParaApiClientERetornarJogoCriado()
    {
        var input = new JogoInput { Nome = "Elden Ring" };
        var criado = new Jogo { Id = 5, Nome = "Elden Ring" };
        _apiMock.Setup(a => a.CriarAsync(input, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(criado);

        var resultado = await _service.CriarAsync(input);

        resultado.Should().BeSameAs(criado);
        _apiMock.Verify(a => a.CriarAsync(input, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoApiRetornaFalso_DeveRetornarFalso()
    {
        var input = new JogoInput { Nome = "Jogo Atualizado" };
        _apiMock.Setup(a => a.AtualizarAsync(1, input, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var resultado = await _service.AtualizarAsync(1, input);

        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task RemoverAsync_QuandoApiRetornaVerdadeiro_DeveRetornarVerdadeiro()
    {
        _apiMock.Setup(a => a.RemoverAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var resultado = await _service.RemoverAsync(1);

        resultado.Should().BeTrue();
    }
}
