using Microsoft.AspNetCore.Mvc;

namespace SpinXEngine.Api.WebModels.Error
{
    /// <summary>
    /// Helper class used for automatic input validation
    /// </summary>
    public static class ErrorResponseHelper
    {
        /// <summary>
        /// Converts the model state errors to generic ErrorResponse
        /// </summary>
        /// <param name="context">The Instance of <seealso cref="ActionContext"/></param>
        /// <returns>The Instance of <seealso cref="IActionResult"/></returns>
        public static IActionResult CreateValidationErrorResponse(ActionContext context)
        {
            var builder = ErrorResponseBuilder.Create();
            foreach (var state in context.ModelState.Where(s => s.Value!.Errors.Count > 0))
            {
                foreach (var error in state.Value!.Errors)
                {
                    if (Enum.TryParse(typeof(ErrorCodes), error.ErrorMessage, out var errorCode))
                    {
                        builder.AddParameterError(state.Key, (ErrorCodes)errorCode!);
                    }
                    else
                    {
                        builder.AddParameterError(state.Key, ErrorCodes.Generic, error.ErrorMessage);
                    }
                }
            }
            var errorResponse = builder.Build();
            return new BadRequestObjectResult(errorResponse);
        }
    }
}
