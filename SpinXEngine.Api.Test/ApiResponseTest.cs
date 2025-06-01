namespace SpinXEngine.Api.Test
{
    [TestFixture]
    public class ApiResponseTests
    {
        [Test]
        public void SuccessResponse_ShouldReturnCorrectResponse()
        {
            // Arrange
            var data = "Test Data";
            var message = "Operation successful";

            // Act
            var response = ApiResponse<object>.SuccessResponse(data, message);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null, "Response should not be Null");
                Assert.That(response.Success, Is.True, $"SuccessResponse should be Success:{true}");
                Assert.That(response.Data, Is.EqualTo(data), $"Response Data should match with {data}");
                Assert.That(response.Message, Is.EqualTo(message), $"Response Message should match with {message}");
            });
        }

        [Test]
        public void FailureResponse_ShouldReturnFailureResponse()
        {
            // Arrange
            var errorMessage = "An error occurred";

            // Act
            var response = ApiResponse<object>.FailureResponse(errorMessage);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null, "Response should not be Null");
                Assert.That(response.Data, Is.Null, "Response Data should be Null");
                Assert.That(response.Success, Is.False, $"FailureResponse should be Success:{false}");
                Assert.That(errorMessage, Is.EqualTo(response.Message), $"It set Message:{errorMessage}");
            });
        }

        [Test]
        public void SuccessResponse_WithNullData_ShouldReturnNullDataResponse()
        {
            // Arrange & Act
            var response = ApiResponse<string>.SuccessResponse(null);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.True, $"Response.Success must be {true}");
                Assert.That(response.Data, Is.Null, $"Response.Data must be {null}");
            });
        }
    }
}