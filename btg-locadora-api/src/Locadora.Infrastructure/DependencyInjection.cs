using Locadora.Domain.Caching;
using Locadora.Domain.Idempotencia;
using Locadora.Domain.Integracoes;
using Locadora.Domain.Repositories;
using Locadora.Infrastructure.Caching;
using Locadora.Infrastructure.Http;
using Locadora.Infrastructure.Idempotencia;
using Locadora.Infrastructure.Persistence;
using Locadora.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Locadora.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LocadoraDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SqlServerDB")));

        services.AddStackExchangeRedisCache(options =>
            options.Configuration = configuration.GetConnectionString("Redis"));

        services.Configure<IdempotenciaSettings>(options =>
        {
            if (int.TryParse(configuration[$"{IdempotenciaSettings.Secao}:{nameof(IdempotenciaSettings.ExpirationHours)}"], out var horas))
                options.ExpirationHours = horas;
        });

        services.Configure<CacheSettings>(options =>
        {
            if (int.TryParse(configuration[$"{CacheSettings.Secao}:{nameof(CacheSettings.ExpirationMinutes)}"], out var minutos))
                options.ExpirationMinutes = minutos;

            if (int.TryParse(configuration[$"{CacheSettings.Secao}:{nameof(CacheSettings.TimeoutMilliseconds)}"], out var ms))
                options.TimeoutMilliseconds = ms;
        });

        services.AddScoped<IJogoRepository, JogoRepository>();
        services.AddScoped<IGeneroRepository, GeneroRepository>();
        services.AddScoped<IDesenvolvedorRepository, DesenvolvedorRepository>();
        services.AddScoped<IPublicadoraRepository, PublicadoraRepository>();
        services.AddScoped<IAmigoRepository, AmigoRepository>();
        services.AddScoped<IEmprestimoRepository, EmprestimoRepository>();
        services.AddSingleton<IArmazenamentoIdempotencia, ArmazenamentoIdempotenciaRedis>();

        services.AddSingleton<IJogoCache, JogoRedisCache>();
        services.AddSingleton<IAmigoCache, AmigoRedisCache>();
        services.AddSingleton<IEmprestimoCache, EmprestimoRedisCache>();

        var jogoExternoApiBaseUrl = configuration["JogoExternoApi:BaseUrl"]
            ?? throw new InvalidOperationException("Configuração 'JogoExternoApi:BaseUrl' não encontrada.");

        services.AddHttpClient<IJogoExternoApiClient, JogoExternoApiClient>(client =>
        {
            client.BaseAddress = new Uri(jogoExternoApiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}
