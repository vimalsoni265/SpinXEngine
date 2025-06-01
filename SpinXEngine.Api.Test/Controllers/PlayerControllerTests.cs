using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SpinXEngine.Api.Controllers.V1;
using SpinXEngine.Common.Contracts;
using SpinXEngine.Core;
using SpinXEngine.Core.Interface;
using SpinXEngine.Repository.Models;

namespace SpinXEngine.Api.Test.Controllers
{
    [TestFixture]
    public class PlayerControllerTests
    {
        private Mock<IPlayerService> m_mockPlayerService;
        private Mock<ILogger<PlayerController>> m_mockLogger;
        private PlayerController m_controller;

        [SetUp]
        public void Setup()
        {
            m_mockPlayerService = new Mock<IPlayerService>();
            m_mockLogger = new Mock<ILogger<PlayerController>>();
            m_controller = new PlayerController(m_mockPlayerService.Object, m_mockLogger.Object);
        }

        #region GetAllPlayers Tests

        [Test]
        public async Task GetAllPlayers_WhenSuccessful_ShouldReturnOkWithPlayers()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player { Id = "1", Balance = 100m },
                new Player { Id = "2", Balance = 200m }
            };

            m_mockPlayerService.Setup(x => x.GetAllAsync())
                .ReturnsAsync(ServiceResult<IEnumerable<Player>>.Ok(players));

