namespace CronogramaCapilar.Domain.Enums;

/// <summary>Os valores começam em 1 de propósito: o 0 (valor padrão) é inválido, então um campo esquecido não passa.</summary>
public enum TreatmentType
{
    /// <summary>Hidratação: repõe a água do fio (maciez e brilho).</summary>
    Hydration = 1,

    /// <summary>Nutrição: repõe os lipídios do fio (contra ressecamento e frizz).</summary>
    Nutrition = 2,

    /// <summary>Reconstrução: repõe a massa do fio (queratina e proteínas) em cabelo danificado.</summary>
    Reconstruction = 3,

    /// <summary>Umectação: óleo aplicado antes da lavagem.</summary>
    Oiling = 4,

    /// <summary>Acidificação: equilibra o pH e fecha as cutículas.</summary>
    Acidification = 5,

    /// <summary>Limpeza profunda: shampoo antirresíduo / detox para tirar o acúmulo de produto.</summary>
    DeepCleansing = 6,
}
