using Locadora.Application.Commands.Jogos.ImportarJogosExterno;
using Locadora.Application.UnitTest.TestHelpers;
using Locadora.Domain.Caching;
using Locadora.Domain.Entities;
using Locadora.Domain.Integracoes;
using Locadora.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Locadora.Application.UnitTest.Commands.Jogos;

public class ImportarJogosExternoCommandHandlerTests
{
    private readonly Mock<IJogoRepository> _jogos = new();
    private readonly Mock<IGeneroRepository> _generos = new();
    private readonly Mock<IDesenvolvedorRepository> _desenvolvedores = new();
    private readonly Mock<IPublicadoraRepository> _publicadoras = new();
    private readonly Mock<IJogoCache> _cache = new();
    private readonly Mock<IJogoExternoApiClient> _jogoExterno = new();

    private ImportarJogosExternoCommandHandler CriarHandler() => new(
        _jogos.Object,
        _generos.Object,
        _desenvolvedores.Object,
        _publicadoras.Object,
        _cache.Object,
        _jogoExterno.Object,
        new Mock<ILogger<ImportarJogosExternoCommandHandler>>().Object);

    [Fact]
    public async Task Handle_DeveIgnorarQuandoJaExistemJogos()
    {
        var existentes = new TestAsyncEnumerable<Jogo>([new Jogo { Id = 1, Nome = "EA FC 26" }]);
        _jogos.Setup(r => r.ListarAsync()).ReturnsAsync(existentes);

        var handler = CriarHandler();
        await handler.Handle(new ImportarJogosExternoCommand(), CancellationToken.None);

        _jogoExterno.Verify(c => c.ListarAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
