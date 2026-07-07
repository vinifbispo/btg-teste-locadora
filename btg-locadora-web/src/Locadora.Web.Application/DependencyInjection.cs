using Locadora.Web.Application.Interfaces;
using Locadora.Web.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Locadora.Web.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAmigoService, AmigoService>();
        services.AddScoped<IJogoService, JogoService>();
        services.AddScoped<IEmprestimoService, EmprestimoService>();
        services.AddScoped<IGeneroService, GeneroService>();
        services.AddScoped<IDesenvolvedorService, DesenvolvedorService>();
        services.AddScoped<IPublicadoraService, PublicadoraService>();

        return services;
    }
}
