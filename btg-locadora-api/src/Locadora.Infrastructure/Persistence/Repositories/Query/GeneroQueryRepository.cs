using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Infrastructure.Persistence.Repositories.Query;

public class GeneroQueryRepository : IGeneroQueryRepository
{
    private readonly LocadoraReadDbContext _context;

    public GeneroQueryRepository(LocadoraReadDbContext context)
    {
        _context = context;
    }

    public async Task<List<Genero>> BuscarPorNomeAsync(string termo)
        => await _context.Generos
            .Where(g => g.Nome.Contains(termo))
            .OrderBy(g => g.Nome)
            .Take(20)
            .ToListAsync();
}
