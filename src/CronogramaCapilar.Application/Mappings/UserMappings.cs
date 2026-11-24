using CronogramaCapilar.Application.DTOs.Users;
using CronogramaCapilar.Domain.Entities;
using CronogramaCapilar.Domain.ValueObjects;

namespace CronogramaCapilar.Application.Mappings;

/// <summary>Converte entidades do domínio nos objetos que a API devolve (o domínio nunca sai cru da API).</summary>
internal static class UserMappings
{
    public static UserResponse ToResponse(this User user, DateOnly today) => new(
        user.Id,
        user.Name,
        user.PreferredName,
        user.Email.Value,
        user.BirthDate,
        user.CalculateAge(today),
        user.Gender,
        user.HairProfile?.ToResponse(),
        user.CreatedAtUtc);

    public static HairProfileResponse ToResponse(this HairProfile hairProfile) => new(
        hairProfile.HairType,
        hairProfile.Length,
        hairProfile.WashesPerWeek,
        hairProfile.Treatments);
}
