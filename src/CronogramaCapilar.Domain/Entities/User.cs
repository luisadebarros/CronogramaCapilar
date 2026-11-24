using CronogramaCapilar.Domain.Common;
using CronogramaCapilar.Domain.Enums;
using CronogramaCapilar.Domain.Exceptions;
using CronogramaCapilar.Domain.ValueObjects;

namespace CronogramaCapilar.Domain.Entities;

/// <summary>
/// Pessoa que usa o app. É a raiz do agregado que inclui o perfil capilar.
/// Os setters são privados: só dá pra mudar o estado pelos métodos, que validam as regras.
/// </summary>
public sealed class User : AggregateRoot
{
    public const int NameMaxLength = 100;
    public const int PreferredNameMaxLength = 50;
    public const int MaxAge = 120;

    public string Name { get; private set; } = string.Empty;

    /// <summary>Como a pessoa quer ser chamada no app.</summary>
    public string PreferredName { get; private set; } = string.Empty;

    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = string.Empty;
    public DateOnly BirthDate { get; private set; }
    public Gender Gender { get; private set; }

    /// <summary>Fica nulo até a pessoa responder o questionário sobre o cabelo.</summary>
    public HairProfile? HairProfile { get; private set; }

    private User() { } // usado pelo EF Core

    public static User Create(
        string name,
        string? preferredName,
        Email email,
        string passwordHash,
        DateOnly birthDate,
        Gender gender,
        DateOnly today)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        var user = new User { Email = email, PasswordHash = passwordHash };
        user.UpdatePersonalInfo(name, preferredName, birthDate, gender, today);
        return user;
    }

    /// <summary>Se <paramref name="preferredName"/> vier vazio, usa o primeiro nome.</summary>
    public void UpdatePersonalInfo(
        string name,
        string? preferredName,
        DateOnly birthDate,
        Gender gender,
        DateOnly today)
    {
        // Valida tudo antes de alterar qualquer campo: ou muda tudo, ou não muda nada.
        var validName = Guard.RequiredText(name, "Nome", NameMaxLength);
        var validPreferredName = string.IsNullOrWhiteSpace(preferredName)
            ? GetFirstName(validName)
            : Guard.RequiredText(preferredName, "Como quer ser chamado(a)", PreferredNameMaxLength);
        ValidateBirthDate(birthDate, today);
        var validGender = Guard.DefinedEnum(gender, "Gênero");

        Name = validName;
        PreferredName = validPreferredName;
        BirthDate = birthDate;
        Gender = validGender;
    }

    public void SetHairProfile(HairProfile hairProfile)
    {
        ArgumentNullException.ThrowIfNull(hairProfile);
        HairProfile = hairProfile;
    }

    public void UpdatePasswordHash(string newPasswordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newPasswordHash);
        PasswordHash = newPasswordHash;
    }

    public int CalculateAge(DateOnly today)
    {
        var age = today.Year - BirthDate.Year;
        return BirthDate > today.AddYears(-age) ? age - 1 : age;
    }

    private static void ValidateBirthDate(DateOnly birthDate, DateOnly today)
    {
        if (birthDate > today)
            throw new DomainException("A data de nascimento não pode estar no futuro.");

        if (birthDate < today.AddYears(-MaxAge))
            throw new DomainException("Data de nascimento inválida.");
    }

    private static string GetFirstName(string name)
    {
        var firstName = name.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
        return firstName.Length <= PreferredNameMaxLength
            ? firstName
            : firstName[..PreferredNameMaxLength];
    }
}
