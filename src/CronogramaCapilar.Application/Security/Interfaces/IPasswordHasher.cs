namespace CronogramaCapilar.Application.Security.Interfaces;

public interface IPasswordHasher
{
    string Hash(string password);

    /// <summary>
    /// Passe <c>null</c> em <paramref name="passwordHash"/> quando o usuário não existir: a verificação
    /// roda mesmo assim, para o tempo de resposta não revelar quais e-mails têm conta.
    /// </summary>
    PasswordVerification Verify(string? passwordHash, string password);
}
