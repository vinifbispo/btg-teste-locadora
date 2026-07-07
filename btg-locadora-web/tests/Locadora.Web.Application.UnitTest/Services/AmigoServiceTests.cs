using FluentAssertions;
using Locadora.Web.Application.Interfaces;
using Locadora.Web.Application.Services;
using Locadora.Web.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace Locadora.Web.Application.UnitTest.Services;

public class AmigoServiceTests
{
    private readonly Mock<IAmigoApiClient> _apiMock = new();
    private readonly AmigoService _service;

    public AmigoServiceTests()
    {
        _service = new AmigoService(_apiMock.Object, Mock.Of<ILogger<AmigoService>>());
    }

    [Fact]
    public async Task ListarAsync_DeveRetornarResultadoDoApiClient()
    {
        var esperado = new PagedResult<Amigo>
        {
            Items = new List<Amigo> { new() { Id = 1, Nome = "Vinicius", Sobrenome = "Bispo" } },
            Page = 1,
            PageSize = 10,
            TotalCount = 1,
            TotalPages = 1
        };
        _apiMock.Setup(a => a.ListarAsync("vinicius", 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(esperado);

        var resultado = await _service.ListarAsync("vinicius");

        resultado.Should().BeSameAs(esperado);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoAmigoNaoExiste_DeveRetornarNull()
    {
        _apiMock.Setup(a => a.ObterPorIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Amigo?)null);

        var resultado = await _service.ObterPorIdAsync(99);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task CriarAsync_DeveRepassarInputParaApiClientERetornarAmigoCriado()
    {
        var input = new AmigoInput { Nome = "Walanem", Sobrenome = "Figueiredo", Idade = 30 };
        var criado = new Amigo { Id = 5, Nome = "Walanem", Sobrenome = "Figueiredo", Idade = 30 };
        _apiMock.Setup(a => a.CriarAsync(input, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(criado);

        var resultado = await _service.CriarAsync(input);

        resultado.Should().BeSameAs(criado);
        _apiMock.Verify(a => a.CriarAsync(input, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoApiRetornaFalso_DeveRetornarFalso()
    {
        var input = new AmigoInput { Nome = "Amigo Atualizado", Sobrenome = "Figueiredo", Idade = 31 };
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
