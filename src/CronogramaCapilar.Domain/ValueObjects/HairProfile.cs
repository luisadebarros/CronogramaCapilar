using CronogramaCapilar.Domain.Common;
using CronogramaCapilar.Domain.Enums;
using CronogramaCapilar.Domain.Exceptions;

namespace CronogramaCapilar.Domain.ValueObjects;

/// <summary>
/// Perfil capilar: como é o cabelo da pessoa e como é a rotina dela. É um objeto de valor:
/// para mudar, cria-se um perfil novo inteiro (nunca se altera um campo isolado).
/// </summary>
public sealed class HairProfile : IEquatable<HairProfile>
{
    public const int MinWashesPerWeek = 1;
    public const int MaxWashesPerWeek = 7;

    public HairType HairType { get; private set; }
    public HairLength Length { get; private set; }
    public int WashesPerWeek { get; private set; }

    /// <summary>Tratamentos que a pessoa já faz (sem repetição e em ordem).</summary>
    public IReadOnlyList<TreatmentType> Treatments { get; private set; } = [];

    private HairProfile() { } // usado pelo EF Core

    public static HairProfile Create(
        HairType hairType,
        HairLength length,
        int washesPerWeek,
        IEnumerable<TreatmentType>? treatments)
    {
        if (washesPerWeek is < MinWashesPerWeek or > MaxWashesPerWeek)
            throw new DomainException(
                $"Lavagens por semana deve estar entre {MinWashesPerWeek} e {MaxWashesPerWeek}.");

        var validTreatments = (treatments ?? [])
            .Select(treatment => Guard.DefinedEnum(treatment, "Tratamentos"))
            .Distinct()
            .Order()
            .ToList();

        return new HairProfile
        {
            HairType = Guard.DefinedEnum(hairType, "Tipo de cabelo"),
            Length = Guard.DefinedEnum(length, "Comprimento"),
            WashesPerWeek = washesPerWeek,
            Treatments = validTreatments,
        };
    }

    public bool Equals(HairProfile? other) =>
        other is not null
        && HairType == other.HairType
        && Length == other.Length
        && WashesPerWeek == other.WashesPerWeek
        && Treatments.SequenceEqual(other.Treatments);

    public override bool Equals(object? obj) => obj is HairProfile other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(HairType, Length, WashesPerWeek, Treatments.Count);
}
