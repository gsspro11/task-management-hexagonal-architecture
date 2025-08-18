using FluentValidation.TestHelper;
using TaskManagement.HexagonalArchitecture.Api.Controllers.Comments;
using Xunit;

namespace TaskManagement.HexagonalArchitecture.Api.Test.Controllers.Comments
{
    public class CommentRequestTests
    {
        private readonly CommentRequestValidator _validator = new();

        private static CommentRequest ValidRequest() => new()
        {
            AssignmentId = Guid.NewGuid(),
            Description = "Comment"
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

        // AssignmentId rules
        [Fact]
        public void AssignmentId_Should_Fail_When_Empty()
        {
            var request = ValidRequest();
            request.AssignmentId = Guid.Empty;

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.AssignmentId)
                .WithErrorMessage("The field AssignmentId is required.");
        }

        [Fact]
        public void Description_Should_Fail_When_Exceeds_Max_Length()
        {
            var request = ValidRequest();
            request.Description = new string('a', 20001);

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("The field Description must be a minimum length of '1' and maximum length of '20000'.");
        }
    }
}
