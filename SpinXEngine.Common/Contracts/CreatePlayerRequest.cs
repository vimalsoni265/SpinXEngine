using System.ComponentModel.DataAnnotations;

namespace SpinXEngine.Common.Contracts
{
    public record CreatePlayerRequest
    (
        [Required]
        decimal amount
    )
    { }
}
