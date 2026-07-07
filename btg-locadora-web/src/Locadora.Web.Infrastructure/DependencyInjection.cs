using Locadora.Web.Application.Interfaces;
using Locadora.Web.Infrastructure.Auth;
using Locadora.Web.Infrastructure.Http;
using Locadora.Web.Infrastructure.Http.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Locadora.Web.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var apiBaseUrl = configuration["ApiBaseUrl"]
            ?? throw new InvalidOperationException("Configuração 'ApiBaseUrl' não encontrada.");

        services.Configure<AutenticacaoApiOptions>(configuration.GetSection("AutenticacaoApi"));
        services.AddSingleton<IApiTokenProvider, ApiTokenProvider>();
        services.AddTransient<AuthorizationTokenHandler>();

        void ConfigurarCliente(HttpClient client)
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        }

        services.AddHttpClient<IAutenticacaoApiClient, AutenticacaoApiClient>(ConfigurarCliente);

        services.AddHttpClient<IAmigoApiClient, AmigoApiClient>(ConfigurarCliente)
            .AddHttpMessageHandler<AuthorizationTokenHandler>();

        services.AddHttpClient<IJogoApiClient, JogoApiClient>(ConfigurarCliente)
            .AddHttpMessageHandler<AuthorizationTokenHandler>();

        services.AddHttpClient<IEmprestimoApiClient, EmprestimoApiClient>(ConfigurarCliente)
            .AddHttpMessageHandler<AuthorizationTokenHandler>();

        services.AddHttpClient<IGeneroApiClient, GeneroApiClient>(ConfigurarCliente)
            .AddHttpMessageHandler<AuthorizationTokenHandler>();

        services.AddHttpClient<IDesenvolvedorApiClient, DesenvolvedorApiClient>(ConfigurarCliente)
            .AddHttpMessageHandler<AuthorizationTokenHandler>();

        services.AddHttpClient<IPublicadoraApiClient, PublicadoraApiClient>(ConfigurarCliente)
            .AddHttpMessageHandler<AuthorizationTokenHandler>();

        return services;
    }
}
