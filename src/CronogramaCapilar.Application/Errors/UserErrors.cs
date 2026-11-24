using CronogramaCapilar.Application.Common;

namespace CronogramaCapilar.Application.Errors;

public static class UserErrors
{
    public static readonly Error NotFound =
        Error.NotFound("user.not_found", "Usuário não encontrado.");
}
