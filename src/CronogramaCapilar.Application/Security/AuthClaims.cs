namespace CronogramaCapilar.Application.Security;

/// <summary>
/// Claims que vão dentro do access token. Só o id do usuário: o JWT pode ser lido por
/// qualquer um (é só Base64), então nada de dados pessoais nele.
/// </summary>
public static class AuthClaims
{
    public const string UserId = "sub";
}
