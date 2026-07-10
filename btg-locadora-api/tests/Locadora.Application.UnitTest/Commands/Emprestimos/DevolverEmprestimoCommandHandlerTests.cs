using Locadora.Application.Commands.Emprestimos.DevolverEmprestimo;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.UnitTest.Commands.Emprestimos;

public class DevolverEmprestimoCommandHandlerTests
{
    private readonly Mock<IEmprestimoRepository> _emprestimos = new();
    private readonly Mock<IEmprestimoCache> _cache = new();

    private DevolverEmprestimoCommandHandler CriarHandler() => new(_emprestimos.Object, _cache.Object, new Mock<ILogger<DevolverEmprestimoCommandHandler>>().Object);

    [Fact]
    public async Task Handle_DeveRetornarFalseQuandoJaDevolvido()
    {
        var emprestimo = new Emprestimo { Id = 1, JogoId = 1, DataEmprestimo = DateTime.UtcNow, DataDevolucao = DateTime.UtcNow };
        _emprestimos.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(emprestimo);

        var handler = CriarHandler();
        var resultado = await handler.Handle(new DevolverEmprestimoCommand(1), CancellationToken.None);

        resultado.Should().BeFalse();
    }
}
