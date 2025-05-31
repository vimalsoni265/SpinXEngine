using SpinXEngine.Repository.Models;

namespace SpinXEngine.Repository.Interfaces
{

    /// <summary>
    /// Defines a contract for managing player-related data operations.
    /// </summary>
    public interface IPlayerRepository : IRepository<Player>
    {
        /// <summary>
        /// Credits the specified amount to the player's balance.
        /// </summary>
        Task<Player> UpdateBalance(string playerId, decimal amount);

        /// <summary>
        /// Creates a new player with the specified ID and default balance.
        /// </summary>
        /// <param name="playerId">The ID for the new player</param>
        /// <param name="defaultBalance">The initial balance for the player (default: 0.00)</param>
        /// <returns>The newly created player</returns>
        Task<Player> CreatePlayerAsync(decimal defaultBalance = 0.00m);
    }
}
