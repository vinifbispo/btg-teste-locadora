using Locadora.Application.Autenticacao;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Locadora.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.Configure<JwtSettings>(options =>
        {
            options.SecretKey = configuration[$"{JwtSettings.Secao}:{nameof(JwtSettings.SecretKey)}"] ?? options.SecretKey;

            if (int.TryParse(configuration[$"{JwtSettings.Secao}:{nameof(JwtSettings.ExpirationMinutes)}"], out var minutos))
                options.ExpirationMinutes = minutos;
        });

        services.Configure<AdminCredenciaisSettings>(options =>
        {
            options.Usuario = configuration[$"{AdminCredenciaisSettings.Secao}:{nameof(AdminCredenciaisSettings.Usuario)}"] ?? options.Usuario;
            options.Senha = configuration[$"{AdminCredenciaisSettings.Secao}:{nameof(AdminCredenciaisSettings.Senha)}"] ?? options.Senha;
        });

        return services;
    }
}
