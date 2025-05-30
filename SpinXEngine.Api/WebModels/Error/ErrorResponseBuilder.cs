namespace SpinXEngine.Api.WebModels.Error
{
    /// <summary>
    /// Builder for Error Response Messages.
    /// </summary>
    public class ErrorResponseBuilder
    {
        /// <summary>
        /// The List of <seealso cref="ParameterErrorDetails"/>
        /// </summary>
        private readonly List<ParameterErrorDetails> _parameterErrors;

        /// <summary>
        /// The List of <seealso cref="ErrorDetails"/>
        /// </summary>
        private readonly List<ErrorDetails> _genericErrors;

        /// <summary>
        /// The Private Constructor of <seealso cref="ErrorResponseBuilder"/>
        /// </summary>
        private ErrorResponseBuilder()
        {
            _parameterErrors = [];
            _genericErrors = [];
        }

        /// <summary>
        /// Creates an Instance of <seealso cref="ErrorResponseBuilder"/>
        /// </summary>
        /// <returns></returns>
        public static ErrorResponseBuilder Create()
        {
            return new ErrorResponseBuilder();
        }

        /// <summary>
        /// Creates an Instance of <seealso cref="ErrorResponseBuilder"/> and initializes it with a generic error.
        /// </summary>
        /// <param name="errorCode">The <seealso cref="ErrorCodes"/> object</param>
        /// <returns>The Instance of <seealso cref="ErrorResponseBuilder"/></returns>
        public static ErrorResponseBuilder Create(ErrorCodes errorCode, string? message = null)
        {
            var builder = new ErrorResponseBuilder();
            builder.AddGenericError(errorCode, message);
            return builder;
        }

        /// <summary>
        /// Adds a Parameter error to the Error Response.
        /// </summary>
        /// <param name="parameter">The Name of the Parameter</param>
        /// <param name="errorCode">The <seealso cref="ErrorCodes"/> object</param>
        /// <param name="message">The Error Message</param>
        /// <returns>The Instance of <seealso cref="ErrorResponseBuilder"/></returns>
        public ErrorResponseBuilder AddParameterError(string parameter, ErrorCodes errorCode, string? message = null)
        {
            var parameterError = _parameterErrors.Find(x => x.Parameter == parameter);
            parameterError?.Errors.Add(new ErrorDetails(errorCode, message));

            if (parameterError == null)
            {
                _parameterErrors.Add(new ParameterErrorDetails(parameter, [new(errorCode, message)]));
            }

            return this;
        }

        /// <summary>
        /// Adds a generic error to the <seealso cref="ErrorResponse"/>
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public ErrorResponseBuilder AddGenericError(ErrorCodes errorCode, string? message = null)
        {
            _genericErrors.Add(new ErrorDetails()
            {
                ErrorCode = errorCode,
                Message = message
            });
            return this;
        }

        /// <summary>
        /// Builds and returns the <seealso cref="ErrorResponse"/> Instance
        /// </summary>
        /// <returns>The <seealso cref="ErrorResponse"/> Instance</returns>
        public ErrorResponse Build()
        {
            return new ErrorResponse(
                _parameterErrors.Any() ? [.. _parameterErrors] : [],
                _genericErrors.Any() ? [.. _genericErrors] : []
            );
        }
    }
}
