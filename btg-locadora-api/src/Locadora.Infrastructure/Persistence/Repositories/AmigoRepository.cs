using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Infrastructure.Persistence.Repositories;

public class AmigoRepository : IAmigoRepository
{
    private readonly LocadoraDbContext _context;

    public AmigoRepository(LocadoraDbContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<Amigo>> ListarAsync()
        => await Task.FromResult<IQueryable<Amigo>>(_context.Amigos.AsNoTracking());

    public async Task<Amigo?> ObterPorIdAsync(int id)
        => await _context.Amigos.FindAsync(id);

    public async Task<bool> ExisteAsync(int id)
        => await _context.Amigos.AnyAsync(a => a.Id == id);

    public async Task AdicionarAsync(Amigo amigo)
    {
        _context.Amigos.Add(amigo);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Amigo amigo)
    {
        _context.Amigos.Update(amigo);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Amigo amigo)
    {
        _context.Amigos.Remove(amigo);
        await _context.SaveChangesAsync();
    }
}
