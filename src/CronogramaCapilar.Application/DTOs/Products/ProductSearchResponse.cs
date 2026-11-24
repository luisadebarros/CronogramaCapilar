namespace CronogramaCapilar.Application.DTOs.Products;

/// <param name="HasMore">Se existe próxima página (para a rolagem infinita do app).</param>
public sealed record ProductSearchResponse(
    IReadOnlyList<ProductResponse> Items,
    int Page,
    int PageSize,
    bool HasMore);
