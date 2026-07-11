using System.Net;
using Locadora.Infrastructure.UnitTest.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace Locadora.Infrastructure.UnitTest.Http;

public class HttpClientRetryPadraoTests
{
    private static ServiceProvider CriarProviderComRetryPadrao(Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        var services = new ServiceCollection();

        services.ConfigureHttpClientDefaults(http =>
        {
            http.AddResilienceHandler("retry-padrao", builder =>
            {
                builder.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    BackoffType = DelayBackoffType.Exponential,
                    Delay = TimeSpan.FromMilliseconds(1)
                });
            });
        });

        services.AddHttpClient("teste")
            .ConfigurePrimaryHttpMessageHandler(() => new FakeHttpMessageHandler(responder));

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task ClienteHttp_DeveTentarNovamenteAteOLimiteQuandoFalhaSempre()
    {
        var chamadas = 0;
        await using var provider = CriarProviderComRetryPadrao(_ =>
        {
            Interlocked.Increment(ref chamadas);
            return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
        });

        var client = provider.GetRequiredService<IHttpClientFactory>().CreateClient("teste");
        var resposta = await client.GetAsync("http://localhost/teste");

        resposta.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        chamadas.Should().Be(4); // 1 tentativa original + 3 retries
    }

    [Fact]
    public async Task ClienteHttp_DeveRetornarSucessoSeApiExternaSeRecuperarDentroDasTentativas()
    {
        var chamadas = 0;
        await using var provider = CriarProviderComRetryPadrao(_ =>
        {
            Interlocked.Increment(ref chamadas);
            return chamadas < 3
                ? new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                : new HttpResponseMessage(HttpStatusCode.OK);
        });

        var client = provider.GetRequiredService<IHttpClientFactory>().CreateClient("teste");
        var resposta = await client.GetAsync("http://localhost/teste");

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        chamadas.Should().Be(3);
    }
}
