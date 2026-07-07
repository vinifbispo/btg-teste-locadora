using Locadora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Locadora.Infrastructure.Persistence.Configurations;

public class PublicadoraConfiguration : IEntityTypeConfiguration<Publicadora>
{
    public void Configure(EntityTypeBuilder<Publicadora> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(p => p.Nome).IsUnique();
    }
}
