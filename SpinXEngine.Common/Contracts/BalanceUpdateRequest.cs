using System.ComponentModel.DataAnnotations;

namespace SpinXEngine.Common.Contracts
{
    /// <summary>
    /// Request model for balance updates
    /// </summary>
    public record BalanceUpdateRequest

    (
        [Required]
        string playerId,
        [Required]
        decimal amount
    )
    { }
}
