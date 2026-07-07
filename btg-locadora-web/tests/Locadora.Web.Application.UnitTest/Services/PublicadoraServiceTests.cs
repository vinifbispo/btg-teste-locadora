using FluentAssertions;
using Locadora.Web.Application.Interfaces;
using Locadora.Web.Application.Services;
using Locadora.Web.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace Locadora.Web.Application.UnitTest.Services;

public class PublicadoraServiceTests
{
    private readonly Mock<IPublicadoraApiClient> _apiMock = new();
    private readonly PublicadoraService _service;

    public PublicadoraServiceTests()
    {
        _service = new PublicadoraService(_apiMock.Object, Mock.Of<ILogger<PublicadoraService>>());
    }

    [Fact]
    public async Task BuscarPorNomeAsync_DeveRetornarResultadoDoApiClient()
    {
        var esperado = new List<Publicadora> { new() { Id = 1, Nome = "CD Projekt" } };
        _apiMock.Setup(a => a.BuscarPorNomeAsync("cd", It.IsAny<CancellationToken>()))
            .ReturnsAsync(esperado);

        var resultado = await _service.BuscarPorNomeAsync("cd");

        resultado.Should().BeSameAs(esperado);
    }
}
