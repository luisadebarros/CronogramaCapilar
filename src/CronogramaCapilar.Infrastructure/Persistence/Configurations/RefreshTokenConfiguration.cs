using CronogramaCapilar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CronogramaCapilar.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(token => token.Id);
        builder.Property(token => token.Id).ValueGeneratedNever();

        builder.Property(token => token.TokenHash)
            .HasMaxLength(RefreshToken.TokenHashLength)
            .IsFixedLength()
            .IsRequired();

        builder.HasIndex(token => token.TokenHash).IsUnique();

        // Agregados se referenciam só pelo Id. No banco vira FK com cascade:
        // excluir o usuário apaga as sessões dele junto.
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(token => token.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
