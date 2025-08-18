using FluentValidation.TestHelper;
using TaskManagement.HexagonalArchitecture.Api.Controllers.Users;
using Xunit;

namespace TaskManagement.HexagonalArchitecture.Api.Test.Controllers.Users
{
    public class UserRequestTests
    {
        private readonly UserRequestValidator _validator = new();

        private static UserRequest ValidRequest() => new()
        {
            FirstName = "Ben",
            LastName = "Parker",
            Email = "ben.parker@example.com",
            Password = "P@ss123" // has uppercase and a special character
        };

        [Fact]
        public void Should_Pass_When_Request_Is_Valid()
        {
            // Arrange
            var request = ValidRequest();

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        // FirstName rules
        [Theory]
        [InlineData("")]
        public void FirstName_Should_Fail_When_Empty(string value)
        {
            var request = ValidRequest();
            request.FirstName = value;

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.FirstName)
                .WithErrorMessage("The field FirstName must be a minimum length of '1' and maximum length of '256'.");
        }

        [Fact]
        public void FirstName_Should_Fail_When_Exceeds_Max_Length()
        {
            var request = ValidRequest();
            request.FirstName = new string('a', 257);

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.FirstName)
                .WithErrorMessage("The field FirstName must be a minimum length of '1' and maximum length of '256'.");
        }

        // LastName rules
        [Theory]
        [InlineData("")]
        public void LastName_Should_Fail_When_Empty(string value)
        {
            var request = ValidRequest();
            request.LastName = value;

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.LastName)
                .WithErrorMessage("The field LastName must be a minimum length of '1' and maximum length of '256'.");
        }

        [Fact]
        public void LastName_Should_Fail_When_Exceeds_Max_Length()
        {
            var request = ValidRequest();
            request.LastName = new string('b', 257);

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.LastName)
                .WithErrorMessage("The field LastName must be a minimum length of '1' and maximum length of '256'.");
        }

        // Email rules
        [Theory]
        [InlineData("")]
        public void Email_Should_Fail_When_Empty(string value)
        {
            var request = ValidRequest();
            request.Email = value;

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("The field Email must be a minimum length of '1' and maximum length of '256'.");
        }

        [Fact]
        public void Email_Should_Fail_When_Exceeds_Max_Length()
        {
            var local = new string('c', 200);
            var domain = new string('d', 57);
            var request = ValidRequest();
            request.Email = $"{local}@{domain}.com"; // 200 + 1 + 57 + 4 = 262 > 256

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("The field Email must be a minimum length of '1' and maximum length of '256'.");
        }

        [Theory]
        [InlineData("plainaddress")]
        [InlineData("missing-at.com")]
        [InlineData("missing.domain@")]
        public void Email_Should_Fail_When_Invalid_Format(string email)
        {
            var request = ValidRequest();
            request.Email = email;

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("The Email field is not a valid e-mail address.");
        }

        // Password rules
        [Theory]
        [InlineData("short")] // 5 chars
        public void Password_Should_Fail_When_Too_Short(string pwd)
        {
            var request = ValidRequest();
            request.Password = pwd;

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("The field Password must be a minimum length of '6' and maximum length of '250'.");
        }

        [Fact]
        public void Password_Should_Fail_When_Too_Long()
        {
            var request = ValidRequest();
            request.Password = new string('X', 251) + "!"; // length 252

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("The field Password must be a minimum length of '6' and maximum length of '250'.");
        }

        [Theory]
        [InlineData("p@ssword1")] // has special but no uppercase
        [InlineData("passw@rd")]  // has special but no uppercase
        public void Password_Should_Fail_When_Missing_Uppercase(string pwd)
        {
            var request = ValidRequest();
            request.Password = pwd;

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("The field Password must contains 1 upper case character.");
        }

        [Theory]
        [InlineData("Password1")] // has uppercase but no special
        [InlineData("Password")]  // has uppercase but no special
        public void Password_Should_Fail_When_Missing_Special_Character(string pwd)
        {
            var request = ValidRequest();
            request.Password = pwd;

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("The field Password must contains 1 special character at least.");
        }

        [Fact]
        public void Password_Should_Pass_When_Meets_All_Requirements()
        {
            var request = ValidRequest();
            request.Password = "GoodP@ss1"; // >=6, has uppercase, has special

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_Report_Multiple_Password_Errors_When_Multiple_Rules_Fail()
        {
            // Length ok to let other rules trigger too
            var request = ValidRequest();
            request.Password = "lower1"; // length 6, no uppercase, no special

            var result = _validator.TestValidate(request);

            // Should fail for missing uppercase
            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("The field Password must contains 1 upper case character.");

            // Should fail for missing special
            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("The field Password must contains 1 special character at least.");
        }
    }
}
