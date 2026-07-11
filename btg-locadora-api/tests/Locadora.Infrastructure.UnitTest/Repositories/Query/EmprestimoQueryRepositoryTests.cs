using Locadora.Application.UnitTest.TestHelpers;
using Locadora.Domain.Entities;
using Locadora.Infrastructure.Persistence.Repositories.Query;

namespace Locadora.Application.UnitTest.Repositories.Query;

public class EmprestimoQueryRepositoryTests
{
    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarEmprestimoQuandoExistir()
    {
        var dados = new List<Emprestimo>
        {
            new() { Id = 1, JogoId = 1, DataEmprestimo = DateTime.UtcNow },
            new() { Id = 2, JogoId = 2, DataEmprestimo = DateTime.UtcNow }
        };

        var mockContext = TestDbContextFactory.CreateRead();
        mockContext.Object.Emprestimos = MockDbSetFactory.Create(dados).Object;
        var repositorio = new EmprestimoQueryRepository(mockContext.Object);

        var resultado = await repositorio.ObterPorIdAsync(2);

        resultado.Should().NotBeNull();
        resultado!.JogoId.Should().Be(2);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarNuloQuandoNaoExistir()
    {
        var dados = new List<Emprestimo> { new() { Id = 1, JogoId = 1, DataEmprestimo = DateTime.UtcNow } };

        var mockContext = TestDbContextFactory.CreateRead();
        mockContext.Object.Emprestimos = MockDbSetFactory.Create(dados).Object;
        var repositorio = new EmprestimoQueryRepository(mockContext.Object);

        var resultado = await repositorio.ObterPorIdAsync(999);

        resultado.Should().BeNull();
    }
}
