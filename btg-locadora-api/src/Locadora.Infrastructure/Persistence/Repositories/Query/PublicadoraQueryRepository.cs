using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Infrastructure.Persistence.Repositories.Query;

public class PublicadoraQueryRepository : IPublicadoraQueryRepository
{
    private readonly LocadoraReadDbContext _context;

    public PublicadoraQueryRepository(LocadoraReadDbContext context)
    {
        _context = context;
    }

    public async Task<List<Publicadora>> BuscarPorNomeAsync(string termo)
        => await _context.Publicadoras
            .Where(p => p.Nome.Contains(termo))
            .OrderBy(p => p.Nome)
            .Take(20)
            .ToListAsync();
}
