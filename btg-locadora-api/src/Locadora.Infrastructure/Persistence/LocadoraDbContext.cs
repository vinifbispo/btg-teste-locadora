using Locadora.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Infrastructure.Persistence;

public class LocadoraDbContext : DbContext
{
    public LocadoraDbContext(DbContextOptions<LocadoraDbContext> options) : base(options)
    {
    }

    public DbSet<Jogo> Jogos { get; set; }
    public DbSet<Amigo> Amigos { get; set; }
    public DbSet<Emprestimo> Emprestimos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LocadoraDbContext).Assembly);
    }
}
