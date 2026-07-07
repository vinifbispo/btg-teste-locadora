using System.Text.Json;
using Locadora.Application.Dtos;
using Locadora.Application.Services;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Exceptions;
using Locadora.Domain.Idempotencia;
using Locadora.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.UnitTest.Services;

public class EmprestimoServiceTests
{
    private readonly Mock<IEmprestimoRepository> _emprestimos = new();
    private readonly Mock<IJogoRepository> _jogos = new();
    private readonly Mock<IAmigoRepository> _amigos = new();
    private readonly Mock<IEmprestimoCache> _cache = new();
    private readonly Mock<IArmazenamentoIdempotencia> _idempotencia = new();

    private EmprestimoService CriarService() => new(
        _emprestimos.Object,
        _jogos.Object,
        _amigos.Object,
        _cache.Object,
        _idempotencia.Object,
        new Mock<ILogger<EmprestimoService>>().Object);

    [Fact]
    public async Task EmprestarAsync_DeveCriarEmprestimoQuandoJogoEAmigoExistem()
    {
        var jogo = new Jogo { Id = 1, Nome = "GTA VI" };
        var amigo = new Amigo { Id = 2, Nome = "Walanem", Sobrenome = "Figueiredo" };

        _jogos.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(jogo);
        _amigos.Setup(r => r.ObterPorIdAsync(2)).ReturnsAsync(amigo);
        _emprestimos.Setup(r => r.ObterAtivoPorJogoAsync(1)).ReturnsAsync((Emprestimo?)null);

        var service = CriarService();
        var dto = await service.EmprestarAsync(new EmprestimoInputDto { JogoId = 1, AmigoId = 2 }, chaveIdempotencia: null);

        dto.JogoNome.Should().Be("GTA VI");
        dto.AmigoNome.Should().Be("Walanem Figueiredo");
        _emprestimos.Verify(r => r.AdicionarAsync(It.IsAny<Emprestimo>()), Times.Once);
    }

    [Fact]
    public async Task EmprestarAsync_DeveRetornarResultadoCacheadoQuandoChaveIdempotenciaJaProcessada()
    {
        var dtoCacheado = new EmprestimoDto { Id = 1, JogoId = 1, JogoNome = "GTA VI", AmigoId = 2, AmigoNome = "Walanem Figueiredo" };
        _idempotencia.Setup(i => i.ObterAsync("emprestimo:criar:chave-1")).ReturnsAsync(JsonSerializer.Serialize(dtoCacheado));

        var service = CriarService();
        var dto = await service.EmprestarAsync(new EmprestimoInputDto(), chaveIdempotencia: "chave-1");

        dto.Id.Should().Be(1);
        dto.JogoNome.Should().Be("GTA VI");
        _emprestimos.Verify(r => r.AdicionarAsync(It.IsAny<Emprestimo>()), Times.Never);
        _jogos.Verify(r => r.ObterPorIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task EmprestarAsync_DeveArmazenarResultadoQuandoChaveIdempotenciaInformadaENaoExisteCache()
    {
        var jogo = new Jogo { Id = 1, Nome = "GTA VI" };
        var amigo = new Amigo { Id = 2, Nome = "Walanem", Sobrenome = "Figueiredo" };

        _idempotencia.Setup(i => i.ObterAsync(It.IsAny<string>())).ReturnsAsync((string?)null);
        _jogos.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(jogo);
        _amigos.Setup(r => r.ObterPorIdAsync(2)).ReturnsAsync(amigo);
        _emprestimos.Setup(r => r.ObterAtivoPorJogoAsync(1)).ReturnsAsync((Emprestimo?)null);

        var service = CriarService();
        await service.EmprestarAsync(new EmprestimoInputDto { JogoId = 1, AmigoId = 2 }, chaveIdempotencia: "chave-2");

        _idempotencia.Verify(i => i.ArmazenarAsync("emprestimo:criar:chave-2", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task EmprestarAsync_DeveLancarNotFoundExceptionQuandoJogoNaoExiste()
    {
        _jogos.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Jogo?)null);

        var service = CriarService();

        await FluentActions.Awaiting(() => service.EmprestarAsync(new EmprestimoInputDto { JogoId = 1, AmigoId = 2 }, chaveIdempotencia: null))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task EmprestarAsync_DeveLancarConflitoExceptionQuandoJogoJaEmprestado()
    {
        var jogo = new Jogo { Id = 1, Nome = "GTA VI" };
        var amigo = new Amigo { Id = 2, Nome = "Walanem", Sobrenome = "Figueiredo" };
        var emprestimoAtivo = new Emprestimo { Id = 5, JogoId = 1, DataEmprestimo = DateTime.UtcNow };

        _jogos.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(jogo);
        _amigos.Setup(r => r.ObterPorIdAsync(2)).ReturnsAsync(amigo);
        _emprestimos.Setup(r => r.ObterAtivoPorJogoAsync(1)).ReturnsAsync(emprestimoAtivo);

        var service = CriarService();

        await FluentActions.Awaiting(() => service.EmprestarAsync(new EmprestimoInputDto { JogoId = 1, AmigoId = 2 }, chaveIdempotencia: null))
            .Should().ThrowAsync<ConflitoException>();
    }

    [Fact]
    public async Task DevolverAsync_DeveRetornarFalseQuandoJaDevolvido()
    {
        var emprestimo = new Emprestimo { Id = 1, JogoId = 1, DataEmprestimo = DateTime.UtcNow, DataDevolucao = DateTime.UtcNow };
        _emprestimos.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(emprestimo);

        var service = CriarService();
        var resultado = await service.DevolverAsync(1);

        resultado.Should().BeFalse();
    }
}
