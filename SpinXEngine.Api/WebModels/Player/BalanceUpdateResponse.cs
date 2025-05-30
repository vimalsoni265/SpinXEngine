using SpinXEngine.Api.WebModels.Error;

namespace SpinXEngine.Api.WebModels.Player
{
    /// <summary>
    /// Response model for balance updates
    /// </summary>
    public record BalanceUpdateResponse
    (
        /// <summary>
        /// The player's balance after the update
        /// </summary>
        double NewBalance,


        /// <summary>
        /// The Error Code
        /// </summary>
        ErrorCodes ErrorCode = ErrorCodes.None
    )
    {
        /// <summary>
        /// Converts <seealso cref="LoginResult"/> to UI DTO
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static BalanceUpdateResponse From(BalanceUpdateResult result)
        {
            return new BalanceUpdateResponse(
                NewBalance: result.NewBalance,
                ErrorCode: ToErrorCodes(result.ErrorCode));
        }

        /// <summary>
        /// Converts Authentication Error to Error Codes
        /// </summary>
        /// <param name="authenticationError"></param>
        /// <returns></returns>
        private static ErrorCodes ToErrorCodes(AuthenticationErrors authenticationError)
        {
            if (Enum.TryParse<ErrorCodes>(authenticationError.ToString(), true, out var errorCode))
            {
                return errorCode;
            }
            return ErrorCodes.Generic;
        }
    }
}
