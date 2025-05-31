using System.ComponentModel.DataAnnotations;

namespace SpinXEngine.Common.Contracts
{
    /// <summary>
    /// Represents a request to place a bet and spin the reels
    /// </summary>
    public record SpinRequest
    (
        [Required]
        string PlayerId,

        [Required]
        decimal BetAmount
    );
}
