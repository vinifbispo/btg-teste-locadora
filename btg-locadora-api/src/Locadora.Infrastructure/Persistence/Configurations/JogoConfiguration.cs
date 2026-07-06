using Locadora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Locadora.Infrastructure.Persistence.Configurations;

public class JogoConfiguration : IEntityTypeConfiguration<Jogo>
{
    public void Configure(EntityTypeBuilder<Jogo> builder)
    {
        builder.HasKey(j => j.Id);

        builder.Property(j => j.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(j => j.ImagemCapa)
            .HasMaxLength(500);

        builder.Property(j => j.Console)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(10);
    }
}
