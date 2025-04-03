namespace TaskManagement.HexagonalArchitecture.Api.Controllers.Users
{
    public class UserResponse(Guid id)
    {
        public Guid Id { get; set; } = id;
    }
}
