using Locadora.Application.Queries.Amigos.ListarAmigos;
using Locadora.Application.UnitTest.TestHelpers;
using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.UnitTest.Queries.Amigos;

public class ListarAmigosQueryHandlerTests
{
    private readonly Mock<IAmigoRepository> _amigos = new();

    private ListarAmigosQueryHandler CriarHandler() => new(_amigos.Object, new Mock<ILogger<ListarAmigosQueryHandler>>().Object);

    [Fact]
    public async Task Handle_DeveRetornarPaginaCorretaOrdenadaPorNome()
    {
        var amigos = new List<Amigo>
        {
            new() { Id = 1, Nome = "Carlos", Sobrenome = "Silva" },
            new() { Id = 2, Nome = "Ana", Sobrenome = "Souza" },
            new() { Id = 3, Nome = "Bruno", Sobrenome = "Costa" }
        };
        _amigos.Setup(r => r.ListarAsync()).ReturnsAsync(new TestAsyncEnumerable<Amigo>(amigos));

        var handler = CriarHandler();
        var resultado = await handler.Handle(new ListarAmigosQuery(Busca: null, Page: 1, PageSize: 2), CancellationToken.None);

        resultado.TotalCount.Should().Be(3);
        resultado.TotalPages.Should().Be(2);
        resultado.Items.Select(a => a.Nome).Should().Equal("Ana", "Bruno");
    }

    [Fact]
    public async Task Handle_DeveFiltrarPorBusca()
    {
        var amigos = new List<Amigo>
        {
            new() { Id = 1, Nome = "Carlos", Sobrenome = "Silva" },
            new() { Id = 2, Nome = "Ana", Sobrenome = "Souza" }
        };
        _amigos.Setup(r => r.ListarAsync()).ReturnsAsync(new TestAsyncEnumerable<Amigo>(amigos));

        var handler = CriarHandler();
        var resultado = await handler.Handle(new ListarAmigosQuery(Busca: "arlos", Page: 1, PageSize: 10), CancellationToken.None);

        resultado.TotalCount.Should().Be(1);
        resultado.Items.Should().ContainSingle(a => a.Nome == "Carlos");
    }
}
