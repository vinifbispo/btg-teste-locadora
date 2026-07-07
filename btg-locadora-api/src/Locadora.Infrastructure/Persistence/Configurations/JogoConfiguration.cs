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

        builder.OwnsMany(j => j.DatasLancamento, datas =>
        {
            datas.ToTable("JogoDatasLancamento");
            datas.WithOwner().HasForeignKey("JogoId");
            datas.Property<int>("Id");
            datas.HasKey("Id");

            datas.Property(d => d.Regiao)
                .IsRequired()
                .HasMaxLength(50);

            datas.Property(d => d.Data)
                .IsRequired()
                .HasMaxLength(50);
        });
    }
}
