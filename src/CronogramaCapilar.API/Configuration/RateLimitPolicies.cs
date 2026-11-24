namespace CronogramaCapilar.API.Configuration;

public static class RateLimitPolicies
{
    /// <summary>Limita tentativas de login/cadastro por IP (protege contra quem tenta adivinhar senhas).</summary>
    public const string Auth = "auth";

    /// <summary>Limita pesquisas de produto por usuário, para ninguém esgotar sozinho a cota do Open Beauty Facts.</summary>
    public const string ProductSearch = "product-search";
}
