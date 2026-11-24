using System.ComponentModel.DataAnnotations;
using CronogramaCapilar.Domain.Enums;
using CronogramaCapilar.Domain.ValueObjects;

namespace CronogramaCapilar.Application.DTOs.Users;

/// <param name="Treatments">Tratamentos que já faz. Pode ficar vazio.</param>
public sealed record HairProfileRequest(
    [Required(ErrorMessage = "Informe o tipo de cabelo.")]
    HairType? HairType,

    [Required(ErrorMessage = "Informe o comprimento do cabelo.")]
    HairLength? Length,

    [Required(ErrorMessage = "Informe quantas vezes lava o cabelo por semana.")]
    [Range(HairProfile.MinWashesPerWeek, HairProfile.MaxWashesPerWeek,
        ErrorMessage = "Lavagens por semana deve estar entre {1} e {2}.")]
    int? WashesPerWeek,

    IReadOnlyList<TreatmentType>? Treatments);
