namespace CronogramaCapilar.Domain.Enums;

/// <summary>Os valores começam em 1 de propósito: o 0 (valor padrão) é inválido, então um campo esquecido não passa.</summary>
public enum HairLength
{
    /// <summary>Muito curto: raspado ou na máquina.</summary>
    VeryShort = 1,

    /// <summary>Curto: até a altura do queixo.</summary>
    Short = 2,

    /// <summary>Médio: até os ombros.</summary>
    Medium = 3,

    /// <summary>Longo: abaixo dos ombros, até o meio das costas.</summary>
    Long = 4,

    /// <summary>Muito longo: da cintura para baixo.</summary>
    VeryLong = 5,
}
