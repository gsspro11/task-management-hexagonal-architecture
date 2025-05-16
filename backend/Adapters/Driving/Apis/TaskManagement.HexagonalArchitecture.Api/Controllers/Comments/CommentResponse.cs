namespace TaskManagement.HexagonalArchitecture.Api.Controllers.Comments
{
    public class CommentResponse(Guid id)
    {
        public Guid Id { get; set; } = id;
    }
}
