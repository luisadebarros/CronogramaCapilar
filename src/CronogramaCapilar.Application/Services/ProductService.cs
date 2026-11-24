using System.Text.RegularExpressions;
using CronogramaCapilar.Application.Common;
using CronogramaCapilar.Application.DTOs.Products;
using CronogramaCapilar.Application.Errors;
using CronogramaCapilar.Application.ExternalServices.Interfaces;
using CronogramaCapilar.Application.Services.Interfaces;

namespace CronogramaCapilar.Application.Services;

/// <summary>Pesquisa de produtos: só consulta o catálogo externo, nada é salvo no nosso banco.</summary>
public sealed partial class ProductService(IProductCatalog productCatalog) : IProductService
{
    public async Task<Result<ProductSearchResponse>> SearchAsync(
        SearchProductsRequest request,
        CancellationToken cancellationToken = default)
    {
        // Normaliza os espaços: "máscara   hidratação " e "máscara hidratação" viram a mesma pesquisa (e o mesmo cache).
        var query = string.Join(' ', (request.Query ?? string.Empty)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

        if (query.Length < SearchProductsRequest.QueryMinLength)
            return ProductErrors.InvalidQuery;

        return await productCatalog.SearchAsync(query, request.Page, request.PageSize, cancellationToken);
    }

    public async Task<Result<ProductResponse>> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        var normalized = (barcode ?? string.Empty).Trim();
        if (!BarcodeFormat().IsMatch(normalized))
            return ProductErrors.InvalidBarcode;

        return await productCatalog.GetByBarcodeAsync(normalized, cancellationToken);
    }

    /// <summary>EAN-8, UPC-A (12), EAN-13 e GTIN-14.</summary>
    [GeneratedRegex(@"^\d{8,14}$")]
    private static partial Regex BarcodeFormat();
}
