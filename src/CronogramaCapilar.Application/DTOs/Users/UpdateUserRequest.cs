using System.ComponentModel.DataAnnotations;
using CronogramaCapilar.Domain.Entities;
using CronogramaCapilar.Domain.Enums;

namespace CronogramaCapilar.Application.DTOs.Users;

/// <param name="PreferredName">Como quer ser chamado(a). Se vier vazio, usa o primeiro nome.</param>
public sealed record UpdateUserRequest(
    [Required(ErrorMessage = "Informe o nome.")]
    [StringLength(User.NameMaxLength, ErrorMessage = "O nome deve ter no máximo {1} caracteres.")]
    string Name,

    [StringLength(User.PreferredNameMaxLength, ErrorMessage = "Deve ter no máximo {1} caracteres.")]
    string? PreferredName,

    [Required(ErrorMessage = "Informe a data de nascimento.")]
    DateOnly? BirthDate,

    [Required(ErrorMessage = "Informe o gênero.")]
    Gender? Gender);
