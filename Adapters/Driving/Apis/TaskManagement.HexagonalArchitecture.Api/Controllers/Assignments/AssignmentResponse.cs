namespace TaskManagement.HexagonalArchitecture.Api.Controllers.Assignments
{
    public class AssignmentResponse(Guid id)
    {
        public Guid Id { get; set; } = id;
    }
}
