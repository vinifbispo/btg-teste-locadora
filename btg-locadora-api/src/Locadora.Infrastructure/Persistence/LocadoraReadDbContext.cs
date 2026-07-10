using Locadora.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Infrastructure.Persistence;

public class LocadoraReadDbContext : DbContext
{
    public LocadoraReadDbContext(DbContextOptions<LocadoraReadDbContext> options) : base(options)
    {
    }

    public DbSet<Jogo> Jogos { get; set; }
    public DbSet<Amigo> Amigos { get; set; }
    public DbSet<Emprestimo> Emprestimos { get; set; }
    public DbSet<Genero> Generos { get; set; }
    public DbSet<Desenvolvedor> Desenvolvedores { get; set; }
    public DbSet<Publicadora> Publicadoras { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LocadoraDbContext).Assembly);
    }
}
