using CronogramaCapilar.Application.Common;

namespace CronogramaCapilar.Application.Errors;

public static class AuthErrors
{
    public static readonly Error EmailInUse =
        Error.Conflict("auth.email_in_use", "Já existe uma conta com este e-mail.");

    // Mensagem genérica de propósito: não revela se o e-mail existe ou se a senha está errada.
    public static readonly Error InvalidCredentials =
        Error.Unauthorized("auth.invalid_credentials", "E-mail ou senha inválidos.");

    public static readonly Error InvalidSession =
        Error.Unauthorized("auth.invalid_session", "Sessão inválida ou expirada. Faça login novamente.");
}
