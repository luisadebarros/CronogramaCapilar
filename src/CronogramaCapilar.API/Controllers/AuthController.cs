using CronogramaCapilar.API.Configuration;
using CronogramaCapilar.Application.DTOs.Auth;
using CronogramaCapilar.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CronogramaCapilar.API.Controllers;

[Route("api/auth")]
[AllowAnonymous]
[EnableRateLimiting(RateLimitPolicies.Auth)]
[ProducesResponseType(StatusCodes.Status429TooManyRequests)]
public sealed class AuthController(IAuthService authService) : ApiControllerBase
{
    /// <summary>Cria a conta e já devolve os tokens (a pessoa entra logada).</summary>
    [HttpPost("register")]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TokenResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(request, cancellationToken);
        return result.IsSuccess ? Created("/api/users/me", result.Value) : ProblemFrom(result.Error);
    }

    /// <summary>Troca e-mail + senha por tokens.</summary>
    [HttpPost("login")]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : ProblemFrom(result.Error);
    }

    /// <summary>Troca o refresh token por tokens novos. O refresh token antigo deixa de valer.</summary>
    [HttpPost("refresh")]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenResponse>> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.RefreshAsync(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : ProblemFrom(result.Error);
    }

    /// <summary>Encerra a sessão deste aparelho.</summary>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LogoutAsync(request, cancellationToken);
        return result.IsSuccess ? NoContent() : ProblemFrom(result.Error);
    }
}
