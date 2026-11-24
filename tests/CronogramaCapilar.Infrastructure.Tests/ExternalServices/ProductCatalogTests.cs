using System.Net;
using CronogramaCapilar.Application.Errors;
using CronogramaCapilar.Infrastructure.Configuration;
using CronogramaCapilar.Infrastructure.ExternalServices;
using CronogramaCapilar.Infrastructure.Tests.Fakes;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace CronogramaCapilar.Infrastructure.Tests.ExternalServices;

public sealed class ProductCatalogTests
{
    // Formato real do Open Beauty Facts, incluindo os casos ruins que aparecem de verdade.
    private const string SearchJson = """
        {
          "count": 45,
          "page": "1",
          "products": [
            {
              "code": "7898623951785",
              "product_name": "Hair mask",
              "product_name_pt": "Máscara de Hidratação",
              "brands": "Salon Line",
              "quantity": "300 g",
              "image_front_url": "https://images.test/400.jpg",
              "image_front_small_url": "https://images.test/200.jpg"
            },
            { "code": "1234567890123", "product_name": "", "product_name_en": "" },
            { "code": "", "product_name": "Produto sem código" },
            { "code": "7898524345096", "product_name": "&quot;Ativador de Cachos&quot; S.O.S", "brands": "" }
          ]
        }
        """;

    private const string ProductJson = """
        {
          "status": 1,
          "product": { "code": "7898623951785", "product_name": "Máscara de Hidratação", "brands": "Salon Line" }
        }
        """;

    private static (ProductCatalog Catalog, StubHttpMessageHandler Handler) CreateCatalog(
        Func<HttpRequestMessage, HttpResponseMessage> respond)
    {
        var handler = new StubHttpMessageHandler(respond);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://catalog.test/") };
        var options = Options.Create(new ProductCatalogOptions { UserAgent = "Tests/1.0" });
        var catalog = new ProductCatalog(
            httpClient,
            new MemoryCache(new MemoryCacheOptions()),
            options,
            NullLogger<ProductCatalog>.Instance);

        return (catalog, handler);
    }

    [Fact]
    public async Task SearchAsync_MapsProductsAndSkipsTheOnesWithoutNameOrBarcode()
    {
        var (catalog, _) = CreateCatalog(_ => StubHttpMessageHandler.Json(SearchJson));

        var result = await catalog.SearchAsync("mascara", page: 1, pageSize: 20);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Items.Count);

        var first = result.Value.Items[0];
        Assert.Equal("7898623951785", first.Barcode);
        Assert.Equal("Máscara de Hidratação", first.Name); // prefere o nome em português
        Assert.Equal("Salon Line", first.Brand);
        Assert.Equal("300 g", first.Quantity);
        Assert.Equal("https://images.test/400.jpg", first.ImageUrl);
        Assert.Equal("https://images.test/200.jpg", first.ThumbnailUrl);

        var second = result.Value.Items[1];
        Assert.Equal("\"Ativador de Cachos\" S.O.S", second.Name); // entidade HTML decodificada
        Assert.Null(second.Brand);                                // marca vazia vira null

        Assert.True(result.Value.HasMore); // 45 resultados > 1 página de 20
    }

    [Fact]
    public async Task SearchAsync_SendsTheQueryEncodedWithPagination()
    {
        var (catalog, handler) = CreateCatalog(_ => StubHttpMessageHandler.Json(SearchJson));

        await catalog.SearchAsync("máscara hidratação", page: 2, pageSize: 10);

        var url = handler.Requests.Single().RequestUri!.AbsoluteUri;
        Assert.Contains("search_terms=m%C3%A1scara%20hidrata%C3%A7%C3%A3o", url);
        Assert.Contains("&page=2&", url);
        Assert.Contains("&page_size=10&", url);
    }

    [Fact]
    public async Task SearchAsync_SameQueryTwice_CallsTheApiOnlyOnce()
    {
        var (catalog, handler) = CreateCatalog(_ => StubHttpMessageHandler.Json(SearchJson));

        await catalog.SearchAsync("Shampoo", 1, 20);
        var second = await catalog.SearchAsync("shampoo", 1, 20);

        Assert.True(second.IsSuccess);
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task SearchAsync_WhenTheApiFails_ReturnsUnavailableAndDoesNotCache()
    {
        var (catalog, handler) = CreateCatalog(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var first = await catalog.SearchAsync("shampoo", 1, 20);
        await catalog.SearchAsync("shampoo", 1, 20);

        Assert.Equal(ProductErrors.CatalogUnavailable, first.Error);
        Assert.Equal(2, handler.Requests.Count);
    }

    [Fact]
    public async Task GetByBarcodeAsync_WhenFound_MapsTheProduct()
    {
        var (catalog, handler) = CreateCatalog(_ => StubHttpMessageHandler.Json(ProductJson));

        var result = await catalog.GetByBarcodeAsync("7898623951785");

        Assert.True(result.IsSuccess);
        Assert.Equal("Máscara de Hidratação", result.Value.Name);
        Assert.Contains("api/v2/product/7898623951785", handler.Requests.Single().RequestUri!.AbsoluteUri);
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound, """{"status":0,"status_verbose":"product not found"}""")]
    [InlineData(HttpStatusCode.OK, """{"status":0,"status_verbose":"no code or invalid code"}""")]
    public async Task GetByBarcodeAsync_WhenNotFound_ReturnsNotFoundAndCachesIt(HttpStatusCode statusCode, string json)
    {
        var (catalog, handler) = CreateCatalog(_ => StubHttpMessageHandler.Json(json, statusCode));

        var first = await catalog.GetByBarcodeAsync("7891234567895");
        var second = await catalog.GetByBarcodeAsync("7891234567895");

        Assert.Equal(ProductErrors.NotFound, first.Error);
        Assert.Equal(ProductErrors.NotFound, second.Error);
        Assert.Single(handler.Requests);
    }
}
