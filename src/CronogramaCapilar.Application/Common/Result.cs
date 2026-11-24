using System.Diagnostics.CodeAnalysis;

namespace CronogramaCapilar.Application.Common;

/// <summary>
/// Resultado de um caso de uso: sucesso ou falha esperada (e-mail em uso, não encontrado...).
/// Falhas esperadas não usam exceção; exceção fica para o que é realmente inesperado.
/// </summary>
public class Result
{
    protected Result(Error? error) => Error = error;

    public Error? Error { get; }

    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => Error is null;

    public static Result Success() => new(null);

    public static Result Failure(Error error) => new(error);

    public static implicit operator Result(Error error) => Failure(error);
}
