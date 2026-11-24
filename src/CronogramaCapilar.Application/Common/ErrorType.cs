namespace CronogramaCapilar.Application.Common;

/// <summary>Categoria da falha. A API traduz cada uma para um status HTTP.</summary>
public enum ErrorType
{
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Unauthorized = 4,

    /// <summary>Um serviço externo (ex.: pesquisa de produtos) não respondeu.</summary>
    Unavailable = 5,
}
