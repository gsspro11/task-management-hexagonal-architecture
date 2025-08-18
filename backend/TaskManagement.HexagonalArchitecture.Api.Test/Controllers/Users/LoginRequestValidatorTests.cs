using FluentValidation.TestHelper;
using TaskManagement.HexagonalArchitecture.Api.Controllers.Users;
using Xunit;

namespace TaskManagement.HexagonalArchitecture.Api.Test.Controllers.Users
{
    public class LoginRequestValidatorTests
    {
        private readonly LoginRequestValidator _validator = new();

        [Fact]
        public void Validate_Should_Pass_When_Request_Is_Valid()
        {
            // Arrange
            var model = new LoginRequest
            {
                UserName = "ben.parker@email.com",
                Password = "P@ss123"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_Should_Fail_When_UserName_Is_Empty()
        {
            // Arrange
            var model = new LoginRequest
            {
                UserName = "",
                Password = "P@ss123"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserName)
                  .WithErrorMessage("The field UserName is required.");
        }

        [Fact]
        public void Validate_Should_Fail_When_UserName_Is_Not_A_Valid_Email()
        {
            // Arrange
            var model = new LoginRequest
            {
                UserName = "not-an-email",
                Password = "P@ss123"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserName)
                  .WithErrorMessage("The field UserName must be a valid email address.");
        }

        [Fact]
        public void Validate_Should_Fail_When_Password_Is_Empty()
        {
            // Arrange
            var model = new LoginRequest
            {
                UserName = "valid@example.com",
                Password = ""
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("The field Password is required.");
        }

        [Fact]
        public void Validate_Should_Return_All_Errors_When_Multiple_Fields_Are_Invalid()
        {
            // Arrange
            var model = new LoginRequest
            {
                UserName = "",
                Password = ""
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserName)
                  .WithErrorMessage("The field UserName is required.");
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("The field Password is required.");
        }
    }
}
