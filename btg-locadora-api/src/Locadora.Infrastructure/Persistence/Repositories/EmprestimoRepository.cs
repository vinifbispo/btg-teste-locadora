using Locadora.Domain.Entities;
using Locadora.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Infrastructure.Persistence.Repositories;

public class EmprestimoRepository : IEmprestimoRepository
{
    private readonly LocadoraDbContext _context;

    public EmprestimoRepository(LocadoraDbContext context)
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

    public async Task<Emprestimo?> ObterAtivoPorJogoAsync(int jogoId)
        => await _context.Emprestimos
            .Where(e => e.JogoId == jogoId && e.DataDevolucao == null)
            .FirstOrDefaultAsync();

    public async Task AdicionarAsync(Emprestimo emprestimo)
    {
        _context.Emprestimos.Add(emprestimo);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Emprestimo emprestimo)
    {
        _context.Emprestimos.Update(emprestimo);
        await _context.SaveChangesAsync();
    }
}
