namespace CronogramaCapilar.Application.Security;

public enum PasswordVerification
{
    Failed = 1,
    Success = 2,

    /// <summary>Senha correta, mas o hash foi gerado com um algoritmo antigo e deve ser refeito.</summary>
    SuccessRehashNeeded = 3,
}
