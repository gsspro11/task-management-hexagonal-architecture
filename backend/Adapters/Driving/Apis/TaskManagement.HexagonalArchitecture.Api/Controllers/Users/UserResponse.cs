namespace TaskManagement.HexagonalArchitecture.Api.Controllers.Users;

public record UserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string? Email,
    string? UserName,
    DateTime CreatedDate,
    DateTime? UpdatedDate);