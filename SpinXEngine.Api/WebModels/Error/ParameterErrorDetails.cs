namespace SpinXEngine.Api.WebModels.Error
{
    /// <summary>
    /// Generic Error model used for error related to specific parameter
    /// </summary>
    public record ParameterErrorDetails
    (
        /// <summary>
        /// The Parameter Name
        /// </summary>
        string Parameter,

        /// <summary>
        /// The List of <seealso cref="ErrorDetails"/>
        /// </summary>
        List<ErrorDetails> Errors
    );
}
