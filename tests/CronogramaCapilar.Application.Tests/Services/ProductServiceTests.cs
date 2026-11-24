using CronogramaCapilar.Application.DTOs.Products;
using CronogramaCapilar.Application.Errors;
using CronogramaCapilar.Application.Services;
using CronogramaCapilar.Application.Tests.Fakes;

namespace CronogramaCapilar.Application.Tests.Services;

public sealed class ProductServiceTests
{
    private readonly FakeProductCatalog _catalog = new();
    private readonly ProductService _service;

    public ProductServiceTests() => _service = new ProductService(_catalog);

    [Fact]
    public async Task SearchAsync_NormalizesSpacesBeforeSearching()
    {
        var result = await _service.SearchAsync(new SearchProductsRequest { Query = "  máscara   hidratação " });

        Assert.True(result.IsSuccess);
        Assert.Equal("máscara hidratação", Assert.Single(_catalog.Queries));
    }

    [Theory]
    [InlineData("a   ")]
    [InlineData("   ")]
    public async Task SearchAsync_WithLessThanTwoCharacters_ReturnsInvalidQuery(string query)
    {
        var result = await _service.SearchAsync(new SearchProductsRequest { Query = query });

        Assert.Equal(ProductErrors.InvalidQuery, result.Error);
        Assert.Empty(_catalog.Queries);
    }

    [Theory]
    [InlineData("1234567")]          // curto demais
    [InlineData("123456789012345")]  // longo demais
    [InlineData("78986239517AB")]    // com letras
    public async Task GetByBarcodeAsync_WithInvalidBarcode_ReturnsInvalidBarcode(string barcode)
    {
        var result = await _service.GetByBarcodeAsync(barcode);

        Assert.Equal(ProductErrors.InvalidBarcode, result.Error);
        Assert.Empty(_catalog.Barcodes);
    }

    [Theory]
    [InlineData("12345678")]         // EAN-8
    [InlineData(" 7898623951785 ")]  // EAN-13 com espaços
    public async Task GetByBarcodeAsync_WithValidBarcode_QueriesTheCatalog(string barcode)
    {
        await _service.GetByBarcodeAsync(barcode);

        Assert.Equal(barcode.Trim(), Assert.Single(_catalog.Barcodes));
    }
}
