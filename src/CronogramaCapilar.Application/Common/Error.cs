namespace CronogramaCapilar.Application.Common;

/// <param name="Code">Código estável que o app pode usar para decidir o que fazer (ex.: "auth.email_in_use").</param>
/// <param name="Message">Mensagem em português para mostrar à pessoa.</param>
public sealed record Error(string Code, string Message, ErrorType Type)
{
    public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);

    public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);

    public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);

    public static Error Unauthorized(string code, string message) => new(code, message, ErrorType.Unauthorized);

    public static Error Unavailable(string code, string message) => new(code, message, ErrorType.Unavailable);
}
