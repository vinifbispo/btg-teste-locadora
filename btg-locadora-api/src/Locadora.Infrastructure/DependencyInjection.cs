using Hangfire;
using Hangfire.SqlServer;
using Locadora.Domain.Caching;
using Locadora.Domain.Idempotencia;
using Locadora.Domain.Integracoes;
using Locadora.Domain.Repositories;
using Locadora.Infrastructure.Caching;
using Locadora.Infrastructure.Http;
using Locadora.Infrastructure.Idempotencia;
using Locadora.Infrastructure.Persistence;
using Locadora.Infrastructure.Persistence.Repositories.Command;
using Locadora.Infrastructure.Persistence.Repositories.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using StackExchange.Redis;

namespace Locadora.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureHttpClientDefaults(http =>
        {
            http.AddResilienceHandler("retry-padrao", builder =>
            {
                builder.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    BackoffType = DelayBackoffType.Exponential,
                    Delay = TimeSpan.FromSeconds(2)
                });
            });
        });

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("SqlServerDB"), new SqlServerStorageOptions
            {
                PrepareSchemaIfNecessary = false
            }));
        services.AddHangfireServer();

        services.AddDbContext<LocadoraDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SqlServerDB"),
                sql => sql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null)));

        services.AddDbContext<LocadoraReadDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SqlServerDB"),
                    sql => sql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        services.AddStackExchangeRedisCache(options =>
            options.Configuration = configuration.GetConnectionString("Redis"));

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));

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

        services.AddScoped<IJogoQueryRepository, JogoQueryRepository>();
        services.AddScoped<IGeneroQueryRepository, GeneroQueryRepository>();
        services.AddScoped<IDesenvolvedorQueryRepository, DesenvolvedorQueryRepository>();
        services.AddScoped<IPublicadoraQueryRepository, PublicadoraQueryRepository>();
        services.AddScoped<IAmigoQueryRepository, AmigoQueryRepository>();
        services.AddScoped<IEmprestimoQueryRepository, EmprestimoQueryRepository>();

        services.AddSingleton<IArmazenamentoIdempotencia, ArmazenamentoIdempotenciaRedis>();

        services.AddSingleton<IJogoCache, JogoRedisCache>();
        services.AddSingleton<IAmigoCache, AmigoRedisCache>();
        services.AddSingleton<IEmprestimoCache, EmprestimoRedisCache>();

        var jogoExternoApiBaseUrl = configuration["JogoExternoApi:BaseUrl"]
            ?? throw new InvalidOperationException("Configuração 'JogoExternoApi:BaseUrl' não encontrada.");

        services.AddHttpClient<IJogoExternoApiClient, JogoExternoApiClient>(client =>
        {
            client.BaseAddress = new Uri(jogoExternoApiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(60);
        });

        return services;
    }
}
