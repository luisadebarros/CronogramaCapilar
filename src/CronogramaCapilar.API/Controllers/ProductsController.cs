using CronogramaCapilar.API.Configuration;
using CronogramaCapilar.Application.DTOs.Products;
using CronogramaCapilar.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CronogramaCapilar.API.Controllers;

/// <summary>Pesquisa de produtos em catálogo externo (Open Beauty Facts). Nada é salvo no nosso banco.</summary>
[Route("api/products")]
[Authorize]
[EnableRateLimiting(RateLimitPolicies.ProductSearch)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
public sealed class ProductsController(IProductService productService) : ApiControllerBase
{
    /// <summary>Pesquisa produtos por nome, marca ou tipo (ex.: "máscara hidratação", "salon line").</summary>
    [HttpGet]
    [ProducesResponseType<ProductSearchResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductSearchResponse>> Search(
        [FromQuery] SearchProductsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await productService.SearchAsync(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : ProblemFrom(result.Error);
    }

    /// <summary>Busca um produto pelo código de barras (quando a pessoa escaneia a embalagem).</summary>
    [HttpGet("{barcode}")]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> GetByBarcode(string barcode, CancellationToken cancellationToken)
    {
        var result = await productService.GetByBarcodeAsync(barcode, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : ProblemFrom(result.Error);
    }
}
