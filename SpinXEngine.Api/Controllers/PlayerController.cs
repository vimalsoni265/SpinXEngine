using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SpinXEngine.Api.WebModels.Error;
using SpinXEngine.Api.WebModels.Player;
using System.Net;

namespace SpinXEngine.Api.Controllers
{
    public class PlayerController : ControllerBase
    {
        private readonly ILogger<PlayerController> _logger;
        // We would inject MongoDB service here
        // private readonly IPlayerRepository _playerRepository;

        public PlayerController(ILogger<PlayerController> logger)
        {
            _logger = logger;
            // _playerRepository = playerRepository;
        }

        //GET /api/player/{id}/balance	Get current balance
        [HttpGet("{id}/balance")]
        public IActionResult GetBalance(int id)
        {
            // Here you would typically retrieve the player's balance from a database or service
            // For demonstration, we return a mock balance
            var balance = 100.0; // Mock balance
            return Ok(new { PlayerId = id, Balance = balance });
        }

        /// <summary>
        /// Updates the player's balance by adding the specified amount
        /// </summary>
        /// <param name="id">The player ID</param>
        /// <param name="request">The balance update request containing the amount to add</param>
        /// <returns>The updated player balance information</returns>
        [HttpPost("{id}/balance")]
        public async Task<IActionResult> UpdateBalance(string id, [FromBody] BalanceUpdateRequest request)
        {
            if (request == null)
                return StatusCode(StatusCodes.Status204NoContent,
                    ErrorResponseBuilder.Create(ErrorCodes.NoContent, nameof(LoginRequest)).Build());
            if(request.Amount == 0)
                return StatusCode(StatusCodes.Status400BadRequest,
                    ErrorResponseBuilder.Create(ErrorCodes.InvalidAmount, nameof(BalanceUpdateRequest.Amount)).Build());
            try
            {
                _logger.LogInformation($"Updating balance for player {id} with amount {request.Amount}");

                var result = _playerService.CreditBalance(id, request.Amount);
                return Ok(UpdateBalanceResponse.From(result));

                /*
                 var result = _userService.Login(request.Username, request.Password);
                 return Ok(LoginResponse.From(result));
                 */

                // 1. Retrieve current player from MongoDB
                // var player = await _playerRepository.GetPlayerByIdAsync(id);
                // 2. If player doesn't exist, return error
                // if (player is null)
                // {
                //    return StatusCode(StatusCodes.Status404NotFound, 
                //        ErrorResponseBuilder.Create(ErrorCodes.PlayerNotFound, $"Player with ID {id} not found.").Build());
                // }
                // 3. Update player balance
                // player.Credit(request.Amount);
                // 4. Update player record in MongoDB
                // player.Balance = newBalance;
                // await _playerRepository.UpdatePlayerAsync(player);
                // 5. Return updated balance information
                //return Ok(new BalanceUpdateResponse
                //{
                //    PlayerId = id.ToString(),
                //    NewBalance = player.Balance
                //});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating balance for player {PlayerId}", id);
                return StatusCode(500, "An error occurred while updating the player's balance.");
            }
        }
    }
}
}
