using CronogramaCapilar.Application.Common;
using CronogramaCapilar.Application.DTOs.Products;

namespace CronogramaCapilar.Application.ExternalServices.Interfaces;

/// <summary>
/// Catálogo externo de produtos. A aplicação não sabe qual API está por trás:
/// trocar de fornecedor é trocar só a implementação na infraestrutura.
/// </summary>
public interface IProductCatalog
{
    Task<Result<ProductSearchResponse>> SearchAsync(
        string query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Result<ProductResponse>> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
}
