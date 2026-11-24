using CronogramaCapilar.Domain.Entities;

namespace CronogramaCapilar.Application.Security.Interfaces;

public interface ITokenService
{
    AccessToken GenerateAccessToken(User user);

    GeneratedRefreshToken GenerateRefreshToken();

    string ComputeHash(string refreshToken);
}
