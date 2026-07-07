using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Infrastructure.Persistence.Repositories;

public class DesenvolvedorRepository : IDesenvolvedorRepository
{
    private readonly LocadoraDbContext _context;

    public DesenvolvedorRepository(LocadoraDbContext context)
    {
        _context = context;
    }

    public async Task<List<Desenvolvedor>> ListarPorIdsAsync(IEnumerable<int> ids)
        => await _context.Desenvolvedores.Where(d => ids.Contains(d.Id)).ToListAsync();

    public async Task<List<Desenvolvedor>> ObterOuCriarPorNomesAsync(IEnumerable<string> nomes)
    {
        var nomesDistintos = nomes.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var nomesEmMinusculo = nomesDistintos.Select(n => n.ToLower()).ToList();

        var existentes = await _context.Desenvolvedores
            .Where(d => nomesEmMinusculo.Contains(d.Nome.ToLower()))
            .ToListAsync();

        var faltantes = nomesDistintos
            .Where(nome => !existentes.Any(d => d.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase)))
            .Select(nome => new Desenvolvedor { Nome = nome })
            .ToList();

        if (faltantes.Count > 0)
        {
            _context.Desenvolvedores.AddRange(faltantes);
            await _context.SaveChangesAsync();
        }

        return existentes.Concat(faltantes).ToList();
    }

    public async Task<List<Desenvolvedor>> BuscarPorNomeAsync(string termo)
        => await _context.Desenvolvedores
            .Where(d => d.Nome.Contains(termo))
            .OrderBy(d => d.Nome)
            .Take(20)
            .ToListAsync();
}
