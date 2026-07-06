using Locadora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Locadora.Infrastructure.Persistence.Configurations;

public class AmigoConfiguration : IEntityTypeConfiguration<Amigo>
{
    public void Configure(EntityTypeBuilder<Amigo> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Sobrenome)
            .IsRequired()
            .HasMaxLength(100);
    }
}
