using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.Configuration;

namespace Locadora.Jobs.UnitTest;

public class HangfireInitializerTests
{
    [Fact]
    public async Task InitializeAsync_DeveLancarInvalidOperationExceptionQuandoConexaoComBancoFalha()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:SqlServerDB"] = "Server=127.0.0.1,1;Database=Teste;Connect Timeout=1;TrustServerCertificate=True;Encrypt=False;"
            })
            .Build();
        var backgroundJobClient = new Mock<IBackgroundJobClient>();

        await FluentActions.Awaiting(() => HangfireInitializer.InitializeAsync(configuration, backgroundJobClient.Object))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Falha ao preparar o schema do Hangfire*");

        backgroundJobClient.Verify(c => c.Create(It.IsAny<Job>(), It.IsAny<IState>()), Times.Never);
    }
}
