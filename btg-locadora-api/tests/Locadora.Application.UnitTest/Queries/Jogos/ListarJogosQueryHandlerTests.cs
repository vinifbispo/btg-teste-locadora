using Locadora.Application.Queries.Jogos.ListarJogos;
using Locadora.Application.UnitTest.TestHelpers;
using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.UnitTest.Queries.Jogos;

public class ListarJogosQueryHandlerTests
{
    private readonly Mock<IJogoQueryRepository> _jogos = new();

    private ListarJogosQueryHandler CriarHandler() => new(_jogos.Object, new Mock<ILogger<ListarJogosQueryHandler>>().Object);

    [Fact]
    public async Task Handle_DeveRetornarPaginaCorretaOrdenadaPorNome()
    {
        var jogos = new List<Jogo>
        {
            new() { Id = 1, Nome = "Zelda" },
            new() { Id = 2, Nome = "Mario" },
            new() { Id = 3, Nome = "Alan Wake" }
        };
        _jogos.Setup(r => r.ListarAsync()).ReturnsAsync(new TestAsyncEnumerable<Jogo>(jogos));

        var handler = CriarHandler();
        var resultado = await handler.Handle(new ListarJogosQuery(Busca: null, Page: 2, PageSize: 2), CancellationToken.None);

        resultado.TotalCount.Should().Be(3);
        resultado.TotalPages.Should().Be(2);
        resultado.Items.Should().ContainSingle(j => j.Nome == "Zelda");
    }

    [Fact]
    public async Task Handle_DeveFiltrarPorBusca()
    {
        var jogos = new List<Jogo>
        {
            new() { Id = 1, Nome = "Zelda" },
            new() { Id = 2, Nome = "Mario" }
        };
        _jogos.Setup(r => r.ListarAsync()).ReturnsAsync(new TestAsyncEnumerable<Jogo>(jogos));

        var handler = CriarHandler();
        var resultado = await handler.Handle(new ListarJogosQuery(Busca: "elda", Page: 1, PageSize: 10), CancellationToken.None);

        resultado.TotalCount.Should().Be(1);
        resultado.Items.Should().ContainSingle(j => j.Nome == "Zelda");
    }
}
