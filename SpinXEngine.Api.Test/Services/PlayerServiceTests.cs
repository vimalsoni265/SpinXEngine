using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SpinXEngine.Core;
using SpinXEngine.Core.GameDesigner;
using SpinXEngine.Core.Implementation;
using SpinXEngine.Repository.Interfaces;
using SpinXEngine.Repository.Models;

namespace SpinXEngine.Api.Test.Services
{
    [TestFixture]
    public class PlayerServiceTests
    {
        private Mock<IPlayerRepository> m_mockPlayerRepository;
        private Mock<ILogger<PlayerService>> m_mockLogger;
        private Mock<ISpinGame> m_mockSpinGame;
        private Mock<IOptionsMonitor<GameSetting>> m_mockGameSettings;
        private PlayerService m_playerService;

        [SetUp]
        public void Setup()
        {
            m_mockPlayerRepository = new Mock<IPlayerRepository>();
            m_mockLogger = new Mock<ILogger<PlayerService>>();
            m_mockSpinGame = new Mock<ISpinGame>();
            m_mockGameSettings = new Mock<IOptionsMonitor<GameSetting>>();

            m_mockGameSettings.Setup(x => x.CurrentValue).Returns(new GameSetting { ReelColumns = 5, ReelRows = 3 });
            m_playerService = new PlayerService(m_mockPlayerRepository.Object,m_mockLogger.Object,() => m_mockSpinGame.Object,m_mockGameSettings.Object);
        }

        [Test]
        public async Task CreditBalanceAsync_WithValidInputs_ShouldUpdateBalanceAndReturnSuccess()
        {
            // Arrange
            var playerId = "player123";
            var initialBalance = 100m;
            var creditAmount = 50m;
            var expectedNewBalance = 150m;

            var player = new Player { Id = playerId, Balance = initialBalance };
            var updatedPlayer = new Player { Id = playerId, Balance = expectedNewBalance };

            m_mockPlayerRepository.Setup(x => x.GetBalanceAsync(playerId))
                                 .ReturnsAsync(initialBalance);
            m_mockPlayerRepository.Setup(x => x.UpdateBalance(playerId, expectedNewBalance))
                                 .ReturnsAsync(updatedPlayer);

            // Act
            var result = await m_playerService.CreditBalanceAsync(playerId, creditAmount);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True, "Operation should be successful");
                Assert.That(result.Status, Is.EqualTo(ServiceStatus.Success));
                Assert.That(result.Data.NewBalance, Is.EqualTo(expectedNewBalance), $"Balance should be {expectedNewBalance}");
            });

            m_mockPlayerRepository.Verify(x => x.UpdateBalance(playerId, expectedNewBalance), Times.Once);
        }

        [Test]
        public async Task CreatePlayerAsync_WithNegativeBalance_ShouldReturnValidationError()
        {
            // Arrange
            var negativeAmount = -50m;

            // Act
            var result = await m_playerService.CreatePlayerAsync(negativeAmount);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False, "Operation should fail");
                Assert.That(result.Status, Is.EqualTo(ServiceStatus.ValidationError));
                Assert.That(result.Message, Is.EqualTo("Initial balance cannot be negative."));
            });

            m_mockPlayerRepository.Verify(x => x.CreatePlayerAsync(It.IsAny<decimal>()), Times.Never,
                "Repository method should not be called");
        }

        [Test]
        public async Task SpinAsync_WithInsufficientBalance_ShouldReturnValidationError()
        {
            // Arrange
            var playerId = "player123";
            var betAmount = 100m;
            var playerBalance = 50m; // Less than betAmount

            var player = new Player { Id = playerId, Balance = playerBalance };

            m_mockPlayerRepository.Setup(x => x.GetByIdAsync(playerId))
                .ReturnsAsync(player);

            // Act
            var result = await m_playerService.SpinAsync(playerId, betAmount);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False, "Operation should fail");
                Assert.That(result.Status, Is.EqualTo(ServiceStatus.ValidationError));
                Assert.That(result.Message, Is.EqualTo("Insufficient balance to place this bet."));
            });

            m_mockPlayerRepository.Verify(x => x.UpdateBalance(It.IsAny<string>(), It.IsAny<decimal>()),
                Times.Never, "Balance should not be updated");
        }

        [Test]
        public async Task GetAllAsync_WhenSucceeds_ShouldReturnAllPlayers()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player { Id = "player1", Balance = 100m },
                new Player { Id = "player2", Balance = 200m }
            };

            m_mockPlayerRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(players);

            // Act
            var result = await m_playerService.GetAllAsync();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True, "Operation should succeed");
                Assert.That(result.Status, Is.EqualTo(ServiceStatus.Success));
                Assert.That(result.Data, Is.EqualTo(players));
            });
        }

        [Test]
        public async Task SpinAsync_WithWin_ShouldUpdateBalanceAndReturnSuccess()
        {
            // Arrange
            var playerId = "player123";
            var initialBalance = 100m;
            var betAmount = 10m;
            var winAmount = 25m;
            // Initial balance - bet amount
            var balanceAfterBet = 90m;
            // Balance after bet + win amount
            var finalBalance = 115m;
            var matrix = new int[3, 3];
            var reelSymbols = "[[1,2,3],[4,5,6],[7,8,9]]";

            // Setup mock for reel symbols matrix and win calculation
            m_mockPlayerRepository.Setup(x => x.GetByIdAsync(playerId))
                .ReturnsAsync(new Player { Id = playerId, Balance = initialBalance });
            m_mockPlayerRepository.Setup(x => x.UpdateBalance(playerId, balanceAfterBet))
                .ReturnsAsync(new Player { Id = playerId, Balance = balanceAfterBet });
            m_mockPlayerRepository.Setup(x => x.UpdateBalance(playerId, finalBalance))
                .ReturnsAsync(new Player { Id = playerId, Balance = finalBalance });

            // Setup mock for spin game
            m_mockSpinGame.Setup(x => x.GenerateReelSymbols(It.IsAny<int>(), It.IsAny<int>())).Returns(matrix);
            m_mockSpinGame.Setup(x => x.Spin(betAmount)).Returns(winAmount);
            m_mockSpinGame.Setup(x => x.ConvertMatrixToString()).Returns(reelSymbols);

            // Act
            var result = await m_playerService.SpinAsync(playerId, betAmount);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True, "Operation should succeed");
                Assert.That(result.Status, Is.EqualTo(ServiceStatus.Success));
                Assert.That(result.Data?.CurrentBalance, Is.EqualTo(finalBalance));
                Assert.That(result.Data?.Win, Is.EqualTo(winAmount));
                Assert.That(result.Data?.ReelSymbols, Is.EqualTo(reelSymbols));
            });
        }
    }
}