            // Act
            var result = await m_controller.GetAllPlayers();

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());

            var okResult = result.Result as OkObjectResult;
            var apiResponse = okResult.Value as ApiResponse<IEnumerable<Player>>;

            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.Success, Is.True, $"ApiResponse must be Success:{true}");
                Assert.That(apiResponse.Data, Is.EqualTo(players), "ApiResponse.Data must be a players data");
            });
        }

        [Test]
        public async Task GetAllPlayers_WhenServiceFails_ShouldReturnAppropriateErrorResponse()
        {
            // Arrange
            var errorMessage = "Database connection error";
            m_mockPlayerService.Setup(x => x.GetAllAsync())
                .ReturnsAsync(ServiceResult<IEnumerable<Player>>.ServerError(errorMessage));

            // Act
            var result = await m_controller.GetAllPlayers();

            // Assert
            Assert.That(result.Result, Is.TypeOf<ObjectResult>());
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult.StatusCode, Is.EqualTo(500), "Method should throw InternalService-500");

            var apiResponse = statusCodeResult.Value as ApiResponse<IEnumerable<Player>>;
            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.Success, Is.False, $"ApiResponse must be Success:{false}");
                Assert.That(apiResponse.Message, Is.EqualTo(errorMessage), "ApiResponse.Message matches the errorMessage");
            });
        }

        #endregion

        #region UpdateBalance Tests

        [Test]
        public async Task UpdateBalance_WithValidRequest_ShouldReturnUpdatedBalance()
        {
            // Arrange
            var request = new BalanceUpdateRequest("player1", 50m);
            var response = new BalanceUpdateResponse { NewBalance = 150m };

            m_mockPlayerService.Setup(x => x.CreditBalanceAsync(request.playerId, request.amount))
                .ReturnsAsync(ServiceResult<BalanceUpdateResponse>.Ok(response));

            // Act
            var result = await m_controller.CreditBalance(request);

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());


            var okResult = result.Result as OkObjectResult;
            var apiResponse = okResult.Value as ApiResponse<BalanceUpdateResponse>;
            Assert.Multiple(() =>
            {
                Assert.That(apiResponse, Is.Not.Null);
                Assert.That(apiResponse?.Success, Is.True, $"ApiResponse must be Success:{true}");
                Assert.That(apiResponse?.Data?.NewBalance, Is.EqualTo(response.NewBalance), $"NewBalance must be {response.NewBalance}");
            });
        }

        [Test]
        public async Task UpdateBalance_WithNullRequest_ShouldReturnBadRequest()
        {
            // Act
            var result = await m_controller.CreditBalance(null);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            var apiResponse = badRequestResult.Value as ApiResponse<BalanceUpdateResponse>;

            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.Success, Is.False, $"ApiResponse must be Success:{false}");
                Assert.That(apiResponse.Message, Is.EqualTo("No request content."));
            });
        }

        [Test]
        public async Task UpdateBalance_WithNegativeAmount_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new BalanceUpdateRequest("player1", -50m);

            // Act
            var result = await m_controller.CreditBalance(request);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            var apiResponse = badRequestResult.Value as ApiResponse<BalanceUpdateResponse>;

            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.Success, Is.False, $"ApiResponse must be Success:{false}");
                Assert.That(apiResponse.Message, Is.EqualTo("Amount must be greater than zero."));
            });
        }

        [Test]
        public async Task UpdateBalance_WithZeroAmount_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new BalanceUpdateRequest("player1", 0m);

            // Act
            var result = await m_controller.CreditBalance(request);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            var apiResponse = badRequestResult.Value as ApiResponse<BalanceUpdateResponse>;

            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.Success, Is.False, $"ApiResponse must be Success:{false}");
                Assert.That(apiResponse.Message, Is.EqualTo("Amount must be greater than zero."));
            });
        }

        [Test]
        public async Task UpdateBalance_WhenPlayerNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var request = new BalanceUpdateRequest("nonexistentPlayer", 50m);
            var errorMessage = "Player not found.";

            m_mockPlayerService.Setup(x => x.CreditBalanceAsync(request.playerId, request.amount))
                .ReturnsAsync(ServiceResult<BalanceUpdateResponse>.NotFound(errorMessage));

            // Act
            var result = await m_controller.CreditBalance(request);

            // Assert
            Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
            var apiResponse = notFoundResult?.Value as ApiResponse<BalanceUpdateResponse>;

            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.Success, Is.False);
                Assert.That(apiResponse.Message, Is.EqualTo(errorMessage));
            });
        }

        #endregion

        #region Spin Tests

        [Test]
        public async Task Spin_WithValidRequest_ShouldReturnSpinResponse()
        {
            // Arrange
            var request = new SpinRequest("player1", 10m);
            var response = new SpinResponse
            {
                ReelSymbols = "[[1,2,3],[4,5,6],[7,8,9]]",
                Win = 20m,
                CurrentBalance = 110m
            };

            m_mockPlayerService.Setup(x => x.SpinAsync(request.PlayerId, request.BetAmount))
                .ReturnsAsync(ServiceResult<SpinResponse>.Ok(response));

            // Act
            var result = await m_controller.Spin(request);

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var apiResponse = okResult.Value as ApiResponse<SpinResponse>;

            Assert.Multiple(() =>
            {
                Assert.That(apiResponse?.Success, Is.True, $"Response Success must be {true}");
                Assert.That(apiResponse?.Data.ReelSymbols, Is.EqualTo("[[1,2,3],[4,5,6],[7,8,9]]"));
                Assert.That(apiResponse.Data.Win, Is.EqualTo(20m));
                Assert.That(apiResponse.Data.CurrentBalance, Is.EqualTo(110m));
            });
        }

        [Test]
        public async Task Spin_WithNullRequest_ShouldReturnBadRequest()
        {
            // Act
            var result = await m_controller.Spin(null);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            var apiResponse = badRequestResult.Value as ApiResponse<SpinResponse>;

            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.Success, Is.False);
                Assert.That(apiResponse.Message, Is.EqualTo("No request content."));
            });
        }

        [Test]
        public async Task Spin_WithNegativeBetAmount_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new SpinRequest("player1", -10m);

            // Act
            var result = await m_controller.Spin(request);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            var apiResponse = badRequestResult.Value as ApiResponse<SpinResponse>;

            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.Success, Is.False);
                Assert.That(apiResponse.Message, Is.EqualTo("Bet amount must be greater than zero."));
            });
        }

        [Test]
        public async Task Spin_WithZeroBetAmount_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new SpinRequest("player1", 0m);

            // Act
            var result = await m_controller.Spin(request);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            var apiResponse = badRequestResult.Value as ApiResponse<SpinResponse>;

            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.Success, Is.False);
                Assert.That(apiResponse.Message, Is.EqualTo("Bet amount must be greater than zero."));
            });
        }

        [Test]
        public async Task Spin_WithEmptyPlayerId_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new SpinRequest("", 10m);

            // Act
            var result = await m_controller.Spin(request);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            var apiResponse = badRequestResult.Value as ApiResponse<SpinResponse>;

            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.Success, Is.False);
                Assert.That(apiResponse.Message, Is.EqualTo("Player ID is required."));
            });
        }

        [Test]
        public async Task Spin_WithWhitespacePlayerId_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new SpinRequest("   ", 10m);

            // Act
            var result = await m_controller.Spin(request);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            var apiResponse = badRequestResult.Value as ApiResponse<SpinResponse>;

            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.Success, Is.False);
                Assert.That(apiResponse.Message, Is.EqualTo("Player ID is required."));
            });
        }

        [Test]
        public async Task Spin_WhenPlayerNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var request = new SpinRequest("nonexistentPlayer", 10m);
            var errorMessage = "Player not found.";

            m_mockPlayerService.Setup(x => x.SpinAsync(request.PlayerId, request.BetAmount))
                .ReturnsAsync(ServiceResult<SpinResponse>.NotFound(errorMessage));

            // Act
            var result = await m_controller.Spin(request);

            // Assert
            Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
            var apiResponse = notFoundResult.Value as ApiResponse<SpinResponse>;

            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.Success, Is.False);
                Assert.That(apiResponse.Message, Is.EqualTo(errorMessage));
            });
        }

        [Test]
        public async Task Spin_WhenInsufficientBalance_ShouldReturnValidationError()
        {
            // Arrange
            var request = new SpinRequest("player1", 1000m);
            var errorMessage = "Insufficient balance to place this bet.";

            m_mockPlayerService.Setup(x => x.SpinAsync(request.PlayerId, request.BetAmount))
                .ReturnsAsync(ServiceResult<SpinResponse>.ValidationError(errorMessage));

            // Act
            var result = await m_controller.Spin(request);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            var apiResponse = badRequestResult.Value as ApiResponse<SpinResponse>;

            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.Success, Is.False);
                Assert.That(apiResponse.Message, Is.EqualTo(errorMessage));
            });
        }

        #endregion

        #region Edge Cases for Decimal Values

        [Test]
        public async Task CreditBalance_WithExtremelyLargeAmount_ShouldHandleGracefully()
        {
            // Arrange
            var request = new BalanceUpdateRequest("player1", decimal.MaxValue);

            m_mockPlayerService.Setup(x => x.CreditBalanceAsync(request.playerId, request.amount))
                .ReturnsAsync(ServiceResult<BalanceUpdateResponse>.Ok(new BalanceUpdateResponse { NewBalance = decimal.MaxValue }));

            // Act
            var result = await m_controller.CreditBalance(request);

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>(), "Should return OK response for valid large amount");
        }

        [Test]
        public async Task CreditBalance_WithVerySmallAmount_ShouldHandleGracefully()
        {
            // Arrange
            var request = new BalanceUpdateRequest("player1", 0.0000001m);

            m_mockPlayerService.Setup(x => x.CreditBalanceAsync(request.playerId, request.amount))
                .ReturnsAsync(ServiceResult<BalanceUpdateResponse>.Ok(new BalanceUpdateResponse { NewBalance = 100.0000001m }));

            // Act
            var result = await m_controller.CreditBalance(request);

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>(), "Should return OK response for valid small amount");
        }

        #endregion

        #region Exception Handling Tests

        [Test]
        public async Task GetAllPlayers_WhenServiceThrowsException_ShouldThrowException()
        {
            // Arrange
            m_mockPlayerService.Setup(x => x.GetAllAsync())
                .ThrowsAsync(new Exception("Unexpected database error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(m_controller.GetAllPlayers);
        }

        [Test]
        public async Task CreditBalance_WhenServiceThrowsException_ShouldThrowException()
        {
            // Arrange
            var request = new BalanceUpdateRequest("player1", 100m);

            m_mockPlayerService.Setup(x => x.CreditBalanceAsync(request.playerId, request.amount))
                .ThrowsAsync(new Exception("Unexpected service error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await m_controller.CreditBalance(request));
        }

        [Test]
        public async Task Spin_WhenServiceThrowsException_ShouldThrowException()
        {
            // Arrange
            var request = new SpinRequest("player1", 50m);

            m_mockPlayerService.Setup(x => x.SpinAsync(request.PlayerId, request.BetAmount))
                .ThrowsAsync(new Exception("Game engine error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await m_controller.Spin(request));
        }

        #endregion
    }
}
