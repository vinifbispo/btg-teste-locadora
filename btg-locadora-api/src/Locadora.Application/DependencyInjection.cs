using Locadora.Application.Autenticacao;
using Locadora.Application.Interfaces;
using Locadora.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Locadora.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IJogoService, JogoService>();
        services.AddScoped<IAmigoService, AmigoService>();
        services.AddScoped<IEmprestimoService, EmprestimoService>();
        services.AddScoped<IDesenvolvedorService, DesenvolvedorService>();
        services.AddScoped<IPublicadoraService, PublicadoraService>();
        services.AddScoped<IGeneroService, GeneroService>();

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

        services.AddScoped<IAutenticacaoService, AutenticacaoService>();

        return services;
    }
}
