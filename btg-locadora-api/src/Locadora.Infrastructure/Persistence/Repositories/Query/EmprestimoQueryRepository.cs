using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Infrastructure.Persistence.Repositories.Query;

public class EmprestimoQueryRepository : IEmprestimoQueryRepository
{
    private readonly LocadoraReadDbContext _context;

    public EmprestimoQueryRepository(LocadoraReadDbContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<Emprestimo>> ListarAsync()
        => await Task.FromResult<IQueryable<Emprestimo>>(_context.Emprestimos
            .Include(e => e.Jogo)
            .Include(e => e.Amigo)
            .AsNoTracking());

    public async Task<Emprestimo?> ObterPorIdAsync(int id)
        => await _context.Emprestimos
            .Include(e => e.Jogo)
            .Include(e => e.Amigo)
            .FirstOrDefaultAsync(e => e.Id == id);
}
