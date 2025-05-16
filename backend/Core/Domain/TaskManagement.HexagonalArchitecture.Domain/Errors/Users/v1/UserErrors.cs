using TaskManagement.HexagonalArchitecture.Domain.Abstractions;

namespace TaskManagement.HexagonalArchitecture.Domain.Errors.Users.v1;

public static class UserErrors
{
    public static readonly Error SameUser = new("Followers.SameUser", "Can't follow yourself");
}