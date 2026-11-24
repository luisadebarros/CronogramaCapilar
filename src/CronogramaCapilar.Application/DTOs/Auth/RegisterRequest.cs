using System.ComponentModel.DataAnnotations;
using CronogramaCapilar.Domain.Entities;
using CronogramaCapilar.Domain.Enums;
using CronogramaCapilar.Domain.ValueObjects;

namespace CronogramaCapilar.Application.DTOs.Auth;

/// <param name="PreferredName">Como quer ser chamado(a). Se vier vazio, usa o primeiro nome.</param>
public sealed record RegisterRequest(
    [Required(ErrorMessage = "Informe o nome.")]
    [StringLength(User.NameMaxLength, ErrorMessage = "O nome deve ter no máximo {1} caracteres.")]
    string Name,

    [StringLength(User.PreferredNameMaxLength, ErrorMessage = "Deve ter no máximo {1} caracteres.")]
    string? PreferredName,

    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [StringLength(Email.MaxLength, ErrorMessage = "O e-mail deve ter no máximo {1} caracteres.")]
    string Email,

    [Required(ErrorMessage = "Informe a senha.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "A senha deve ter entre {2} e {1} caracteres.")]
    string Password,

    [Required(ErrorMessage = "Informe a data de nascimento.")]
    DateOnly? BirthDate,

    [Required(ErrorMessage = "Informe o gênero.")]
    Gender? Gender);
