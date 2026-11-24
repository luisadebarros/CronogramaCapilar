namespace CronogramaCapilar.API.Configuration;

public static class RateLimitPolicies
{
    /// <summary>Limita tentativas de login/cadastro por IP (protege contra quem tenta adivinhar senhas).</summary>
    public const string Auth = "auth";
}
