using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Infrastructure.Persistence.Repositories.Query;

public class AmigoQueryRepository : IAmigoQueryRepository
{
    private readonly LocadoraReadDbContext _context;

    public AmigoQueryRepository(LocadoraReadDbContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<Amigo>> ListarAsync()
        => await Task.FromResult<IQueryable<Amigo>>(_context.Amigos.AsNoTracking());

    public async Task<Amigo?> ObterPorIdAsync(int id)
        => await _context.Amigos.FirstOrDefaultAsync(a => a.Id == id);
}
