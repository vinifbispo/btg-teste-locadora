using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Infrastructure.Persistence.Repositories.Command;

public class PublicadoraRepository : IPublicadoraRepository
{
    private readonly LocadoraDbContext _context;

    public PublicadoraRepository(LocadoraDbContext context)
    {
        _context = context;
    }

    public async Task<List<Publicadora>> ListarPorIdsAsync(IEnumerable<int> ids)
        => await _context.Publicadoras.Where(p => ids.Contains(p.Id)).ToListAsync();

    public async Task<List<Publicadora>> ObterOuCriarPorNomesAsync(IEnumerable<string> nomes)
    {
        var nomesDistintos = nomes.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var nomesEmMinusculo = nomesDistintos.Select(n => n.ToLower()).ToList();

        var existentes = await _context.Publicadoras
            .Where(p => nomesEmMinusculo.Contains(p.Nome.ToLower()))
            .ToListAsync();

        var faltantes = nomesDistintos
            .Where(nome => !existentes.Any(p => p.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase)))
            .Select(nome => new Publicadora { Nome = nome })
            .ToList();

        if (faltantes.Count > 0)
        {
            _context.Publicadoras.AddRange(faltantes);
            await _context.SaveChangesAsync();
        }

        return existentes.Concat(faltantes).ToList();
    }
}
