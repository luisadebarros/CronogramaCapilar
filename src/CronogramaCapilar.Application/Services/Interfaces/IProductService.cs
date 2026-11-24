using CronogramaCapilar.Application.Common;
using CronogramaCapilar.Application.DTOs.Products;

namespace CronogramaCapilar.Application.Services.Interfaces;

public interface IProductService
{
    Task<Result<ProductSearchResponse>> SearchAsync(
        SearchProductsRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<ProductResponse>> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
}
