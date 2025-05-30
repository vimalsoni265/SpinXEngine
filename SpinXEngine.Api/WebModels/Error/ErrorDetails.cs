namespace SpinXEngine.Api.WebModels.Error
{
    /// <summary>
    /// Generic Error Model used for storing Error Details
    /// </summary>
    public record ErrorDetails
    (
        /// <summary>
        /// The Error Code
        /// </summary>
        ErrorCodes ErrorCode = ErrorCodes.Generic,

        /// <summary>
        /// The Error Message
        /// </summary>
        string? Message = null
    );
}
