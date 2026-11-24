using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CronogramaCapilar.Application.Common;
using CronogramaCapilar.Application.DTOs.Products;
using CronogramaCapilar.Application.Errors;
using CronogramaCapilar.Application.ExternalServices.Interfaces;
using CronogramaCapilar.Infrastructure.Configuration;
using CronogramaCapilar.Infrastructure.ExternalServices.OpenBeautyFacts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CronogramaCapilar.Infrastructure.ExternalServices;

/// <summary>
/// Pesquisa produtos no Open Beauty Facts e traduz a resposta para o nosso formato
/// (camada anticorrupção: o resto do sistema não conhece o formato deles).
/// O Open Beauty Facts permite 10 pesquisas e 15 consultas de produto por minuto por IP,
/// e todas as chamadas saem do nosso servidor; por isso os resultados ficam em cache.
/// </summary>
public sealed class ProductCatalog(
    HttpClient httpClient,
    IMemoryCache cache,
    IOptions<ProductCatalogOptions> options,
    ILogger<ProductCatalog> logger) : IProductCatalog
{
    private const string Fields =
        "code,product_name,product_name_pt,product_name_en,brands,quantity,image_front_url,image_front_small_url";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
    };

    private readonly ProductCatalogOptions _options = options.Value;

    public async Task<Result<ProductSearchResponse>> SearchAsync(
        string query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"products:search:{query.ToLowerInvariant()}:{page}:{pageSize}";
        if (cache.TryGetValue(cacheKey, out ProductSearchResponse? cached) && cached is not null)
            return cached;

        var url = "cgi/search.pl?action=process&json=1&search_simple=1&sort_by=unique_scans_n"
            + $"&search_terms={Uri.EscapeDataString(query)}&page={page}&page_size={pageSize}&fields={Fields}";

        try
        {
            var body = await httpClient.GetFromJsonAsync<OpenBeautyFactsSearchResponse>(url, JsonOptions, cancellationToken);

            var items = (body?.Products ?? [])
                .Select(ToProductResponse)
                .OfType<ProductResponse>()
                .ToList();

            var response = new ProductSearchResponse(items, page, pageSize, HasMore: (body?.Count ?? 0) > page * pageSize);
            cache.Set(cacheKey, response, TimeSpan.FromMinutes(_options.SearchCacheMinutes));

            return response;
        }
        catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(exception, "Product search failed for query {Query}", query);
            return ProductErrors.CatalogUnavailable;
        }
    }

    public async Task<Result<ProductResponse>> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        // Um valor nulo no cache significa "já sabemos que não existe".
        var cacheKey = $"products:barcode:{barcode}";
        if (cache.TryGetValue(cacheKey, out ProductResponse? cached))
            return cached is null ? ProductErrors.NotFound : cached;

        try
        {
            using var httpResponse = await httpClient.GetAsync($"api/v2/product/{barcode}?fields={Fields}", cancellationToken);

            // O Open Beauty Facts responde "não encontrado" de dois jeitos: 404, ou 200 com status 0.
            ProductResponse? product = null;
            if (httpResponse.StatusCode != HttpStatusCode.NotFound)
            {
                httpResponse.EnsureSuccessStatusCode();
                var body = await httpResponse.Content.ReadFromJsonAsync<OpenBeautyFactsProductResponse>(JsonOptions, cancellationToken);
                product = body is { Status: 1, Product: not null } ? ToProductResponse(body.Product) : null;
            }

            if (product is null)
            {
                cache.Set<ProductResponse?>(cacheKey, null, TimeSpan.FromMinutes(_options.NotFoundCacheMinutes));
                return ProductErrors.NotFound;
            }

            cache.Set(cacheKey, product, TimeSpan.FromHours(_options.ProductCacheHours));
            return product;
        }
        catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(exception, "Product lookup failed for barcode {Barcode}", barcode);
            return ProductErrors.CatalogUnavailable;
        }
    }

    /// <summary>Produto sem código ou sem nome não serve para a pessoa escolher, então é descartado.</summary>
    private static ProductResponse? ToProductResponse(OpenBeautyFactsProduct product)
    {
        var name = FirstFilled(product.ProductNamePt, product.ProductName, product.ProductNameEn);
        var barcode = NullIfBlank(product.Code);
        if (barcode is null || name is null)
            return null;

        return new ProductResponse(
            barcode,
            name,
            CleanText(product.Brands),
            CleanText(product.Quantity),
            NullIfBlank(product.ImageFrontUrl),
            NullIfBlank(product.ImageFrontSmallUrl));
    }

    private static string? FirstFilled(params string?[] values) =>
        values.Select(CleanText).FirstOrDefault(value => value is not null);

    /// <summary>Alguns textos vêm com entidades HTML (ex.: "&amp;quot;"); aqui viram o caractere de verdade.</summary>
    private static string? CleanText(string? value) => NullIfBlank(WebUtility.HtmlDecode(value));

    private static string? NullIfBlank(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
