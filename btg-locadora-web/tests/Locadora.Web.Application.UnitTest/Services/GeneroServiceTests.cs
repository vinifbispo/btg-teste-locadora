using FluentAssertions;
using Locadora.Web.Application.Interfaces;
using Locadora.Web.Application.Services;
using Locadora.Web.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace Locadora.Web.Application.UnitTest.Services;

public class GeneroServiceTests
{
    private readonly Mock<IGeneroApiClient> _apiMock = new();
    private readonly GeneroService _service;

    public GeneroServiceTests()
    {
        _service = new GeneroService(_apiMock.Object, Mock.Of<ILogger<GeneroService>>());
    }

    [Fact]
    public async Task BuscarPorNomeAsync_DeveRetornarResultadoDoApiClient()
    {
        var esperado = new List<Genero> { new() { Id = 1, Nome = "RPG" } };
        _apiMock.Setup(a => a.BuscarPorNomeAsync("rpg", It.IsAny<CancellationToken>()))
            .ReturnsAsync(esperado);

        var resultado = await _service.BuscarPorNomeAsync("rpg");

        resultado.Should().BeSameAs(esperado);
    }
}
