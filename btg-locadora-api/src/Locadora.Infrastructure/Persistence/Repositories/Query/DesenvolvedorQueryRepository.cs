using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Infrastructure.Persistence.Repositories.Query;

public class DesenvolvedorQueryRepository : IDesenvolvedorQueryRepository
{
    private readonly LocadoraReadDbContext _context;

    public DesenvolvedorQueryRepository(LocadoraReadDbContext context)
    {
        _context = context;
    }

    public async Task<List<Desenvolvedor>> BuscarPorNomeAsync(string termo)
        => await _context.Desenvolvedores
            .Where(d => d.Nome.Contains(termo))
            .OrderBy(d => d.Nome)
            .Take(20)
            .ToListAsync();
}
