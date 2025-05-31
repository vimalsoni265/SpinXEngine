using Microsoft.AspNetCore.Mvc;
using SpinXEngine.Core;
using System.Diagnostics.CodeAnalysis;

namespace SpinXEngine.Api
{
    [ExcludeFromCodeCoverage(Justification = "This class is used for API responses and does not contain any logic that requires unit testing.")]
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string? message = null) =>
            new() { Success = true, Data = data, Message = message };

        public static ApiResponse<T> FailureResponse(string message) =>
            new() { Success = false, Data = default, Message = message };
    }


    [ExcludeFromCodeCoverage(Justification ="This class is used for API responses and does not contain any logic that requires unit testing.")]
    /// <summary>
    /// Converter Extension.
    public static class ControllerExtensions
    {
        #region Public Methods

        /// <summary>
        /// Converts a <see cref="ServiceResult{T}"/> into an <see cref="ActionResult{T}"/> containing an <see cref="ApiResponse{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the data contained in the <see cref="ServiceResult{T}"/>.</typeparam>
        /// <param name="ctrl">The <see cref="ControllerBase"/> instance used to generate the <see cref="ActionResult{T}"/>.</param>
        /// <param name="result">The <see cref="ServiceResult{T}"/> to be converted into an API response.</param>
        public static ActionResult<ApiResponse<T>> ToApiResponse<T>(this ControllerBase ctrl, ServiceResult<T> result)
        {
            var apiResponse = result.Success
                ? ApiResponse<T>.SuccessResponse(result.Data!)
                : ApiResponse<T>.FailureResponse(result.Message!);

            return result.Status switch
            {
                ServiceStatus.Success => ctrl.Ok(apiResponse),
                ServiceStatus.NotFound => ctrl.NotFound(apiResponse),
                ServiceStatus.ValidationError => ctrl.BadRequest(apiResponse),
                ServiceStatus.Conflict => ctrl.Conflict(apiResponse),
                ServiceStatus.Unauthorized => ctrl.Unauthorized(apiResponse),
                ServiceStatus.Forbidden => ctrl.Forbid(),
                ServiceStatus.ServerError => ctrl.StatusCode(StatusCodes.Status500InternalServerError, apiResponse),
                _ => ctrl.StatusCode(StatusCodes.Status500InternalServerError, apiResponse)
            };
        } 

        #endregion
    }
}
