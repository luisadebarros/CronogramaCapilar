using CronogramaCapilar.Domain.Entities;
using CronogramaCapilar.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CronogramaCapilar.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    private const int EnumMaxLength = 30;

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id).ValueGeneratedNever();

        builder.Property(user => user.Name)
            .HasMaxLength(User.NameMaxLength)
            .IsRequired();

        builder.Property(user => user.PreferredName)
            .HasMaxLength(User.PreferredNameMaxLength)
            .IsRequired();

        // Objeto de valor guardado como uma coluna de texto comum.
        builder.Property(user => user.Email)
            .HasConversion(email => email.Value, value => Email.Create(value))
            .HasMaxLength(Email.MaxLength)
            .IsRequired();

        builder.HasIndex(user => user.Email).IsUnique();

        builder.Property(user => user.PasswordHash)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(user => user.BirthDate).IsRequired();

        // Enums salvos como texto ("Female"): o banco fica legível e não quebra se a ordem do enum mudar.
        builder.Property(user => user.Gender)
            .HasConversion<string>()
            .HasMaxLength(EnumMaxLength)
            .IsRequired();

        // Perfil capilar: objeto de valor do usuário, guardado numa tabela própria (1 para 1).
        builder.OwnsOne(user => user.HairProfile, hairProfile =>
        {
            hairProfile.ToTable("HairProfiles");
            hairProfile.WithOwner().HasForeignKey("UserId");

            hairProfile.Property(p => p.HairType)
                .HasConversion<string>()
                .HasMaxLength(EnumMaxLength)
                .IsRequired();

            hairProfile.Property(p => p.Length)
                .HasConversion<string>()
                .HasMaxLength(EnumMaxLength)
                .IsRequired();

            hairProfile.Property(p => p.WashesPerWeek).IsRequired();

            // Lista guardada como JSON numa coluna: ["Hydration","Nutrition"].
            hairProfile.PrimitiveCollection(p => p.Treatments)
                .ElementType(element => element.HasConversion<string>())
                .IsRequired();
        });

        builder.Navigation(user => user.HairProfile).IsRequired(false);
    }
}
