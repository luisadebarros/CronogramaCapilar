using CronogramaCapilar.Application.Common;
using CronogramaCapilar.Application.DTOs.Products;
using CronogramaCapilar.Application.Errors;
using CronogramaCapilar.Application.ExternalServices.Interfaces;

namespace CronogramaCapilar.Application.Tests.Fakes;

/// <summary>Catálogo falso: só registra o que recebeu, para os testes conferirem.</summary>
internal sealed class FakeProductCatalog : IProductCatalog
{
    public List<string> Queries { get; } = [];
    public List<string> Barcodes { get; } = [];

    public Task<Result<ProductSearchResponse>> SearchAsync(
        string query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        Queries.Add(query);
        return Task.FromResult<Result<ProductSearchResponse>>(new ProductSearchResponse([], page, pageSize, HasMore: false));
    }

    public Task<Result<ProductResponse>> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        Barcodes.Add(barcode);
        return Task.FromResult<Result<ProductResponse>>(ProductErrors.NotFound);
    }
}
