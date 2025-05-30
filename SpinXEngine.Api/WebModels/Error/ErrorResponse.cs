namespace SpinXEngine.Api.WebModels.Error
{
    /// <summary>
    /// Generic Error Response returned in case of API errors.
    /// </summary>
    public record ErrorResponse
    (
        /// <summary>
        /// The List of <seealso cref="ParameterErrorDetails"/>
        /// </summary>
        List<ParameterErrorDetails> ParameterErrors,

        /// <summary>
        /// The List of <seealso cref="ErrorDetails"/>
        /// </summary>
        List<ErrorDetails> GenericErrors
    );
}
