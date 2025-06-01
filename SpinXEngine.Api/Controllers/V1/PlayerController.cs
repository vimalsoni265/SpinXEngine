using Microsoft.AspNetCore.Mvc;
using SpinXEngine.Common.Contracts;
using SpinXEngine.Core.Interface;
using SpinXEngine.Repository.Models;

namespace SpinXEngine.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        #region Private Members
        private readonly IPlayerService m_playerService;
        private readonly ILogger<PlayerController> m_logger;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for PlayerController
        /// </summary>
        /// <param name="playerService"></param>
        /// <param name="logger"></param>
        public PlayerController(IPlayerService playerService, ILogger<PlayerController> logger)
        {
            m_playerService = playerService;
            m_logger = logger;
        }

        #endregion

        #region Public Methods (API Endpoints)

        /// <summary>
        /// Gets a list of all players in the system
        /// </summary>
        /// <returns>A list of all players</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Player>>>> GetAllPlayers()
        {
            m_logger.LogInformation("GetAllPlayers called");

            var result = await m_playerService.GetAllAsync();
            return this.ToApiResponse(result);
        }

        /// <summary>
        /// Updates the player's balance by adding the specified amount
        /// </summary>
        /// <param name="id">The player ID</param>
        /// <param name="request">The balance update request containing the amount to add</param>
        /// <returns>The updated player balance information</returns>
        [HttpPost("credit")]
        public async Task<ActionResult<ApiResponse<BalanceUpdateResponse>>> CreditBalance([FromBody] BalanceUpdateRequest request)
        {
            m_logger.LogInformation("UpdateBalance called with request: {@Request}", request);

            if (request == null)
                return BadRequest(ApiResponse<BalanceUpdateResponse>.FailureResponse("No request content."));

            if (request.amount <= 0)
                return BadRequest(ApiResponse<BalanceUpdateResponse>.FailureResponse("Amount must be greater than zero."));

            // You don't need the try-catch here if you use global exception handling middleware.
            var result = await m_playerService.CreditBalanceAsync(request.playerId, request.amount);
            return this.ToApiResponse(result);
        }

        /// <summary>
        /// Creates a new player with optional initial balance (defaults to 0.00)
        /// </summary>
        /// <param name="initialBalance">Optional initial balance for the player</param>
        /// <returns>The newly created player information</returns>
        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<BalanceUpdateResponse>>> CreatePlayer([FromBody] CreatePlayerRequest request)
        {
            m_logger.LogInformation($"CreatePlayer called with initial balance: {request.amount}");

            // PlayerService will validate if balance is negative
            var result = await m_playerService.CreatePlayerAsync(request.amount);
            return this.ToApiResponse(result);
        }

        /// <summary>
        /// Processes a player's bet and returns the result of the spin
        /// </summary>
        /// <param name="request">The spin request containing the player ID and bet amount</param>
        /// <returns>The result of the spin, including the reel symbols, win amount, and updated player balance</returns>
        [HttpPost("spin")]
        public async Task<ActionResult<ApiResponse<SpinResponse>>> Spin([FromBody] SpinRequest request)
        {
            m_logger.LogInformation("Spin called with request: {@Request}", request);

            if (request == null)
                return BadRequest(ApiResponse<SpinResponse>.FailureResponse("No request content."));

            if (request.BetAmount <= 0)
                return BadRequest(ApiResponse<SpinResponse>.FailureResponse("Bet amount must be greater than zero."));

            if (string.IsNullOrWhiteSpace(request.PlayerId))
                return BadRequest(ApiResponse<SpinResponse>.FailureResponse("Player ID is required."));

            var result = await m_playerService.SpinAsync(request.PlayerId, request.BetAmount);
            return this.ToApiResponse(result);
        } 

        #endregion
    }
}
