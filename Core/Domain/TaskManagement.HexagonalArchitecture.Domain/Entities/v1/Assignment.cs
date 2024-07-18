using TaskManagement.HexagonalArchitecture.Domain.Enums;

namespace TaskManagement.HexagonalArchitecture.Domain.Entities.v1
{
    public class Assignment
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime DueDate { get; private set; }
        public int Priority { get; private set; }
        public AssignmentStatus Status { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime? UpdatedDate { get; private set; }

        public List<User> Users { get; private set; }
        public List<Comment> Comments { get; private set; }
    }
}