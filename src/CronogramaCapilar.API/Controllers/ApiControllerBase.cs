using CronogramaCapilar.API.Extensions;
using CronogramaCapilar.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace CronogramaCapilar.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>Id do usuário dono do access token.</summary>
    protected Guid CurrentUserId => User.GetUserId();

    /// <summary>Transforma uma falha do caso de uso numa resposta HTTP no padrão ProblemDetails.</summary>
    protected ObjectResult ProblemFrom(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError,
        };

        var problem = ProblemDetailsFactory.CreateProblemDetails(HttpContext, statusCode, title: error.Message);
        problem.Extensions["code"] = error.Code;

        return new ObjectResult(problem) { StatusCode = statusCode };
    }
}
