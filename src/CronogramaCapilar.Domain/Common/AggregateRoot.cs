namespace CronogramaCapilar.Domain.Common;

/// <summary>
/// Raiz de agregado: a única porta de entrada para alterar o agregado.
/// Só raízes de agregado têm repositório próprio.
/// </summary>
public abstract class AggregateRoot : Entity
{
    /// <summary>Preenchido automaticamente pela infraestrutura ao salvar.</summary>
    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>Preenchido automaticamente pela infraestrutura ao salvar.</summary>
    public DateTime UpdatedAtUtc { get; private set; }
}
