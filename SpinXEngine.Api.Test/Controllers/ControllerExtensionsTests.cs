using Microsoft.AspNetCore.Mvc;
using SpinXEngine.Core;
using System.Net;
using System.Reflection;

namespace SpinXEngine.Api.Test.Controllers
{
    [TestFixture]
    public class ControllerExtensionsTests
    {
        private TestController _controller;

        [SetUp]
        public void Setup()
        {
            _controller = new TestController();
        }

        [Test]
        public void ToApiResponse_SuccessResult_ShouldReturnsOkResult()
        {
            // Arrange
            var data = "Success data";
            var result = ServiceResult<string>.Ok(data);

            // Act
            var response = _controller.ToApiResponse(result);

            // Assert
            Assert.That(response.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)response.Result;
            Assert.That(okResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));

            var apiResponse = (ApiResponse<string>)okResult.Value;
            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.Success, Is.True);
                Assert.That(apiResponse.Data, Is.EqualTo(data));
            });
        }

        [Test]
        public void ToApiResponse_NotFoundResult_ShouldReturnsNotFoundResult()
        {
            // Arrange
            var message = "Resource not found";
            var result = ServiceResult<string>.NotFound(message);

            // Act
            var response = _controller.ToApiResponse(result);

            // Assert
            Assert.That(response.Result, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = (NotFoundObjectResult)response.Result;
            Assert.That(notFoundResult.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));

            var apiResponse = (ApiResponse<string>)notFoundResult.Value;
            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.Success, Is.False);
                Assert.That(apiResponse.Message, Is.EqualTo(message));
            });
        }

        [Test]
        public void ToApiResponse_ValidationErrorResult_ShouldReturnsBadRequestResult()
        {
            // Arrange
            var message = "Validation failed";
            var result = ServiceResult<int>.ValidationError(message);

            // Act
            var response = _controller.ToApiResponse(result);

            // Assert
            Assert.That(response.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = (BadRequestObjectResult)response.Result;
            Assert.That(badRequestResult.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));

            var apiResponse = (ApiResponse<int>)badRequestResult.Value;
            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.Success, Is.False);
                Assert.That(apiResponse.Message, Is.EqualTo(message));
            });
        }

        [Test]
        public void ToApiResponse_ServerErrorResult_ShouldReturnsStatusCode500Result()
        {
            // Arrange
            var message = "Server error occurred";
            var result = ServiceResult<double>.ServerError(message);

            // Act
            var response = _controller.ToApiResponse(result);

            // Assert
            Assert.That(response.Result, Is.TypeOf<ObjectResult>());
            var serverErrorResult = (ObjectResult)response.Result;
            Assert.That(serverErrorResult.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));

            var apiResponse = (ApiResponse<double>)serverErrorResult.Value;
            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.Success, Is.False);
                Assert.That(apiResponse.Message, Is.EqualTo(message));
            });
        }

        [Test]
        public void ToApiResponse_UnknownStatus_ShouldReturnsStatusCode500Result()
        {
            // Arrange
            // This test simulates an unknown ServiceStatus value (which shouldn't happen in practice)
            // but tests the default case in the switch statement
            var result = CreateServiceResult((ServiceStatus)999, "Test data", "Unknown status");

            // Act
            var response = _controller.ToApiResponse(result);
            var serverErrorResult = (ObjectResult)response.Result;
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Result, Is.TypeOf<ObjectResult>(), "Response should be of type ObjectResult");
                Assert.That(serverErrorResult.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError), "Status code should be 500 Internal Server Error");
            });
        }

        /// <summary>
        /// Represents a dummy controller.
        /// </summary>
        private class TestController : ControllerBase { }

        /// <summary>
        /// Helper method that creates a new instance of the 
        /// <see cref="ServiceResult{T}"/> class with the specified status, data, and message.
        /// </summary>
        private static ServiceResult<T> CreateServiceResult<T>(ServiceStatus status, T? data = default, string? message = null)
        {
            Type type = typeof(ServiceResult<T>);
            var constructor = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0];
            return (ServiceResult<T>)constructor.Invoke(new object[] { status, data, message });
        }

    }
}