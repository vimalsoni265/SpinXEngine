using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SpinXEngine.Api.Controllers.V1;
using SpinXEngine.Repository.Models;

namespace SpinXEngine.Api.Test.Controllers
{
    [TestFixture]
    public class ConfigControllerTests
    {
        private Mock<IOptionsMonitor<GameSetting>> m_mockGameSettingsMonitor;
        private Mock<ILogger<ConfigController>> m_mockLogger;
        private ConfigController m_controller;
        private GameSetting m_defaultGameSetting;

        [SetUp]
        public void Setup()
        {
            m_mockGameSettingsMonitor = new Mock<IOptionsMonitor<GameSetting>>();
            m_mockLogger = new Mock<ILogger<ConfigController>>();

            // Create a default game setting for tests
            m_defaultGameSetting = new GameSetting { ReelRows = 3, ReelColumns = 5 };

            // Setup the monitor to return our default settings
            m_mockGameSettingsMonitor.Setup(x => x.CurrentValue).Returns(m_defaultGameSetting);
            m_controller = new ConfigController(m_mockGameSettingsMonitor.Object,m_mockLogger.Object);
        }

        [Test]
        public void GetGameSettings_ShouldReturnCurrentSettings()
        {
            // Act
            var result = m_controller.GetGameSettings();

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());

            var okResult = result.Result as OkObjectResult;
            var apiResponse = okResult.Value as ApiResponse<GameSetting>;

            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.Success, Is.True, "ApiResponse should indicate success");
                Assert.That(apiResponse.Data, Is.SameAs(m_defaultGameSetting), "Returned settings should match the monitor's current value");
                Assert.That(apiResponse.Data.ReelRows, Is.EqualTo(3), "ReelRows should match expected value");
                Assert.That(apiResponse.Data.ReelColumns, Is.EqualTo(5), "ReelColumns should match expected value");
            });
        }

        [Test]
        public void GetGameSettings_WhenMonitorReturnsNull_ShouldHandleGracefully()
        {
            // Arrange
            m_mockGameSettingsMonitor.Setup(x => x.CurrentValue)
                .Returns((GameSetting)null);

            // Act
            var result = m_controller.GetGameSettings();

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());

            var okResult = result.Result as OkObjectResult;
            var apiResponse = okResult.Value as ApiResponse<GameSetting>;

            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.Success, Is.True, "ApiResponse should indicate success");
                Assert.That(apiResponse.Data, Is.Null, "Data should be null when monitor returns null");
            });
        }

        [TestCase(0,5)]
        [TestCase(3, 0)]
        [TestCase(-3, -6)]
        public void UpdateGameSettings_WithZeroReelRows_ShouldReturnBadRequest(int rows, int cols)
        {
            // Arrange
            var invalidSettings = new GameSetting { ReelRows = rows, ReelColumns = cols };

            // Act
            var result = m_controller.UpdateGameSettings(invalidSettings);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());

            var badRequestResult = result.Result as BadRequestObjectResult;
            var apiResponse = badRequestResult.Value as ApiResponse<GameSetting>;

            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.Success, Is.False, "ApiResponse should indicate failure");
                Assert.That(apiResponse.Message, Is.Not.Null);
                Assert.That(m_defaultGameSetting.ReelRows, Is.EqualTo(3), "Original ReelRows should remain unchanged");
                Assert.That(m_defaultGameSetting.ReelColumns, Is.EqualTo(5), "Original ReelColumns should remain unchanged");
            });
        }

        [Test]
        public void UpdateGameSettings_WithNullSettings_ShouldReturnServerError()
        {
            // Act
            var result = m_controller.UpdateGameSettings(null);

            // Assert
            Assert.That(result.Result, Is.TypeOf<ObjectResult>());
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult.StatusCode, Is.EqualTo(500));

            var apiResponse = statusCodeResult.Value as ApiResponse<GameSetting>;
            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.Success, Is.False);
                Assert.That(apiResponse.Message, Is.EqualTo("Failed to update settings"));
            });
        }

        [Test]
        public void UpdateGameSettings_WithValidSettings_ShouldUpdateAndReturnSuccess()
        {
            // Arrange
            var newSettings = new GameSetting { ReelRows = 4, ReelColumns = 6 };

            // Act
            var result = m_controller.UpdateGameSettings(newSettings);

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());

            var okResult = result.Result as OkObjectResult;
            var apiResponse = okResult.Value as ApiResponse<GameSetting>;

            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.Success, Is.True, "ApiResponse should indicate success");
                Assert.That(m_defaultGameSetting.ReelRows, Is.EqualTo(4), "ReelRows should be updated to 4");
                Assert.That(m_defaultGameSetting.ReelColumns, Is.EqualTo(6), "ReelColumns should be updated to 6");
            });
        }

        [Test]
        public void UpdateGameSettings_WithExtremelyLargeValues_ShouldHandleGracefully()
        {
            // Arrange
            var extremeSettings = new GameSetting { ReelRows = int.MaxValue, ReelColumns = int.MaxValue };

            // Act
            var result = m_controller.UpdateGameSettings(extremeSettings);

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());

            // Verify settings were updated despite extreme values
            Assert.Multiple(() =>
            {
                Assert.That(m_defaultGameSetting.ReelRows, Is.EqualTo(int.MaxValue));
                Assert.That(m_defaultGameSetting.ReelColumns, Is.EqualTo(int.MaxValue));
            });
        }
    }
}
