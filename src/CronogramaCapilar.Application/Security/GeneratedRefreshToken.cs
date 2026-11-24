namespace CronogramaCapilar.Application.Security;

/// <param name="Token">Valor entregue ao app. Nunca é salvo no banco.</param>
/// <param name="Hash">O que é salvo no banco.</param>
public sealed record GeneratedRefreshToken(string Token, string Hash, DateTime ExpiresAtUtc);
