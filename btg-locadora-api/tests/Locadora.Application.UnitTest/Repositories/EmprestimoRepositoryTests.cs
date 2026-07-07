using Locadora.Application.UnitTest.TestHelpers;
using Locadora.Domain.Entities;
using Locadora.Infrastructure.Persistence.Repositories;

namespace Locadora.Application.UnitTest.Repositories;

public class EmprestimoRepositoryTests
{
    [Fact]
    public async Task ObterAtivoPorJogoAsync_DeveRetornarEmprestimoSemDataDevolucao()
    {
        var dados = new List<Emprestimo>
        {
            new() { Id = 1, JogoId = 10, DataEmprestimo = DateTime.UtcNow, DataDevolucao = null },
            new() { Id = 2, JogoId = 20, DataEmprestimo = DateTime.UtcNow, DataDevolucao = DateTime.UtcNow }
        };

        var mockContext = TestDbContextFactory.Create();
        mockContext.Object.Emprestimos = MockDbSetFactory.Create(dados).Object;
        var repositorio = new EmprestimoRepository(mockContext.Object);

        var resultado = await repositorio.ObterAtivoPorJogoAsync(10);

        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(1);
    }

    [Fact]
    public async Task ObterAtivoPorJogoAsync_DeveRetornarNuloQuandoJogoJaFoiDevolvido()
    {
        var dados = new List<Emprestimo>
        {
            new() { Id = 2, JogoId = 20, DataEmprestimo = DateTime.UtcNow, DataDevolucao = DateTime.UtcNow }
        };

        var mockContext = TestDbContextFactory.Create();
        mockContext.Object.Emprestimos = MockDbSetFactory.Create(dados).Object;
        var repositorio = new EmprestimoRepository(mockContext.Object);

        var resultado = await repositorio.ObterAtivoPorJogoAsync(20);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task AdicionarAsync_DeveAdicionarEmprestimoNaLista()
    {
        var dados = new List<Emprestimo>();

        var mockContext = TestDbContextFactory.Create();
        mockContext.Object.Emprestimos = MockDbSetFactory.Create(dados).Object;
        var repositorio = new EmprestimoRepository(mockContext.Object);

        await repositorio.AdicionarAsync(new Emprestimo { JogoId = 1, AmigoId = 2, DataEmprestimo = DateTime.UtcNow });

        dados.Should().ContainSingle();
    }
}
