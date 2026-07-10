using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Infrastructure.Persistence.Repositories.Command;

public class GeneroRepository : IGeneroRepository
{
    private readonly LocadoraDbContext _context;

    public GeneroRepository(LocadoraDbContext context)
    {
        _context = context;
    }

    public async Task<List<Genero>> ListarPorIdsAsync(IEnumerable<int> ids)
        => await _context.Generos.Where(g => ids.Contains(g.Id)).ToListAsync();

    public async Task<List<Genero>> ObterOuCriarPorNomesAsync(IEnumerable<string> nomes)
    {
        var nomesDistintos = nomes.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var nomesEmMinusculo = nomesDistintos.Select(n => n.ToLower()).ToList();

        var existentes = await _context.Generos
            .Where(g => nomesEmMinusculo.Contains(g.Nome.ToLower()))
            .ToListAsync();

        var faltantes = nomesDistintos
            .Where(nome => !existentes.Any(g => g.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase)))
            .Select(nome => new Genero { Nome = nome })
            .ToList();

        if (faltantes.Count > 0)
        {
            _context.Generos.AddRange(faltantes);
            await _context.SaveChangesAsync();
        }

        return existentes.Concat(faltantes).ToList();
    }
}
