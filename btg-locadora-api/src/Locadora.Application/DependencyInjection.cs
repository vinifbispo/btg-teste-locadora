using Locadora.Application.Interfaces;
using Locadora.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Locadora.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IJogoService, JogoService>();
        services.AddScoped<IAmigoService, AmigoService>();
        services.AddScoped<IEmprestimoService, EmprestimoService>();

        return services;
    }
}
