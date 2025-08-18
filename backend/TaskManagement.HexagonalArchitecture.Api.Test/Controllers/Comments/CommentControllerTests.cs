using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagement.HexagonalArchitecture.Api.Controllers.Comments;
using TaskManagement.HexagonalArchitecture.Domain.Abstractions;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;
using TaskManagement.HexagonalArchitecture.Domain.Services.v1;
using Xunit;

namespace TaskManagement.HexagonalArchitecture.Api.Test.Controllers.Comments
{
    public class CommentControllerTests
    {
        private readonly Guid _userId;
        private readonly CommentController _controller;

        private readonly Mock<ICommentService> _commentServiceMock;

        public CommentControllerTests()
        {
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
            _commentServiceMock = new Mock<ICommentService>(MockBehavior.Strict);
            
            _userId = Guid.NewGuid();
            
            var httpContext = BuildHttpContextWithUser(_userId);
            httpContextAccessorMock.SetupGet(x => x.HttpContext).Returns(httpContext);

            _controller = new CommentController(httpContextAccessorMock.Object, _commentServiceMock.Object);
        }

        // GET /api/v1/comment/{commentId}
        [Fact]
        public async Task GetAsync_ShouldReturnOk_WhenServiceSucceeds()
        {
            // Arrange
            var id = Guid.NewGuid();
            var comment = new Comment(id, "Description", "john.doe@example.com");

            _commentServiceMock
                .Setup(s => s.GetAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CustomResult<Comment>.Success(comment));

            // Act
            var result = await _controller.GetAsync(id, It.IsAny<CancellationToken>());

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);

            // We don't know the anonymous type; reflect to validate fields.
            var value = ok.Value!;
            Assert.Equal(id, GetProp<Guid>(value, "Id"));
            Assert.Equal("Description", GetProp<string>(value, "Description"));
            Assert.Equal("john.doe@example.com", GetProp<string>(value, "UserName"));

            _commentServiceMock.VerifyAll();
        }

        [Fact]
        public async Task GetAsync_ShouldReturnBadRequest_WhenServiceFails()
        {
            // Arrange
            var id = Guid.NewGuid();
            var error = new CustomError("USR_404", "Comment not found");

            _commentServiceMock
                .Setup(s => s.GetAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CustomResult<Comment>.Failure(error));

            // Act
            var result = await _controller.GetAsync(id, It.IsAny<CancellationToken>());

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(error, bad.Value);
            _commentServiceMock.VerifyAll();
        }

        // GET /api/v1/comment?assignmentId=...
        [Fact]
        public async Task GetByAssignmentIdAsync_ShouldReturnOk_WhenServiceSucceeds()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();

            var comments = new List<Comment>()
            {
                new("Description 1", assignmentId, _userId),
                new("Description 2", assignmentId, _userId),
                new("Description 3", assignmentId, _userId),
            };

            _commentServiceMock
                .Setup(s => s.ListByAssignmentIdAsync(assignmentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CustomResult<List<Comment>>.Success(comments));

            // Act
            var result = await _controller.ListAsync(assignmentId, It.IsAny<CancellationToken>());

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(comments, ok.Value);
            _commentServiceMock.VerifyAll();
        }

        [Fact]
        public async Task GetByAssignmentIdAsync_ShouldReturnBadRequest_WhenServiceFails()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var error = new CustomError("USR_400", "Invalid query");

            _commentServiceMock
                .Setup(s => s.ListByAssignmentIdAsync(assignmentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CustomResult<List<Comment>>.Failure(error));

            // Act
            var result = await _controller.ListAsync(assignmentId, It.IsAny<CancellationToken>());

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(error, bad.Value);
            _commentServiceMock.VerifyAll();
        }

