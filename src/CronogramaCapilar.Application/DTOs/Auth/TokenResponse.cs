namespace CronogramaCapilar.Application.DTOs.Auth;

/// <param name="AccessToken">Vai no header "Authorization: Bearer ..." de cada requisição. Dura pouco.</param>
/// <param name="RefreshToken">Guardado no celular (armazenamento seguro). Troca por tokens novos sem pedir senha.</param>
public sealed record TokenResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc)
{
    public string TokenType => "Bearer";
}
