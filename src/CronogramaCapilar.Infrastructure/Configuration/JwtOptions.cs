using System.ComponentModel.DataAnnotations;

namespace CronogramaCapilar.Infrastructure.Configuration;

/// <summary>
/// Configurações do JWT, lidas da seção "Jwt" (appsettings + user-secrets / variáveis de ambiente).
/// </summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    /// <summary>
    /// Chave secreta de assinatura (HMAC-SHA256), com no mínimo 32 caracteres.
    /// NUNCA no appsettings.json: use user-secrets em dev e variável de ambiente em produção.
    /// </summary>
    [Required, MinLength(32)]
    public string Key { get; init; } = string.Empty;

    [Range(1, 60)]
    public int AccessTokenMinutes { get; init; } = 15;

    [Range(1, 365)]
    public int RefreshTokenDays { get; init; } = 30;
}
