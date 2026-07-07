using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Infrastructure.Persistence.Repositories;

public class JogoRepository : IJogoRepository
{
    private readonly LocadoraDbContext _context;

    public JogoRepository(LocadoraDbContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<Jogo>> ListarAsync()
        => await Task.FromResult<IQueryable<Jogo>>(_context.Jogos.AsNoTracking());

    public async Task<Jogo?> ObterPorIdAsync(int id)
        => await _context.Jogos.FirstOrDefaultAsync(j => j.Id == id);

    public async Task AdicionarAsync(Jogo jogo)
    {
        _context.Jogos.Add(jogo);
        await _context.SaveChangesAsync();
    }

    public async Task AdicionarVariosAsync(IEnumerable<Jogo> jogos)
    {
        _context.Jogos.AddRange(jogos);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Jogo jogo)
    {
        _context.Jogos.Update(jogo);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Jogo jogo)
    {
        _context.Jogos.Remove(jogo);
        await _context.SaveChangesAsync();
    }
}
