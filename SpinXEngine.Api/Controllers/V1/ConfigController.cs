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

        private readonly IOptions<GameSetting> m_gameSettings;
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
            IOptions<GameSetting> gameSettings,
            IOptionsMonitor<GameSetting> gameSettingsMonitor,
            ILogger<ConfigController> logger)
        {
            m_gameSettings = gameSettings;
            m_gameSettingsMonitor = gameSettingsMonitor;
            m_logger = logger;
        }

        #endregion

        #region Public Methods (API Endpoints)

        [HttpGet]
        public ActionResult<ApiResponse<GameSetting>> GetGameSettings()
        {
            return Ok(ApiResponse<GameSetting>.SuccessResponse(m_gameSettings.Value));
        }

        [HttpPut]
        public ActionResult<ApiResponse<GameSetting>> UpdateGameSettings([FromBody] GameSetting settings)
        {
            try
            {
                m_logger.LogInformation("Updating game settings: Rows={Rows}, Columns={Columns}",
                    settings.ReelRows, settings.ReelColumns);

                // Use reflection to update the in-memory settings
                var currentSettings = m_gameSettingsMonitor.CurrentValue;
                typeof(GameSetting).GetProperty(nameof(GameSetting.ReelRows))?.SetValue(currentSettings, settings.ReelRows);
                typeof(GameSetting).GetProperty(nameof(GameSetting.ReelColumns))?.SetValue(currentSettings, settings.ReelColumns);

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
