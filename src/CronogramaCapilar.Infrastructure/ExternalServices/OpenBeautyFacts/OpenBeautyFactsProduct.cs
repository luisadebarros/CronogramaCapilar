namespace CronogramaCapilar.Infrastructure.ExternalServices.OpenBeautyFacts;

/// <summary>Produto no formato do Open Beauty Facts (só os campos que pedimos em "fields").</summary>
internal sealed record OpenBeautyFactsProduct(
    string? Code,
    string? ProductName,
    string? ProductNamePt,
    string? ProductNameEn,
    string? Brands,
    string? Quantity,
    string? ImageFrontUrl,
    string? ImageFrontSmallUrl);
