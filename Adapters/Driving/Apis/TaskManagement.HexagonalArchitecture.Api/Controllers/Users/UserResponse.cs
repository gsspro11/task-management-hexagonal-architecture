namespace TaskManagement.HexagonalArchitecture.Api.Controllers.Users
{
    public class UserResponse
    {
        public UserResponse(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}
