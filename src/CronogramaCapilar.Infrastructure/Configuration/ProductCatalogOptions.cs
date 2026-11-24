using System.ComponentModel.DataAnnotations;

namespace CronogramaCapilar.Infrastructure.Configuration;

/// <summary>Configurações da pesquisa de produtos, lidas da seção "ProductCatalog".</summary>
public sealed class ProductCatalogOptions
{
    public const string SectionName = "ProductCatalog";

    [Required, Url]
    public string BaseUrl { get; init; } = "https://world.openbeautyfacts.org/";

    /// <summary>
    /// O Open Beauty Facts exige um User-Agent que identifique o app,
    /// no formato "NomeDoApp/Versão (e-mail de contato)".
    /// </summary>
    [Required]
    public string UserAgent { get; init; } = string.Empty;

    [Range(1, 1440)]
    public int SearchCacheMinutes { get; init; } = 30;

    [Range(1, 168)]
    public int ProductCacheHours { get; init; } = 24;

    /// <summary>Código de barras que não existe também fica em cache, para não gastar a cota repetindo a consulta.</summary>
    [Range(1, 1440)]
    public int NotFoundCacheMinutes { get; init; } = 60;
}
