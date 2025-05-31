using MongoDB.Driver;
using SpinXEngine.Repository.Context;
using SpinXEngine.Repository.Interfaces;
using SpinXEngine.Repository.Models;

namespace SpinXEngine.Repository.Implementation
{
    /// <summary>
    /// Implementation of the <see cref="IPlayerRepository"/> for managing player data.
    /// </summary>
    public class PlayerRepository : Repository<Player>, IPlayerRepository
    {
        #region Constructor

        /// <summary>
        /// Constructor for PlayerRepository
        /// </summary>
        /// <param name="context"></param>
        public PlayerRepository(SpinXEngineDbContext context)
            : base(context)
        {
        }

        #endregion

        #region Implement IPlayerRepository Methods

        /// <inheritdoc/>
        public async Task<Player> UpdateBalance(string playerId, decimal amount)
        {
            // Use FindOneAndUpdate to atomically update the balance
            var filter = Builders<Player>.Filter.Eq(p => p.Id, playerId);
            var update = Builders<Player>.Update.Set(p => p.Balance, amount);
            var options = new FindOneAndUpdateOptions<Player>
            {
                ReturnDocument = ReturnDocument.After
            };

            return await m_collection.FindOneAndUpdateAsync(filter, update, options);
        }

        /// <inheritdoc/>
        public async Task<Player> CreatePlayerAsync(decimal defaultBalance = 0.00m)
        {
            return await AddAsync(new Player
            {
                Balance = defaultBalance
            });
        } 

        #endregion
    }
}
