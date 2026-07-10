using Locadora.Domain.Entities;
using Locadora.Domain.Exceptions;
using Locadora.Domain.Repositories;

namespace Locadora.Application.Commands.Jogos;

internal static class JogoAssociacoesResolver
{
    public static async Task<List<Genero>> ResolverGenerosAsync(IGeneroRepository repositorio, List<int> ids)
    {
        var generos = await repositorio.ListarPorIdsAsync(ids);
        var faltantes = ids.Except(generos.Select(g => g.Id)).ToList();
        if (faltantes.Count > 0)
            throw new NotFoundException($"Gênero(s) não encontrado(s): {string.Join(", ", faltantes)}.");

        return generos;
    }

    public static async Task<List<Desenvolvedor>> ResolverDesenvolvedoresAsync(IDesenvolvedorRepository repositorio, List<int> ids)
    {
        var desenvolvedores = await repositorio.ListarPorIdsAsync(ids);
        var faltantes = ids.Except(desenvolvedores.Select(d => d.Id)).ToList();
        if (faltantes.Count > 0)
            throw new NotFoundException($"Desenvolvedor(es) não encontrado(s): {string.Join(", ", faltantes)}.");

        return desenvolvedores;
    }

    public static async Task<List<Publicadora>> ResolverPublicadorasAsync(IPublicadoraRepository repositorio, List<int> ids)
    {
        var publicadoras = await repositorio.ListarPorIdsAsync(ids);
        var faltantes = ids.Except(publicadoras.Select(p => p.Id)).ToList();
        if (faltantes.Count > 0)
            throw new NotFoundException($"Publicadora(s) não encontrada(s): {string.Join(", ", faltantes)}.");

        return publicadoras;
    }
}
