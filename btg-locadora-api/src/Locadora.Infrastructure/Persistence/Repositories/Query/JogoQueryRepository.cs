using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Infrastructure.Persistence.Repositories.Query;

public class JogoQueryRepository : IJogoQueryRepository
{
    private readonly LocadoraReadDbContext _context;

    public JogoQueryRepository(LocadoraReadDbContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<Jogo>> ListarAsync()
        => await Task.FromResult<IQueryable<Jogo>>(_context.Jogos
            .Include(j => j.Generos)
            .Include(j => j.Desenvolvedores)
            .Include(j => j.Publicadoras)
            .AsNoTracking());

    public async Task<Jogo?> ObterPorIdAsync(int id)
        => await _context.Jogos
            .Include(j => j.Generos)
            .Include(j => j.Desenvolvedores)
            .Include(j => j.Publicadoras)
            .FirstOrDefaultAsync(j => j.Id == id);
}
