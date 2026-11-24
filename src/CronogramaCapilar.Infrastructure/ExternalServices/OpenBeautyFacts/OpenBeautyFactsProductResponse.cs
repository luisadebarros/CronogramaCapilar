namespace CronogramaCapilar.Infrastructure.ExternalServices.OpenBeautyFacts;

/// <summary>Resposta de GET /api/v2/product/{código}.</summary>
/// <param name="Status">1 = encontrado; 0 = não encontrado ou código inválido.</param>
internal sealed record OpenBeautyFactsProductResponse(
    int Status,
    OpenBeautyFactsProduct? Product);
