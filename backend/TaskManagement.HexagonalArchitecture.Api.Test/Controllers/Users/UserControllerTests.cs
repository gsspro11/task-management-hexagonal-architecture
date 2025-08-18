using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagement.HexagonalArchitecture.Api.Controllers.Users;
using TaskManagement.HexagonalArchitecture.Domain.Abstractions;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;
using TaskManagement.HexagonalArchitecture.Domain.Services.v1;
using Xunit;

namespace TaskManagement.HexagonalArchitecture.Api.Test.Controllers.Users
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>(MockBehavior.Strict);
            _controller = new UserController(_userServiceMock.Object);
        }

        // GET /api/v1/user/{userId}
        [Fact]
        public async Task GetAsync_ShouldReturnOk_WhenServiceSucceeds()
        {
            // Arrange
            var id = Guid.NewGuid();
            var user = new User("John", "Doe", "john.doe@example.com");

            _userServiceMock
                .Setup(s => s.GetAsync(id))
                .ReturnsAsync(CustomResult<User>.Success(user));

            // Act
            var result = await _controller.GetAsync(id);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);

            // We don't know the anonymous type; reflect to validate fields.
            var value = ok.Value!;
            Assert.Equal("John", GetProp<string>(value, "FirstName"));
            Assert.Equal("Doe", GetProp<string>(value, "LastName"));
            Assert.Equal("john.doe@example.com", GetProp<string>(value, "Email"));
            Assert.Equal("john.doe@example.com", GetProp<string>(value, "UserName"));

            _userServiceMock.VerifyAll();
        }

        [Fact]
        public async Task GetAsync_ShouldReturnBadRequest_WhenServiceFails()
        {
            // Arrange
            var id = Guid.NewGuid();
            var error = new CustomError("USR_404", "User not found");

            _userServiceMock
                .Setup(s => s.GetAsync(id))
                .ReturnsAsync(CustomResult<User>.Failure(error));

            // Act
            var result = await _controller.GetAsync(id);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(error, bad.Value);
            _userServiceMock.VerifyAll();
        }

        // GET /api/v1/user?email=...
        [Fact]
        public async Task GetByEmailAsync_ShouldReturnOk_WhenServiceSucceeds()
        {
            // Arrange
            const string email = "jane.doe@example.com";

            var user = new User("Jane", "Doe", email);

            _userServiceMock
                .Setup(s => s.GetByEmailAsync(email))
                .ReturnsAsync(CustomResult<User>.Success(user));

            // Act
            var result = await _controller.GetByEmailAsync(email);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var value = ok.Value!;
            
            Assert.Equal("Jane", GetProp<string>(value, "FirstName"));
            Assert.Equal("Doe", GetProp<string>(value, "LastName"));
            Assert.Equal(email, GetProp<string>(value, "Email"));
            Assert.Equal(email, GetProp<string>(value, "UserName"));
            
            _userServiceMock.VerifyAll();
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnBadRequest_WhenServiceFails()
        {
            // Arrange
            var email = "missing@example.com";
            var error = new CustomError("USR_404", "User not found");

            _userServiceMock
                .Setup(s => s.GetByEmailAsync(email))
                .ReturnsAsync(CustomResult<User>.Failure(error));

            // Act
            var result = await _controller.GetByEmailAsync(email);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(error, bad.Value);
            _userServiceMock.VerifyAll();
        }

        // GET /api/v1/user/autocomplete?userName=...
        [Fact]
        public async Task GetByUserNameAsync_ShouldReturnOk_WhenServiceSucceeds()
        {
            // Arrange
            var query = "jo";
            var suggestions = new List<string> { "john.doe", "jose", "jordan" };

            _userServiceMock
                .Setup(s => s.GetByUserNameAsync(query))
                .ReturnsAsync(CustomResult<List<string>>.Success(suggestions));

            // Act
            var result = await _controller.GetByUserNameAsync(query);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(suggestions, ok.Value);
            _userServiceMock.VerifyAll();
        }

        [Fact]
        public async Task GetByUserNameAsync_ShouldReturnBadRequest_WhenServiceFails()
        {
            // Arrange
            var query = "xx";
            var error = new CustomError("USR_400", "Invalid query");

            _userServiceMock
                .Setup(s => s.GetByUserNameAsync(query))
                .ReturnsAsync(CustomResult<List<string>>.Failure(error));

            // Act
            var result = await _controller.GetByUserNameAsync(query);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(error, bad.Value);
            _userServiceMock.VerifyAll();
        }

        // POST /api/v1/user
        [Fact]
        public async Task CreateAsync_ShouldReturnCreated_WhenServiceSucceeds()
        {
            // Arrange
            var request = new UserRequest
            {
                FirstName = "New",
                LastName = "User",
                Email = "new.user@example.com",
                Password = "P@ssw0rd!"
            };

            var returnedUser = new User(request.FirstName, request.LastName, request.Email);

            _userServiceMock
                .Setup(s => s.CreateAsync(request.FirstName, request.LastName, request.Email, request.Password))
                .ReturnsAsync(CustomResult<User>.Success(returnedUser));

            // Act
            var result = await _controller.CreateAsync(request);

            // Assert
            var created = Assert.IsType<CreatedResult>(result);
            Assert.Equal("api/v1/user", created.Location);
            Assert.Equal(returnedUser.Id, created.Value);
            _userServiceMock.VerifyAll();
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnBadRequest_WhenServiceFails()
        {
            // Arrange
            var request = new UserRequest
            {
                FirstName = "New",
                LastName = "User",
                Email = "invalid-email",
                Password = "123"
            };
            var errors = new[]
                { new CustomError("VAL_001", "Invalid email"), new CustomError("VAL_002", "Weak password") };

            _userServiceMock
                .Setup(s => s.CreateAsync(request.FirstName, request.LastName, request.Email, request.Password))
                .ReturnsAsync(CustomResult<User>.Failure(errors));

            // Act
            var result = await _controller.CreateAsync(request);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Same(errors, bad.Value);
            _userServiceMock.VerifyAll();
        }

        // PUT /api/v1/user
        [Fact]
        public async Task UpdateAsync_ShouldReturnOk_WhenServiceSucceeds()
        {
            // Arrange
            var request = new UserRequest
            {
                FirstName = "Updated",
                LastName = "Name",
                Email = "updated@example.com",
                Password = "ignored"
            };

            var updated = new User(request.FirstName, request.LastName, request.Email);

            _userServiceMock
                .Setup(s => s.UpdateAsync(updated.Id, request.FirstName, request.LastName, request.Email))
                .ReturnsAsync(CustomResult<User>.Success(updated));

            // Act
            var result = await _controller.UpdateAsync(updated.Id, request);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(updated.Id, ok.Value);
            _userServiceMock.VerifyAll();
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnBadRequest_WhenServiceFails()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UserRequest
            {
                FirstName = "Updated",
                LastName = "Name",
                Email = "bad-email",
                Password = "Passw0rd!"
            };

            var errors = new[] { new CustomError("VAL_001", "Invalid email") };

            _userServiceMock
                .Setup(s => s.UpdateAsync(id, request.FirstName, request.LastName, request.Email))
                .ReturnsAsync(CustomResult<User>.Failure(errors));

            // Act
            var result = await _controller.UpdateAsync(id, request);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Same(errors, bad.Value);
            _userServiceMock.VerifyAll();
        }

        // DELETE /api/v1/user
        [Fact]
        public async Task DeleteAsync_ShouldReturnOk_WhenServiceSucceeds()
        {
            // Arrange
            const string email = "john.doe@example.com";

            var deleted = new User("John", "Doe", email);

            _userServiceMock
                .Setup(s => s.DeleteAsync(email))
                .ReturnsAsync(CustomResult<User>.Success(deleted));

            // Act
            var result = await _controller.DeleteAsync(email);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(deleted.Id, ok.Value);
            _userServiceMock.VerifyAll();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnBadRequest_WhenServiceFails()
        {
            // Arrange
            var email = "missing@example.com";
            var errors = new[] { new CustomError("USR_404", "User not found") };

            _userServiceMock
                .Setup(s => s.DeleteAsync(email))
                .ReturnsAsync(CustomResult<User>.Failure(errors));

            // Act
            var result = await _controller.DeleteAsync(email);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Same(errors, bad.Value);
            _userServiceMock.VerifyAll();
        }

        // POST /api/v1/user/login
        [Fact]
        public async Task LoginAsync_ShouldReturnOk_WhenServiceSucceeds()
        {
            // Arrange
            const string token = "jwt-token-abc";

            var request = new LoginRequest { UserName = "john.doe@example.com", Password = "P@ssw0rd!" };

            _userServiceMock
                .Setup(s => s.LoginAsync(request.UserName, request.Password))
                .ReturnsAsync(CustomResult<string>.Success(token));

            // Act
            var result = await _controller.LoginAsync(request);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(token, ok.Value);
            _userServiceMock.VerifyAll();
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnUnauthorized_WhenServiceFails()
        {
            // Arrange
            var request = new LoginRequest { UserName = "john.doe@example.com", Password = "bad" };
            var error = new CustomError("AUTH_401", "Invalid credentials");

            _userServiceMock
                .Setup(s => s.LoginAsync(request.UserName, request.Password))
                .ReturnsAsync(CustomResult<string>.Failure(error));

            // Act
            var result = await _controller.LoginAsync(request);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(error, unauthorized.Value);
            _userServiceMock.VerifyAll();
        }

        private static T GetProp<T>(object obj, string name)
        {
            var prop = obj.GetType().GetProperty(name);
            Assert.NotNull(prop);
            return (T)prop!.GetValue(obj)!;
        }
    }
}