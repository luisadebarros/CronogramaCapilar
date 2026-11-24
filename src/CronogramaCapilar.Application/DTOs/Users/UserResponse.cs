using CronogramaCapilar.Domain.Enums;

namespace CronogramaCapilar.Application.DTOs.Users;

/// <param name="HairProfile">Nulo enquanto a pessoa não responder o questionário sobre o cabelo.</param>
public sealed record UserResponse(
    Guid Id,
    string Name,
    string PreferredName,
    string Email,
    DateOnly BirthDate,
    int Age,
    Gender Gender,
    HairProfileResponse? HairProfile,
    DateTime CreatedAtUtc);
