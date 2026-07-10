using Locadora.Application.Queries.Emprestimos.ListarEmprestimos;
using Locadora.Application.UnitTest.TestHelpers;
using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.UnitTest.Queries.Emprestimos;

public class ListarEmprestimosQueryHandlerTests
{
    private readonly Mock<IEmprestimoQueryRepository> _emprestimos = new();

    private ListarEmprestimosQueryHandler CriarHandler() => new(_emprestimos.Object, new Mock<ILogger<ListarEmprestimosQueryHandler>>().Object);

    [Fact]
    public async Task Handle_DeveFiltrarApenasAtivosEPaginar()
    {
        var jogo1 = new Jogo { Id = 1, Nome = "Jogo 1" };
        var jogo2 = new Jogo { Id = 2, Nome = "Jogo 2" };
        var jogo3 = new Jogo { Id = 3, Nome = "Jogo 3" };
        var amigo = new Amigo { Id = 1, Nome = "Walanem", Sobrenome = "Figueiredo" };

        var emprestimos = new List<Emprestimo>
        {
            new() { Id = 1, JogoId = 1, Jogo = jogo1, AmigoId = 1, Amigo = amigo, DataEmprestimo = DateTime.UtcNow.AddDays(-3), DataDevolucao = DateTime.UtcNow.AddDays(-1) },
            new() { Id = 2, JogoId = 2, Jogo = jogo2, AmigoId = 1, Amigo = amigo, DataEmprestimo = DateTime.UtcNow.AddDays(-2) },
            new() { Id = 3, JogoId = 3, Jogo = jogo3, AmigoId = 1, Amigo = amigo, DataEmprestimo = DateTime.UtcNow.AddDays(-1) }
        };
        _emprestimos.Setup(r => r.ListarAsync()).ReturnsAsync(new TestAsyncEnumerable<Emprestimo>(emprestimos));

        var handler = CriarHandler();
        var resultado = await handler.Handle(new ListarEmprestimosQuery(JogoId: null, AmigoId: null, ApenasAtivos: true, Page: 1, PageSize: 10), CancellationToken.None);

        resultado.TotalCount.Should().Be(2);
        resultado.Items.Should().OnlyContain(e => e.DataDevolucao == null);
    }

    [Fact]
    public async Task Handle_DevePaginarResultados()
    {
        var jogo1 = new Jogo { Id = 1, Nome = "Jogo 1" };
        var jogo2 = new Jogo { Id = 2, Nome = "Jogo 2" };
        var jogo3 = new Jogo { Id = 3, Nome = "Jogo 3" };
        var amigo = new Amigo { Id = 1, Nome = "Walanem", Sobrenome = "Figueiredo" };

        var emprestimos = new List<Emprestimo>
        {
            new() { Id = 1, JogoId = 1, Jogo = jogo1, AmigoId = 1, Amigo = amigo, DataEmprestimo = DateTime.UtcNow.AddDays(-3) },
            new() { Id = 2, JogoId = 2, Jogo = jogo2, AmigoId = 1, Amigo = amigo, DataEmprestimo = DateTime.UtcNow.AddDays(-2) },
            new() { Id = 3, JogoId = 3, Jogo = jogo3, AmigoId = 1, Amigo = amigo, DataEmprestimo = DateTime.UtcNow.AddDays(-1) }
        };
        _emprestimos.Setup(r => r.ListarAsync()).ReturnsAsync(new TestAsyncEnumerable<Emprestimo>(emprestimos));

        var handler = CriarHandler();
        var resultado = await handler.Handle(new ListarEmprestimosQuery(JogoId: null, AmigoId: null, ApenasAtivos: null, Page: 1, PageSize: 2), CancellationToken.None);

        resultado.TotalCount.Should().Be(3);
        resultado.TotalPages.Should().Be(2);
        resultado.Items.Should().HaveCount(2);
        resultado.Items.First().Id.Should().Be(3);
    }
}
