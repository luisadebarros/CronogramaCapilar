using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using CronogramaCapilar.Domain.Exceptions;

namespace CronogramaCapilar.Domain.ValueObjects;

/// <summary>
/// Objeto de valor: não tem identidade, é definido só pelo conteúdo.
/// Garante que todo e-mail do sistema é válido e está normalizado (sem espaços e em minúsculas).
/// </summary>
public sealed partial record Email
{
    public const int MaxLength = 254;

    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string? value) =>
        TryCreate(value, out var email) ? email : throw new DomainException("E-mail inválido.");

    public static bool TryCreate(string? value, [NotNullWhen(true)] out Email? email)
    {
        email = null;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var normalized = value.Trim().ToLowerInvariant();
        if (normalized.Length > MaxLength || !EmailFormat().IsMatch(normalized))
            return false;

        email = new Email(normalized);
        return true;
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailFormat();
}
