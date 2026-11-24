namespace CronogramaCapilar.Domain.Exceptions;

/// <summary>Uma regra do domínio foi violada (ex.: lavagens por semana fora do permitido).</summary>
public sealed class DomainException(string message) : Exception(message);
