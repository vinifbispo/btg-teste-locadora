using Locadora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Locadora.Infrastructure.Persistence.Configurations;

public class EmprestimoConfiguration : IEntityTypeConfiguration<Emprestimo>
{
    public void Configure(EntityTypeBuilder<Emprestimo> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.Jogo)
            .WithMany()
            .HasForeignKey(e => e.JogoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Amigo)
            .WithMany()
            .HasForeignKey(e => e.AmigoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
