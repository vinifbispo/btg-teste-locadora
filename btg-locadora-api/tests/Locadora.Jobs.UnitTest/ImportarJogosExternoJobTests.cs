using Locadora.Application.Commands.Jogos.ImportarJogosExterno;
using MediatR;

namespace Locadora.Jobs.UnitTest;

public class ImportarJogosExternoJobTests
{
    [Fact]
    public async Task ExecutarAsync_DeveEnviarImportarJogosExternoCommandComOTokenRecebido()
    {
        var sender = new Mock<ISender>();
        var job = new ImportarJogosExternoJob(sender.Object);
        using var cts = new CancellationTokenSource();

        await job.ExecutarAsync(cts.Token);

        sender.Verify(s => s.Send(It.IsAny<ImportarJogosExternoCommand>(), cts.Token), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DevePropagarExcecaoDoSender()
    {
        var sender = new Mock<ISender>();
        sender.Setup(s => s.Send(It.IsAny<ImportarJogosExternoCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Falha simulada"));
        var job = new ImportarJogosExternoJob(sender.Object);

        await FluentActions.Awaiting(() => job.ExecutarAsync(CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>();
    }
}
