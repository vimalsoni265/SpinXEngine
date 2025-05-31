using SpinXEngine.Common.Contracts;
using SpinXEngine.Repository.Models;

namespace SpinXEngine.Core.Interface
{
    public interface IPlayerService : IBaseService<Player>
    {
        /// <summary>
        /// Credits the specified amount to the player's balance.
        /// </summary>
        /// <param name="amount">The amount needs to be credited.</param>
        /// <param name="playerId">The ID for the existng player</param>
        Task<ServiceResult<BalanceUpdateResponse>> CreditBalanceAsync(string playerId, decimal amount);

        /// <summary>
        /// Creates a new player with a default balance of 0.00
        /// </summary>
        /// <param name="amount">The amount with the player profile needs to be credited.</param>
        Task<ServiceResult<BalanceUpdateResponse>> CreatePlayerAsync(decimal amount);

        /// <summary>
        /// Processes a bet for a player and returns the spin result
        /// </summary>
        /// <param name="playerId">The ID of the player placing the bet</param>
        /// <param name="betAmount">The amount the player wishes to bet</param>
        /// <returns>A service result containing the spin response with reel symbols, win amount, and updated balance</returns>
        Task<ServiceResult<SpinResponse>> SpinAsync(string playerId, decimal betAmount);
    }
}
