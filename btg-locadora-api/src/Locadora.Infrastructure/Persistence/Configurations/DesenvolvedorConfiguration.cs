using Locadora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Locadora.Infrastructure.Persistence.Configurations;

public class DesenvolvedorConfiguration : IEntityTypeConfiguration<Desenvolvedor>
{
    public void Configure(EntityTypeBuilder<Desenvolvedor> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(d => d.Nome).IsUnique();
    }
}
