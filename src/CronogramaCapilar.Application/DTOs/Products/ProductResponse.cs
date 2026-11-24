namespace CronogramaCapilar.Application.DTOs.Products;

/// <param name="Barcode">Código de barras (EAN).</param>
/// <param name="Brand">Marca(s), quando informada.</param>
/// <param name="Quantity">Ex.: "300 g", "250 ml".</param>
/// <param name="ImageUrl">Foto da frente da embalagem (400 px).</param>
/// <param name="ThumbnailUrl">Foto pequena para listas (200 px).</param>
public sealed record ProductResponse(
    string Barcode,
    string Name,
    string? Brand,
    string? Quantity,
    string? ImageUrl,
    string? ThumbnailUrl);
