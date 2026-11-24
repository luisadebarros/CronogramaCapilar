using CronogramaCapilar.Domain.Exceptions;

namespace CronogramaCapilar.Domain.Common;

/// <summary>
/// Validações reaproveitadas pelas entidades e objetos de valor.
/// O <c>fieldName</c> é o nome do campo como a pessoa vê, porque vai na mensagem de erro.
/// </summary>
internal static class Guard
{
    public static string RequiredText(string? value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"O campo '{fieldName}' é obrigatório.");

        var text = value.Trim();
        if (text.Length > maxLength)
            throw new DomainException($"O campo '{fieldName}' deve ter no máximo {maxLength} caracteres.");

        return text;
    }

    public static TEnum DefinedEnum<TEnum>(TEnum value, string fieldName) where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
            throw new DomainException($"O valor informado em '{fieldName}' não é válido.");

        return value;
    }
}
