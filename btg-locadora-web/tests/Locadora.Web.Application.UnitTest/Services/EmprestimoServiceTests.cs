using FluentAssertions;
using Locadora.Web.Application.Interfaces;
using Locadora.Web.Application.Services;
using Locadora.Web.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace Locadora.Web.Application.UnitTest.Services;

public class EmprestimoServiceTests
{
    private readonly Mock<IEmprestimoApiClient> _apiMock = new();
    private readonly EmprestimoService _service;

    public EmprestimoServiceTests()
    {
        _service = new EmprestimoService(_apiMock.Object, Mock.Of<ILogger<EmprestimoService>>());
    }

    [Fact]
    public async Task ListarAsync_DeveRepassarFiltrosParaApiClient()
    {
        var esperado = new PagedResult<Emprestimo>
        {
            Items = new List<Emprestimo> { new() { Id = 1, JogoId = 2, AmigoId = 3 } },
            Page = 1,
            PageSize = 10,
            TotalCount = 1,
            TotalPages = 1
        };
        _apiMock.Setup(a => a.ListarAsync(2, 3, true, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(esperado);

        var resultado = await _service.ListarAsync(jogoId: 2, amigoId: 3, apenasAtivos: true);

        resultado.Should().BeSameAs(esperado);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoEmprestimoNaoExiste_DeveRetornarNull()
    {
        _apiMock.Setup(a => a.ObterPorIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Emprestimo?)null);

        var resultado = await _service.ObterPorIdAsync(99);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task CriarAsync_DeveRepassarInputParaApiClientERetornarEmprestimoCriado()
    {
        var input = new EmprestimoInput { JogoId = 1, AmigoId = 2 };
        var criado = new Emprestimo { Id = 10, JogoId = 1, AmigoId = 2 };
        _apiMock.Setup(a => a.CriarAsync(input, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(criado);

        var resultado = await _service.CriarAsync(input);

        resultado.Should().BeSameAs(criado);
        _apiMock.Verify(a => a.CriarAsync(input, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DevolverAsync_QuandoApiRetornaVerdadeiro_DeveRetornarVerdadeiro()
    {
        _apiMock.Setup(a => a.DevolverAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var resultado = await _service.DevolverAsync(1);

        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task DevolverAsync_QuandoApiRetornaFalso_DeveRetornarFalso()
    {
        _apiMock.Setup(a => a.DevolverAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var resultado = await _service.DevolverAsync(99);

        resultado.Should().BeFalse();
    }
}
