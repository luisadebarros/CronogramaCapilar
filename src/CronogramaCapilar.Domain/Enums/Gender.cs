namespace CronogramaCapilar.Domain.Enums;

/// <summary>Os valores começam em 1 de propósito: o 0 (valor padrão) é inválido, então um campo esquecido não passa.</summary>
public enum Gender
{
    /// <summary>Feminino.</summary>
    Female = 1,

    /// <summary>Masculino.</summary>
    Male = 2,

    /// <summary>Outro.</summary>
    Other = 3,

    /// <summary>Prefiro não informar.</summary>
    PreferNotToSay = 4,
}
