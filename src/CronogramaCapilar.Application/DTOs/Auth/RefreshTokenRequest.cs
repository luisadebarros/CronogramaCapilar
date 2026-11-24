using System.ComponentModel.DataAnnotations;

namespace CronogramaCapilar.Application.DTOs.Auth;

public sealed record RefreshTokenRequest(
    [Required(ErrorMessage = "Informe o refresh token.")] string RefreshToken);
