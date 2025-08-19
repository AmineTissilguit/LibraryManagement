using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Extensions;

public static class ErrorExtensions
{
    public static IActionResult ToProblemDetails(this List<Error> errors, ControllerBase controller)
    {
        if (errors.Count is 0)
        {
            return controller.Problem();
        }

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return controller.ValidationProblem(errors);
        }

        return controller.Problem(errors[0]);
    }

    private static IActionResult Problem(this ControllerBase controller, Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError,
        };

        return controller.Problem(statusCode: statusCode, title: error.Description);
    }

    private static IActionResult ValidationProblem(this ControllerBase controller, List<Error> errors)
    {
        var modelStateDictionary = new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary();

        foreach (var error in errors)
        {
            modelStateDictionary.AddModelError(error.Code, error.Description);
        }

        return controller.ValidationProblem(modelStateDictionary);
    }
}
