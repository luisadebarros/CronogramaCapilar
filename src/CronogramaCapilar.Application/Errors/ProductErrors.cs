using CronogramaCapilar.Application.Common;

namespace CronogramaCapilar.Application.Errors;

public static class ProductErrors
{
    public static readonly Error NotFound =
        Error.NotFound("product.not_found", "Produto não encontrado.");

    public static readonly Error InvalidQuery =
        Error.Validation("product.invalid_query", "Digite pelo menos 2 letras para pesquisar.");

    public static readonly Error InvalidBarcode =
        Error.Validation("product.invalid_barcode", "O código de barras deve ter de 8 a 14 dígitos.");

    public static readonly Error CatalogUnavailable =
        Error.Unavailable("product.catalog_unavailable", "A pesquisa de produtos está indisponível no momento. Tente de novo em instantes.");
}