        // POST /api/v1/comment
        [Fact]
        public async Task CreateAsync_ShouldReturnCreated_WhenServiceSucceeds()
        {
            // Arrange
            var request = new CommentRequest
            {
                AssignmentId = Guid.NewGuid(),
                Description = "Comment",
            };

            var returnedComment = new Comment(request.Description, request.AssignmentId, _userId);

            _commentServiceMock
                .Setup(s => s.CreateAsync(request.Description, request.AssignmentId, _userId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CustomResult<Comment>.Success(returnedComment));

            // Act
            var result = await _controller.CreateAsync(request, It.IsAny<CancellationToken>());

            // Assert
            var created = Assert.IsType<CreatedResult>(result);
            Assert.Equal("api/v1/comment", created.Location);
            Assert.Equal(returnedComment.Id, created.Value);
            _commentServiceMock.VerifyAll();
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnBadRequest_WhenServiceFails()
        {
            // Arrange
            var request = new CommentRequest
            {
                AssignmentId = Guid.NewGuid(),
                Description = "Comment",
            };

            var errors = new[]
                { new CustomError("VAL_001", "Invalid assignment"), new CustomError("VAL_002", "Invalid description") };

            _commentServiceMock
                .Setup(s => s.CreateAsync(request.Description, request.AssignmentId, _userId,
                    It.IsAny<CancellationToken>())).ReturnsAsync(CustomResult<Comment>.Failure(errors));

            // Act
            var result = await _controller.CreateAsync(request, It.IsAny<CancellationToken>());

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Same(errors, bad.Value);
            _commentServiceMock.VerifyAll();
        }

        // PUT /api/v1/comment
        [Fact]
        public async Task UpdateAsync_ShouldReturnOk_WhenServiceSucceeds()
        {
            // Arrange
            var request = new CommentUpdateRequest
            {
                Id = Guid.NewGuid(),
                Description = "Comment",
            };

            var updated = new Comment(request.Id, request.Description, "john.doe@example.com");

            _commentServiceMock
                .Setup(s => s.UpdateAsync(request.Id, request.Description, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CustomResult<Comment>.Success(updated));

            // Act
            var result = await _controller.UpdateAsync(request, It.IsAny<CancellationToken>());

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(updated.Id, ok.Value);
            _commentServiceMock.VerifyAll();
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnBadRequest_WhenServiceFails()
        {
            // Arrange
            var request = new CommentUpdateRequest
            {
                Id = Guid.NewGuid(),
                Description = "Comment",
            };

            var errors = new[] { new CustomError("VAL_001", "Invalid comment") };

            _commentServiceMock
                .Setup(s => s.UpdateAsync(request.Id, request.Description, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CustomResult<Comment>.Failure(errors));

            // Act
            var result = await _controller.UpdateAsync(request, It.IsAny<CancellationToken>());

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Same(errors, bad.Value);
            _commentServiceMock.VerifyAll();
        }

        // DELETE /api/v1/comment
        [Fact]
        public async Task DeleteAsync_ShouldReturnOk_WhenServiceSucceeds()
        {
            // Arrange
            var id = Guid.NewGuid();

            var deleted = new Comment(id, "Description", "john.doe@example.com");

            _commentServiceMock
                .Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CustomResult<Comment>.Success(deleted));

            // Act
            var result = await _controller.DeleteAsync(id, It.IsAny<CancellationToken>());

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(deleted.Id, ok.Value);
            _commentServiceMock.VerifyAll();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnBadRequest_WhenServiceFails()
        {
            // Arrange
            var id = Guid.NewGuid();

            var errors = new[] { new CustomError("USR_404", "Comment not found") };

            _commentServiceMock
                .Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CustomResult<Comment>.Failure(errors));

            // Act
            var result = await _controller.DeleteAsync(id, It.IsAny<CancellationToken>());

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Same(errors, bad.Value);
            _commentServiceMock.VerifyAll();
        }

        private static T GetProp<T>(object obj, string name)
        {
            var prop = obj.GetType().GetProperty(name);
            Assert.NotNull(prop);
            return (T)prop!.GetValue(obj)!;
        }
        
        private static DefaultHttpContext BuildHttpContextWithUser(Guid userId)
        {
            var httpContext = new DefaultHttpContext();
            var identity = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            ], "TestAuth");

            httpContext.User = new ClaimsPrincipal(identity);
            return httpContext;
        }
    }
}