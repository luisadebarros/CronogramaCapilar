using CronogramaCapilar.Application.Common;
using CronogramaCapilar.Application.DTOs.Auth;

namespace CronogramaCapilar.Application.Services.Interfaces;

public interface IAuthService
{
    Task<Result<TokenResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<Result<TokenResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<Result<TokenResponse>> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);

    Task<Result> LogoutAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
}
