using CronogramaCapilar.Application.DTOs.Users;
using CronogramaCapilar.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CronogramaCapilar.API.Controllers;

/// <summary>Tudo aqui é sobre a pessoa logada ("me"). Ninguém acessa dados de outra pessoa.</summary>
[Route("api/users/me")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public sealed class UsersController(IUserService userService) : ApiControllerBase
{
    /// <summary>Dados da pessoa logada, incluindo o perfil capilar.</summary>
    [HttpGet]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> Get(CancellationToken cancellationToken)
    {
        var result = await userService.GetAsync(CurrentUserId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : ProblemFrom(result.Error);
    }

    /// <summary>Atualiza nome, como quer ser chamado(a), data de nascimento e gênero.</summary>
    [HttpPut]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> UpdatePersonalInfo(
        UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await userService.UpdatePersonalInfoAsync(CurrentUserId, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : ProblemFrom(result.Error);
    }

    /// <summary>Cria ou substitui o perfil capilar (questionário sobre o cabelo).</summary>
    [HttpPut("hair-profile")]
    [ProducesResponseType<HairProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HairProfileResponse>> SetHairProfile(
        HairProfileRequest request,
        CancellationToken cancellationToken)
    {
        var result = await userService.SetHairProfileAsync(CurrentUserId, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : ProblemFrom(result.Error);
    }

    /// <summary>Exclui a conta e todos os dados dela. Não tem volta.</summary>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAccount(CancellationToken cancellationToken)
    {
        var result = await userService.DeleteAccountAsync(CurrentUserId, cancellationToken);
        return result.IsSuccess ? NoContent() : ProblemFrom(result.Error);
    }
}
