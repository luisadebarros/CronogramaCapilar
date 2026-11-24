namespace CronogramaCapilar.Domain.Enums;

/// <summary>Os valores começam em 1 de propósito: o 0 (valor padrão) é inválido, então um campo esquecido não passa.</summary>
public enum HairType
{
    /// <summary>Liso.</summary>
    Straight = 1,

    /// <summary>Ondulado.</summary>
    Wavy = 2,

    /// <summary>Cacheado.</summary>
    Curly = 3,

    /// <summary>Crespo.</summary>
    Coily = 4,
}
