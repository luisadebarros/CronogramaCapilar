using System.Security.Claims;
using CronogramaCapilar.Application.Security;

namespace CronogramaCapilar.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal) =>
        Guid.TryParse(principal.FindFirstValue(AuthClaims.UserId), out var id)
            ? id
            : throw new InvalidOperationException("O token não tem o identificador do usuário.");
}
