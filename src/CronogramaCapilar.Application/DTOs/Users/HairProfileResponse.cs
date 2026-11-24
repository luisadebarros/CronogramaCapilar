using CronogramaCapilar.Domain.Enums;

namespace CronogramaCapilar.Application.DTOs.Users;

public sealed record HairProfileResponse(
    HairType HairType,
    HairLength Length,
    int WashesPerWeek,
    IReadOnlyList<TreatmentType> Treatments);
