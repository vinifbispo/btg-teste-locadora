using FluentAssertions;
using Locadora.Web.Application.Interfaces;
using Locadora.Web.Application.Services;
using Locadora.Web.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace Locadora.Web.Application.UnitTest.Services;

public class DesenvolvedorServiceTests
{
    private readonly Mock<IDesenvolvedorApiClient> _apiMock = new();
    private readonly DesenvolvedorService _service;

    public DesenvolvedorServiceTests()
    {
        _service = new DesenvolvedorService(_apiMock.Object, Mock.Of<ILogger<DesenvolvedorService>>());
    }

    [Fact]
    public async Task BuscarPorNomeAsync_DeveRetornarResultadoDoApiClient()
    {
        var esperado = new List<Desenvolvedor> { new() { Id = 1, Nome = "CD Projekt Red" } };
        _apiMock.Setup(a => a.BuscarPorNomeAsync("cd", It.IsAny<CancellationToken>()))
            .ReturnsAsync(esperado);

        var resultado = await _service.BuscarPorNomeAsync("cd");

        resultado.Should().BeSameAs(esperado);
    }
}
