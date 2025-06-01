using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SpinXEngine.Repository.Models;

namespace SpinXEngine.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        #region Private Members
        private readonly IOptionsMonitor<GameSetting> m_gameSettingsMonitor;
        private readonly ILogger<ConfigController> m_logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for ConfigController
        /// </summary>
        /// <param name="gameSettings"></param>
        /// <param name="gameSettingsMonitor"></param>
        /// <param name="logger"></param>
        public ConfigController(
            IOptionsMonitor<GameSetting> gameSettingsMonitor,
            ILogger<ConfigController> logger)
        {
            m_gameSettingsMonitor = gameSettingsMonitor;
            m_logger = logger;
        }

        #endregion

        #region Public Methods (API Endpoints)

        [HttpGet]
        public ActionResult<ApiResponse<GameSetting>> GetGameSettings()
        {
            // Use m_gameSettingsMonitor.CurrentValue instead of m_gameSettings.Value
            return Ok(ApiResponse<GameSetting>.SuccessResponse(m_gameSettingsMonitor.CurrentValue));
        }

        [HttpPut]
        public ActionResult<ApiResponse<GameSetting>> UpdateGameSettings([FromBody] GameSetting settings)
        {
            try
            {
                // Basic validation
                if (settings.ReelRows <= 0 || settings.ReelColumns <= 0)
                {
                    return BadRequest(ApiResponse<GameSetting>.FailureResponse(
                        "Reel rows and columns must be greater than zero"));
                }

                m_logger.LogInformation("Updating game settings: Rows={Rows}, Columns={Columns}",
                    settings.ReelRows, settings.ReelColumns);

                // Update the settings directly - no reflection needed
                var currentSettings = m_gameSettingsMonitor.CurrentValue;
                currentSettings.ReelRows = settings.ReelRows;
                currentSettings.ReelColumns = settings.ReelColumns;

                return Ok(ApiResponse<GameSetting>.SuccessResponse(currentSettings));
            }
            catch (Exception ex)
            {
                m_logger.LogError(ex, "Error updating game settings");
                return StatusCode(500, ApiResponse<GameSetting>.FailureResponse("Failed to update settings"));
            }
        } 

        #endregion
    }
}
