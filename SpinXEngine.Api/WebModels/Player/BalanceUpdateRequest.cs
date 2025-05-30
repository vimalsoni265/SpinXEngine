using System.ComponentModel.DataAnnotations;

namespace SpinXEngine.Api.WebModels.Player
{
    /// <summary>
    /// Request model for balance updates
    /// </summary>
    public record BalanceUpdateRequest
    
    (
        [Required]
        double Amount,
        [Required]
        string PlayerId
    )
    {}

}
