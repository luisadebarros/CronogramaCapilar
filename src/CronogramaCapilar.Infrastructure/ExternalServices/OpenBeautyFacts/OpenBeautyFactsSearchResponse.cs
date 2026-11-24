namespace CronogramaCapilar.Infrastructure.ExternalServices.OpenBeautyFacts;

/// <summary>Resposta de GET /cgi/search.pl.</summary>
/// <param name="Count">Total de produtos encontrados (em todas as páginas).</param>
internal sealed record OpenBeautyFactsSearchResponse(
    int Count,
    IReadOnlyList<OpenBeautyFactsProduct>? Products);
