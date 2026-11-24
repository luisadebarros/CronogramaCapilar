using System.ComponentModel.DataAnnotations;

namespace CronogramaCapilar.Application.DTOs.Products;

/// <summary>Parâmetros da URL: <c>?query=mascara+hidratacao&amp;page=1&amp;pageSize=20</c>.</summary>
public sealed class SearchProductsRequest
{
    public const int QueryMinLength = 2;
    public const int QueryMaxLength = 100;
    public const int MaxPage = 100;
    public const int MaxPageSize = 50;

    /// <summary>Nome, marca ou tipo de produto (ex.: "máscara hidratação", "salon line").</summary>
    [Required(ErrorMessage = "Informe o que deseja pesquisar.")]
    [StringLength(QueryMaxLength, MinimumLength = QueryMinLength,
        ErrorMessage = "A pesquisa deve ter entre {2} e {1} caracteres.")]
    public string Query { get; init; } = string.Empty;

    [Range(1, MaxPage, ErrorMessage = "A página deve estar entre {1} e {2}.")]
    public int Page { get; init; } = 1;

    [Range(1, MaxPageSize, ErrorMessage = "O tamanho da página deve estar entre {1} e {2}.")]
    public int PageSize { get; init; } = 20;
}
