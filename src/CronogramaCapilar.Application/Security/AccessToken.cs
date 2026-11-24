namespace CronogramaCapilar.Application.Security;

public sealed record AccessToken(string Token, DateTime ExpiresAtUtc);
