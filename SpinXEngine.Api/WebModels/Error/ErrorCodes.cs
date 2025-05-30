namespace SpinXEngine.Api.WebModels.Error
{
    /// <summary>
    /// Standardized Error codes those are sent to the Front-End on occurance of ERROR while API calls
    /// These error codes are knows to the Frond-End, which transalates them into user-friendly messages using translation file.
    /// </summary>
    /// <remarks>
    /// Warning: When Changing the error codes, make sure they have proper message defined in the Frond-End.
    /// </remarks>
    public enum ErrorCodes
    {
        // Generic Server Codes
        None,
        Generic,
        BadRequest,
        NoContent,
        NotFound,
        UnAuthorized,
        MethodNotAllowed,
        InternalServerError,

        // Player Codes
        PlayerNotFound,
        InvalidAmount,

    }
}
