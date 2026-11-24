using System.ComponentModel.DataAnnotations;

namespace CronogramaCapilar.Application.DTOs.Auth;

public sealed record LoginRequest(
    [Required(ErrorMessage = "Informe o e-mail.")] string Email,
    [Required(ErrorMessage = "Informe a senha.")] string Password);
