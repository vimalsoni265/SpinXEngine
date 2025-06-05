using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpinXEngine.Common.Contracts;
using SpinXEngine.Core.GameDesigner;
using SpinXEngine.Core.Interface;
using SpinXEngine.Repository.Interfaces;
using SpinXEngine.Repository.Models;

namespace SpinXEngine.Core.Implementation
{
    /// <summary>
    /// Provides functionality for managing player and perform related operations.
    /// </summary>
    public class PlayerService : IPlayerService
    {
        #region Private Members
        /// <summary>
        /// Represents the repository used to manage player data.
        /// </summary>
        private readonly IPlayerRepository m_playerRepository;

        /// <summary>
        /// A logger instance used to log details of <see cref="PlayerService">.
        /// </summary>
        private readonly ILogger<PlayerService> m_logger;

        /// <summary>
        /// Factory function to create new SpinGame instances
        /// </summary>
        private readonly Func<ISpinGame> m_spinGameFactory;

        /// <summary>
        /// Provides access to the current game settings configuration
        /// </summary>
        private readonly IOptionsMonitor<GameSetting> m_gameSettings;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for PlayerService
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="playerRepository"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public PlayerService(IPlayerRepository playerRepository, ILogger<PlayerService> logger, Func<ISpinGame> spinGameFactory,
            IOptionsMonitor<GameSetting> gameSettings)
        {
            m_playerRepository = playerRepository ??
                throw new ArgumentNullException(nameof(playerRepository));
            m_logger = logger ??
                throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
            m_spinGameFactory = spinGameFactory ??
                throw new ArgumentNullException(nameof(spinGameFactory));
            m_gameSettings = gameSettings ??
                throw new ArgumentNullException(nameof(gameSettings));
        }

        /// <summary>
        /// Constructor for PlayerService with default SpinGame factory
        /// </summary>
        /// <param name="playerRepository">Repository for player data</param>
        /// <param name="logger">Logger instance</param>
        public PlayerService(IPlayerRepository playerRepository, ILogger<PlayerService> logger, IOptionsMonitor<GameSetting> gameSettings)
            : this(playerRepository, logger, () => new SpinGame(), gameSettings)
        {
        }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public async Task<ServiceResult<BalanceUpdateResponse>> CreditBalanceAsync(string playerId, decimal amount)
        {
            // 1. Validate input
            if (string.IsNullOrWhiteSpace(playerId))
                return ServiceResult<BalanceUpdateResponse>.ValidationError("Player ID is required.");

            if (amount <= 0)
                return ServiceResult<BalanceUpdateResponse>.ValidationError("Amount must be greater than zero.");

            // 2. Check if player exists
            var playerBalance = await m_playerRepository.GetBalanceAsync(playerId);
            if(playerBalance is null)
                return ServiceResult<BalanceUpdateResponse>.ValidationError("Player not found.");

            try
            {
                // 3. Credit the balance (assumes this method returns updated player)
                // Ensure amount is rounded to 2 decimal places
                amount = Math.Round((decimal) playerBalance + amount, 2);
                var updatedPlayer = await m_playerRepository.UpdateBalance(playerId, amount);

                // 4. Create and return response
                return ServiceResult<BalanceUpdateResponse>.Ok(new BalanceUpdateResponse { NewBalance = updatedPlayer.Balance });
            }
            catch (Exception ex)
            {
                // Log the exception
                m_logger.LogError(ex, $"An error occurred while updating the balance for player {playerId} with amount {amount}.");
                // 5. Handle unexpected errors (log if needed)
                return ServiceResult<BalanceUpdateResponse>.ServerError($"An error occurred while updating the balance.");
            }
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<BalanceUpdateResponse>> CreatePlayerAsync(decimal amount)
        {
            try
            {
                // Ensure amount is not negative
                if (amount < 0)
                    return ServiceResult<BalanceUpdateResponse>.ValidationError("Initial balance cannot be negative.");

                // Create the new player with specified balance
                var newPlayer = await m_playerRepository.CreatePlayerAsync(amount);
                return ServiceResult<BalanceUpdateResponse>.Ok(new BalanceUpdateResponse { NewBalance = newPlayer.Balance });
            }
            catch (Exception ex)
            {
                // Log the exception
                m_logger.LogError(ex, "An error occurred while creating a new player.");
                return ServiceResult<BalanceUpdateResponse>.ServerError("An error occurred while creating the player.");
            }
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<IEnumerable<Player>>> GetAllAsync()
        {
            try
            {
                // Get all players from the repository
                var players = await m_playerRepository.GetAllAsync();

                // Return successful result with players
                return ServiceResult<IEnumerable<Player>>.Ok(players);
            }
            catch (Exception ex)
            {
                // Log the exception
                m_logger.LogError(ex, "An error occurred while retrieving all players.");

                // Return server error
                return ServiceResult<IEnumerable<Player>>.ServerError("An error occurred while retrieving players.");
            }
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<SpinResponse>> SpinAsync(string playerId, decimal betAmount)
        {
            // 1. Validate input
            if (string.IsNullOrWhiteSpace(playerId))
                return ServiceResult<SpinResponse>.ValidationError("Player ID is required.");

            if (betAmount <= 0)
                return ServiceResult<SpinResponse>.ValidationError("Bet amount must be greater than zero.");

            // 2. Check if player exists
            var player = await m_playerRepository.GetByIdAsync(playerId);
            if (player == null)
                return ServiceResult<SpinResponse>.NotFound("Player not found.");

            // 3. Check if player has enough balance
            if (player.Balance < betAmount)
                return ServiceResult<SpinResponse>.ValidationError("Insufficient balance to place this bet.");

            try
            {
                // Create a new SpinGame instance for this request to avoid concurrency issues
                var spinGame = m_spinGameFactory();

                // 4. Deduct the bet amount from player balance
                decimal newBalance = Math.Round(player.Balance - betAmount, 2);
                await m_playerRepository.UpdateBalance(playerId, newBalance);

                // 5. Generate the reel symbols matrix (3 rows, 5 columns)
                var currentSettings = m_gameSettings.CurrentValue;
                var reelSymbols = spinGame.GenerateReelSymbols(currentSettings.ReelRows, currentSettings.ReelColumns);

                // 6. Calculate win amount based on the reel symbols
                decimal winAmount = spinGame.Spin(betAmount);

                // 7. Update player balance with winnings, if any
                if (winAmount > 0)
                {
                    newBalance = Math.Round(newBalance + winAmount, 2);
                    await m_playerRepository.UpdateBalance(playerId, newBalance);
                }

                // 9. Create and return response
                var response = new SpinResponse
                {
                    ReelSymbols = spinGame.ConvertMatrixToString(),
                    Win = winAmount,
                    CurrentBalance = newBalance
                };

                return ServiceResult<SpinResponse>.Ok(response);
            }
            catch (Exception ex)
            {
                // Log the exception
                m_logger.LogError(ex, $"An error occurred while processing spin for player {playerId} with bet amount {betAmount}.");
                // Handle unexpected errors
                return ServiceResult<SpinResponse>.ServerError("An error occurred while processing the spin.");
            }
        } 
       
        #endregion
    }
}
